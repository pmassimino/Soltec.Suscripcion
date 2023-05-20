using Microsoft.AspNetCore.Mvc;
using Soltec.Suscripcion.Service;

namespace Soltec.Suscripcion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ControllerBase
    {
        
        ICommonService commonService;        
        private readonly ILogger<WeatherForecastController> _logger;

        public CommonController(ILogger<WeatherForecastController> logger,ICommonService commonService, IConfiguration configuration)
        {
            _logger = logger;
            this.commonService = commonService;
            this.commonService.baseUrl = configuration["Soltec.Sae.Api:UrlService"].ToString();
            this.commonService.ApiKey = configuration["Soltec.Sae.Api:ApiKey"].ToString();
          
        }
        [HttpGet("isRuning")]
        public bool IsRunning()
        {         
         return commonService.IsRunning();

        }
        [HttpGet("CallBack")]
        public IActionResult CallBack()
        {
            return Ok();
        }
    }
}