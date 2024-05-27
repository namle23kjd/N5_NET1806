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
            CreateMap<Staff, StaffDTO>().ReverseMap();
            CreateMap<AddStaffRequestDTO, Staff>().ReverseMap();
            CreateMap<UpdateStaffRequestDTO, Staff>().ReverseMap();
            CreateMap<Manager, ManagerDTO>().ReverseMap();
            CreateMap<AddManagerRequestDTO, Manager>().ReverseMap();
            CreateMap<UpdateManagerRequestDTO, Manager>().ReverseMap();
        }
    }
}
