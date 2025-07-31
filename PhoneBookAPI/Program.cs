using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PhonebookApi.Data;
using PhonebookApi.Mapping;
using PhonebookApi.Middleware; // Error handling middleware için
using PhonebookApi.Repositories;
using PhonebookApi.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Entity Framework ile Retry Policy
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    ));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation ile Controllers
builder.Services.AddControllers()
    .AddFluentValidation(config =>
    {
        config.RegisterValidatorsFromAssemblyContaining<Program>();
        config.AutomaticValidationEnabled = true;
    });

// Repository Pattern
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Veritabanýný otomatik oluþtur
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Veritabanýný oluþtur (eðer yoksa)
        context.Database.EnsureCreated();
        Console.WriteLine("Database successfully created/updated!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database creation failed: {ex.Message}");
        // Uygulama çalýþmaya devam etsin
    }
}

// HTTP Pipeline - ÖNEMLÝ: Sýralama çok önemli!

// 1. Error handling middleware (EN ÜSTTE OLMALI)
app.UseErrorHandling();

// 2. Development environment için detaylý hata sayfasý
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Bu artýk middleware tarafýndan handle ediliyor ama yine de ekleyelim
}

// 3. HTTPS redirection
app.UseHttpsRedirection();

// 4. Swagger (sadece development'ta)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Phonebook API V1");
        c.RoutePrefix = string.Empty; // Swagger'ý root'ta açar (https://localhost:xxxx/)
    });
}

// 5. Authentication & Authorization (gelecekte eklenebilir)
app.UseAuthentication(); // Þimdilik boþ ama yapý hazýr
app.UseAuthorization();

// 6. Controllers
app.MapControllers();

// 7. Fallback route (opsiyonel)
app.MapFallback(async context =>
{
    context.Response.StatusCode = 404;
    await context.Response.WriteAsync("Endpoint not found");
});

app.Run();
