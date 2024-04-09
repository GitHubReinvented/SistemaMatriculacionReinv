using Application.Contracts.Identity;
using Application.Models.Authorization;
using Application.Models.Email;
using Application.Persistence;
using AutoMapper;
using Domain;
using EllipticCurve.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReinventedPuembo.Interfaces;
using ReinventedPuembo.Models;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;



namespace ReinventedPuembo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumento _documento;
        private readonly IRegistroPagos _registroPagos;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IAuthService _authService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _configuration;

        private readonly IEmailServicePuembo _emailServicePuembo;
        private readonly IEmailServiceSantaClara _emailServiceSantaClara;

        public HomeController(
            ILogger<HomeController> logger,
            IUnitOfWork unitOfWork,
            IDocumento documento,
            IWebHostEnvironment hostEnvironment,
            IAuthService authService,
            UserManager<Usuario> userManager,
            IEmailServicePuembo emailServicePuembo,
            IEmailServiceSantaClara emailServiceSantaClara,
            IRegistroPagos registroPagos,
            IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _documento = documento;
            _registroPagos = registroPagos;
            _hostEnvironment = hostEnvironment;
            _authService = authService;
            _userManager = userManager;
            _emailServicePuembo = emailServicePuembo;
            _emailServiceSantaClara = emailServiceSantaClara;
            _configuration = configuration;

        }

        public async Task<IActionResult> Index()
        {
            var baseUri = _configuration.GetSection("LinkOptions")["url"];
            ViewBag.BaseUri = baseUri;
            var Ambiente = _configuration.GetSection("ambiente")["ambiente"];
            ViewBag.Ambiente = Ambiente;

            if (User.IsInRole(Role.REPRESENTANTE))
            {
                //Consulto para ver si la persona logeada tiene hijos registrados
                //Codigo Usuario logueandose
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                ViewBag.UsuarioID = userId;
                ViewBag.TokePagoM = Guid.NewGuid().ToString("N");
                ViewBag.TokePagoP = Guid.NewGuid().ToString("N");
                var includesc = new List<Expression<Func<Usuario, object>>>
                {
                    p => p.UsuarioSucursal!,
                };
                var user = await _unitOfWork.Repository<Usuario>()
                      .GetEntityAsync(
                      x => x.Id!.Equals(userId),
                      includesc,
                      false
                      );
                var sucursalEntity = user.UsuarioSucursal!.FirstOrDefault(x => x.UsuarioId!.Equals(user.Id));
                var suc = await _unitOfWork.Repository<Sucursal>().GetEntityAsync(y => y.Id == sucursalEntity!.SucursalId);

                var Sucursal = await _unitOfWork.Repository<Sucursal>().GetAsync(x => x.Nombre == suc!.Nombre!);
                var codigoSucursal = Sucursal.FirstOrDefault().Id;

                ViewBag.SucursalA = suc!.Nombre!;
                ViewBag.DatosSucursal = Sucursal.FirstOrDefault();
                ViewBag.NombreSucursal = Sucursal.FirstOrDefault().Nombre;
                ViewBag.EmailSucursal = Sucursal.FirstOrDefault().Email;




                var CicloEscolar = await _unitOfWork.Repository<CicloEscolar>().GetAsync(x => x.Estado == true && x.SucursalId == codigoSucursal);
                var cicloEscolarId = CicloEscolar.FirstOrDefault().Id;
                ViewBag.cicloEscolarId = cicloEscolarId;
                ViewBag.cicloEscolar = CicloEscolar.FirstOrDefault().FechaInicio.ToString("yyyy") + "-" + CicloEscolar.FirstOrDefault().FechaFin.ToString("yyyy");


                //busco lo registros  necesarios para los select en proceso
                var includesCuentas = new List<Expression<Func<CuentaSucursal, object>>>
                    {
                     p => p.Banco!,
                     p => p.TipoCuenta!

                    };

                var cuentasSucursal = await _unitOfWork.Repository<CuentaSucursal>().GetAsync(x => x.SucursalId == codigoSucursal && x.Id > 0, null, includesCuentas);
                var NivelEscolar = await _unitOfWork.Repository<NivelEscolar>().GetAllAsync();
                var TipoRelacion = await _unitOfWork.Repository<TipoRelacion>().GetAllAsync();
                var TipoIdentificacion = await _unitOfWork.Repository<TipoIdentificacion>().GetAllAsync();

                var Rubros = await _unitOfWork.Repository<Rubro>().GetAsync(x => x.CicloEscolarId == cicloEscolarId && x.TipoRubroId == 1);
                ViewBag.CostoMatricula = Rubros.FirstOrDefault().Costo;

                //Traigo los botones de Abitmedia
                var BotonPago1Sucursal = await _unitOfWork.Repository<BotonPagoSucursal>().GetAsync(x => x.SucursalId == codigoSucursal && x.BotonPagoId == 1);

                if (BotonPago1Sucursal.Count() > 0)
                {

                    var TokenAbitmediaPruebas = BotonPago1Sucursal.FirstOrDefault().TokenPruebas;
                    var TokenAbitmediaProduccion = BotonPago1Sucursal.FirstOrDefault().TokenProduccion;

                    if (Ambiente == "pruebas")
                        ViewBag.TokenAbitmedia = TokenAbitmediaPruebas;
                    else//producción
                        ViewBag.TokenAbitmedia = TokenAbitmediaProduccion;
                }
                else
                {
                    ViewBag.Error = "No se contraron parametros de botones de pago Abitmedia";
                    return View("Error");
                }

                //Traigo los botones de PayPhone
                /* comentado hasta completar desarrollo payphone */
                var BotonPago2Sucursal = await _unitOfWork.Repository<BotonPagoSucursal>().GetAsync(x => x.SucursalId == codigoSucursal && x.BotonPagoId == 2);

                if (BotonPago2Sucursal.Count() > 0)
                {

                    var TokenCorrientePayPhoneCorriente = BotonPago2Sucursal.FirstOrDefault().TokenPruebas;
                    var TokenVariosPayPhoneVarios = BotonPago2Sucursal.FirstOrDefault().TokenProduccion;

                    if (Ambiente == "pruebas")
                        ViewBag.TokenPayPhone = TokenCorrientePayPhoneCorriente;
                    else//producción
                        ViewBag.TokenPayPhone = TokenVariosPayPhoneVarios;
                }
                else
                {
                    //ViewBag.Error = "No se contraron parametros de botones de pago PayPhone";
                    //return View("Error");
                }



                ViewBag.NivelEscolar = NivelEscolar;
                ViewBag.TipoRelacion = TipoRelacion;
                ViewBag.TipoIdentificacion = TipoIdentificacion;
                ViewBag.sucursalColegio = codigoSucursal;
                ViewBag.cuentasSucursal = cuentasSucursal;

                List<ObligacionPendiente> ListaObligacionesMatricula = new List<ObligacionPendiente>();
                //no tiene registrado estudiantes en proceso de matriculación
                //busco que este en inscripciones
                var estudianteInscripcion = await _unitOfWork.Repository<EstudianteInscripcion>().GetAsync(x => x.UsuarioId == userId && x.procesado == false);
                ViewBag.NumeroHijosAProcesar = estudianteInscripcion.Count();

                if (estudianteInscripcion.Count() > 0)
                {

                    var DataEstudiante = estudianteInscripcion.FirstOrDefault();

                    var data_estudiante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion.Trim() == DataEstudiante.Identificacion.Trim());

                    if (data_estudiante.Count() > 0)
                    {
                        ViewBag.DatosEstudiante = data_estudiante.FirstOrDefault();
                        var codEstudiante = data_estudiante.FirstOrDefault().Id;
                        //mando los datos del estudiante para que se visualicen

                        //mando los datos del domicilio del estudiante
                        var domicilioEstudiante = await _unitOfWork.Repository<Direccion>().GetAsync(x => x.TipoDireccionId == 1 && x.PersonaId == codEstudiante);
                        ViewBag.domicilioEstudiante = domicilioEstudiante.FirstOrDefault();//para rellenar

                        var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codEstudiante && x.CicloEscolarId == cicloEscolarId);
                        var codigoMatricula = MatriculaEstudiante.FirstOrDefault().Id;
                        ViewBag.MatriculaEstudiante = MatriculaEstudiante.FirstOrDefault();

                        var includesFichaMedica = new List<Expression<Func<FichaMedica, object>>>
                        {
                         p => p.FichaObservacionMedica!,
                         p => p.HistorialMedico!

                        };
                        var FichaMedica = await _unitOfWork.Repository<FichaMedica>().GetAsync(x => x.MatriculaId == codigoMatricula, null, includesFichaMedica);
                        ViewBag.FichaMedica = FichaMedica.FirstOrDefault();//para rellenar
                    }
                    else
                    {
                        var pdataEstudiante = new Persona();
                        pdataEstudiante.TipoIdentificacionId = DataEstudiante.TipoIdentificacionId != null ? DataEstudiante.TipoIdentificacionId : 0;
                        pdataEstudiante.Nombres = DataEstudiante.Nombres != null ? DataEstudiante.Nombres : "";
                        pdataEstudiante.Apellidos = DataEstudiante.Apellidos != null ? DataEstudiante.Apellidos : "";
                        pdataEstudiante.Identificacion = DataEstudiante.Identificacion != null ? DataEstudiante.Identificacion : "";
                        pdataEstudiante.FechaNacimiento = DataEstudiante.FechaNacimiento != null ? DataEstudiante.FechaNacimiento : new DateTime(1900, 1, 1);//esto DateTime(1900, 1, 1) esta controlado en la interfaz del estudiante
                        ViewBag.DatosEstudiante = pdataEstudiante;
                    }


                    //solo para precarga de nivel escolar
                    if (DataEstudiante.Grado != null)
                    {
                        var DataMAtricula = new Matricula();
                        DataMAtricula.NivelEscolarId = DataEstudiante.Grado;
                        DataMAtricula.Id = 0;
                        ViewBag.MatriculaEstudiante = DataMAtricula;
                    }

                    var cod_estudianteAnterior = 0;

                    if (DataEstudiante.CedulaMadre != null)
                    {
                        //para precarga de datos madre
                        //pregunto si esta madre ya está registrada por un hijo anterior

                        var madre_estudiante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion.Trim() == DataEstudiante.CedulaMadre.Trim());

                        if (madre_estudiante.Count() > 0)
                        {
                            //si está registrada
                            ViewBag.DatosMadre = madre_estudiante.FirstOrDefault();
                            var cod_madre = madre_estudiante.FirstOrDefault().Id;
                            var relacion_madre = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == cod_madre);
                            cod_estudianteAnterior = relacion_madre.FirstOrDefault().PersonaId;

                            ViewBag.PreExisteMadre = "1";
                            if (madre_estudiante.FirstOrDefault().Representante == true)
                            {
                                ViewBag.nombresRepresentante = madre_estudiante.FirstOrDefault().Nombres;
                                ViewBag.apellidosRepresentante = madre_estudiante.FirstOrDefault().Apellidos;
                                ViewBag.TipIdRepresentante = madre_estudiante.FirstOrDefault().TipoIdentificacionId;
                                ViewBag.numIdRepresentante = madre_estudiante.FirstOrDefault().Identificacion;
                                ViewBag.EmailRepresentante = madre_estudiante.FirstOrDefault().EmailPrincipal;
                                ViewBag.TelfRepresentante = madre_estudiante.FirstOrDefault().TelefonoMovil;
                            }
                        }
                        else
                        {
                            var pdataMadre = new Persona();
                            pdataMadre.TipoIdentificacionId = 1;
                            pdataMadre.Identificacion = DataEstudiante.CedulaMadre != null ? DataEstudiante.CedulaMadre : "";
                            pdataMadre.Nombres = DataEstudiante.NombresMadre != null ? DataEstudiante.NombresMadre : "";
                            pdataMadre.Apellidos = DataEstudiante.ApellidosMadre != null ? DataEstudiante.ApellidosMadre : "";
                            pdataMadre.TelefonoMovil = DataEstudiante.MovilMadre != null ? DataEstudiante.MovilMadre : "";
                            pdataMadre.EmailPrincipal = DataEstudiante.EmailMadre != null ? DataEstudiante.EmailMadre : "";
                            ViewBag.DatosMadre = pdataMadre;
                        }
                    }

                    //para precarga datos padre
                    if (DataEstudiante.CedulaPadre != null)
                    {
                        //pregunto si esta padre ya está registrado por un hijo anterior

                        var padre_estudiante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion.Trim() == DataEstudiante.CedulaPadre.Trim());

                        if (padre_estudiante.Count() > 0)
                        {
                            //si está registrado
                            ViewBag.DatosPadre = padre_estudiante.FirstOrDefault();
                            ViewBag.PreExistePadre = "1";

                            var cod_padre = padre_estudiante.FirstOrDefault().Id;
                            var relacion_padre = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == cod_padre);
                            cod_estudianteAnterior = relacion_padre.FirstOrDefault().PersonaId;

                            if (padre_estudiante.FirstOrDefault().Representante == true)
                            {
                                ViewBag.nombresRepresentante = padre_estudiante.FirstOrDefault().Nombres;
                                ViewBag.apellidosRepresentante = padre_estudiante.FirstOrDefault().Apellidos;
                                ViewBag.TipIdRepresentante = padre_estudiante.FirstOrDefault().TipoIdentificacion;
                                ViewBag.numIdRepresentante = padre_estudiante.FirstOrDefault().Identificacion;
                                ViewBag.EmailRepresentante = padre_estudiante.FirstOrDefault().EmailPrincipal;
                                ViewBag.TelfRepresentante = padre_estudiante.FirstOrDefault().TelefonoMovil;
                            }
                        }
                        else
                        {

                            var pdataPadre = new Persona();
                            pdataPadre.TipoIdentificacionId = 1;
                            pdataPadre.Identificacion = DataEstudiante.CedulaPadre != null ? DataEstudiante.CedulaPadre : "";
                            pdataPadre.Nombres = DataEstudiante.NombresPadre != null ? DataEstudiante.NombresPadre : "";
                            pdataPadre.Apellidos = DataEstudiante.ApellidosPadre != null ? DataEstudiante.ApellidosPadre : "";
                            pdataPadre.TelefonoMovil = DataEstudiante.MovilPadre != null ? DataEstudiante.MovilPadre : "";
                            pdataPadre.EmailPrincipal = DataEstudiante.Emailpadre != null ? DataEstudiante.Emailpadre : "";

                            ViewBag.DatosPadre = pdataPadre;
                        }

                    }


                    var relacion_Autorizado = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId == cod_estudianteAnterior && x.TipoRelacionId != 1 && x.TipoRelacionId != 2);
                    if (relacion_Autorizado.Count() > 0)
                    {
                        var cod_autoriador = relacion_Autorizado.FirstOrDefault().Persona2Id;

                        var autorizado_estudiante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Id == cod_autoriador);
                        ViewBag.DatosPersonaAutorizadaRetirar = autorizado_estudiante.FirstOrDefault();
                        ViewBag.RelacionAutorizadoRetirar = relacion_Autorizado.FirstOrDefault();
                        ViewBag.PreExisteAutorizado = "1";

                    }
                    else
                    {
                        ViewBag.TareaCompletadaProceso = 2;

                        //var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == data_estudiante.FirstOrDefault().Id && x.CicloEscolarId == cicloEscolarId);

                        //var EstadoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().EstadoMatriculaId;

                        //if (EstadoMatriculaEstudiante == 3)//matrícula registrada, aún no acepta el contrato
                        //{
                        //    ViewBag.TareaCompletadaProceso = 3;
                        //    //tiene completada la tarea de registrar autorizado a retirar, le mando a ver docs seguro y contrato
                        //    return View("progreso");
                        //}
                    }
                    //busco persona autorizada a retirar

                    //--------------------------------------------
                    ViewBag.TareaCompletadaProceso = 0;
                    return View("progreso");



                }


                //---------------------------------------------------------------
                //Determinar proceso
                //Tiene pendiente un proceso de matriculación?
                //proceso de maticulación, registra:  estudiante > madre padre representante > persona retira >contrato > pago matrícula
                //un usuario puede tener relación con uno o más estudiantes
                //busco todos los estudiantes relacionados con el usuario

                var estudianteXusuario = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId == userId && x.Representante == false && x.TipoEstudianteId != null);

                if (estudianteXusuario.Count > 0)
                {
                    //el usuario tiene uno o más estudiantes
                    //si tiene estudiantes de uno en uno veo hasta donde llegó en el proceso de matriculación
                    foreach (var estudiante in estudianteXusuario)
                    {
                        var codEstudiante = estudiante.Id;
                        //mando los datos del estudiante para que se visualicen
                        ViewBag.DatosEstudiante = estudiante;//para rellenar
                                                             //mando los datos del domicilio del estudiante
                        var domicilioEstudiante = await _unitOfWork.Repository<Direccion>().GetAsync(x => x.TipoDireccionId == 1 && x.PersonaId == codEstudiante);
                        ViewBag.domicilioEstudiante = domicilioEstudiante.FirstOrDefault();//para rellenar

                        var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codEstudiante && x.CicloEscolarId == cicloEscolarId);
                        var codigoMatricula = MatriculaEstudiante.FirstOrDefault().Id;
                        ViewBag.MatriculaEstudiante = MatriculaEstudiante.FirstOrDefault();

                        var includesFichaMedica = new List<Expression<Func<FichaMedica, object>>>
                            {
                             p => p.FichaObservacionMedica!,
                             p => p.HistorialMedico!

                            };
                        var FichaMedica = await _unitOfWork.Repository<FichaMedica>().GetAsync(x => x.MatriculaId == codigoMatricula, null, includesFichaMedica);
                        ViewBag.FichaMedica = FichaMedica.FirstOrDefault();//para rellenar


                        //veo si el estudiante tiene registrado padre o madre
                        var RelacionPadresEstudiante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId == codEstudiante && (x.TipoRelacionId == 1 || x.TipoRelacionId == 2));

                        if (RelacionPadresEstudiante.Count > 0)
                        {
                            //el estudiante si tiene registrado padre y/o madre

                            foreach (var relacionEstudiante in RelacionPadresEstudiante)
                            {
                                if (relacionEstudiante.TipoRelacionId == 1) //es madre
                                {
                                    var DatosMadre = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Id == relacionEstudiante.Persona2Id);
                                    ViewBag.DatosMadre = DatosMadre.FirstOrDefault();//para rellenar

                                    if (DatosMadre?.FirstOrDefault()?.Representante == true)
                                    {
                                        ViewBag.nombresRepresentante = DatosMadre.FirstOrDefault().Nombres;
                                        ViewBag.apellidosRepresentante = DatosMadre.FirstOrDefault().Apellidos;
                                        ViewBag.TipIdRepresentante = DatosMadre.FirstOrDefault().TipoIdentificacionId;
                                        ViewBag.numIdRepresentante = DatosMadre.FirstOrDefault().Identificacion;
                                        ViewBag.EmailRepresentante = DatosMadre.FirstOrDefault().EmailPrincipal;
                                        ViewBag.TelfRepresentante = DatosMadre.FirstOrDefault().TelefonoMovil;
                                    }

                                }

                                if (relacionEstudiante.TipoRelacionId == 2) //es padre
                                {
                                    var DatosPadre = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Id == relacionEstudiante.Persona2Id);
                                    ViewBag.DatosPadre = DatosPadre.FirstOrDefault();//para rellenar
                                    if (DatosPadre?.FirstOrDefault()?.Representante == true)
                                    {
                                        ViewBag.nombresRepresentante = DatosPadre.FirstOrDefault().Nombres;
                                        ViewBag.apellidosRepresentante = DatosPadre.FirstOrDefault().Apellidos;
                                        ViewBag.TipIdRepresentante = DatosPadre.FirstOrDefault().TipoIdentificacion;
                                        ViewBag.numIdRepresentante = DatosPadre.FirstOrDefault().Identificacion;
                                        ViewBag.EmailRepresentante = DatosPadre.FirstOrDefault().EmailPrincipal;
                                        ViewBag.TelfRepresentante = DatosPadre.FirstOrDefault().TelefonoMovil;
                                    }
                                }

                            }
                            //veo si el estudiante tiene registrado persona autorizada a retirar diferente de padre y madre

                            var autorizadoRetirarEstudiante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId == codEstudiante && (x.TipoRelacionId != 1 && x.TipoRelacionId != 2));
                            if (autorizadoRetirarEstudiante.Count() > 0)
                            {
                                //ya tiene registrado autorizado a retirar

                                var DatosPersonaAutorizadaRetirar = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Id == autorizadoRetirarEstudiante.FirstOrDefault().Persona2Id && x.AutorizadoRetirar == true);
                                ViewBag.DatosPersonaAutorizadaRetirar = DatosPersonaAutorizadaRetirar.FirstOrDefault();//para rellenar
                                ViewBag.RelacionAutorizadoRetirar = autorizadoRetirarEstudiante.FirstOrDefault();
                                //pregunto si el estudiante tiene aprobado contrato para el ciclo escolar en curso

                                var EstadoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().EstadoMatriculaId;

                                if (EstadoMatriculaEstudiante == 3)//matrícula registrada, aún no acepta el contrato
                                {
                                    ViewBag.TareaCompletadaProceso = 3;
                                    //tiene completada la tarea de registrar autorizado a retirar, le mando a ver docs seguro y contrato
                                    return View("progreso");
                                }

                                if (EstadoMatriculaEstudiante == 4)
                                {
                                    //si pasa por aquí la matrícula tiene estado 4, aceptado contrato, entonces pasa al siguiente estudiante
                                    //hasta que todos los estudiantes tengan aceptados el contrato
                                    //luego se manda a pagar matrícula de todos
                                    var includesObligaciones = new List<Expression<Func<ObligacionPendiente, object>>>
                                        {
                                         p => p.Matricula!,
                                         p => p.Matricula.Persona!

                                        };

                                    var obligacionesMatricula = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatricula && x.RubroId == 1 && x.EstadoCuotaId == 2, null, includesObligaciones);
                                    if (obligacionesMatricula.Count() > 0)
                                        ListaObligacionesMatricula.Add(obligacionesMatricula.FirstOrDefault());
                                }



                            }
                            else
                            {
                                //no tiene registrado persona autorizada a retirar  kenny 12122023
                                //ViewBag.TareaCompletadaProceso = 2;//tiene registrado padre y/o madre
                                //return View("progreso");

                                try
                                {
                                    var DatosPersonaAutorizadaRetirar = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Id == autorizadoRetirarEstudiante.FirstOrDefault().Persona2Id && x.AutorizadoRetirar == true);
                                    ViewBag.DatosPersonaAutorizadaRetirar = DatosPersonaAutorizadaRetirar.FirstOrDefault();//para rellenar
                                    ViewBag.RelacionAutorizadoRetirar = autorizadoRetirarEstudiante.FirstOrDefault();
                                    //pregunto si el estudiante tiene aprobado contrato para el ciclo escolar en curso
                                }
                                catch (Exception e)
                                {


                                }


                                var EstadoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().EstadoMatriculaId;

                                if (EstadoMatriculaEstudiante == 3)//matrícula registrada, aún no acepta el contrato
                                {
                                    ViewBag.TareaCompletadaProceso = 3;
                                    //tiene completada la tarea de registrar autorizado a retirar, le mando a ver docs seguro y contrato
                                    return View("progreso");
                                }

                                if (EstadoMatriculaEstudiante == 4)
                                {
                                    //si pasa por aquí la matrícula tiene estado 4, aceptado contrato, entonces pasa al siguiente estudiante
                                    //hasta que todos los estudiantes tengan aceptados el contrato
                                    //luego se manda a pagar matrícula de todos
                                    var includesObligaciones = new List<Expression<Func<ObligacionPendiente, object>>>
                                        {
                                         p => p.Matricula!,
                                         p => p.Matricula.Persona!

                                        };

                                    var obligacionesMatricula = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatricula && x.RubroId == 1 && x.EstadoCuotaId == 2, null, includesObligaciones);
                                    if (obligacionesMatricula.Count() > 0)
                                        ListaObligacionesMatricula.Add(obligacionesMatricula.FirstOrDefault());
                                }

                            }
                        }
                        else
                        {
                            //no tiene registrado padre o madre
                            ViewBag.TareaCompletadaProceso = 1;//solo esta registrado estudiante

                            return View("progreso");
                        }
                    }

                    //sino tiene le mando a la tarea 2
                }
                else
                {
                    //no tiene registrado estudiantes


                    //--------------------------------------------
                    ViewBag.TareaCompletadaProceso = 0;
                    return View("progreso");
                }

                if (ListaObligacionesMatricula.Count() > 0)
                {
                    //cuando llega aquí quiere decir que ya terminó el proceso de matriculación de todos los estudiantes del usuario
                    //mando a pagar las oblogaciones de matrícula de uno o más estudiantes que se fueron recolectando anteriormente
                    ViewBag.TareaCompletadaProceso = 5;
                    ViewBag.ListaObligacionesMatricula = ListaObligacionesMatricula;
                    return View("progreso");
                }


                //1. si no esta reguistrado el representante
                var persona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);

                if (persona.Count <= 0)//si el usuario no existe como persona no pasó por registro  madre/padre/representante
                {

                    return View("Index");

                    //ViewBag.TareaCompletadaProceso = 1;
                    //return View("progreso");
                }


                //si el usuario existe como persona ya pasó por registro  estudiante/madre/padre/representante
                var idRepresentante = persona.FirstOrDefault().Id;

                var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == idRepresentante);
                var codigoEstudiante = 0;




                var codigoPersona = persona.FirstOrDefault().Id;
                var relacion = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id.Equals(codigoPersona), null);

                if (relacion.Count <= 0)
                    return View("Index");
                else
                {


                    var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);
                    var codigoRep = representante.FirstOrDefault().Id;
                    relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);
                    Persona padre = new Persona();
                    Persona madre = new Persona();
                    List<Persona> listaEstudiantes = new List<Persona>() { };
                    codigoEstudiante = 0;
                    IEnumerable<Relacion> relacionesEstudianteMadre = new List<Relacion>();
                    IEnumerable<Relacion> relacionesEstudiantePadre = new List<Relacion>();
                    List<ObligacionPendiente> obligacionesRepresentante = new List<ObligacionPendiente>();
                    IEnumerable<Matricula> datosmatricula = new List<Matricula>();
                    IEnumerable<Direccion> direccionEstudiante = new List<Direccion>();
                    IEnumerable<HistorialMedico> historialMedicoEstudiante = new List<HistorialMedico>();
                    IEnumerable<FichaMedica> fichaMedicaEstudiante = new List<FichaMedica>();
                    IEnumerable<AlumnoTransporte> rutaDireccion = new List<AlumnoTransporte>();


                    var codigoMadre = 0;
                    var codigoPadre = 0;

                    foreach (var item in relacionesRepresentante)
                    {
                        codigoEstudiante = item.PersonaId;
                        //busco las obligaciones pendientes del representante de pensiones y seguro estudiantil
                        //busco las matriculas por cada estudiante que represente
                        var includesMatricula = new List<Expression<Func<Matricula, object>>>
                        {
                            p => p.NivelEscolar!,

                        };

                        var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codigoEstudiante, null, includesMatricula);

                        var codigoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().Id;
                        datosmatricula = MatriculaEstudiante;





                        var direccion = await _unitOfWork.Repository<Direccion>().GetAsync(x => x.PersonaId == codigoEstudiante, null);

                        direccionEstudiante = direccion;

                        var rutaDireccionEstudiante = await _unitOfWork.Repository<AlumnoTransporte>().GetAsync(x => x.DireccionId == direccion.FirstOrDefault().Id, null);
                        rutaDireccion = rutaDireccionEstudiante;

                        var fichamed = await _unitOfWork.Repository<FichaMedica>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante);
                        fichaMedicaEstudiante = fichamed;

                        var historialEstudiante = await _unitOfWork.Repository<HistorialMedico>().GetAsync(x => x.FichaMedicaId == fichamed.FirstOrDefault().Id);
                        historialMedicoEstudiante = historialEstudiante;
                        //busco las obligaciones de esa matricula


                        var includesPersonaObligacion = new List<Expression<Func<ObligacionPendiente, object>>>
                        {
                            p => p.Matricula!,
                            p=>p.Matricula.Persona!

                        };
                        var obligacionesSeguro = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante && x.RubroId == 3 && (x.EstadoCuotaId == 2 || x.EstadoCuotaId == 4), null, includesPersonaObligacion);
                        if (obligacionesSeguro.Count() > 0)
                        {

                            //si no rechaza seguro muestro las pensiones
                            var obligacionesPensiones = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante && x.RubroId == 2 && x.EstadoCuotaId == 2, null, includesPersonaObligacion);
                            if (obligacionesPensiones.Count() > 0)
                            {
                                obligacionesRepresentante.Add(obligacionesSeguro.FirstOrDefault());
                                obligacionesRepresentante.Add(obligacionesPensiones.FirstOrDefault());
                            }

                        }


                        relacionesEstudianteMadre = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId == codigoEstudiante && x.TipoRelacionId == 1, null);
                        if (relacionesEstudianteMadre.Count() > 0)
                        {
                            codigoMadre = relacionesEstudianteMadre.FirstOrDefault().Persona2Id;
                            madre = await _unitOfWork.Repository<Persona>().GetByIdAsync(codigoMadre);
                        }


                        relacionesEstudiantePadre = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId == codigoEstudiante && x.TipoRelacionId == 2, null);
                        if (relacionesEstudiantePadre.Count() > 0)
                        {
                            codigoPadre = relacionesEstudiantePadre.FirstOrDefault().Persona2Id;
                            padre = await _unitOfWork.Repository<Persona>().GetByIdAsync(codigoPadre);
                        }


                        var includesPersonaMatricula = new List<Expression<Func<Persona, object>>>
                        {
                            p => p.Matricula!,

                        };

                        var estudiantes = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Id == codigoEstudiante, null, includesPersonaMatricula);
                        foreach (var estudiante in estudiantes)
                        {
                            listaEstudiantes.Add(estudiantes.FirstOrDefault());
                        }
                        //var estudiantes = await _unitOfWork.Repository<Persona>().GetByIdAsync(codigoEstudiante);
                        //listaEstudiantes.Add(estudiantes);
                    }


                    var includes = new List<Expression<Func<Servicio, object>>>
                        {
                            p => p.Sucursal!
                        };

                    var servicio = await _unitOfWork.Repository<Servicio>().GetAsync(x => x.Sucursal!.Nombre == suc!.Nombre!
                    , null, includes);


                    //codigoMatriculaEstudiante


                    ViewData["Padre"] = padre;
                    ViewData["Madre"] = madre;
                    ViewData["Representante"] = representante;
                    ViewBag.Estudiante = listaEstudiantes;
                    ViewBag.MatriculaEstudiante = datosmatricula;
                    ViewBag.ObligacionesRepresentante = obligacionesRepresentante;
                    ViewData["Servicio"] = servicio;
                    ViewBag.sucursalColegio = codigoSucursal;
                    ViewBag.fichaMedica = fichaMedicaEstudiante;
                    ViewBag.historialEstudiante = historialMedicoEstudiante;
                    ViewBag.direccionEstudiante = direccionEstudiante;
                    ViewBag.rutaEstudiante = rutaDireccion;
                    //return View("ResumenRepresentante", ViewBag.Estudiante);
                    return View("ResumenRepresentante", ViewBag.ObligacionesRepresentante);
                }

            }
            else if (User.IsInRole(Role.SUPERADMIN))
            {
                return RedirectToAction("Registrar", "Administracion");
            }
            else if (User.IsInRole(Role.ADMIN))
            {
                return RedirectToAction("RegistrarSucursal", "Administracion");
            }
            else if (User.IsInRole(Role.GUARDIA))
            {

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var includesc = new List<Expression<Func<Usuario, object>>>
                {
                    p => p.UsuarioSucursal!,
                };
                var user = await _unitOfWork.Repository<Usuario>()
                      .GetEntityAsync(
                      x => x.Id!.Equals(userId),
                      includesc,
                      false
                      );

                var sucursalEntity = user.UsuarioSucursal!.FirstOrDefault(x => x.UsuarioId!.Equals(user.Id));
                var sucur = await _unitOfWork.Repository<Sucursal>().GetEntityAsync(y => y.Id == sucursalEntity!.SucursalId);

                var codigoSucursal = sucur.Id;
                if (codigoSucursal == 1)
                {
                    return RedirectToAction("Puembo", "Guardia");
                }
                else if (codigoSucursal == 2)
                {
                    return RedirectToAction("SantaClara", "Guardia");
                }

            }
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> ObtenerDetallesEstudiante(int id)
        {
            try
            {
                var direccion = await _unitOfWork.Repository<Direccion>().GetAsync(x => x.PersonaId == id);
                var includesMatricula = new List<Expression<Func<Matricula, object>>>
                        {
                            p => p.NivelEscolar!,
                            p => p.Persona!
                        };
                var CuantasReferencias = await _unitOfWork.Repository<AlumnoTransporte>()
                    .GetAsync(x => x.DireccionId == direccion.FirstOrDefault().Id);
                bool registrado = false;
                if (CuantasReferencias != null)
                {
                    if (CuantasReferencias.Count > 0)
                        registrado = true;
                }
                var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == id, null, includesMatricula);
                if (MatriculaEstudiante != null)
                {
                    return Json(new
                    {
                        nombres = MatriculaEstudiante.FirstOrDefault()!.Persona!.Nombres,
                        apellidos = MatriculaEstudiante.FirstOrDefault()!.Persona!.Apellidos,
                        nivelEscolar = MatriculaEstudiante.FirstOrDefault()!.NivelEscolar!.Descripcion,
                        callePrincipal = direccion.FirstOrDefault().CallePrincipal,
                        calleSecundaria = direccion.FirstOrDefault().CalleSecundaria,
                        numero = direccion.FirstOrDefault().Numero,
                        regist = registrado
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    nombres = "" + e.Message,
                    apellidos = "",
                    nivelEscolar = "",
                    callePrincipal = "",
                    calleSecundaria = "",
                    numero = "",
                    regist = true
                });

            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> GrabarJustificacion(string NombreJustifica,
            string telfJustifica,
            string CedulaJustifica,
            string txtjustifica,
            string ObligacionesPensionesSeguro,
            string IdSeguro,
            string alumnoEst,
            string alumnoCel//aqui llega el código de pensión y seguro por cada estudiante, puede ser uno más alumnos
            )
        {

            txtjustifica = txtjustifica?.Replace('|', '\0');

            JustificaNoSeguro justifica = new JustificaNoSeguro();
            justifica.FechaRegistro = DateTime.Now;

            justifica.NombreRepresentante = NombreJustifica;
            justifica.TelefonoRepresentante = telfJustifica;
            justifica.IdentificacionRepresentante = CedulaJustifica;
            justifica.Justificacion = txtjustifica;// +"|"+ alumnoEst+"|"+ alumnoCel; 
            justifica.Estado = false;



            var justificacionSeguro = new JustificacionSeguro
            {
                Nombre = NombreJustifica,
                Cedula = CedulaJustifica,
                Telefono = telfJustifica,
                Justificacion = txtjustifica + "|" + alumnoEst + "|" + alumnoCel
            };



            _unitOfWork.Repository<JustificaNoSeguro>().AddEntity(justifica);
            var resultado = await _unitOfWork.Complete();

            if (resultado <= 0)
                return BadRequest("No se registro autorizado a retirar");


            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var includesc = new List<Expression<Func<Usuario, object>>>
                {
                    p => p.UsuarioSucursal!,
                };
            var user = await _unitOfWork.Repository<Usuario>()
                  .GetEntityAsync(
                  x => x.Id!.Equals(userId),
                  includesc,
                  false
                  );
            var sucursalEntity = user.UsuarioSucursal!.FirstOrDefault(x => x.UsuarioId!.Equals(user.Id));
            var suc = await _unitOfWork.Repository<Sucursal>().GetEntityAsync(y => y.Id == sucursalEntity!.SucursalId);





            var EmailSeguroPuembo = _configuration.GetSection("EmailSeguroEstudiantilPuembo")["To"];
            var EmailSeguroSC = _configuration.GetSection("EmailSeguroEstudiantilSantaClara")["To"];

            if (suc.Id == 1)
            {
                await _emailServicePuembo.SendEmailHtml(EmailSeguroPuembo, "Justificación de negativa del seguro de beca estudiantil", justificacionSeguro, "_JustifiacionNoSeguro");
            }
            else if (suc.Id == 2)
            {
                await _emailServiceSantaClara.SendEmailHtml(EmailSeguroSC, "Justificación de negativa del seguro de beca estudiantil", justificacionSeguro, "_JustifiacionNoSeguro");
            }

            //string[] datosCadenas = ObligacionesPensionesSeguro.ToString().Replace("[", "").Replace("]", "").Split(',');

            //int[] datosEnteros = Array.ConvertAll(datosCadenas, int.Parse);

            //foreach(var Obligacion in datosEnteros)//aqui llega obligaciones de pensión y seguro, solo actualizo el seguro a rechazado
            //{
            var registroObligacion = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.Id.Equals(Int32.Parse(IdSeguro)) && x.RubroId == 3);

            if (registroObligacion.Count() > 0)
            {

                ObligacionPendiente registro = new ObligacionPendiente();
                registro = registroObligacion.FirstOrDefault();
                registro.EstadoCuotaId = 6;//rechazo seguro

                _unitOfWork.Repository<ObligacionPendiente>().UpdateEntity(registro);
                var resultado2 = await _unitOfWork.Complete();

                if (resultado2 <= 0)
                {
                    return BadRequest("No se actualizo el estado de la obligacion a conciliar");
                }

            }
            // }


            return RedirectToAction("ObligacionesRepresentante", "Home");
        }


        public async Task<IActionResult> ObtenerPlantilla(string nombrePlantilla)
        {
            byte[] archivo = await _documento.ObtenerPlantilla(nombrePlantilla);
            if (archivo == null)
            {
                return BadRequest();
            }
            return File(archivo, "text/html");
        }
        public IActionResult Privacy()
        {
            return View();
        }




        public ActionResult GuardarPagoXTC(PagoXTarjeta model)
        {
            try
            {
                var respuesta = _registroPagos.GuardarPagoXTC(model);
                if (respuesta > 0)
                    return Ok(respuesta);
                else
                    return BadRequest("No se guardó el pago");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public ActionResult CambiarEstadoObligacionesEnCero(TransferenciaDTO model)
        {

            try
            {



                string[] datosCadenas = model.ObligacionPendienteId.ToString().Replace("[", "").Replace("]", "").Split(',');


                int[] datosEnteros = Array.ConvertAll(datosCadenas, int.Parse);


                model.auxiliar = datosEnteros;


                var respuesta = _registroPagos.GuardarPagoTransferencia(model, "");

                if (respuesta > 0)
                    return Ok(respuesta);
                else
                    return BadRequest("No se guardó el pago");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPagoTransferenciaMatricula(TransferenciaDTO model)
        {

            if (model.fotoTransferencia == null) { return BadRequest("La imagen no es valida"); }

            if (model.fotoTransferencia.Length == 0)
            {
                TempData["Error"] = "Archivo no valido";
                return RedirectToAction("Index");//pruebas
            }

            string numReferencia = model.numeroReferencia;
            string extensionArchivo = Path.GetExtension(model.fotoTransferencia.FileName);

            //if (extensionArchivo != ".jpg" && extensionArchivo != ".jpeg")
            //{

            //    return BadRequest("Solo se permite archivos jpg");
            //}

            // Generar un nombre único para el archivo
            string id = Guid.NewGuid().ToString("N");
            string nombreArchivo = $"{id}{extensionArchivo}";
            // Ruta de destino para guardar el archivo
            string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "ImagenPagoTransferencia", nombreArchivo);


            FileStream file = null;
            var filePath = Path.Combine(rutaGuardar);
            file = new FileStream(filePath, FileMode.Create);
            await model.fotoTransferencia.CopyToAsync(file);
            file.Close();

            try
            {




                string[] datosCadenas = model.ObligacionPendienteId.ToString().Replace("[", "").Replace("]", "").Split(',');

                int[] datosEnteros = Array.ConvertAll(datosCadenas, int.Parse);


                model.auxiliar = datosEnteros;


                var respuesta = _registroPagos.GuardarPagoTransferencia(model, id);

                if (respuesta > 0)
                    return Ok(respuesta);
                else
                    return BadRequest("No se guardó el pago");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }


        [HttpPost]
        public async Task<IActionResult> GuardarPagoTransferencia(TransferenciaDTO model)
        {

            string numReferencia = model.numeroReferencia;
            string extensionArchivo = Path.GetExtension(model.fotoTransferencia.FileName);






            if (model.fotoTransferencia.Length == 0)
            {
                TempData["Error"] = "Archivo no valido";
                return RedirectToAction("Index");//pruebas
            }

            if (extensionArchivo != ".jpg" && extensionArchivo != ".jpeg")
            {
                TempData["Error"] = "Solo se permite archivos jpg o .jpeg";
                return RedirectToAction("Index");//pruebas
            }
            // Generar un nombre único para el archivo
            string id = Guid.NewGuid().ToString("N");
            string nombreArchivo = $"{id}{extensionArchivo}";
            // Ruta de destino para guardar el archivo
            string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "ImagenPagoTransferencia", nombreArchivo);

            //using (var stream = new FileStream(rutaGuardar, FileMode.Create))
            //{
            // model.fotoTransferencia.CopyToAsync(stream);
            //stream.Dispose();                }
            //save file kenny 

            FileStream file = null;
            var filePath = Path.Combine(rutaGuardar);
            file = new FileStream(filePath, FileMode.Create);
            await model.fotoTransferencia.CopyToAsync(file);
            file.Close();


            //return RedirectToAction("Index");//pruebas
            try
            {

                string[] datosCadenas = model.ObligacionPendienteId.ToString().Replace("[", "").Replace("]", "").Split(',');

                int[] datosEnteros = Array.ConvertAll(datosCadenas, int.Parse);


                model.auxiliar = datosEnteros;


                var respuesta = _registroPagos.GuardarPagoTransferencia(model, id);
                if (respuesta > 0)
                    return RedirectToAction("ObligacionesRepresentante");
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }



        public async Task<ActionResult> ObligacionesRepresentante()
        {

            /*
            if (!User.IsInRole(Role.REPRESENTANTE))
            {
                TempData["Error"] = "Usuario no es representante";
                return RedirectToAction("Inde", "Home");
            }*/
            //Consulto para ver si la persona logeada tiene hijos registrados
            //Codigo Usuario logueandose
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;



            var includesc = new List<Expression<Func<Usuario, object>>>
                {
                    p => p.UsuarioSucursal!,
                };
            var user = await _unitOfWork.Repository<Usuario>()
                  .GetEntityAsync(
                  x => x.Id!.Equals(userId),
                  includesc,
                  false
                  );
            var sucursalEntity = user.UsuarioSucursal!.FirstOrDefault(x => x.UsuarioId!.Equals(user.Id));

            var Sucursal = await _unitOfWork.Repository<Sucursal>().GetAsync(x => x.Id == sucursalEntity!.SucursalId);
            var codigoSucursal = Sucursal.FirstOrDefault().Id;

            var CicloEscolar = await _unitOfWork.Repository<CicloEscolar>().GetAsync(x => x.Estado == true && x.SucursalId == codigoSucursal);
            var cicloEscolarId = CicloEscolar.FirstOrDefault().Id;
            //ViewBag.cicloEscolar = CicloEscolar.FirstOrDefault().FechaInicio.ToString("yyyy") + "-" + CicloEscolar.FirstOrDefault().FechaFin.ToString("yyyy");
            //Buscar todos sus representados
            var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);
            var codigoRep = representante.FirstOrDefault().Id;
            var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);


            List<Persona> listaEstudiantes = new List<Persona>();
            var codigoEstudiante = 0;


            List<ObligacionPendiente> obligacionesRepresentante = new List<ObligacionPendiente>();


            ViewBag.ObligacionesRepresentante = "";

            foreach (var item in relacionesRepresentante)
            {
                codigoEstudiante = item.PersonaId;
                //busco las obligaciones del representante en base a las matriculas
                //busco las matriculas por cada estudiante que represente
                var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codigoEstudiante, null);
                var codigoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().Id;
                //busco las obligaciones de esa matricula

                var includes = new List<Expression<Func<ObligacionPendiente, object>>>
                        {
                            p => p.EstadoCuota!,
                            p => p.Rubro.TipoRubro!,
                            p => p.Matricula.Persona!,

                        };

                var obligaciones = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante, null, includes);

                foreach (var obligacion in obligaciones)
                {
                    obligacionesRepresentante.Add(obligacion);
                }


                //ViewData["Obligaciones"] = obligaciones;
                var estudiantes = await _unitOfWork.Repository<Persona>().GetByIdAsync(codigoEstudiante);
                listaEstudiantes.Add(estudiantes);
            }




            //ViewData["Representante"] = representante;
            ViewBag.Estudiante = listaEstudiantes;


            ViewBag.sucursalColegio = codigoSucursal;
            return View(obligacionesRepresentante);

            //return View(ViewBag.ObligacionesRepresentante);
        }




        public async Task<ActionResult> CargaTextoParaPdf(string textoTopdf, string cedulaEstudiante, string codSucursal, string CicloEscolar)
        {

            var nombreDocGenerado = _documento.GenerarPDFcontrato(textoTopdf, cedulaEstudiante, codSucursal, CicloEscolar);

            if (nombreDocGenerado != null)
            {
                //var baseUri = $"{Request.Scheme}://{Request.Host}";
                var baseUri = _configuration.GetSection("LinkOptions")["url"];

                baseUri = baseUri + "DocsLegales/" + nombreDocGenerado.Result;
                return Ok(baseUri);
            }

            else
                return BadRequest("No se ha generado el PDF");
        }


        public async Task<IActionResult> GrabaAutorizadosRetirar(AutRetirarViewModel autRetirarViewModel)
        {
            if (autRetirarViewModel == null)
                return BadRequest("No llegaron datos");

            if (autRetirarViewModel.CedulaAlumno.IsNullOrEmpty())
                return BadRequest("No llego cédula del estudiante");

            string cedulaEstudiante = autRetirarViewModel.CedulaAlumno;


            if (autRetirarViewModel.Identificacion == null || autRetirarViewModel.Identificacion.Equals(""))
            {
                return Ok();
            }

            var datosEstudiante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion.Equals(cedulaEstudiante), null);

            if (datosEstudiante.IsNullOrEmpty())
                return BadRequest("No existe registro de estudiante con esa cédula");

            int codigoEstudiante = datosEstudiante.FirstOrDefault().Id;

            var ConsultaExitePersona1 = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(autRetirarViewModel.Identificacion), null);

            if (ConsultaExitePersona1 != null && ConsultaExitePersona1.Count > 0)
            { //ya Existe la persona, se trata de otro niño para registrar autorización


                var personaId2 = ConsultaExitePersona1.FirstOrDefault().Id;
                var existe = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId!.Equals(codigoEstudiante) && x.Persona2Id!.Equals(personaId2) && x.TipoRelacionId!.Equals(autRetirarViewModel.TipoRelacionId), null);
                if (existe.Count() == 0)
                {
                    var relacion2 = new Relacion();
                    relacion2.PersonaId = codigoEstudiante;
                    relacion2.Persona2Id = personaId2;
                    relacion2.TipoRelacionId = autRetirarViewModel.TipoRelacionId;

                    _unitOfWork.Repository<Relacion>().AddEntity(relacion2);

                    var resultRelacion2 = await _unitOfWork.Complete();

                    if (resultRelacion2 <= 0)
                        return BadRequest("No se registró relación con autorizado a retirar");
                }
                return Ok();
            }


            var persona = new Persona();
            persona.Nombres = autRetirarViewModel.Nombres;
            persona.Apellidos = autRetirarViewModel.Apellidos;
            persona.TipoIdentificacionId = autRetirarViewModel.TipoIdentificacionId;
            persona.Identificacion = autRetirarViewModel.Identificacion;
            persona.TelefonoMovil = autRetirarViewModel.TelefonoMovil;
            persona.AutorizadoRetirar = true;
            persona.SucursalId = autRetirarViewModel.SucursalId;

            if (autRetirarViewModel.Avatar != null && autRetirarViewModel.Avatar.Length > 0)
            {
                // Obtener el nombre y la extensión del archivo
                string numCedula = autRetirarViewModel.Identificacion;
                string extensionArchivo = Path.GetExtension(autRetirarViewModel.Avatar.FileName);

                if (!(extensionArchivo.ToLower().Equals(".png") || extensionArchivo.ToLower().Equals(".jpg")
                || extensionArchivo.ToLower().Equals(".jpeg")))
                {
                    return BadRequest("Foto Autorización de retiro, seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)");
                }

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

            if (autRetirarViewModel.PDFIdentificacion != null)
            {
                string numCedula = autRetirarViewModel.Identificacion;
                string extensionArchivo = Path.GetExtension(autRetirarViewModel.PDFIdentificacion.FileName);

                if (!extensionArchivo.ToLower().Equals(".pdf"))
                {
                    return BadRequest("Pdf Cédula Autorización de retiro, seleccione un archivo con las siguiente extensión  (pdf)");
                }

                // Generar un nombre único para el archivo
                string nombreArchivo = $"{numCedula}{extensionArchivo}";

                string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "DocsIdAutorizadosRetiro", nombreArchivo);

                string[] archivosExistentes = Directory.GetFiles(Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacion"));
                string nombreArchivoPDFAnterior = archivosExistentes.FirstOrDefault(file => Path.GetFileName(file) == autRetirarViewModel.PDFIdentificacion.FileName);

                if (autRetirarViewModel.PDFIdentificacion.FileName != Path.GetFileName(nombreArchivoPDFAnterior) && nombreArchivoPDFAnterior != null)
                {
                    System.IO.File.Delete(nombreArchivoPDFAnterior);
                }


                using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                {
                    await autRetirarViewModel.PDFIdentificacion.CopyToAsync(stream);
                }
            }
            _unitOfWork.Repository<Persona>().AddEntity(persona);

            var resultPersona = await _unitOfWork.Complete();

            if (resultPersona <= 0)
                return BadRequest("No se registro autorizado a retirar");

            var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(autRetirarViewModel.Identificacion), null);

            if (resultadoPersona.IsNullOrEmpty())
                return BadRequest("No se registro autorizado a retirar");

            var personaId = resultadoPersona.FirstOrDefault()!.Id;

            var relacion = new Relacion();
            relacion.PersonaId = codigoEstudiante;
            relacion.Persona2Id = personaId;
            relacion.TipoRelacionId = autRetirarViewModel.TipoRelacionId;

            _unitOfWork.Repository<Relacion>().AddEntity(relacion);

            var resultRelacion = await _unitOfWork.Complete();

            if (resultRelacion <= 0)
                return BadRequest("No se registró relación con autorizado a retirar");



            return Ok();

        }

        public async Task<IActionResult> GrabaPadres(RepresentantesViewModel representantesViewModel)
        {

            if (representantesViewModel == null)
            {

                return BadRequest("No hay datos de padres");

            }

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

                return BadRequest("Solo debe existir un representnte");
            }

            if (representantesViewModel.RepresentantePadre == false && representantesViewModel.RepresentanteMadre == false)
            {

                return BadRequest("No registraron representante");
            }

            if (representantesViewModel.RepresentantePadre == null || representantesViewModel.RepresentanteMadre == null)
            {

                return BadRequest("No registraron representante");
            }

            //busca el usuario que inició sesion para relacionarlo con el representante, el representante es el que tiene acceso
            var currentUser = _authService.GetSessionUser();
            var user = await _userManager.FindByIdAsync(currentUser);

            string cedulaEstudiante = representantesViewModel.CedulaHijo;

            if (cedulaEstudiante.IsNullOrEmpty())
            {
                return BadRequest("No se registró cedula del estudiante");
            }
            // busca el código del hijo para usarlo en la relación con padre y madre
            int codigoHijo;
            var datosHijo = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion.Equals(cedulaEstudiante), null);
            codigoHijo = datosHijo.FirstOrDefault().Id;

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            try
            {
                var resultadoPersona1 = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(representantesViewModel.IdentificacionPadre), null);
                var resultadoPersona2 = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(representantesViewModel.IdentificacionMadre), null);

                if (resultadoPersona1 == null || resultadoPersona1.Count <= 0)
                {
                    string extPadreArchivo = Path.GetExtension(representantesViewModel.AvatarPadre.FileName);
                    string extPadreArchivoPdf = Path.GetExtension(representantesViewModel.PDFIdentificacionPadre.FileName);

                    if (!(extPadreArchivo.ToLower().Equals(".png") || extPadreArchivo.ToLower().Equals(".jpg")
                        || extPadreArchivo.ToLower().Equals(".jpeg")))
                    {
                        return BadRequest("Foto Padre, seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)");
                    }

                    if (!extPadreArchivoPdf.ToLower().Equals(".pdf"))
                    {
                        return BadRequest("Pdf Cédula Padre, seleccione un archivo con las siguiente extensión  (pdf)");
                    }
                }

                if (resultadoPersona2 == null || resultadoPersona2.Count <= 0)
                {                 

                    string extMadrenArchivo = Path.GetExtension(representantesViewModel.AvatarMadre.FileName);
                    string extMadreArchivoPdf = Path.GetExtension(representantesViewModel.PDFIdentificacionMadre.FileName);

                    if (!(extMadrenArchivo.ToLower().Equals(".png") || extMadrenArchivo.ToLower().Equals(".jpg")
                        || extMadrenArchivo.ToLower().Equals(".jpeg")))
                    {
                        return BadRequest("Foto Madre, seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)");
                    }

                    if (!extMadreArchivoPdf.ToLower().Equals(".pdf"))
                    {
                        return BadRequest("Pdf Cédula Madre, seleccione un archivo con las siguiente extensión  (pdf)");
                    }
                }

                if (!representantesViewModel.NombresPadre.IsNullOrEmpty())
                {
                    if (resultadoPersona1 == null || resultadoPersona1.Count <= 0)
                    {
                        //en padre no existe

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
                        padre.Cargo = representantesViewModel.CargoPadre;
                        padre.Representante = (bool)representantesViewModel.RepresentantePadre;
                        padre.AutorizadoRetirar = true;//por default puede retirar a su hijo
                        padre.SucursalId = representantesViewModel.SucursalId;
                        //padre.UsuarioId = ;
                        if (padre.Representante == true)
                        {
                            padre.UsuarioId = userId;

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


                        if (representantesViewModel.PDFIdentificacionPadre != null)
                        {
                            string numCedula = representantesViewModel.IdentificacionPadre;
                            string extensionArchivo = Path.GetExtension(representantesViewModel.PDFIdentificacionPadre.FileName);

                            // Generar un nombre único para el archivo
                            string nombreArchivo = $"{numCedula}{extensionArchivo}";

                            string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacionPadres", nombreArchivo);

                            string[] archivosExistentes = Directory.GetFiles(Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacion"));
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

                        _unitOfWork.Repository<Persona>().AddEntity(padre);

                        var resultPersona = await _unitOfWork.Complete();

                        if (resultPersona <= 0)
                        {

                            return BadRequest("Error al registrar padre");
                        }

                        //busca el código del padre que se genero para usarlo en la relación
                        var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(representantesViewModel.IdentificacionPadre), null);
                        var personaId2 = resultadoPersona.FirstOrDefault()!.Id;



                        var relacion = new Relacion();
                        relacion.PersonaId = codigoHijo;
                        relacion.Persona2Id = personaId2;
                        relacion.TipoRelacionId = 2; //2 es Padre

                        _unitOfWork.Repository<Relacion>().AddEntity(relacion);

                        var resultRelacion = await _unitOfWork.Complete();

                        if (resultRelacion <= 0)
                        {

                            return BadRequest("Error registro relación padre");
                        }

                    }
                    else
                    {
                        //el padre ya existe puede tratarse de otro hijo
                        var personaId2 = resultadoPersona1.FirstOrDefault()!.Id;
                        var existe = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId!.Equals(codigoHijo) && x.Persona2Id!.Equals(personaId2) && x.TipoRelacionId!.Equals(2), null);
                        if (existe.Count() == 0)
                        {
                            var relacion = new Relacion();
                            relacion.PersonaId = codigoHijo;
                            relacion.Persona2Id = personaId2;
                            relacion.TipoRelacionId = 2; //2 es Padre

                            _unitOfWork.Repository<Relacion>().AddEntity(relacion);

                            var resultRelacion = await _unitOfWork.Complete();
                            if (resultRelacion <= 0)
                            {

                                return BadRequest("Error registro relación padre");
                            }
                        }

                    }

                }


                if (!representantesViewModel.NombresMadre.IsNullOrEmpty())
                {
                    if (resultadoPersona2 == null || resultadoPersona2.Count <= 0)
                    {
                        //no existe registro de madre entonces ingreso

                        var madre = new Persona();
                        madre.Nombres = representantesViewModel.NombresMadre;
                        madre.Apellidos = representantesViewModel.ApellidosMadre;
                        madre.TipoIdentificacionId = representantesViewModel.TipoIdentificacionIdMadre;
                        madre.Identificacion = representantesViewModel.IdentificacionMadre;
                        madre.Profesion = representantesViewModel.ProfesionMadre;
                        madre.LugarTrabajo = representantesViewModel.LugarTrabajoMadre;
                        madre.TelefonoMovil = representantesViewModel.CelularMadre;
                        madre.EmailPrincipal = representantesViewModel.EmailPrincipalMadre;
                        madre.Cargo = representantesViewModel.CargoMadre;
                        madre.Representante = (bool)representantesViewModel.RepresentanteMadre;
                        madre.AutorizadoRetirar = true;//por default la madre esta autorizada a retirar
                        madre.SucursalId = representantesViewModel.SucursalId;

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
                        if (representantesViewModel.PDFIdentificacionMadre != null)
                        {
                            string numCedula = representantesViewModel.IdentificacionMadre;
                            string extensionArchivo = Path.GetExtension(representantesViewModel.PDFIdentificacionMadre.FileName);

                            // Generar un nombre único para el archivo
                            string nombreArchivo = $"{numCedula}{extensionArchivo}";

                            string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacionPadres", nombreArchivo);

                            string[] archivosExistentes = Directory.GetFiles(Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "documentosIdentificacion"));
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
                        _unitOfWork.Repository<Persona>().AddEntity(madre);

                        var resultMadre = await _unitOfWork.Complete();

                        if (resultMadre <= 0)
                        {

                            return BadRequest("Error registro madre");
                        }

                        //busco el código de la madre que se generó
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

                            return BadRequest("Error registro relación madre");
                        }
                    }
                    else
                    {
                        //la madre ya existe puede tratarse de otro hijo
                        var personaId2 = resultadoPersona2.FirstOrDefault()!.Id;
                        var existe = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.PersonaId!.Equals(codigoHijo) && x.Persona2Id!.Equals(personaId2) && x.TipoRelacionId!.Equals(1), null);
                        if (existe.Count() == 0)
                        {
                            var relacionM = new Relacion();
                            relacionM.TipoRelacionId = 1; //1 de Madre
                            relacionM.PersonaId = codigoHijo;
                            relacionM.Persona2Id = personaId2;

                            _unitOfWork.Repository<Relacion>().AddEntity(relacionM);

                            var resultRelacionM = await _unitOfWork.Complete();

                            if (resultRelacionM <= 0)
                            {

                                return BadRequest("Error registro relación madre");
                            }
                        }
                    }

                }
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        public async Task<IActionResult> AceptarContrato(int matricula, decimal valorMatricula)
        {
            //lega de aceptar contrato
            // cambio el estado de la matrícula de registrada(3) a aceptada (4) ya que el usuario aceptó el contrato
            // la matrícula se generó cuando se registro el estudiante
            //busco el registro de la matrícula que me llega
            Matricula matriculaEstudiante = new Matricula();

            var includesMatriculaPersona = new List<Expression<Func<Matricula, object>>>
            {
              p => p.Persona!,
            };

            var registroMatricula = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.Id.Equals(matricula), null, includesMatriculaPersona);

            if (registroMatricula == null || registroMatricula.Count <= 0)
            {
                return BadRequest("No existe matrícula");
            }

            matriculaEstudiante = registroMatricula.FirstOrDefault();
            matriculaEstudiante.EstadoMatriculaId = 4;
            _unitOfWork.Repository<Matricula>().UpdateEntity(matriculaEstudiante);
            var resultado = await _unitOfWork.Complete();

            if (resultado <= 0)
            {

                return BadRequest("No se actualizo el estado de la matrícula");
            }

            #region generar cuota de matrícula -------------------------------------------------------------------------------------
            //busco descuentos y abonos a matricula en inscripcion del estudiante
            var cedulaEstudiante = registroMatricula.FirstOrDefault().Persona.Identificacion;

            decimal PorcentajeDescuentoMatricula = 0;
            decimal AbonoMatricula = 0;

            decimal PorcentajeDescuentoPension = 0;
            decimal AbonoPension = 0;

            decimal PorcentajeDescuentoSeguro = 0;
            decimal AbonoSeguro = 0;

            var InscripcionEstudiante = await _unitOfWork.Repository<EstudianteInscripcion>().GetAsync(x => x.Identificacion == cedulaEstudiante);

            if (InscripcionEstudiante.Count() > 0)
            {
                PorcentajeDescuentoMatricula = InscripcionEstudiante.FirstOrDefault().MatriculaDescuento == null ? 0 : (decimal)InscripcionEstudiante.FirstOrDefault().MatriculaDescuento;
                AbonoMatricula = InscripcionEstudiante.FirstOrDefault().MatriculaAbono == null ? 0 : (decimal)InscripcionEstudiante.FirstOrDefault().MatriculaAbono;

                PorcentajeDescuentoPension = InscripcionEstudiante.FirstOrDefault().PensionDescuento == null ? 0 : (decimal)InscripcionEstudiante.FirstOrDefault().PensionDescuento;
                AbonoPension = InscripcionEstudiante.FirstOrDefault().PensionAbono == null ? 0 : (decimal)InscripcionEstudiante.FirstOrDefault().PensionAbono;

                PorcentajeDescuentoSeguro = InscripcionEstudiante.FirstOrDefault().SeguroDescuento == null ? 0 : (decimal)InscripcionEstudiante.FirstOrDefault().SeguroDescuento;
                AbonoSeguro = InscripcionEstudiante.FirstOrDefault().SeguroAbono == null ? 0 : (decimal)InscripcionEstudiante.FirstOrDefault().SeguroAbono;

            }
            ObligacionPendiente cuotaMatricula = new ObligacionPendiente();

            cuotaMatricula.MatriculaId = matricula;//número matrícula
            cuotaMatricula.Cuota = 1;//la matrícula solo tiene una cuota
            cuotaMatricula.Valor = valorMatricula;//aqui va el valor que este en la base de datos en base a la sucursal
            cuotaMatricula.PorcentajeDescuento = PorcentajeDescuentoMatricula;
            decimal descuento = valorMatricula * PorcentajeDescuentoMatricula / 100;
            cuotaMatricula.Descuento = descuento;
            decimal valorFinal = valorMatricula - descuento;
            cuotaMatricula.ValorFinal = valorFinal;
            cuotaMatricula.Abono = AbonoMatricula;
            cuotaMatricula.Saldo = valorFinal - AbonoMatricula;
            cuotaMatricula.EstadoCuotaId = 2;//pendiente de pago
            cuotaMatricula.ServicioId = null;//no se trata de un servicio
            cuotaMatricula.RubroId = 1;//es una matrícula

            string FilaOblicacion = "<td>" + registroMatricula.FirstOrDefault().Persona.Nombres + " " + registroMatricula.FirstOrDefault().Persona.Apellidos + "</td ><td>Matrícula</td ><td>$</td><td>" + cuotaMatricula.Valor + "</td ><td>$</td><td>" + cuotaMatricula.Descuento + "</td ><td>$</td><td>" + cuotaMatricula.Abono + "</td ><td>$</td><td>" + cuotaMatricula.Saldo + "</td >";

            var existe = await _unitOfWork.Repository<ObligacionPendiente>().GetEntityAsync(x => x.MatriculaId == matricula && x.RubroId == 1);
            if (existe == null)
            {
                _unitOfWork.Repository<ObligacionPendiente>().AddEntity(cuotaMatricula);
                var resultado2 = await _unitOfWork.Complete();

                if (resultado2 <= 0)
                {

                    return BadRequest("No se registro obligación de matrícula");
                }

            }



            var resultadoObligacion = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == matricula && x.RubroId == 1);

            var obligacionId = resultadoObligacion.FirstOrDefault().Id;

            if (obligacionId <= 0)
            {

                return BadRequest("No se obtuvo código de obligación");
            }
            #endregion
            //genero las pensión------------------------------------------------------------------------------
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var includesc = new List<Expression<Func<Usuario, object>>>
                {
                    p => p.UsuarioSucursal!,
                };
            var user = await _unitOfWork.Repository<Usuario>()
                  .GetEntityAsync(
                  x => x.Id!.Equals(userId),
                  includesc,
                  false
                  );

            var sucursalEntity = user.UsuarioSucursal!.FirstOrDefault(x => x.UsuarioId!.Equals(user.Id));
            var sucur = await _unitOfWork.Repository<Sucursal>().GetEntityAsync(y => y.Id == sucursalEntity!.SucursalId);

            var Sucursal = await _unitOfWork.Repository<Sucursal>().GetAsync(x => x.Nombre == sucur!.Nombre!);
            var codigoSucursal = Sucursal.FirstOrDefault().Id;

            var CicloEscolar = await _unitOfWork.Repository<CicloEscolar>().GetAsync(x => x.Estado == true && x.SucursalId == codigoSucursal);
            var cicloEscolarId = CicloEscolar.FirstOrDefault().Id;
            int numeroPeriodosCiclo = CicloEscolar.FirstOrDefault().NumeroPeriodos;

            var RubrosPension = await _unitOfWork.Repository<Rubro>().GetAsync(x => x.CicloEscolarId == cicloEscolarId && x.TipoRubroId == 2);
            var CostoPension = RubrosPension.FirstOrDefault().Costo;

            var CostoPensionAnual = numeroPeriodosCiclo * CostoPension;



            // genero cuota de pensión anual
            ObligacionPendiente PensionAnual = new ObligacionPendiente();

            PensionAnual.MatriculaId = matricula;//número matrícula
            PensionAnual.Cuota = 1;//una sola pensión anula
            PensionAnual.Valor = (decimal)CostoPensionAnual;//aqui va el valor que este en la base de datos en base a la sucursal
            PensionAnual.PorcentajeDescuento = PorcentajeDescuentoPension;
            decimal descuentoPension = (decimal)CostoPensionAnual * PorcentajeDescuentoPension / 100;
            PensionAnual.Descuento = descuentoPension;
            decimal valorFinalPension = (decimal)CostoPensionAnual - descuentoPension;
            PensionAnual.ValorFinal = valorFinalPension;

            PensionAnual.Abono = AbonoPension;
            PensionAnual.Saldo = valorFinalPension - AbonoPension;
            PensionAnual.EstadoCuotaId = 2;//pendiente de pago
            PensionAnual.ServicioId = null;//no se trata de un servicio
            PensionAnual.RubroId = 2;//es una pensión

            var existeP = await _unitOfWork.Repository<ObligacionPendiente>().GetEntityAsync(x => x.MatriculaId == matricula && x.RubroId == 2);
            if (existeP == null)
            {
                _unitOfWork.Repository<ObligacionPendiente>().AddEntity(PensionAnual);
                var resultadoPension = await _unitOfWork.Complete();

                if (resultadoPension <= 0)
                {

                    return BadRequest("No se registro obligación de pensión anual");
                }
            }


            var resultadoObligacionPension = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == matricula && x.RubroId == 2);

            var obligacionIdPension = resultadoObligacionPension.FirstOrDefault().Id;

            if (obligacionIdPension <= 0)
            {

                return BadRequest("No se obtuvo código de obligación pensión");
            }

            //creo obligacion para seguro estudiantil

            // el seguro estudiantil es por nivel escolar
            var RubrosSeguroEstudantilNivelEscolar = await _unitOfWork.Repository<NivelEscolar>().GetAsync(x => x.Id == matriculaEstudiante.NivelEscolarId);
            var CostoSeguroEstudiantil = RubrosSeguroEstudantilNivelEscolar.FirstOrDefault().CostoSeguroEstudiantil;
            //ViewBag.cicloEscolar = CicloEscolar.FirstOrDefault().FechaInicio.ToString("yyyy") + "-" + CicloEscolar.FirstOrDefault().FechaFin.ToString("yyyy");

            ObligacionPendiente SeguroAnual = new ObligacionPendiente();

            SeguroAnual.MatriculaId = matricula;//número de matrícula
            SeguroAnual.Cuota = 1;//una sola cuota  seguro anual
            SeguroAnual.Valor = (decimal)CostoSeguroEstudiantil;//aqui va el valor que este en la base de datos en base a la sucursal
            SeguroAnual.PorcentajeDescuento = PorcentajeDescuentoSeguro;
            decimal descuentoSeguro = (decimal)CostoSeguroEstudiantil * PorcentajeDescuentoSeguro / 100;
            SeguroAnual.Descuento = descuentoSeguro;
            decimal valorFinalSeguro = (decimal)CostoSeguroEstudiantil - descuentoSeguro;
            SeguroAnual.ValorFinal = valorFinalSeguro;

            SeguroAnual.Abono = AbonoSeguro;
            SeguroAnual.Saldo = valorFinalSeguro - AbonoSeguro;
            SeguroAnual.EstadoCuotaId = 2;//pendiente de pago
            SeguroAnual.ServicioId = null;//no se trata de un servicio
            SeguroAnual.RubroId = 3;//es seguro estudiantil

            var existeS = await _unitOfWork.Repository<ObligacionPendiente>().GetEntityAsync(x => x.MatriculaId == matricula && x.RubroId == 3);
            if (existeS == null)
            {
                _unitOfWork.Repository<ObligacionPendiente>().AddEntity(SeguroAnual);
                var resultadoSeguro = await _unitOfWork.Complete();

                if (resultadoSeguro <= 0)
                {

                    return BadRequest("No se registro obligación de seguro estudiantil anual");
                }
            }


            var resultadoObligacionSeguro = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == matricula && x.RubroId == 3);

            var obligacionIdSeguro = resultadoObligacionSeguro.FirstOrDefault().Id;

            if (obligacionIdPension <= 0)
            {

                return BadRequest("No se obtuvo código de obligación seguro");
            }


            // actulizo el estado proceso de inscripcion
            var inscripcionEstudiante = InscripcionEstudiante.FirstOrDefault();
            if (inscripcionEstudiante != null)
            {
                inscripcionEstudiante.procesado = true;
                _unitOfWork.Repository<EstudianteInscripcion>().UpdateEntity(inscripcionEstudiante);

                var resultadoUpdateInscripcion = await _unitOfWork.Complete();

                if (resultadoUpdateInscripcion <= 0)
                {

                    return BadRequest("No se actualizo el estado de la inscripción");
                }
                //mando el id obligacion de matricula
                //return Ok(obligacionId);
            }


            var data = new { ObligacionId = obligacionId, FilaTablaMatricula = FilaOblicacion };
            return Json(data);


        }


        public async Task<IActionResult> GrabaEstudiante(EstudianteViewModel estudianteViewModel)
        {

            if (estudianteViewModel == null)
                return BadRequest("No llego datos");

            string extArchivo = Path.GetExtension(estudianteViewModel.Avatar.FileName);
            string extArchivoPdf = Path.GetExtension(estudianteViewModel.PDFIdentificacion.FileName);
            if (!(extArchivo.ToLower().Equals(".png") || extArchivo.ToLower().Equals(".jpg")
                || extArchivo.ToLower().Equals(".jpeg")))
            {
                return BadRequest("Foto Estudiante, seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)");
            }

            if (!extArchivoPdf.ToLower().Equals(".pdf"))
            {
                return BadRequest("Pdf Cédula Estudiante, seleccione un archivo con las siguiente extensión  (pdf)");
            }

            if (estudianteViewModel.Identificacion.IsNullOrEmpty())
                return BadRequest("No llego tipo Id o Id");

            //consulta existencia matrícula x persona x ciclo escolar
            //un estudiante solo puede tener una matrícula por ciclo escolar
            var includesPersona = new List<Expression<Func<Matricula, object>>>
            {
               p => p.Persona!,

            };
            var ConsultaExiteMatriculaPersona1 = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.Persona.Identificacion == estudianteViewModel.Identificacion && x.CicloEscolarId == estudianteViewModel.CicloEscolarId, null, includesPersona);

            if (ConsultaExiteMatriculaPersona1 != null && ConsultaExiteMatriculaPersona1.Count > 0)
            { //ya Existe la persona 
                //return Ok("Este estudiante ya esta registrado para este año escolar");
                return Ok(ConsultaExiteMatriculaPersona1.FirstOrDefault().Id);
            }

            try
            {
                //1. ingreso personal

                var persona = new Persona();
                persona.TipoIdentificacionId = estudianteViewModel.TipoIdentificacionId;
                persona.Nombres = estudianteViewModel.Nombres;
                persona.Apellidos = estudianteViewModel.Apellidos;
                persona.Identificacion = estudianteViewModel.Identificacion;
                persona.FechaNacimiento = estudianteViewModel.FechaNacimiento;
                persona.TipoEstudianteId = 3; // Estudiante Estandard;
                persona.UsuarioId = estudianteViewModel.UsuarioRegistra;
                persona.SucursalId = estudianteViewModel.SucursalId;

                if (estudianteViewModel.Avatar != null && estudianteViewModel.Avatar.Length > 0)
                {
                    // Obtener el nombre y la extensión del archivo
                    string numCedula = estudianteViewModel.Identificacion;
                    string extensionArchivo = Path.GetExtension(estudianteViewModel.Avatar.FileName);

                    // Generar un nombre único para el archivo
                    string nombreArchivo = $"{numCedula}{extensionArchivo}";
                    persona.Avatar = nombreArchivo;
                    // Ruta de destino para guardar el archivo
                    string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                    using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                    {
                        await estudianteViewModel.Avatar.CopyToAsync(stream);
                    }

                }
                else
                {
                    persona.Avatar = "avatar-default.png";
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


                _unitOfWork.Repository<Persona>().AddEntity(persona);

                var resultPersona = await _unitOfWork.Complete();

                if (resultPersona <= 0)
                {

                    return BadRequest("No se guardo persona");
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

                    return BadRequest("No se guardo dirección");
                }

                var matricula = new Matricula();
                matricula.PersonaId = personaId;
                matricula.EstadoMatriculaId = 3; //registrada
                matricula.NivelEscolarId = estudianteViewModel.nivel;
                matricula.CicloEscolarId = estudianteViewModel.CicloEscolarId;

                _unitOfWork.Repository<Matricula>().AddEntity(matricula);

                var resultMatricula = await _unitOfWork.Complete();

                if (resultMatricula <= 0)
                {

                    return BadRequest("No se registró matricula");
                }
                /*
                var includesCiclo = new List<Expression<Func<Matricula, object>>>
                 {
                                    p => p.Paralelo!
                 };
                */
                var numeroMatricula = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId.Equals(personaId));//, null, includesCiclo);

                var fichaMedica = new FichaMedica();
                fichaMedica.MatriculaId = numeroMatricula.FirstOrDefault().Id;
                fichaMedica.Alergias = estudianteViewModel?.alergias == null ? "" : estudianteViewModel?.alergias;
                fichaMedica.NotasImportanteSalud = estudianteViewModel?.DatoAdicional == null ? "" : estudianteViewModel?.DatoAdicional;
                fichaMedica.RestriccionesAlimenticias = estudianteViewModel?.restriccionAlimento == null ? "" : estudianteViewModel?.restriccionAlimento;
                fichaMedica.SeguroMedico = estudianteViewModel?.SeguroPrivado == null ? "" : estudianteViewModel?.SeguroPrivado;

                if (estudianteViewModel.Analgesico == true)
                {

                    fichaMedica.AutorizaAntiinflamatorioAnalgesico = true;
                }
                else
                {
                    fichaMedica.AutorizaAntiinflamatorioAnalgesico = false;
                }

                if (estudianteViewModel?.NombreMedico != null && estudianteViewModel?.NombreMedico != "")
                {
                    fichaMedica.NombresMedico = estudianteViewModel?.NombreMedico == null ? "" : estudianteViewModel?.NombreMedico;
                    fichaMedica.TelefonoMedico = estudianteViewModel?.TelefonoMedico == null ? "" : estudianteViewModel?.TelefonoMedico;
                    fichaMedica.DireccionMedico = estudianteViewModel?.DireccionMedico == null ? "" : estudianteViewModel?.DireccionMedico;
                    fichaMedica.CelularMedico = estudianteViewModel?.CelularMedico == null ? "" : estudianteViewModel?.CelularMedico;
                }


                _unitOfWork.Repository<FichaMedica>().AddEntity(fichaMedica);

                var resultFicha = await _unitOfWork.Complete();

                if (resultFicha <= 0)
                {

                    return BadRequest("No se registró ficha medica");
                }

                var historialMed = await _unitOfWork.Repository<FichaMedica>().GetAsync(x => x.MatriculaId == numeroMatricula.FirstOrDefault().Id);

                var idhistorial = historialMed.FirstOrDefault().Id;

                var historialMedicoEstudiante = new HistorialMedico();
                historialMedicoEstudiante.FichaMedicaId = idhistorial;
                historialMedicoEstudiante.Peso = estudianteViewModel.Peso;
                historialMedicoEstudiante.Talla = estudianteViewModel.Talla;

                _unitOfWork.Repository<HistorialMedico>().AddEntity(historialMedicoEstudiante);

                var resulthistorial = await _unitOfWork.Complete();

                if (resultFicha <= 0)
                {

                    return BadRequest("No se registró peso y talla en historia medica");
                }

                //Tabla FichaObservacionesMedicas
                var observacion = new FichaObservacionMedica();
                if (estudianteViewModel.NumeroDosis != null)
                {
                    observacion.FichaMedicaId = idhistorial;
                    observacion.Respuesta = true;
                    observacion.Especificacion = estudianteViewModel.NumeroDosis.ToString();
                    observacion.ObservacionMedicaId = 1; //VacunaCovid

                    _unitOfWork.Repository<FichaObservacionMedica>().AddEntity(observacion);
                    var vacuna = await _unitOfWork.Complete();

                    if (vacuna <= 0)
                    {

                        return BadRequest("No se registró dosis vacuna");
                    }
                }
                else
                {
                    observacion.FichaMedicaId = idhistorial;
                    observacion.Respuesta = false;
                    observacion.Especificacion = "";
                    observacion.ObservacionMedicaId = 1; //VacunaCovid

                    _unitOfWork.Repository<FichaObservacionMedica>().AddEntity(observacion);
                    var vacuna = await _unitOfWork.Complete();

                    if (vacuna <= 0)
                    {

                        return BadRequest("No se registró dosis vacuna");
                    }

                }

                var observacion2 = new FichaObservacionMedica();

                if (estudianteViewModel.EnfermedadCronica != null)
                {
                    observacion2.FichaMedicaId = idhistorial;
                    observacion2.Respuesta = true;
                    observacion2.Especificacion = estudianteViewModel.EnfermedadCronica;
                    observacion2.ObservacionMedicaId = 2; //Enfermedad crónica

                    _unitOfWork.Repository<FichaObservacionMedica>().AddEntity(observacion2);
                    var enfermedad = await _unitOfWork.Complete();

                    if (enfermedad <= 0)
                    {

                        return BadRequest("No se registró enfermedad crónica");
                    }

                }
                else
                {
                    observacion2.FichaMedicaId = idhistorial;
                    observacion2.Respuesta = false;
                    observacion2.Especificacion = "";
                    observacion2.ObservacionMedicaId = 2; //Enfermedad crónica

                    _unitOfWork.Repository<FichaObservacionMedica>().AddEntity(observacion2);
                    var enfermedad = await _unitOfWork.Complete();

                    if (enfermedad <= 0)
                    {

                        return BadRequest("No se registró enfermedad crónica");
                    }

                }

                var observacion3 = new FichaObservacionMedica();

                if (estudianteViewModel.MedicacionContinua != null)
                {
                    observacion3.FichaMedicaId = idhistorial;
                    observacion3.Respuesta = true;
                    observacion3.Especificacion = estudianteViewModel.MedicacionContinua;
                    observacion3.ObservacionMedicaId = 3; //medicacion continua

                    _unitOfWork.Repository<FichaObservacionMedica>().AddEntity(observacion3);
                    var medicacion = await _unitOfWork.Complete();

                    if (medicacion <= 0)
                    {

                        return BadRequest("No se registró medicación continua");
                    }

                }
                else
                {
                    observacion3.FichaMedicaId = idhistorial;
                    observacion3.Respuesta = false;
                    observacion3.Especificacion = "";
                    observacion3.ObservacionMedicaId = 3; //medicacion continua

                    _unitOfWork.Repository<FichaObservacionMedica>().AddEntity(observacion3);
                    var medicacion = await _unitOfWork.Complete();

                    if (medicacion <= 0)
                    {

                        return BadRequest("No se registró medicación continua");
                    }
                }

                var observacion4 = new FichaObservacionMedica();

                if (estudianteViewModel.cirugia != null)
                {
                    observacion4.FichaMedicaId = idhistorial;
                    observacion4.Respuesta = true;
                    observacion4.Especificacion = estudianteViewModel.cirugia;
                    observacion4.ObservacionMedicaId = 4; //cirugia

                    _unitOfWork.Repository<FichaObservacionMedica>().AddEntity(observacion4);
                    var cirugia = await _unitOfWork.Complete();

                    if (cirugia <= 0)
                    {

                        return BadRequest("No se registró cirugía");
                    }

                }
                else
                {
                    observacion4.FichaMedicaId = idhistorial;
                    observacion4.Respuesta = false;
                    observacion4.Especificacion = "";
                    observacion4.ObservacionMedicaId = 4; //cirugia

                    _unitOfWork.Repository<FichaObservacionMedica>().AddEntity(observacion4);
                    var cirugia = await _unitOfWork.Complete();

                    if (cirugia <= 0)
                    {

                        return BadRequest("No se registró cirugía");
                    }

                }


                return Ok(numeroMatricula.FirstOrDefault().Id);
            }
            catch (Exception ex)
            {

                return BadRequest("No guardo datos estudiantes");
            }
        }


        public async Task<IActionResult> GuardarTransporte([FromForm] TransporteViewModel transporteViewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("Index", "Home");

            //}

            //Validacion

            int codigoEstudiante = transporteViewModel.idEstudiante;

            if (codigoEstudiante == 0)
            {
                return Ok(new { data = "Cargue los datos del Estudiante." });
            }

            if (transporteViewModel.NombresAutorizado1 != null && transporteViewModel.ApellidosAutorizado1 != null)
            {
                string extensionArchivo = Path.GetExtension(transporteViewModel.AvatarAutorizado1.FileName);

                if (!(extensionArchivo.ToLower().Equals(".png") || extensionArchivo.ToLower().Equals(".jpg")
                        || extensionArchivo.ToLower().Equals(".jpeg")))
                {
                    return Ok(new { data = "Foto del Autorizado, seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)" });
                }
            }

            if (transporteViewModel.NombresAutorizado2 != null && transporteViewModel.ApellidosAutorizado2 != null)
            {
                string extensionArchivo = Path.GetExtension(transporteViewModel.AvatarAutorizado2.FileName);

                if (!(extensionArchivo.ToLower().Equals(".png") || extensionArchivo.ToLower().Equals(".jpg")
                        || extensionArchivo.ToLower().Equals(".jpeg")))
                {
                    return Ok(new { data = "Foto del Autorizado, seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)" });
                }
            }

            if (transporteViewModel.NombresAutorizado3 != null && transporteViewModel.ApellidosAutorizado3 != null)
            {
                string extensionArchivo = Path.GetExtension(transporteViewModel.AvatarAutorizado3.FileName);

                if (!(extensionArchivo.ToLower().Equals(".png") || extensionArchivo.ToLower().Equals(".jpg")
                        || extensionArchivo.ToLower().Equals(".jpeg")))
                {
                    return Ok(new { data = "Foto del Autorizado, seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)" });
                }
            }

            if (transporteViewModel.NombresAutorizado4 != null && transporteViewModel.ApellidosAutorizado4 != null)
            {
                string extensionArchivo = Path.GetExtension(transporteViewModel.AvatarAutorizado4.FileName);

                if (!(extensionArchivo.ToLower().Equals(".png") || extensionArchivo.ToLower().Equals(".jpg")
                        || extensionArchivo.ToLower().Equals(".jpeg")))
                {
                    return Ok(new { data = "Foto del Autorizado, seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)" });
                }
            }

            var direccionCasa = await _unitOfWork.Repository<Direccion>().GetAsync(x => x.PersonaId == codigoEstudiante, null);

            var lista = await _unitOfWork.Repository<AlumnoTransporte>().GetAsync(x => x.DireccionId == direccionCasa.FirstOrDefault().Id, null);
            if (lista.Count > 0)
            {
                return Ok(new { data = "Error ya existen datos" });
            }


            int idDireccion = direccionCasa.FirstOrDefault().Id;

            //Edito
            var EditarDireccion = await _unitOfWork.Repository<Direccion>().GetByIdAsync(idDireccion);
            EditarDireccion.Referencia = transporteViewModel.Referencia1 ?? "";
            EditarDireccion.linkGoogleMaps = transporteViewModel.linkGoogleMaps1 ?? "";

            await _unitOfWork.Repository<Direccion>().UpdateAsync(EditarDireccion);

            // creo ruta para el primer domicilio con su cedula de identidad:


            var rutaAlumno = new AlumnoTransporte();
            rutaAlumno.DireccionId = idDireccion; // Dirección de la casa
            rutaAlumno.DiaId = 1; // lunes

            _unitOfWork.Repository<AlumnoTransporte>().AddEntity(rutaAlumno);


            var rutaAlumnoMartes = new AlumnoTransporte();
            rutaAlumnoMartes.DireccionId = idDireccion; // Dirección de la casa
            rutaAlumnoMartes.DiaId = 2; //martes

            _unitOfWork.Repository<AlumnoTransporte>().AddEntity(rutaAlumnoMartes);

            var rutaAlumnoMiercoles = new AlumnoTransporte();
            rutaAlumnoMiercoles.DireccionId = idDireccion; // Dirección de la casa
            rutaAlumnoMiercoles.DiaId = 3; //miercoles

            _unitOfWork.Repository<AlumnoTransporte>().AddEntity(rutaAlumnoMiercoles);

            var rutaAlumnoJueves = new AlumnoTransporte();
            rutaAlumnoJueves.DireccionId = idDireccion; // Dirección de la casa
            rutaAlumnoJueves.DiaId = 4; //jueves
            _unitOfWork.Repository<AlumnoTransporte>().AddEntity(rutaAlumnoJueves);

            var rutaAlumnoViernes = new AlumnoTransporte();
            rutaAlumnoViernes.DireccionId = idDireccion; // Dirección de la casa
            rutaAlumnoViernes.DiaId = 5; //viernes

            _unitOfWork.Repository<AlumnoTransporte>().AddEntity(rutaAlumnoViernes);
            var resultViernes = await _unitOfWork.Complete();
            if (resultViernes <= 0)
            {
                return Ok(new { data = "Error a guardar Dirección" });
            }



            if (transporteViewModel.CallePrincipal2 != null && transporteViewModel.Numero2 != null && transporteViewModel.CalleSecundaria2 != null)
            {
                var direccionCasa2 = await _unitOfWork.Repository<Direccion>().GetAsync(x => x.PersonaId == codigoEstudiante && x.CallePrincipal == transporteViewModel.CallePrincipal2, null);

                if (direccionCasa2.Count == 0)
                {
                    var domicilioNuevo = new Direccion();
                    domicilioNuevo.CallePrincipal = transporteViewModel.CallePrincipal2;
                    domicilioNuevo.CalleSecundaria = transporteViewModel.CalleSecundaria2;
                    domicilioNuevo.Numero = transporteViewModel.Numero2;
                    domicilioNuevo.Referencia = transporteViewModel.Referencia2;
                    domicilioNuevo.linkGoogleMaps = transporteViewModel.linkGoogleMaps2;
                    domicilioNuevo.TipoDireccionId = 1;
                    domicilioNuevo.PersonaId = codigoEstudiante;

                    _unitOfWork.Repository<Direccion>().AddEntity(domicilioNuevo);
                    var resultDireccion = await _unitOfWork.Complete();
                    if (resultDireccion <= 0)
                    {
                        return Ok(new { data = "Error a guardar Dirección" });
                    }
                }
            }

            if (transporteViewModel.NombresRastreo1 != null && transporteViewModel.ApellidosRastreo1 != null)
            {
                var rastreo1 = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.Identificacion1)
                && x.Nombres == transporteViewModel.NombresRastreo1
                && x.Apellidos == transporteViewModel.ApellidosRastreo1
                , null);

                if (rastreo1.Count == 0)
                {
                    var personaRastreo1 = new Persona();
                    personaRastreo1.Nombres = transporteViewModel.NombresRastreo1;
                    personaRastreo1.Apellidos = transporteViewModel.ApellidosRastreo1;
                    personaRastreo1.EmailPrincipal = transporteViewModel.EmailPrincipal1;
                    personaRastreo1.TelefonoMovil = transporteViewModel.TelefonoMovil1;
                    personaRastreo1.Identificacion = transporteViewModel.Identificacion1;
                    personaRastreo1.TipoIdentificacionId = 1;
                    personaRastreo1.Avatar = "avatar-default.png";

                    _unitOfWork.Repository<Persona>().AddEntity(personaRastreo1);
                    var resultPersona1 = await _unitOfWork.Complete();
                    if (resultPersona1 <= 0)
                    {
                        return Ok(new { data = "Error a guardar Dirección" });
                    }


                    var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.Identificacion1), null);
                    var personaId = resultadoPersona.FirstOrDefault()!.Id;

                    var relacion = new Relacion();
                    relacion.PersonaId = codigoEstudiante;
                    relacion.Persona2Id = personaId;
                    relacion.TipoRelacionId = transporteViewModel.TipoRelacionId1;

                    var exite = await _unitOfWork.Repository<Relacion>().GetEntityAsync(y => y.PersonaId == codigoEstudiante
                    && y.Persona2Id == personaId);
                    if (exite == null)
                    {
                        _unitOfWork.Repository<Relacion>().AddEntity(relacion);
                        var resultRelacion = await _unitOfWork.Complete();

                        if (resultRelacion <= 0)
                        {
                            return Ok(new { data = "Error a guardar Dirección" });
                        }
                    }

                }
            }


            if (transporteViewModel.NombresRastreo2 != null && transporteViewModel.ApellidosRastreo2 != null)
            {

                var rastreo1 = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.Identificacion2)
                && x.Nombres == transporteViewModel.NombresRastreo2
                && x.Apellidos == transporteViewModel.ApellidosRastreo2
                , null);

                if (rastreo1.Count == 0)
                {
                    var personaRastreo2 = new Persona();
                    personaRastreo2.Nombres = transporteViewModel.NombresRastreo2;
                    personaRastreo2.Apellidos = transporteViewModel.ApellidosRastreo2;
                    personaRastreo2.EmailPrincipal = transporteViewModel.EmailPrincipal2;
                    personaRastreo2.TelefonoMovil = transporteViewModel.TelefonoMovil2;
                    personaRastreo2.Identificacion = transporteViewModel.Identificacion2;
                    personaRastreo2.TipoIdentificacionId = 1;
                    personaRastreo2.Avatar = "avatar-default.png";


                    _unitOfWork.Repository<Persona>().AddEntity(personaRastreo2);
                    var resultPersona2 = await _unitOfWork.Complete();
                    if (resultPersona2 <= 0)
                    {
                        return Ok(new { data = "Error a guardar Dirección" });
                    }


                    var resultadoPersona2 = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.Identificacion2), null);
                    var personaId2 = resultadoPersona2.FirstOrDefault()!.Id;

                    var relacion2 = new Relacion();
                    relacion2.PersonaId = codigoEstudiante;
                    relacion2.Persona2Id = personaId2;
                    relacion2.TipoRelacionId = transporteViewModel.TipoRelacionId2;

                    var exite = await _unitOfWork.Repository<Relacion>().GetEntityAsync(y => y.PersonaId == codigoEstudiante
                    && y.Persona2Id == personaId2);
                    if (exite == null)
                    {
                        _unitOfWork.Repository<Relacion>().AddEntity(relacion2);
                        var resultRelacion2 = await _unitOfWork.Complete();

                        if (resultRelacion2 <= 0)
                        {
                            return Ok(new { data = "Error a guardar Dirección" });
                        }
                    }

                }
            }
            if (transporteViewModel.NombresRastreo3 != null && transporteViewModel.ApellidosRastreo3 != null)
            {
                var rastreo1 = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.Identificacion3)
                && x.Nombres == transporteViewModel.NombresRastreo3
                && x.Apellidos == transporteViewModel.ApellidosRastreo3
                , null);
                if (rastreo1.Count == 0)
                {
                    var personaRastreo3 = new Persona();
                    personaRastreo3.Nombres = transporteViewModel.NombresRastreo3;
                    personaRastreo3.Apellidos = transporteViewModel.ApellidosRastreo3;
                    personaRastreo3.EmailPrincipal = transporteViewModel.EmailPrincipal3;
                    personaRastreo3.TelefonoMovil = transporteViewModel.TelefonoMovil3;
                    personaRastreo3.TipoIdentificacionId = 1;
                    personaRastreo3.Identificacion = transporteViewModel.Identificacion3;
                    personaRastreo3.Avatar = "avatar-default.png";



                    _unitOfWork.Repository<Persona>().AddEntity(personaRastreo3);
                    var resultPersona3 = await _unitOfWork.Complete();
                    if (resultPersona3 <= 0)
                    {
                        return Ok(new { data = "Error a guardar Dirección" });
                    }


                    var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.Identificacion3), null);
                    var personaId = resultadoPersona.FirstOrDefault()!.Id;

                    var relacion = new Relacion();
                    relacion.PersonaId = codigoEstudiante;
                    relacion.Persona2Id = personaId;
                    relacion.TipoRelacionId = transporteViewModel.TipoRelacionId3;

                    var exite = await _unitOfWork.Repository<Relacion>().GetEntityAsync(y => y.PersonaId == codigoEstudiante
                 && y.Persona2Id == personaId);

                    if (exite == null)
                    {
                        _unitOfWork.Repository<Relacion>().AddEntity(relacion);
                        var resultRelacion = await _unitOfWork.Complete();

                        if (resultRelacion <= 0)
                        {
                            return Ok(new { data = "Error a guardar Dirección" });
                        }
                    }

                }
            }

            if (transporteViewModel.NombresAutorizado1 != null && transporteViewModel.ApellidosAutorizado1 != null)
            {
                var autorizado = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.IdentificacionAutorizado1)
                && x.Nombres == transporteViewModel.NombresAutorizado1
                && x.Apellidos == transporteViewModel.ApellidosAutorizado1
                , null);

                if (autorizado.Count == 0)
                {
                    var personaAutorizada1 = new Persona();
                    personaAutorizada1.Nombres = transporteViewModel.NombresAutorizado1;
                    personaAutorizada1.Apellidos = transporteViewModel.ApellidosAutorizado1;
                    personaAutorizada1.EmailPrincipal = transporteViewModel.EmailAutorizado1;
                    personaAutorizada1.TelefonoMovil = transporteViewModel.TelefonoMovilAutorizado1;
                    personaAutorizada1.TipoIdentificacionId = 1;
                    personaAutorizada1.Identificacion = transporteViewModel.IdentificacionAutorizado1;
                    personaAutorizada1.AutorizadoRecibir = true;

                    if (transporteViewModel.AvatarAutorizado1 != null && transporteViewModel.AvatarAutorizado1.Length > 0)
                    {
                        // Obtener el nombre y la extensión del archivo
                        string numCedula = transporteViewModel.IdentificacionAutorizado1;
                        string extensionArchivo = Path.GetExtension(transporteViewModel.AvatarAutorizado1.FileName);

                        if (!(extensionArchivo.ToLower().Equals(".png") || extensionArchivo.ToLower().Equals(".jpg")
                                || extensionArchivo.ToLower().Equals(".jpeg")))
                        {
                            return Ok(new { data = "Seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)" });
                        }

                        // Generar un nombre único para el archivo
                        string nombreArchivo = $"{numCedula}{extensionArchivo}";
                        personaAutorizada1.Avatar = nombreArchivo;
                        // Ruta de destino para guardar el archivo
                        string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                        using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                        {
                            await transporteViewModel.AvatarAutorizado1.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        personaAutorizada1.Avatar = "avatar-default.png";
                    }

                    _unitOfWork.Repository<Persona>().AddEntity(personaAutorizada1);
                    var resultPersona1 = await _unitOfWork.Complete();
                    if (resultPersona1 <= 0)
                    {
                        return Ok(new { data = "Error a guardar Dirección" });
                    }

                    var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.IdentificacionAutorizado1), null);
                    var personaId = resultadoPersona.FirstOrDefault()!.Id;

                    var relacion = new Relacion();
                    relacion.PersonaId = codigoEstudiante;
                    relacion.Persona2Id = personaId;
                    relacion.TipoRelacionId = transporteViewModel.TipoRelacionAutorizado1;

                    var exite = await _unitOfWork.Repository<Relacion>().GetEntityAsync(y => y.PersonaId == codigoEstudiante
               && y.Persona2Id == personaId);
                    if (exite == null)
                    {
                        _unitOfWork.Repository<Relacion>().AddEntity(relacion);
                        var resultRelacion = await _unitOfWork.Complete();

                        if (resultRelacion <= 0)
                        {
                            return Ok(new { data = "Error a guardar Dirección" });
                        }
                    }
                }
            }

            if (transporteViewModel.NombresAutorizado2 != null && transporteViewModel.ApellidosAutorizado2 != null)
            {
                var autorizado = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.IdentificacionAutorizado2)
                && x.Nombres == transporteViewModel.NombresAutorizado2
                && x.Apellidos == transporteViewModel.ApellidosAutorizado2
                , null);

                if (autorizado.Count == 0)
                {
                    var personaAutorizada2 = new Persona();
                    personaAutorizada2.Nombres = transporteViewModel.NombresAutorizado2;
                    personaAutorizada2.Apellidos = transporteViewModel.ApellidosAutorizado2;
                    personaAutorizada2.EmailPrincipal = transporteViewModel.EmailAutorizado2;
                    personaAutorizada2.TelefonoMovil = transporteViewModel.TelefonoMovilAutorizado2;
                    personaAutorizada2.TipoIdentificacionId = 1;
                    personaAutorizada2.Identificacion = transporteViewModel.IdentificacionAutorizado2;
                    personaAutorizada2.AutorizadoRecibir = true;

                    if (transporteViewModel.AvatarAutorizado2 != null && transporteViewModel.AvatarAutorizado2.Length > 0)
                    {
                        // Obtener el nombre y la extensión del archivo
                        string numCedula = transporteViewModel.IdentificacionAutorizado2;
                        string extensionArchivo = Path.GetExtension(transporteViewModel.AvatarAutorizado2.FileName);

                        if (!(extensionArchivo.ToLower().Equals(".png") || extensionArchivo.ToLower().Equals(".jpg")
                                || extensionArchivo.ToLower().Equals(".jpeg")))
                        {
                            return Ok(new { data = "Seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)" });
                        }

                        // Generar un nombre único para el archivo
                        string nombreArchivo = $"{numCedula}{extensionArchivo}";
                        personaAutorizada2.Avatar = nombreArchivo;
                        // Ruta de destino para guardar el archivo
                        string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                        using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                        {
                            await transporteViewModel.AvatarAutorizado2.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        personaAutorizada2.Avatar = "avatar-default.png";
                    }

                    _unitOfWork.Repository<Persona>().AddEntity(personaAutorizada2);
                    var resultAutorizado2 = await _unitOfWork.Complete();
                    if (resultAutorizado2 <= 0)
                    {
                        return Ok(new { data = "Error a guardar Dirección" });
                    }

                    var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.IdentificacionAutorizado2), null);
                    var personaId = resultadoPersona.FirstOrDefault()!.Id;

                    var relacion = new Relacion();
                    relacion.PersonaId = codigoEstudiante;
                    relacion.Persona2Id = personaId;
                    relacion.TipoRelacionId = transporteViewModel.TipoRelacionAutorizado2;

                    //var saveData = await _unitOfWork.Repository<Relacion>().GetEntityAsync(
                    //f =>f.PersonaId== relacion.PersonaId 
                    //&& f.Persona2Id== relacion.Persona2Id);
                    //if (saveData==null)
                    //{

                    var exite = await _unitOfWork.Repository<Relacion>().GetEntityAsync(y => y.PersonaId == codigoEstudiante
              && y.Persona2Id == personaId);

                    if (exite == null)
                    {
                        _unitOfWork.Repository<Relacion>().AddEntity(relacion);
                        var resultRelacion = await _unitOfWork.Complete();

                        if (resultRelacion <= 0)
                        {
                            return Ok(new { data = "Error a guardar Dirección" });
                        }

                    }
                }
            }



            if (transporteViewModel.NombresAutorizado3 != null && transporteViewModel.ApellidosAutorizado3 != null)
            {
                var autorizado = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.IdentificacionAutorizado3)
                && x.Nombres == transporteViewModel.NombresAutorizado3
                && x.Apellidos == transporteViewModel.ApellidosAutorizado3
                , null);

                if (autorizado.Count == 0)
                {
                    var personaAutorizada3 = new Persona();
                    personaAutorizada3.Nombres = transporteViewModel.NombresAutorizado3;
                    personaAutorizada3.Apellidos = transporteViewModel.ApellidosAutorizado3;
                    personaAutorizada3.EmailPrincipal = transporteViewModel.EmailAutorizado3;
                    personaAutorizada3.TelefonoMovil = transporteViewModel.TelefonoMovilAutorizado3;
                    personaAutorizada3.TipoIdentificacionId = 1;
                    personaAutorizada3.Identificacion = transporteViewModel.IdentificacionAutorizado3;
                    personaAutorizada3.AutorizadoRecibir = true;

                    if (transporteViewModel.AvatarAutorizado3 != null && transporteViewModel.AvatarAutorizado3.Length > 0)
                    {
                        // Obtener el nombre y la extensión del archivo
                        string numCedula = transporteViewModel.IdentificacionAutorizado3;
                        string extensionArchivo = Path.GetExtension(transporteViewModel.AvatarAutorizado3.FileName);

                        if (!(extensionArchivo.ToLower().Equals(".png") || extensionArchivo.ToLower().Equals(".jpg")
                                || extensionArchivo.ToLower().Equals(".jpeg")))
                        {
                            return Ok(new { data = "Seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)" });
                        }

                        // Generar un nombre único para el archivo
                        string nombreArchivo = $"{numCedula}{extensionArchivo}";
                        personaAutorizada3.Avatar = nombreArchivo;
                        // Ruta de destino para guardar el archivo
                        string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                        using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                        {
                            await transporteViewModel.AvatarAutorizado3.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        personaAutorizada3.Avatar = "avatar-default.png";
                    }

                    _unitOfWork.Repository<Persona>().AddEntity(personaAutorizada3);
                    var resultAutorizado3 = await _unitOfWork.Complete();
                    if (resultAutorizado3 <= 0)
                    {
                        return Ok(new { data = "Error a guardar Dirección" });
                    }

                    var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.IdentificacionAutorizado3), null);
                    var personaId = resultadoPersona.FirstOrDefault()!.Id;

                    var relacion = new Relacion();
                    relacion.PersonaId = codigoEstudiante;
                    relacion.Persona2Id = personaId;
                    relacion.TipoRelacionId = transporteViewModel.TipoRelacionAutorizado3;


                    var exite = await _unitOfWork.Repository<Relacion>().GetEntityAsync(y => y.PersonaId == codigoEstudiante
             && y.Persona2Id == personaId);
                    if (exite == null)
                    {
                        _unitOfWork.Repository<Relacion>().AddEntity(relacion);
                        var resultRelacion = await _unitOfWork.Complete();

                        if (resultRelacion <= 0)
                        {
                            return Ok(new { data = "Error a guardar Dirección" });
                        }
                    }

                }
            }



            if (transporteViewModel.NombresAutorizado4 != null && transporteViewModel.ApellidosAutorizado4 != null)
            {
                var autorizado = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.IdentificacionAutorizado4)
                && x.Nombres == transporteViewModel.NombresAutorizado4
                && x.Apellidos == transporteViewModel.ApellidosAutorizado4
                , null);
                if (autorizado == null)
                {
                    var personaAutorizada4 = new Persona();
                    personaAutorizada4.Nombres = transporteViewModel.NombresAutorizado4;
                    personaAutorizada4.Apellidos = transporteViewModel.ApellidosAutorizado4;
                    personaAutorizada4.EmailPrincipal = transporteViewModel.EmailAutorizado4;
                    personaAutorizada4.TelefonoMovil = transporteViewModel.TelefonoMovilAutorizado4;
                    personaAutorizada4.TipoIdentificacionId = 1;
                    personaAutorizada4.Identificacion = transporteViewModel.IdentificacionAutorizado4;
                    personaAutorizada4.AutorizadoRecibir = true;


                    if (transporteViewModel.AvatarAutorizado4 != null && transporteViewModel.AvatarAutorizado4.Length > 0)
                    {
                        // Obtener el nombre y la extensión del archivo
                        string numCedula = transporteViewModel.IdentificacionAutorizado3;
                        string extensionArchivo = Path.GetExtension(transporteViewModel.AvatarAutorizado4.FileName);

                        if (!(extensionArchivo.ToLower().Equals(".png") || extensionArchivo.ToLower().Equals(".jpg")
                                || extensionArchivo.ToLower().Equals(".jpeg")))
                        {
                            return Ok(new { data = "Seleccione un archivo con las siguientes extensiones (png,jpg,jpeg)" });
                        }

                        // Generar un nombre único para el archivo
                        string nombreArchivo = $"{numCedula}{extensionArchivo}";
                        personaAutorizada4.Avatar = nombreArchivo;
                        // Ruta de destino para guardar el archivo
                        string rutaGuardar = Path.Combine(_hostEnvironment.WebRootPath, "assets", "media", "avatars", nombreArchivo);

                        using (var stream = new FileStream(rutaGuardar, FileMode.Create))
                        {
                            await transporteViewModel.AvatarAutorizado4.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        personaAutorizada4.Avatar = "avatar-default.png";
                    }

                    _unitOfWork.Repository<Persona>().AddEntity(personaAutorizada4);
                    var resultAutorizado4 = await _unitOfWork.Complete();
                    if (resultAutorizado4 <= 0)
                    {
                        return Ok(new { data = "Error a guardar Dirección" });
                    }

                    var resultadoPersona = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Identificacion!.Equals(transporteViewModel.IdentificacionAutorizado4), null);
                    var personaId = resultadoPersona.FirstOrDefault()!.Id;

                    var relacion = new Relacion();
                    relacion.PersonaId = codigoEstudiante;
                    relacion.Persona2Id = personaId;
                    relacion.TipoRelacionId = transporteViewModel.TipoRelacionAutorizado4;

                    var exite = await _unitOfWork.Repository<Relacion>().GetEntityAsync(y => y.PersonaId == codigoEstudiante
                           && y.Persona2Id == personaId);
                    if (exite == null)
                    {
                        _unitOfWork.Repository<Relacion>().AddEntity(relacion);
                        var resultRelacion = await _unitOfWork.Complete();

                        if (resultRelacion <= 0)
                        {
                            return Ok(new { data = "Error a guardar Dirección" });
                        }
                    }

                }
            }

            return Ok(new { data = "OK" });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult Mantenimiento()
        {
            return View();
        }

    }
}