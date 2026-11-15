using GestorExcursiones.Application.Core;
using GestorExcursiones.Application.Dtos;
using GestorExcursiones.Domain.Entities;
using GestorExcursiones.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestorExcursiones.Application.Service;

public class ReservaService(
    ReservaRepository repository,
    ClienteRepository clienteRepository,
    ExcursionRepository excursionRepository)
{
    private const string Cancelada = "Cancelada";
    private static readonly string[] Estados = { "Pendiente", "Confirmada", Cancelada };

    private readonly ReservaRepository Repo = repository;

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
                : ServiceResult.Fail($"No se encontró una reserva con el id {id}.");
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("El registro tiene datos relacionados y no puede eliminarse.");
        }
    }

    public async Task<ServiceResult<List<ReservaDto>>> GetAllAsync()
    {
        var items = await Repo.GetConDetallesAsync();
        return ServiceResult<List<ReservaDto>>.Ok(items.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<ReservaDto>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<ReservaDto>.Fail("El id debe ser mayor que cero.");
        }

        var entity = await Repo.GetConDetalleAsync(id);
        if (entity is null)
        {
            return ServiceResult<ReservaDto>.Fail($"No se encontró una reserva con el id {id}.");
        }

        return ServiceResult<ReservaDto>.Ok(ToDto(entity));
    }

    public async Task<ServiceResult<ReservaDto>> CreateAsync(CreateReservaDto dto)
    {
        if (dto is null)
        {
            return ServiceResult<ReservaDto>.Fail("Los datos de la reserva son obligatorios.");
        }

        var errors = new List<string>();

        if (dto.ClienteId <= 0)
        {
            errors.Add("Debe indicar un cliente válido.");
        }

        if (dto.ExcursionId <= 0)
        {
            errors.Add("Debe indicar una excursión válida.");
        }

        ValidarMontosYCantidad(dto.CantidadParticipantes, dto.MontoPagado, errors);
        if (errors.Count > 0)
        {
            return ServiceResult<ReservaDto>.Fail(errors.ToArray());
        }

        var cliente = await clienteRepository.GetByIdAsync(dto.ClienteId);
        if (cliente is null)
        {
            return ServiceResult<ReservaDto>.Fail($"No se encontró un cliente con el id {dto.ClienteId}.");
        }

        var excursion = await excursionRepository.GetByIdAsync(dto.ExcursionId);
        if (excursion is null)
        {
            return ServiceResult<ReservaDto>.Fail($"No se encontró una excursión con el id {dto.ExcursionId}.");
        }

        if (excursion.Fecha.Date < DateTime.Today)
        {
            return ServiceResult<ReservaDto>.Fail("No se puede reservar una excursión que ya ocurrió.");
        }

        var disponibles = await CalcularDisponibles(excursion, null);
        if (dto.CantidadParticipantes > disponibles)
        {
            return ServiceResult<ReservaDto>.Fail($"No hay cupo suficiente. Cupos disponibles: {disponibles}.");
        }

        var total = excursion.Precio * dto.CantidadParticipantes;
        if (dto.MontoPagado > total)
        {
            return ServiceResult<ReservaDto>.Fail("El monto pagado no puede ser mayor que el monto total.");
        }

        var entity = new Reserva
        {
            ClienteId = dto.ClienteId,
            ExcursionId = dto.ExcursionId,
            FechaReserva = DateTime.Now,
            CantidadParticipantes = dto.CantidadParticipantes,
            Estado = "Pendiente",
            MontoTotal = total,
            MontoPagado = dto.MontoPagado
        };

        await Repo.AddAsync(entity);

        var creada = await Repo.GetConDetalleAsync(entity.Id);
        return ServiceResult<ReservaDto>.Ok(ToDto(creada ?? entity), "Reserva creada correctamente");
    }

    public async Task<ServiceResult<ReservaDto>> UpdateAsync(UpdateReservaDto dto)
    {
        if (dto is null)
        {
            return ServiceResult<ReservaDto>.Fail("Los datos de la reserva son obligatorios.");
        }

        var errors = new List<string>();

        if (dto.Id <= 0)
        {
            errors.Add("El id debe ser mayor que cero.");
        }

        var estado = Estados.FirstOrDefault(e => e.Equals(dto.Estado?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (estado is null)
        {
            errors.Add("El estado debe ser Pendiente, Confirmada o Cancelada.");
        }

        ValidarMontosYCantidad(dto.CantidadParticipantes, dto.MontoPagado, errors);
        if (errors.Count > 0)
        {
            return ServiceResult<ReservaDto>.Fail(errors.ToArray());
        }

        var entity = await Repo.GetByIdAsync(dto.Id);
        if (entity is null)
        {
            return ServiceResult<ReservaDto>.Fail($"No se encontró una reserva con el id {dto.Id}.");
        }

        var excursion = await excursionRepository.GetByIdAsync(entity.ExcursionId);
        if (excursion is null)
        {
            return ServiceResult<ReservaDto>.Fail("La excursión asociada a la reserva ya no existe.");
        }

        if (estado != Cancelada)
        {
            var disponibles = await CalcularDisponibles(excursion, entity.Id);
            if (dto.CantidadParticipantes > disponibles)
            {
                return ServiceResult<ReservaDto>.Fail($"No hay cupo suficiente. Cupos disponibles: {disponibles}.");
            }
        }

        var total = excursion.Precio * dto.CantidadParticipantes;
        if (dto.MontoPagado > total)
        {
            return ServiceResult<ReservaDto>.Fail("El monto pagado no puede ser mayor que el monto total.");
        }

        entity.CantidadParticipantes = dto.CantidadParticipantes;
        entity.Estado = estado!;
        entity.MontoTotal = total;
        entity.MontoPagado = dto.MontoPagado;

        await Repo.UpdateAsync(entity);

        var actualizada = await Repo.GetConDetalleAsync(entity.Id);
        return ServiceResult<ReservaDto>.Ok(ToDto(actualizada ?? entity), "Reserva actualizada correctamente");
    }

    private ReservaDto ToDto(Reserva entity)
    {
        return new ReservaDto
        {
            Id = entity.Id,
            ClienteId = entity.ClienteId,
            ClienteNombre = entity.Cliente is null ? string.Empty : $"{entity.Cliente.Nombre} {entity.Cliente.Apellido}",
            ExcursionId = entity.ExcursionId,
            ExcursionNombre = entity.Excursion?.Nombre ?? string.Empty,
            FechaReserva = entity.FechaReserva,
            CantidadParticipantes = entity.CantidadParticipantes,
            Estado = entity.Estado,
            MontoTotal = entity.MontoTotal,
            MontoPagado = entity.MontoPagado,
            SaldoPendiente = entity.SaldoPendiente,
            EstadoPago = entity.EstadoPago
        };
    }

    private async Task<int> CalcularDisponibles(Excursion excursion, int? reservaIgnorada)
    {
        var reservas = await Repo.GetAllAsync();
        var ocupados = reservas
            .Where(r => r.ExcursionId == excursion.Id && r.Estado != Cancelada && r.Id != reservaIgnorada)
            .Sum(r => r.CantidadParticipantes);

        return excursion.MaxParticipantes - ocupados;
    }

    private static void ValidarMontosYCantidad(int cantidad, decimal montoPagado, List<string> errors)
    {
        if (cantidad < 1)
        {
            errors.Add("La cantidad de participantes debe ser al menos 1.");
        }
        else if (cantidad > 500)
        {
            errors.Add("La cantidad de participantes no puede exceder 500.");
        }

        if (montoPagado < 0)
        {
            errors.Add("El monto pagado no puede ser negativo.");
        }
    }
}
