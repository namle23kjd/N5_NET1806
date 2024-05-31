using Microsoft.AspNetCore.Mvc;
using PetSpa.Models.DTO.Respone;

namespace PetSpa.CustomActionFilter
{
    public class ApiResponseService
    {
        public ApiResponse<T> CreateSuccessResponse<T>(T data, string message = "Succeeded")
        {
            return new ApiResponse<T>
            {
                Status = true,
                Msg = message,
                Data = data
            };
        }

        public ApiResponse<object> CreateErrorResponse(string message)
        {
            return new ApiResponse<object>
            {
                Status = false,
                Msg = message,
                Data = null
            };
        }

        public IActionResult CreateResponse<T>(bool isSuccess, T data, string message, int statusCode)
        {
            var response = new ApiResponse<T>
            {
                Status = isSuccess,
                Msg = message,
                Data = isSuccess ? data : default
            };
            return new ObjectResult(response) { StatusCode = statusCode };
        }

        public IActionResult CreateUnauthorizedResponse()
        {
            return CreateResponse(false, (object?)null, "Unauthorized access", 401);
        }

        public IActionResult CreatePaymentRequiredResponse()
        {
            return CreateResponse(false, (object?)null, "Payment required", 402);
        }

        public IActionResult CreatePaymentNotFound()
        {
            return CreateResponse(false, (object?)null, "Payment required", 404);
        }

        public IActionResult CreateCreatedResponse<T>(string actionName, object routeValues, T data)
        {
            var response = CreateSuccessResponse(data);
            return new CreatedAtActionResult(actionName, null, routeValues, response);
        }
    }

}
