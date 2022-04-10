using PlatformService.Data;
using Microsoft.EntityFrameworkCore;
using PlatformService.SyncDataServices.Http;
using PlatformService.AsyncDataServices;
using PlatformService.SyncDataServices.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ----------------
if (builder.Environment.IsProduction())
{
	Console.WriteLine("--> Using SqlServer Db");
	// Database context - SQL server
	builder.Services.AddDbContext<AppDbContext>(opt =>
		// specify database type and name
		opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"))
	);
}
else
{
	Console.WriteLine("--> Using InMem Db");
	// Database context - In memory
	builder.Services.AddDbContext<AppDbContext>(opt =>
		// specify database type and name
		opt.UseInMemoryDatabase("InMem")
	);
}

// Platform Repo
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

// Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Http client
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

// RabbitMQ
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

// gRPC
builder.Services.AddGrpc();

// ----------------

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

//Console.WriteLine($"--> CommandService Endpoint {Configuration["CommandService"]}");
Console.WriteLine($"--> CommandService Endpoint {app.Configuration.GetConnectionString("CommandService")}");

//app.UseHttpsRedirection();


// gRPC
app.MapGrpcService<GrpcPlatformService>();
// Optional: add endpoint to serve our contract
app.MapGet("/protos/platforms.proto", async context =>
{
	await context.Response.WriteAsync(File.ReadAllText("Protos/platform.proto"));
});


app.UseAuthorization();

app.MapControllers();


// Prep data
PrepDb.PrepPopulation(app, app.Environment.IsProduction());


app.Run();