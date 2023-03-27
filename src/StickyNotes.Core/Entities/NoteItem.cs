namespace StickyNotes.Core.Entities
{
    public class NoteItem
    {
        public int Id { get; set; }
        public DateTime LastModificationTime { get; set; }
        public string? Content { get; set; }
    }
}