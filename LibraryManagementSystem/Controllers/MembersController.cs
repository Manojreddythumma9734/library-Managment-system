using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly ICosmosDbService _cosmosDbService;

    public MembersController(ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Member>> GetMemberById(string id)
    {
        var member = await _cosmosDbService.GetItemAsync<Member>(id, "Members");
        if (member == null)
        {
            return NotFound();
        }
        return Ok(member);
    }

    [HttpPost]
    public async Task<ActionResult> AddMember([FromBody] Member member)
    {
        await _cosmosDbService.AddItemAsync(member, "Members");
        return CreatedAtAction(nameof(GetMemberById), new { id = member.UId }, member);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateMember(string id, [FromBody] Member member)
    {
        await _cosmosDbService.UpdateItemAsync(id, member, "Members");
        return NoContent();
    }
}
