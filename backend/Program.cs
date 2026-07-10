using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Data;
using CabScheduler.Api.Agents;
using CabScheduler.Api.Services;
using CabScheduler.Api.EventBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<CabSchedulerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IEventBus, StubEventBus>();
builder.Services.AddScoped<EmployeeAgent>();
builder.Services.AddScoped<SupervisorAgent>();
builder.Services.AddScoped<SchedulingEngine>();
builder.Services.AddScoped<SchedulerAgent>();
builder.Services.AddScoped<AdaptiveSchedulerAgent>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var adaptiveAgent = app.Services.GetRequiredService<AdaptiveSchedulerAgent>();
await adaptiveAgent.InitializeAsync();

app.UseCors("AllowAngular");
app.MapControllers();

app.Run();
