using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Soltec.Suscripcion.Model;
using Soltec.Suscripcion.Service;

namespace Soltec.Suscripcion.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SujetoController : ControllerBase
    {

        ICommonService commonService;
        ISujetoService service;
        private readonly IMemoryCache cache;

        private readonly ILogger<WeatherForecastController> _logger;

        public SujetoController(ILogger<WeatherForecastController> logger, ICommonService commonService, IConfiguration configuration, ISujetoService service, IMemoryCache memoryCache)
        {
            _logger = logger;
            this.commonService = commonService;
            this.commonService.baseUrl = configuration["Soltec.Sae.Api:UrlService"].ToString();
            this.commonService.ApiKey = configuration["Soltec.Sae.Api:ApiKey"].ToString();
            this.service = service;
            this.service.baseUrl = configuration["Soltec.Sae.Api:UrlService"].ToString();
            this.service.ApiKey = configuration["Soltec.Sae.Api:ApiKey"].ToString();
            this.cache = memoryCache;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("")]
        public IActionResult List()
        {
            if (!cache.TryGetValue("tmpSujetos", out IList<Sujeto> model))
            {
                // Si el valor no está en la caché, obtenerlo de una fuente de datos
                model = service.List().OrderBy(o => o.Nombre).ToList();

                // Almacenar en caché el valor obtenido con un tiempo de expiración de 10 minutos
                cache.Set("tmpSujetos", model, TimeSpan.FromMinutes(1));
            }
           
            return Ok(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("{id}")]
        public IActionResult ListOne(string id)
        {
            var model = service.FindOne(id);
            if (model == null) 
            {
                return NotFound();
            }
            return Ok(model);
        }
    }
}
