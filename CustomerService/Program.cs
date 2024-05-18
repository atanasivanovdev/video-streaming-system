using CustomerService.Models;
using CustomerService.Services;
using Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("Database"));

builder.Services.AddJwt(builder.Configuration);

builder.Services.AddTransient<IEncryptor, Encryptor>();

builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<InboxService>();
builder.Services.AddHostedService<PubSubHostedService>();

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", builder.Configuration["Authentication:Google:CredentialsFilePath"]);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
