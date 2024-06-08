using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Combo;
using PetSpa.Repositories.ComboRepository;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComboController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IComboRespository comboRespository;
        private readonly ApiResponseService apiResponseService;

        public ComboController(IMapper mapper, IComboRespository comboRespository, ApiResponseService apiResponseService)
        {
            this.mapper = mapper;
            this.comboRespository = comboRespository;
            this.apiResponseService = apiResponseService;
        }

        // Create Combo
        // Post : api/Combo
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddComboRequestDTO addComboRequestDTO)
        {
            // Map DTO to Domain Model
            var comboDomainModle = mapper.Map<Combo>(addComboRequestDTO);
            await comboRespository.CreateAsync(comboDomainModle);
            var combo = mapper.Map<ComboDTO>(comboDomainModle);
            if (combo != null)
            {
                return Ok(apiResponseService.CreateSuccessResponse(comboDomainModle));
            }
            else
            {
                return Ok(apiResponseService.CreateUnauthorizedResponse());
            }
        }

        // Get Combo
        // Get: api/combo
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comboDomainModel = await comboRespository.GetAllAsync();

            // Map Domain Model To DTO
            if (comboDomainModel != null)
            {
                return Ok(mapper.Map<List<ComboDTO>>(comboDomainModel));
            }
            else
            {
                return Ok(apiResponseService.CreatePaymentNotFound());
            }
        }

        // Get Combo by ID
        // Get: /api/Combo/{id}
        [HttpGet]
        [Route("{ComboId:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid ComboId)
        {
            var comboDomainModels = await comboRespository.GetByIdAsync(ComboId);
            if (comboDomainModels == null)
            {
                return apiResponseService.CreatePaymentNotFound();
            }
            // Map DomainModel 
            var combo = mapper.Map<ComboDTO>(comboDomainModels);
            return Ok(apiResponseService.CreateSuccessResponse(combo));
        }

        // Update Combo By ID
        [HttpPut]
        [Route("{ComboId:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid ComboId, UpdateComboRequestDTO updateComboRequestDTO)
        {
            // Map DTO to Domain 
            var comboDomainModels = mapper.Map<Combo>(updateComboRequestDTO);
            await comboRespository.UpdateAsync(ComboId, comboDomainModels);
            if (comboDomainModels == null) return apiResponseService.CreatePaymentNotFound();
            // Map Domain DTO
            return Ok(mapper.Map<ComboDTO>(comboDomainModels));
        }

        // Delete Combo By ID (Set Status to False)
        [HttpDelete]
        [Route("{ComboId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid ComboId)
        {
            var comboDomainModels = await comboRespository.DeleteAsync(ComboId);
            if (comboDomainModels == null)
            {
                return apiResponseService.CreatePaymentNotFound();
            }
            // Map DomainModel 
            var combo = mapper.Map<ComboDTO>(comboDomainModels);
            return Ok(apiResponseService.CreateSuccessResponse(combo));
        }
    }
}
