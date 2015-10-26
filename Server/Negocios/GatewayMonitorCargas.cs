﻿using Microsoft.AspNet.SignalR;
using Server.Bibliotecas;
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

namespace Server.Negocios.SignalR
{
    public class GatewayMonitorCargas
    {
        private Semaphore semaforo;
        private Semaphore semaforoExecucao;
        private List<MonitorCargas> list;
        private IHubContext context;
        //private painel_taxservices_dbContext _db;
        private FiltroMonitorCargas filtro;
        private SqlConnection connection;
        private SqlCommand command;
        private string script;
        private string connectionId;
        // flag
        private bool alterouFiltro;



        public GatewayMonitorCargas(string connectionId, FiltroMonitorCargas filtro)
        {
            setFiltro(connectionId, filtro);
            semaforo = new Semaphore(1, 1);
            semaforoExecucao = new Semaphore(1, 1);
            context = GlobalHost.ConnectionManager.GetHubContext<ServerHub>();
            //_db = new painel_taxservices_dbContext();
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

            script = @"
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

                        WHERE YEAR(pos.LogExecution.dtaFiltroTransacoes) = " + DateTime.Now.Year +
                        @" AND MONTH(pos.LogExecution.dtaFiltroTransacoes) = " + DateTime.Now.Month;

            command = new SqlCommand(script, connection);
        }

        public void setFiltro(string connectionId, FiltroMonitorCargas filtro)
        {
            this.filtro = filtro;
            this.connectionId = connectionId;
            alterouFiltro = true;
        }

        public void initList()
        {
            // Erro!
            if (filtro.Token == null || !Permissoes.Autenticado(filtro.Token))
            {
                list = null;
                return;
            }

            if(connection == null)
                connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

            // YYYYMM
            int ano = filtro.Data.Length >= 4 ? Convert.ToInt32(filtro.Data.Substring(0, 4)) : DateTime.Now.Year;
            int mes = filtro.Data.Length >= 6 ? Convert.ToInt32(filtro.Data.Substring(4, 2)) : DateTime.Now.Month;

            script = @"
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

            // Usuário está amarrado a um grupo empresa?
            Int32 IdGrupo = Permissoes.GetIdGrupo(filtro.Token);
            if (IdGrupo != 0) script += @" AND cliente.grupo_empresa.id_grupo = " + IdGrupo;
            else if (Permissoes.isAtosRoleVendedor(filtro.Token))
            {
                // Perfil Comercial tem uma carteira de clientes específica
                List<Int32> listaIdsGruposEmpresas = Permissoes.GetIdsGruposEmpresasVendedor(filtro.Token);
                if (listaIdsGruposEmpresas.Count > 0) {
                    script += @" AND (cliente.grupo_empresa.id_grupo = " + listaIdsGruposEmpresas[0];
                    for (int k = 1; k < listaIdsGruposEmpresas.Count; k++)
                        script += @" OR cliente.grupo_empresa.id_grupo = " + listaIdsGruposEmpresas[k];
                    script += @")";
                }
            }
            else if (filtro.IdGrupo > 0)
                script += @" AND cliente.grupo_empresa.id_grupo = " + filtro.IdGrupo;

            // Usuário está amarrado a uma filial?
            string CnpjEmpresa = Permissoes.GetCNPJEmpresa(filtro.Token);
            if (!CnpjEmpresa.Equals(""))
                script += @" AND cliente.empresa.nu_cnpj = '" + CnpjEmpresa + @"'";
            else if (!filtro.NuCnpj.Equals(""))
                script += @" AND cliente.empresa.nu_cnpj = '" + filtro.NuCnpj + @"'";

            if(filtro.CdAdquirente > 0)
                script += @" AND pos.Operadora.id = " + filtro.CdAdquirente;

            // Filtro de status é feito no momento do agrupamento!

            semaforoExecucao.WaitOne();

            // Obtém novo comando
            command = new SqlCommand(script, connection);

            // Registra evento de notificação
            registraEventoNotificacao();


            if (!connection.State.Equals(ConnectionState.Open)) connection.Open();

            using (var reader = command.ExecuteReader())
            {

                //if (list != null) list.Clear();

                //semaforo.WaitOne();

                list = reader.Cast<IDataRecord>()
                                .Select(e => new MonitorCargas
                                {
                                    id = Convert.ToInt32(e["id"]),
                                    dtaFiltroTransacoes = (DateTime)e["dtaFiltroTransacoes"],
                                    statusExecution = Convert.ToString(e["statusExecution"]),
                                    dtaExecucaoFim = e["dtaExecucaoFim"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtaExecucaoFim"],
                                    dtaExecucaoProxima = e["dtaExecucaoProxima"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtaExecucaoProxima"],
                                    loginOperadora = new LoginOperadoraMonitor
                                    {
                                        id = Convert.ToInt32(e["idLoginOperadora"]),
                                        status = Convert.ToBoolean(e["status"]),
                                    },
                                    grupoempresa = new GrupoEmpresaMonitor
                                    {
                                        id_grupo = Convert.ToInt32(e["id_grupo"]),
                                        ds_nome = Convert.ToString(e["ds_nome"]),
                                    },
                                    empresa = new EmpresaMonitor
                                    {
                                        nu_cnpj = Convert.ToString(e["nu_cnpj"]),
                                        ds_fantasia = Convert.ToString(e["ds_fantasia"]),
                                        filial = Convert.ToString(e["filial"]),
                                    },
                                    operadora = new OperadoraMonitor
                                    {
                                        id = Convert.ToInt32(e["idOperadora"]),
                                        nmOperadora = Convert.ToString(e["nmOperadora"])
                                    }
                                }).ToList<MonitorCargas>();

                alterouFiltro = false;

                //semaforo.Release(1);
            }

            semaforoExecucao.Release(1);
        }



        private List<dynamic> getListaAgrupadaEOrdenada(List<MonitorCargas> lista, SqlNotificationInfo Info = SqlNotificationInfo.Unknown)
        {
            if (lista == null) return null;

            if (lista.Count == 0) return new List<dynamic>();

           //semaforo.WaitOne();

            // Agrupa
            List<MonitorCargasAgrupado> newList = lista
                        .GroupBy(e => new { e.loginOperadora.id, e.loginOperadora.status})
                        .Select(e => new MonitorCargasAgrupado
                        {
                            id = e.Key.id,
                            status = e.Key.status,
                            logExecution = e.Select(x => new LogExecutionMonitor
                            {
                                id = x.id,
                                dtaFiltroTransacoes = x.dtaFiltroTransacoes,
                                statusExecution = x.statusExecution,
                                dtaExecucaoFim = x.dtaExecucaoFim,
                                dtaExecucaoProxima = x.dtaExecucaoProxima,
                            }).OrderBy(x => x.dtaFiltroTransacoes).ToList<LogExecutionMonitor>(),
                            ultimaDataExecucaoFim = e.OrderByDescending(x => x.dtaExecucaoFim)
                                                     .Select(x => x.dtaExecucaoFim)
                                                     .FirstOrDefault(),
                            prioridade = e.Where(x => x.statusExecution.Equals("0")).Count() > 0 ? 1 : 0,
                            grupoempresa = e.Select(x => x.grupoempresa).FirstOrDefault(),
                            empresa = e.Select(x => x.empresa).FirstOrDefault(),
                            operadora = e.Select(x => x.operadora).FirstOrDefault(),
                        })
                        .ToList<MonitorCargasAgrupado>();

            //semaforo.Release(1);

            // Filtro de status?
            if (!filtro.Status.Equals(""))
            {
                if (filtro.Status.Equals("-1"))
                {
                    // Não carregado!
                    newList = newList.Where(e => e.logExecution.Count() == 0)
                                     .OrderByDescending(e => e.prioridade)
                                     .ThenByDescending(e => e.ultimaDataExecucaoFim)
                                     .ThenBy(e => e.empresa.ds_fantasia)
                                     .ThenBy(e => e.empresa.filial)
                                     .ThenBy(e => e.operadora.nmOperadora)
                                     .ToList<MonitorCargasAgrupado>();
                }
                else
                {
                    newList = newList.Where(e => e.logExecution.Any(l => l.statusExecution.Equals(filtro.Status)))
                                     .OrderByDescending(e => e.prioridade)
                                     .ThenByDescending(e => e.ultimaDataExecucaoFim)
                                     .ThenBy(e => e.empresa.ds_fantasia)
                                     .ThenBy(e => e.empresa.filial)
                                     .ThenBy(e => e.operadora.nmOperadora)
                                     .ToList<MonitorCargasAgrupado>();
                }
            }
            else
            {
                // Sem filtro de status => apenas ordena
                newList = newList.OrderByDescending(e => e.prioridade)
                                 .ThenByDescending(e => e.ultimaDataExecucaoFim)
                                 .ThenBy(e => e.empresa.ds_fantasia)
                                 .ThenBy(e => e.empresa.filial)
                                 .ThenBy(e => e.operadora.nmOperadora)
                                 .ToList<MonitorCargasAgrupado>();
            }

            if (Info.Equals(SqlNotificationInfo.Unknown)) return newList.ToList<dynamic>();

            return new List<dynamic>()
            {
                new { NotificationInfo = Info.ToString().ToUpper(),
                      objetos = newList
                    }
            };
        }


        private List<dynamic> obtemListaComMudancas(SqlNotificationInfo Info = SqlNotificationInfo.Unknown)
        {
            semaforo.WaitOne();

            List<dynamic> mudancas = new List<dynamic>();

            List<MonitorCargas> oldList = list != null ? list : new List<MonitorCargas>();

            initList();

            DateTime dtOut = DateTime.Now;

            if (list != null)
            {
                try {
                    if (Info.Equals(SqlNotificationInfo.Delete))
                        // Delete
                        mudancas = getListaAgrupadaEOrdenada(oldList
                                                                .Where(e => !list.Any(l => l.id == e.id))
                                                                .ToList<MonitorCargas>(), Info);
                    else
                        // Insert, Update
                        mudancas = getListaAgrupadaEOrdenada(list
                                                                .Where(e => !oldList.Any(l => l.id == e.id && l.statusExecution.Equals(e.statusExecution) && 
                                                                                         ((l.dtaExecucaoFim == null && e.dtaExecucaoFim == null) || 
                                                                                          (l.dtaExecucaoFim != null && e.dtaExecucaoFim != null && l.dtaExecucaoFim.Equals(e.dtaExecucaoFim)))))
                                                                .ToList<MonitorCargas>(), Info);
                }catch(Exception e)
                {
                }
            }

            semaforo.Release(1);

            return mudancas;
        }

        public void enviaLista()
        {
            semaforo.WaitOne();
            initList();
            context.Clients.Client(connectionId).enviaLista(list == null ? new List<dynamic>() : getListaAgrupadaEOrdenada(list));
            semaforo.Release();
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            //Clean up the old notification
            SqlDependency dep = (SqlDependency)sender;
            dep.OnChange -= dependency_OnChange;

            registraEventoNotificacao();

            if (e.Info.Equals(SqlNotificationInfo.Insert) ||
                e.Info.Equals(SqlNotificationInfo.Update) ||
                e.Info.Equals(SqlNotificationInfo.Delete))
            {
                // Só envia se de fato tiveram mudanças para o filtro selecionado
                List<dynamic> mudancas = obtemListaComMudancas(e.Info);
                if(mudancas.Count > 0) context.Clients.Client(connectionId).enviaMudancas(mudancas[0]);
            }
        }


        private void registraEventoNotificacao()
        {
            if (command == null) command = new SqlCommand(script, connection);

            // Make sure the command object does not already have
            // a notification object associated with it.
            command.Notification = null;

            SqlDependency dependency = new SqlDependency(command);
            dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
            
        }
    }
}