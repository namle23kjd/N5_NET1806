using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
using PetSpa.Repositories.BookingRepository;

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


        //Create Booking
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddBookingRequestDTO addBookingRequestDTO)
        {
            //Map DTO to DomainModel
            //var bookingDomaiModels =  mapper.Map<Booking>(addBookingRequestDTO);

            //await bookingRepository.CreateAsync(bookingDomaiModels);
            //return Ok(mapper.Map<BookingDTO>(bookingDomaiModels));
            // Check if the booking date is in the past
            if (addBookingRequestDTO.BookingSchedule < DateTime.Now)
            {
                return BadRequest("Ngày đặt lịch không được trễ hơn ngày hiện tại. Vui lòng chọn ngày khác.");
            }

            var isScheduleTaken = petSpaContext.Bookings.Any(b => b.BookingSchedule == addBookingRequestDTO.BookingSchedule && b.Status);

            if (isScheduleTaken)
            {
                return BadRequest("Thời gian này đã được đặt. Vui lòng chọn lịch khác.");
            }

            // Map DTO to DomainModel
            var bookingDomainModels = mapper.Map<Booking>(addBookingRequestDTO);

            await bookingRepository.CreateAsync(bookingDomainModels);
            return Ok(mapper.Map<BookingDTO>(bookingDomainModels));
        }

        [HttpGet("available")]
        public async Task<IActionResult> CheckAvailabilityBooking (DateTime bookingSchedule)
        {
            bool isAvailable = !petSpaContext.Bookings.Any(b => b.BookingSchedule == bookingSchedule && b.Status);

            if (isAvailable)
            {
                return Ok("Thời gian này có sẵn.");
            }

            return BadRequest("Thời gian này đã được đặt. Vui lòng chọn lịch khác.");
        }


        [HttpGet("availableDate")]
        public async Task<IActionResult> CheckAvailabilityDate(DateTime bookingSchedule)
        {
            // Check if the booking date is in the past
            if (bookingSchedule < DateTime.Now)
            {
                return BadRequest("Ngày đặt lịch không được trễ hơn ngày hiện tại. Vui lòng chọn ngày khác.");
            }

            bool isAvailable = !petSpaContext.Bookings.Any(b => b.BookingSchedule == bookingSchedule && b.Status);

            if (isAvailable)
            {
                return Ok("Thời gian này có sẵn.");
            }

            return BadRequest("Thời gian này đã được đặt. Vui lòng chọn lịch khác.");
        }


        //Get booking 
        //Get : /api/booking
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
          var bookingDomainModels =  await bookingRepository.GetAllAsync();
            //Map Domain Model to DTO
            return Ok(mapper.Map<List<BookingDTO>>(bookingDomainModels));
        }

        //Get Booking ID 
        //Get : api/booking/{BookingId}
        [HttpGet]
        [Route("{BookingId:Guid}")]

        public async Task<IActionResult> GetById([FromRoute] Guid BookingId)
        {
            var bookingDomainModel = await bookingRepository.GetByIdAsync(BookingId);
            if(bookingDomainModel == null)
            {
                return apiResponseService.CreatePaymentNotFound();
            }

            //Map DomainModel to DTO
            return Ok(mapper.Map<BookingDTO>(bookingDomainModel));
        }

        //Update Booking 
        //Put: /api/Booking/{BookingId}

        [HttpPut]
        [Route("{BookingId:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid BookingId, UpdateBookingRequestDTO updateBookingRequestDTO)
        {
            var bookingDomainModel = mapper.Map<Booking>(updateBookingRequestDTO);
            await bookingRepository.UpdateAsync(BookingId, bookingDomainModel);

            if(bookingDomainModel == null)  return apiResponseService.CreatePaymentNotFound();

            //Map DomainModel to DTO

            return Ok(mapper.Map<BookingDTO>(bookingDomainModel));
        }

    }
}
