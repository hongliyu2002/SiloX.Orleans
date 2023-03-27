using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Vending.Projection.EntityFrameworkCore;

public class ProjectionDbContextDesignFactory : IDesignTimeDbContextFactory<ProjectionDbContext>
{
    /// <inheritdoc />
    public ProjectionDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=123.60.184.85;Integrated Security=False;User Id=sa;Password=Bosshong2010;TrustServerCertificate=True;Database=Vending";
        var optionsBuilder = new DbContextOptionsBuilder<ProjectionDbContext>().UseSqlServer(connectionString);
        return new ProjectionDbContext(optionsBuilder.Options);
    }
}
