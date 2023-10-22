using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace LoginRegistrationApp.Hubs
{
    public class ChatHub : Hub
    {
        // Concurrent dictionary to store ContextId and Username mapping
        private static readonly ConcurrentDictionary<string, string> contextIdToUsername = new ConcurrentDictionary<string, string>();
        public async Task JoinChat(string user, string message)
        {
            string connectionId = Context.ConnectionId;

            // Add the mapping of ContextId to Username
            contextIdToUsername.TryAdd(user, connectionId);
            await Clients.Others.SendAsync("ReceiveMessage", user, message);
        }


        //public async Task SendMessage(string user, string message) => await Clients.All.SendAsync("ReceiveMessage", user, message);


        public async Task SendPrivateMessage(string targetUser, string user, string message)
        {
            // Retrieve the ContextId of the target Username
            if (contextIdToUsername.TryGetValue(targetUser, out string targetConnectionId))
            {
                await Clients.Client(targetConnectionId).SendAsync("ReceiveMessage", user, message);
                await Clients.Caller.SendAsync("ReceiveMessage", user, message);
            }
        }
    }
}

