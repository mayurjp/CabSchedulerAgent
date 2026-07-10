using Microsoft.AspNetCore.Mvc;
using CabScheduler.Api.Agents;

namespace CabScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly EmployeeAgent _employeeAgent;

    public ChatController(EmployeeAgent employeeAgent)
    {
        _employeeAgent = employeeAgent;
    }

    [HttpPost("employee/message")]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
    {
        var reply = await _employeeAgent.ProcessChatMessageAsync(request.EmployeeId, request.Message);
        return Ok(new { reply });
    }
}

public class ChatRequest
{
    public int EmployeeId { get; set; }
    public string Message { get; set; } = string.Empty;
}
