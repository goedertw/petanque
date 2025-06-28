using Microsoft.EntityFrameworkCore;
using Petanque.Api.Controllers;
using Petanque.Services;
using Petanque.Services.Interfaces;
using Petanque.Services.Services;
using Petanque.Storage;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community; // Deze regel kan nu mogelijk niet werken als 'LicenseType' niet bestaat


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("*")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();

// Verbinding met de MySQL database
var connectionString = builder.Configuration.GetConnectionString("LocalMySQL");
builder.Services.AddDbContext<Id312896PetanqueContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Services toevoegen
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IDagKlassementService, DagKlassementService>();
builder.Services.AddScoped<ISpelverdelingService, SpelverdelingService>();
builder.Services.AddScoped<IAanwezigheidService, AanwezigheidService>();
builder.Services.AddScoped<IScoreService, ScoreService>();
builder.Services.AddScoped<ISpeeldagService, SpeeldagService>();
builder.Services.AddScoped<IDagKlassementPDFService, DagKlassementPDFService>();
builder.Services.AddScoped<ISpelverdelingPDFService, SpelverdelingPDFService>();

builder.Services.AddScoped<ISeizoensKlassementService, SeizoensKlassementService>();
builder.Services.AddScoped<ISeizoensKlassementPDFService, SeizoensKlassementPDFService>();
builder.Services.AddScoped<ISeizoensService, SeizoensService>();

var app = builder.Build();

app.UseCors("AllowFrontend");

app.MapControllers();
app.UseHttpsRedirection();
app.UseRouting();
app.Run();
