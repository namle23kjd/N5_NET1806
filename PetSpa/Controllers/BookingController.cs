using AutoMapper;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Helper;
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
        private readonly ApiResponseService responseService;
        private readonly ILogger<BookingController> logger;

        public BookingController(IMapper mapper, ApiResponseService apiResponseService, IBookingRepository bookingRepository, PetSpaContext petSpaContext, ApiResponseService responseService, ILogger<BookingController> logger)
        {
            this.mapper = mapper;
            this.apiResponseService = apiResponseService;
            this.bookingRepository = bookingRepository;
            this.petSpaContext = petSpaContext;
            this.responseService = responseService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddBookingRequestDTO addBookingRequestDTO)
        {
            if (addBookingRequestDTO.BookingSchedule < DateTime.Now)
            {
                return BadRequest("Scheduled date cannot be later than the current date. Please choose another date.");
            }
            var managerWithLeastBookings = await bookingRepository.GetManagerWithLeastBookingsAsync();
            if (managerWithLeastBookings == null)
            {
                return BadRequest("Not found any manager");
            }

            var bookingDomainModels = mapper.Map<Booking>(addBookingRequestDTO);
            bookingDomainModels.ManaId = managerWithLeastBookings.ManaId;
            bookingDomainModels.StartDate = addBookingRequestDTO.BookingSchedule;
            bookingDomainModels.PaymentStatus = false;
            //
            var totalDuration = bookingDomainModels.BookingDetails.Sum(bd => bd.Duration.Ticks);
            var totalDurationTimeSpan = new TimeSpan(totalDuration) + TimeSpan.FromMinutes(20);
            bookingDomainModels.EndDate = bookingDomainModels.StartDate + totalDurationTimeSpan;
            bookingDomainModels.TotalAmount = bookingDomainModels.BookingDetails.Sum(bd =>
            {
                var comboPrice = bd.ComboId.HasValue ? petSpaContext.Combos.FirstOrDefault(c => c.ComboId == bd.ComboId)?.Price ?? 0 : 0;
                var servicePrice = bd.ServiceId.HasValue ? petSpaContext.Services.FirstOrDefault(s => s.ServiceId == bd.ServiceId)?.Price ?? 0 : 0;
                return comboPrice + servicePrice;
            });
            var startOfDay = new DateTime(bookingDomainModels.StartDate.Year, bookingDomainModels.StartDate.Month, bookingDomainModels.StartDate.Day, 8, 0, 0);
            var endOfDay = new DateTime(bookingDomainModels.EndDate.Year, bookingDomainModels.EndDate.Month, bookingDomainModels.EndDate.Day, 20, 0, 0);

            if (bookingDomainModels.StartDate < startOfDay || bookingDomainModels.EndDate > endOfDay)
            {
                return BadRequest("Bookings can only be made between 08:00 and 20:00.");
            }
            var availableStaffs = bookingRepository.GetAvailableStaffsForStartTime(bookingDomainModels.StartDate, bookingDomainModels.EndDate);
            if (availableStaffs == null || !availableStaffs.Any())
            {
                return NotFound("No available staff found for the given time slot.");
            }


            foreach (var detail in bookingDomainModels.BookingDetails)
            {
                detail.BookingDetailId = Guid.NewGuid();
                detail.BookingId = bookingDomainModels.BookingId;
                if (detail.StaffId == Guid.Empty) detail.StaffId = null;
                if (detail.ComboId == Guid.Empty) detail.ComboId = null;
                if (detail.ServiceId == Guid.Empty) detail.ServiceId = null;
            }

            await bookingRepository.CreateAsync(bookingDomainModels);
            var customer = await petSpaContext.Customers.FirstOrDefaultAsync(c => c.CusId == addBookingRequestDTO.CusId);
            if (customer != null)
            {
                bookingDomainModels.Customer = customer;
            }
            return Ok(mapper.Map<BookingDTO>(bookingDomainModels));
        }
        [HttpGet("available")]
        public IActionResult GetAvailableStaffs([FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            try
            {
                var availableStaffs = bookingRepository.GetAvailableStaffsForStartTime(startTime, endTime);
                if (availableStaffs == null || !availableStaffs.Any())
                {
                    return NotFound("No available staff found for the given time slot.");
                }
                return Ok(availableStaffs);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
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

        [HttpPut("{BookingId:Guid}/accept")]
        public async Task<IActionResult> AcceptBooking([FromRoute] Guid BookingId, [FromBody] AcceptBookingRequest acceptRequest)
        {
            var booking = await bookingRepository.GetByIdAsync(BookingId);
            if (booking == null)
            {
                return NotFound("Booking not found");
            }

            booking.CheckAccept = acceptRequest.CheckAccept;
            await bookingRepository.UpdateAsync(BookingId, booking);

            return Ok(mapper.Map<BookingDTO>(booking));
        }



        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedBookings()
        {
            var completedBookings = await bookingRepository.GetCompletedBookingsAsync();
            return Ok(mapper.Map<List<BookingDTO>>(completedBookings));
        }
    }
}
