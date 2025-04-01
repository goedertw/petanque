using Microsoft.EntityFrameworkCore;
using Petanque.Services.Interfaces;
using Petanque.Services.Services;
using Petanque.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("LocalMySQL");
var serverVersion = new MySqlServerVersion(ServerVersion.AutoDetect(connectionString));
builder.Services.AddDbContext<Id312896PetanqueContext>(options =>
    options.UseMySql(connectionString, serverVersion));

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IDagKlassementService, DagKlassementService>();
builder.Services.AddScoped<ISpelverdelingService, SpelverdelingService>();
builder.Services.AddScoped<IAanwezigheidService, AanwezigheidService>();
builder.Services.AddScoped<IScoreService, ScoreService>();
builder.Services.AddScoped<ISpeeldagService, SpeeldagService>();

var app = builder.Build();

app.MapControllers();
app.UseHttpsRedirection();
app.UseRouting();
app.Run();