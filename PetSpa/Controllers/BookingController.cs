using AutoMapper;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
using PetSpa.Repositories.BookingRepository;
using PetSpa.Repositories.ServiceRepository;
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

        public BookingController(IMapper mapper, ApiResponseService apiResponseService, IBookingRepository bookingRepository, PetSpaContext petSpaContext, ApiResponseService responseService)
        {
            this.mapper = mapper;
            this.apiResponseService = apiResponseService;
            this.bookingRepository = bookingRepository;
            this.petSpaContext = petSpaContext;
            this.responseService = responseService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AddBookingRequestDTO addBookingRequestDTO)
        {
            if (addBookingRequestDTO.BookingSchedule < DateTime.Now)
            {
                return BadRequest("Scheduled date cannot be later than the current date. Please choose another date.");
            }

            //var isScheduleTaken = await bookingRepository.IsScheduleTakenAsync(addBookingRequestDTO.BookingSchedule);
            //if (isScheduleTaken)
            //{
            //    return BadRequest("Thời gian này đã được đặt. Vui lòng chọn lịch khác.");
            //}


            var managerWithLeastBookings = await bookingRepository.GetManagerWithLeastBookingsAsync();
            if (managerWithLeastBookings == null)
            {
                return BadRequest("Not found any manager");
            }

            var bookingDomainModels = mapper.Map<Booking>(addBookingRequestDTO);
            bookingDomainModels.ManaId = managerWithLeastBookings.ManaId;
            bookingDomainModels.StartDate = addBookingRequestDTO.BookingSchedule;
            var totalDuration = bookingDomainModels.BookingDetails.Sum(bd => bd.Duration.Ticks);
            var totalDurationTimeSpan = new TimeSpan(totalDuration) + TimeSpan.FromMinutes(20);
            bookingDomainModels.EndDate = bookingDomainModels.StartDate + totalDurationTimeSpan;
            bookingDomainModels.TotalAmount = bookingDomainModels.BookingDetails.Sum(bd => (bd.Combo?.Price ?? 0) + (bd.Service?.Price ?? 0));
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
            return Ok(mapper.Map<BookingDTO>(bookingDomainModels));
        }




        [HttpGet("available")]
        [Authorize]
        public async Task<IActionResult> GetAvailableStaffs([FromQuery] DateTime startTime, [FromQuery] Guid serviceCode, [FromQuery] Guid? staffId)
        {
            try
            {
                if (startTime < DateTime.Now)
                {
                    return BadRequest("Scheduled date cannot be earlier than the current date. Please choose another date.");
                }

                TimeSpan totalDurationTimeSpan;

                // Kiểm tra nếu serviceCode là từ Combo
                var combo = await petSpaContext.Combos
                    .Include(c => c.Services)
                    .FirstOrDefaultAsync(c => c.ComboId == serviceCode);

                if (combo != null)
                {
                    // Tính tổng thời gian từ các dịch vụ trong combo
                    var totalDurationTicks = combo.Services.Sum(service => service.Duration.Ticks);
                    totalDurationTimeSpan = new TimeSpan(totalDurationTicks) + TimeSpan.FromMinutes(20);
                }
                else
                {
                    // Fetch the service duration based on the provided service code
                    var service = await petSpaContext.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceCode);
                    if (service == null)
                    {
                        return NotFound("Service not found for the given service code.");
                    }

                    // Initialize total duration with the fetched service duration
                    totalDurationTimeSpan = service.Duration + TimeSpan.FromMinutes(20);
                }

                // Compute the end time based on the start time and total duration
                var endTime = startTime + totalDurationTimeSpan;

                // Define working hours
                var startOfDay = new DateTime(startTime.Year, startTime.Month, startTime.Day, 8, 0, 0);
                var endOfDay = new DateTime(endTime.Year, endTime.Month, endTime.Day, 20, 0, 0);

                if (startTime < startOfDay || endTime > endOfDay)
                {
                    return BadRequest("Bookings can only be made between 08:00 and 20:00.");
                }

                // Fetch available staff based on the calculated start and end times
                List<Staff> availableStaffs;

                if (staffId.HasValue)
                {
                    // Check availability for the specific staff member
                    availableStaffs = bookingRepository.GetAvailableStaffsForStartTime(startTime, endTime, staffId.Value);
                }
                else
                {
                    // Check availability for all staff members
                    availableStaffs = bookingRepository.GetAvailableStaffsForStartTime(startTime, endTime);
                }

                if (availableStaffs == null || !availableStaffs.Any())
                {
                    return NotFound("No available staff found for the given time slot.");
                }

                return Ok(availableStaffs);
            }
            catch (InvalidOperationException ex)
            {
                // Handle specific invalid operation exception
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                return StatusCode(500, "An error occurred while processing your request: " + ex.Message);
            }
        }
        [HttpGet("availableForPeriod")]
        [Authorize]
        public async Task<IActionResult> GetAvailableStaffsForPeriod([FromQuery] DateTime startTime, [FromQuery] Guid serviceCode, [FromQuery] Guid? staffId, [FromQuery] int? periodMonths)
        {
            try
            {
                if (startTime < DateTime.Now)
                {
                    return BadRequest("Scheduled date cannot be earlier than the current date. Please choose another date.");
                }

                TimeSpan totalDurationTimeSpan;

                // Kiểm tra nếu serviceCode là từ Combo
                var combo = await petSpaContext.Combos
                    .Include(c => c.Services)
                    .FirstOrDefaultAsync(c => c.ComboId == serviceCode);

                if (combo != null)
                {
                    // Tính tổng thời gian từ các dịch vụ trong combo
                    var totalDurationTicks = combo.Services.Sum(service => service.Duration.Ticks);
                    totalDurationTimeSpan = new TimeSpan(totalDurationTicks) + TimeSpan.FromMinutes(20);
                }
                else
                {
                    // Fetch the service duration based on the provided service code
                    var service = await petSpaContext.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceCode);
                    if (service == null)
                    {
                        return NotFound("Service not found for the given service code.");
                    }

                    // Initialize total duration with the fetched service duration
                    totalDurationTimeSpan = service.Duration + TimeSpan.FromMinutes(20);
                }

                // Compute the end time based on the start time and total duration
                var endTime = startTime + totalDurationTimeSpan;

                // Define working hours
                var startOfDay = new DateTime(startTime.Year, startTime.Month, startTime.Day, 8, 0, 0);
                var endOfDay = new DateTime(endTime.Year, endTime.Month, endTime.Day, 20, 0, 0);

                if (startTime < startOfDay || endTime > endOfDay)
                {
                    return BadRequest("Bookings can only be made between 08:00 and 20:00.");
                }

                // Fetch available staff based on the calculated start and end times
                var (availableStaffs, errorMessage) = bookingRepository.GetAvailableStaffs(startTime, endTime, periodMonths, staffId);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                if (availableStaffs == null || !availableStaffs.Any())
                {
                    return NotFound("No available staff found for the given time slot.");
                }

                return Ok(availableStaffs);
            }
            catch (InvalidOperationException ex)
            {
                // Handle specific invalid operation exception
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                return StatusCode(500, "An error occurred while processing your request: " + ex.Message);
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
