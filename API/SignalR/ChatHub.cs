using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        //Send Comment
        public async Task SendComment(Create.Command command) //This is from Application.Comments
        {
            //Method is all important. Client needs to know the method they are invoking.
            //Save Comment
            var comment = await _mediator.Send(command);
            //Send back the comment to everyone is connected to that activity.
            await Clients.Group(command.ActivityId.ToString())
                .SendAsync("ReceiveComment", comment.Value);

        }
        //When applying connections to a Hub, you want them to join a Group. Because we are sending to Group in above method.
        public override async Task OnConnectedAsync()
        {
            //We will get activity ID from query string from httpContext
            var httpContext = Context.GetHttpContext();
            var activityId = httpContext.Request.Query["activityId"]; //make sure the spelling is right as there is not safety here.

            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            var result = await _mediator.Send(new List.Query { ActivityId = Guid.Parse(activityId) });
            //
            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }
        //NOTE: We dont have to do anything on disconnect. SignalR will clear it for us.
    }
}