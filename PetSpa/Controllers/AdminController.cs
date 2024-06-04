using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Models.DTO.Admin;
using PetSpa.Repositories.AdminRepository;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ApiResponseService apiResponseService;
        private readonly IAdminRepository adminRepository;

        public AdminController(IMapper mapper, ApiResponseService apiResponseService, IAdminRepository adminRepository)
        {
            this.mapper = mapper;
            this.apiResponseService = apiResponseService;
            this.adminRepository = adminRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var adminModelDetail =  adminRepository.GetAll();
            return Ok(mapper.Map<AdminDTO>(adminModelDetail));
        }
    }
}
