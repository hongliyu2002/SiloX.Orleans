using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Vending.Domain.EntityFrameworkCore;

public class DomainDbContextDesignFactory : IDesignTimeDbContextFactory<DomainDbContext>
{
    /// <inheritdoc />
    public DomainDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=123.60.184.85;Integrated Security=False;User Id=sa;Password=Bosshong2010;TrustServerCertificate=True;Database=Vending-Domain";
        var optionsBuilder = new DbContextOptionsBuilder<DomainDbContext>().UseSqlServer(connectionString);
        return new DomainDbContext(optionsBuilder.Options);
    }
}
