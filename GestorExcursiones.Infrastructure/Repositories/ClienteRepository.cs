using GestorExcursiones.Domain.Entities;using GestorExcursiones.Infrastructure.Context;using GestorExcursiones.Infrastructure.Core;using Microsoft.EntityFrameworkCore;
namespace GestorExcursiones.Infrastructure.Repositories;
public class ClienteRepository(ApplicationDbContext c):BaseRepository<Cliente>(c){public Task<bool> DocumentoExisteAsync(string d)=>_context.Clientes.AnyAsync(x=>x.DocumentoIdentificacion==d);}
