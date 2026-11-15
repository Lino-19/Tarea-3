using GestorExcursiones.Domain.Entities;using GestorExcursiones.Infrastructure.Context;using GestorExcursiones.Infrastructure.Core;
namespace GestorExcursiones.Infrastructure.Repositories;
public class GuiaRepository(ApplicationDbContext c):BaseRepository<Guia>(c){}
