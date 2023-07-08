using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soltec.Suscripcion.Model;
using Soltec.Suscripcion.Service;

namespace Soltec.Suscripcion.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PlanController : Controller
    {
        private IGenericRepository<Plan> repository = null;
        private IGenericRepository<Model.Suscripcion> suscripcionRepository = null;

        public PlanController()
        {
            this.repository = new GenericRepository<Plan>();
            this.suscripcionRepository = new GenericRepository<Model.Suscripcion>();
        }
        [HttpGet]       
        [Route("")]
        public IActionResult GetAll()
        {
            var model = repository.GetAll();
            return Ok(model);
        }
        [HttpGet]
        [Route("{Id}")]
        public ActionResult<Plan> GetById(int Id)
        {
            var result = repository.GetById(Id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] Plan item)
        {
            List<string[]> errorValidacion = new List<string[]>();
            if (string.IsNullOrEmpty(item.Nombre))
            {
                errorValidacion.Add(new string[] { "Nombre", "Ingrese un nombre válido" });
            }
            if (errorValidacion.Count > 0)
            {
                return BadRequest(errorValidacion);
            }
            repository.Insert(item);
            repository.Save();
            return Ok(item);
        }
        [Route("{Id}")]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id,[FromBody] Plan item)
        {
            List<string[]> errorValidacion = new List<string[]>(); ;
            var entity = repository.GetAll().Where(w => w.Id == item.Id).FirstOrDefault();
            if (entity == null)
            {
                errorValidacion.Add(new string[] { "Plan", "No existe un plan con esos parámetros" });
            }
          
            if (string.IsNullOrEmpty(item.Nombre))
            {
                errorValidacion.Add(new string[] { "Nombre", "Ingrese un nombre válido" });
            }
            if (errorValidacion.Count > 0)
            {
                return BadRequest(errorValidacion);
            }
            entity.Nombre = item.Nombre;
            repository.Update(entity);         
            repository.Save();
            return Ok(item);
        }
        [HttpDelete]
        [Route("{Id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<Plan> Delete(int Id)
        {
            List<string[]> errorValidacion = new List<string[]>();
            var result = repository.GetById(Id);
            if (result == null)
            {
                errorValidacion.Add(new string[] { "Plan", "Plan no existe" });
            }
            if (result != null)
            {
                var tieneSus = this.suscripcionRepository.GetAll().Where(w => w.IdPlan == Id).Count();
                if (tieneSus > 0)
                {
                    errorValidacion.Add(new string[] { "Plan", "El plan esta asociado a algúna suscripcion, no se puede eliminar" });
                }
            }
                if (errorValidacion.Count > 0)
                {
                    return BadRequest(errorValidacion);
                }

                repository.Delete(Id);
                repository.Save();
                return Ok(result);
            }


        }
    }

