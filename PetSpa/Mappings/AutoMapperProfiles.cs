using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Admin;
using PetSpa.Models.DTO.Booking;
using PetSpa.Models.DTO.BookingDetail;
using PetSpa.Models.DTO.Combo;
using PetSpa.Models.DTO.Customer;
using PetSpa.Models.DTO.Invoice;
using PetSpa.Models.DTO.Manager;
using PetSpa.Models.DTO.Pet;
using PetSpa.Models.DTO.RegisterDTO;
using PetSpa.Models.DTO.Service;
using PetSpa.Models.DTO.Staff;

namespace PetSpa.Mappings
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ApplicationUser, IdentityUser>().ReverseMap();
            CreateMap<Staff, StaffDTO>()
                           .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                           .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails))
                           .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.Manager));
            CreateMap<UpdateStaffRequestDTO, Staff>().ReverseMap();
            CreateMap<AddStaffRequestDTO, Staff>().ReverseMap();
            CreateMap<AddRequestManagerDTO, Manager>().ReverseMap();
            CreateMap<Manager, ManagerDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Admins, opt => opt.MapFrom(src => src.Admins))
                .ForMember(dest => dest.Staffs, opt => opt.MapFrom(src => src.Staffs))
                .ForMember(dest => dest.Vouchers, opt => opt.MapFrom(src => src.Vouchers))
                .ForMember(dest => dest.Bookings, opt => opt.MapFrom(src => src.Bookings));
            CreateMap<UpdateManagerRequestDTO, Manager>().ReverseMap();
            CreateMap<AddServiceRequest, Service>().ReverseMap();
            CreateMap<Service, ServiceDTO>().ReverseMap();
            CreateMap<Combo, ComboDTO>().ReverseMap();
            CreateMap<UpdateServiceRequestDTO, Service>().ReverseMap();
            CreateMap<AddComboRequestDTO, Combo>().ReverseMap();
            CreateMap<Combo, ComboDTO>().ReverseMap();
            CreateMap<BookingDetail, BookingDetailDTO>().ReverseMap();
            CreateMap<UpdateComboRequestDTO, Combo>().ReverseMap();
            CreateMap<AddBookingRequestDTO, Booking>().ReverseMap();
            CreateMap<Booking, BookingDTO>().ReverseMap();
            CreateMap<Invoice, InvoiceDTO>().ReverseMap();
            CreateMap<UpdateBookingRequestDTO, Booking>().ReverseMap();
            CreateMap<AddBookingDetailRequestDTO,BookingDetail>().ReverseMap();
            CreateMap<UpdateBookingDetailDTO, BookingDetail>().ReverseMap();
            CreateMap<AddPetRequestDTO, Pet>().ReverseMap();
            CreateMap<UpdatePetRequestDTO, Pet>().ReverseMap();
            CreateMap<AddCusRequestDTO, Customer>().ReverseMap();
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<Pet, PetDTO>().ReverseMap();
            CreateMap<AddAdminRequestDTO, Admin>().ReverseMap();
            CreateMap<Admin, AdminDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<AddAdminRequestDTO, Admin>().ReverseMap();
        }
    }
}
