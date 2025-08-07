using PhoneBookAPI.Data;

namespace PhonebookApi.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        Task<List<Person>> GetAllAsync();
        Task<Person?> GetByIdAsync(int id);
        Task AddAsync(Person person);
        void Update(Person person);
        void Delete(Person person);
        Task<bool> SaveChangesAsync();
    }
}
