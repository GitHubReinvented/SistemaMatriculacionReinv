using Application.Contracts.Identity;
using Application.Models;
using Application.Models.Authorization;
using Application.Persistence;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ReinventedPuembo.Models;
using System.Linq.Expressions;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Hosting;
//using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace ReinventedPuembo.Controllers
{
    
    public class DatosController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly UserManager<Usuario> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        public DatosController(IUnitOfWork unitOfWork,
            IAuthService authService,
            IMapper mapper,
            UserManager<Usuario> userManager,
            IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _mapper = mapper;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Representantes()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);
            if (representante.Count == 0)
            {
                return View("RegistrarRepresentantes");
            }
            else
            {

                var codigoRep = representante.FirstOrDefault().Id;
                var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);
                Persona padre = new Persona();
                Persona madre = new Persona();
                IEnumerable<Relacion> relacionesEstudianteMadre = new List<Relacion>();
                IEnumerable<Relacion> relacionesEstudiantePadre = new List<Relacion>();
                var codigoMadre = 0;
                var codigoPadre = 0;
                var codigoEstudiante = 0;
                foreach (var item in relacionesRepresentante)
                {
                    codigoEstudiante = item.PersonaId;
                    relacionesEstudianteMadre = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId == codigoEstudiante && x.TipoRelacionId == 1, null);
                    if(relacionesEstudianteMadre.Count() > 0)
                    {
                        codigoMadre = relacionesEstudianteMadre.FirstOrDefault().Persona2Id;
                        madre = await _unitOfWork.Repository<Persona>().GetByIdAsync(codigoMadre);
                    }
                    

                    relacionesEstudiantePadre = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId == codigoEstudiante && x.TipoRelacionId == 2, null);
                    if(relacionesEstudiantePadre.Count()>0)
                    {
                        codigoPadre = relacionesEstudiantePadre.FirstOrDefault().Persona2Id;
                        padre = await _unitOfWork.Repository<Persona>().GetByIdAsync(codigoPadre);
                    }
                    
                }

                ViewBag.Padre = padre;
                ViewBag.Madre = madre;
                return View("Representantes");
            }
        }
        public async Task<IActionResult> GuardarRepresentantes([FromForm] RepresentantesViewModel representantesViewModel)
        {
            if (representantesViewModel.RepresentantePadre == null)
            {
                representantesViewModel.RepresentantePadre = false;
            }
            if (representantesViewModel.RepresentanteMadre == null)
            {
                representantesViewModel.RepresentanteMadre = false;
            }

            if (representantesViewModel.RepresentantePadre == true && representantesViewModel.RepresentanteMadre == true)
            {
                TempData["Error"] = "Solo una persona puede ser representante";
                return RedirectToAction("Representantes", "Datos");
            }

            if (representantesViewModel.RepresentantePadre == false && representantesViewModel.RepresentanteMadre == false)
            {
                TempData["Error"] = "Faltó determinar quien es el representante";
                return RedirectToAction("Representantes", "Datos");
            }
            var currentUser = _authService.GetSessionUser();
            var user = await _userManager.FindByIdAsync(currentUser);

			string cedulaEstudiante = TempData["CedulaEstudiante"] as string;

			if (cedulaEstudiante.IsNullOrEmpty())
			{
                TempData["Error"] ="No se registró cedula del estudiante";
                return RedirectToAction("Representantes", "Datos");
            }

			int codigoEstudiante;
			if (cedulaEstudiante.IsNullOrEmpty())
			{
                TempData["Error"] = "No se registró cedula del estudiante";
                return RedirectToAction("Representantes", "Datos");
            }
			else
			{
				var datosEstudiante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion.Equals(cedulaEstudiante), null);
				codigoEstudiante = datosEstudiante.FirstOrDefault().Id;
			}
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            //var personaEntity = _mapper.Map<Persona>(estudianteViewModel);

            //personaEntity.Representante = true;
            //personaEntity.UsuarioId = user!.Id;
            //personaEntity.TipoPersonaId = null;

            if (representantesViewModel == null)
            {
                TempData["Error"] = "No se registró cedula del estudiante";
                return RedirectToAction("Representantes", "Datos");

            }

            var cedulaHijo = "";
            int codigoHijo;
            if(cedulaEstudiante.IsNullOrEmpty())
            {
                TempData["Error"] = "No se registró cedula del estudiante";
                return RedirectToAction("Representantes", "Datos");
            }
            else
            {
               var datosHijo = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion.Equals(cedulaEstudiante), null);
                codigoHijo = datosHijo.FirstOrDefault().Id;
            }

            try
            {



                //1. Ingreso Padre

                var padre = new Persona();
                padre.Nombres = representantesViewModel.NombresPadre;
                padre.Apellidos = representantesViewModel.ApellidosPadre;
                padre.EmailPrincipal = representantesViewModel.EmailPrincipalPadre;
                padre.TipoIdentificacionId = representantesViewModel.TipoIdentificacionIdPadre;
                padre.Identificacion = representantesViewModel.IdentificacionPadre;
                padre.Profesion = representantesViewModel.ProfesionPadre;
                padre.LugarTrabajo = representantesViewModel.LugarTrabajoPadre;
                padre.TelefonoMovil = representantesViewModel.CelularPadre;
                padre.EmailPrincipal = representantesViewModel.CorreoPadre;
                padre.Cargo = representantesViewModel.CargoPadre;
                padre.Representante = (bool)representantesViewModel.RepresentantePadre;
                padre.AutorizadoRetirar = true;
                if (padre.Representante == true)
                {
                    padre.UsuarioId = userId;
                    //Actualizo el correo electrónico en el aspUsers
                }
                if (representantesViewModel.AvatarPadre != null && representantesViewModel.AvatarPadre.Length > 0)
                {
                    // Obtener el nombre y la extensión del archivo
                    string numCedula = representantesViewModel.IdentificacionPadre;
                    string extensionArchivo = Path.GetExtension(representantesViewModel.AvatarPadre.FileName);

                    // Generar un nombre único para el archivo
                    string nombreArchivo = $"{numCedula}{extensionArchivo}";
                    padre.Avatar = nombreArchivo;
                    // Ruta de destino para guardar el archivo
                    string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                    using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                    {
                        await representantesViewModel.AvatarPadre.CopyToAsync(stream);
                    }

                }
                else
                {
                    padre.Avatar = "avatar-default.png";
                }


                _unitOfWork.Repository<Persona>().AddEntity(padre);

                var resultPersona = await _unitOfWork.Complete();

                if (resultPersona <= 0)
                {
                    TempData["Error"] = "No se registró datos";
                    return RedirectToAction("Representantes", "Datos");
                }

                var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(representantesViewModel.IdentificacionPadre), null);

                var personaId = resultadoPersona.FirstOrDefault()!.Id;

                var relacion = new Relacion();
                relacion.PersonaId = codigoHijo;
                relacion.Persona2Id = personaId;
                relacion.TipoRelacionId = 2; //2 de Padre

                _unitOfWork.Repository<Relacion>().AddEntity(relacion);

                var resultRelacion = await _unitOfWork.Complete();

                if (resultRelacion <= 0)
                {
                    TempData["Error"] = "No se registró datos";
                    return RedirectToAction("Representantes", "Datos");
                }


                var madre = new Persona();
                madre.Nombres = representantesViewModel.NombresMadre;
                madre.Apellidos = representantesViewModel.ApellidosMadre;
                madre.TipoIdentificacionId = representantesViewModel.TipoIdentificacionIdMadre;
                madre.Identificacion = representantesViewModel.IdentificacionMadre;
                madre.Profesion = representantesViewModel.ProfesionMadre;
                madre.LugarTrabajo = representantesViewModel.LugarTrabajoMadre;
                madre.TelefonoMovil = representantesViewModel.CelularMadre;
                madre.EmailPrincipal = representantesViewModel.CorreoMadre;
                madre.Cargo = representantesViewModel.CargoMadre;
                madre.Representante = (bool)representantesViewModel.RepresentanteMadre;
                madre.AutorizadoRetirar = true;

                if (madre.Representante == true)
                {
                    madre.UsuarioId = userId;
                }

                if (representantesViewModel.AvatarMadre != null && representantesViewModel.AvatarMadre.Length > 0)
                {
                    // Obtener el nombre y la extensión del archivo
                    string numCedula = representantesViewModel.IdentificacionMadre;
                    string extensionArchivo = Path.GetExtension(representantesViewModel.AvatarMadre.FileName);

                    // Generar un nombre único para el archivo
                    string nombreArchivo = $"{numCedula}{extensionArchivo}";
                    madre.Avatar = nombreArchivo;
                    // Ruta de destino para guardar el archivo
                    string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                    using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                    {
                        await representantesViewModel.AvatarMadre.CopyToAsync(stream);
                    }

                }
                else
                {
                    madre.Avatar = "avatar-default.png";
                }
                _unitOfWork.Repository<Persona>().AddEntity(madre);

                var resultMadre = await _unitOfWork.Complete();

                if (resultMadre <= 0)
                {
                    TempData["Error"] = "No se registró datos";
                    return RedirectToAction("Representantes", "Datos");
                }

                var resultadoMadre = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(representantesViewModel.IdentificacionMadre), null);

                var madreId = resultadoMadre.FirstOrDefault()!.Id;

                var relacionM = new Relacion();
                relacionM.TipoRelacionId = 1; //1 de Madre
                relacionM.PersonaId = codigoHijo;
                relacionM.Persona2Id = madreId;

                _unitOfWork.Repository<Relacion>().AddEntity(relacionM);

                var resultRelacionM = await _unitOfWork.Complete();

                if (resultRelacionM <= 0)
                {
                    TempData["Error"] = "No se registró datos";
                    return RedirectToAction("Representantes", "Datos");
                }

                
                return RedirectToAction("AutorizadosRetirar", "Datos");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se registró datos";
                return RedirectToAction("Representantes", "Datos");
            }

        }
        public async Task<IActionResult> AutorizadosRetirar()
        {
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var user = await _userManager.GetUserAsync(User);
            string userId = user!.Id;

            //Obtengo el id del representante en relación el ID que va en Persona2
            var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);
            var codigoRep = representante.FirstOrDefault().Id;
            var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);

            var includesRelacion = new List<Expression<Func<Relacion, object>>>
            {
                p => p.Persona2!,
                p => p.Persona2!.Usuario,
                p=> p.TipoRelacion!,
                p => p.Persona!,
                p => p.Persona!.Usuario

            };

            var autorizadosRetiro = await _unitOfWork.Repository<Relacion>().GetEntityAsync(x => x.Persona2!.UsuarioId == user!.Id, includesRelacion);
            //aqui debo consultar el id de la persona 1 a lo está ligada la persona 2

            var relacionesEstudiante = await _unitOfWork.Repository<Relacion>().GetAsync(
                x=>x.PersonaId == autorizadosRetiro.PersonaId
                && x.Persona2!.AutorizadoRetirar == true, null, includesRelacion);

            return View(relacionesEstudiante);
        }
        [HttpPost]
        public async Task<IActionResult> RegistrarAutorizadosRetirar(AutRetirarViewModel autRetirarViewModel)
        {
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);
            var codigoRep = representante.FirstOrDefault().Id;
            var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);
            List<Persona> listaEstudiantes = new List<Persona>();
            var codigoEstudiante =0 ;
            foreach (var item in relacionesRepresentante)
            {
                codigoEstudiante = item.PersonaId;

                var estudiantes = await _unitOfWork.Repository<Persona>().GetByIdAsync(codigoEstudiante);
                listaEstudiantes.Add(estudiantes);
            }
           

            string cedulaEstudiante = listaEstudiantes.FirstOrDefault().Identificacion;

            var datosEstudiante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion.Equals(cedulaEstudiante), null);
            codigoEstudiante = datosEstudiante.FirstOrDefault().Id;

                List<Persona> oListaAutorizados = new List<Persona>() { };

                var persona = new Persona();
                persona.Nombres = autRetirarViewModel.Nombres;
                persona.Apellidos = autRetirarViewModel.Apellidos;
                persona.TipoIdentificacionId = autRetirarViewModel.TipoIdentificacionId;
                persona.Identificacion = autRetirarViewModel.Identificacion;
                persona.TelefonoMovil = autRetirarViewModel.TelefonoMovil;
                persona.AutorizadoRetirar = true;

                    if (autRetirarViewModel.Avatar != null && autRetirarViewModel.Avatar.Length > 0)
                    {
                        // Obtener el nombre y la extensión del archivo
                        string numCedula = autRetirarViewModel.Identificacion;
                        string extensionArchivo = Path.GetExtension(autRetirarViewModel.Avatar.FileName);

                        // Generar un nombre único para el archivo
                        string nombreArchivo = $"{numCedula}{extensionArchivo}";
                        persona.Avatar = nombreArchivo;
                        // Ruta de destino para guardar el archivo
                        string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                        using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                        {
                            await autRetirarViewModel.Avatar.CopyToAsync(stream);
                        }

                    }
                    else
                    {
                        persona.Avatar = "avatar-default.png";
                    }

            _unitOfWork.Repository<Persona>().AddEntity(persona);

                var resultPersona = await _unitOfWork.Complete();

                if (resultPersona <= 0)
                {
                    return BadRequest();
                }

                var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(autRetirarViewModel.Identificacion), null);

                var personaId = resultadoPersona.FirstOrDefault()!.Id;

                var relacion = new Relacion();
                relacion.PersonaId = codigoEstudiante;
                relacion.Persona2Id = personaId;
                relacion.TipoRelacionId = autRetirarViewModel.TipoRelacionId; //2 de Padre

                _unitOfWork.Repository<Relacion>().AddEntity(relacion);

                var resultRelacion = await _unitOfWork.Complete();

                if (resultRelacion <= 0)
                {
                    return BadRequest();
                }

                    oListaAutorizados.Add(persona);
               
                    //return View("AutorizadosRetirar", oListaAutorizados);
                    return RedirectToAction("AutorizadosRetirar","Datos");

        }

        public async Task<IActionResult> EditarAutorizadosRetirar(AutRetirarViewModel autRetirarViewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);
            //var descuentoEditar = await _unitOfWork.Repository<AplicaDescuento>().GetByIdAsync(descuentoViewModel.DescuentoId);
            var relacion = await _unitOfWork.Repository<Relacion>().GetByIdAsync((int)autRetirarViewModel.id);
            relacion.TipoRelacionId = autRetirarViewModel.TipoRelacionId;
            await _unitOfWork.Repository<Relacion>().UpdateAsync(relacion);

            var autorizado = await _unitOfWork.Repository<Persona>().GetByIdAsync((int)autRetirarViewModel.idPersona2);
            autorizado.Nombres = autRetirarViewModel.Nombres;
            autorizado.Apellidos = autRetirarViewModel.Apellidos;
            autorizado.TipoIdentificacionId = autRetirarViewModel.TipoIdentificacionId;
            autorizado.Identificacion = autRetirarViewModel.Identificacion;
            await _unitOfWork.Repository<Persona>().UpdateAsync(autorizado);

            return RedirectToAction("AutorizadosRetirar","Datos");

        }

        public async Task<IActionResult> EliminarAutorizadosRetirar(int id, int idPersona, int idTipoRelacion)
        {

            if(idTipoRelacion == 1 || idTipoRelacion == 2)
            {

                var Persona2 = await _unitOfWork.Repository<Persona>().GetByIdAsync(idPersona);
                Persona2.AutorizadoRetirar = false;

                await _unitOfWork.Repository<Persona>().UpdateAsync(Persona2);

                return RedirectToAction("AutorizadosRetirar", "Datos");
            }
            else
            {
                var relacion = await _unitOfWork.Repository<Relacion>().GetByIdAsync(id);
                var Persona2 = await _unitOfWork.Repository<Persona>().GetByIdAsync(idPersona);

                _unitOfWork.Repository<Relacion>().DeleteEntity(relacion);
                _unitOfWork.Repository<Persona>().DeleteEntity(Persona2);

                var result = await _unitOfWork.Complete();

                if (result <= 0)
                {
                    return BadRequest();
                }

                return RedirectToAction("AutorizadosRetirar", "Datos");
            }
           
        }

        public async Task<IActionResult> Estudiante()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);
            if(representante.Count == 0)
            {
                return View("RegistrarEstudiante");
            }
            else
            {
            var codigoRep = representante.FirstOrDefault().Id;
            var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);
            List<Persona> listaEstudiantes = new List<Persona>();
            IEnumerable<Matricula> datosmatricula = new List<Matricula>();
            IEnumerable<HistorialMedico> historialMedicoEstudiante = new List<HistorialMedico>();
            IEnumerable<FichaMedica> fichaMedicaEstudiante = new List<FichaMedica>();
            IEnumerable<Direccion> DireccionEstudiante = new List<Direccion>();
            IEnumerable<FichaObservacionMedica> fichaObsMedicaEstudiante = new List<FichaObservacionMedica>();
                var codigoEstudiante = 0;

            foreach (var item in relacionesRepresentante)
            {
                codigoEstudiante = item.PersonaId;

                var includesDireccion = new List<Expression<Func<Persona, object>>>
                        {
                            p => p.Direccion!
                        };
                var estudiantes = await _unitOfWork.Repository<Persona>().GetEntityAsync(x => x.Id == codigoEstudiante,includesDireccion);

                    var includesMatricula = new List<Expression<Func<Matricula, object>>>
                        {
                            p => p.NivelEscolar!,

                        };

                    var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codigoEstudiante, null, includesMatricula);
                    var codigoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().Id;
                    datosmatricula = MatriculaEstudiante;

                    var direccion = await _unitOfWork.Repository<Direccion>().GetAsync(x => x.PersonaId == codigoEstudiante, null);
                    DireccionEstudiante = direccion;

                    var fichamed = await _unitOfWork.Repository<FichaMedica>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante);
                    fichaMedicaEstudiante = fichamed;
                    var idFichamed = fichamed.FirstOrDefault().Id;

                    var observacionesMedicas = await _unitOfWork.Repository<FichaObservacionMedica>().GetAsync(x => x.FichaMedicaId == idFichamed);
                    fichaObsMedicaEstudiante = observacionesMedicas;
                    var historialEstudiante = await _unitOfWork.Repository<HistorialMedico>().GetAsync(x => x.FichaMedicaId == fichamed.FirstOrDefault().Id);
                    historialMedicoEstudiante = historialEstudiante;

                    if (estudiantes!=null) { listaEstudiantes.Add(estudiantes); }

                    
            }

                ViewBag.Estudiantes = listaEstudiantes;
                ViewBag.fichaMedica = fichaMedicaEstudiante;
                ViewBag.historialEstudiante = historialMedicoEstudiante;
                ViewBag.fichaObsMedicaEstudiante = fichaObsMedicaEstudiante;

                var viewModel = new DatosEstudianteViewModel
                {
                    Estudiantes = listaEstudiantes,
                    FichaMedicaEstudiante = fichaMedicaEstudiante,
                    HistorialEstudiante = historialMedicoEstudiante,
                    FichaObsMedicaEstudiante = fichaObsMedicaEstudiante,
                    DireccionEstudiante = DireccionEstudiante
                    
                };

                return View("Estudiante", viewModel);
            }
            
        }

        public async Task<IActionResult> ObtenerInfoEstudiante(int estudianteId)
        {
            var estudiante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Id == estudianteId, null);

            var includesMatricula = new List<Expression<Func<Matricula, object>>>
            {
                p => p.NivelEscolar!
            };

            var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == estudianteId, null, includesMatricula);
            var codigoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().Id;

            var fichamed = await _unitOfWork.Repository<FichaMedica>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante);
            var idFichamed = fichamed.FirstOrDefault().Id;

            var observacionesMedicas = await _unitOfWork.Repository<FichaObservacionMedica>().GetAsync(x => x.FichaMedicaId == idFichamed);
            var historialEstudiante = await _unitOfWork.Repository<HistorialMedico>().GetAsync(x => x.FichaMedicaId == idFichamed);

            var direccion = await _unitOfWork.Repository<Direccion>().GetAsync(x => x.PersonaId == estudianteId, null);
            var viewModel = new DetalleEstudianteViewModel
            {
                EstudiantesSeleccionado = estudiante.FirstOrDefault(),
                HistorialMedico = historialEstudiante.ToList(),
                ObservacionesMedicas = observacionesMedicas.ToList(),
                MatriculaEstudiante = MatriculaEstudiante.FirstOrDefault(),
                DireccionEstudiante = direccion.FirstOrDefault(),
                FichaEstudiante = fichamed.FirstOrDefault()
            };

            return PartialView("_DetallesEstudiante", viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> RegistroEstudiante([FromForm] EstudianteViewModel estudianteViewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(estudianteViewModel);
            //}

            string error="";
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var currentUser = _authService.GetSessionUser();
            var user = await _userManager.FindByIdAsync(currentUser);

            //var personaEntity = _mapper.Map<Persona>(estudianteViewModel);

            //personaEntity.Representante = true;
            //personaEntity.UsuarioId = user!.Id;
            //personaEntity.TipoPersonaId = null;

            if (estudianteViewModel == null)
            {
                TempData["Error"] = "No se recibió datos";
                return RedirectToAction("Index", "Home");
            }


            if (estudianteViewModel.TipoIdentificacionId == null || estudianteViewModel.Identificacion == null)
            {
                TempData["Error"] = "No se recibió datos";
                return RedirectToAction("Index", "Home");
            }

            //var productoPersona = await context.tb_sccd_personaproducto.FindAsync(personaCrearDTO.prp_id);

            //if (productoPersona == null)
            //{
            //    return -1;
            //}

            //using var transaction = context.Database.BeginTransaction();

            try
                {
                    //1. ingreso referencia personal

                    var persona = new Persona();
                    persona.TipoIdentificacionId = estudianteViewModel.TipoIdentificacionId;
                    persona.TipoEstudianteId = null;
                    persona.Nombres = estudianteViewModel.Nombres;
                    persona.Apellidos = estudianteViewModel.Apellidos;
                    persona.Identificacion = estudianteViewModel.Identificacion;
                    persona.FechaNacimiento = estudianteViewModel.FechaNacimiento;

                    persona.TipoEstudianteId = 3; // Estudiante Estandard
                    TempData["CedulaEstudiante"] = persona.Identificacion;

                //persona.Avatar = estudianteViewModel.Identificacion + ".jpg";
                //persona.UsuarioId = userId;
                if (estudianteViewModel.Avatar != null && estudianteViewModel.Avatar.Length > 0)
                {
                    // Obtener el nombre y la extensión del archivo
                    string numCedula = estudianteViewModel.Identificacion;
                    string extensionArchivo = Path.GetExtension(estudianteViewModel.Avatar.FileName);

                    // Generar un nombre único para el archivo
                    string nombreArchivo = $"{numCedula}{extensionArchivo}";
                    persona.Avatar = nombreArchivo;
                    // Ruta de destino para guardar el archivo
                    string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath,"assets", "media", "avatars", nombreArchivo);

                    using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                    {
                        await estudianteViewModel.Avatar.CopyToAsync(stream);
                    }

                }
                else
                {
                    persona.Avatar = "avatar-default.png";
                }
                //Session["CedulaEstudiante"] = persona.Identificacion;

                _unitOfWork.Repository<Persona>().AddEntity(persona);

                    var resultPersona = await _unitOfWork.Complete();

                    if (resultPersona <= 0)
                        {
                            TempData["Error"] = "No se registró datos";
                            return RedirectToAction("Index", "Home");
                        }

                var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(estudianteViewModel.Identificacion), null);

                var personaId = resultadoPersona.FirstOrDefault()!.Id;



                var direccion = new Direccion();
                direccion.CallePrincipal = estudianteViewModel.CallePrincipal;
                direccion.CalleSecundaria = estudianteViewModel?.CalleSecundaria;
                direccion.Numero = estudianteViewModel?.Numero;
                direccion.TipoDireccionId = 1; //Codigo 1 es dirección de domicilio
                direccion.PersonaId = personaId;
                direccion.Provincia = 17; // Pichincha
                direccion.Canton = 1; //Quito
                direccion.Parroquia = 113; 
                _unitOfWork.Repository<Direccion>().AddEntity(direccion);

                var resultDireccion = await _unitOfWork.Complete();

                if (resultDireccion <= 0)
                {
                    TempData["Error"] = "No se registró datos";
                    return RedirectToAction("Index", "Home");
                }

                var matricula = new Matricula();
                matricula.PersonaId = personaId;
                matricula.EstadoMatriculaId= 1; //1 de inscripción
                matricula.NivelEscolarId= estudianteViewModel.nivel;

                _unitOfWork.Repository<Matricula>().AddEntity(matricula);

                var resultMatricula = await _unitOfWork.Complete();

                if (resultMatricula <= 0)
                {
                    TempData["Error"] = "No se registró datos";
                    return RedirectToAction("Index", "Home");
                }
                /*
                var includesCiclo = new List<Expression<Func<Matricula, object>>>
                    {
                        p => p.Paralelo!
                    };
                */
                var numeroMatricula = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId.Equals(personaId));//, null,includesCiclo);

                var fichaMedica = new FichaMedica();
                fichaMedica.NombresMedico = estudianteViewModel?.NombreMedico;
                fichaMedica.TelefonoMedico = estudianteViewModel?.TelefonoMedico;
                fichaMedica.DireccionMedico = estudianteViewModel?.DireccionMedico;
                fichaMedica.CelularMedico = estudianteViewModel?.CelularMedico;
                fichaMedica.Alergias = estudianteViewModel?.alergias;
                fichaMedica.NotasImportanteSalud = estudianteViewModel?.DatoAdicional;
                fichaMedica.RestriccionesAlimenticias = estudianteViewModel?.restriccionAlimento;
                fichaMedica.MatriculaId = numeroMatricula.FirstOrDefault().Id;
                _unitOfWork.Repository<FichaMedica>().AddEntity(fichaMedica);

                var resultFicha = await _unitOfWork.Complete();

                if (resultFicha <= 0)
                {
                    TempData["Error"] = "No se registró datos";
                    return RedirectToAction("Index", "Home");
                }

                //var persona = mapper.Map<Persona>(personaCrearDTO);
                //context.tb_sccd_persona.Add(persona);
                //await context.SaveChangesAsync();



                //2. actualizo tarea proceso
                //context.Entry(productoPersona).State = EntityState.Modified;
                //await context.SaveChangesAsync();



                //transaction.Commit();



                //return (int)persona.Id;

                return RedirectToAction("Representantes", "Datos");
            }
                catch (Exception ex)
                {
                TempData["Error"] = "Error en el registro de datos";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> ContactoEmergencia()
        {
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var user = await _userManager.GetUserAsync(User);
            string userId = user!.Id;

            //Obtengo el id del representante en relación el ID que va en Persona2
            var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);
            var codigoRep = representante.FirstOrDefault().Id;
            var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);

            var includesRelacion = new List<Expression<Func<Relacion, object>>>
            {
                p => p.Persona2!,
                p => p.Persona2!.Usuario,
                p=> p.TipoRelacion!,
                p => p.Persona!,
                p => p.Persona!.Usuario

            };

            var autorizadosRetiro = await _unitOfWork.Repository<Relacion>().GetEntityAsync(x => x.Persona2!.UsuarioId == user!.Id, includesRelacion);
            //aqui debo consultar el id de la persona 1 a lo está ligada la persona 2

            var relacionesEstudiante = await _unitOfWork.Repository<Relacion>().GetAsync(
                x => x.PersonaId == autorizadosRetiro.PersonaId
                && x.Persona2!.ContactoEmergencia == true, null, includesRelacion);

            return View(relacionesEstudiante);
        }

        public async Task<IActionResult> EliminarContactoEmergencia(int id, int idPersona)
        {

            var relacion = await _unitOfWork.Repository<Relacion>().GetByIdAsync(id);
            var Persona2 = await _unitOfWork.Repository<Persona>().GetByIdAsync(idPersona);

            _unitOfWork.Repository<Relacion>().DeleteEntity(relacion);
            _unitOfWork.Repository<Persona>().DeleteEntity(Persona2);

            var result = await _unitOfWork.Complete();

            if (result <= 0)
            {
                return BadRequest();
            }

            return RedirectToAction("ContactoEmergencia", "Datos");
        }

        public async Task<IActionResult> RegistrarContactoEmergencia(ContactoEmergenciaViewModel contactoEmergenciaViewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AutorizadosRetirar", "Datos", contactoEmergenciaViewModel);

            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);
            var codigoRep = representante.FirstOrDefault().Id;
            var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);
            List<Persona> listaEstudiantes = new List<Persona>();
            var codigoEstudiante = 0;
            foreach (var item in relacionesRepresentante)
            {
                codigoEstudiante = item.PersonaId;

                var estudiantes = await _unitOfWork.Repository<Persona>().GetByIdAsync(codigoEstudiante);
                listaEstudiantes.Add(estudiantes);
            }


            string cedulaEstudiante = listaEstudiantes.FirstOrDefault().Identificacion;


            List<Persona> oListaContacto = new List<Persona>() { };

            if (ModelState.IsValid)
            {
                Persona obj = new Persona();
                obj.Nombres = contactoEmergenciaViewModel.Nombres;
                obj.Apellidos = contactoEmergenciaViewModel.Apellidos;
                obj.Identificacion = contactoEmergenciaViewModel.Identificacion;
                obj.TipoIdentificacionId = contactoEmergenciaViewModel.TipoIdentificacionId;
                obj.TelefonoMovil = contactoEmergenciaViewModel.TelefonoMovil;
                obj.ContactoEmergencia = true;

                if (contactoEmergenciaViewModel.Avatar != null && contactoEmergenciaViewModel.Avatar.Length > 0)
                {
                    // Obtener el nombre y la extensión del archivo
                    string numCedula = contactoEmergenciaViewModel.Identificacion;
                    string extensionArchivo = Path.GetExtension(contactoEmergenciaViewModel.Avatar.FileName);

                    // Generar un nombre único para el archivo
                    string nombreArchivo = $"{numCedula}{extensionArchivo}";
                    obj.Avatar = nombreArchivo;
                    // Ruta de destino para guardar el archivo
                    string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                    using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                    {
                        await contactoEmergenciaViewModel.Avatar.CopyToAsync(stream);
                    }

                }
                else
                {
                    obj.Avatar = "avatar-default.png";
                }


                _unitOfWork.Repository<Persona>().AddEntity(obj);

                var result = await _unitOfWork.Complete();
                if (result <= 0)
                {
                    return BadRequest();
                }

                var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(contactoEmergenciaViewModel.Identificacion), null);

                var personaId = resultadoPersona.FirstOrDefault()!.Id;

                var relacion = new Relacion();
                relacion.PersonaId = codigoEstudiante;
                relacion.Persona2Id = personaId;
                relacion.TipoRelacionId = contactoEmergenciaViewModel.TipoRelacionId;

                _unitOfWork.Repository<Relacion>().AddEntity(relacion);

                var resultRelacion = await _unitOfWork.Complete();

                if (resultRelacion <= 0)
                {
                    return BadRequest();
                }


                var includesPersona = new List<Expression<Func<Persona, object>>>
                    {
                        p => p.AutorizadoRetirar!
                    };
                var personaAut = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Id != null, null);


                oListaContacto.Add(obj);

                return RedirectToAction("ContactoEmergencia", "Datos");
            }

            return View();

        }

        public async Task<IActionResult> EditarContactoEmergencia(ContactoEmergenciaViewModel autRetirarViewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);
            //var descuentoEditar = await _unitOfWork.Repository<AplicaDescuento>().GetByIdAsync(descuentoViewModel.DescuentoId);
            var relacion = await _unitOfWork.Repository<Relacion>().GetByIdAsync((int)autRetirarViewModel.id);
            relacion.TipoRelacionId = autRetirarViewModel.TipoRelacionId;
            await _unitOfWork.Repository<Relacion>().UpdateAsync(relacion);

            var autorizado = await _unitOfWork.Repository<Persona>().GetByIdAsync((int)autRetirarViewModel.idPersona2);
            autorizado.Nombres = autRetirarViewModel.Nombres;
            autorizado.Apellidos = autRetirarViewModel.Apellidos;
            autorizado.TipoIdentificacionId = autRetirarViewModel.TipoIdentificacionId;
            autorizado.Identificacion = autRetirarViewModel.Identificacion;
            await _unitOfWork.Repository<Persona>().UpdateAsync(autorizado);

            return RedirectToAction("ContactoEmergencia", "Datos");

        }

        [HttpGet]
        public async Task<IActionResult> EditarEstudiante(int id)
            {
            var estudiante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Id == id, null);

            var includesMatricula = new List<Expression<Func<Matricula, object>>>
            {
                p => p.NivelEscolar!
            };

            var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == id, null, includesMatricula);
            var codigoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault()!.Id;

            var fichamed = await _unitOfWork.Repository<FichaMedica>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante);
            var idFichamed = fichamed.FirstOrDefault()!.Id;

            var observacionesMedicas = await _unitOfWork.Repository<FichaObservacionMedica>().GetAsync(x => x.FichaMedicaId == idFichamed);
            var historialEstudiante = await _unitOfWork.Repository<HistorialMedico>().GetAsync(x => x.FichaMedicaId == idFichamed);

            var direccion = await _unitOfWork.Repository<Direccion>().GetAsync(x => x.PersonaId == id, null);
            var viewModel = new DetalleEstudianteViewModel
            {
                EstudiantesSeleccionado = estudiante.FirstOrDefault()!,
                HistorialMedico = historialEstudiante.ToList(),
                ObservacionesMedicas = observacionesMedicas.ToList(),
                MatriculaEstudiante = MatriculaEstudiante.FirstOrDefault()!,
                DireccionEstudiante = direccion.FirstOrDefault()!,
                FichaEstudiante = fichamed.FirstOrDefault()!
            };

            return View(viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> EditarEstudiante(EstudianteViewModel estudianteViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var EditarEstudiante = await _unitOfWork.Repository<Persona>().GetEntityAsync(x => x.Identificacion == estudianteViewModel.Identificacion);


            if(EditarEstudiante != null)
            {
                EditarEstudiante.Nombres = estudianteViewModel.Nombres;
                EditarEstudiante.Apellidos = estudianteViewModel.Apellidos;
                EditarEstudiante.TipoIdentificacionId = estudianteViewModel.TipoIdentificacionId;
                EditarEstudiante.Identificacion = estudianteViewModel.Identificacion;
                EditarEstudiante.FechaNacimiento = estudianteViewModel.FechaNacimiento;
                EditarEstudiante.TipoEstudianteId = 3; // Estudiante Estandard


                if (estudianteViewModel.Avatar != null && estudianteViewModel.Avatar.Length > 0)
                {
                    string numCedula = estudianteViewModel.Identificacion;
                    string extensionArchivo = Path.GetExtension(estudianteViewModel.Avatar.FileName);

                    // Generar un nombre único para el archivo
                    string nombreArchivo = $"{numCedula}{extensionArchivo}";
                    EditarEstudiante.Avatar = nombreArchivo;
                    // Ruta de destino para guardar el archivo
                    string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                    string rutaImagenAnterior = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", EditarEstudiante.Avatar);
                    if (System.IO.File.Exists(rutaImagenAnterior))
                    {
                        System.IO.File.Delete(rutaImagenAnterior);
                    }
                    using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                    {
                        await estudianteViewModel.Avatar.CopyToAsync(stream);
                    }
                }

                if (estudianteViewModel.PDFIdentificacion != null)
                {
                    string numCedula = estudianteViewModel.Identificacion;
                    string extensionArchivo = Path.GetExtension(estudianteViewModel.PDFIdentificacion.FileName);

                    // Generar un nombre único para el archivo
                    string nombreArchivo = $"{numCedula}{extensionArchivo}";
                    
                    string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacion", nombreArchivo);

                    string[] archivosExistentes = Directory.GetFiles(Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacion"));
                    string nombreArchivoPDFAnterior = archivosExistentes.FirstOrDefault(file => Path.GetFileName(file) == estudianteViewModel.PDFIdentificacion.FileName);

                    if (estudianteViewModel.PDFIdentificacion.FileName != Path.GetFileName(nombreArchivoPDFAnterior) && nombreArchivoPDFAnterior != null)
                    {
                        System.IO.File.Delete(nombreArchivoPDFAnterior);
                    }


                    using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                    {
                        await estudianteViewModel.PDFIdentificacion.CopyToAsync(stream);
                    }
                }

                await _unitOfWork.Repository<Persona>().UpdateAsync(EditarEstudiante);


                var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(estudianteViewModel.Identificacion), null);

                var personaId = resultadoPersona.FirstOrDefault()!.Id;

                var DireccionEdit = await _unitOfWork.Repository<Direccion>().GetEntityAsync(x => x.PersonaId!.Equals(personaId), null);

                DireccionEdit.CallePrincipal = !string.IsNullOrEmpty(estudianteViewModel.CallePrincipal) ? estudianteViewModel.CallePrincipal : "";
                DireccionEdit.CalleSecundaria = !string.IsNullOrEmpty(estudianteViewModel.CalleSecundaria) ? estudianteViewModel.CalleSecundaria : "";
                DireccionEdit.Numero = !string.IsNullOrEmpty(estudianteViewModel.Numero) ? estudianteViewModel.Numero : "";

                await _unitOfWork.Repository<Direccion>().UpdateAsync(DireccionEdit);

                var numeroMatricula = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId.Equals(personaId));
                var idMat = numeroMatricula.FirstOrDefault()!.Id;


                var fichaMedica = await _unitOfWork.Repository<FichaMedica>().GetEntityAsync(x => x.MatriculaId.Equals(idMat));
                
                fichaMedica.NombresMedico = !string.IsNullOrEmpty(estudianteViewModel?.NombreMedico) ? estudianteViewModel.NombreMedico : "";
                fichaMedica.TelefonoMedico = !string.IsNullOrEmpty(estudianteViewModel?.TelefonoMedico) ? estudianteViewModel.TelefonoMedico : "";
                fichaMedica.DireccionMedico = !string.IsNullOrEmpty(estudianteViewModel?.DireccionMedico) ? estudianteViewModel.DireccionMedico : "";
                fichaMedica.CelularMedico = !string.IsNullOrEmpty(estudianteViewModel?.CelularMedico) ? estudianteViewModel.CelularMedico : "";
                fichaMedica.Alergias = !string.IsNullOrEmpty(estudianteViewModel?.alergias) ? estudianteViewModel.alergias : "";
                fichaMedica.NotasImportanteSalud = !string.IsNullOrEmpty(estudianteViewModel?.DatoAdicional) ? estudianteViewModel.DatoAdicional : "";
                fichaMedica.RestriccionesAlimenticias = !string.IsNullOrEmpty(estudianteViewModel?.restriccionAlimento) ? estudianteViewModel.restriccionAlimento : "";

                if(estudianteViewModel.Analgesico == null)
                {
                    fichaMedica.AutorizaAntiinflamatorioAnalgesico = false;
                }
                else if(estudianteViewModel.Analgesico == false)
                {
                fichaMedica.AutorizaAntiinflamatorioAnalgesico = true;
                }


                await _unitOfWork.Repository<FichaMedica>().UpdateAsync(fichaMedica);

                var historialMed = await _unitOfWork.Repository<FichaMedica>().GetAsync(x => x.MatriculaId == idMat);

                var idhistorial = historialMed.FirstOrDefault().Id;

                var historialMedicoEstudiante = await _unitOfWork.Repository<HistorialMedico>().GetEntityAsync(x => x.FichaMedicaId == idhistorial);
                historialMedicoEstudiante.Peso = estudianteViewModel.Peso;
                historialMedicoEstudiante.Talla = estudianteViewModel.Talla;

                await _unitOfWork.Repository<HistorialMedico>().UpdateAsync(historialMedicoEstudiante);

                if (estudianteViewModel.NumeroDosis != null)
                {
                var observacion = await _unitOfWork.Repository<FichaObservacionMedica>().GetEntityAsync(x => x.FichaMedicaId == idhistorial && x.ObservacionMedicaId == 1 );
                    observacion.FichaMedicaId = idhistorial;
                    observacion.Respuesta = true;
                    observacion.Especificacion = estudianteViewModel.NumeroDosis.ToString();
                    observacion.ObservacionMedicaId = 1; //VacunaCovid

                    await _unitOfWork.Repository<FichaObservacionMedica>().UpdateAsync(observacion);

                }
                else
                {
                    var observacion = await _unitOfWork.Repository<FichaObservacionMedica>().GetEntityAsync(x => x.FichaMedicaId == idhistorial && x.ObservacionMedicaId == 1);
                    observacion.FichaMedicaId = idhistorial;
                    observacion.Respuesta = false;
                    observacion.Especificacion = "";
                    observacion.ObservacionMedicaId = 1; //VacunaCovid

                    await _unitOfWork.Repository<FichaObservacionMedica>().UpdateAsync(observacion);

                }


                if (estudianteViewModel.EnfermedadCronica != null)
                {
                    var observacion2 = await _unitOfWork.Repository<FichaObservacionMedica>().GetEntityAsync(x => x.FichaMedicaId == idhistorial && x.ObservacionMedicaId == 2);

                    observacion2.FichaMedicaId = idhistorial;
                    observacion2.Respuesta = true;
                    observacion2.Especificacion = estudianteViewModel.EnfermedadCronica;
                    observacion2.ObservacionMedicaId = 2; //Enfermedad crónica
                    await _unitOfWork.Repository<FichaObservacionMedica>().UpdateAsync(observacion2);


                }
                else
                {
                    var observacion2 = await _unitOfWork.Repository<FichaObservacionMedica>().GetEntityAsync(x => x.FichaMedicaId == idhistorial && x.ObservacionMedicaId == 2);

                    observacion2.FichaMedicaId = idhistorial;
                    observacion2.Respuesta = false;
                    observacion2.Especificacion = "";
                    observacion2.ObservacionMedicaId = 2; //Enfermedad crónica

                    await _unitOfWork.Repository<FichaObservacionMedica>().UpdateAsync(observacion2);
                }


                if (estudianteViewModel.MedicacionContinua != null)
                {
                    var observacion3 = await _unitOfWork.Repository<FichaObservacionMedica>().GetEntityAsync(x => x.FichaMedicaId == idhistorial && x.ObservacionMedicaId == 3);

                    observacion3.FichaMedicaId = idhistorial;
                    observacion3.Respuesta = true;
                    observacion3.Especificacion = estudianteViewModel.MedicacionContinua;
                    observacion3.ObservacionMedicaId = 3; //medicacion continua

                    await _unitOfWork.Repository<FichaObservacionMedica>().UpdateAsync(observacion3);

                }
                else
                {
                    var observacion3 = await _unitOfWork.Repository<FichaObservacionMedica>().GetEntityAsync(x => x.FichaMedicaId == idhistorial && x.ObservacionMedicaId == 3);
                    observacion3.FichaMedicaId = idhistorial;
                    observacion3.Respuesta = false;
                    observacion3.Especificacion = "";
                    observacion3.ObservacionMedicaId = 3; //medicacion continua

                    await _unitOfWork.Repository<FichaObservacionMedica>().UpdateAsync(observacion3);

                }


                if (estudianteViewModel.cirugia != null)
                {
                    var observacion4 = await _unitOfWork.Repository<FichaObservacionMedica>().GetEntityAsync(x => x.FichaMedicaId == idhistorial && x.ObservacionMedicaId == 4);

                    observacion4.FichaMedicaId = idhistorial;
                    observacion4.Respuesta = true;
                    observacion4.Especificacion = estudianteViewModel.cirugia;
                    observacion4.ObservacionMedicaId = 4; //cirugia
                    await _unitOfWork.Repository<FichaObservacionMedica>().UpdateAsync(observacion4);

                }
                else
                {
                    var observacion4 = await _unitOfWork.Repository<FichaObservacionMedica>().GetEntityAsync(x => x.FichaMedicaId == idhistorial && x.ObservacionMedicaId == 4);

                    observacion4.FichaMedicaId = idhistorial;
                    observacion4.Respuesta = false;
                    observacion4.Especificacion = "";
                    observacion4.ObservacionMedicaId = 4; //cirugia

                    await _unitOfWork.Repository<FichaObservacionMedica>().UpdateAsync(observacion4);


                }

            }

            return RedirectToAction("Estudiante","Datos");

        }

        [HttpGet]
        public async Task<IActionResult> EditarRepresentante()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);
            if (representante.Count == 0)
            {
                return View("RegistrarRepresentantes");
            }
            else
            {

                var codigoRep = representante.FirstOrDefault().Id;
                var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);
                Persona padre = new Persona();
                Persona madre = new Persona();
                IEnumerable<Relacion> relacionesEstudianteMadre = new List<Relacion>();
                IEnumerable<Relacion> relacionesEstudiantePadre = new List<Relacion>();
                var codigoMadre = 0;
                var codigoPadre = 0;
                var codigoEstudiante = 0;
                foreach (var item in relacionesRepresentante)
                {
                    codigoEstudiante = item.PersonaId;
                    relacionesEstudianteMadre = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId == codigoEstudiante && x.TipoRelacionId == 1, null);
                    if(relacionesEstudianteMadre.Count()>0)
                    {
                        codigoMadre = relacionesEstudianteMadre.FirstOrDefault().Persona2Id;
                        madre = await _unitOfWork.Repository<Persona>().GetByIdAsync(codigoMadre);
                    }
                    

                    relacionesEstudiantePadre = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId == codigoEstudiante && x.TipoRelacionId == 2, null);
                    if(relacionesEstudiantePadre.Count() > 0)
                    {
                    codigoPadre = relacionesEstudiantePadre.FirstOrDefault().Persona2Id;
                    padre = await _unitOfWork.Repository<Persona>().GetByIdAsync(codigoPadre);
                    }
                    
                }

                ViewBag.Padre = padre;
                ViewBag.Madre = madre;

                return View("EditarRepresentante");

            }
        }
        [HttpPost]
        public async Task<IActionResult> EditarRepresentante(RepresentantesViewModel representantesViewModel)
        {
            if (representantesViewModel.NombresMadre != null && representantesViewModel.ApellidosMadre != null)
            {
                var EditarMadre = await _unitOfWork.Repository<Persona>().GetEntityAsync(x => x.Identificacion == representantesViewModel.IdentificacionMadre);
                if (EditarMadre != null)
                {
                    EditarMadre.Nombres = representantesViewModel.NombresMadre;
                    EditarMadre.Apellidos = representantesViewModel.ApellidosMadre;
                    EditarMadre.EmailPrincipal = representantesViewModel.CorreoMadre;
                    EditarMadre.TipoIdentificacionId = representantesViewModel.TipoIdentificacionIdMadre;
                    EditarMadre.Identificacion = representantesViewModel.IdentificacionMadre;
                    EditarMadre.Profesion = representantesViewModel.ProfesionMadre;
                    EditarMadre.Cargo = representantesViewModel.CargoMadre;
                    EditarMadre.LugarTrabajo = representantesViewModel.LugarTrabajoMadre;
                    EditarMadre.TelefonoMovil = representantesViewModel.CelularMadre;

                    if (representantesViewModel.AvatarMadre != null && representantesViewModel.AvatarMadre.Length > 0)
                    {
                        string numCedula = representantesViewModel.IdentificacionMadre;

                        string extensionArchivo = Path.GetExtension(representantesViewModel.AvatarMadre.FileName);

                        // Generar un nombre único para el archivo
                        string nombreArchivo = $"{numCedula}{extensionArchivo}";
                        EditarMadre.Avatar = nombreArchivo;
                        // Ruta de destino para guardar el archivo
                        string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                        string rutaImagenAnterior = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", EditarMadre.Avatar);
                        if (System.IO.File.Exists(rutaImagenAnterior))
                        {
                            System.IO.File.Delete(rutaImagenAnterior);
                        }

                        using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                        {
                            await representantesViewModel.AvatarMadre.CopyToAsync(stream);
                        }
                    }

                    if (representantesViewModel.PDFIdentificacionMadre != null)
                    {
                        string numCedula = representantesViewModel.IdentificacionMadre;
                        string extensionArchivo = Path.GetExtension(representantesViewModel.PDFIdentificacionMadre.FileName);

                        // Generar un nombre único para el archivo
                        string nombreArchivo = $"{numCedula}{extensionArchivo}";

                        string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacionPadres", nombreArchivo);

                        string[] archivosExistentes = Directory.GetFiles(Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacionPadres"));
                        string nombreArchivoPDFAnterior = archivosExistentes.FirstOrDefault(file => Path.GetFileName(file) == representantesViewModel.PDFIdentificacionMadre.FileName);

                        if (representantesViewModel.PDFIdentificacionMadre.FileName != Path.GetFileName(nombreArchivoPDFAnterior) && nombreArchivoPDFAnterior != null)
                        {
                            System.IO.File.Delete(nombreArchivoPDFAnterior);
                        }


                        using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                        {
                            await representantesViewModel.PDFIdentificacionMadre.CopyToAsync(stream);
                        }
                    }
                    await _unitOfWork.Repository<Persona>().UpdateAsync(EditarMadre);
                }

            }

            if (representantesViewModel.NombresPadre != null && representantesViewModel.ApellidosPadre != null)
            {
                var EditarPadre = await _unitOfWork.Repository<Persona>().GetEntityAsync(x => x.Identificacion == representantesViewModel.IdentificacionPadre);
                if (EditarPadre != null)
                {
                    EditarPadre.Nombres = representantesViewModel.NombresPadre;
                    EditarPadre.Apellidos = representantesViewModel.ApellidosPadre;
                    EditarPadre.EmailPrincipal = representantesViewModel.CorreoPadre;
                    EditarPadre.TipoIdentificacionId = representantesViewModel.TipoIdentificacionIdPadre;
                    EditarPadre.Identificacion = representantesViewModel.IdentificacionPadre;
                    EditarPadre.Profesion = representantesViewModel.ProfesionPadre;
                    EditarPadre.Cargo = representantesViewModel.CargoPadre;
                    EditarPadre.LugarTrabajo = representantesViewModel.LugarTrabajoPadre;
                    EditarPadre.TelefonoMovil = representantesViewModel.CelularPadre;

                    if (representantesViewModel.AvatarPadre != null && representantesViewModel.AvatarPadre.Length > 0)
                    {
                        string numCedula = representantesViewModel.IdentificacionPadre;
                        string extensionArchivo = Path.GetExtension(representantesViewModel.AvatarPadre.FileName);

                        // Generar un nombre único para el archivo
                        string nombreArchivo = $"{numCedula}{extensionArchivo}";
                        EditarPadre.Avatar = nombreArchivo;
                        // Ruta de destino para guardar el archivo
                        string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);
                        string rutaImagenAnterior = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", EditarPadre.Avatar);
                        if (System.IO.File.Exists(rutaImagenAnterior))
                        {
                            System.IO.File.Delete(rutaImagenAnterior);
                        }
                        using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                        {
                            await representantesViewModel.AvatarPadre.CopyToAsync(stream);
                        }
                    }

                    if (representantesViewModel.PDFIdentificacionPadre != null)
                    {
                        string numCedula = representantesViewModel.IdentificacionPadre;
                        string extensionArchivo = Path.GetExtension(representantesViewModel.PDFIdentificacionPadre.FileName);

                        // Generar un nombre único para el archivo
                        string nombreArchivo = $"{numCedula}{extensionArchivo}";

                        string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacionPadres", nombreArchivo);

                        string[] archivosExistentes = Directory.GetFiles(Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacionPadres"));
                        string nombreArchivoPDFAnterior = archivosExistentes.FirstOrDefault(file => Path.GetFileName(file) == representantesViewModel.PDFIdentificacionPadre.FileName);

                        if (representantesViewModel.PDFIdentificacionPadre.FileName != Path.GetFileName(nombreArchivoPDFAnterior) && nombreArchivoPDFAnterior != null)
                        {
                            System.IO.File.Delete(nombreArchivoPDFAnterior);
                        }


                        using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                        {
                            await representantesViewModel.PDFIdentificacionPadre.CopyToAsync(stream);
                        }
                    }
                    await _unitOfWork.Repository<Persona>().UpdateAsync(EditarPadre);

                }
            }
           
            else
            {
                return RedirectToAction("Representantes", "Datos");
            }

            return RedirectToAction("Representantes", "Datos");

        }
        public async Task<IActionResult> VerEstudiante()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var includes = new List<Expression<Func<Persona, object>>>
            {
                p => p.RelacionPersona2
            };

            //Buscar toda la representante
            var persona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.TipoEstudianteId != null, null, includes);
                
                return View(persona);
        }
    }
}
