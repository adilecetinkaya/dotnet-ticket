using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TicketResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TicketResponseDto>>> GetAll(
        [FromQuery] TicketQueryParameters parameters,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetTicketsQuery(parameters), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponseDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var ticket = await _mediator.Send(new GetTicketByIdQuery(id), cancellationToken);

        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketResponseDto>> Create(
        [FromBody] CreateTicketCommand command,
        CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TicketResponseDto>> Update(
        Guid id,
        [FromBody] UpdateTicketRequest body,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTicketCommand(
            id, body.Subject, body.Description, body.Status, body.Priority);

        var updated = await _mediator.Send(command, cancellationToken);

        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteTicketCommand(id), cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}

public record UpdateTicketRequest(
    string Subject,
    string Description,
    TicketStatus Status,
    TicketPriority Priority);
