using Microsoft.EntityFrameworkCore;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class HoursOfOperationModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProvidersUnitOfWork.HoursOfOperation>(entity => { });
    }
}