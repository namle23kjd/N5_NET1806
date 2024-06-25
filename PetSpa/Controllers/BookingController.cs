using AutoMapper;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Helper;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
using PetSpa.Repositories.BookingRepository;
using PetSpa.Repositories.ServiceRepository;
using System;
using System.Diagnostics;
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
                return BadRequest("Ngày đặt lịch không được trễ hơn ngày hiện tại. Vui lòng chọn ngày khác.");
            }

            var isScheduleTaken = await bookingRepository.IsScheduleTakenAsync(addBookingRequestDTO.BookingSchedule);
            if (isScheduleTaken)
            {
                return BadRequest("Thời gian này đã được đặt. Vui lòng chọn lịch khác.");
            }

            var managerWithLeastBookings = await bookingRepository.GetManagerWithLeastBookingsAsync();
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
            bookingDomainModels.TotalAmount = bookingDomainModels.BookingDetails.Sum(bd =>
            {
                var comboPrice = bd.ComboId.HasValue ? petSpaContext.Combos.FirstOrDefault(c => c.ComboId == bd.ComboId)?.Price ?? 0 : 0;
                var servicePrice = bd.ServiceId.HasValue ? petSpaContext.Services.FirstOrDefault(s => s.ServiceId == bd.ServiceId)?.Price ?? 0 : 0;
                return comboPrice + servicePrice;
            });
            bookingDomainModels.Status = BookingStatus.NotStarted;
            bookingDomainModels.PaymentStatus = false;

            // Optimize price calculation
            var comboIds = bookingDomainModels.BookingDetails
                .Where(bd => bd.ComboId.HasValue)
                .Select(bd => bd.ComboId.Value)
                .ToList();

            var serviceIds = bookingDomainModels.BookingDetails
                .Where(bd => bd.ServiceId.HasValue)
                .Select(bd => bd.ServiceId.Value)
                .ToList();

            var combos = await petSpaContext.Combos
                .Where(c => comboIds.Contains(c.ComboId))
                .ToDictionaryAsync(c => c.ComboId, c => c.Price);

            var services = await petSpaContext.Services
                .Where(s => serviceIds.Contains(s.ServiceId))
                .ToDictionaryAsync(s => s.ServiceId, s => s.Price);


            foreach (var detail in bookingDomainModels.BookingDetails)
            {
                detail.BookingDetailId = Guid.NewGuid();
                detail.BookingId = bookingDomainModels.BookingId;
                if (detail.StaffId == Guid.Empty) detail.StaffId = null;
                if (detail.ComboId == Guid.Empty) detail.ComboId = null;
                if (detail.ServiceId == Guid.Empty) detail.ServiceId = null;
            }

            await bookingRepository.CreateAsync(bookingDomainModels);

            // Truy xuất tên khách hàng từ CusId
            var customer = await petSpaContext.Customers.FirstOrDefaultAsync(c => c.CusId == addBookingRequestDTO.CusId);
            if (customer != null)
            {
                bookingDomainModels.Customer = customer;
            }
            var bookingDTO = mapper.Map<BookingDTO>(bookingDomainModels);
            bookingDTO.CustomerName = customer?.FullName;
            return Ok(apiResponseService.CreateSuccessResponse(bookingDTO, "Booking created successfully"));
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
            try
            {
                var bookingDomainModels = await bookingRepository.GetAllAsync();
                var bookingDTOs = mapper.Map<List<BookingDTO>>(bookingDomainModels);
                return Ok(apiResponseService.CreateSuccessResponse(bookingDTOs, "Bookings retrieved successfully"));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting all bookings.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while getting all bookings"));
            }
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
        [HttpPut("update-time/{bookingId:Guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateBookingTime([FromRoute] Guid bookingId, [FromBody] UpdateBookingTimeRequest updateBookingTimeRequest)
        {
            if (updateBookingTimeRequest.NewDateTime < DateTime.Now)
            {
                return BadRequest("New scheduled date cannot be in the past.");
            }

            var booking = await bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Check if 24 hours have passed since the booking was created
           

            booking.StartDate = updateBookingTimeRequest.NewDateTime;

            var updatedBooking = await bookingRepository.UpdateAsync(bookingId, booking);
            if (updatedBooking == null)
            {
                return BadRequest("Failed to update booking.");
            }

            return Ok(responseService.CreateSuccessResponse(mapper.Map<BookingDTO>(updatedBooking)));
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
            try
            {
                var completedBookings = await bookingRepository.GetCompletedBookingsAsync();
                var completedBookingDTOs = mapper.Map<List<BookingDTO>>(completedBookings);
                return Ok(apiResponseService.CreateSuccessResponse(completedBookingDTOs, "Completed bookings retrieved successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting completed bookings.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while getting completed bookings"));
            }
        }

        // Danh sách booking chưa làm
        [HttpGet("not-started")]
        public async Task<IActionResult> GetNotStartedBookings()
        {
            try
            {
                var bookings = await bookingRepository.GetBookingsByStatusAsync(BookingStatus.NotStarted);
                return Ok(apiResponseService.CreateSuccessResponse(mapper.Map<List<BookingDTO>>(bookings), "Not started bookings retrieved successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting not started bookings.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while getting not started bookings"));
            }
        }

        // Danh sách booking đang làm
        [HttpGet("in-progress")]
        public async Task<IActionResult> GetInProgressBookings()
        {
            try
            {
                var bookings = await bookingRepository.GetBookingsByStatusAsync(BookingStatus.InProgress);
                return Ok(apiResponseService.CreateSuccessResponse(mapper.Map<List<BookingDTO>>(bookings), "In progress bookings retrieved successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting in progress bookings.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while getting in progress bookings"));
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingBookings()
        {
            try
            {
                var bookings = await bookingRepository.GetBookingsByApprovalStatusAsync(false);
                return Ok(apiResponseService.CreateSuccessResponse(mapper.Map<List<BookingDTO>>(bookings), "Pending bookings retrieved successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting pending bookings.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while getting pending bookings"));
            }
        }

        // Danh sách booking đã duyệt thành công
        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedBookings()
        {
            try
            {
                var bookings = await bookingRepository.GetBookingsByApprovalStatusAsync(true);
                return Ok(apiResponseService.CreateSuccessResponse(mapper.Map<List<BookingDTO>>(bookings), "Approved bookings retrieved successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting approved bookings.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while getting approved bookings"));
            }
        }

        [HttpPut("assign-staff")]
        public async Task<IActionResult> AssignStaff([FromBody] AssignStaffDTO assignStaffDTO)
        {
            try
            {
                var booking = await bookingRepository.GetByIdAsync(assignStaffDTO.BookingId);
                if (booking == null)
                {
                    return NotFound(apiResponseService.CreateErrorResponse("Booking not found"));
                }

                var staff = await petSpaContext.Staff.FindAsync(assignStaffDTO.StaffId);
                if (staff == null)
                {
                    return NotFound(apiResponseService.CreateErrorResponse("Staff not found"));
                }

                // Check if the staff is available during the booking time
                var isStaffAvailable = bookingRepository.GetAvailableStaffsForStartTime(booking.StartDate, booking.EndDate, assignStaffDTO.StaffId).Any();
                if (!isStaffAvailable)
                {
                    return BadRequest(apiResponseService.CreateErrorResponse("Staff is not available during the booking time"));
                }

                // Assign staff to booking
                foreach (var detail in booking.BookingDetails)
                {
                    detail.StaffId = assignStaffDTO.StaffId;
                }

                await bookingRepository.UpdateAsync(booking.BookingId, booking);
                return Ok(apiResponseService.CreateSuccessResponse("Staff assigned successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while assigning staff.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while assigning staff"));
            }
        }

        // Accept booking
        [HttpPut("accept-booking")]
        public async Task<IActionResult> AcceptBooking([FromBody] AcceptBookingDTO acceptBookingDTO)
        {
            try
            {
                var booking = await bookingRepository.GetByIdAsync(acceptBookingDTO.BookingId);
                if (booking == null)
                {
                    return NotFound(apiResponseService.CreateErrorResponse("Booking not found"));
                }

                // Automatically assign a staff if not already assigned
                if (!booking.BookingDetails.Any(detail => detail.StaffId.HasValue))
                {
                    var availableStaffs = bookingRepository.GetAvailableStaffsForStartTime(booking.StartDate, booking.EndDate);
                    if (!availableStaffs.Any())
                    {
                        return BadRequest(apiResponseService.CreateErrorResponse("No available staff found for the booking time"));
                    }

                    var assignedStaff = availableStaffs.First();
                    foreach (var detail in booking.BookingDetails)
                    {
                        detail.StaffId = assignedStaff.StaffId;
                    }
                }

                booking.CheckAccept = true;
                await bookingRepository.UpdateAsync(booking.BookingId, booking);

                return Ok(apiResponseService.CreateSuccessResponse("Booking accepted successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while accepting booking.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while accepting booking"));
            }
        }

        [HttpGet("total-revenue/current-month")]
        public async Task<IActionResult> GetTotalRevenueForCurrentMonth([FromQuery] DateTime? startDate)
        {
            var totalAmount = await bookingRepository.GetAllToTalForMonthAsync(startDate);
            var dailyRevenues = await bookingRepository.GetDailyRevenueForCurrentMonthAsync(startDate);
            return Ok(new { TotalAmount = totalAmount, DailyRevenues = dailyRevenues });
        }

        [HttpGet("total-revenue/from-start")]
        public async Task<IActionResult> GetTotalRevenueFromStart()
        {
            var totalAmount = await bookingRepository.GetAllToTalAsync();
            return Ok(new { TotalAmount = totalAmount });
        }
    }
}
