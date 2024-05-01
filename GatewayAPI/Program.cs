using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
	{
		options.AddPolicy("CorsPolicy",
			builder => builder.WithOrigins("http://localhost:5162", "https://localhost:7297")
			.AllowAnyMethod()
			.AllowAnyHeader());
	}
);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.UseCors("CorsPolicy");

await app.UseOcelot();

app.Run();
