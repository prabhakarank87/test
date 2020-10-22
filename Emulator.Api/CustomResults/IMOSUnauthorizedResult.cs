namespace Emulator.Api.CustomResults
{
    using Emulator.Contracts.IMOSError;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class IMOSUnauthorizedResult : IActionResult
    {
        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.ContentType = "application/xml";
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var errorModel = new VesonApiError
            {
                Errors = new Errors()
                {
                    Error = new Error
                    {
                        Code = "BAD_REQUEST",
                        Message = "Invalid Api Token"
                    }
                },
                Format = "VESLINK_API_STANDARD"
            };
            await new ObjectResult(errorModel).ExecuteResultAsync(context);
        }
    }
}
