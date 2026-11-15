using GestorExcursiones.Application.Service;
using GestorExcursiones.Infrastructure.Context;
using GestorExcursiones.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<ExcursionRepository>();
builder.Services.AddScoped<GuiaRepository>();
builder.Services.AddScoped<ReservaRepository>();

builder.Services.AddScoped<ExcursionService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<GuiaService>();
builder.Services.AddScoped<ReservaService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
