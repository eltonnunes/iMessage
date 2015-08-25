using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using api.Negocios.SignalR;

namespace Server.Hubs
{
    [HubName("ServerAtosCapital")]
    public class ServerHub : Hub
    {
        private GatewayMonitorCargas monitorCargas;

        public void Conectado(string data)
        {
            if (monitorCargas == null) monitorCargas = new GatewayMonitorCargas(data);
            else monitorCargas.setData(data);

            monitorCargas.enviaLista(Context.ConnectionId);
        }
    }
}