using GestorExcursiones.Application.Core;
using GestorExcursiones.Application.Dtos;
using GestorExcursiones.Domain.Entities;
using GestorExcursiones.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestorExcursiones.Application.Service;

public class ClienteService(ClienteRepository repository)
{
    private readonly ClienteRepository Repo = repository;

    public async Task<ServiceResult<List<ClienteDto>>> GetAllAsync()
    {
        var items = await Repo.GetAllAsync();
        return ServiceResult<List<ClienteDto>>.Ok(items.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<ClienteDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<ClienteDto>.Fail("El id debe ser mayor que cero.");
        }

        var entity = await Repo.GetByIdAsync(id);
        if (entity is null)
        {
            return ServiceResult<ClienteDto>.Fail($"No se encontró un cliente con el id {id}.");
        }

        return ServiceResult<ClienteDto>.Ok(ToDto(entity));
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
                : ServiceResult.Fail($"No se encontró un cliente con el id {id}.");
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("El registro tiene datos relacionados y no puede eliminarse.");
        }
    }

    public async Task<ServiceResult<ClienteDto>> CreateAsync(CreateClienteDto dto)
    {
        if (dto is null)
        {
            return ServiceResult<ClienteDto>.Fail("Los datos del cliente son obligatorios.");
        }

        var errors = Validar(dto);
        if (errors.Count > 0)
        {
            return ServiceResult<ClienteDto>.Fail(errors.ToArray());
        }

        var documento = ValidationHelper.OnlyDigits(dto.DocumentoIdentificacion);
        if (await Repo.DocumentoExisteAsync(documento))
        {
            return ServiceResult<ClienteDto>.Fail("Ya existe un cliente con ese documento de identificación.");
        }

        var entity = new Cliente
        {
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            DocumentoIdentificacion = documento,
            Telefono = (dto.Telefono ?? string.Empty).Trim(),
            CorreoElectronico = (dto.CorreoElectronico ?? string.Empty).Trim()
        };

        await Repo.AddAsync(entity);
        return ServiceResult<ClienteDto>.Ok(ToDto(entity), "Cliente creado correctamente");
    }

    public async Task<ServiceResult<ClienteDto>> UpdateAsync(UpdateClienteDto dto)
    {
        if (dto is null)
        {
            return ServiceResult<ClienteDto>.Fail("Los datos del cliente son obligatorios.");
        }

        if (dto.Id <= 0)
        {
            return ServiceResult<ClienteDto>.Fail("El id debe ser mayor que cero.");
        }

        var errors = Validar(dto);
        if (errors.Count > 0)
        {
            return ServiceResult<ClienteDto>.Fail(errors.ToArray());
        }

        var entity = await Repo.GetByIdAsync(dto.Id);
        if (entity is null)
        {
            return ServiceResult<ClienteDto>.Fail($"No se encontró un cliente con el id {dto.Id}.");
        }

        var documento = ValidationHelper.OnlyDigits(dto.DocumentoIdentificacion);
        var todos = await Repo.GetAllAsync();
        if (todos.Any(c => c.Id != dto.Id && c.DocumentoIdentificacion == documento))
        {
            return ServiceResult<ClienteDto>.Fail("Ya existe otro cliente con ese documento de identificación.");
        }

        entity.Nombre = dto.Nombre.Trim();
        entity.Apellido = dto.Apellido.Trim();
        entity.DocumentoIdentificacion = documento;
        entity.Telefono = (dto.Telefono ?? string.Empty).Trim();
        entity.CorreoElectronico = (dto.CorreoElectronico ?? string.Empty).Trim();

        await Repo.UpdateAsync(entity);
        return ServiceResult<ClienteDto>.Ok(ToDto(entity), "Cliente actualizado correctamente");
    }

    private ClienteDto ToDto(Cliente entity)
    {
        return new ClienteDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Apellido = entity.Apellido,
            DocumentoIdentificacion = entity.DocumentoIdentificacion,
            Telefono = entity.Telefono,
            CorreoElectronico = entity.CorreoElectronico
        };
    }

    private static List<string> Validar(CreateClienteDto dto)
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

        if (string.IsNullOrWhiteSpace(dto.DocumentoIdentificacion))
        {
            errors.Add("El documento de identificación es obligatorio.");
        }
        else if (ValidationHelper.OnlyDigits(dto.DocumentoIdentificacion).Length != 11)
        {
            errors.Add("El documento de identificación debe tener 11 dígitos.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Telefono) && !ValidationHelper.IsValidPhone(dto.Telefono.Trim()))
        {
            errors.Add("El teléfono no tiene un formato válido.");
        }

        if (!string.IsNullOrWhiteSpace(dto.CorreoElectronico))
        {
            var correo = dto.CorreoElectronico.Trim();
            if (correo.Length > 100)
            {
                errors.Add("El correo electrónico no puede exceder los 100 caracteres.");
            }
            else if (!ValidationHelper.IsValidEmail(correo))
            {
                errors.Add("El correo electrónico no tiene un formato válido.");
            }
        }

        return errors;
    }
}
