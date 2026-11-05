using EnglishTutor.Application.Interfaces.Repositories;
using EnglishTutor.Infrastructure.Persistence;
using System.Collections.Concurrent;

namespace EnglishTutor.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TutorDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public UnitOfWork(TutorDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            return (IGenericRepository<T>)_repositories.GetOrAdd(
                typeof(T),
                _ => new GenericRepository<T>(_context));
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}
