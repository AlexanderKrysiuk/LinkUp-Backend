﻿using ErrorOr;
using LinkUpBackend.ServiceErrors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LinkUpBackend.Controllers
{
    public class ApiController : ControllerBase
    {
        protected IActionResult Problem(List<Error> errors)
        {
            if (errors.All(e => e.Type == ErrorType.Validation))
            {
                var modelStateDictionary = new ModelStateDictionary();
                foreach (var error in errors)
                {
                    modelStateDictionary.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem(modelStateDictionary);
            }
            if (errors.Any(e => e.Type == ErrorType.Unexpected))
            {
                return Problem();
            }
            var firstError = errors[0];
            var statusCode = (int)firstError.Type switch
            {
                (int)CustomErrorType.BadRequest => StatusCodes.Status400BadRequest,
                (int)CustomErrorType.Authorization => StatusCodes.Status401Unauthorized,
                (int)ErrorType.NotFound => StatusCodes.Status404NotFound,
                (int)ErrorType.Validation => StatusCodes.Status400BadRequest,
                (int)ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(statusCode: statusCode, title: firstError.Description);
        }
    }
}
