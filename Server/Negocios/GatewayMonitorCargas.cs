using Microsoft.AspNet.SignalR;
using Server.Hubs;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace api.Negocios.SignalR
{
    public class GatewayMonitorCargas
    {
        private Semaphore semaforo;
        private List<dynamic> list;
        private IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ServerHub>();
        private painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
        private string data;

        public GatewayMonitorCargas(string data)
        {
            setData(data);
            semaforo = new Semaphore(1, 1);
        }

        public void setData(string data)
        {
            this.data = data;
        }

        public void initList()
        {

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

            connection.Open();

            // YYYYMM
            int ano = data.Length >= 4 ? Convert.ToInt32(data.Substring(0, 4)) : DateTime.Now.Year;
            int mes = data.Length >= 6 ? Convert.ToInt32(data.Substring(4, 2)) : DateTime.Now.Month;

            string script = @"
                        SELECT
                        pos.LogExecution.id,
                        pos.LogExecution.dtaFiltroTransacoes,
                        pos.LogExecution.statusExecution,
                        pos.LogExecution.idLoginOperadora,
                        pos.LogExecution.dtaExecucaoFim,
                        pos.LoginOperadora.status,
                        cliente.empresa.nu_cnpj,
                        cliente.empresa.ds_fantasia,
                        pos.Operadora.id AS idOperadora,
                        pos.Operadora.nmOperadora

                        FROM
                        pos.LogExecution
                        INNER JOIN pos.LoginOperadora ON pos.LogExecution.idLoginOperadora = pos.LoginOperadora.id
                        INNER JOIN cliente.empresa ON pos.LoginOperadora.cnpj = cliente.empresa.nu_cnpj
                        INNER JOIN pos.Operadora ON pos.LoginOperadora.idOperadora = pos.Operadora.id                        

                        WHERE YEAR(pos.LogExecution.dtaFiltroTransacoes) = " + ano + 
                        @" AND MONTH(pos.LogExecution.dtaFiltroTransacoes) = " + mes;

            using (SqlCommand command = new SqlCommand(script, connection))
            {
                // Make sure the command object does not already have
                // a notification object associated with it.
                command.Notification = null;


                SqlDependency dependency = new SqlDependency(command);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);


                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (var reader = command.ExecuteReader())
                {

                    //if (list != null) list.Clear();

                    semaforo.WaitOne();

                    list = reader.Cast<IDataRecord>()
                                    .Select(e => new
                                    {
                                        id = Convert.ToInt32(e["id"]),
                                        dtaFiltroTransacoes = e["dtaFiltroTransacoes"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtaFiltroTransacoes"],
                                        statusExecution = Convert.ToString(e["statusExecution"]),
                                        dtaExecucaoFim = e["dtaExecucaoFim"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtaExecucaoFim"],
                                        loginOperadora = new
                                        {
                                            id = Convert.ToInt32(e["idLoginOperadora"]),
                                            status = Convert.ToBoolean(e["status"]),
                                        },
                                        empresa = new
                                        {
                                            nu_cnpj = Convert.ToString(e["nu_cnpj"]),
                                            ds_fantasia = Convert.ToString(e["ds_fantasia"])
                                        },
                                        operadora = new
                                        {
                                            id = Convert.ToInt32(e["idOperadora"]),
                                            nmOperadora = Convert.ToString(e["nmOperadora"])
                                        }
                                    }).ToList<dynamic>();

                    semaforo.Release(1);
                }


            }
        }



        private List<dynamic> getListaAgrupadaEOrdenada(List<dynamic> lista)
        {
            if (lista == null) return null;

            semaforo.WaitOne();

            // Agrupa
            List<dynamic> newList = lista
                        .GroupBy(e => e.loginOperadora)
                        .Select(e => new
                        {
                            id = e.Key.id,
                            status = e.Key.status,
                            obj = e.Select(x => new
                            {
                                id = x.id,
                                dtaFiltroTransacoes = x.dtaFiltroTransacoes,
                                statusExecution = x.statusExecution,
                                dtaExecucaoFim = x.dtaExecucaoFim
                            }),
                            ultimaDataExecucaoFim = e.OrderByDescending(x => x.dtaExecucaoFim)
                                                     .Select(x => x.dtaExecucaoFim)
                                                     .FirstOrDefault(),
                            empresa = e.Select(x => x.empresa).FirstOrDefault(),
                            operadora = e.Select(x => x.operadora).FirstOrDefault(),
                        })
                        .ToList<dynamic>();

            semaforo.Release(1);

            // Ordena
            return newList.OrderByDescending(e => e.ultimaDataExecucaoFim).ToList<dynamic>();
        }


        private List<dynamic> obtemListaComMudancas(SqlNotificationInfo Info = SqlNotificationInfo.Unknown)
        {
            semaforo.WaitOne();

            List<dynamic> mudancas = new List<dynamic>();

            List<dynamic> oldList = list != null ? list : new List<dynamic>();

            initList();

            DateTime dtOut = DateTime.Now;

            if (list != null)
            {

                if (Info.Equals(SqlNotificationInfo.Delete))
                    // Delete
                    mudancas = getListaAgrupadaEOrdenada( oldList
                                                            .Where(e => !list.Any(l => l.id == e.id && l.statusExecution.Equals(e.statusExecution) && l.dtaExecucaoFim.Equals(e.dtaExecucaoFim)))
                                                            .ToList<dynamic>());
                else
                    // Insert, Update
                    mudancas = getListaAgrupadaEOrdenada(list
                                                            .Where(e => !oldList.Any(l => l.id == e.id && l.statusExecution.Equals(e.statusExecution) && l.dtaExecucaoFim.Equals(e.dtaExecucaoFim)))
                                                            .ToList<dynamic>());
            }

            semaforo.Release(1);

            return mudancas;
        }

        public void enviaLista(string connectionId)
        {
            if (list == null) initList();
            context.Clients.Client(connectionId).notifyCarga(getListaAgrupadaEOrdenada(list));
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Info.Equals(SqlNotificationInfo.Insert) || 
                e.Info.Equals(SqlNotificationInfo.Update) ||
                e.Info.Equals(SqlNotificationInfo.Delete))
                context.Clients.All.notifyCarga(obtemListaComMudancas(e.Info));
        }
    }
}