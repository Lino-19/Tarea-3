using GestorExcursiones.Domain.Entities;using GestorExcursiones.Infrastructure.Repositories;using GestorExcursiones.API.Models.Dtos;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;
namespace GestorExcursiones.API.Controllers;
[ApiController][Route("api/[controller]")] public class ExcursionsController(ExcursionRepository repo):ControllerBase{
 [HttpGet] public async Task<ActionResult<IEnumerable<ExcursionDto>>> GetAll()=>Ok((await repo.GetAllAsync()).Select(Map));
 [HttpGet("future")] public async Task<ActionResult<IEnumerable<ExcursionDto>>> GetFuture()=>Ok((await repo.GetFuturasAsync()).Select(Map));
 [HttpGet("{id:int}")] public async Task<ActionResult<ExcursionDto>> GetById(int id){var x=await repo.GetByIdAsync(id);return x is null?NotFound():Ok(Map(x));}
 [HttpPost] public async Task<ActionResult<ExcursionDto>> Create(ExcursionCreateDto d){if(d.Precio<0||d.MaxParticipantes<1)return BadRequest("Precio o capacidad inválidos.");var x=new Excursion{Nombre=d.Nombre,Descripcion=d.Descripcion,Fecha=d.Fecha,Precio=d.Precio,MaxParticipantes=d.MaxParticipantes};await repo.AddAsync(x);return CreatedAtAction(nameof(GetById),new{id=x.Id},Map(x));}
 [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,ExcursionCreateDto d){var x=await repo.GetByIdAsync(id);if(x is null)return NotFound();if(d.Precio<0||d.MaxParticipantes<1)return BadRequest("Precio o capacidad inválidos.");x.Nombre=d.Nombre;x.Descripcion=d.Descripcion;x.Fecha=d.Fecha;x.Precio=d.Precio;x.MaxParticipantes=d.MaxParticipantes;await repo.UpdateAsync(x);return NoContent();}
 [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){try{return await repo.DeleteAsync(id)?NoContent():NotFound();}catch(DbUpdateException){return Conflict("La excursión tiene reservas asociadas.");}}
 static ExcursionDto Map(Excursion x)=>new(){Id=x.Id,Nombre=x.Nombre,Descripcion=x.Descripcion,Fecha=x.Fecha,Precio=x.Precio,MaxParticipantes=x.MaxParticipantes};}
