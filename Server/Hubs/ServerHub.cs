using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using api.Negocios.SignalR;
using Server.Models.Object;

namespace Server.Hubs
{
    [HubName("ServerAtosCapital")]
    public class ServerHub : Hub
    {
        private GatewayMonitorCargas monitorCargas;

        public void obtemLista(FiltroMonitorCargas filtro)
        {
            if (monitorCargas == null) monitorCargas = new GatewayMonitorCargas(filtro);
            else monitorCargas.setFiltro(filtro);

            monitorCargas.enviaLista(Context.ConnectionId);
        }
    }
}