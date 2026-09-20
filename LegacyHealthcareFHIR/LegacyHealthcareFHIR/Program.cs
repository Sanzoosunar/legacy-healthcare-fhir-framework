using LegacyHealthcareFHIR.Core.Enums;
using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Mapping;
using LegacyHealthcareFHIR.Infrastructure.AI;
using LegacyHealthcareFHIR.Infrastructure.Csv;
using LegacyHealthcareFHIR.Infrastructure.Data;
using LegacyHealthcareFHIR.Infrastructure.Processors;
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
builder.Services.AddScoped<ILegacyDataConverter, LegacyDataConverter>();
builder.Services.AddScoped<ILegacyDataValidator, LegacyDataValidator>();
builder.Services.AddScoped<DataValidationProcessor>();

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

builder.Services.AddScoped<ResourceTypeDetectionProcessor>();
builder.Services.AddScoped<FieldMappingProcessor>();


builder.Services.AddHostedService<BackgroundJobWorker>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

builder.Services.AddScoped<ResourceTypeDetectionService>();
builder.Services.AddScoped<FieldMappingService>();

builder.Services.AddScoped<ImportsService>();

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
