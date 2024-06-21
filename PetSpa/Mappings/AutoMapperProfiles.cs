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
            CreateMap<UpdateServiceRequestDTO, Service>().ReverseMap();
            CreateMap<UpdateComboRequestDTO, Combo>().ReverseMap();
            CreateMap<AddBookingRequestDTO, Booking>().ReverseMap();
            CreateMap<Invoice, InvoiceDTO>().ReverseMap();
            CreateMap<AddBookingDetailRequestDTO,BookingDetail>().ReverseMap();
            CreateMap<AddPetRequestDTO, Pet>().ReverseMap();
            CreateMap<UpdatePetRequestDTO, Pet>().ReverseMap();
            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Bookings, opt => opt.MapFrom(src => src.Bookings))
                .ForMember(dest => dest.Pets, opt => opt.MapFrom(src => src.Pets))
                .ForMember(dest => dest.Vouchers, opt => opt.MapFrom(src => src.Vouchers));
            CreateMap<UpdateCustomerRequestDTO, Customer>().ReverseMap();
            CreateMap<UpdateCustomerRequestByAdminDTO, Customer>().ReverseMap();
            CreateMap<AddAdminRequestDTO, Admin>().ReverseMap();
            CreateMap<Admin, AdminDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<AddAdminRequestDTO, Admin>().ReverseMap();
            CreateMap<Pet, PetDTO>().ReverseMap();
            CreateMap<AddPetRequestDTO, Pet>().ReverseMap();
            CreateMap<UpdatePetRequestDTO, Pet>().ReverseMap();
            CreateMap<Combo, ComboDTO>()
                            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration.ToString())); // Chuyển đổi TimeSpan thành chuỗi

            CreateMap<AddComboRequestDTO, Combo>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.Parse(src.Duration)));

            CreateMap<UpdateComboRequestDTO, Combo>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.Parse(src.Duration)));

            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration.ToString())); // Chuyển đổi TimeSpan thành chuỗi

            CreateMap<AddServiceRequest, Service>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.Parse(src.Duration)))
                .ForMember(dest => dest.ComboId, opt => opt.MapFrom(src => src.ComboId));

            CreateMap<UpdateServiceRequestDTO, Service>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.Parse(src.Duration)))
                .ForMember(dest => dest.ComboId, opt => opt.MapFrom(src => src.ComboId));

            CreateMap<AddBookingRequestDTO, Booking>()
           .ForMember(dest => dest.BookingId, opt => opt.Ignore())
           .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails)).ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore());
            CreateMap<Booking, BookingDTO>()
             .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingDetails)).
             ForMember(dest => dest.Cus, opt => opt.MapFrom(src => src.Customer.FullName)) // Ánh xạ tên khách hàng
            .ForMember(dest => dest.Cus, opt => opt.MapFrom(src => src.Customer))
            .ForMember(dest => dest.CusId, opt => opt.MapFrom(src => src.Customer.CusId));

            CreateMap<AddBookingDetailRequestDTO, BookingDetail>()
           .ForMember(dest => dest.BookingDetailId, opt => opt.Ignore())
           .ForMember(dest => dest.Duration, opt => opt.Ignore());

            CreateMap<BookingDetail, BookingDetailDTO>().ReverseMap();

            CreateMap<UpdateBookingRequestDTO, Booking>().ReverseMap();
            CreateMap<UpdateBookingDetailDTO, BookingDetail>().ReverseMap();
            CreateMap<CompleteBookingRequestDTO, Booking>().ReverseMap();
        }
    }
}
