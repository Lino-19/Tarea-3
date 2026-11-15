using GestorExcursiones.Application.Dtos;
using GestorExcursiones.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace GestorExcursiones.API.Controllers;

[Route("api/[controller]")]
public class ExcursionsController(ExcursionService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Respond(await service.GetAllAsync());
    }

    [HttpGet("future")]
    public async Task<IActionResult> GetFuture()
    {
        return Respond(await service.GetFuturasAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Respond(await service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExcursionDto dto)
    {
        return Respond(await service.CreateAsync(dto), true);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateExcursionDto dto)
    {
        dto.Id = id;
        return Respond(await service.UpdateAsync(dto));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return Respond(await service.DeleteAsync(id));
    }
}
