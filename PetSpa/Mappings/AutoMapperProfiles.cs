using AutoMapper;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Account;
using PetSpa.Models.DTO.Booking;
using PetSpa.Models.DTO.BookingDetail;
using PetSpa.Models.DTO.Combo;
using PetSpa.Models.DTO.Customer;
using PetSpa.Models.DTO.Invoice;
using PetSpa.Models.DTO.Job;
using PetSpa.Models.DTO.Manager;
using PetSpa.Models.DTO.Service;
using PetSpa.Models.DTO.Staff;

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
            CreateMap<UpdateManagerRequestDTO, Manager>().ReverseMap();
            CreateMap<Job, JobDTO>().ReverseMap();
            CreateMap<AddJobRequestDTO, Job>().ReverseMap();
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
        }
    }
}
