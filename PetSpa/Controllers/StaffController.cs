using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Staff;
using PetSpa.Repositories.StaffRepository;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IStaffRepository staffRepository;
        private readonly ApiResponseService _apiResponseService;

        public StaffController(IMapper mapper, IStaffRepository staffRepository, ApiResponseService apiResponseService)
        {
            this.mapper = mapper;
            this.staffRepository = staffRepository;
            this._apiResponseService = apiResponseService;
        }
        //Create Staff 
        //Post: /api/Staff
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddStaffRequestDTO addStaffRequestDTO)
        {
            // Map DTO to DomainModel
            var staffDomainModel = mapper.Map<Staff>(addStaffRequestDTO);
            await staffRepository.CreateAsync(staffDomainModel);
            //Map Domail model to DTO
            return Ok(mapper.Map<StaffDTO>(staffDomainModel));
        }

        //Get Staff
        //Get : /api/Staff
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
           var staffDomainModel =  await staffRepository.GetALlAsync();

            //Map domainModelt to DTO
            return Ok(mapper.Map<List<StaffDTO>>(staffDomainModel));
        }

        //Get Saff By ID
        //Get /api/Staff/{id}
        [HttpGet]
        [Route("{StaffId:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid StaffId)
        {
            //var staffDomainModels = await staffRepository.GetByIdAsync(StaffId);
            //if( staffDomainModels == null)
            //{
            //    return NotFound();
            //}
            ////Map Domain Models DTO
            //return Ok(mapper.Map<StaffDTO>(staffDomainModels));

            var staffDomainModel = await staffRepository.GetByIdAsync(StaffId);

            if (staffDomainModel == null)
            {
                return _apiResponseService.CreateUnauthorizedResponse();
            }

            var staffDTO = mapper.Map<StaffDTO>(staffDomainModel);
            return Ok(_apiResponseService.CreateSuccessResponse(staffDTO));
        }

        //Update Staff
        // PUT: /api/Staff/{id}
        [HttpPut]
        [Route("{StaffId:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid StaffId, UpdateStaffRequestDTO updateStaffRequestController)
        {
            //Map DTO to Domain Model
            var staffDomainModels = mapper.Map<Staff>(updateStaffRequestController);
            staffDomainModels = await staffRepository.UpdateAsync(StaffId, staffDomainModels);

            if(staffDomainModels == null)
            {
                return NotFound();
            }
            //Map Domain Models to DTO
            return Ok(mapper.Map<StaffDTO>(staffDomainModels));
        }


    }
}
