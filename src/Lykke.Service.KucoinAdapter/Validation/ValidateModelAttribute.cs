using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Service.KucoinAdapter.Validation
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var error = context.ModelState.SelectMany(x => x.Value.Errors).FirstOrDefault();

                var msg = error?.ErrorMessage ?? error?.Exception?.Message ?? "Validation error";

                context.Result = new BadRequestObjectResult(msg);
            }
        }
    }
}
