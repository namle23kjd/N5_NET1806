using AutoMapper;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.CustomerDTO;
using PetSpa.Models.DTO.PetDTO;
using PetSpa.Models.DTO.RegisterDTO;



namespace PetSpa.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Pet, PetDTO>().ReverseMap();
            CreateMap<AddPetRequestDTO, Pet>().ReverseMap();
            CreateMap<UpdatePetRequestDTO, Pet>().ReverseMap();
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<AddCusRequestDTO, Customer>().ReverseMap();
            CreateMap<RegisterPequestDto, Account>().ReverseMap();



        }
    }
}
