using Microsoft.EntityFrameworkCore;
using PhonebookApi.Data;
using PhonebookApi.Models;
using PhonebookApi.Repositories.Interfaces;

namespace PhonebookApi.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;

        public PersonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Person>> GetAllAsync()
        {
            return await _context.People.ToListAsync();
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _context.People.FindAsync(id);
        }

        public async Task AddAsync(Person person)
        {
            await _context.People.AddAsync(person);
        }

        public void Update(Person person)
        {
            _context.People.Update(person);
        }

        public void Delete(Person person)
        {
            _context.People.Remove(person);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
