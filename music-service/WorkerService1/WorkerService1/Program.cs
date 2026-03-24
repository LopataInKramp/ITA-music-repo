using Microsoft.EntityFrameworkCore;
using WorkerService1.Data;
using WorkerService1.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(_ => { });

var connectionString = builder.Configuration.GetConnectionString("MusicDb")
	?? "Host=localhost;Port=5432;Database=musicdb;Username=music;Password=musicpass";

if (builder.Environment.IsEnvironment("Testing"))
{
	builder.Services.AddDbContext<MusicDbContext>(options =>
		options.UseInMemoryDatabase("MusicServiceTestingDb"));
}
else
{
	builder.Services.AddDbContext<MusicDbContext>(options => options.UseNpgsql(connectionString));
}
builder.Services.AddScoped<IMusicRepository, MusicRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpLogging();
app.UseAuthorization();
app.MapControllers();

if (!app.Environment.IsEnvironment("Testing"))
{
	using var scope = app.Services.CreateScope();
	var db = scope.ServiceProvider.GetRequiredService<MusicDbContext>();
	db.Database.EnsureCreated();
}

app.Run();

public partial class Program;
