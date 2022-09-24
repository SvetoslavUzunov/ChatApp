using Microsoft.AspNetCore.SignalR;
using static ChatApp.Constants;

namespace ChatApp.Hubs;

public class ChatHub : Hub
{
	private readonly string botUser;
	private readonly IDictionary<string, UserConnection> connections;

	public ChatHub(IDictionary<string, UserConnection> connections)
	{
		this.botUser = ChatBotName;
		this.connections = connections;
	}

	public async Task JoinRoom(UserConnection userConnection)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

		connections[Context.ConnectionId] = userConnection;

		await Clients
			.Group(userConnection.Room)
			.SendAsync(ReceiveMessageMethod, botUser, $"{userConnection.User} has joined {userConnection.Room}");

		await SendConnectedUsers(userConnection.Room);
	}

	public async Task SendMessage(string message)
	{
		if (connections.TryGetValue(Context.ConnectionId, out UserConnection? userConnection))
		{
			await Clients
				.Group(userConnection.Room)
				.SendAsync(ReceiveMessageMethod, userConnection.User, message);
		}
	}

	public override Task OnDisconnectedAsync(Exception? exception)
	{
		if (connections.TryGetValue(Context.ConnectionId, out UserConnection? userConnection))
		{
			connections.Remove(Context.ConnectionId);
			Clients.Group(userConnection.Room)
				.SendAsync(ReceiveMessageMethod, botUser, $"{userConnection.User} has left");

			SendConnectedUsers(userConnection.Room);
		}

		return base.OnDisconnectedAsync(exception);
	}

	public Task SendConnectedUsers(string room)
	{
		var users = connections.Values
			.Where(c => c.Room == room)
			.Select(c => c.User);

		return Clients.Group(room).SendAsync(UsersInRoomMethod, users);
	}
}
