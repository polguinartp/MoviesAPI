using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Background;
using MoviesAPI.DTOs.API;
using static MoviesAPI.Auth.Constants;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Read)]
    public class StatusController : Controller
    {
        [HttpGet("")]
        public ActionResult<IMDBStatus> Get()
        {
            return Ok(IMDBStatusSingleton.Instance.Status);
        }
    }
}
