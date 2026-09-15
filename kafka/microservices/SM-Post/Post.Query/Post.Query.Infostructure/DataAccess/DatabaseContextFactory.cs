using Microsoft.EntityFrameworkCore;

namespace Post.Query.Infostructure.DataAccess;

public class DatabaseContextFactory
{
    private readonly Action<DbContextOptionsBuilder> _configreDbContextOptions;

    public DatabaseContextFactory(Action<DbContextOptionsBuilder> configreDbContextOptions)
    {
        _configreDbContextOptions = configreDbContextOptions;
    }

    public DatabaseContext CreateDbContext()
    {
        DbContextOptionsBuilder<DatabaseContext> optionsBuilder = new();
        _configreDbContextOptions(optionsBuilder);
        return new DatabaseContext(optionsBuilder.Options);
    }
}