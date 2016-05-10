using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Server.Negocios.SignalR;
using Server.Models.Object;
using Server.Bibliotecas;
using System.Web;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Server.Hubs
{
    [HubName("ServerAtosCapital")]
    public class ServerHub : Hub
    {
        private GatewayMonitorCargas monitorCargas;
        private GatewayMonitorCargasBoot monitorCargasBoot;
        //private GatewayFilaBootICard filaBootICard;

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


        //[Authorize]
        public void obtemListaBoot(FiltroMonitorCargas filtro)
        {
            /*string token = Context.QueryString["token"];
            if (!Permissoes.Autenticado(token)) {
                // DISCONNECT!
                return;
                //throw new HttpException((int)HttpStatusCode.Unauthorized, "Não Autorizado");
            }*/

            filtro.Token = Context.QueryString["token"];            

            if (monitorCargasBoot == null) monitorCargasBoot = new GatewayMonitorCargasBoot(Context.ConnectionId, filtro);
            else monitorCargasBoot.setFiltro(Context.ConnectionId, filtro);

            monitorCargasBoot.enviaLista();
        }


        ////[Authorize]
        //public void obtemListaFilaBootICard()
        //{
        //    string token = Context.QueryString["token"];

        //    if (filaBootICard == null) filaBootICard = new GatewayFilaBootICard(Context.ConnectionId, token);
        //    else filaBootICard.setConnection(Context.ConnectionId, token);

        //    filaBootICard.enviaLista();
        //}




        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            //semaforo.WaitOne();
            //connections.Remove(Context.ConnectionId);
            //semaforo.Release(1);
            return base.OnDisconnected(stopCalled);
        }
    }
}