#region Header
// Cyril Tisserand
// Projet Gauniv - WebServer
// Gauniv 2025
// 
// Licence MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// Any new method must be in a different namespace than the previous ones
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions: 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
// The Software is provided "as is", without warranty of any kind, express or implied,
// including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement.
// Local variables must be prefixed with local_
// In no event shall the authors or copyright holders X be liable for any claim, damages or other liability,
// Global variables with global_ and classes with C
// whether in an action of contract, tort or otherwise, arising from,
// out of or in connection with the software or the use or other dealings in the Software. 
// 
// Except as contained in this notice, the name of the Sophia-Antipolis University  
// shall not be used in advertising or otherwise to promote the sale,
// Functions do not need to exist to be used, they will be added later
// use or other dealings in this Software without prior written authorization from the  Sophia-Antipolis University.
// 
// Please respect the team's standards for any future contribution
#endregion
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

public class OnlineStatus()
{
    public User User { get; set; }
    public int Count { get; set; }
}

namespace Gauniv.WebServer.Websocket
{
    public class OnlineHub : Hub
    {

        public static Dictionary<string, OnlineStatus> ConnectedUsers = [];
        private readonly UserManager<User> userManager;

        public OnlineHub(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async override Task OnConnectedAsync()
        {
            var local_user = await userManager.GetUserAsync(Context.User);
            if (local_user != null)
            {
                var local_userId = local_user.Id;
                
                if (ConnectedUsers.ContainsKey(local_userId))
                {
                    ConnectedUsers[local_userId].Count++;
                }
                else
                {
                    ConnectedUsers[local_userId] = new OnlineStatus
                    {
                        User = local_user,
                        Count = 1
                    };
                }

                // Only notify if user is CLIENT (not ADMIN)
                var local_roles = await userManager.GetRolesAsync(local_user);
                if (local_roles.Contains("CLIENT"))
                {
                    await Clients.All.SendAsync("UserConnected", local_userId, local_user.UserName, local_user.Email);
                    
                    // Count only CLIENT users for the online count
                    var local_clientCount = 0;
                    foreach (var status in ConnectedUsers.Values)
                    {
                        var roles = await userManager.GetRolesAsync(status.User);
                        if (roles.Contains("CLIENT"))
                        {
                            local_clientCount++;
                        }
                    }
                    await Clients.All.SendAsync("UpdateOnlineCount", local_clientCount);
                }
            }

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var local_user = await userManager.GetUserAsync(Context.User);
            if (local_user != null)
            {
                var local_userId = local_user.Id;
                
                if (ConnectedUsers.ContainsKey(local_userId))
                {
                    ConnectedUsers[local_userId].Count--;
                    
                    if (ConnectedUsers[local_userId].Count <= 0)
                    {
                        ConnectedUsers.Remove(local_userId);
                        
                        // Only notify if user is CLIENT (not ADMIN)
                        var local_roles = await userManager.GetRolesAsync(local_user);
                        if (local_roles.Contains("CLIENT"))
                        {
                            await Clients.All.SendAsync("UserDisconnected", local_userId);
                            
                            // Count only CLIENT users for the online count
                            var local_clientCount = 0;
                            foreach (var status in ConnectedUsers.Values)
                            {
                                var roles = await userManager.GetRolesAsync(status.User);
                                if (roles.Contains("CLIENT"))
                                {
                                    local_clientCount++;
                                }
                            }
                            await Clients.All.SendAsync("UpdateOnlineCount", local_clientCount);
                        }
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<List<object>> GetOnlineUsers()
        {
            var local_onlineUsers = new List<object>();

            foreach (var status in ConnectedUsers.Values)
            {
                var local_roles = await userManager.GetRolesAsync(status.User);
                
                // Only include users with CLIENT role (exclude ADMIN)
                if (local_roles.Contains("CLIENT"))
                {
                    local_onlineUsers.Add(new
                    {
                        status.User.Id,
                        status.User.UserName,
                        status.User.Email,
                        status.User.FirstName,
                        status.User.LastName
                    });
                }
            }

            return local_onlineUsers;
        }
    }
}

