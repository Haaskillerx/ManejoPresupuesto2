using ManejoPresupuesto.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//SERVICIOS DE CONTENEDOR
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transacciones}/{action=Index}/{id?}");

app.Run();
