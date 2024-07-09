using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Customer;
using PetSpa.Repositories.CustomerRepository;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly ApiResponseService _apiResponseService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(IMapper mapper, ICustomerRepository customerRepository, ApiResponseService apiResponseService, UserManager<ApplicationUser> userManager, ILogger<CustomerController> logger)
        {
            this._mapper = mapper;
            this._customerRepository = customerRepository;
            this._apiResponseService = apiResponseService;
            this._userManager = userManager;
            this._logger = logger;
        }

        // Get All Customers
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customerDomainModels = await _customerRepository.GetAllAsync();
                var customerDTOs = _mapper.Map<List<CustomerDTO>>(customerDomainModels);
                return Ok(_apiResponseService.CreateSuccessResponse(customerDTOs, "Customers retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all customers.");
                return Ok(_apiResponseService.CreateErrorResponse("An error occurred while getting all customers"));
            }
        }


        // Get Customer By ID
        // Get /api/Customer/{id}
        [HttpGet]
        [Route("{CusId:guid}")]
        //[Authorize]
        public async Task<IActionResult> GetById([FromRoute] Guid CusId)
        {
            try
            {
                var customerDomainModel = await _customerRepository.GetByIdAsync(CusId);

                if (customerDomainModel == null)
                {
                    return NotFound(_apiResponseService.CreateErrorResponse("Customer not found"));
                }

                var customerDTO = _mapper.Map<CustomerDTO>(customerDomainModel);
                return Ok(_apiResponseService.CreateSuccessResponse(customerDTO, "Customer retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting customer by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }

        [HttpGet]
        [Route("{cusId:guid}/bookings")]
        //[Authorize]
        public async Task<IActionResult> GetBookingsById([FromRoute] Guid cusId)
        {
            try
            {
                var customerDomainModel = await _customerRepository.GetByIdBookingAsync(cusId);

                if (customerDomainModel == null)
                {
                    return NotFound(_apiResponseService.CreateErrorResponse("Customer not found"));
                }

                var customerDTO = _mapper.Map<CustomerDTO>(customerDomainModel);
                return Ok(_apiResponseService.CreateSuccessResponse(customerDTO, "Customer retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting customer by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }

        // Update Customer by User
        // PUT: /api/Customer/UpdateByUser/{id}
        [HttpPut]
        [Route("UpdateByUser/{CusId:guid}")]
        public async Task<IActionResult> UpdateByUser([FromRoute] Guid CusId, UpdateCustomerRequestDTO updateCustomerByUserRequestDTO)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(CusId);
                if (existingCustomer == null)
                {
                    return NotFound(_apiResponseService.CreateErrorResponse("Customer not found"));
                }

                existingCustomer.FullName = updateCustomerByUserRequestDTO.FullName;
                existingCustomer.Gender = updateCustomerByUserRequestDTO.Gender;
                existingCustomer.PhoneNumber = updateCustomerByUserRequestDTO.PhoneNumber;

                var updatedCustomer = await _customerRepository.UpdateAsync(CusId, existingCustomer);
                var customerDTO = _mapper.Map<CustomerDTO>(updatedCustomer);
                return Ok(_apiResponseService.CreateSuccessResponse(customerDTO, "Customer updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating customer.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }


        // Update Customer by Admin
        // PUT: /api/Customer/UpdateByAdmin/{id}
        [HttpPut]
        [Route("UpdateByAdmin/{CusId:guid}")]
        public async Task<IActionResult> UpdateByAdmin([FromRoute] Guid CusId, UpdateCustomerRequestByAdminDTO updateCustomerByAdminRequestDTO)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(CusId);
                if (existingCustomer == null)
                {
                    return NotFound(_apiResponseService.CreateErrorResponse("Customer not found"));
                }

                existingCustomer.FullName = updateCustomerByAdminRequestDTO.FullName;
                existingCustomer.Gender = updateCustomerByAdminRequestDTO.Gender;
                existingCustomer.PhoneNumber = updateCustomerByAdminRequestDTO.PhoneNumber;
                existingCustomer.CusRank = updateCustomerByAdminRequestDTO.CusRank;

                var updatedCustomer = await _customerRepository.UpdateAsync(CusId, existingCustomer);
                var customerDTO = _mapper.Map<CustomerDTO>(updatedCustomer);
                return Ok(_apiResponseService.CreateSuccessResponse(customerDTO, "Customer updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating customer.");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }

        [HttpDelete("delete-customer")]
        public async Task<IActionResult> DeleteCustomer(Guid cusId)
        {
            try 
            {
                var customerDomainModel = await _customerRepository.DeleteAsync(cusId);
                if (!customerDomainModel)
                {
                    return NotFound(_apiResponseService.CreateErrorResponse("Customer not found"));
                }

                return Ok(_apiResponseService.CreateSuccessResponse("Customer deleted successfull"));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occured while deleting customer. ");
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponseService.CreateErrorResponse("Internal server error"));
            }
        }

    }
}