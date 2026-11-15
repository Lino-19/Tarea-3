using GestorExcursiones.Domain.Entities;using GestorExcursiones.Infrastructure.Repositories;using Microsoft.AspNetCore.Mvc;
namespace GestorExcursiones.API.Controllers;
[ApiController][Route("api/[controller]")] public class GuidesController(GuiaRepository repo):ControllerBase{
 [HttpGet] public async Task<ActionResult<IEnumerable<Guia>>> GetAll()=>Ok(await repo.GetAllAsync());
 [HttpGet("{id:int}")] public async Task<ActionResult<Guia>> GetById(int id){var x=await repo.GetByIdAsync(id);return x is null?NotFound():Ok(x);}
 [HttpPost] public async Task<ActionResult<Guia>> Create(Guia x){x.Id=0;await repo.AddAsync(x);return CreatedAtAction(nameof(GetById),new{id=x.Id},x);}
 [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,Guia d){var x=await repo.GetByIdAsync(id);if(x is null)return NotFound();x.Nombre=d.Nombre;x.Apellido=d.Apellido;x.Telefono=d.Telefono;x.Especialidad=d.Especialidad;await repo.UpdateAsync(x);return NoContent();}
 [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id)=>await repo.DeleteAsync(id)?NoContent():NotFound();}
