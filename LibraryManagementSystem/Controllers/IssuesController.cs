using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly ICosmosDbService _cosmosDbService;

    public IssuesController(ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Issue>> GetIssueById(string id)
    {
        var issue = await _cosmosDbService.GetItemAsync<Issue>(id, "Issues");
        if (issue == null)
        {
            return NotFound();
        }
        return Ok(issue);
    }

    [HttpPost]
    public async Task<ActionResult> AddIssue([FromBody] Issue issue)
    {
        await _cosmosDbService.AddItemAsync(issue, "Issues");
        return CreatedAtAction(nameof(GetIssueById), new { id = issue.UId }, issue);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateIssue(string id, [FromBody] Issue issue)
    {
        await _cosmosDbService.UpdateItemAsync(id, issue, "Issues");
        return NoContent();
    }
}
