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
        private readonly ILogger<AccountController> logger;

        public AccountController(PetSpaContext petSpaContext, IAccountRepository accountRepository, IMapper mapper, ILogger<AccountController> logger)
        {
            this.mapper = mapper;
            this.logger = logger;
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

        //Update
        [HttpPut]
        [Route("{AccId:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid AccId, [FromBody] UpdateAccountRequestDTO updateAccountRequestDTO)
        {
            var accountDomainModels = mapper.Map<Account>(updateAccountRequestDTO);
            accountDomainModels = await accountRepository.UpdateAsync(AccId, accountDomainModels);

            if (accountDomainModels == null)
            {
                return NotFound();
            }
            var accountDTO = mapper.Map<AccountDTO>(accountDomainModels);

            return Ok(accountDTO);
        }

        [HttpDelete]
        [Route("{AccId:guid}")]
         public async Task<IActionResult> Delete([FromRoute] Guid AccId)
        {
            var accountDomainModels = await accountRepository.DeleteAsync(AccId);
            if (accountDomainModels == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<AccountDTO>(accountDomainModels));
        }
    }
}
