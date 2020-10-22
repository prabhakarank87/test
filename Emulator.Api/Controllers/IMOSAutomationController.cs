namespace Emulator.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Emulator.Bll;

    [Route("[controller]")]
    [ApiController]
    public class IMOSAutomationController : ControllerBase
    {
        private readonly IOptions<EmulatorSettings> _options;
        public IMOSAutomationController(IOptions<EmulatorSettings> options)
        {
            _options = options;
        }

        [HttpGet("{companyCode}")]
        public string PortCallList(int arrivalFromDays, int arrivalToDays, string apiToken)
        {
            return "Port Call List";
        }

        [HttpGet("{companyCode}/details")]
        public string PortCallDetails(string vesselCode, int voyageNo, int portCallSeq, string apiToken)
        {
            return "Port call data";
        }
    }
}
