using LibraryAPI;
using LibraryAPI.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<LibraryDb>(opt => opt.UseInMemoryDatabase("books"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var books = app.MapGroup("/books");

books.MapGet("/", GetAllBooks);
books.MapGet("/{id}", GetBook);
books.MapPost("/", CreateBook);
books.MapPut("/{id}", UpdateBook);
books.MapDelete("/{id}", DeleteBook);

app.Run();

static async Task<IResult> GetAllBooks(LibraryDb db)
{
    return TypedResults.Ok(await db.Books.Select(x => new BookDTO(x)).ToListAsync());
}

static async Task<IResult> GetBook(int id, LibraryDb db)
{
    return await db.Books.FindAsync(id)
        is Book book
            ? TypedResults.Ok(book)
            : TypedResults.NotFound();
}

static async Task<IResult> CreateBook(BookDTO bookDTO, LibraryDb db)
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
        db.Books.Add(book);
        await db.SaveChangesAsync();
        bookDTO = new BookDTO(book);
        return TypedResults.Created($"/books/{book.Id}", bookDTO);
    }
}

static async Task<IResult> UpdateBook(int id, BookDTO newBookDTO, LibraryDb db)
{
    var book = await db.Books.FindAsync(id);
    if (book is null || newBookDTO.name == "" || newBookDTO.name == null || newBookDTO.author == "" || newBookDTO.author == null) return TypedResults.NotFound();
    book.name = newBookDTO.name;
    book.author = newBookDTO.author;
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteBook(int id, LibraryDb db)
{
    if (await db.Books.FindAsync(id) is Book book)
    {
        db.Books.Remove(book);
        await db.SaveChangesAsync();
        return TypedResults.Ok(book);
    }
    return TypedResults.NotFound();
}