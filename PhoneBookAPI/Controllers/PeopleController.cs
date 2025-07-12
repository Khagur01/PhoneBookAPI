using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhonebookApi.Data;
using PhonebookApi.Dtos;
using PhonebookApi.Models;

namespace PhonebookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PeopleController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PersonDto>> GetPeople()
        {
            var people = _context.People.ToList();
            return Ok(_mapper.Map<List<PersonDto>>(people));
        }

        [HttpGet("{id}")]
        public ActionResult<PersonDto> GetPerson(int id)
        {
            var person = _context.People.Find(id);
            if (person == null) return NotFound();
            return Ok(_mapper.Map<PersonDto>(person));
        }

        [HttpPost]
        public ActionResult<PersonDto> CreatePerson(CreatePersonDto createDto)
        {
            var person = _mapper.Map<Person>(createDto);
            _context.People.Add(person);
            _context.SaveChanges();

            var personDto = _mapper.Map<PersonDto>(person);
            return CreatedAtAction(nameof(GetPerson), new { id = personDto.Id }, personDto);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePerson(int id, UpdatePersonDto updateDto)
        {
            var person = _context.People.Find(id);
            if (person == null) return NotFound();

            _mapper.Map(updateDto, person);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePerson(int id)
        {
            var person = _context.People.Find(id);
            if (person == null) return NotFound();

            _context.People.Remove(person);
            _context.SaveChanges();
            return NoContent();
        }
    }
}