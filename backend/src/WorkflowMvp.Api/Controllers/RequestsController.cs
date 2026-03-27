using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowMvp.Api.Auth;
using WorkflowMvp.Api.Models;
using WorkflowMvp.Api.Services;

namespace WorkflowMvp.Api.Controllers;

[ApiController]
[Route("api/requests")]
public class RequestsController(IRequestService requestService, ICurrentUserAccessor currentUserAccessor) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RequestDetailDto>> Create([FromBody] CreateRequestDto dto, CancellationToken cancellationToken)
    {
        var actor = currentUserAccessor.GetActor(User);
        var result = await requestService.CreateAsync(dto, actor, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyCollection<RequestListItemDto>>> List(CancellationToken cancellationToken)
        => Ok(await requestService.ListAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<RequestDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await requestService.GetAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "manager,finance")]
    public async Task<ActionResult<RequestDetailDto>> Approve(Guid id, [FromBody] ActionRequestDto dto, CancellationToken cancellationToken)
    {
        var actor = currentUserAccessor.GetActor(User);
        var role = currentUserAccessor.GetRole(User);

        try
        {
            var result = await requestService.ApproveAsync(id, actor, role, dto.Comment, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "manager,finance")]
    public async Task<ActionResult<RequestDetailDto>> Reject(Guid id, [FromBody] ActionRequestDto dto, CancellationToken cancellationToken)
    {
        var actor = currentUserAccessor.GetActor(User);
        var role = currentUserAccessor.GetRole(User);

        try
        {
            var result = await requestService.RejectAsync(id, actor, role, dto.Comment, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/summary")]
    [Authorize]
    public async Task<ActionResult<RequestSummaryDto>> Summary(Guid id, CancellationToken cancellationToken)
    {
        var result = await requestService.GetSummaryAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
