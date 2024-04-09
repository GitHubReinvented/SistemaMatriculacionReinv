using Application.Persistence;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ReinventedPuembo.Controllers
{
    public class GuardiaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public GuardiaController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Puembo() 
        {
            //Carga los select de los estudiantes
            var estudiantes = await _unitOfWork.Repository<Persona>().GetAsync(x => x.TipoEstudianteId != null && x.SucursalId == 1);
            ViewBag.Estudiantes = estudiantes;

            //Carga datos autorizados
            var autorizado = await _unitOfWork.Repository<Persona>().GetAsync(x => x.AutorizadoRetirar == true && x.SucursalId == 1);
            ViewBag.Autorizados = autorizado;

            return View(); 
        
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerEstudiante(int id)
        {
            var estudiante = await _unitOfWork.Repository<Persona>().GetByIdAsync(id);
            if (estudiante == null)
            {
                return NotFound();
            }
            var includesMatricula = new List<Expression<Func<Matricula, object>>>
            {
                p => p.NivelEscolar!

            };
            var matricula = await _unitOfWork.Repository<Matricula>().GetAsync(x=>x.PersonaId == id,null,includesMatricula);

            var includesRelacion = new List<Expression<Func<Relacion, object>>>
            {
                p => p.TipoRelacion!,
                p => p.Persona2!

            };
            var autorizados = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId == id && x.Persona2!.AutorizadoRetirar == true, null, includesRelacion);
            
            var autorizadosData = autorizados.Select(autorizado => new
            {
                nombres = autorizado.Persona2.Nombres,
                apellidos = autorizado.Persona2.Apellidos,
                identificacion = autorizado.Persona2.Identificacion,
                parentesco = autorizado.TipoRelacion.Descripcion,
                imagenUrl = autorizado.Persona2.Avatar
            }).ToList();

            return Json(new
            {
                estudiantes = new
                {
                    nombres = estudiante.Nombres,
                    apellidos = estudiante.Apellidos,
                    identificacion = estudiante.Identificacion,
                    grado = matricula.FirstOrDefault()!.NivelEscolar!.Descripcion,
                    imagenUrl = estudiante.Avatar,
                },
                autorizados = autorizadosData
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerAutorizadosPorEstudiante(int id)
        {

            var includes = new List<Expression<Func<Relacion, object>>>
            {
                p => p.Persona,
                
                p => p.Persona2,
                p => p.TipoRelacion
             
            };

            var relaciones = await _unitOfWork.Repository<Relacion>().GetAsync(r => r.Persona2Id == id,null,includes);
            var estudianteId = relaciones.FirstOrDefault().PersonaId;

            var includesMatricula = new List<Expression<Func<Matricula, object>>>
            {
                p => p.NivelEscolar,
                p => p.Persona
            };
            var estudiante = await _unitOfWork.Repository<Matricula>().GetAsync(r => r.PersonaId == estudianteId, null, includesMatricula);
            var autorizados = new List<object>();

            var rest = relaciones.Select(relaciones => new
            {
                nombresAutorizado = relaciones.Persona2.Nombres,
                apellidosAutorizado = relaciones.Persona2.Apellidos,
                identificacionAutorizado = relaciones.Persona2.Identificacion,
                imagenAutorizado = relaciones.Persona2.Avatar,
                parentesco = relaciones.TipoRelacion.Descripcion,
                nombresEstudiante = relaciones.Persona.Nombres,
                apellidosEstudiante = relaciones.Persona.Apellidos,
                identificacionEstudiante = relaciones.Persona.Identificacion,
                gradoEstudiante =  _unitOfWork.Repository<Matricula>().GetAsync(r => r.PersonaId == relaciones.PersonaId, null, includesMatricula).Result?.FirstOrDefault()?.NivelEscolar?.Descripcion,  // Obtener el grado del estudiante si es necesario
                imagenUrlEstudiante = relaciones.Persona.Avatar
            }).ToList();


            return Json(new
            {
                data = rest
            }) ;
        }

        public async Task<IActionResult> SantaClara() 
        {
            //Carga los select de los estudiantes
            var estudiantes = await _unitOfWork.Repository<Persona>().GetAsync(x => x.TipoEstudianteId != null && x.SucursalId == 2);
            ViewBag.Estudiantes = estudiantes;

            //Carga datos autorizados
            var autorizado = await _unitOfWork.Repository<Persona>().GetAsync(x => x.AutorizadoRetirar == true && x.SucursalId == 2);
            ViewBag.Autorizados = autorizado;
            return View(); 
        
        }

    }
}
