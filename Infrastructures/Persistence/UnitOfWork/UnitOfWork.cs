using Application.Interfaces.UnitOfWork;
using Infrastructures.Persistence.Context;

namespace Infrastructures.Persistence.UnitOfWork
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}