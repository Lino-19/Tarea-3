using GestorExcursiones.Application.Core;
using GestorExcursiones.Application.Dtos;
using GestorExcursiones.Domain.Entities;
using GestorExcursiones.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestorExcursiones.Application.Service;

public class ExcursionService(ExcursionRepository repository)
{
    private readonly ExcursionRepository Repo = repository;

    public async Task<ServiceResult<List<ExcursionDto>>> GetAllAsync()
    {
        var items = await Repo.GetAllAsync();
        return ServiceResult<List<ExcursionDto>>.Ok(items.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<ExcursionDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<ExcursionDto>.Fail("El id debe ser mayor que cero.");
        }

        var entity = await Repo.GetByIdAsync(id);
        if (entity is null)
        {
            return ServiceResult<ExcursionDto>.Fail($"No se encontró una excursión con el id {id}.");
        }

        return ServiceResult<ExcursionDto>.Ok(ToDto(entity));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            return ServiceResult.Fail("El id debe ser mayor que cero.");
        }

        try
        {
            var deleted = await Repo.DeleteAsync(id);
            return deleted
                ? ServiceResult.Ok("Registro eliminado correctamente")
                : ServiceResult.Fail($"No se encontró una excursión con el id {id}.");
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("El registro tiene datos relacionados y no puede eliminarse.");
        }
    }

    public async Task<ServiceResult<ExcursionDto>> CreateAsync(CreateExcursionDto dto)
    {
        if (dto is null)
        {
            return ServiceResult<ExcursionDto>.Fail("Los datos de la excursión son obligatorios.");
        }

        var errors = Validar(dto, true);
        if (errors.Count > 0)
        {
            return ServiceResult<ExcursionDto>.Fail(errors.ToArray());
        }

        var existentes = await Repo.GetAllAsync();
        var duplicada = existentes.Any(e =>
            e.Nombre.Equals(dto.Nombre.Trim(), StringComparison.OrdinalIgnoreCase) &&
            e.Fecha.Date == dto.Fecha.Date);

        if (duplicada)
        {
            return ServiceResult<ExcursionDto>.Fail("Ya existe una excursión con el mismo nombre y fecha.");
        }

        var entity = new Excursion
        {
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion.Trim(),
            Fecha = dto.Fecha,
            Precio = dto.Precio,
            MaxParticipantes = dto.MaxParticipantes
        };

        await Repo.AddAsync(entity);
        return ServiceResult<ExcursionDto>.Ok(ToDto(entity), "Excursión creada correctamente");
    }

    public async Task<ServiceResult<ExcursionDto>> UpdateAsync(UpdateExcursionDto dto)
    {
        if (dto is null)
        {
            return ServiceResult<ExcursionDto>.Fail("Los datos de la excursión son obligatorios.");
        }

        if (dto.Id <= 0)
        {
            return ServiceResult<ExcursionDto>.Fail("El id debe ser mayor que cero.");
        }

        var entity = await Repo.GetByIdAsync(dto.Id);
        if (entity is null)
        {
            return ServiceResult<ExcursionDto>.Fail($"No se encontró una excursión con el id {dto.Id}.");
        }

        var errors = Validar(dto, entity.Fecha.Date != dto.Fecha.Date);
        if (errors.Count > 0)
        {
            return ServiceResult<ExcursionDto>.Fail(errors.ToArray());
        }

        var existentes = await Repo.GetAllAsync();
        var duplicada = existentes.Any(e =>
            e.Id != dto.Id &&
            e.Nombre.Equals(dto.Nombre.Trim(), StringComparison.OrdinalIgnoreCase) &&
            e.Fecha.Date == dto.Fecha.Date);

        if (duplicada)
        {
            return ServiceResult<ExcursionDto>.Fail("Ya existe otra excursión con el mismo nombre y fecha.");
        }

        entity.Nombre = dto.Nombre.Trim();
        entity.Descripcion = dto.Descripcion.Trim();
        entity.Fecha = dto.Fecha;
        entity.Precio = dto.Precio;
        entity.MaxParticipantes = dto.MaxParticipantes;

        await Repo.UpdateAsync(entity);
        return ServiceResult<ExcursionDto>.Ok(ToDto(entity), "Excursión actualizada correctamente");
    }

    public async Task<ServiceResult<List<ExcursionDto>>> GetFuturasAsync()
    {
        var items = await Repo.GetFuturasAsync();
        return ServiceResult<List<ExcursionDto>>.Ok(items.Select(ToDto).ToList());
    }

    private ExcursionDto ToDto(Excursion entity)
    {
        return new ExcursionDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            Fecha = entity.Fecha,
            Precio = entity.Precio,
            MaxParticipantes = entity.MaxParticipantes
        };
    }

    private static List<string> Validar(CreateExcursionDto dto, bool exigirFechaFutura)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            errors.Add("El nombre es obligatorio.");
        }
        else if (dto.Nombre.Trim().Length < 3 || dto.Nombre.Trim().Length > 100)
        {
            errors.Add("El nombre debe tener entre 3 y 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(dto.Descripcion))
        {
            errors.Add("La descripción es obligatoria.");
        }
        else if (dto.Descripcion.Trim().Length > 500)
        {
            errors.Add("La descripción no puede exceder los 500 caracteres.");
        }

        if (dto.Fecha == default)
        {
            errors.Add("La fecha es obligatoria.");
        }
        else if (exigirFechaFutura && dto.Fecha.Date < DateTime.Today)
        {
            errors.Add("La fecha no puede estar en el pasado.");
        }

        if (dto.Precio <= 0)
        {
            errors.Add("El precio debe ser mayor que cero.");
        }
        else if (dto.Precio > 1000000)
        {
            errors.Add("El precio no puede exceder 1,000,000.");
        }

        if (dto.MaxParticipantes < 1 || dto.MaxParticipantes > 500)
        {
            errors.Add("El máximo de participantes debe estar entre 1 y 500.");
        }

        return errors;
    }
}
