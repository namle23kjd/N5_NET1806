using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Admin;
using PetSpa.Repositories.AdminRepository;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ApiResponseService apiResponseService;
        private readonly IAdminRepository adminRepository;
        private readonly ILogger<AdminController> logger;

        public AdminController(IMapper mapper, ApiResponseService apiResponseService, IAdminRepository adminRepository, ILogger<AdminController> logger)
        {
            this.mapper = mapper;
            this.apiResponseService = apiResponseService;
            this.adminRepository = adminRepository;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var adminModelDetails = await adminRepository.GetAllAsync();
                var adminDTOs = mapper.Map<List<AdminDTO>>(adminModelDetails);
                return Ok(apiResponseService.CreateSuccessResponse(adminDTOs, "Admins retrieved successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting all admins.");
                return Ok(apiResponseService.CreateErrorResponse("An error occurred while getting all admins"));
            }
        }

        [HttpPost]
        [Route("CreateAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] AddAdminRequestDTO addAdminRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(apiResponseService.CreateErrorResponse("Invalid data"));
            }

            try
            {
                var result = await adminRepository.CreateAdminAsync(addAdminRequestDTO);
                if (!result)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error creating admin");
                }

                return Ok(apiResponseService.CreateSuccessResponse("Admin created successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating admin.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }


        [HttpGet("{adminId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FindAdmin(Guid adminId)
        {
            try
            {
                var admin = await adminRepository.FindAdminAsync(adminId);
                if (admin == null)
                {
                    return NotFound(apiResponseService.CreateErrorResponse("Admin not found"));
                }
                var adminDTO = mapper.Map<AdminDTO>(admin);
                return Ok(apiResponseService.CreateSuccessResponse(adminDTO, "Admin retrieved successfully"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while finding admin.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

    }
}
