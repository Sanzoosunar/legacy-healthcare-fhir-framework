using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Infrastructure.AI;
using LegacyHealthcareFHIR.Infrastructure.Csv;
using LegacyHealthcareFHIR.Infrastructure.Data;
using LegacyHealthcareFHIR.Infrastructure.Queues;
using LegacyHealthcareFHIR.Infrastructure.Repositories;
using LegacyHealthcareFHIR.Infrastructure.Services;
using LegacyHealthcareFHIR.Web.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSignalR();
builder.Services.AddSingleton<IAiService, OpenAiService>();
builder.Services.AddScoped<ISignalRNotifier, SignalRNotifier>();
builder.Services.AddScoped<LegacyCsvReader>();
builder.Services.AddScoped<SourceFileService>();

builder.Services.AddScoped<IJobRepository, JobRepository>();

builder.Services.AddScoped<IResourceTypeAiService,ResourceTypeAiService>();
builder.Services.AddScoped<IFieldMappingAiService, FieldMappingAiService>();

builder.Services.AddSingleton(sp =>
{
    var environment =
        sp.GetRequiredService<IWebHostEnvironment>();

    var uploadDirectory = Path.Combine(
        environment.ContentRootPath,
        "App_Data",
        "Uploads");

    return new LocalFileStorageService(uploadDirectory);
});

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await DatabaseSeeder.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<SignalRHub>("/notifications/fhir-integration");

app.Run();
