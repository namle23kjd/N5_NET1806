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
using PetSpa.Models.DTO.PaymentDTO;
using PetSpa.Models.DTO.Pet;
using PetSpa.Models.DTO.RegisterDTO;
using PetSpa.Models.DTO.Service;
using PetSpa.Models.DTO.Staff;
using PetSpa.Models.DTO.UserDTO;

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
                .ForMember(dest => dest.Pets, opt => opt.MapFrom(src => src.Pets));
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
            .ForMember(dest => dest.CusId, opt => opt.MapFrom(src => src.Customer.CusId))
            .ForMember(dest => dest.ServiceId , opt => opt.MapFrom(src => src.BookingDetails.FirstOrDefault().ServiceId))
            .ForMember(dest => dest.ComboId, opt => opt.MapFrom(src => src.BookingDetails.FirstOrDefault().ComboId))
            .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.BookingDetails.FirstOrDefault().StaffId));

            CreateMap<AddBookingDetailRequestDTO, BookingDetail>()
           .ForMember(dest => dest.BookingDetailId, opt => opt.Ignore())
           .ForMember(dest => dest.Duration, opt => opt.Ignore()).
           ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId.HasValue ? src.StaffId : (Guid?)null));

            CreateMap<BookingDetail, BookingDetailDTO>().ReverseMap();
            CreateMap<UpdateBookingRequestDTO, Booking>().ReverseMap();
            CreateMap<UpdateBookingDetailDTO, BookingDetail>().ReverseMap();
            CreateMap<CompleteBookingRequestDTO, Booking>().ReverseMap();

            CreateMap<ApplicationUser, UserDTO>()
           .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName ?? string.Empty))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? string.Empty))
           .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<Payment, PaymentDTO>()
            .ForMember(dest => dest.CusId, opt => opt.MapFrom(src => src.CusId))
            .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.ExpirationTime, opt => opt.MapFrom(src => src.ExpirationTime))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName));

            CreateMap<PaymentDTO, Payment>();

            CreateMap<Payment, PaymentHistoryDTO>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.ExpirationTime, opt => opt.MapFrom(src => src.ExpirationTime))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Bookings.Sum(b => b.TotalAmount ?? 0)))
            .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.Bookings.SelectMany(b => b.BookingDetails)));

            CreateMap<BookingDetail, BookingDetailHistoryDTO>()
            .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
            .ForMember(dest => dest.PetId, opt => opt.MapFrom(src => src.PetId))
            .ForMember(dest => dest.ScheduleDate, opt => opt.MapFrom(src => src.Booking.StartDate))
            .ForMember(dest => dest.ComboId, opt => opt.MapFrom(src => src.ComboId))
            .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
            .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
            .ForMember(dest => dest.ServicePrice, opt => opt.MapFrom(src => src.ServiceId.HasValue ? src.Service.Price : src.Combo.Price))
            .ForMember(dest => dest.CheckAccept, opt => opt.MapFrom(src => src.Booking.CheckAccept))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Booking.Status))
            .ForMember(dest => dest.Feedback, opt => opt.MapFrom(src => src.Booking.Feedback))
            .ForMember(dest => dest.BookingSchedule, opt => opt.MapFrom(src => src.Booking.BookingSchedule));
        }
    }
    
}
