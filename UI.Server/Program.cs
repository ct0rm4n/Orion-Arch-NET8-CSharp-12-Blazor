using Autofac.Extensions.DependencyInjection;
using Autofac;
using UI.Server.Components;
using DependencyResolvers.Autofac;
using DependencyResolvers;
using UI.Client.Pages;
using Infra.IoC.DependencyResolvers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddWebLayerInjections();
builder.Services.AddSession();
builder.Services.AddIdentityService();
builder.Services.AddApplicationLayerInjections();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureServices(x => x.AddAutofac()).UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacPersistanceModule());
    builder.RegisterModule(new AutofacInnerInfrastructureModule());
});

builder.Services.AddScoped<StartupInject>();
var inject = new StartupInject();
inject.ConfigureServices(builder);

builder.Services.AddBlazorBootstrap();//todo

WebApplication app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

inject.BuildMain(app);
app.Run();