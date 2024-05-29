using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.CustomerDTO;
using PetSpa.Models.DTO.PetDTO;
using PetSpa.Repositories;
using PetSpa.Repositories.Customer;
using PetSpa.Repositories.Pet;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController  : ControllerBase
    {
        private readonly PetSpaContext petSpaContext;
        private readonly ICusRepository cusRepository;
        private readonly IMapper mapper;

        public CustomerController(PetSpaContext petSpaContext, ICusRepository cusRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.petSpaContext = petSpaContext;
            this.cusRepository = cusRepository;
        }

        [HttpGet]
        [ValidateModeAtrribute]
        public async Task<IActionResult> GetAll()
        {
            var cusDomainModel = await cusRepository.GetALLAsync();
            return Ok(mapper.Map<List<CustomerDTO>>(cusDomainModel));
        }
        [HttpGet]
        [Route("{ID:guid}")]
        [ValidateModeAtrribute]
        public async Task<IActionResult> GetById(Guid ID)
        {
            var cus = await cusRepository.getByIdAsync(ID);

            if (cus == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<CustomerDTO>(cus));
        }

        [HttpPost]

        [Route("{ID:guid}")]
        [ValidateModeAtrribute]
        public async Task<IActionResult> Create(Guid ID, [FromBody] AddCusRequestDTO AddCusRequestDTO)
        {
            //if (false)//check id cus có tồn tại không
            //{
            //   return badrequest("stock does not exist");
            //}


            var cusDomainModels = mapper.Map<Customer>(AddCusRequestDTO);
            cusDomainModels.AccId = ID;
            cusDomainModels.CusId = new Guid();
            cusDomainModels = await cusRepository.CreateAsync(cusDomainModels);


            var CusDTO = mapper.Map<CustomerDTO>(cusDomainModels);

            return CreatedAtAction(nameof(GetById), new { petId = cusDomainModels.CusId }, CusDTO);
        }

        [HttpPut]
        [ValidateModeAtrribute]
        [Route("{ID:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid ID, [FromBody] AddCusRequestDTO updatecusRequestDTO)
        {


            var cusDomainModels = mapper.Map<Customer>(updatecusRequestDTO);
            //Check region exis
            cusDomainModels = await cusRepository.UpdateAsync(ID, cusDomainModels);

            if (cusDomainModels == null)
            {
                return NotFound();
            }


            return (Ok(mapper.Map<CustomerDTO>(cusDomainModels)));

        }
    }
}
