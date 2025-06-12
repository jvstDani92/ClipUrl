using ClipUrl.Api.Configuration;
using ClipUrl.Application;
using ClipUrl.Infrastructure;
using ClipUrl.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddIdentityAndJwtAuth(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var env = app.Environment;

    await IdentitySeeder.SeedAsync(scope.ServiceProvider, builder.Configuration, env.IsDevelopment());
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
