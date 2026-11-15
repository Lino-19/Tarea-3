using GestorExcursiones.Application.Dtos;
using GestorExcursiones.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace GestorExcursiones.API.Controllers;

[Route("api/[controller]")]
public class ClientsController(ClienteService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Respond(await service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Respond(await service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClienteDto dto)
    {
        return Respond(await service.CreateAsync(dto), true);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateClienteDto dto)
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
