using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Pet;
using PetSpa.Repositories;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetRepository _petRepository;
        private readonly IMapper _mapper;

        public PetController(IPetRepository petRepository, IMapper mapper)
        {
            _petRepository = petRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ValidateModeAtrribute]
        public async Task<IActionResult> GetAll()
        {
            var petDomainModels = await _petRepository.GetALLAsync();
            return Ok(_mapper.Map<List<PetDTO>>(petDomainModels));
        }

        [HttpGet]
        [ValidateModeAtrribute]
        [Route("{ID:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid ID)
        {
            var pet = await _petRepository.GetByIdAsync(ID);

            if (pet == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PetDTO>(pet));
        }

        [HttpPost]
        [ValidateModeAtrribute]
        public async Task<IActionResult> Create([FromBody] AddPetRequestDTO addPetRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var petDomainModel = _mapper.Map<Pet>(addPetRequestDTO);
            petDomainModel.PetId = Guid.NewGuid();

            var createdPet = await _petRepository.CreateAsync(petDomainModel);

            var petDTO = _mapper.Map<PetDTO>(createdPet);

            return CreatedAtAction(nameof(GetById), new { ID = petDTO.PetId }, petDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var pet = await _petRepository.DeleteAsync(id);

            if (pet == null)
            {
                return NotFound("Pet does not exist");
            }

            return Ok(_mapper.Map<PetDTO>(pet));
        }

        [HttpPut]
        [ValidateModeAtrribute]
        [Route("{ID:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid ID, [FromBody] UpdatePetRequestDTO updatePetRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPet = await _petRepository.GetByIdAsync(ID);
            if (existingPet == null)
            {
                return NotFound();
            }

            var petDomainModel = _mapper.Map(updatePetRequestDTO, existingPet);

            var updatedPet = await _petRepository.UpdateAsync(ID, petDomainModel);

            return Ok(_mapper.Map<PetDTO>(updatedPet));
        }
    }
}
