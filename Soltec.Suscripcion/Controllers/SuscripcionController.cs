using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soltec.Suscripcion.Code;
using Soltec.Suscripcion.Model;
using Soltec.Suscripcion.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

#nullable enable
namespace Soltec.Suscripcion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuscripcionController : ControllerBase
    {
        private ICommonService commonService;
        private ICtaCteService ctaCteService;
        private readonly ILogger<WeatherForecastController> _logger;
        private IGenericRepository<Soltec.Suscripcion.Model.Suscripcion> repository;
        private IGenericRepository<Usuario> usuarioRepository;
        private IGenericRepository<Plan> planRepository;
        private ISujetoService sujetoService;
        private ISessionService sessionService;
        private readonly IMemoryCache cache;
        private IConfiguration configuration;
        private IWebHostEnvironment hostingEnvironment;

        public SuscripcionController(
          ILogger<WeatherForecastController> logger,
          ICommonService commonService,
          IConfiguration configuration,
          ICtaCteService ctaCteService,
          ISujetoService sujetoService,
          ISessionService sessionService,
          IMemoryCache memoryCache,
          IWebHostEnvironment environment)
        {
            this._logger = logger;
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
            this.repository = (IGenericRepository<Soltec.Suscripcion.Model.Suscripcion>)new GenericRepository<Soltec.Suscripcion.Model.Suscripcion>();
            this.planRepository = (IGenericRepository<Plan>)new GenericRepository<Plan>();
            this.usuarioRepository = (IGenericRepository<Usuario>)new GenericRepository<Usuario>();
            this.sessionService = sessionService;
            this.hostingEnvironment = environment;
            this.cache = memoryCache;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] Soltec.Suscripcion.Model.Suscripcion item)
        {
            List<string[]> error = new List<string[]>();
            if (this.sujetoService.FindOne(item.IdCuenta) == null)
                error.Add(new string[2]
                {
          "Sujeto",
          "Ingrese un cliente válido"
                });
            if (this.planRepository.GetById((object)item.IdPlan) == null)
                error.Add(new string[2]
                {
          "Plan",
          "Ingrese un plan válido"
                });
            if (this.repository.GetAll().Where<Soltec.Suscripcion.Model.Suscripcion>((Expression<Func<Soltec.Suscripcion.Model.Suscripcion, bool>>)(w => w.IdCuenta == item.IdCuenta && w.IdPlan == item.IdPlan)).FirstOrDefault<Soltec.Suscripcion.Model.Suscripcion>() != null)
                error.Add(new string[2]
                {
          "Suscripcion",
          "Ya existe una suscripcion con esos parámetros"
                });
            if (item.Importe < 0M)
                error.Add(new string[2]
                {
          "Importe",
          "Importe debe ser mayor a cero"
                });
            if (error.Count > 0)
                return (IActionResult)this.BadRequest((object)error);
            this.repository.Insert(item);
            this.repository.Save();
            return (IActionResult)this.Ok((object)item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{Id}")]
        public IActionResult Edit(int id, [FromBody] Soltec.Suscripcion.Model.Suscripcion item)
        {
            List<string[]> error = new List<string[]>();
            if (this.sujetoService.FindOne(item.IdCuenta) == null)
                error.Add(new string[2]
                {
          "Sujeto",
          "Ingrese un cliente válido"
                });
            if (this.planRepository.GetById((object)item.IdPlan) == null)
                error.Add(new string[2]
                {
          "Plan",
          "Ingrese un plan válido"
                });
            Soltec.Suscripcion.Model.Suscripcion suscripcion = this.repository.GetAll().Where<Soltec.Suscripcion.Model.Suscripcion>((Expression<Func<Soltec.Suscripcion.Model.Suscripcion, bool>>)(w => w.Id == item.Id)).FirstOrDefault<Soltec.Suscripcion.Model.Suscripcion>();
            if (suscripcion == null)
                error.Add(new string[2]
                {
          "Suscripcion",
          "No existe una suscripcion con esos parámetros"
                });
            if (this.repository.GetAll().Where<Soltec.Suscripcion.Model.Suscripcion>((Expression<Func<Soltec.Suscripcion.Model.Suscripcion, bool>>)(w => w.IdCuenta == item.IdCuenta && w.IdPlan == item.IdPlan && w.Id != id)).FirstOrDefault<Soltec.Suscripcion.Model.Suscripcion>() != null)
                error.Add(new string[2]
                {
          "Suscripcion",
          "Ya existe una suscripcion con esos parámetros"
                });
            if (!((IEnumerable<string>)new string[3]
            {
        "ACTIVO",
        "AVISO",
        "SUSPENDIDO"
            }).Contains<string>(item.Estado))
                error.Add(new string[2]
                {
          "Estado",
          "Estado no válido"
                });
            if (item.Importe < 0M)
                error.Add(new string[2]
                {
          "Importe",
          "Importe debe ser mayor a cero"
                });
            if (error.Count > 0)
                return (IActionResult)this.BadRequest((object)error);
            suscripcion.IdCuenta = item.IdCuenta;
            suscripcion.Estado = item.Estado;
            suscripcion.IdPlan = item.IdPlan;
            suscripcion.Importe = item.Importe;
            this.repository.Update(suscripcion);
            this.repository.Save();
            return (IActionResult)this.Ok((object)suscripcion);
        }

        [HttpDelete]
        [Route("{Id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<Soltec.Suscripcion.Model.Suscripcion> Delete(int Id)
        {
            List<string[]> error = new List<string[]>();
            Soltec.Suscripcion.Model.Suscripcion byId = this.repository.GetById((object)Id);
            if (byId == null)
                error.Add(new string[2] { "Plan", "Plan no existe" });
            if (error.Count > 0)
                return (ActionResult<Soltec.Suscripcion.Model.Suscripcion>)(ActionResult)this.BadRequest((object)error);
            this.repository.Delete((object)Id);
            this.repository.Save();
            return (ActionResult<Soltec.Suscripcion.Model.Suscripcion>)(ActionResult)this.Ok((object)byId);
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll() => (IActionResult)this.Ok((object)this.repository.GetAll());

        [HttpGet]
        [Route("view")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllView()
        {
            IList<Sujeto> inner;
            if (!this.cache.TryGetValue<IList<Sujeto>>((object)"tmpSujetos", out inner))
            {
                inner = this.sujetoService.List();
                this.cache.Set<IList<Sujeto>>((object)"tmpSujetos", inner, TimeSpan.FromMinutes(10.0));
            }
            return (IActionResult)this.Ok((object)this.repository.GetAll().Include<Soltec.Suscripcion.Model.Suscripcion, Plan>((Expression<Func<Soltec.Suscripcion.Model.Suscripcion, Plan>>)(i => i.Plan)).ToList<Soltec.Suscripcion.Model.Suscripcion>().Join((IEnumerable<Sujeto>)inner, (Func<Soltec.Suscripcion.Model.Suscripcion, string>)(s => s.IdCuenta), (Func<Sujeto, string>)(suj => suj.Id), (s, suj) => new
            {
                s = s,
                suj = suj
            }).OrderBy(_param1 => _param1.suj.Nombre).ThenBy(_param1 => _param1.suj.Id).Select(_param1 => new
            {
                id = _param1.s.Id,
                idCuenta = _param1.s.IdCuenta,
                nombre = _param1.suj.Nombre,
                idPlan = _param1.s.IdPlan,
                plan = _param1.s.Plan.Nombre,
                importe = _param1.s.Importe,
                estado = _param1.s.Estado
            }).ToList());
        }

        [HttpGet]
        [Route("{Id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetById(int id)
        {
            return (IActionResult)this.Ok((object)this.repository.GetById((object)id));
        }

        [HttpGet]
        [Route("estado")]
        [Authorize]
        public IActionResult Estado()
        {
            int idUsuario = this.sessionService.IdUsuario;
            Usuario usuario = this.usuarioRepository.GetAll().Include<Usuario, IList<UsuarioRol>>((Expression<Func<Usuario, IList<UsuarioRol>>>)(i => i.Roles)).Include<Usuario, IList<UsuarioCuenta>>((Expression<Func<Usuario, IList<UsuarioCuenta>>>)(c => c.Cuentas)).Where<Usuario>((Expression<Func<Usuario, bool>>)(w => w.Id == idUsuario)).FirstOrDefault<Usuario>();
            if (usuario.Cuentas.Count<UsuarioCuenta>() == 0)
                return (IActionResult)this.BadRequest((object)"El usuario no tiene cuentas asignadas");
            Soltec.Suscripcion.Model.Suscripcion suscripcion = this.repository.GetAll().ToList<Soltec.Suscripcion.Model.Suscripcion>().Where<Soltec.Suscripcion.Model.Suscripcion>((Func<Soltec.Suscripcion.Model.Suscripcion, bool>)(s => usuario.Cuentas.Any<UsuarioCuenta>((Func<UsuarioCuenta, bool>)(c => c.IdCuenta == s.IdCuenta)))).FirstOrDefault<Soltec.Suscripcion.Model.Suscripcion>();
            SuscripcionController.EstadoSuscripcion estadoSuscripcion = new SuscripcionController.EstadoSuscripcion();
            if (suscripcion != null)
                estadoSuscripcion.Estado = suscripcion.Estado;
            return (IActionResult)this.Ok((object)estadoSuscripcion);
        }

        [HttpGet]
        [Route("resumenCtaCte")]
        public IActionResult ResumenCtaCte()
        {
            int idUsuario = this.sessionService.IdUsuario;
            Usuario usuario = this.usuarioRepository.GetAll().Include<Usuario, IList<UsuarioRol>>((Expression<Func<Usuario, IList<UsuarioRol>>>)(i => i.Roles)).Include<Usuario, IList<UsuarioCuenta>>((Expression<Func<Usuario, IList<UsuarioCuenta>>>)(c => c.Cuentas)).Where<Usuario>((Expression<Func<Usuario, bool>>)(w => w.Id == idUsuario)).FirstOrDefault<Usuario>();
            if (usuario.Cuentas.Count<UsuarioCuenta>() == 0)
                return (IActionResult)this.BadRequest((object)"El usuario no tiene cuentas asignadas");
            Soltec.Suscripcion.Model.Suscripcion suscripcion = this.repository.GetAll().ToList<Soltec.Suscripcion.Model.Suscripcion>().Where<Soltec.Suscripcion.Model.Suscripcion>((Func<Soltec.Suscripcion.Model.Suscripcion, bool>)(s => usuario.Cuentas.Any<UsuarioCuenta>((Func<UsuarioCuenta, bool>)(c => c.IdCuenta == s.IdCuenta)))).FirstOrDefault<Soltec.Suscripcion.Model.Suscripcion>();
            if (suscripcion == null)
                return (IActionResult)this.BadRequest((object)"No tiene suscripcion activas");
            string idCuentaMayor = this.configuration["IdCuentaMayor"].ToString();
            string idCuenta = suscripcion.IdCuenta;
            DateTime fecha = DateTime.Now.AddDays(-60.0);
            DateTime now = DateTime.Now;
            IList<MovCtaCte> movCtaCteList = this.ctaCteService.List(idCuenta, idCuentaMayor, fecha, now);
            Sujeto one = this.sujetoService.FindOne(idCuenta);
            FileStreamResult fileStreamResult = new FileStreamResult((Stream)new CtaCteReportTemplate()
            {
                FechaDesde = fecha,
                FechaHasta = now,
                Sujeto = one,
                MovCtaCte = movCtaCteList,
                NombreEmpresa = "Soltec S.A.S",
                Path = this.hostingEnvironment.ContentRootPath
            }.ListPDF(), "application/pdf");
            fileStreamResult.FileDownloadName = "ResumenCtaCte.pdf";
            return (IActionResult)fileStreamResult;
        }

        public class EstadoSuscripcion
        {
            public string Estado { get; set; } = "";

            public string Mensage { get; set; } = "";
        }
    }
}
