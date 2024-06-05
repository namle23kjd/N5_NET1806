using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Pet;
using PetSpa.Repositories;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly PetSpaContext petSpaContext;
        private readonly IPetRepository petRepository;
        private readonly IMapper mapper;

        public PetController(PetSpaContext petSpaContext,IPetRepository petRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.petSpaContext = petSpaContext;
            this.petRepository = petRepository;
        }
        [HttpGet]
        [ValidateModeAtrribute]
        public async Task<IActionResult> GetAll()
        {
            var petDomainModel = await petRepository.GetALLAsync();
            return Ok(mapper.Map<List<PetDTO>>(petDomainModel));
        }

        [HttpGet]
        [ValidateModeAtrribute]
        [Route("{ID:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid ID)
        {
           
            var comment = await petRepository.getByIdAsync(ID);

            if (comment == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<PetDTO>(comment));
        }

        [HttpPost]
        
        [Route("{ID:guid}")]
        public async Task<IActionResult> Create( Guid ID, [FromBody] AddPetRequestDTO AddPetRequestDTO)
        {
            //if (false)//check id cus có tồn tại không
            //{
            //   return badrequest("stock does not exist");
            //}
            

            var petDomainModels = mapper.Map<Pet>(AddPetRequestDTO);
            petDomainModels.CusId = ID;
            petDomainModels.PetId = new Guid();
            petDomainModels.Status = true;
            petDomainModels = await petRepository.CreateAsync(petDomainModels);
            

            var accountDTO = mapper.Map<PetDTO>(petDomainModels);

            return CreatedAtAction(nameof(GetById), new { petId = petDomainModels.PetId}, accountDTO);
        }
        [HttpDelete]

        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var petModel = await petRepository.DeleteAsync(id);
            if (petModel == null)
            {
                return NotFound("Comment does not exist");
            }
            return Ok(Ok(mapper.Map<PetDTO>(petModel)));
        }


        [HttpPut]
        [ValidateModeAtrribute]
        [Route("{ID:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid ID, [FromBody] UpdatePetRequestDTO updatePetRequestDTO)
        {


            var regionDomainModels = mapper.Map<Pet>(updatePetRequestDTO);
            //Check region exist
            regionDomainModels = await petRepository.UpdateAsync(ID, regionDomainModels);

            if (regionDomainModels == null)
            {
                return NotFound();
            }


            return (Ok(mapper.Map<PetDTO>(regionDomainModels)));

        }
    }
}
