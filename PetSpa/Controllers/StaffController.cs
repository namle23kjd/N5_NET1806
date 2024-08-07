﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Staff;
using PetSpa.Repositories.StaffRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using PetSpa.Repositories.BookingRepository;
using PetSpa.Models.DTO.Booking;
using Microsoft.AspNetCore.Authorization;
using Serilog.Core;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStaffRepository _staffRepository;
        private readonly ApiResponseService _apiResponseService;
        private readonly ILogger<StaffController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingRepository _bookingRepository;

        public StaffController(IMapper mapper, IStaffRepository staffRepository, ApiResponseService apiResponseService, ILogger<StaffController> logger, UserManager<ApplicationUser> userManager, IBookingRepository _bookingRepository)
        {
            this._mapper = mapper;
            this._staffRepository = staffRepository;
            this._apiResponseService = apiResponseService;
            this._logger = logger;
            this._userManager = userManager;
            this._bookingRepository = _bookingRepository;
        }

        //Get Staff
        //Get : /api/Staff
        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            try
            {
                var staffDomainModel = await _staffRepository.GetALlAsync();
                var staffDTOs = _mapper.Map<List<StaffDTO>>(staffDomainModel);
                return Ok(_apiResponseService.CreateSuccessResponse(staffDTOs, "Staff retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all staff.");
                return Ok(_apiResponseService.CreateErrorResponse("An error occurred while getting all staff"));
            }
        }

        // Create Staff
        // POST: /api/Staff
        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] AddStaffRequestDTO addStaffRequestDTO)
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
                    UserName = addStaffRequestDTO.UserName,
                    Email = addStaffRequestDTO.Email,
                    EmailConfirmed = true
                };

                // Tạo người dùng mới trong bảng AspNetUsers
                var result = await _userManager.CreateAsync(user, addStaffRequestDTO.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Error creating user: Code={Code}, Description={Description}", error.Code, error.Description);
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Error creating user"));
                }

                // Thêm người dùng vào role "Staff"
                await _userManager.AddToRoleAsync(user, "Staff");

                // Tạo bản ghi mới trong bảng Staff
                var staff = _mapper.Map<Staff>(addStaffRequestDTO);
                staff.StaffId = Guid.NewGuid();
                staff.Id = user.Id;

                var createdStaff = await _staffRepository.CreateAsync(staff);
                var staffDTO = _mapper.Map<StaffDTO>(createdStaff);

                return Ok(_apiResponseService.CreateSuccessResponse(staffDTO, "Staff created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating staff.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }

        //Get Staff By ID
        //Get /api/Staff/{id}
        [HttpGet]
        [Route("{StaffId:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid StaffId)
        {
            try
            {
                var staffDomainModel = await _staffRepository.GetByIdAsync(StaffId);

                if (staffDomainModel == null)
                {
                    return NotFound(_apiResponseService.CreateErrorResponse("Staff not found"));
                }

                var staffDTO = _mapper.Map<StaffDTO>(staffDomainModel);
                return Ok(_apiResponseService.CreateSuccessResponse(staffDTO, "Staff retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting staff by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }

        //Update Staff
        // PUT: /api/Staff/{id}
        [HttpPut]
        [Route("{StaffId:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid StaffId, UpdateStaffRequestDTO updateStaffRequestDTO)
        {
            try
            {
                var staffDomainModel = _mapper.Map<Staff>(updateStaffRequestDTO);
                staffDomainModel = await _staffRepository.UpdateAsync(StaffId, staffDomainModel);

                if (staffDomainModel == null)
                {
                    return NotFound(_apiResponseService.CreateErrorResponse("Staff not found"));
                }

                var staffDTO = _mapper.Map<StaffDTO>(staffDomainModel);
                return Ok(_apiResponseService.CreateSuccessResponse(staffDTO, "Staff updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating staff.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }

        
        //Lấy thông tin booking mà nhân viên đang làm
        [HttpGet("{staffId:guid}/current-booking")]
        //[Authorize(Roles ="Staff")]
        public async Task<IActionResult> GetCurrentBookingForStaff(Guid staffId)
        {
            var bookings = await _staffRepository.GetBookingsByStatusAsync(staffId, BookingStatus.InProgress);
            var bookingDtos = bookings.Select(b => new StaffBookingDTO
            {
                BookingId = b.BookingId,
                CustomerName = b.Customer.FullName,
                ServiceName = b.BookingDetails.FirstOrDefault()?.Service?.ServiceName ?? "N/A",
                PetName = b.BookingDetails.FirstOrDefault()?.Pet?.PetName ?? "N/A",
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = (BookingStatus)b.Status
            }).ToList();
            return Ok(bookingDtos);

        }
        [HttpGet("{staffId:guid}/booking-history")]
        public async Task<IActionResult> GetBookingHistoryForStaf(Guid staffId)
        {
            try 
            {
                var bookings = await _bookingRepository.GetBookingHistoryForStaffAsync(staffId);
                return Ok(_apiResponseService.CreateSuccessResponse(_mapper.Map<List<BookingDTO>>(bookings), "Booking history retrieved successfully"));
            }catch (Exception ex)
            {
                _logger.LogError(ex, "An Error Occurred While Getting Booking History For The Staff");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("An Error Occurred While Getting Booking History For The Staff"));
            }

        }

        [HttpGet("{staffId}/completed-bookings")]
        public async Task<IActionResult> GetCompletedBookings(Guid staffId)
        {
            var bookings = await _staffRepository.GetBookingsByStatusAsync(staffId, BookingStatus.Completed);
            var bookingDtos = bookings.Select(b => new StaffBookingDTO
            {
                BookingId = b.BookingId,
                CustomerName = b.Customer.FullName ?? "Unknown",
                ServiceName = b.BookingDetails.FirstOrDefault()?.Service?.ServiceName ?? "Unknown",
                PetName = b.BookingDetails.FirstOrDefault()?.Pet?.PetName ?? "Unknown",
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = (BookingStatus)b.Status 
            }).ToList();
            return Ok(bookingDtos);
        }

        [HttpGet("{staffId}/pending-bookings")]
        public async Task<IActionResult> GetPendingBookings(Guid staffId)
        {
            var bookings = await _staffRepository.GetBookingsByStatusAsync(staffId, BookingStatus.NotStarted);
            var bookingDtos = bookings.Select(b => new StaffBookingDTO
            {
                BookingId = b.BookingId,
                CustomerName = b.Customer.FullName ?? "Unknown",
                ServiceName = b.BookingDetails.FirstOrDefault()?.Service?.ServiceName ?? "Unknown",
                PetName = b.BookingDetails.FirstOrDefault()?.Pet?.PetName ?? "Unknown",
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = (BookingStatus)b.Status
            }).ToList();
            return Ok(bookingDtos);
        }

        [HttpGet("bookings-summary")]
        public async Task<IActionResult> GetStaffBookingsSummary([FromQuery] DateTime date)
        {
            try
            {
                var staffBookings = await _staffRepository.GetStaffBookingsByDateAsync(date);
                return Ok(_apiResponseService.CreateSuccessResponse(staffBookings, "Staff bookings summary retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting staff bookings summary.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("An error occurred while getting staff bookings summary"));
            }
        }

    }
}
