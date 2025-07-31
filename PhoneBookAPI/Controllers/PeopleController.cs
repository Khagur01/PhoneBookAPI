using Microsoft.AspNetCore.Mvc;
using PhonebookApi.Models;
using PhonebookApi.Repositories.Interfaces;

namespace PhonebookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonRepository _personRepository;

        public PersonController(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        // GET: api/person
        [HttpGet]
        public async Task<ActionResult<List<Person>>> GetAllPersons()
        {
            try
            {
                var persons = await _personRepository.GetAllAsync();
                return Ok(persons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/person/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid person ID");
                }

                var person = await _personRepository.GetByIdAsync(id);

                if (person == null)
                {
                    return NotFound($"Person with ID {id} not found");
                }

                return Ok(person);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/person
        [HttpPost]
        public async Task<ActionResult<Person>> CreatePerson([FromBody] Person person)
        {
            try
            {
                if (person == null)
                {
                    return BadRequest("Person data is required");
                }

                if (string.IsNullOrWhiteSpace(person.FullName))
                {
                    return BadRequest("Full name is required");
                }

                if (string.IsNullOrWhiteSpace(person.PhoneNumber))
                {
                    return BadRequest("Phone number is required");
                }

                // ID'yi sıfırla çünkü veritabanı tarafından otomatik olarak atanacak
                person.Id = 0;

                await _personRepository.AddAsync(person);
                var saved = await _personRepository.SaveChangesAsync();

                if (!saved)
                {
                    return StatusCode(500, "Failed to save person to database");
                }

                return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/person/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] Person person)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid person ID");
                }

                if (person == null)
                {
                    return BadRequest("Person data is required");
                }

                if (id != person.Id)
                {
                    return BadRequest("ID mismatch between URL and request body");
                }

                if (string.IsNullOrWhiteSpace(person.FullName))
                {
                    return BadRequest("Full name is required");
                }

                if (string.IsNullOrWhiteSpace(person.PhoneNumber))
                {
                    return BadRequest("Phone number is required");
                }

                var existingPerson = await _personRepository.GetByIdAsync(id);
                if (existingPerson == null)
                {
                    return NotFound($"Person with ID {id} not found");
                }

                // Mevcut kişinin bilgilerini güncelle
                existingPerson.FullName = person.FullName;
                existingPerson.PhoneNumber = person.PhoneNumber;

                _personRepository.Update(existingPerson);
                var saved = await _personRepository.SaveChangesAsync();

                if (!saved)
                {
                    return StatusCode(500, "Failed to update person in database");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/person/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid person ID");
                }

                var person = await _personRepository.GetByIdAsync(id);
                if (person == null)
                {
                    return NotFound($"Person with ID {id} not found");
                }

                _personRepository.Delete(person);
                var saved = await _personRepository.SaveChangesAsync();

                if (!saved)
                {
                    return StatusCode(500, "Failed to delete person from database");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}