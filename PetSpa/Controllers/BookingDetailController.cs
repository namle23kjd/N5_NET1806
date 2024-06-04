using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.BookingDetail;
using PetSpa.Repositories.BookingDetailRepository;


namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingDetailController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IBookingDetailsRepository bookingDetailsRepository;
        private readonly ApiResponseService apiResponseService;

        public BookingDetailController(IMapper mapper , IBookingDetailsRepository bookingDetailsRepository, ApiResponseService apiResponseService)
        {
            this.mapper = mapper;
            this.bookingDetailsRepository = bookingDetailsRepository;
            this.apiResponseService = apiResponseService;
        }

        //Create BookingDetail
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddBookingDetailRequestDTO addBookingDetailRequestDTO )
        {
            var bookingDetailDomainModel = mapper.Map<BookingDetail>(addBookingDetailRequestDTO);
            await bookingDetailsRepository.CreateAsync(bookingDetailDomainModel);
            return Ok(mapper.Map<BookingDetailDTO>(bookingDetailDomainModel));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookingDetailsDomainModel = await bookingDetailsRepository.GetAllAsync();
            return Ok(mapper.Map<List<BookingDetailDTO>>(bookingDetailsDomainModel));
        }

        [HttpGet]
        [Route("{BookingDetailId:guid}")]
        public async Task<IActionResult> GetByID([FromRoute] Guid BookingDetailId)
        {
            var bookingDetailDomain = await bookingDetailsRepository.GetByIdAsync(BookingDetailId);

            if( bookingDetailDomain == null)
            {   
                return apiResponseService.CreatePaymentNotFound();
            }
            return Ok(mapper.Map<BookingDetailDTO>(bookingDetailDomain));
        }
        [HttpPut]
        [Route("{BookingDetailId:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid BookingDetailId, UpdateBookingDetailDTO updateBookingDetailDTO)
        {
            var bookingDetailDomainModels = mapper.Map<BookingDetail>(updateBookingDetailDTO);

            //Check bookingDetails Exist

            if(bookingDetailDomainModels == null) return apiResponseService.CreatePaymentNotFound();

            var bookingDetailDTO = mapper.Map<BookingDetailDTO>(bookingDetailDomainModels);
            return Ok(apiResponseService.CreateSuccessResponse(bookingDetailDTO));
        }
    }
}
