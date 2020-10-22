namespace Emulator.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Emulator.Api.Filters;
    using Microsoft.Extensions.Options;
    using Emulator.Bll;

    [Route("v1/agent/portcalls")]
    [ApiController]
    [ServiceFilter(typeof(IMOSApiKeyAuthFilter))]
    public class IMOSController : ControllerBase
    {
        private readonly IOptions<EmulatorSettings> _options;
        public IMOSController(IOptions<EmulatorSettings> options)
        {
            _options = options;
        }

        [HttpGet("{companyCode}")]
        [Produces("application/xml")]
        public string PortCallList(int arrivalFromDays, int arrivalToDays, string apiToken)
        {
            return "Port Call List";            
        }

        [HttpGet("{companyCode}/details")]
        [Produces("application/xml")]
        public string PortCallDetails(string vesselCode, int voyageNo, int portCallSeq, string apiToken)
        {
            return "Port call data";
        }
    }
}
