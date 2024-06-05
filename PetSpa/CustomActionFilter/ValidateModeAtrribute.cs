using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PetSpa.CustomActionFilter
{
    public class ValidateModeAtrribute : ActionFilterAttribute
    {

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                if (UseCustomResponse)
                {
                    var errorDetails = new
                    {
                        Message = CustomErrorMessage,
                        Errors = context.ModelState
                            .Where(ms => ms.Value.Errors.Count > 0)
                            .Select(ms => new { Field = ms.Key, Errors = ms.Value.Errors.Select(e => e.ErrorMessage) })
                    };

                    context.Result = new BadRequestObjectResult(errorDetails);
                }
                else
                {
                    context.Result = new BadRequestResult();
                }
                context.Result = new BadRequestResult();

            }
        }
    }
}
