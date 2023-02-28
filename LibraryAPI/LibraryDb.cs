using LibraryAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI
{
    public class LibraryDb : DbContext
    {
        public LibraryDb(DbContextOptions<LibraryDb> options)
       : base(options) { }

        public DbSet<Book> Books => Set<Book>();
    }
}
