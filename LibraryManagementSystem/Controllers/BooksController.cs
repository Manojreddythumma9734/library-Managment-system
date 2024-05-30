using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly ICosmosDbService _cosmosDbService;

    public BooksController(ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBookById(string id)
    {
        var book = await _cosmosDbService.GetItemAsync<Book>(id, "Books");
        if (book == null)
        {
            return NotFound();
        }
        return Ok(book);
    }

    [HttpGet("title/{title}")]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooksByTitle(string title)
    {
        var books = await _cosmosDbService.GetItemsAsync<Book>($"SELECT * FROM c WHERE c.Title = '{title}'", "Books");
        return Ok(books);
    }

    [HttpPost]
    public async Task<ActionResult> AddBook([FromBody] Book book)
    {
        await _cosmosDbService.AddItemAsync(book, "Books");
        return CreatedAtAction(nameof(GetBookById), new { id = book.UId }, book);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateBook(string id, [FromBody] Book book)
    {
        await _cosmosDbService.UpdateItemAsync(id, book, "Books");
        return NoContent();
    }
}
