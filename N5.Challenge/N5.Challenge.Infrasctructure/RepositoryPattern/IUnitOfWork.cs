﻿using Microsoft.EntityFrameworkCore.Storage;
using N5.Challenge.Infrasctructure.Repositories;

namespace N5.Challenge.Infrasctructure.RepositoryPattern
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        Task RollbackAsync();
        Task CommitTransactionAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        IExecutionStrategy CreateExecutionEstrategy();
        IPermissionRepository GetPermissionRepository();
    }
}
