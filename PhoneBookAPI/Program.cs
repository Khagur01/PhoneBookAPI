using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PhonebookApi.Data;
using PhonebookApi.Mapping;
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

// HTTP pipeline
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();
