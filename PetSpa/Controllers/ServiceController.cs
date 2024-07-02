using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.ApiResponseDTO;
using PetSpa.Models.DTO.Service;
using PetSpa.Repositories.ServiceRepository;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IServiceRepository serviceRepository;
        private readonly ApiResponseService apiResponseService;

        public ServiceController(IMapper mapper, IServiceRepository serviceRepository, ApiResponseService apiResponseService)
        {
            this.mapper = mapper;
            this.serviceRepository = serviceRepository;
            this.apiResponseService = apiResponseService;
        }

        // Create Service
        // Post: /api/Service
        [HttpPost]
        [Authorize(Roles = "Admin,Customer,Manager")]

        public async Task<IActionResult> Create([FromBody] AddServiceRequest addServiceRequest)
        {
            if (!ModelState.IsValid)
            {
                var errorDetails = new ValidationProblemDetails(ModelState);
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(errorDetails));
                return BadRequest(ModelState); // Trả về lỗi xác thực chi tiết
            }
            var serviceDomainModel = mapper.Map<Service>(addServiceRequest);
            await serviceRepository.CreateAsync(serviceDomainModel);
            var service = mapper.Map<Service>(serviceDomainModel);
            return Ok(apiResponseService.CreateSuccessResponse(apiResponseService.CreateSuccessResponse(service), "Successed"));

        }
        // Get Service
        // Get: /api/Service
        [HttpGet]
        //[Authorize(Roles = "Admin,Customer,Manager")]

        public async Task<IActionResult> GetAll()
        {
            var serviceDomainModel = await serviceRepository.GetAllAsync();
          return Ok(apiResponseService.CreateSuccessResponse(apiResponseService.CreateSuccessResponse(serviceDomainModel), "Successed"));

        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchServices(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest(apiResponseService.CreateErrorResponse("Search term is required"));
            }

            var allServices = await serviceRepository.GetAllAsync();

            var filteredServices = allServices.Where(service =>
                service.ServiceName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                service.ServiceDescription.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!filteredServices.Any())
            {
                return NotFound(apiResponseService.CreateErrorResponse("No services found matching the search term"));
            }

            return Ok(apiResponseService.CreateSuccessResponse(filteredServices, "Search successful"));
        }

        // Get Service by ID
        // Get: /api/Service/{id}
        [HttpGet]
        [Route("{ServiceId:guid}")]
        

        public async Task<IActionResult> GetById([FromRoute] Guid ServiceId)
        {
            var serviceDomainModel = await serviceRepository.GetByIdAsync(ServiceId);

            if (serviceDomainModel == null)
            {
                return apiResponseService.CreateUnauthorizedResponse();
            }
            var service = mapper.Map<ServiceDTO>(serviceDomainModel);
            return Ok(apiResponseService.CreateSuccessResponse(service));
        }

        // Update Service by ServiceId
        // Put: /api/Service/{ServiceId}
        [HttpPut]
        [Route("{ServiceId:guid}")]
        [Authorize(Roles = "Admin,Manager")]

        public async Task<IActionResult> Update([FromRoute] Guid ServiceId, [FromBody] UpdateServiceRequestDTO updateServiceRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                // Log chi tiết lỗi xác thực
                var errorDetails = new ValidationProblemDetails(ModelState);
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(errorDetails));
                return BadRequest(ModelState); // Trả về lỗi xác thực chi tiết
            }

            // Lấy thông tin dịch vụ hiện tại từ cơ sở dữ liệu
            var existingService = await serviceRepository.GetByIdAsync(ServiceId);
            if (existingService == null)
            {
                return NotFound("Service not found");
            }

            // Cập nhật thông tin từ DTO vào mô hình dịch vụ
            existingService.ServiceName = updateServiceRequestDTO.ServiceName;
            existingService.Status = updateServiceRequestDTO.Status;
            existingService.ServiceDescription = updateServiceRequestDTO.ServiceDescription;
            existingService.ServiceImage = updateServiceRequestDTO.ServiceImage;
            existingService.Duration = TimeSpan.Parse(updateServiceRequestDTO.Duration);
            existingService.ComboId = updateServiceRequestDTO.ComboId;

            // Lưu thay đổi vào cơ sở dữ liệu
            var updatedService = await serviceRepository.UpdateAsync(ServiceId, existingService);
            if (updatedService == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating service");
            }

            // Trả về kết quả cập nhật thành công
            var serviceDTO = mapper.Map<ServiceDTO>(updatedService);
            return Ok(apiResponseService.CreateSuccessResponse(serviceDTO));
        }


        // Delete Service by ServiceId (Set Status to False)
        [HttpDelete]
        [Route("{ServiceId:guid}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete([FromRoute] Guid ServiceId)
        {
            var serviceDomainModel = await serviceRepository.DeleteAsync(ServiceId);
            if (serviceDomainModel == null)
            {
                return apiResponseService.CreatePaymentNotFound();
            }
            var service = mapper.Map<ServiceDTO>(serviceDomainModel);
            return Ok(apiResponseService.CreateSuccessResponse(service));
        }
    }
}
