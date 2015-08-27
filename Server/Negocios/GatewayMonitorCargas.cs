using Microsoft.AspNet.SignalR;
using Server.Hubs;
using Server.Models;
using Server.Models.Object;
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
        private FiltroMonitorCargas filtro;

        public GatewayMonitorCargas(FiltroMonitorCargas filtro)
        {
            setFiltro(filtro);
            semaforo = new Semaphore(1, 1);
        }

        public void setFiltro(FiltroMonitorCargas filtro)
        {
            this.filtro = filtro;
        }

        public void initList()
        {

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

            connection.Open();

            // YYYYMM
            int ano = filtro.Data.Length >= 4 ? Convert.ToInt32(filtro.Data.Substring(0, 4)) : DateTime.Now.Year;
            int mes = filtro.Data.Length >= 6 ? Convert.ToInt32(filtro.Data.Substring(4, 2)) : DateTime.Now.Month;

            string script = @"
                        SELECT
                        pos.LogExecution.id,
                        pos.LogExecution.dtaFiltroTransacoes,
                        pos.LogExecution.statusExecution,
                        pos.LogExecution.idLoginOperadora,
                        pos.LogExecution.dtaExecucaoFim,
                        pos.LogExecution.dtaExecucaoProxima,
                        pos.LoginOperadora.status,
                        cliente.grupo_empresa.id_grupo,
                        cliente.grupo_empresa.ds_nome,
                        cliente.empresa.nu_cnpj,
                        cliente.empresa.ds_fantasia,
                        cliente.empresa.filial,
                        pos.Operadora.id AS idOperadora,
                        pos.Operadora.nmOperadora

                        FROM
                        pos.LogExecution
                        INNER JOIN pos.LoginOperadora ON pos.LogExecution.idLoginOperadora = pos.LoginOperadora.id
                        INNER JOIN cliente.empresa ON pos.LoginOperadora.cnpj = cliente.empresa.nu_cnpj
                        INNER JOIN pos.Operadora ON pos.LoginOperadora.idOperadora = pos.Operadora.id
                        INNER JOIN cliente.grupo_empresa ON pos.LoginOperadora.idGrupo = cliente.grupo_empresa.id_grupo                        

                        WHERE YEAR(pos.LogExecution.dtaFiltroTransacoes) = " + ano + 
                        @" AND MONTH(pos.LogExecution.dtaFiltroTransacoes) = " + mes;

            if (filtro.IdGrupo > 0)
                script += @" AND cliente.grupo_empresa.id_grupo = " + filtro.IdGrupo;

            if(!filtro.NuCnpj.Equals(""))
                script += @" AND cliente.empresa.nu_cnpj = '" + filtro.NuCnpj + @"'";

            if(filtro.CdAdquirente > 0)
                script += @" AND pos.Operadora.id = " + filtro.CdAdquirente;

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

                    //semaforo.WaitOne();

                    list = reader.Cast<IDataRecord>()
                                    .Select(e => new
                                    {
                                        id = Convert.ToInt32(e["id"]),
                                        dtaFiltroTransacoes = e["dtaFiltroTransacoes"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtaFiltroTransacoes"],
                                        statusExecution = Convert.ToString(e["statusExecution"]),
                                        dtaExecucaoFim = e["dtaExecucaoFim"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtaExecucaoFim"],
                                        dtaExecucaoProxima = e["dtaExecucaoProxima"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtaExecucaoProxima"],
                                        loginOperadora = new
                                        {
                                            id = Convert.ToInt32(e["idLoginOperadora"]),
                                            status = Convert.ToBoolean(e["status"]),
                                        },
                                        grupoempresa = new
                                        {
                                            id_grupo = Convert.ToInt32(e["id_grupo"]),
                                            ds_nome = Convert.ToString(e["ds_nome"]),
                                        },
                                        empresa = new
                                        {
                                            nu_cnpj = Convert.ToString(e["nu_cnpj"]),
                                            ds_fantasia = Convert.ToString(e["ds_fantasia"]),
                                            filial = Convert.ToString(e["filial"]),
                                        },
                                        operadora = new
                                        {
                                            id = Convert.ToInt32(e["idOperadora"]),
                                            nmOperadora = Convert.ToString(e["nmOperadora"])
                                        }
                                    }).ToList<dynamic>();

                    //semaforo.Release(1);
                }


            }
        }



        private List<dynamic> getListaAgrupadaEOrdenada(List<dynamic> lista, SqlNotificationInfo Info = SqlNotificationInfo.Unknown)
        {
            if (lista == null) return null;

            if (lista.Count == 0) return lista;

           //semaforo.WaitOne();

            // Agrupa
            List<dynamic> newList = lista
                        .GroupBy(e => e.loginOperadora)
                        .Select(e => new
                        {
                            id = e.Key.id,
                            status = e.Key.status,
                            logExecution = e.Select(x => new
                            {
                                id = x.id,
                                dtaFiltroTransacoes = x.dtaFiltroTransacoes,
                                statusExecution = x.statusExecution,
                                dtaExecucaoFim = x.dtaExecucaoFim,
                                dtaExecucaoProxima = x.dtaExecucaoProxima,
                            }).OrderBy(x => x.dtaFiltroTransacoes).ToList<dynamic>(),
                            ultimaDataExecucaoFim = e.OrderByDescending(x => x.dtaExecucaoFim)
                                                     .Select(x => x.dtaExecucaoFim)
                                                     .FirstOrDefault(),
                            grupoempresa = e.Select(x => x.grupoempresa).FirstOrDefault(),
                            empresa = e.Select(x => x.empresa).FirstOrDefault(),
                            operadora = e.Select(x => x.operadora).FirstOrDefault(),
                        })
                        .ToList<dynamic>();

            //semaforo.Release(1);

            // Ordena
            newList = newList.OrderByDescending(e => e.ultimaDataExecucaoFim)
                             .ThenBy(e => e.empresa.ds_fantasia)
                             .ThenBy(e => e.empresa.filial)
                             .ThenBy(e => e.operadora.nmOperadora)
                             .ToList<dynamic>();

            if (Info.Equals(SqlNotificationInfo.Unknown)) return newList;

            return new List<dynamic>()
            {
                new { NotificationInfo = Info.ToString(),
                      objetos = newList
                    }
            };
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
                try {
                    if (Info.Equals(SqlNotificationInfo.Delete))
                        // Delete
                        mudancas = getListaAgrupadaEOrdenada(oldList
                                                                .Where(e => !list.Any(l => l.id == e.id && l.statusExecution.Equals(e.statusExecution) && l.dtaExecucaoFim.Equals(e.dtaExecucaoFim)))
                                                                .ToList<dynamic>(), Info);
                    else
                        // Insert, Update
                        mudancas = getListaAgrupadaEOrdenada(list
                                                                .Where(e => !oldList.Any(l => l.id == e.id && l.statusExecution.Equals(e.statusExecution) && l.dtaExecucaoFim.Equals(e.dtaExecucaoFim)))
                                                                .ToList<dynamic>(), Info);
                }catch { }
            }

            semaforo.Release(1);

            return mudancas;
        }

        public void enviaLista(string connectionId)
        {
            semaforo.WaitOne();
            if (list == null) initList();
            context.Clients.Client(connectionId).enviaLista(getListaAgrupadaEOrdenada(list));
            semaforo.Release();
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Info.Equals(SqlNotificationInfo.Insert) ||
                e.Info.Equals(SqlNotificationInfo.Update) ||
                e.Info.Equals(SqlNotificationInfo.Delete))
            {
                // Só envia se de fato tiveram mudanças para o filtro selecionado
                List<dynamic> mudancas = obtemListaComMudancas(e.Info);
                if(mudancas.Count > 0)
                    context.Clients.All.enviaMudancas(mudancas[0]);
            }
        }
    }
}