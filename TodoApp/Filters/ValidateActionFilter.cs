using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TodoApp.Dtos.Responses.Base;

namespace TodoApp.Filters
{
    public class ValidateActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var response = new BaseResponseDto<bool>();

            if (!context.ModelState.IsValid)
            {
                var fields = context.ModelState.Keys;

                foreach (var field in fields)
                {
                    var errorCodes = context.ModelState[field].Errors.Select(e => e.ErrorMessage).ToList();

                    var errorDto = new ErrorDto
                    {
                        IsValidationError = true,
                        Field = field
                    };

                    errorDto.Codes.AddRange(errorCodes.Select(e => new ErrorCode
                    {
                        Code = e
                    }));

                    response.Errors.Add(errorDto);
                }

                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}