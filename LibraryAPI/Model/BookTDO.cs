namespace LibraryAPI.Model
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string? name { get; set; }
        public string? author { get; set; }

        public BookDTO() { }
        public BookDTO(Book book)
        {
            (Id, name, author) = (book.Id, book.name, book.author);
        }
    }
}
