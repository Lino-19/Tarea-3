using GestorExcursiones.Domain.Entities;using GestorExcursiones.Infrastructure.Context;using GestorExcursiones.Infrastructure.Core;using Microsoft.EntityFrameworkCore;
namespace GestorExcursiones.Infrastructure.Repositories;
public class ExcursionRepository(ApplicationDbContext c):BaseRepository<Excursion>(c){public Task<List<Excursion>> GetFuturasAsync()=>_context.Excursiones.AsNoTracking().Where(x=>x.Fecha>=DateTime.Today).OrderBy(x=>x.Fecha).ToListAsync();}
