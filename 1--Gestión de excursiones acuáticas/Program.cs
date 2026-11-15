using GestorExcursiones.Infrastructure.Context;using GestorExcursiones.Infrastructure.Repositories;using Microsoft.EntityFrameworkCore;
var builder=WebApplication.CreateBuilder(args);builder.Services.AddControllers();builder.Services.AddEndpointsApiExplorer();builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(o=>o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ClienteRepository>();builder.Services.AddScoped<ExcursionRepository>();builder.Services.AddScoped<GuiaRepository>();builder.Services.AddScoped<ReservaRepository>();
var app=builder.Build();if(app.Environment.IsDevelopment()){app.UseSwagger();app.UseSwaggerUI();}app.UseHttpsRedirection();app.UseAuthorization();app.MapControllers();app.Run();
