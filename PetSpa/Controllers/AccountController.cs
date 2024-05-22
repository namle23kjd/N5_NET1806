using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO;
using PetSpa.Repositories;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly PetSpaContext petSpaContext;
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;

        public AccountController(PetSpaContext petSpaContext, IAccountRepository accountRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.petSpaContext = petSpaContext;
            this.accountRepository = accountRepository;
        }
        //: /api/account
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accountDomainModel = await accountRepository.GetALLAsync();
            return Ok(mapper.Map<List<AccountDTO>>(accountDomainModel));
        }


        //Create Account
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AddAccountRequestDTO
            addAccountRequestDTO)
        {
            
                var acccountDomainModels = mapper.Map<Account>(addAccountRequestDTO);

                acccountDomainModels = await accountRepository.CreateAsync(acccountDomainModels);

                var accountDTO = mapper.Map<AccountDTO>(acccountDomainModels);

                return CreatedAtAction(nameof(GetByID), new { AccId = accountDTO.AccId }, accountDTO);
        }
 
            
            
        
      
      
        [HttpGet]
        [Route("{AccId:guid}")]

        public async Task<IActionResult> GetByID([FromRoute] Guid AccId)
        {
            var accountDomain = await accountRepository.GetByIDAsybc(AccId);

            if( accountDomain == null)
            {
                return NotFound();  
            }
            return Ok(mapper.Map<AccountDTO>(accountDomain));
        }

    }
}
