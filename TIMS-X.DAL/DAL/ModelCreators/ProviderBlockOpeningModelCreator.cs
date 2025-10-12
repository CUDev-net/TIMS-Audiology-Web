using Microsoft.EntityFrameworkCore;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class ProviderBlockOpeningModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderBlockOpening>(entity => { });
    }
}