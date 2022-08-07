global using Microsoft.EntityFrameworkCore;

namespace Minimal_API
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext>options) : base(options)
        {

        }

        public DbSet<Person> Persons => Set<Person>();
    }
}
