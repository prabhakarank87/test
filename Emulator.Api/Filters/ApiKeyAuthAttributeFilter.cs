namespace Emulator.Api.Filters
{
    using Emulator.Bll;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Options;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string KeyParamName = "apitoken";
        private const string RootParamName = "CompanyCode";
        private readonly IOptions<EmulatorSettings> _options;
        public ApiKeyAuthAttribute(IOptions<EmulatorSettings> options)
        {
            _options = options;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            var principle = request.RouteValues[RootParamName].ToString();
            var authKey = GetRequestApiKeyValue(request);
            if (string.IsNullOrEmpty(authKey)
                || string.IsNullOrEmpty(principle)
                || !_options.Value.ApiKeys.Any(x => x.Key == principle)
                || !_options.Value.ApiKeys.Where(x => x.Key == principle && x.Value == authKey).Any())
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
        }

        private string GetRequestApiKeyValue(HttpRequest request)
        {
            return request.Query[KeyParamName].Any() ? request.Query[KeyParamName].First() : null;
        }
    }
}
