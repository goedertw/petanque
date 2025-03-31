using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Petanque.Services;
using Petanque.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("LocalMySQL");
var serverVersion = new MySqlServerVersion(ServerVersion.AutoDetect(connectionString));
builder.Services.AddDbContext<Id312896PetanqueContext>(options =>
    options.UseMySql(connectionString, serverVersion));

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IDagKlassementService, DagKlassementService>();
builder.Services.AddScoped<IAanwezigheidService, AanwezigheidService>();

var app = builder.Build();
//dit is een test voor een branche

app.MapControllers();
app.UseHttpsRedirection();
app.UseRouting();
app.Run();