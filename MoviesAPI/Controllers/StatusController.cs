using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Background;
using MoviesAPI.DTOs.API;
using System.Threading.Tasks;
using static MoviesAPI.Auth.Constants;

namespace MoviesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Read)]
public class StatusController : Controller
{
    [HttpGet("")]
    public async Task<ActionResult<IMDBStatus>> GetAsync()
    {
        return await Task.FromResult(Ok(IMDBStatusSingleton.Instance.Status));
    }
}
