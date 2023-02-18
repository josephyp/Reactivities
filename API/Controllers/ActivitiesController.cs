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

        [HttpGet]   //api/activities
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            return await Mediator.Send(new List.Query());
            //return await _context.Activities.ToListAsync();
        }

        [HttpGet("{id}")] //api/activities/23
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            //If there is no record with that id, then it returns null and then on browser it is a 204 which is OK but without content 
            return await Mediator.Send(new Details.Query { Id = id });
            //return await _context.Activities.FindAsync(id);
        }

        //Note: Because this is a post method, controller is smart to know that it has to find Activity in Body. But you can decorate it with [FromBody] attribute. However, since we added [ApiController] attribute to the Base ApiController, we dont need to add FromBody.
        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            return Ok(await Mediator.Send(new Create.Command { Activity = activity }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            activity.Id = id;
            return Ok(await Mediator.Send(new Edit.Command { Activity = activity }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            return Ok(await Mediator.Send(new Delete.Command { Id = id }));

        }
    }
}