using Application.Persistence;
using Domain;
using Microsoft.AspNetCore.Mvc;
using ReinventedPuembo.Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Application.Models.Authorization;
using System.Security.Claims;
using Humanizer;
using System.Collections.Generic;
using System.Drawing;
using GrapeCity.Documents.Pdf.Annotations;
using Org.BouncyCastle.Asn1.X509;
using Microsoft.IdentityModel.Tokens;

namespace ReinventedPuembo.Controllers
{
    public class AdministracionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _configuration;

        public AdministracionController(IUnitOfWork unitOfWork, UserManager<Usuario> userManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> RegistrarSucursal()
        {

            //List<UsuarioViewModel> lista = new List<UsuarioViewModel>() { };

            //try
            //{
            //    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            //    var includesc = new List<Expression<Func<Usuario, object>>>
            //    {
            //        p => p.UsuarioSucursal!,
            //    };
            //    var user = await _unitOfWork.Repository<Usuario>()
            //              .GetEntityAsync(
            //              x => x.Id!.Equals(userId),
            //              includesc,
            //              false
            //              );
            //    var sucursalEntity = user.UsuarioSucursal!.FirstOrDefault(x => x.UsuarioId!.Equals(user.Id));
               
            //    var includesSucursal = new List<Expression<Func<UsuarioSucursal, object>>>
            //{
            //    p => p.Usuario!,
            //    p => p.Sucursal!

            //};

            //    var sucursalUsuario = await _unitOfWork.Repository<UsuarioSucursal>().GetAsync(x => x.UsuarioId != "" && x.Sucursal.Id == sucursalEntity!.SucursalId, null, includesSucursal);
             
            //    foreach (var item in sucursalUsuario)
            //    {
            //        var rolUser = (await _userManager.GetRolesAsync(item.Usuario!)).FirstOrDefault();

            //        var userAdd = new UsuarioViewModel
            //        {
            //            id = item.Usuario!.Id,
            //            Nombres = item.Usuario.Nombres,
            //            Apellidos = item.Usuario.Apellidos,
            //            Email = item.Usuario.Email,
            //            Rol = rolUser,
            //            Sucursal = item.Sucursal!.Nombre,
            //            isActive = item.Usuario.isActive

            //        };
            //        lista.Add(userAdd);
            //    }

               
            //}
            //catch (Exception ex)
            //{

               
            //}
            //return View(lista);
            return RedirectToAction("Registrar", "Administracion");
        }
        public async Task<IActionResult> Descuentos()
        {

            var inlcudesDescuentos = new List<Expression<Func<AplicaDescuento, object>>>
            {
                p => p.Descuento!,
                p => p.Rubro!.TipoRubro
            };

            var aplicaDescuentos = await _unitOfWork.Repository<AplicaDescuento>().GetAsync(x => x.Porcentaje != null, null, inlcudesDescuentos);

            return View(aplicaDescuentos);
        }

        public async Task<IActionResult> RegistrarDescuentos([FromForm] DescuentoViewModel descuentoViewModel)
        {
            var descuento = new Descuento();
            descuento.Descripcion = descuentoViewModel.DescripcionDescuento;

            _unitOfWork.Repository<Descuento>().AddEntity(descuento);
            var resultDescuento = await _unitOfWork.Complete();

            if (resultDescuento <= 0)
            {
                TempData["Error"] = "No se registró datos";
                return RedirectToAction("Index", "Home");
            }

            var descuentoAgregado = await _unitOfWork.Repository<Descuento>().GetEntityAsync(x => x.Descripcion == descuentoViewModel.DescripcionDescuento);
            var idDescuento = descuentoAgregado.Id;

            var descuentoAplicado = new AplicaDescuento();
            descuentoAplicado.DescuentoId = idDescuento;
            descuentoAplicado.RubroId = descuentoViewModel.RubroId;
            descuentoAplicado.Porcentaje = descuentoViewModel.Porcentaje;
            descuentoAplicado.Estado = 1;

            _unitOfWork.Repository<AplicaDescuento>().AddEntity(descuentoAplicado);
            var result = await _unitOfWork.Complete();

            if (result <= 0)
            {
                TempData["Error"] = "No se registró datos";
                return RedirectToAction("Index", "Home");
            }


            return RedirectToAction("Descuentos","Administracion");
        }
        public async Task<IActionResult> EditarDescuentos([FromForm] DescuentoViewModel descuentoViewModel)
        {
            var descuentoEditar = await _unitOfWork.Repository<AplicaDescuento>().GetByIdAsync(descuentoViewModel.DescuentoId);
            if (descuentoEditar != null)
            {
                var descuento = await _unitOfWork.Repository<Descuento>().GetByIdAsync(descuentoViewModel.DescuentoId);
                descuento.Descripcion = descuentoViewModel.DescripcionDescuento;
                await _unitOfWork.Repository<Descuento>().UpdateAsync(descuento);


                descuentoEditar.Porcentaje = descuentoViewModel.Porcentaje;

                await _unitOfWork.Repository<AplicaDescuento>().UpdateAsync(descuentoEditar);


                //var rubroEditar = await _unitOfWork.Repository<Rubro>().GetByIdAsync(rubroViewModel.idRubro);
                //rubroEditar.Costo = rubroViewModel.Costo;
                //await _unitOfWork.Repository<Rubro>().UpdateAsync(rubroEditar);


                return RedirectToAction("Descuentos", "Administracion");
            }
            else
            {
                return RedirectToAction("Descuentos", "Administracion");
            }
        }

        public async Task<IActionResult> EliminarDescuentos(int id)
        {
            var descuento = await _unitOfWork.Repository<Descuento>().GetByIdAsync(id);
            var aplicaDescuento = await _unitOfWork.Repository<AplicaDescuento>().GetByIdAsync(id);

            _unitOfWork.Repository<Descuento>().DeleteEntity(descuento); 
            _unitOfWork.Repository<AplicaDescuento>().DeleteEntity(aplicaDescuento);

            var result = await _unitOfWork.Complete();

            if (result <= 0)
            {
                return BadRequest();
            }

            return RedirectToAction("Descuentos", "Administracion");
        }

        public async Task<IActionResult> MatriculaPension()
        {
            var includes = new List<Expression<Func<Rubro, object>>>
            {
                p => p.CicloEscolar!.Sucursal!,
                p => p.TipoRubro
            };
            var rubros = await _unitOfWork.Repository<Rubro>().GetAsync(x => x.Id != 0, null, includes);
            //var rubros = await _unitOfWork.Repository<Rubro>().GetAllAsync();
            return View(rubros);
        }

        public async Task<IActionResult> IngresarRubro([FromForm] RubroViewModel rubroViewModel)
        {
            var cicloEscolar = new CicloEscolar();
            cicloEscolar.SucursalId = rubroViewModel.idSucursal;
            cicloEscolar.TipoPeriodoId = 1; // Mensal
            cicloEscolar.NumeroPeriodos = 10; //Lo que dura el ciclo
            cicloEscolar.FechaInicio = rubroViewModel.FechaInicio;
            cicloEscolar.FechaFin = rubroViewModel.FechaFin;

            _unitOfWork.Repository<CicloEscolar>().AddEntity(cicloEscolar);
            var resultciclo = await _unitOfWork.Complete();

            if (resultciclo <= 0)
            {
                TempData["Error"] = "No se registró datos";
                return RedirectToAction("MatriculaPension", "Administracion");
            }

            var cicloAgregado = await _unitOfWork.Repository<CicloEscolar>().GetEntityAsync(x => x.FechaInicio == rubroViewModel.FechaInicio && x.FechaFin == rubroViewModel.FechaFin);
            var idCiclo = cicloAgregado.Id;

            var rubro = new Rubro();
            rubro.CicloEscolarId = idCiclo;
            if(rubroViewModel.idRubro == 1)
            {
                rubro.TipoRubroId = 1;// "Matrícula";
            }
            else
            {
                rubro.TipoRubroId = 2;// "Pensión";
            }
            rubro.Costo = rubroViewModel.Costo;

            _unitOfWork.Repository<Rubro>().AddEntity(rubro);
            var resultrubro = await _unitOfWork.Complete();

            if (resultrubro <= 0)
            {
                TempData["Error"] = "No se registró datos";
                return RedirectToAction("MatriculaPension", "Administracion");
            }


            return RedirectToAction("MatriculaPension", "Administracion");
        }
        public async Task<IActionResult> EditarRubro([FromForm] RubroViewModel rubroViewModel)
        {
            var cicloEditar = await _unitOfWork.Repository<CicloEscolar>().GetByIdAsync(rubroViewModel.idCiclo);
            if (cicloEditar != null)
            {
                cicloEditar.FechaInicio = rubroViewModel.FechaInicio; 
                cicloEditar.FechaFin = rubroViewModel.FechaFin;
                await _unitOfWork.Repository<CicloEscolar>().UpdateAsync(cicloEditar);


                var rubroEditar = await _unitOfWork.Repository<Rubro>().GetByIdAsync(rubroViewModel.idRubro);
                rubroEditar.Costo = rubroViewModel.Costo;
                await _unitOfWork.Repository<Rubro>().UpdateAsync(rubroEditar);


                return RedirectToAction("MatriculaPension", "Administracion");
            }
            else
            {
                return RedirectToAction("MatriculaPension", "Administracion");
            }
        }
        public async Task<IActionResult> Registrar()
        {
            var lista = new List<UsuarioViewModel>();
            try
            {
                var includesSucursal = new List<Expression<Func<UsuarioSucursal, object>>>
            {
                p => p.Usuario!,
                p => p.Sucursal!

            };

                var sucursalUsuario = await _unitOfWork.Repository<UsuarioSucursal>().GetAsync(x => x.UsuarioId != "", null, includesSucursal);
               
                foreach (var item in sucursalUsuario)
                {
                    if (item.Usuario!=null) { 
                    var rolUser = (await _userManager.GetRolesAsync(item.Usuario!));
                    if (rolUser?.Count()!=0)
                    {
                        var userAdd = new UsuarioViewModel
                        {
                            id = item.Usuario!.Id,
                            Nombres = item.Usuario.Nombres,
                            Apellidos = item.Usuario.Apellidos,
                            Email = item.Usuario.Email,
                            Rol = rolUser.FirstOrDefault(),
                            Sucursal = item.Sucursal!.Nombre,
                            isActive = item.Usuario.isActive

                        };
                        lista.Add(userAdd);
                    }
                    else
                    {
                        Console.WriteLine(item.Usuario);
                    }
                    }
                    else
                    {
                        Console.WriteLine(item);
                    }

                }
            }
            catch (Exception e)
            {

                return View(lista);
            }

            return View(lista);

        }
       
        public async Task<IActionResult> InsertarUsuario([FromForm] UsuarioViewModel usuarioViewModel)
        {
            var usuarioNormal = new Usuario
            {
                Nombres = usuarioViewModel.Nombres,
                Apellidos = usuarioViewModel.Apellidos,
                Email = usuarioViewModel.Email,
                UserName = (usuarioViewModel.Email.Trim()).TrimStart(),
            };

            var result = await _userManager.CreateAsync(usuarioNormal, usuarioViewModel.Password);

            if (!result.Succeeded)
            {
                string errorMessage = "No se registró el usuario. ";

                foreach (var error in result.Errors)
                {
                    // Personaliza las traducciones según tus necesidades
                    switch (error.Code)
                    {
                        case "DuplicateUserName":
                            errorMessage += "El nombre de usuario "+ usuarioViewModel.Email + " ya está en uso. ";
                            break;

                        case "InvalidEmail":
                            errorMessage += "La dirección de correo electrónico no es válida. ";
                            break;

                        // Agrega más casos según los códigos de error que desees manejar
                        // ...

                        default:
                            errorMessage += error.Description + ". ";
                            break;
                    }
                }

                TempData["Error"] = errorMessage;
                return RedirectToAction("Registrar", "Administracion");
            }

            await _userManager.AddToRoleAsync(usuarioNormal, usuarioViewModel.Rol);

            var usuarioSucursal = new UsuarioSucursal
            {
                UsuarioId = usuarioNormal.Id,
                SucursalId = usuarioViewModel.SucursalId
            };

            _unitOfWork.Repository<UsuarioSucursal>().AddEntity(usuarioSucursal);
            var resultado = await _unitOfWork.Complete();

            if (resultado <= 0)
            {
                TempData["Error"] = "No se registraron los datos de sucursal";
                return RedirectToAction("Registrar", "Administracion");
            }

            return RedirectToAction("Registrar", "Administracion");
        }

        public async Task<IActionResult> EditarUsuario([FromForm] UsuarioViewModel usuarioViewModel)
        {
            var usuarioEditar = await _userManager.FindByIdAsync(usuarioViewModel.id!);
            
            if (usuarioEditar != null)
            {   usuarioEditar.Nombres = usuarioViewModel.Nombres;
                usuarioEditar.Apellidos = usuarioViewModel.Apellidos;
                usuarioEditar.Email = usuarioViewModel.Email;

                var rolesActuales = await _userManager.GetRolesAsync(usuarioEditar);
                // Eliminar al usuario de su rol actual
                await _userManager.RemoveFromRolesAsync(usuarioEditar, rolesActuales);
                // Agregar al usuario al nuevo rol
                await _userManager.AddToRoleAsync(usuarioEditar, usuarioViewModel.Rol);

                await _userManager.UpdateAsync(usuarioEditar);

                var usuarioSucursal = await _unitOfWork.Repository<UsuarioSucursal>().GetEntityAsync(us => us.UsuarioId == usuarioViewModel.id);
                // Actualizar la propiedad SucursalId con el nuevo valor
                usuarioSucursal.SucursalId = usuarioViewModel.SucursalId;

                // Actualizar la entidad UsuarioSucursal en la base de datos
                _unitOfWork.Repository<UsuarioSucursal>().UpdateEntity(usuarioSucursal);
                var resultado = await _unitOfWork.Complete();

                if (resultado <= 0)
                {
                    TempData["Error"] = "No se registraron los datos de sucursal";
                    return RedirectToAction("Registrar", "Administracion");
                }
            }
            else
            {
                return RedirectToAction("Registrar", "Administracion");
            }
                return RedirectToAction("Registrar", "Administracion");
        }

        public async Task<IActionResult> EliminarUsuario(string id)
        {
            // Buscar al usuario en la base de datos
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario != null)
            {
                // Actualizar la propiedad isActive a false
            
                // Guardar los cambios en la base de datos
                if (usuario.isActive)
                {
                    usuario.isActive = false;
                    await _userManager.UpdateAsync(usuario);
                }
                else
                {
                    usuario.isActive = true;
                    await _userManager.UpdateAsync(usuario);
                }
              
            }

            return RedirectToAction("Registrar", "Administracion");
        }
        public async Task<IActionResult> Usuarios()
        {
            //Buscar a la persona con id del usuario para relacionar con sucursal (Representante)
            //Buscar la relación para encontrar al estudiante
            //Encontrado al estudiante se le asigna a una lista para asignar a Estudiantes en el ViewModel

            //var repesentante = await _unitOfWork.Repository<UsuarioSucursal>();
            //var representanteSucursal =  await _unitOfWork.Repository<UsuarioSucursal>();
            var baseUrl = _configuration.GetSection("LinkOptions")["url"];
            ViewBag.BaseUrl = baseUrl;
            return View();
        }

        public async Task<IActionResult> Cuentas()
        {
            
            var includes = new List<Expression<Func<CuentaSucursal, object>>>
            {
                p => p.Sucursal!,
                p => p.TipoCuenta!,
                p => p.Banco!
            };
            var cuentas = await _unitOfWork.Repository<CuentaSucursal>().GetAsync(x => x.NumeroCuenta != "", null, includes);
            
            if(cuentas == null)
            {
                return View();

            }
            else
            {
            return View(cuentas);
            }
        }

        public async Task<IActionResult> RegistrarCuentas(CuentasViewModel cuentasViewModel)
        {
            var cuenta = new CuentaSucursal();

            cuenta.SucursalId = cuentasViewModel.SucursalId;
            cuenta.TipoCuentaId = cuentasViewModel.TipoCuentaId;
            cuenta.BancoId = cuentasViewModel.BancoId;
            cuenta.NumeroCuenta = cuentasViewModel.NumeroCuenta;
            cuenta.Estado = true;

            _unitOfWork.Repository<CuentaSucursal>().AddEntity(cuenta);

            var result = await _unitOfWork.Complete();

            if (result <= 0)
            {
                return BadRequest();
            }
            return RedirectToAction("Cuentas","Administracion",cuenta);
        }

        public async Task<IActionResult> EditarCuentas([FromForm] CuentasViewModel cuentasViewModel)
        {
            var cuentasEditar = await _unitOfWork.Repository<CuentaSucursal>().GetByIdAsync(cuentasViewModel.CuentaId);
            if (cuentasEditar != null)
            {
                cuentasEditar.SucursalId = cuentasViewModel.SucursalId;
                cuentasEditar.TipoCuentaId = cuentasViewModel.TipoCuentaId;
                cuentasEditar.BancoId =cuentasViewModel.BancoId;
                cuentasEditar.NumeroCuenta = cuentasViewModel.NumeroCuenta;


                await _unitOfWork.Repository<CuentaSucursal>().UpdateAsync(cuentasEditar);


                //var rubroEditar = await _unitOfWork.Repository<Rubro>().GetByIdAsync(rubroViewModel.idRubro);
                //rubroEditar.Costo = rubroViewModel.Costo;
                //await _unitOfWork.Repository<Rubro>().UpdateAsync(rubroEditar);


                return RedirectToAction("Cuentas", "Administracion");
            }
            else
            {
                return RedirectToAction("Cuentas", "Administracion");
            }
        }
        public async Task<IActionResult> Servicios()
        {
            var includes = new List<Expression<Func<Servicio, object>>>
            {
                p => p.TipoServicio!,
                p => p.Sucursal!
            };
            var servicio = await _unitOfWork.Repository<Servicio>().GetAsync(x => x.Nombre != null, null, includes);

            if (servicio == null)
            {
                return View();

            }
            else
            {
                return View(servicio);
            }
        }
        public async Task<IActionResult> RegistrarServicios(ServicioViewModel servicioViewModel)
        {
            var servicio = new Servicio();
            servicio.Nombre = servicioViewModel.Nombre;
            servicio.Descripcion = servicioViewModel.Descripcion;
            servicio.Periodos = servicioViewModel.Periodos;
            servicio.CostoPeriodo = servicioViewModel.CostoPeriodo;
            servicio.Link = servicioViewModel.Link;
            servicio.TipoServicioId = servicioViewModel.TipoServicioId;

            _unitOfWork.Repository<Servicio>().AddEntity(servicio);

            var result = await _unitOfWork.Complete();

            if (result <= 0)
            {
                TempData["Error"] = "No se registro servicio";
                return RedirectToAction("Servicios", "Administracion");
            }
            return RedirectToAction("Servicios", "Administracion", result);

        }
        [HttpPost]
        public async Task<IActionResult> EditarServicio(ServicioViewModel servicioViewModel)
        {
           var servicioEditar = await _unitOfWork.Repository<Servicio>().GetByIdAsync(servicioViewModel.TipoServicioId);
            if (servicioEditar != null)
            {
                servicioEditar.Nombre = servicioViewModel.Nombre;
                servicioEditar.Descripcion = servicioViewModel.Descripcion;
                servicioEditar.Periodos = servicioViewModel.Periodos;
                servicioEditar.CostoPeriodo = servicioViewModel.CostoPeriodo;
                servicioEditar.Link = servicioViewModel.Link;
                await _unitOfWork.Repository<Servicio>().UpdateAsync(servicioEditar);

                return RedirectToAction("Servicios", "Administracion");
            }
            else
            {
                return RedirectToAction("Servicios", "Administracion");
            }
        }

        
        public async Task<IActionResult> Consolidacion()
        {

            //busco todos los representantes
            var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Representante==true);

            if(representante.Count() <= 0)
            {
                TempData["Error"] = "No existen representantes";
                return RedirectToAction("Registrar", "Administracion");

            }

            // Estado del Registro de pago
    

             var includesSucursal = new List<Expression<Func<UsuarioSucursal, object>>>
             {
                 p => p.Sucursal!
             };

             var includePersona = new List<Expression<Func<Relacion, object>>>
             {
                 p => p.Persona!,
                 p => p.Persona2!

             };

            // var listaObligaciones = new List<ObigacioneRepresentante>();
             var lista2 = new List<ConsolidacionViewModel>();
            /* var listaTransferencias = new List<TransferenciasViewModel>();
             var ListaRecazoSeguro = new List<RechazoSeguroViewModel>();
            List<Persona> listaEstudiantes = new List<Persona>();*/

            string ResumenEstado = "";

            ViewBag.ObligacionesRepresentante = "";
            ViewBag.RechazoSeguroRepresentante = "";

            foreach (var item in representante)
            {
                
                //por cada representate busco sus hijos     
                var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == item.Id, null, includePersona);
                

                var codigoEstudiante = 0;
                //cada representate puede tener uno o más hijos
                foreach (var item2 in relacionesRepresentante)
                {
                    ResumenEstado = "";
                    codigoEstudiante = item2.PersonaId;
                    
                    //busco las matrículas por cada estudiante
                    var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codigoEstudiante, null);
                    
                    foreach(var matricula in MatriculaEstudiante)
                    {//por cada matrícula busco sus obligaciones
                        var codigoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().Id;

                        //busco las obligaciones de esa matricula
                        var includes = new List<Expression<Func<ObligacionPendiente, object>>>
                        {
                            p => p.EstadoCuota!,
                            p => p.Rubro.TipoRubro!,

                        };

                        var obligaciones = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante, null, includes);
                      
                        
                       
                        foreach (var item3 in obligaciones)
                        {
                            //tomo los estados diferentes de Pagado para alertar al usuario al fina si ResumenEstado es vacio pongo pagado
                            if (item3.EstadoCuotaId!=3)
                                ResumenEstado = item3.EstadoCuota.Descripcion;
                            
                        }
                    }

                }
               

                //por cada representante busca la sucursal a ala que pertenence
                if (item?.UsuarioId != null) { 
                var sucursalrep = await _unitOfWork.Repository<UsuarioSucursal>().GetEntityAsync(x => x.UsuarioId == item.UsuarioId, includesSucursal);

                    if (sucursalrep!=null) {
                        if (relacionesRepresentante.FirstOrDefault() != null) { 
                    var consolidacion = new ConsolidacionViewModel
                    {
                        IdRepresentante= relacionesRepresentante.FirstOrDefault().Persona2.Id,
                        Nombre = relacionesRepresentante.FirstOrDefault().Persona2.Nombres,
                        Apellido = relacionesRepresentante.FirstOrDefault().Persona2.Apellidos,
                        CedulaRepresentante = relacionesRepresentante.FirstOrDefault().Persona2.Identificacion,
                        NombreEstudiante = relacionesRepresentante.FirstOrDefault().Persona.Nombres,
                        ApellidoEstudiante = relacionesRepresentante.FirstOrDefault().Persona.Apellidos,
                        Sucursal = sucursalrep.Sucursal.Nombre,
                        Estado = ResumenEstado.IsNullOrEmpty()?"Pagada": ResumenEstado,

                    };
                            lista2.Add(consolidacion);
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine("");
                    }
                }
            }

           


            if (representante == null)
             {
                 return View();

             }
             else
             {

                 return View(lista2);
             }
             
        }

        public async Task<IActionResult> ConsolidarOblicaciones( string cedulaRepresentante, int idRepresentante)
        {
            string ContenidoRespuesta = "Contenido de respuesta Obligaciones";

            try
            {

            
                var listaObligaciones = new List<ObigacionesRepresentanteViewModel>();
                //por cada representate busco sus hijos     
                var includePersona = new List<Expression<Func<Relacion, object>>>
                 {
                     p => p.Persona!,
                     p => p.Persona2!

                 };
                var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == idRepresentante, null, includePersona);


                var codigoEstudiante = 0;
                //cada representate puede tener uno o más hijos
                foreach (var item2 in relacionesRepresentante)
                {
                    codigoEstudiante = item2.PersonaId;

                    //busco las matrículas por cada estudiante
                    var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codigoEstudiante, null);

                    foreach (var matricula in MatriculaEstudiante)
                    {//por cada matrícula busco sus obligaciones
                        var codigoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().Id;

                        //busco las obligaciones de esa matricula
                        var includes = new List<Expression<Func<ObligacionPendiente, object>>>
                            {
                                p => p.EstadoCuota!,
                                p => p.Rubro.TipoRubro!,
                                 p => p.RegistroPagos!,
                                p => p.RegistroPagos.TipoPago!

                            };

                        var obligaciones = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante, null, includes);


                        foreach (var item3 in obligaciones)
                        {
                            //tomo los estados diferentes de Pagado para alertar al usuario al fina si ResumenEstado es vacio pongo pagado

                            //una matrícula puede tener una o más obligaciones: matrícula,pensión,seguro estudiantil
                            var obligacionesRepresentante = new ObigacionesRepresentanteViewModel
                            {
                                Id = item3.Id,//.FirstOrDefault().Id,
                                IdRepresentante = idRepresentante,
                                Concepto = item3.Rubro.TipoRubro.Descripcion,
                                Valor = item3.Valor,
                                Descuento = item3.Descuento,
                                ValorFinal = item3.ValorFinal,
                                Abono = item3.Abono,
                                Saldo = item3.Saldo,
                                Estado = item3.EstadoCuota.Descripcion,
                                EstadoId = item3.EstadoCuotaId,
                                MedioPago = item3.RegistroPagosId!=null?item3.RegistroPagos.TipoPago.Descripcion:"",

                            };
                            listaObligaciones.Add(obligacionesRepresentante);


                        }
                    }
                }
            
                string cabecera = "<table class=\"table table-striped table - hover table - bordered\"><thead><tr class=\"fw-bold fs-5\"><th>Concepto</th><th>Valor</th><th>Descuento %</th><th>Descuento</th><th>Abono</th><th>Saldo</th><th>Estado</th><th>Medio Pago</th></tr></thead>";
                string body = "<tbody>";
                string pie = "</table>";
            
            
                foreach (var obligacion in listaObligaciones)
                {

                    body = body +"<tr>";
                    body = body + "<td>"+ obligacion.Concepto + "</td>";
                    body = body + "<td align=\"right\">" + obligacion.Valor+"</td>";
                    body = body + "<td align=\"right\">" + obligacion.PorcentajeDescuento+"</td>";
                    body = body + "<td align=\"right\">" + obligacion.Descuento+"</td>";
                    //body = body + "<td align=\"right\">" + obligacion.ValorFinal+"</td>";
                    body = body + "<td align=\"right\">" + obligacion.Abono+"</td>";
                    body = body + "<td align=\"right\">" + obligacion.Saldo+"</td>";
                    body = body + "<td>"+obligacion.Estado+"</td>";
                    body = body + "<td>" + obligacion.MedioPago + "</td>";
                    body = body + "</tr>";
                
                }
                body = body + "</tbody>";
                ContenidoRespuesta = cabecera +body+ pie;

                return Ok(ContenidoRespuesta);
            }
            catch (Exception ex)
            {
                return BadRequest("Error "+ContenidoRespuesta);
            }
        }

        public async Task<IActionResult> ConsolidarTransferencias(string cedulaRepresentante, int idRepresentante)
        {

            string ContenidoRespuesta = "Contenido de respuesta tranferencias";

            try 
            { 
                
                var listaTransferencias = new List<TransferenciasViewModel>();
               
                //del representate busco sus hijos     
                var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == idRepresentante);


                var codigoEstudiante = 0;
                //cada representate puede tener uno o más hijos
                foreach (var item2 in relacionesRepresentante)
                {
                    codigoEstudiante = item2.PersonaId;

                    //busco las matrículas por cada estudiante
                    var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codigoEstudiante, null);

                    foreach (var matricula in MatriculaEstudiante)
                    {//por cada matrícula busco sus obligaciones
                        var codigoMatriculaEstudiante = matricula.Id;

                        //busco las obligaciones de esa matricula
                        var includes = new List<Expression<Func<ObligacionPendiente, object>>>
                            {
                               p => p.Matricula.Persona!,
                                p => p.Rubro.TipoRubro!,
                                p => p.RegistroPagos!,
                                p=> p.RegistroPagos.EstadoPago!,
                                p => p.RegistroPagos.CuentaSucursal.TipoCuenta!,
                                p => p.RegistroPagos.CuentaSucursal.Banco!
                            };

                        //busco si la matrícula tiene pago con transferencia
                        var obligaciones2 = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante);
                        var obligaciones = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante && x.RegistroPagos.TipoPagoId == 1, null, includes);

                        if (obligaciones.Count() > 0)
                        {
                            foreach (var item3 in obligaciones)
                            {


                                //una matrícula puede tener una o más pagos: pago de matrícula, pago de pensión y seguro

                                var tarsferenciasRepresentante = new TransferenciasViewModel
                                {
                                    Estudiante= item3.Matricula?.Persona?.Nombres +" "+ item3.Matricula?.Persona?.Apellidos,
                                    Id = item3.RegistroPagosId,
                                    IdRepresentante = idRepresentante,//guardo representante para en la vista mostrar por representante
                                                                      //ObligacionPendienteId = Transferencias.FirstOrDefault().ObligacionPendienteId,
                                    Concepto = item3.Rubro.TipoRubro.Descripcion,
                                    NumeroReferencia = item3.RegistroPagos.NumeroReferencia,
                                    Valor = item3.RegistroPagos.Valor,
                                    FechaTransaccion = item3.RegistroPagos.FechaTransaccion,
                                    EstadoPagoId = item3.RegistroPagos.EstadoPagoId,
                                    EstadoPago = item3.RegistroPagos.EstadoPago.Descripcion,

                                    FotoTransferencia = item3.RegistroPagos.Foto,

                                        CuentaDestinoId = item3.RegistroPagos.CuentaSucursalId, // Transferencias.FirstOrDefault().CuentaSucursalId,
                                        CuentaDestino = item3.RegistroPagos.CuentaSucursal.Banco.Nombre + " "
                                                        + item3.RegistroPagos.CuentaSucursal.TipoCuenta.Descripcion + " "
                                                        + item3.RegistroPagos.CuentaSucursal.NumeroCuenta,
                                        
                                    };

                                //if (!listaTransferencias.Any(t => t.NumeroReferencia == tarsferenciasRepresentante.NumeroReferencia))
                                //{

                                    listaTransferencias.Add(tarsferenciasRepresentante);
                                //}
                                


                            }
                        }
                    }

               
                }

                string cabecera = "<table class=\"table table-striped table - hover table - bordered\"><thead><tr class=\"fw-bold fs-5\"><th>Estudiante</th><th>Concepto</th><th>Referencia #</th><th>Valor</th><th>Fecha Transacción</th><th>Cuenta Destino</th><th>Estado</th><th>Acción</th></tr></thead>";
                string body = "<tbody>";
                string pie = "</table>";

                var baseUri = _configuration.GetSection("LinkOptions")["url"];
                 baseUri = baseUri + "ImagenPagoTransferencia/";

                foreach (var registro in listaTransferencias)
                {

                    body = body + "<tr>";
                    body = body + "<td>" + registro.Estudiante + "</td>";
                    body = body + "<td>" + registro.Concepto + "</td>";
                    body = body + "<td><a target=\"_blank\" href=\"" + baseUri + registro.FotoTransferencia + "\">" + registro.NumeroReferencia + "</a></td>";
                    body = body + "<td align=\"right\">" + registro.Valor + "</td>";
                    body = body + "<td align=\"center\">" + registro.FechaTransaccion.ToString("dd/MM/yyyy") + "</td>";
                    body = body + "<td>" + registro.CuentaDestino + "</td>";
                   
                    body = body + "<td align=\"right\">" + registro.EstadoPago + "</td>";

                    if(registro.EstadoPagoId==2)
                    {
                        body = body + "<td>";
                        body = body + "<select class=\"form-select\" onchange=\"CambioEstado('"+registro.Id+"' ,this )\" > ";
                        body = body + "<option value=\"\">Seleccionar....</option>";
                        body = body + "<option value=\"3\">Pagada</option>";
                        body = body + "<option value=\"4\">Anulada</option>";
                        body = body + "</select>";
                        body = body + "</td>";
                    }
                    else
                        body = body + "<td>&nbsp;</td>";

                
                    body = body + "</tr>";

                }
                body = body + "</tbody>";
                ContenidoRespuesta = cabecera + body + pie;
                return Ok(ContenidoRespuesta);
            }
            catch (Exception ex)
            {
                return BadRequest("Error "+ContenidoRespuesta);
            }
        }

        
        public async Task<IActionResult> ConsolidarPagosTarjeta(string cedulaRepresentante, int idRepresentante)
        {

            string ContenidoRespuesta = "Contenido de respuesta pagos tarjeta";

            try
            {
                
                var listaTx = new List<PagosTarjetaViewModel>();

              
                //por cada representate busco sus hijos     
                var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == idRepresentante);


                var codigoEstudiante = 0;
                //cada representate puede tener uno o más hijos
                foreach (var item2 in relacionesRepresentante)
                {
                    codigoEstudiante = item2.PersonaId;

                    //busco las matrículas por cada estudiante
                    var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codigoEstudiante, null);

                    foreach (var matricula in MatriculaEstudiante)
                    {//por cada matrícula busco sus obligaciones
                        var codigoMatriculaEstudiante = matricula.Id;

                        //busco las obligaciones de esa matricula
                        var includes = new List<Expression<Func<ObligacionPendiente, object>>>
                            {
                                p => p.EstadoCuota!,
                                p => p.Rubro.TipoRubro!,
                                p => p.RegistroPagos!,
                                p=> p.RegistroPagos.EstadoPago!

                            };

                        //busco si la matrícula tiene pago con tarjeta 
                        var obligaciones = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante && x.RegistroPagos.TipoPagoId==2, null, includes);


                        if(obligaciones.Count()>0)
                        {//si fue pagada con tarjeta tomo sus datos
                            
                            foreach (var item3 in obligaciones)
                            {
                              //una matrícula puede tener una o más pagos: pago de matrícula, pago de pensión y seguro
                                
                                    var pagosXTCRepresentante = new PagosTarjetaViewModel
                                    {
                                        Id = item3.RegistroPagosId,
                                        IdRepresentante = idRepresentante,//guardo representante 
                                        Concepto = item3.Rubro.TipoRubro.Descripcion,
                                        NumeroReferencia = item3.RegistroPagos.NumeroReferencia,
                                        Valor = item3.RegistroPagos.Valor,
                                        FechaTransaccion = item3.RegistroPagos.FechaTransaccion,
                                        EstadoPagoId = item3.RegistroPagos.EstadoPagoId,
                                        EstadoPago = item3.RegistroPagos.EstadoPago.Descripcion,
                                        EstadoTarjeta = item3.RegistroPagos.EstadoTarjeta,
                                        DescripcionTarjeta = item3.RegistroPagos.DescripcionTarjeta,
                                        ObservacionTarjeta = item3.RegistroPagos.ObservacionTarjeta,
                                        IdTransaccion = item3.RegistroPagos.idTxTarjeta,
                                        codTransaccion = item3.RegistroPagos.codTx,
                                        Operador = item3.RegistroPagos.CreatedBy
                                    };
                                    listaTx.Add(pagosXTCRepresentante);
                                


                            }

                        }

                       
                    }


                }

                string cabecera = "<table id=\"Detalle\" class=\"table table-striped table-row-bordered gy-5 gs-7\" ><thead><tr class=\"fw-bold fs-5\"><th>Concepto</th><th>Referencia #</th><th>Valor</th><th>Fecha</th><th>Estado Tarjeta</th><th>Descripción Tarjeta</th><th>Observación Tarjeta</th><th>Id transacción</th><th>Código transacción</th><th>Operador</th><th>Estado Pago</th><th>Acción</th></tr></thead>";
                string body = "<tbody>";
                string pie = "</table>";


                foreach (var registro in listaTx)
                {

                    body = body + "<tr>";
                    body = body + "<td>" + registro.Concepto + "</td>";
                    body = body + "<td align=\"right\">" + registro.NumeroReferencia + "</td>";
                    body = body + "<td align=\"right\">" + registro.Valor + "</td>";
                    body = body + "<td align=\"center\">" + registro.FechaTransaccion.ToString("dd/MM/yyyy") + "</td>";
                    body = body + "<td align=\"right\">" + registro.EstadoTarjeta + "</td>";
                    body = body + "<td align=\"right\">" + registro.DescripcionTarjeta + "</td>";
                    body = body + "<td align=\"right\">" + registro.ObservacionTarjeta + "</td>";
                    body = body + "<td align=\"right\">" + registro.IdTransaccion + "</td>";
                    body = body + "<td align=\"right\">" + registro.codTransaccion + "</td>";
                    body = body + "<td align=\"right\">" + registro.Operador + "</td>";

                    body = body + "<td>" + registro.EstadoPago + "</td>";
                    

                    if (registro.EstadoPagoId == 2)
                    {
                        body = body + "<td>";
                        body = body + "<select class=\"form-select\" onchange=\"CambioEstado('" + registro.Id + "' ,this )\" > ";
                        body = body + "<option value=\"\">Seleccionar....</option>";
                        body = body + "<option value=\"3\">Pagada</option>";
                        body = body + "<option value=\"4\">Anulada</option>";
                        body = body + "<option value=\"1\">Reversada</option>";
                        body = body + "</select>";
                        body = body + "</td>";
                    }
                    else
                        body = body + "<td>&nbsp;</td>";


                    body = body + "</tr>";

                }
                body = body + "</tbody>";
                ContenidoRespuesta = cabecera + body + pie;
                return Ok(ContenidoRespuesta);
            }
            catch (Exception ex)
            {
                return BadRequest("Error " + ContenidoRespuesta);
            }
        }

        public async Task<IActionResult> ConsolidarRechazoSeguro(string cedulaRepresentante, int idRepresentante)
        {

            string ContenidoRespuesta = "Contenido de respuesta rechazo seguros";
            try
            {
                //sacar representate 
               var representante= await _unitOfWork.Repository<Persona>().GetEntityAsync(t => t.Id == idRepresentante);

                //por cada representante busco si a rechazado seguro
                var RechazoSeguro = await _unitOfWork.Repository<JustificaNoSeguro>().GetAsync(x => x.IdentificacionRepresentante == representante.Identificacion);
                var ListaRecazoSeguro = new List<RechazoSeguroViewModel>();
                if (RechazoSeguro.Count() > 0)
                {
                    foreach (var rechazoSeguro in RechazoSeguro)
                    {
                        var RechazoSeguroRepresentante = new RechazoSeguroViewModel
                        {
                            Id= rechazoSeguro.Id,
                            FechaRegistro = rechazoSeguro.FechaRegistro,
                            NombreRepresentante = rechazoSeguro.NombreRepresentante,
                            TelefonoRepresentante = rechazoSeguro.TelefonoRepresentante,
                            IdentificacionRepresentante = rechazoSeguro.IdentificacionRepresentante,
                            Justificacion = rechazoSeguro.Justificacion,
                            Estado = rechazoSeguro.Estado,
                            EstadoDescripcion = rechazoSeguro.Estado == true ? "Aprobado" : "Por aprobar",

                        };
                        ListaRecazoSeguro.Add(RechazoSeguroRepresentante);
                    }
                }


                string cabecera = "<table class=\"table table-striped table - hover table - bordered\"><thead><tr class=\"fw-bold fs-5\"><th>Fecha Registro</th><th>Representante</th><th>Teléfono</th><th>Identificación</th><th>Justificación</th><th>Estado</th><th>Acción</th></tr></thead>";
                string body = "<tbody>";
                string pie = "</table>";


                foreach (var registro in ListaRecazoSeguro)
                {

                    body = body + "<tr>";
                    body = body + "<td>" + registro.FechaRegistro + "</td>";
                    body = body + "<td>" + registro.NombreRepresentante + "</td>";
                    body = body + "<td>" + registro.TelefonoRepresentante + "</td>";
                    body = body + "<td>" + registro.IdentificacionRepresentante + "</td>";
                    body = body + "<td><textarea cols=\"50\" rows=\"3\" readonly>" + registro.Justificacion + "</textarea></td>";
                    body = body + "<td>" + registro.EstadoDescripcion + "</td>";

                    if (registro.Estado == false)
                    {
                        body = body + "<td>";
                        body = body + "<select class=\"form-select\" onchange=\"ApruebaRechazo('" + registro.IdentificacionRepresentante + "','" + registro.Id + "' ,this )\" > ";
                        body = body + "<option value=\"\">Seleccionar....</option>";
                        body = body + "<option value=\"1\">Aprobada</option>";
                        body = body + "</select>";
                        body = body + "</td>";
                    }
                    else
                        body = body + "<td>&nbsp;</td>";


                    body = body + "</tr>";

                }
                body = body + "</tbody>";
                ContenidoRespuesta = cabecera + body + pie;
                return Ok(ContenidoRespuesta);

            }
            catch (Exception ex)
            {
                return BadRequest("Error " + ContenidoRespuesta);
            }
        }


        public async Task<IActionResult> CambiarEstadoCuota(int codPago, int codnuevoEstado)
        {
            //actualizo estado en tabla registros pagos
            var registroObligacion = await _unitOfWork.Repository<RegistroPagos>().GetAsync(x => x.Id.Equals(codPago));

            if (registroObligacion == null || registroObligacion.Count <= 0)
            {
                TempData["Error"] = "No se actualizo el estado de la obligacion";
                return RedirectToAction("Consolidacion", "Administracion");
            }

            RegistroPagos registro = new RegistroPagos();
            registro = registroObligacion.FirstOrDefault();
            registro.EstadoPagoId = codnuevoEstado;

            _unitOfWork.Repository<RegistroPagos>().UpdateEntity(registro);
            var resultado = await _unitOfWork.Complete();

            if (resultado <= 0)
            {
                TempData["Error"] = "No se actualizo el estado de la obligacion de pagos";

            }

            //actualizo estado en tabla obligaciones pendientes, un pago puede afectar una o más obligaciones
            var registroObligacion2 = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.RegistroPagosId.Equals(codPago));

            if (registroObligacion2 == null || registroObligacion2.Count <= 0)
            {
                TempData["Error"] = "No existe obligaciones";
                return RedirectToAction("Consolidacion", "Administracion");
            }


           

            foreach (var obligacion in registroObligacion2)
            {

                 obligacion.EstadoCuotaId = codnuevoEstado;

                _unitOfWork.Repository<ObligacionPendiente>().UpdateEntity(obligacion);
                var resultado2 = await _unitOfWork.Complete();

                if (resultado2 <= 0)
                {
                    TempData["Error"] = "No se actualizo el estado de la obligacion";

                }
            }
            

            return RedirectToAction("Consolidacion", "Administracion");
        }

        
        public async Task<IActionResult> ApruebaRechazoSeguro(string idRepresentante,int idrechazoSeguro)
        {
            ObligacionPendiente registro = new ObligacionPendiente();
            //busco el representate por su cédula
            var representante = await _unitOfWork.Repository<Persona>().GetAsync(x => x.Representante == true && x.Identificacion.Equals(idRepresentante));
            if (representante == null || representante.Count <= 0)
            {
                TempData["Error"] = "No existe este representante";
                return RedirectToAction("Consolidacion", "Administracion");
            }
            var idrepresentante = representante.FirstOrDefault().Id;
            //busco las relaciones del represetante
            var codigoEstudiante = 0;
            var relacionesRepresentante = await _unitOfWork.Repository<Relacion>().GetAsync(x => x.Persona2Id == idrepresentante, null);

            foreach (var item in relacionesRepresentante)// puede tener uno o mas hijos
            {
                codigoEstudiante = item.PersonaId;
                //busco las matriculas de los representados
                var MatriculaEstudiante = await _unitOfWork.Repository<Matricula>().GetAsync(x => x.PersonaId == codigoEstudiante, null);
                var codigoMatriculaEstudiante = MatriculaEstudiante.FirstOrDefault().Id;
                //busco las obligaciones de seguro rechazadas
                var obligacionesSeguro = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.MatriculaId == codigoMatriculaEstudiante && x.RubroId == 3 && x.EstadoCuotaId == 6);
                if (obligacionesSeguro.Count() > 0)
                {
                    //cambio el estado rechazo seguro a anulada , puede tener 1 o más representados
                    foreach(var obligacion in obligacionesSeguro)
                    {
                        //traigo el registro a modificar
                        var registroObligacion = await _unitOfWork.Repository<ObligacionPendiente>().GetAsync(x => x.Id.Equals(obligacionesSeguro.FirstOrDefault().Id));

                        registro = registroObligacion.FirstOrDefault();
                        registro.EstadoCuotaId = 4;//Anulada

                        _unitOfWork.Repository<ObligacionPendiente>().UpdateEntity(registro);
                        var resultado = await _unitOfWork.Complete();

                        if (resultado <= 0)
                        {
                            TempData["Error"] = "No se actualizo el estado de la obligacion";
                            return RedirectToAction("Consolidacion", "Administracion");
                        }

                    }
                   
                }

            }

            //actulaizo es estado del registo de rechazo seguro
            //traigo el registro a modificar
            var registroJustificacion = await _unitOfWork.Repository<JustificaNoSeguro>().GetAsync(x => x.IdentificacionRepresentante.Equals(idRepresentante)&& x.Estado==false && x.Id== idrechazoSeguro);

            var registro2 = registroJustificacion.FirstOrDefault();
                registro2.Estado = true;//Justificada

            _unitOfWork.Repository<JustificaNoSeguro>().UpdateEntity(registro2);
            var resultado2 = await _unitOfWork.Complete();

            if (resultado2 <= 0)
            {
                TempData["Error"] = "No se actualizo el estado del rechazo";
                
            }

            return RedirectToAction("Consolidacion", "Administracion");

        }
    }
}
