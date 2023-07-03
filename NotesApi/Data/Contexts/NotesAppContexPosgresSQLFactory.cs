using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NotesApi.Data.Contexts;

public class NotesAppContexPosgresSQLFactory : IDesignTimeDbContextFactory<NotesAppContextPostgreSQL>
{
    public NotesAppContextPostgreSQL CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotesAppContextPostgreSQL>();

        optionsBuilder.UseNpgsql(args[0]);

        return new NotesAppContextPostgreSQL(optionsBuilder.Options);
    }
}