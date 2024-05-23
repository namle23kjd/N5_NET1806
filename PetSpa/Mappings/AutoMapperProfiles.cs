using AutoMapper;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO;

namespace PetSpa.Mappings
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<AddAccountRequestDTO, Account>().ReverseMap();
            CreateMap<UpdateAccountRequestDTO, Account>().ReverseMap();
        }
    }
}
