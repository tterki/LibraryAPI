using LibraryAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryAPI
{
    public class BookService
    {
        public async Task<IResult> GetAllBooks(LibraryDb db)
        {
            return TypedResults.Ok(await db.Books.Select(x => new BookDTO(x)).ToListAsync());
        }

        public async Task<IResult> GetBookById(int id, LibraryDb db)
        {
            return await db.Books.FindAsync(id)
            is Book book
                ? TypedResults.Ok(book)
                : TypedResults.NotFound();
        }

        public async Task<IResult> CreateBook(BookDTO bookDTO, LibraryDb db)
        {
            if (bookDTO == null || bookDTO.name == "" || bookDTO.name == null || bookDTO.author == "" || bookDTO.author == null)
            {
                return TypedResults.NoContent();
            }
            else
            {
                var book = new Book
                {
                    name = bookDTO.name,
                    author = bookDTO.author
                };

                // Vérifier s'il le livre du même auteur existe déjà
                foreach (Book b in db.Books)
                {
                    if (b.name == bookDTO.name && b.author == bookDTO.author) return TypedResults.NoContent();
                }

                db.Books.Add(book);
                await db.SaveChangesAsync();
                bookDTO = new BookDTO(book);
                return TypedResults.Created($"/books/{book.Id}", bookDTO);
            }
        }

        public async Task<IResult> UpdateBook(int id, BookDTO newBookDTO, LibraryDb db)
        {
            var book = await db.Books.FindAsync(id);
            if (book is null || newBookDTO.name == "" || newBookDTO.name == null || newBookDTO.author == "" || newBookDTO.author == null) return TypedResults.NotFound();
            book.name = newBookDTO.name;
            book.author = newBookDTO.author;
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        public async Task<IResult> DeleteBook(int id, LibraryDb db)
        {
            if (await db.Books.FindAsync(id) is Book book)
            {
                db.Books.Remove(book);
                await db.SaveChangesAsync();
                return TypedResults.Ok(book);
            }
            return TypedResults.NotFound();
        }
    }
}
