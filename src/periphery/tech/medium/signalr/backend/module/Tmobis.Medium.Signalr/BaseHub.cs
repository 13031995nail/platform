namespace Tmobis.Medium.Signalr;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public abstract class BaseHub : Hub
{
}
