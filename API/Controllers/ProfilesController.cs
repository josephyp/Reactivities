using Application.Photos;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class ProfilesController : BaseApiController
    {
        [HttpGet("{username}/activities")]   //api/profile/{username}/activities
        public async Task<IActionResult> GetUserActivities(string username, [FromQuery] UserActivityParams param) //([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListActivities.Query { Params = param, Username = username }));
            //return await _context.Activities.ToListAsync();
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<Profile>> GetProfile(string username)
        {
            return HandleResult(await Mediator.Send(new Application.Profiles.Details.Query { Username = username }));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(Edit.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }
    }
}