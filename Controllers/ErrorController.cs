using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ApiPhoneBook.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet("/error")]
        public IActionResult Error() 
        { 
            var context =HttpContext.Features.Get<IExceptionHandlerFeature>();
            var stackTrace = context.Error.StackTrace;
            var errorMassage = context.Error.Message;

            return Problem();
        }
    }
}
