using Microsoft.AspNetCore.Mvc;
using PetSpa.Models.DTO.RegisterDTO;

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
        public ApiResponse<T> LoginSuccessResponse<T>(T data, string message = "Succeeded")
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
        public IActionResult CreateBadRequestResponse(string message)
        {
            return CreateResponse(false, (object?)null, message, 400);
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
        public IActionResult CreateConflictResponse(string message)
        {
            return CreateResponse(false, (object?)null, message, 409);
        }

        public IActionResult CreateCreatedResponse<T>(string actionName, object routeValues, T data)
        {
            var response = CreateSuccessResponse(data);
            return new CreatedAtActionResult(actionName, null, routeValues, response);
        }

        public IActionResult CreateInternalServerErrorResponse(string message = "An error occurred on the server")
        {
            return CreateResponse(false, (object?)null, message, 500);
        }

        public IActionResult CreateServiceUnavailableResponse(string message = "Service is unavailable")
        {
            return CreateResponse(false, (object?)null, message, 503);
        }

    }
}

