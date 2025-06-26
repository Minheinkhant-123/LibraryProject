namespace LibraryProject.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public int PublicationYear { get; set; }
        public string Publisher { get; set; }
        public int AvailableCopies { get; set; }

        public List<BookAuthor> BookAuthors { get; set; }
        public List<BookGenre> BookGenres { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BookAuthor> BookAuthors { get; set; }
    }

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BookGenre> BookGenres { get; set; }
    }

    public class BookAuthor
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }

    public class BookGenre
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
