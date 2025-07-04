using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var politicaUsuariosautenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();




//SERVICIOS DE CONTENEDOR
builder.Services.AddControllersWithViews(opciones=>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosautenticados));
});

//REPO TIPOS CUENTAS
builder.Services.AddTransient<IRepositorioTiposCuentas, RepositorioTiposCuentas>();
//REPO USUARUIOS
builder.Services.AddTransient<IServiciosUsuarios, ServicioUsuarios>();
//REPO CUENTAS
builder.Services.AddTransient<IRepositorioCuentas, RepositorioCuentas>();
//REPO Categorias
builder.Services.AddTransient<IRepositorioCategorias, RepositorioCategorias>();
//REPO Transacciones
builder.Services.AddTransient<IRepositorioTransacciones, RepositorioTransacciones>();
builder.Services.AddHttpContextAccessor();
////REPO USUARIOS
builder.Services.AddTransient<IRepositorioUsuarios, RepositorioUsuarios>();

builder.Services.AddTransient<SignInManager<Usuario>>();

//
builder.Services.AddTransient<IUserStore<Usuario>, UsuarioStore>();
builder.Services.AddIdentityCore<Usuario>(opciones =>
{
    opciones.Password.RequireDigit = false;
    opciones.Password.RequireLowercase = false;
    opciones.Password.RequireUppercase = false;
    opciones.Password.RequireNonAlphanumeric = false;
}).AddErrorDescriber<MensajesDeErrorIdentity>();

//COOKIES
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, opciones=>
{
    opciones.LoginPath = "/usuarios/Login";
});







//
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IServicioReportes, ServiciosReportes>();


//AUTOMAPPER
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//COOKIE
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transacciones}/{action=Index}/{id?}");

app.Run();
