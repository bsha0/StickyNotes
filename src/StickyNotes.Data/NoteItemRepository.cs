using Microsoft.EntityFrameworkCore;
using StickyNotes.Core.Data;
using StickyNotes.Core.Entities;

namespace StickyNotes.Data
{
    public sealed class NoteItemRepository : INoteItemRepository
    {
        private readonly NoteItemContext _context;

        public NoteItemRepository(string databasePath)
        {
            var options = new DbContextOptionsBuilder<NoteItemContext>().UseSqlite($"data source={databasePath}").Options;
            //_context = new NoteItemContext(options);
            _context = new NoteItemContext();
        }

        public void Add(NoteItem entity)
        {
            _context.Add(entity);
            Save();
        }

        public void Delete(int id)
        {
            var item = _context.NoteItems.Find(id);
            if (item != null)
            {
                _context.NoteItems.Remove(item);
                Save();
            }
        }

        public IEnumerable<NoteItem> GetAll()
        {
            return _context.NoteItems.ToList();
        }

        public NoteItem GetById(int id)
        {
            return _context.NoteItems.Find(id);
        }

        public void Update(NoteItem entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            Save();
        }

        private void Save()
        {
            _context.SaveChanges();
        }
    }
}
