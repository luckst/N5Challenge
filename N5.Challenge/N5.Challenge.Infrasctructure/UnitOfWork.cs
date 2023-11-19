using Microsoft.EntityFrameworkCore.Storage;
using N5.Challenge.Domain;
using N5.Challenge.Infrasctructure.Repositories;
using N5.Challenge.Infrasctructure.RepositoryPattern;

namespace N5.Challenge.Infrasctructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ServiceDbContext _context;

        public UnitOfWork(ServiceDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }

        public IExecutionStrategy CreateExecutionEstrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public IPermissionRepository GetPermissionRepository()
        {
            return new PermissionRepository(_context);
        }
    }
}
