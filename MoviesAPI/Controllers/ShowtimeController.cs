using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Auth;
using MoviesAPI.DTOs.Requests;
using MoviesAPI.DTOs.Responses;
using MoviesAPI.Requests;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowtimeController(ISender sender) : Controller
{	
	[HttpGet]
	//[Authorize(Roles = Constants.Roles.Read)]
	public async Task<ActionResult<IEnumerable<ShowtimeResponse>>> GetAsync([FromQuery] DateTime? date, [FromQuery] string? movieTitle)
	{
		var entities = await sender.Send(new GetShowtimeRequest(date, movieTitle), CancellationToken.None);
		return Ok(entities);
	}

	[HttpGet("{id}")]
	//[Authorize(Roles = Constants.Roles.Read)]
	public async Task<ActionResult<ShowtimeResponse>> GetAsync([FromRoute] int id)
	{
		var showtime = await sender.Send(new GetShowtimeByIdRequest(id), CancellationToken.None);
		return Ok(showtime);
	}

	[HttpPost]
	//[Authorize(Roles = Constants.Roles.Write)]
	public async Task<ActionResult<ShowtimeResponse>> PostAsync([FromBody] ShowtimeRequest showtime)
	{
		var result = await sender.Send(new CreateShowtimeRequest(showtime), CancellationToken.None);
		return Ok(result);
	}

	[HttpPut("{id}")]
	//[Authorize(Roles = Constants.Roles.Write)]
	public async Task<ActionResult<ShowtimeResponse>> Put([FromRoute] int id, [FromBody] ShowtimeRequest showtime)
	{
		var existing = await sender.Send(new GetShowtimeByIdRequest(id), CancellationToken.None);
		if (existing is null)
		{
			return NotFound($"Not found Showtime with provided id");
		}

		var result = await sender.Send(new UpdateShowtimeRequest(id, showtime), CancellationToken.None);
		return Ok(result);
	}

	[HttpDelete("{id}")]
	//[Authorize(Roles = Constants.Roles.Write)]
	public async Task<ActionResult> Delete([FromRoute] int id)
	{
		var existing = await sender.Send(new GetShowtimeByIdRequest(id), CancellationToken.None);
		if (existing is null)
		{
			return NotFound($"Not found Showtime with provided id");
		}

		await sender.Send(new DeleteShowtimeRequest(id), CancellationToken.None);
		return Ok();
	}

	[HttpPatch]
	//[Authorize(Roles = Constants.Roles.Write)]
	public ActionResult Patch()
	{
		return StatusCode(StatusCodes.Status500InternalServerError, "Test error handler");
	}
}
