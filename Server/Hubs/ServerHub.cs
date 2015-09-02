using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using api.Negocios.SignalR;
using Server.Models.Object;
using Server.Bibliotecas;
using System.Web;
using System.Net;

namespace Server.Hubs
{
    [HubName("ServerAtosCapital")]
    public class ServerHub : Hub
    {
        private GatewayMonitorCargas monitorCargas;

        //[Authorize]
        public void obtemLista(FiltroMonitorCargas filtro)
        {
            /*string token = Context.QueryString["token"];
            if (!Permissoes.Autenticado(token)) {
                // DISCONNECT!
                return;
                //throw new HttpException((int)HttpStatusCode.Unauthorized, "Não Autorizado");
            }*/

            filtro.Token = Context.QueryString["token"];

            if (monitorCargas == null) monitorCargas = new GatewayMonitorCargas(Context.ConnectionId, filtro);
            else monitorCargas.setFiltro(Context.ConnectionId, filtro);

            monitorCargas.enviaLista();
        }
    }
}