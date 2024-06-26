using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Manager;
using PetSpa.Repositories.ManagerRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IManagerRepository _managerRepository;
        private readonly ApiResponseService _apiResponseService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(IMapper mapper, IManagerRepository managerRepository, ApiResponseService apiResponseService, UserManager<ApplicationUser> userManager, ILogger<ManagerController> logger)
        {
            this._mapper = mapper;
            this._managerRepository = managerRepository;
            this._apiResponseService = apiResponseService;
            this._userManager = userManager;
            this._logger = logger;
        }

        // Get All Managers
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var manaDomainModels = await _managerRepository.GetAllAsync();
                var managerDTOs = _mapper.Map<List<ManagerDTO>>(manaDomainModels);
                return Ok(_apiResponseService.CreateSuccessResponse(managerDTOs, "Managers retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all managers.");
                return Ok(_apiResponseService.CreateErrorResponse("An error occurred while getting all managers"));
            }
        }

        // Get Manager By ID
        // Get /api/Manager/{id}
        [HttpGet]
        [Route("{ManaId:guid}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetById([FromRoute] Guid ManaId)
        {
            try
            {
                var manaDomainModels = await _managerRepository.GetByIDAsync(ManaId);
                if (manaDomainModels == null)
                {
                    return NotFound(_apiResponseService.CreateErrorResponse("Manager not found"));
                }

                var managerDTO = _mapper.Map<ManagerDTO>(manaDomainModels);
                return Ok(_apiResponseService.CreateSuccessResponse(managerDTO, "Manager retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting manager by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }

        // Create Manager
        // POST: /api/Manager
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateManager([FromBody] AddRequestManagerDTO addManagerRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(_apiResponseService.CreateErrorResponse("Invalid data"));
            }

            try
            {
                // Tạo một đối tượng ApplicationUser mới
                var user = new ApplicationUser
                {
                    UserName = addManagerRequestDTO.UserName,
                    Email = addManagerRequestDTO.Email,
                    EmailConfirmed = true
                };

                // Tạo người dùng mới trong bảng AspNetUsers
                var result = await _userManager.CreateAsync(user, addManagerRequestDTO.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Error creating user: Code={Code}, Description={Description}", error.Code, error.Description);
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Error creating user"));
                }

                // Thêm người dùng vào role "Manager"
                await _userManager.AddToRoleAsync(user, "Manager");

                // Tạo bản ghi mới trong bảng Manager
                var manager = _mapper.Map<Manager>(addManagerRequestDTO);
                manager.ManaId = Guid.NewGuid();
                manager.Id = user.Id;

                var createdManager = await _managerRepository.CreateAsync(manager);
                var managerDTO = _mapper.Map<ManagerDTO>(createdManager);

                return Ok(_apiResponseService.CreateSuccessResponse(managerDTO, "Manager created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating manager.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }

        // Update Manager
        // PUT: /api/Manager/{id}
        [HttpPut]
        [Route("{ManaId:guid}")]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> Update([FromRoute] Guid ManaId, UpdateManagerRequestDTO updateManagerRequestDTO)
        {
            try
            {
                var managerDomainModels = _mapper.Map<Manager>(updateManagerRequestDTO);
                managerDomainModels = await _managerRepository.UpdateAsync(ManaId, managerDomainModels);

                if (managerDomainModels == null)
                {
                    return NotFound(_apiResponseService.CreateErrorResponse("Manager not found"));
                }

                var managerDTO = _mapper.Map<ManagerDTO>(managerDomainModels);
                return Ok(_apiResponseService.CreateSuccessResponse(managerDTO, "Manager updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating manager.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }
    }
}
