using Application.Contracts.Identity;
using Application.Models;
using Application.Models.Authorization;
using Application.Models.Email;
using Application.Models.User;
using Application.Persistence;
using AutoMapper;
using Domain;
using Infrasctructure.Services.Email;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using ReinventedPuembo.Models;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;

namespace ReinventedPuembo.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IDataFiles _dataFiles;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        private readonly RoleManager<RolUsuario> _roleManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;

        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailServicePuembo _emailServicePuembo;
        private readonly IEmailServiceSantaClara _emailServiceSantaClara;
        public UsuariosController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IUnitOfWork unitOfWork,
            IDataFiles dataFiles,
            RoleManager<RolUsuario> roleManager,
            IMapper mapper,
            IAuthService authService,
            IEmailServicePuembo emailServicePuembo,
            IEmailServiceSantaClara emailServiceSantaClara,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _dataFiles = dataFiles;
            _roleManager = roleManager;
            _mapper = mapper;
            _authService = authService;
            _emailServicePuembo = emailServicePuembo;
            _emailServiceSantaClara = emailServiceSantaClara;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(Models.RecoveryViewModel model )
        {
            if (!ModelState.IsValid)
            {
            return View(model);

            }

            // Obtener el usuario correspondiente al modelo RecoveryViewModel (por ejemplo, a través de su correo electrónico)
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }
            // Generar el token de restablecimiento de contraseña para el usuario encontrado
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var passwordChangeLink = _configuration.GetSection("LinkOptions")["url"] + "Usuarios/ChangePassword?&token=" + WebUtility.UrlEncode(token) + "&email=" + user.Email;

            var emailChangePassword = new EmailChangePassword
            {
                passwordChangeLink = passwordChangeLink,
                email = user.Email,
                userName = user.UserName,
            };

            await _emailServicePuembo.SendEmailHtml(user.Email, "Cambio de Contraseña", emailChangePassword, "_RecoveryPasswordUserEmail");


            TempData["SweetAlertMessage"] = "Revise su correo electrónico para cambiar la contraseña";

            return RedirectToAction("login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ChangePassword(string token, string email)
        {

            Models.RecoveryPasswordViewModel model = new RecoveryPasswordViewModel();
            model.token = token;
            model.Email = email;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(Models.RecoveryPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);

                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return NotFound();
                }
                var result = await _userManager.ResetPasswordAsync(user, model.token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Contraseña modificada con éxito";
                    return View("login");
                }
                else
                {
                    // Si el cambio de contraseña no fue exitoso, puedes agregar errores de validación al modelo ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            ViewBag.Message = "Contraseña modificada con éxito";
            return View("login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult NewChangePassword()
        {      
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> NewChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                string email = User.FindFirst(ClaimTypes.Email)?.Value;

                // Obtener el usuario por el correo electrónico
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound();
                }

                // Verificar si la contraseña actual coincide
                var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!isPasswordCorrect)
                {
                    ModelState.AddModelError(nameof(model.CurrentPassword), "La contraseña actual no coincide.");
                    return View(model);
                }

                // Cambiar la contraseña del usuario utilizando el método ChangePasswordAsync
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    // Contraseña modificada con éxito, almacenar mensaje en TempData
                    TempData["SuccessMessage"] = "Contraseña modificada con éxito";

                    // Redireccionar a la vista "login" después de cambiar la contraseña
                    return RedirectToAction("Login", "Usuarios");
                }
                else
                {
                    // Si el cambio de contraseña no fue exitoso, puedes agregar errores de validación al modelo ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ListAsync(string tipo)
        {
            var includes = new List<Expression<Func<Persona, object>>>
            {
                p => p.TipoEstudiante!
            };

            switch (tipo)
            {
                case Role.ADMIN:
                case Role.GUARDIA:
                case Role.REPRESENTANTE:
                case Role.ESTUDIANTE:

                    // Obtén la lista de usuarios con un rol específico
                    var userIds = (await _userManager.GetUsersInRoleAsync(tipo)).Select(x => x.Id).ToList();

                    var users = await _unitOfWork.Repository<Persona>()
                        .GetAsync(
                        x => userIds.Contains(x.UsuarioId!),
                        y => y.OrderBy(o => o.Nombres),
                        includes,
                        true
                        );

                    var usersMap = _mapper.Map<IReadOnlyList<UsuarioMap>>(users).Select(elemento => new UsuarioMap
                    {
                        Avatar = elemento.Avatar,
                        Email = elemento.Email,
                        Nombre = elemento.Nombre,
                        Apellido = elemento.Apellido,
                        Cedula = elemento.Cedula,
                        Sucursal = elemento.Sucursal,
                        Rol = tipo
                    });

                    ViewData["tipo"] = tipo;
                    return View(usersMap);

                default:
                    return NotFound();
            }
        }

        [AllowAnonymous]
        public IActionResult Login(string? mensaje = null)
        {
            if (mensaje is not null)
            {
                ViewData["mensaje"] = mensaje;
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel modelo)
        {
            try
            {
                var includes = new List<Expression<Func<Usuario, object>>>
                {
                    p => p.UsuarioSucursal!,
                };

                if (!ModelState.IsValid)
                {
                    return View(modelo);
                }

                var user = await _unitOfWork.Repository<Usuario>()
                        .GetEntityAsync(
                        x => x.Email!.Equals(modelo.Email),
                        includes,
                        false
                        );

                
                if (user is null)
                {
                    return Json(new { status = 404, details = "El usuario y/o contraseña están incorrectos" });
                }

                // Verificar si el usuario está activo
                if (!user.isActive)
                {
                    return Json(new { status = 403, details = "Este usuario está desactivado y no puede iniciar sesión" });
                }

                var includesRepresentate = new List<Expression<Func<Persona, object>>>
                {
                    e => e.TipoEstudiante!,
                };
                var representante = await _unitOfWork.Repository<Persona>().GetEntityAsync(x => x.UsuarioId!.Equals(user.Id) && x.Representante, includesRepresentate);

               

                var sucursalEntity = user.UsuarioSucursal!.FirstOrDefault(x => x.UsuarioId!.Equals(user.Id));
                var sucursal = await _unitOfWork.Repository<Sucursal>().GetEntityAsync(y => y.Id == sucursalEntity!.SucursalId);
                // Crear el claim
                var claims = new List<Claim> {};
                if (representante is not null)
                {
                    //var claimRepresentate = new Claim("Representante", JsonConvert.SerializeObject(representante));
                    //await _userManager.AddClaimAsync(user, claimRepresentate);
                    claims = new List<Claim> {

                    new Claim("Avatar", user.Avatar!),
                    new Claim("Sucursal", sucursal!.Nombre!),
                    new Claim("NombreCompleto", user.Nombres! + " "+ user.Apellidos!),
                     new Claim("Representante", JsonConvert.SerializeObject(representante)),
                    };
                }
                else
                {
                    claims = new List<Claim> {
                    new Claim("Avatar", user.Avatar!),
                    new Claim("Sucursal", sucursal!.Nombre!),
                    new Claim("NombreCompleto", user.Nombres! + " "+ user.Apellidos!),

                    };
                }



                var resultado = await _signInManager.PasswordSignInAsync(
                    user,
                    modelo.Password!,
                    modelo.Recuerdame,
                    lockoutOnFailure: false);



               

                //if (resultado.Succeeded)
                if (resultado.Succeeded)
                {
                   var aa= await _userManager.AddClaimsAsync(user, claims);

                  


                    //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var linkConfirmacion = _configuration.GetSection("LinkOptions")["url"] + "confirmarEmail?usuarioId=" + user.Id + "&token=" + WebUtility.UrlEncode(token) + "&email=" + user.Email;

                    //var emailConfirmed = new EmailConfirmed
                    //{
                    //    linkConfirmacion = _configuration["LinkOptions:url"],
                    //    email = user.Email,
                    //    userName = user.Apellidos,
                    //    name = user.Nombres,
                    //    sucursal = user.UsuarioSucursal!.FirstOrDefault()!.SucursalId,
                    //};



                    //switch (sucursal.Id)
                    //{
                    //    case 1:
                    //        emailConfirmed.password = _emailServicePuembo.GeneratePassword();
                    //        await _emailServicePuembo.SendEmailHtml(user.Email!, "ED SCHOOL PUEMBO TE DA LA BIENVENIDA", emailConfirmed, "_CreateUserEmailPuembo");

                    //        break;
                    //    case 2:
                    //        emailConfirmed.password = _emailServiceSantaClara.GeneratePassword();
                    //        await _emailServiceSantaClara.SendEmailHtml(user.Email!, "ED SCHOOL SANTA CLARA TE DA LA BIENVENIDA", emailConfirmed, "_CreateUserEmail");

                    //        break;
                    //    default:
                    //        throw new Exception();
                    //}



                    return Json(new { status = 200 });
                }
                else
                {
                    return Json(new { status = 400 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = 500, details = ex });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("login", "usuarios");
        }

        public IActionResult Registro(string tipo)
        {
            try
            {
                ViewBag.TipoPersona = tipo.ToLower();

                switch (tipo)
                {

                    case Role.ADMIN:
                    case Role.PADRE:
                    case Role.REPRESENTANTE:
                    case Role.ESTUDIANTE:
                        return View();

                    default:
                        return NotFound();
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

        }



        [AllowAnonymous]
        public IActionResult ResetPassword(string? mensaje = null)
        {
            if (mensaje is not null)
            {
                ViewData["mensaje"] = mensaje;
            }

            return View();
        }
    }
}