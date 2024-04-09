using Application.Config.KTheme;
using Application.Contracts.Identity;
using Application.Mappings;
using Application.Models.Email;
using Application.Models.Helpers;
using Application.Persistence;
using AutoMapper;
using Domain;
using Infrasctructure.Persistence;
using Infrasctructure.Repositories;
using Infrasctructure.Services.Auth;
using Infrasctructure.Services.Email;
using Infrasctructure.Services.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReinventedPuembo.AccesoDatos;
using ReinventedPuembo.Interfaces;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Add services to the container.
builder.Services.AddControllersWithViews(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));

}).AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseMySql(builder.Configuration.GetConnectionString("cadenaSQL"), new MySqlServerVersion(new Version(11, 1, 0)))

);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


//AutoMapper
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddIdentity<Usuario, RolUsuario>(opt =>
{
    //Vale loguearse con cualquier cuenta
    opt.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


//Redirecciona a la vista de login cuando no ingresa sesi�n
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/usuarios/login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(45);
    });

//Configuracion para trabajar con mis propios dise�os
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    opt =>
    {
        opt.LoginPath = "/usuarios/login";
        opt.AccessDeniedPath = "/usuarios/login";

    });


//Configuraci�n de Email
//EmailService
var emailConfigPuembo = builder.Configuration.GetSection("EmailConfigurationPuembo")
.Get<EmailConfigurationPuembo>();

var emailConfigSantaClara = builder.Configuration.GetSection("EmailConfigurationSantaClara")
.Get<EmailConfigurationSantaClara>();

builder.Services.AddSingleton(emailConfigPuembo!);
builder.Services.AddSingleton(emailConfigSantaClara!);
builder.Services.AddScoped<IEmailServicePuembo, EmailServicePuembo>();
builder.Services.AddScoped<IEmailServiceSantaClara, EmailServiceSantaClara>();
builder.Services.AddScoped<ITemplateService, TemplateService>();

//PasswordOptions
builder.Services.Configure<PasswordConfiguration>(builder.Configuration.GetSection("PasswordOptions"));
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();



builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});


//Interfaces propias

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddScoped<IDataFiles, DataFiles>();
builder.Services.AddScoped<IDocumento, DocumentoRepositorio>();
builder.Services.AddScoped<IRegistroPagos, RegistroPagosRepositorio>();

builder.Services.AddScoped<IKTTheme, KTTheme>();
builder.Services.AddSingleton<IKTBootstrapBase, KTBootstrapBase>();
IConfiguration themeConfiguration = new ConfigurationBuilder()
    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
    .AddJsonFile("wwwroot/config/themesettings.json", optional: false, reloadOnChange: true)
    .Build();

IConfiguration iconsConfiguration = new ConfigurationBuilder()
    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
    .AddJsonFile("wwwroot/config/icons.json", optional: false, reloadOnChange: true)
    .Build();

KTThemeSettings.init(themeConfiguration);
KTIconsSettings.init(iconsConfiguration);

var app = builder.Build();
app.UseForwardedHeaders();
bool Mantenimiento(IConfiguration configuration)
{
    return configuration.GetValue<bool>("Mantenimiento");
}

//app.Use(async (context, next) =>
//{
//    IConfiguration configuration = context.RequestServices.GetRequiredService<IConfiguration>();
//    bool estaEnMantenimiento = Mantenimiento(configuration);


//    if (estaEnMantenimiento)
//    {
//        context.Response.Redirect("/mantenimiento");
//        return;
//    }

//    await next();
//});



app.UseRequestLocalization(opt =>
{
    opt.DefaultRequestCulture = new RequestCulture("MX");

});
// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllerRoute(
    name: "mantenimiento",
    pattern: "/mantenimiento",
    defaults: new { controller = "Home", action = "Mantenimiento" }
);

builder.WebHost.UseWebRoot("wwwroot");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var loggerFactory = service.GetRequiredService<ILoggerFactory>();

    try
    {
        var context = service.GetRequiredService<ApplicationDbContext>();
        var usuarioManager = service.GetRequiredService<UserManager<Usuario>>();
        var roleManager = service.GetRequiredService<RoleManager<RolUsuario>>();
       // await context.Database.MigrateAsync();

        //Insertar Data de prueba
       //await ApplicationDbContextData.LoadDataAsync(context, usuarioManager, roleManager, loggerFactory);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "Error en la migraci�n");
    }
}

app.Run();
