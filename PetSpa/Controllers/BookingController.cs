using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
using PetSpa.Repositories.BookingRepository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ApiResponseService apiResponseService;
        private readonly IBookingRepository bookingRepository;
        private readonly PetSpaContext petSpaContext;

        public BookingController(IMapper mapper, ApiResponseService apiResponseService, IBookingRepository bookingRepository, PetSpaContext petSpaContext)
        {
            this.mapper = mapper;
            this.apiResponseService = apiResponseService;
            this.bookingRepository = bookingRepository;
            this.petSpaContext = petSpaContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddBookingRequestDTO addBookingRequestDTO)
        {
            if (addBookingRequestDTO.BookingSchedule < DateTime.Now)
            {
                return BadRequest("Ngày đặt lịch không được trễ hơn ngày hiện tại. Vui lòng chọn ngày khác.");
            }

            var isScheduleTaken = await petSpaContext.Bookings.AnyAsync(b => b.BookingSchedule == addBookingRequestDTO.BookingSchedule && b.Status);
            if (isScheduleTaken)
            {
                return BadRequest("Thời gian này đã được đặt. Vui lòng chọn lịch khác.");
            }

            var managerWithLeastBookings = await petSpaContext.Managers
                .OrderBy(m => petSpaContext.Bookings.Count(b => b.ManaId == m.ManaId))
                .FirstOrDefaultAsync();

            if (managerWithLeastBookings == null)
            {
                return BadRequest("Không tìm thấy quản lý.");
            }

            var bookingDomainModels = mapper.Map<Booking>(addBookingRequestDTO);
            bookingDomainModels.ManaId = managerWithLeastBookings.ManaId;
            bookingDomainModels.StartDate = addBookingRequestDTO.BookingSchedule;
            var totalDuration = bookingDomainModels.BookingDetails.Sum(bd => bd.Duration.Ticks);
            var totalDurationTimeSpan = new TimeSpan(totalDuration) + TimeSpan.FromMinutes(20);
            bookingDomainModels.EndDate = bookingDomainModels.StartDate + totalDurationTimeSpan;
            bookingDomainModels.TotalAmount = bookingDomainModels.BookingDetails.Sum(bd => (bd.Combo?.Price ?? 0) + (bd.Service?.Price ?? 0));

            foreach (var detail in bookingDomainModels.BookingDetails)
            {
                detail.BookingDetailId = Guid.NewGuid();
                detail.BookingId = bookingDomainModels.BookingId;
                // Ensure staffId, comboId, and serviceId are properly set
                if (detail.StaffId == Guid.Empty)
                {
                    detail.StaffId = null;
                }
                if (detail.ComboId == Guid.Empty)
                {
                    detail.ComboId = null;
                }
                if (detail.ServiceId == Guid.Empty)
                {
                    detail.ServiceId = null;
                }
            }

            await bookingRepository.CreateAsync(bookingDomainModels);
            return Ok(mapper.Map<BookingDTO>(bookingDomainModels));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookingDomainModels = await bookingRepository.GetAllAsync();
            return Ok(mapper.Map<List<BookingDTO>>(bookingDomainModels));
        }

        [HttpGet]
        [Route("{BookingId:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid BookingId)
        {
            var bookingDomainModel = await bookingRepository.GetByIdAsync(BookingId);
            if (bookingDomainModel == null)
            {
                return apiResponseService.CreatePaymentNotFound();
            }
            return Ok(mapper.Map<BookingDTO>(bookingDomainModel));
        }

        [HttpPut]
        [Route("{BookingId:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid BookingId, UpdateBookingRequestDTO updateBookingRequestDTO)
        {
            var bookingDomainModel = mapper.Map<Booking>(updateBookingRequestDTO);
            var updatedBooking = await bookingRepository.UpdateAsync(BookingId, bookingDomainModel);

            if (updatedBooking == null)
            {
                return apiResponseService.CreatePaymentNotFound();
            }
            return Ok(mapper.Map<BookingDTO>(updatedBooking));
        }
    }
}
