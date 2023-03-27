using Microsoft.EntityFrameworkCore;
using StickyNotes.Core.Entities;

namespace StickyNotes.Data
{
    public sealed class NoteItemContext : DbContext
    {
        private readonly string _dbPath;
        public DbSet<NoteItem> NoteItems { get; set; }

        //public NoteItemContext(DbContextOptions<NoteItemContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // options.UseSqlite($"Data Source={_dbPath}");
            //var folder = Environment.SpecialFolder.LocalApplicationData;
            //var path = Environment.GetFolderPath(folder);
            //var dbPath = Path.Join(path, "note.db3");
            var dbPath = "note.db3";
            options.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NoteItem>().HasKey(m => m.Id);
        }
    }
}