using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
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
        private readonly ApiResponseService _apiResponseService;

        public AccountController(PetSpaContext petSpaContext, IAccountRepository accountRepository, IMapper mapper, ILogger<AccountController> logger, ApiResponseService apiResponseService)
        {
            this.mapper = mapper;
            this.logger = logger;
            this._apiResponseService = apiResponseService;
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
            switch (addAccountRequestDTO.Role)
            {
                case "Staff":
                    await accountRepository.AddStaffAsync(acccountDomainModels.AccId);
                    break;
                case "Manager":
                    await accountRepository.AddManagerAsync(acccountDomainModels.AccId);
                    break;
                case "Customer":
                    await accountRepository.AddCustomerAsync(acccountDomainModels.AccId);
                    break;
            }

            var accountDTO = mapper.Map<AccountDTO>(acccountDomainModels);

            return _apiResponseService.CreateCreatedResponse(nameof(GetByID), new { AccId = accountDTO.AccId }, accountDTO);
        }
 
               
        [HttpGet]
        [Route("{AccId:guid}")]
        public async Task<IActionResult> GetByID([FromRoute] Guid AccId)
        {
            var accountDomain = await accountRepository.GetByIDAsybc(AccId);

            if (accountDomain == null)
            {
                return _apiResponseService.CreateUnauthorizedResponse();
            }

            var accountDTO = mapper.Map<AccountDTO>(accountDomain);
            return Ok(_apiResponseService.CreateSuccessResponse(accountDTO));
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
                return _apiResponseService.CreateUnauthorizedResponse();
            }
            var accountDTO = mapper.Map<AccountDTO>(accountDomainModels);

            return Ok(_apiResponseService.CreateSuccessResponse(accountDTO));
        }

        [HttpDelete]
        [Route("{AccId:guid}")]
         public async Task<IActionResult> Delete([FromRoute] Guid AccId)
        {
            var accountDomainModels = await accountRepository.DeleteAsync(AccId);
            if (accountDomainModels == null)
            {
                return _apiResponseService.CreateUnauthorizedResponse();
                
            }
            return Ok(mapper.Map<AccountDTO>(accountDomainModels));
        }
    }
}
