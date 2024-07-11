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
using PetSpa.Models.DTO.Staff;
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
    //[Authorize(Roles = "Admin,Customer,Manager")]
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
        //[Authorize(Roles = "Admin,Customer,Manager")]
        public async Task<IActionResult> Create([FromBody] AddBookingRequestDTO addBookingRequestDTO)
        {
            if (addBookingRequestDTO.BookingSchedule < DateTime.Now)
            {
                logger.LogWarning("Booking date cannot be later than the current date. BookingSchedule: {BookingSchedule}", addBookingRequestDTO.BookingSchedule);
                return BadRequest("Booking date cannot be later than the current date. Please select another date.");
            }

            var isScheduleTaken = await bookingRepository.IsScheduleTakenAsync(addBookingRequestDTO.BookingSchedule);
            if (isScheduleTaken)
            {
                logger.LogWarning("This time slot has already been booked. BookingSchedule: {BookingSchedule}", addBookingRequestDTO.BookingSchedule);
                return BadRequest("This time slot has already been booked. Please select another schedule.");
            }

            var managerWithLeastBookings = await bookingRepository.GetManagerWithLeastBookingsAsync();
            if (managerWithLeastBookings == null)
            {

                return BadRequest("Manager not found.");
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
            // Fetch the customer to apply discount based on CusRank
            var customer = await petSpaContext.Customers.FirstOrDefaultAsync(c => c.CusId == addBookingRequestDTO.CusId);
            if (customer != null)
            {
                if (customer.CusRank == "Silver")
                {
                    bookingDomainModels.TotalAmount = bookingDomainModels.TotalAmount * 0.95m; // Giảm 5%
                }
                else if (customer.CusRank == "Gold")
                {
                    bookingDomainModels.TotalAmount = bookingDomainModels.TotalAmount * 0.90m; // Giảm 10%
                }
                bookingDomainModels.Customer = customer;
            }
            bookingDomainModels.Status = BookingStatus.NotStarted;
            bookingDomainModels.BookingSchedule = DateTime.Now;
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
            bookingDomainModels.BookingSchedule = DateTime.Now;

            await bookingRepository.CreateAsync(bookingDomainModels);

                bookingDomainModels.Customer = customer;
            var bookingDTO = mapper.Map<BookingDTO>(bookingDomainModels);
            bookingDTO.CustomerName = customer?.FullName;
            return Ok(apiResponseService.CreateSuccessResponse(bookingDTO, "Booking created successfully"));
        }

        [HttpGet("available")]
        //
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize(Roles = "Admin,Manager")]
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
        //[Authorize(Roles = "Admin,Customer,Manager")]
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
        //[Authorize]
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
            if (booking.Status == BookingStatus.Completed)
            {
                return BadRequest("Cannot update the time for booking. Becasue Booking is completed");
            }
            if ( booking.Status == BookingStatus.InProgress )
            {
                return BadRequest("Cannot update the time for booking. Because Booking is InProgress");
            }
            if (booking.Status == BookingStatus.Canceled)
            {
                return BadRequest("Cannot update the time for booking. Because Booking is Canceled");
            }


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
        //[Authorize(Roles = "Admin,Manager")]
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

        [HttpPost("update-time-booking")]
        public async Task<IActionResult> UpdateBooking([FromBody] UpdateTimeBookingRequestDTO updateBookingRequest)
        {

            // Tìm kiếm booking dựa trên bookingId
            var booking = await petSpaContext.Bookings
                                              .Include(b => b.BookingDetails)
                                              .FirstOrDefaultAsync(b => b.BookingId == updateBookingRequest.BookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Kiểm tra trạng thái của booking, chỉ cho phép thay đổi nếu booking chưa hoàn thành
            if (booking.Status == BookingStatus.Completed)
            {
                return BadRequest("Booking Đã Hoàn Thành, Không thế Thay đồi!!!");
            }
            else if (booking.Status == BookingStatus.InProgress)
            {
                return BadRequest("Booking đang trong quá trình thực hiện. Không thể thay đổi!!");
            }


            // Cập nhật StaffId cho từng chi tiết booking và tính lại Duration
            foreach (var detail in booking.BookingDetails)
            {
                detail.StaffId = updateBookingRequest.NewStaffId;
                detail.Duration = detail.Duration; // Nếu bạn cần tính lại duration chi tiết từ thông tin khác, bạn có thể tính lại ở đây
            }

            // Cập nhật lại thời gian bắt đầu của booking
            booking.StartDate = updateBookingRequest.NewBookingSchedule;

            // Tính tổng duration và cập nhật EndDate
            var totalDurationTicks = booking.BookingDetails.Sum(bd => bd.Duration.Ticks);
            var totalDurationTimeSpan = new TimeSpan(totalDurationTicks);

            booking.EndDate = booking.StartDate + totalDurationTimeSpan;

            // Lưu thay đổi vào cơ sở dữ liệu
            petSpaContext.Bookings.Update(booking);
            await petSpaContext.SaveChangesAsync();

            return Ok("Booking updated successfully.");
        }
        [HttpPost("update-feedback")]
        public async Task<IActionResult> UpdateFeedback([FromBody] UpdateFeebackDTO updateBookingRequest)
        {

            // Tìm kiếm booking dựa trên bookingId
            var booking = await petSpaContext.Bookings
                                              .Include(b => b.BookingDetails)
                                              .FirstOrDefaultAsync(b => b.BookingId == updateBookingRequest.BookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            
            booking.Feedback = updateBookingRequest.Feedback;

            

            // Lưu thay đổi vào cơ sở dữ liệu
            petSpaContext.Bookings.Update(booking);
            await petSpaContext.SaveChangesAsync();

            return Ok("Booking updated successfully.");
        }

        [HttpPost("update-time-booking-nostaff")]
        public async Task<IActionResult> UpdateBookingNoTime([FromBody] UpdateTimeBookingNoStaffRequest updateTimeBookingNoStaffRequest)
        {
            // Kiểm tra xem thời gian đặt lịch có nằm trong khung giờ làm việc từ 8h sáng đến 8h tối không
            var startOfWorkDay = new TimeSpan(8, 0, 0); // 8h sáng
            var endOfWorkDay = new TimeSpan(20, 0, 0); // 8h tối
            var bookingTimeOfDay = updateTimeBookingNoStaffRequest.NewBookingSchedule.TimeOfDay;

            if (bookingTimeOfDay < startOfWorkDay || bookingTimeOfDay > endOfWorkDay)
            {
                return BadRequest("Thời gian đặt lịch phải trong khung giờ làm việc từ 8h sáng đến 8h tối.");
            }

            // Tìm kiếm booking dựa trên bookingId
            var booking = await petSpaContext.Bookings
                                              .Include(b => b.BookingDetails)
                                              .FirstOrDefaultAsync(b => b.BookingId == updateTimeBookingNoStaffRequest.BookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Kiểm tra trạng thái của booking, chỉ cho phép thay đổi nếu booking chưa hoàn thành
            if (booking.Status == BookingStatus.Completed)
            {
                return BadRequest("Booking Đã Hoàn Thành, Không thế Thay đồi!!!");
            }
            else if (booking.Status == BookingStatus.InProgress)
            {
                return BadRequest("Booking đang trong quá trình thực hiện. Không thể thay đổi!!");
            }

            // Chuyển đổi StartDate thành DateTime trước khi tính toán
            var bookingStartDate = booking.StartDate;

            // Tính toán Duration
            TimeSpan duration = updateTimeBookingNoStaffRequest.NewBookingSchedule - bookingStartDate;

            // Nếu cần giữ nguyên duration thì tính duration từ StartDate cũ
            var originalDuration = booking.EndDate - booking.StartDate;

            // Cập nhật lại thời gian bắt đầu của booking
            booking.StartDate = updateTimeBookingNoStaffRequest.NewBookingSchedule;

            // Tính tổng duration và cập nhật EndDate dựa trên originalDuration
            booking.EndDate = booking.StartDate + originalDuration;

            // Đặt lại trạng thái checkAccept thành false
            booking.CheckAccept = CheckAccpectStatus.NotChecked;

            // Lưu thay đổi vào cơ sở dữ liệu
            petSpaContext.Bookings.Update(booking);
            await petSpaContext.SaveChangesAsync();
            return Ok("Booking updated successfully.");
        }


        [HttpGet("completed")]
        //[Authorize(Roles = "Admin,Customer,Manager")]

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
                var bookings = await bookingRepository.GetBookingsByApprovalStatusAsync(CheckAccpectStatus.NotChecked);
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
                var bookings = await bookingRepository.GetBookingsByApprovalStatusAsync(CheckAccpectStatus.Accepted);
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

                // Gán StaffId cho tất cả các BookingDetail
                foreach (var detail in booking.BookingDetails)
                {
                    detail.StaffId = acceptBookingDTO.StaffId;
                }

                // Chuyển trạng thái CheckAccept sang true
                booking.CheckAccept = CheckAccpectStatus.Accepted;
                await bookingRepository.UpdateAsync(booking.BookingId, booking);

                return Ok(apiResponseService.CreateSuccessResponse("Booking accepted successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while accepting booking.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while accepting booking"));
            }
        }

        [HttpPut("accept-booking-havestaff")]
        public async Task<IActionResult> AcceptBookingHaveStaff([FromBody] AccpectBookingHaveStaffDTO acceptBookingHaveStaffDTO)
        {
            try
            {
                var booking = await bookingRepository.GetByIdAsync(acceptBookingHaveStaffDTO.BookingId);
                if (booking == null)
                {
                    return NotFound(apiResponseService.CreateErrorResponse("Booking not found"));
                }
                // Chuyển trạng thái CheckAccept sang true
                booking.CheckAccept = CheckAccpectStatus.Accepted;
                await bookingRepository.UpdateAsync(booking.BookingId, booking);

                return Ok(apiResponseService.CreateSuccessResponse("Booking accepted successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while accepting booking.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while accepting booking"));
            }
        }

        [HttpPut("deny-booking")]
        public async Task<IActionResult> DenyBooking([FromBody] AccpectCheckCustomerBooking denyBookingDTO)
        {
            try
            {
                var booking = await bookingRepository.GetByIdAsync(denyBookingDTO.BookingId);
                if (booking == null)
                {
                    return NotFound(apiResponseService.CreateErrorResponse("Booking not found"));
                }

                // Kiểm tra nếu trạng thái là InProgress (1)
                if (booking.Status == BookingStatus.InProgress)
                {
                    return BadRequest(apiResponseService.CreateErrorResponse("Booking is not in progress"));
                }

                // Cập nhật trạng thái CheckAccept thành -1
                booking.CheckAccept = CheckAccpectStatus.Deny;
                await bookingRepository.UpdateAsync(booking.BookingId, booking);

                return Ok(apiResponseService.CreateSuccessResponse("Booking denied successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while denying booking.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while denying booking"));
            }
        }



        [HttpGet("total-revenue/current-month")]
        public async Task<IActionResult> GetTotalRevenueForCurrentMonth([FromQuery] DateTime? startDate)
        {
            var now = DateTime.UtcNow;
            if (startDate.HasValue && startDate.Value > now)
            {
                return apiResponseService.CreateBadRequestResponse("Start date cannot be a future date.");
            }

            var totalAmount = await bookingRepository.GetAllToTalForMonthAsync(startDate);
            var dailyRevenues = await bookingRepository.GetDailyRevenueForCurrentMonthAsync(startDate);
            return apiResponseService.CreateResponse(true, new { TotalAmount = totalAmount, DailyRevenues = dailyRevenues }, "Succeeded", 201);
        }

        [HttpGet("total-revenue/from-start")]
        public async Task<IActionResult> GetTotalRevenueFromStart()
        {
            var totalAmount = await bookingRepository.GetAllToTalAsync();
            return apiResponseService.CreateResponse(true, new { TotalAmount = totalAmount }, "Succeeded", 201);
        }

        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenue([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var now = DateTime.UtcNow;
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return apiResponseService.CreateBadRequestResponse("Start date and end date are required.");
            }

            if (endDate < startDate)
            {
                return apiResponseService.CreateBadRequestResponse("End date must be greater than or equal to start date.");
            }

            if (startDate.Value > now || endDate.Value > now)
            {
                return apiResponseService.CreateBadRequestResponse("Start date and end date cannot be future dates.");
            }

            var totalAmount = await bookingRepository.GetTotalRevenueAsync(startDate.Value, endDate.Value);
            return apiResponseService.CreateResponse(true, new { TotalAmount = totalAmount }, "Succeeded", 201);
        }

        [HttpGet("pending-bookings")]
        public async Task<IActionResult> GetBookingNotAccpects()
        {
            try
            {
                var bookings = await bookingRepository.GetBookingsByCheckAcceptAsync(CheckAccpectStatus.NotChecked);
                return Ok(apiResponseService.CreateSuccessResponse(mapper.Map<List<BookingDTO>>(bookings), "Pending bookings retrieved successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting pending bookings.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while getting pending bookings"));
            }
        }

        [HttpPost("cancel-booking")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequest cancelBookingRequest)
        {
            using (var transaction = await petSpaContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var booking = await petSpaContext.Bookings
                                                      .Include(b => b.Payments) // Include related Payment
                                                      .FirstOrDefaultAsync(b => b.BookingId == cancelBookingRequest.BookingId);

                    if (booking == null)
                    {
                        return NotFound(new { message = "Booking not found" });
                    }

                    if (booking.Status == BookingStatus.Canceled)
                    {
                        return BadRequest(new { message = "Booking is already canceled" });
                    }

                    // Update booking status to canceled
                    booking.Status = BookingStatus.Canceled;

                    // Subtract the booking amount from the payment's total
                    if (booking.Payments != null)
                    {
                        booking.Payments.TotalPayment -= booking.TotalAmount ?? 0;

                        // Subtract the booking amount from the customer's total spent
                        var customer = await petSpaContext.Customers.FirstOrDefaultAsync(c => c.CusId == booking.CusId);
                        if (customer != null)
                        {
                            customer.TotalSpent -= booking.TotalAmount ?? 0;
                            customer.UpdateCusRank(); // Update customer rank if needed
                        }
                    }

                    await petSpaContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { message = "Booking canceled successfully" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Internal server error: {ex.Message}" });
                }
            }
        }

        [HttpPost("refund-booking")]
        public async Task<IActionResult> RefundBooking([FromBody] CancelBookingRequest cancelBookingRequest)
        {
            using (var transaction = await petSpaContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var booking = await petSpaContext.Bookings
                                                      .Include(b => b.Payments) // Include related Payment
                                                      .FirstOrDefaultAsync(b => b.BookingId == cancelBookingRequest.BookingId);

                    if (booking == null)
                    {
                        return NotFound(new { message = "Booking not found" });

                    }

                    if (booking.Status == BookingStatus.InProgress)
                    {
                        return BadRequest(new { message = "Booking is In Progress, so cannot be refunded" });
                    }


                    if (booking.Status == BookingStatus.Completed)
                    {
                        return BadRequest(new { message = "Booking is Completed, so cannot be refunded" });
                    }

                    if (booking.Status == BookingStatus.Canceled)
                    {
                        return BadRequest(new { message = "Booking has already been refunded" });
                    }

                    // Update booking status to canceled
                    booking.Status = BookingStatus.Canceled;

                    // Calculate refund amount based on customer rank
                    decimal refundPercentage = 0;
                    var customer = await petSpaContext.Customers.FirstOrDefaultAsync(c => c.CusId == booking.CusId);

                    if (customer != null)
                    {                       
                        refundPercentage = 0.70m;              
                        decimal refundAmount = (booking.TotalAmount ?? 0) * refundPercentage;

                        // Subtract the refund amount from the payment's total
                        if (booking.Payments != null)
                        {
                            booking.Payments.TotalPayment -= refundAmount;

                            // Subtract the refund amount from the customer's total spent
                            customer.TotalSpent -= refundAmount;
                            customer.UpdateCusRank(); // Update customer rank if needed
                        }
                    }

                    await petSpaContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { message = "Booking canceled successfully" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Internal server error: {ex.Message}" });
                }
            }
        }

        // API to get bookings with checkAccept = true
        [HttpGet("bookings/accepted")]
        public async Task<IActionResult> GetAcceptedBookings()
        {
            var bookings = await bookingRepository.GetBookingsByCheckAcceptAsync(CheckAccpectStatus.Accepted);
            var bookingDtos = bookings.Select(b => new BookingStaffDTO
            {
                BookingId = b.BookingId,
                CustomerName = b.Customer.FullName ?? "Unknown",
                ServiceName = b.BookingDetails.FirstOrDefault()?.Service?.ServiceName ?? "Unknown",
                PetName = b.BookingDetails.FirstOrDefault()?.Pet?.PetName ?? "Unknown",
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = (BookingStatus)b.Status,
                StaffId = b.BookingDetails.FirstOrDefault()?.Staff?.StaffId ?? Guid.Empty,
                StaffName = b.BookingDetails.FirstOrDefault()?.Staff?.FullName ?? "Unknown",
                CheckAccept = b.CheckAccept
            }).ToList();
            return Ok(apiResponseService.CreateSuccessResponse(bookingDtos, "Accepted bookings retrieved successfully"));
        } 

        // API to get bookings with checkAccept = false
        [HttpGet("bookings/not-accepted")]
        public async Task<IActionResult> GetNotAcceptedBookings()
        {
            var bookings = await bookingRepository.GetBookingsByCheckAcceptAsync(CheckAccpectStatus.NotChecked);
            var bookingDtos = bookings.Select(b => new BookingStaffDTO
            {
                BookingId = b.BookingId,
                CustomerName = b.Customer.FullName ?? "Unknown",
                ServiceId = b.BookingDetails.FirstOrDefault()?.Service?.ServiceId ?? Guid.Empty,
                ServiceName = b.BookingDetails.FirstOrDefault()?.Service?.ServiceName ?? "Unknown",
                PetName = b.BookingDetails.FirstOrDefault()?.Pet?.PetName ?? "Unknown",
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = (BookingStatus)b.Status,
                StaffId = b.BookingDetails.FirstOrDefault()?.Staff?.StaffId ?? Guid.Empty,
                StaffName = b.BookingDetails.FirstOrDefault()?.Staff?.FullName ?? "Unknown",
                CheckAccept = (CheckAccpectStatus)b.CheckAccept
            }).ToList();
            return Ok(apiResponseService.CreateSuccessResponse(bookingDtos, "Not accepted bookings retrieved successfully"));
        }


        //booking deny
        [HttpGet("bookings/deny")]
        public async Task<IActionResult> GetDenyBookings()
        {
            var bookings = await bookingRepository.GetBookingsByCheckAcceptAsync(CheckAccpectStatus.Deny);
            var bookingDtos = bookings.Select(b => new BookingStaffDTO
            {
                BookingId = b.BookingId,
                CustomerName = b.Customer.FullName ?? "Unknown",
                ServiceId = b.BookingDetails.FirstOrDefault()?.Service?.ServiceId ?? Guid.Empty,
                ServiceName = b.BookingDetails.FirstOrDefault()?.Service?.ServiceName ?? "Unknown",
                PetName = b.BookingDetails.FirstOrDefault()?.Pet?.PetName ?? "Unknown",
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = (BookingStatus)b.Status,
                StaffId = b.BookingDetails.FirstOrDefault()?.Staff?.StaffId ?? Guid.Empty,
                StaffName = b.BookingDetails.FirstOrDefault()?.Staff?.FullName ?? "Unknown",
                CheckAccept = (CheckAccpectStatus)b.CheckAccept
            }).ToList();
            return Ok(apiResponseService.CreateSuccessResponse(bookingDtos, "Not accepted bookings retrieved successfully"));
        }


        [HttpGet("staff-denied-bookings/{staffId:Guid}")]
        public async Task<IActionResult> GetDeniedBookingsByStaffId([FromRoute] Guid staffId)
        {
            try
            {
                var bookings = await bookingRepository.GetDeniedBookingsByStaffIdAsync(staffId);
                var bookingDtos = bookings.Select(b => new BookingStaffDTO
                {
                    BookingId = b.BookingId,
                    CustomerName = b.Customer.FullName ?? "Unknown",
                    ServiceName = b.BookingDetails.FirstOrDefault()?.Service?.ServiceName ?? "Unknown",
                    PetName = b.BookingDetails.FirstOrDefault()?.Pet?.PetName ?? "Unknown",
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Status = (BookingStatus)b.Status,
                    StaffId = b.BookingDetails.FirstOrDefault()?.Staff?.StaffId ?? Guid.Empty,
                    StaffName = b.BookingDetails.FirstOrDefault()?.Staff?.FullName ?? "Unknown",
                    CheckAccept = b.CheckAccept
                }).ToList();
                return Ok(apiResponseService.CreateSuccessResponse(bookingDtos, "Denied bookings for staff retrieved successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting denied bookings for staff.");
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponseService.CreateErrorResponse("An error occurred while getting denied bookings for staff"));
            }
        }

        [HttpGet("bookings/status-2")]
        public async Task<IActionResult> GetStatus2Bookings()
        {
            var bookings = await bookingRepository.GetBookingsByStatus2Async(BookingStatus.Canceled);
            var bookingDtos = bookings.Select(b => new BookingStaffDTO
            {
                BookingId = b.BookingId,
                CustomerName = b.Customer.FullName ?? "Unknown",
                ServiceId = b.BookingDetails.FirstOrDefault()?.Service?.ServiceId ?? Guid.Empty,
                ServiceName = b.BookingDetails.FirstOrDefault()?.Service?.ServiceName ?? "Unknown",
                PetName = b.BookingDetails.FirstOrDefault()?.Pet?.PetName ?? "Unknown",
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = (BookingStatus)b.Status,
                StaffId = b.BookingDetails.FirstOrDefault()?.Staff?.StaffId ?? Guid.Empty,
                StaffName = b.BookingDetails.FirstOrDefault()?.Staff?.FullName ?? "Unknown",
                CheckAccept = (CheckAccpectStatus)b.CheckAccept
            }).ToList();

            return Ok(apiResponseService.CreateSuccessResponse(bookingDtos, "Status 2 bookings retrieved successfully"));
        }

    }    
}
