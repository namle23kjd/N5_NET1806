using AutoMapper;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Admin;
using PetSpa.Models.DTO.Booking;
using PetSpa.Models.DTO.BookingDetail;
using PetSpa.Models.DTO.Combo;
using PetSpa.Models.DTO.Customer;
using PetSpa.Models.DTO.Invoice;
using PetSpa.Models.DTO.Manager;
using PetSpa.Models.DTO.Pet;
using PetSpa.Models.DTO.Service;
using PetSpa.Models.DTO.Staff;

namespace PetSpa.Mappings
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Staff, StaffDTO>().ReverseMap();
            CreateMap<UpdateStaffRequestDTO, Staff>().ReverseMap();
            CreateMap<Manager, ManagerDTO>().ReverseMap();
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
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<Invoice, InvoiceDTO>().ReverseMap();
            CreateMap<UpdateBookingRequestDTO, Booking>().ReverseMap();
            CreateMap<AddBookingDetailRequestDTO,BookingDetail>().ReverseMap();
            CreateMap<Pet, PetDTO>().ReverseMap();
            CreateMap<UpdateBookingDetailDTO, BookingDetail>().ReverseMap();
            CreateMap<Admin, AdminDTO>().ReverseMap();
        }
    }
}
