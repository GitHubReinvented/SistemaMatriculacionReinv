using Application.Persistence;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.CodeAnalysis.Scripting;
using Org.BouncyCastle.Asn1.Crmf;
using ReinventedPuembo.Interfaces;
using ReinventedPuembo.Models;
using RestSharp;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ReinventedPuembo.Controllers
{
    public class responsePago : Controller
    {

        
            private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
      
        private readonly IRegistroPagos _registroPagos;
        private readonly IWebHostEnvironment _hostEnvironment;

        private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _configuration;

        private readonly IEmailServicePuembo _emailServicePuembo;
        private readonly IEmailServiceSantaClara _emailServiceSantaClara;

        public responsePago(
            ILogger<HomeController> logger,
            IUnitOfWork unitOfWork,
            IDocumento documento,
            IWebHostEnvironment hostEnvironment,
    
            UserManager<Usuario> userManager,
            IEmailServicePuembo emailServicePuembo,
            IEmailServiceSantaClara emailServiceSantaClara,
            IRegistroPagos registroPagos,
            IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
         
            _hostEnvironment = hostEnvironment;
       
            _userManager = userManager;
            _registroPagos = registroPagos;
            _configuration = configuration;

        }

        // GET: responsePago
        public async Task<IActionResult> Index(int id,string clientTransactionId)
        {
            try
            {
                
                @ViewBag.Mensaje = "listo";
                @ViewBag.id = id;
                @ViewBag.clientTransactionId = clientTransactionId;
                var Ambiente = _configuration.GetSection("ambiente")["ambiente"];
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                ViewBag.UsuarioID = userId;
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
                var BotonPago2Sucursal = await _unitOfWork.Repository<BotonPagoSucursal>().GetAsync(x => x.SucursalId == suc.Id && x.BotonPagoId == 2);
         
                if (BotonPago2Sucursal.Count() > 0)
                {

                    if (Ambiente == "pruebas")
                    {
                        @ViewBag.token = BotonPago2Sucursal.FirstOrDefault().TokenPruebas;
                    }
                    else { @ViewBag.token = BotonPago2Sucursal.FirstOrDefault().TokenProduccion; }

                    decimal totalPago = 0;
                    decimal sumatoriaPensiones = 0;
                    decimal costoSeguroEstudiantil = 0;
                    List<int> globalArrayobligaciones = new List<int> { };
                    List<ObligacionPendiente> obligacionesRepresentante = new List<ObligacionPendiente>();

                    var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.UsuarioId!.Equals(userId) && x.Representante == true, null);
                    var codigoRep = representante.FirstOrDefault().Id;
                   var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == codigoRep, null);

                    var codigoMadre = 0;
                    var codigoPadre = 0;
                    foreach (var item in relacionesRepresentante)
                    {
                        var codigoEstudiante = item.PersonaId;
                        //busco las obligaciones pendientes del representante de pensiones y seguro estudiantil
                        //busco las matriculas por cada estudiante que represente
                        var includesMatricula = new List<Expression<Func<Matricula, object>>>
                        {
                            p => p.NivelEscolar!,

                        };

                        var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codigoEstudiante, null, includesMatricula);

                        var codigoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().Id;
                        var includesPersonaObligacion = new List<Expression<Func<ObligacionPendiente, object>>>
                        {
                            p => p.Matricula!,
                            p=>p.Matricula.Persona!

                        };
                        var obligacionesSeguro = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante && x.RubroId == 3 
                        && (x.EstadoCuotaId == 2 || x.EstadoCuotaId == 4), null, includesPersonaObligacion);
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


                      

                    }
                    foreach (var obligacion in obligacionesRepresentante)
                    {
                        if (obligacion.RubroId == 2)
                        {
                            //< !--Pensión es cod 2-- >
                            totalPago = totalPago + obligacion.Saldo;
                            sumatoriaPensiones = sumatoriaPensiones + obligacion.Saldo;
                            globalArrayobligaciones.Add(obligacion.Id);



                        }

                        if (obligacion.RubroId == 3 && obligacion.EstadoCuotaId == 2)
                        {
                            //< !--Seguro estudiantil es cod 3-- >
                            totalPago = totalPago + obligacion.Saldo;
                            costoSeguroEstudiantil = obligacion.Saldo;
                            globalArrayobligaciones.Add(obligacion.Id);
                        }
                    }
                    @ViewBag.globalArrayobligaciones=globalArrayobligaciones;

                }
                else
                {
                    //ViewBag.Error = "No se contraron parametros de botones de pago PayPhone";
                    //return View("Error");
                }

                
            }
            catch (Exception)
            {
                @ViewBag.Mensaje = "Error al hacer el pago";
                return View();
            }


            return View();


        }



        public ActionResult GuardarPagoXTC(PagoXTarjeta model)
        {
            try
            {
                model.FechaTransaccion = DateTime.Now;
                    var respuesta = _registroPagos.GuardarPagoPyXTC(model);
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

        // GET: responsePago/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: responsePago/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: responsePago/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: responsePago/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: responsePago/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: responsePago/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: responsePago/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
