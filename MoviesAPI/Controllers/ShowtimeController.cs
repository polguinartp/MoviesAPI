using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Auth;
using MoviesAPI.DTOs.API.Requests;
using MoviesAPI.DTOs.API.Responses;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowtimeController(IShowtimeService service, IMapper mapper) : Controller
{	
	[HttpGet]
	//[Authorize(Roles = Constants.Roles.Read)]
	public async Task<ActionResult<IEnumerable<ShowtimeResponse>>> GetAsync([FromQuery] DateTime? date, [FromQuery] string movieTitle)
	{
		var entities = await service.GetAsync(date, movieTitle);
		var result = entities;
		return Ok(result);
	}

	[HttpGet("{id}")]
	//[Authorize(Roles = Constants.Roles.Read)]
	public async Task<ActionResult<ShowtimeResponse>> GetAsync([FromRoute] int id)
	{
		var showtime = await service.GetByIdAsync(id);
		return Ok(showtime);
	}

	[HttpPost]
	//[Authorize(Roles = Constants.Roles.Write)]
	public async Task<ActionResult<ShowtimeResponse>> PostAsync([FromBody] ShowtimeRequest showtime)
	{
		var result = await service.CreateAsync(showtime);
		return Ok(result);
	}

	[HttpPut]
	//[Authorize(Roles = Constants.Roles.Write)]
	public async Task<ActionResult<ShowtimeResponse>> Put([FromBody] ShowtimeRequest showtime)
	{
		if (await service.GetByIdAsync(showtime.Id) is null)
		{
			return NotFound($"Not found Showtime with provided id");
		}

		var result = await service.UpdateAsync(showtime);
		return Ok(result);
	}

	[HttpDelete("{id}")]
	//[Authorize(Roles = Constants.Roles.Write)]
	public async Task<ActionResult> Delete([FromRoute] int id)
	{
		if (await service.GetByIdAsync(id) is null)
		{
			return NotFound($"Not found Showtime with provided id");
		}

		await service.DeleteAsync(id);
		return Ok();
	}

	[HttpPatch]
	//[Authorize(Roles = Constants.Roles.Write)]
	public ActionResult Patch()
	{
		return StatusCode(StatusCodes.Status500InternalServerError, "Test error handler");
	}
}
