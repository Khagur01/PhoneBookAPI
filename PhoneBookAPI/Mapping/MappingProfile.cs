using AutoMapper;
using PhonebookApi.Dtos;
using PhoneBookAPI.Data;

namespace PhonebookApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Person, PersonDto>().ReverseMap();
            CreateMap<CreatePersonDto, Person>();
            CreateMap<UpdatePersonDto, Person>();
        }
    }
}