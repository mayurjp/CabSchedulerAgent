using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
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
        var session = await _employeeAgent.CreateSessionAsync();
        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, $"employee {request.EmployeeId} says: {request.Message}")
        };

        var response = await _employeeAgent.RunAsync(messages, session);

        var reply = response.Messages.Any()
            ? response.Messages.Last().Text ?? "I didn't understand that."
            : "I didn't understand that.";

        return Ok(new { reply });
    }
}

public class ChatRequest
{
    public int EmployeeId { get; set; }
    public string Message { get; set; } = string.Empty;
}
