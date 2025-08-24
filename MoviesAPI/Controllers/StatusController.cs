using Infrastructure.SQS.Factories;
using Infrastructure.SQS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Background;
using MoviesAPI.DTOs.API;
using MoviesAPI.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static MoviesAPI.Auth.Constants;
using MoviesAPI.Extensions;

namespace MoviesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = Roles.Read)]
public class StatusController : Controller
{
    private readonly IQueueService _queueService;

    public StatusController(IQueueService queueService)
    {
        queueService = null;
        queueService.ThrowIfNull();
        _queueService = queueService;
    }

    [HttpGet("")]
    public async Task<ActionResult<IMDBStatus>> GetAsync()
    {
        await _queueService.SendAsync(new Domain.Queues.QueueMessage()
        {
            DateTime = DateTime.Now,
            Id = Guid.NewGuid(),
            Message = "Hello, this is a SQS FIFO test from POL :)"
        });        

        return await Task.FromResult(Ok(IMDBStatusSingleton.Instance.Status));
    }
}
