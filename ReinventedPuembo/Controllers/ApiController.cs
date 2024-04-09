using Application.Models.Authorization;
using Application.Models;
using Application.Persistence;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Identity;
using AutoMapper;
using System.Linq.Expressions;
using ReinventedPuembo.Models;
using System.Linq.Dynamic;
namespace ReinventedPuembo.Controllers
{
    [Route("api/reinvented")]
    public class ApiController : ControllerBase
    {
        public string draw = "";
        public string start = "";
        public string length = "";
        public string sortColumn = "";
        public string sortColumnDir = "";
        public string searchValue = "";

        public int pageSize, skip, recordsTotal;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly UserManager<Usuario> _userManager;

        public ApiController(IUnitOfWork unitOfWork,
            IAuthService authService,
            IMapper mapper,
            UserManager<Usuario> userManager)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("tipoIdentificacion")]
        public async Task<IActionResult> TipoIndentificacion()
        {
            var tiposIdentificacion = await _unitOfWork.Repository<TipoIdentificacion>().GetAllAsync();
            if (tiposIdentificacion is null)
            {
                return NotFound();
            }

            return Ok(tiposIdentificacion);
        }


        [HttpGet("tipoCargo")]
        public async Task<IActionResult> TipoCargo()
        {
            var profesiones = await _unitOfWork.Repository<Profesion>().GetAllAsync();
            if (profesiones is null)
            {
                return NotFound();
            }

            return Ok(profesiones);
        }


        [HttpPost("register/{tipo}", Name = "Register")]
        public async Task<IActionResult> Register(string tipo, [FromForm] PersonaMap persona)
        {
            try
            {
                var currentUser = _authService.GetSessionUser();

                var user = await _userManager.FindByIdAsync(currentUser);

                var personaEntity = _mapper.Map<Persona>(persona);

                if (tipo.Equals(Role.REPRESENTANTE.ToLower()))
                {
                    personaEntity.Representante = true;
                    personaEntity.UsuarioId = user!.Id;
                    personaEntity.TipoEstudianteId = null;
                }

                _unitOfWork.Repository<Persona>().AddEntity(personaEntity);

                var result = await _unitOfWork.Complete();
                if (result <= 0)
                {
                    return BadRequest();
                }

                return Ok();


            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

        }

        [HttpGet("entidades", Name = "Entidades")]
        public async Task<IActionResult> Entidades(int id, string entidad)
        {
            try
            {
                var includes = new List<Expression<Func<Relacion, object>>>();
                includes.Add(p => p.Persona!);
                includes.Add(p => p.Persona2!);
                includes.Add(p => p.TipoRelacion!);

                switch (entidad)
                {
                    //Mando el id de los hijos para obtener a los padres
                    case "padres":

                        var hijo = await _unitOfWork.Repository<Relacion>().GetAsync(
                            x => x.PersonaId == id,
                            null,
                            includes,
                            true
                        );

                        //Tipo de Relacion Padre Id = 2
                        //Tipo de Relacion Madre Id = 1

                        var padres = hijo.Where(x => x.TipoRelacionId == 1 || x.TipoRelacionId == 2).Select(x => new
                        {
                            Avatar = x.Persona2!.Avatar,
                            Nombres = x.Persona2!.Nombres,
                            Apellidos = x.Persona2.Apellidos,
                            Parentesco = x.TipoRelacion.Descripcion
                        });

                        return Ok(padres);

                    //Mando el id de los padres para obtener los hijos
                    case "hijos":


                        var padre = await _unitOfWork.Repository<Relacion>().GetAsync(
                            x => x.Persona2Id == id,
                            null,
                            includes,
                            true
                        );

                        var hijos = padre.Where(x => x.TipoRelacionId == 1 || x.TipoRelacionId == 2).Select(x => new
                        {
                            Avatar = x.Persona!.Avatar,
                            Nombres = x.Persona!.Nombres,
                            Apellidos = x.Persona.Apellidos,
                        });

                        return Ok(hijos);
                    default:
                        return BadRequest();
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpPost("hijos", Name = "Hijos")]
        public async Task<IActionResult> Hijos()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["searchHijo"].FirstOrDefault();
                var grado = Request.Form["grado"].FirstOrDefault();
                var escuela = Request.Form["escuela"].FirstOrDefault();

                pageSize = length != null ? Convert.ToInt32(length) : 0;
                skip = start != null ? Convert.ToInt32(start) : 0;
                recordsTotal = 0;

                var includeEstudiante = new List<Expression<Func<Matricula, object>>>
                {
                    p => p.Persona!,
                    p => p.Persona!.Sucursal,
                    p => p.NivelEscolar!,
                    p => p.CicloEscolar!,
                    p => p.Persona.TipoEstudiante
                   
                };
                var estudiantesList = await _unitOfWork.Repository<Matricula>().GetAsync(x =>
                            x.Persona.TipoEstudianteId != null &&
                            (x.Persona.Nombres.ToLower().Contains(searchValue.ToLower()) ||
                             x.Persona.Apellidos.ToLower().Contains(searchValue.ToLower()) ||
                              x.Persona.Identificacion.Contains(searchValue) ||
                             (x.Persona.Nombres.ToLower().Trim() + " " + x.Persona.Apellidos.ToLower().Trim()).Contains(searchValue.ToLower())
                            ),
                            null, includeEstudiante);

                if (!grado.Equals("Show All"))
                {
                    estudiantesList = estudiantesList.Where(x => x.NivelEscolar.Descripcion.ToLower().Contains(grado.ToLower())).ToList();
                }

                if (!escuela.Equals("Show All"))
                {
                    estudiantesList = estudiantesList.Where(x => x.Persona.Sucursal.Nombre.ToLower().Equals(escuela.ToLower())).ToList();
                }

                var query = estudiantesList.AsQueryable();
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    switch (sortColumnDir)
                    {
                        case "asc":
                            query = query.OrderBy(x => x.Persona.Nombres);
                            break;
                        case "desc":
                            query = query.OrderBy(x => x.Persona.Nombres);
                            break;
                        default:
                            query = query.OrderBy(x => x.Persona.Nombres);
                            break;
                    }
                }
                recordsTotal = query.Count();
                estudiantesList = query.Skip(skip).Take(pageSize).ToList();

                return Ok(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = estudiantesList
                });
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpPost("padres", Name = "Padres")]
        public async Task<IActionResult> Padres()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search"].FirstOrDefault();
                var sucursalFilter = Request.Form["sucursal"].FirstOrDefault();

                pageSize = length != null ? Convert.ToInt32(length) : 0;
                skip = start != null ? Convert.ToInt32(start) : 0;
                recordsTotal = 0;

                var inlcudesPadres = new List<Expression<Func<Relacion, object>>>
                {
                    p => p.Persona2,
                    p => p.Persona2.Sucursal,

                };

                var padresTodos = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.TipoRelacionId == 1 || x.TipoRelacionId == 2, null, inlcudesPadres);
                if (searchValue is not "")
                {
                    padresTodos = padresTodos.Where(x =>  
                    
                     x.Persona2.Identificacion.Contains(searchValue)
                    || (x.Persona2.Nombres.ToLower().Trim() + " " + x.Persona2.Apellidos.ToLower().Trim()).Contains(searchValue.ToLower())).ToList();
                }

                if (!sucursalFilter.Equals("Show All"))
                {
                    padresTodos = padresTodos.Where(x => x.Persona2.Sucursal.Nombre.ToLower().Equals(sucursalFilter.ToLower())).ToList();
                }

                recordsTotal = padresTodos.Count;
                padresTodos = padresTodos.Skip(skip).Take(pageSize).ToList();

                return Ok(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = padresTodos
                });
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }
    }
}
