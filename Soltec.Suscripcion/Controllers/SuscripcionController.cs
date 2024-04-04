using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soltec.Suscripcion.Service;
using Soltec.Suscripcion.Model;
using Soltec.Suscripcion.Code;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Soltec.Suscripcion.Controllers
{
    [ApiController]  
    [Route("api/[controller]")]
    public class SuscripcionController : ControllerBase
    {

        ICommonService commonService;
        ICtaCteService ctaCteService;
        private readonly ILogger<WeatherForecastController> _logger;
        private IGenericRepository<Model.Suscripcion> repository = null;
        private IGenericRepository<Model.Usuario> usuarioRepository;
        private IGenericRepository<Model.Plan> planRepository = null;
        private ISujetoService sujetoService;
        private ISessionService sessionService;          
        private readonly IMemoryCache cache;
        IConfiguration configuration;
        private IWebHostEnvironment hostingEnvironment;


        public SuscripcionController(ILogger<WeatherForecastController> logger, ICommonService commonService, IConfiguration configuration, ICtaCteService ctaCteService, ISujetoService sujetoService,ISessionService sessionService, IMemoryCache memoryCache, IWebHostEnvironment environment)
        {
            _logger = logger;
            this.commonService = commonService;
            this.configuration = configuration;
            this.commonService.baseUrl = configuration["Soltec.Sae.Api:UrlService"].ToString();
            this.commonService.ApiKey = configuration["Soltec.Sae.Api:ApiKey"].ToString();
            this.ctaCteService = ctaCteService;
            this.ctaCteService.baseUrl = configuration["Soltec.Sae.Api:UrlService"].ToString();
            this.ctaCteService.ApiKey = configuration["Soltec.Sae.Api:ApiKey"].ToString();
            this.sujetoService = sujetoService;
            this.sujetoService.baseUrl = configuration["Soltec.Sae.Api:UrlService"].ToString();
            this.sujetoService.ApiKey = configuration["Soltec.Sae.Api:ApiKey"].ToString();
            this.repository = new GenericRepository<Model.Suscripcion>();
            this.planRepository = new GenericRepository<Model.Plan>();
            this.usuarioRepository = new GenericRepository<Model.Usuario>();
            this.sessionService = sessionService;
            this.hostingEnvironment = environment;
            this.cache = memoryCache;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] Model.Suscripcion item)
        {
            List<string[]> errorValidacion = new List<string[]>();
            var sujeto = sujetoService.FindOne(item.IdCuenta);
            if (sujeto == null) 
            {
                errorValidacion.Add(new string[] { "Sujeto", "Ingrese un cliente válido" });
            }
            var plan = planRepository.GetById(item.IdPlan);
            if (plan == null)
            {
                errorValidacion.Add(new string[] { "Plan", "Ingrese un plan válido" });
            }
           var suscripcion = repository.GetAll().Where(w=>w.IdCuenta==item.IdCuenta  && w.IdPlan == item.IdPlan).FirstOrDefault();
            if (suscripcion != null) 
            {
                errorValidacion.Add(new string[] { "Suscripcion", "Ya existe una suscripcion con esos parámetros" });
            }
            if (item.Importe < 0)
            {
                errorValidacion.Add(new string[] { "Importe", "Importe debe ser mayor a cero" });
            }
            if (errorValidacion.Count > 0)
            {
                return BadRequest(errorValidacion);
            }
            repository.Insert(item);
            repository.Save();
            return Ok(item);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{Id}")]
        public IActionResult Edit(int id,[FromBody] Model.Suscripcion item)
        {
            List<string[]> errorValidacion = new List<string[]>();
            var sujeto = sujetoService.FindOne(item.IdCuenta);
            if (sujeto == null)
            {
                errorValidacion.Add(new string[] { "Sujeto", "Ingrese un cliente válido" });
            }
            var plan = planRepository.GetById(item.IdPlan);
            if (plan == null)
            {
                errorValidacion.Add(new string[] { "Plan", "Ingrese un plan válido" });
            }         
            var entity = repository.GetAll().Where(w => w.Id == item.Id).FirstOrDefault();
            if (entity == null)
            {
                errorValidacion.Add(new string[] { "Suscripcion", "No existe una suscripcion con esos parámetros" });
            }
            var suscripcion = repository.GetAll().Where(w => w.IdCuenta == item.IdCuenta && w.IdPlan == item.IdPlan && w.Id !=id).FirstOrDefault();
            if (suscripcion != null)
            {
                errorValidacion.Add(new string[] { "Suscripcion", "Ya existe una suscripcion con esos parámetros" });
            }
            if (new string[]{ "ACTIVO","AVISO", "SUSPENDIDO"}.Contains(item.Estado) == false)
            {
                errorValidacion.Add(new string[] { "Estado", "Estado no válido" });
            }
            if (item.Importe < 0)
            {
                errorValidacion.Add(new string[] { "Importe", "Importe debe ser mayor a cero" });
            }
            if (errorValidacion.Count > 0)
            {
                return BadRequest(errorValidacion);
            }           
            entity.IdCuenta = item.IdCuenta;            
            entity.Estado = item.Estado;
            entity.IdPlan = item.IdPlan;
            entity.Importe = item.Importe;            

            repository.Update(entity);
            repository.Save();
            return Ok(entity);
        }
        [HttpDelete]
        [Route("{Id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<Model.Suscripcion> Delete(int Id)
        {
            List<string[]> errorValidacion = new List<string[]>();
            var result = repository.GetById(Id);
            if (result == null)
            {
                errorValidacion.Add(new string[] { "Plan", "Plan no existe" });
            }            
            if (errorValidacion.Count > 0)
            {
                return BadRequest(errorValidacion);
            }
            repository.Delete(Id);
            repository.Save();
            return Ok(result);
        }
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var model = repository.GetAll();
            return Ok(model);
        }
        
        [HttpGet]
        [Route("view")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllView()
        {
            if (!cache.TryGetValue("tmpSujetos", out IList<Sujeto> tmpSujetos))
            {
                // Si el valor no está en la caché, obtenerlo de una fuente de datos
                tmpSujetos = sujetoService.List();

                // Almacenar en caché el valor obtenido con un tiempo de expiración de 10 minutos
                cache.Set("tmpSujetos",  tmpSujetos, TimeSpan.FromMinutes(10));
            }
            var tmpresult = repository.GetAll().Include(i=>i.Plan).ToList();
            
            var result = from s in tmpresult
                         join suj in tmpSujetos on s.IdCuenta equals suj.Id
                         orderby suj.Nombre,suj.Id
                         select new { id=s.Id,idCuenta = s.IdCuenta, nombre = suj.Nombre, idPlan = s.IdPlan,plan=s.Plan.Nombre,  importe = s.Importe, estado = s.Estado };
            
            return Ok(result.ToList());
        }
        [HttpGet]
        [Route("{Id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            var model = repository.GetById(id);
            return Ok(model);
        }
        [HttpGet]
        [Route("estado")]
        [Authorize]
        public IActionResult Estado()
        {
            int idUsuario = sessionService.IdUsuario;
            var usuario = this.usuarioRepository.GetAll().Include(i => i.Roles).Include(c => c.Cuentas).Where(w=>w.Id==idUsuario).FirstOrDefault();
            if (usuario.Cuentas.Count() == 0) 
            {
                return BadRequest("El usuario no tiene cuentas asignadas");
            }
            var tmpSuscripcion = repository.GetAll().ToList().Where(s=>usuario.Cuentas.Any(c=>c.IdCuenta ==s.IdCuenta)).FirstOrDefault();
            var result = new EstadoSuscripcion();
            if (tmpSuscripcion != null) 
            {
                result.Estado = tmpSuscripcion.Estado;
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("resumenCtaCte")]
        public IActionResult ResumenCtaCte()
        {
            int idUsuario = sessionService.IdUsuario;
            var usuario = this.usuarioRepository.GetAll().Include(i => i.Roles).Include(c => c.Cuentas).Where(w => w.Id == idUsuario).FirstOrDefault();
            if (usuario.Cuentas.Count() == 0)
            {
                return BadRequest("El usuario no tiene cuentas asignadas");
            }
            var tmpSuscripcion = repository.GetAll().ToList().Where(s => usuario.Cuentas.Any(c => c.IdCuenta == s.IdCuenta)).FirstOrDefault();
            
            if (tmpSuscripcion == null)
            {
                return BadRequest("No tiene suscripcion activas");
            }
            string idCuentaMayor = this.configuration["IdCuentaMayor"].ToString();
            string idCuenta = tmpSuscripcion.IdCuenta;
            DateTime fecha = DateTime.Now.AddDays(-60);
            DateTime fechaHasta = DateTime.Now;
            var movCtaCte = ctaCteService.List(idCuenta,idCuentaMayor,fecha,fechaHasta);
            var sujeto = this.sujetoService.FindOne(idCuenta);
            CtaCteReportTemplate template = new CtaCteReportTemplate();
            template.FechaDesde = fecha;
            template.FechaHasta = fechaHasta;
            template.Sujeto = sujeto;
            template.MovCtaCte = movCtaCte;
            template.NombreEmpresa = "Soltec S.A.S";
            template.Path = hostingEnvironment.ContentRootPath ;
            var result = template.ListPDF();
            return new FileStreamResult(result, "application/pdf") { FileDownloadName = "ResumenCtaCte.pdf" };


            //return Ok(result);
        }
        public class EstadoSuscripcion 
        {
            public string Estado { get; set; } = "";
            public string Mensage { get; set; } = "";
        }


    }
}
