using GestorExcursiones.Application.Core;
using GestorExcursiones.Application.Dtos;
using GestorExcursiones.Domain.Entities;
using GestorExcursiones.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestorExcursiones.Application.Service;

public class GuiaService(GuiaRepository repository)
{
    private readonly GuiaRepository Repo = repository;

    public async Task<ServiceResult<List<GuiaDto>>> GetAllAsync()
    {
        var items = await Repo.GetAllAsync();
        return ServiceResult<List<GuiaDto>>.Ok(items.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<GuiaDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<GuiaDto>.Fail("El id debe ser mayor que cero.");
        }

        var entity = await Repo.GetByIdAsync(id);
        if (entity is null)
        {
            return ServiceResult<GuiaDto>.Fail($"No se encontró un guía con el id {id}.");
        }

        return ServiceResult<GuiaDto>.Ok(ToDto(entity));
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
                : ServiceResult.Fail($"No se encontró un guía con el id {id}.");
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("El registro tiene datos relacionados y no puede eliminarse.");
        }
    }

    public async Task<ServiceResult<GuiaDto>> CreateAsync(CreateGuiaDto dto)
    {
        if (dto is null)
        {
            return ServiceResult<GuiaDto>.Fail("Los datos del guía son obligatorios.");
        }

        var errors = Validar(dto);
        if (errors.Count > 0)
        {
            return ServiceResult<GuiaDto>.Fail(errors.ToArray());
        }

        var existentes = await Repo.GetAllAsync();
        if (existentes.Any(g => Duplicado(g, dto)))
        {
            return ServiceResult<GuiaDto>.Fail("Ya existe un guía con el mismo nombre, apellido y teléfono.");
        }

        var entity = new Guia
        {
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            Telefono = (dto.Telefono ?? string.Empty).Trim(),
            Especialidad = dto.Especialidad.Trim()
        };

        await Repo.AddAsync(entity);
        return ServiceResult<GuiaDto>.Ok(ToDto(entity), "Guía creado correctamente");
    }

    public async Task<ServiceResult<GuiaDto>> UpdateAsync(UpdateGuiaDto dto)
    {
        if (dto is null)
        {
            return ServiceResult<GuiaDto>.Fail("Los datos del guía son obligatorios.");
        }

        if (dto.Id <= 0)
        {
            return ServiceResult<GuiaDto>.Fail("El id debe ser mayor que cero.");
        }

        var errors = Validar(dto);
        if (errors.Count > 0)
        {
            return ServiceResult<GuiaDto>.Fail(errors.ToArray());
        }

        var entity = await Repo.GetByIdAsync(dto.Id);
        if (entity is null)
        {
            return ServiceResult<GuiaDto>.Fail($"No se encontró un guía con el id {dto.Id}.");
        }

        var existentes = await Repo.GetAllAsync();
        if (existentes.Any(g => g.Id != dto.Id && Duplicado(g, dto)))
        {
            return ServiceResult<GuiaDto>.Fail("Ya existe otro guía con el mismo nombre, apellido y teléfono.");
        }

        entity.Nombre = dto.Nombre.Trim();
        entity.Apellido = dto.Apellido.Trim();
        entity.Telefono = (dto.Telefono ?? string.Empty).Trim();
        entity.Especialidad = dto.Especialidad.Trim();

        await Repo.UpdateAsync(entity);
        return ServiceResult<GuiaDto>.Ok(ToDto(entity), "Guía actualizado correctamente");
    }

    private GuiaDto ToDto(Guia entity)
    {
        return new GuiaDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Apellido = entity.Apellido,
            Telefono = entity.Telefono,
            Especialidad = entity.Especialidad
        };
    }

    private static bool Duplicado(Guia guia, CreateGuiaDto dto)
    {
        return guia.Nombre.Equals(dto.Nombre.Trim(), StringComparison.OrdinalIgnoreCase) &&
               guia.Apellido.Equals(dto.Apellido.Trim(), StringComparison.OrdinalIgnoreCase) &&
               guia.Telefono == (dto.Telefono ?? string.Empty).Trim();
    }

    private static List<string> Validar(CreateGuiaDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            errors.Add("El nombre es obligatorio.");
        }
        else if (dto.Nombre.Trim().Length < 2 || dto.Nombre.Trim().Length > 50)
        {
            errors.Add("El nombre debe tener entre 2 y 50 caracteres.");
        }
        else if (!ValidationHelper.IsValidName(dto.Nombre.Trim()))
        {
            errors.Add("El nombre solo puede contener letras.");
        }

        if (string.IsNullOrWhiteSpace(dto.Apellido))
        {
            errors.Add("El apellido es obligatorio.");
        }
        else if (dto.Apellido.Trim().Length < 2 || dto.Apellido.Trim().Length > 50)
        {
            errors.Add("El apellido debe tener entre 2 y 50 caracteres.");
        }
        else if (!ValidationHelper.IsValidName(dto.Apellido.Trim()))
        {
            errors.Add("El apellido solo puede contener letras.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Telefono) && !ValidationHelper.IsValidPhone(dto.Telefono.Trim()))
        {
            errors.Add("El teléfono no tiene un formato válido.");
        }

        if (string.IsNullOrWhiteSpace(dto.Especialidad))
        {
            errors.Add("La especialidad es obligatoria.");
        }
        else if (dto.Especialidad.Trim().Length < 3 || dto.Especialidad.Trim().Length > 100)
        {
            errors.Add("La especialidad debe tener entre 3 y 100 caracteres.");
        }

        return errors;
    }
}
