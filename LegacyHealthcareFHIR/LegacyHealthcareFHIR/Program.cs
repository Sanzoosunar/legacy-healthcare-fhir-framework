using LegacyHealthcareFHIR.Core.Interfaces;
using LegacyHealthcareFHIR.Core.Mapping;
using LegacyHealthcareFHIR.Core.Transformation;
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSignalR();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

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
builder.Services.AddScoped<IJobProcessor, ResourceTypeDetectionProcessor>();
builder.Services.AddScoped<IJobProcessor, FieldMappingProcessor>();
builder.Services.AddScoped<IJobProcessor, DataValidationProcessor>();
builder.Services.AddScoped<IJobProcessor, FhirTransformationProcessor>();
builder.Services.AddScoped<IJobProcessor, CompletedProcessor>();

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

builder.Services.AddScoped<ResourceTypeDetectionService>();
builder.Services.AddScoped<FieldMappingService>();

builder.Services.AddScoped<ImportsService>();
builder.Services.AddScoped<FhirTransformationProcessor>();
builder.Services.AddScoped<INormalizedDataTransformer, FhirDataTransformer>();

var app = builder.Build();
app.UseCors("AngularClient");


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await DatabaseSeeder.SeedAsync(dbContext);
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();


app.MapHub<SignalRHub>("/hubs/import-job");

app.Run();
