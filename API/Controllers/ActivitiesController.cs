using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    //No need to add [ApiController] attribute as it is derived from a class that has this attribute.
    public class ActivitiesController : BaseApiController
    {
        //[Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]   //api/activities
        public async Task<IActionResult> GetActivities()
        {
            return HandleResult(await Mediator.Send(new List.Query()));
            //return await _context.Activities.ToListAsync();
        }

        [HttpGet("{id}")] //api/activities/23
        public async Task<IActionResult> GetActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        //Note: Because this is a post method, controller is smart to know that it has to find Activity in Body. But you can decorate it with [FromBody] attribute. However, since we added [ApiController] attribute to the Base ApiController, we dont need to add FromBody.
        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Activity = activity }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            activity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Activity = activity }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));

        }
    }
}