using ChatAppProject.Data;
using ChatAppProject.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using Group = ChatAppProject.Models.Group;

namespace ChatAppProject.Hubs
{
    public class ChatHub:Hub
    {
        public async Task GetUsername(string username)
        {
            Client client = new Client
            {
                ConnectionId = Context.ConnectionId,
                Username = username
            };

            ClientSource.Clients.Add(client);
            await Clients.Others.SendAsync("clientJoined", username);
            await Clients.All.SendAsync("clients", ClientSource.Clients);
        }

        public async Task SendMessageAsync(string message,string connectionId)
        {
            if (String.IsNullOrEmpty(connectionId))
                await Clients.Others.SendAsync("receiveMessage", message, ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId)?.Username);
            else
                await Clients.Client(connectionId).SendAsync("receiveMessage",message,
                    ClientSource.Clients.FirstOrDefault(c=>c.ConnectionId == Context.ConnectionId)?.Username);
        }

        public async Task SendMessageToGroup(string message,IEnumerable<string> groups)
        {
            foreach (string group in groups)
            {
                if (!String.IsNullOrEmpty(group)) 
                    await Clients.Group(group).SendAsync("receiveMessage", message, ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId)?.Username);
            }
        }

        public async Task AddClientToGroup(IEnumerable<string> groups)
        {
           
            foreach (string groupname in groups)
            {
                Group _group = GroupSource.Groups.FirstOrDefault(g => g.GroupName == groupname.Trim());
                if (_group is null)
                {
                    Group group = new Group();
                    group.GroupName = groupname.Trim();
                   
                    group.Clients.Add(ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId));
                    
                    GroupSource.Groups.Add(group);
                }
                else
                {
                    if(!_group.Clients.Any(c => c.ConnectionId == Context.ConnectionId))
                        _group.Clients.Add(ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId));
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, groupname.Trim());
            }
            await Clients.All.SendAsync("getGroups", GroupSource.Groups);
        }


        public async Task GetClientToGroup(string groupName)
        {
            await Clients.Caller.SendAsync("clients", String.IsNullOrEmpty(groupName) ? ClientSource.Clients : GroupSource.Groups?.FirstOrDefault(g => g.GroupName == groupName)?.Clients);
        }




        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ClientSource.Clients.Remove(ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId));
            await Clients.All.SendAsync("clients", ClientSource.Clients);
        }


    }
}
