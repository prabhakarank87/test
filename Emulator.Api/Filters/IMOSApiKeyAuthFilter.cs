namespace Emulator.Api.Filters
{
    using Emulator.Api.CustomResults;
    using Emulator.Bll;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Options;
    using System.Linq;

    public class IMOSApiKeyAuthFilter : IAuthorizationFilter
    {
        private const string KeyParamName = "apitoken";
        private const string RootParamName = "CompanyCode";
        private readonly IOptions<EmulatorSettings> _options;

        public IMOSApiKeyAuthFilter(IOptions<EmulatorSettings> options)
        {
            _options = options;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var principle = request.RouteValues[RootParamName].ToString();
            var authKey = GetRequestApiKeyValue(request);
            if (string.IsNullOrEmpty(authKey)
                || string.IsNullOrEmpty(principle)
                || !_options.Value.ApiKeys.Any(x => x.Key == principle)
                || !_options.Value.ApiKeys.Where(x => x.Key == principle && x.Value == authKey).Any())
            {
                ProcessFailedAuthorization(context);
            }
        }
        private string GetRequestApiKeyValue(HttpRequest request)
        {
            return request.Query[KeyParamName].Any() ? request.Query[KeyParamName].First() : null;
        }
        private void ProcessFailedAuthorization(AuthorizationFilterContext context)
        {
            context.Result = new IMOSUnauthorizedResult();
        }

    }
}
