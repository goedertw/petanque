using Microsoft.EntityFrameworkCore;
using Petanque.Services.Interfaces;
using Petanque.Services.Services;
using Petanque.Storage;

var builder = WebApplication.CreateBuilder(args);

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

// Services toevoegen
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IDagKlassementService, DagKlassementService>();
builder.Services.AddScoped<ISpelverdelingService, SpelverdelingService>();
builder.Services.AddScoped<IAanwezigheidService, AanwezigheidService>();
builder.Services.AddScoped<IScoreService, ScoreService>();
builder.Services.AddScoped<ISpeeldagService, SpeeldagService>();

var app = builder.Build();

app.UseCors("AllowFrontend");

app.MapControllers();
app.UseHttpsRedirection();
app.UseRouting();
app.Run();
