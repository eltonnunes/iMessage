using Microsoft.AspNet.SignalR;
using Server.Bibliotecas;
using Server.Hubs;
using Server.Models;
using Server.Models.Object;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace Server.Negocios.SignalR
{
    public class GatewayMonitorCargasBoot
    {
        private Semaphore semaforo;
        private Semaphore semaforoExecucao;
        private List<MonitorCargasBoot> list;
        private IHubContext context;
        //private painel_taxservices_dbContext _db;
        private FiltroMonitorCargas filtro;
        private SqlConnection connection;
        private SqlCommand command;
        private string script;
        private string connectionId;
        // flag
        private bool alterouFiltro;



        public GatewayMonitorCargasBoot(string connectionId, FiltroMonitorCargas filtro)
        {
            setFiltro(connectionId, filtro);
            semaforo = new Semaphore(1, 1);
            semaforoExecucao = new Semaphore(1, 1);
            context = GlobalHost.ConnectionManager.GetHubContext<ServerHub>();
            //_db = new painel_taxservices_dbContext();
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

            script = @"
                        SELECT
                        card.tbLogCarga.idLogCarga,
                        card.tbLogCarga.dtCompetencia,
                        card.tbLogCarga.flStatusVendasCredito,
                        card.tbLogCarga.flStatusVendasDebito,
                        card.tbLogCarga.flStatusPagosCredito,
                        card.tbLogCarga.flStatusPagosDebito,
                        card.tbLogCarga.flStatusPagosAntecipacao,
                        card.tbLogCarga.flStatusReceber,
                        card.tbLogCargaDetalhe.idLogCargaDetalhe,
                        card.tbLogCargaDetalhe.dtExecucaoIni,
                        card.tbLogCargaDetalhe.dtExecucaoFim,
                        card.tbLogCargaDetalhe.flStatus,
                        card.tbLogCargaDetalhe.dsMensagem,
                        card.tbLogCargaDetalhe.dsModalidade,
                        card.tbLogCargaDetalhe.qtTransacoes,
                        card.tbLogCargaDetalhe.vlTotalProcessado,
                        card.tbLogCargaDetalhe.qtTransacoesCS,
                        card.tbLogCargaDetalhe.vlTotalProcessadoCS,
                        card.tbLogCargaDetalhe.txAuditoria,
                        cliente.grupo_empresa.ds_nome,
                        cliente.empresa.ds_fantasia,
                        cliente.empresa.filial,
                        card.tbAdquirente.nmAdquirente

                        FROM
                        card.tbLogCargaDetalhe
                        INNER JOIN card.tbLogCarga ON card.tbLogCargaDetalhe.idLogCarga = card.tbLogCarga.idLogCarga
                        INNER JOIN card.tbAdquirente ON card.tbLogCarga.cdAdquirente = card.tbAdquirente.cdAdquirente
                        INNER JOIN cliente.empresa ON card.tbLogCarga.nrCNPJ = cliente.empresa.nu_cnpj
                        INNER JOIN cliente.grupo_empresa ON cliente.empresa.id_grupo = cliente.grupo_empresa.id_grupo                        

                        WHERE YEAR(card.tbLogCarga.dtCompetencia) = " + DateTime.Now.Year +
                        @" AND MONTH(card.tbLogCarga.dtCompetencia) = " + DateTime.Now.Month;

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

            // YYYYMM
            int ano = filtro.Data.Length >= 4 ? Convert.ToInt32(filtro.Data.Substring(0, 4)) : DateTime.Now.Year;
            int mes = filtro.Data.Length >= 6 ? Convert.ToInt32(filtro.Data.Substring(4, 2)) : DateTime.Now.Month;

            script = @"
                        SELECT
                        card.tbLogCarga.idLogCarga,
                        card.tbLogCarga.dtCompetencia,
                        card.tbLogCarga.flStatusVendasCredito,
                        card.tbLogCarga.flStatusVendasDebito,
                        card.tbLogCarga.flStatusPagosCredito,
                        card.tbLogCarga.flStatusPagosDebito,
                        card.tbLogCarga.flStatusPagosAntecipacao,
                        card.tbLogCarga.flStatusReceber,
                        card.tbLogCargaDetalhe.idLogCargaDetalhe,
                        card.tbLogCargaDetalhe.dtExecucaoIni,
                        card.tbLogCargaDetalhe.dtExecucaoFim,
                        card.tbLogCargaDetalhe.flStatus,
                        card.tbLogCargaDetalhe.dsMensagem,
                        card.tbLogCargaDetalhe.dsModalidade,
                        card.tbLogCargaDetalhe.qtTransacoes,
                        card.tbLogCargaDetalhe.vlTotalProcessado,
                        card.tbLogCargaDetalhe.qtTransacoesCS,
                        card.tbLogCargaDetalhe.vlTotalProcessadoCS,
                        card.tbLogCargaDetalhe.txAuditoria,
                        cliente.grupo_empresa.id_grupo,
                        cliente.grupo_empresa.ds_nome,
                        cliente.empresa.nu_cnpj,
                        cliente.empresa.ds_fantasia,
                        cliente.empresa.filial,
                        card.tbAdquirente.cdAdquirente,
                        card.tbAdquirente.nmAdquirente

                        FROM
                        card.tbLogCargaDetalhe 
                        INNER JOIN card.tbLogCarga ON card.tbLogCargaDetalhe.idLogCarga = card.tbLogCarga.idLogCarga
                        INNER JOIN card.tbAdquirente ON card.tbLogCarga.cdAdquirente = card.tbAdquirente.cdAdquirente
                        INNER JOIN cliente.empresa ON card.tbLogCarga.nrCNPJ = cliente.empresa.nu_cnpj
                        INNER JOIN cliente.grupo_empresa ON cliente.empresa.id_grupo = cliente.grupo_empresa.id_grupo                        

                        WHERE YEAR(card.tbLogCarga.dtCompetencia) = " + ano +
                        @" AND MONTH(card.tbLogCarga.dtCompetencia) = " + mes;

            // Usuário está amarrado a um grupo empresa?
            Int32 IdGrupo = Permissoes.GetIdGrupo(filtro.Token);
            if (IdGrupo != 0) script += @" AND cliente.grupo_empresa.id_grupo = " + IdGrupo;
            else if (Permissoes.isAtosRoleVendedor(filtro.Token))
            {
                // Perfil Comercial tem uma carteira de clientes específica
                List<Int32> listaIdsGruposEmpresas = Permissoes.GetIdsGruposEmpresasVendedor(filtro.Token);
                if (listaIdsGruposEmpresas.Count > 0)
                {
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

            if (filtro.CdAdquirente > 0)
                script += @" AND card.tbAdquirente.cdAdquirente = " + filtro.CdAdquirente;

            // Filtro de status é feito no momento do agrupamento!

            semaforoExecucao.WaitOne();

            DateTime now = DateTime.Now;

            try
            {
                if (connection == null)
                    connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);


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
                                    // Descarta as execuções que "dizem" estar em curso (status 0 ou 9) que começaram a mais de 48 horas da data corrente
                                    .Where(e => (Convert.ToByte(e["flStatus"]) != 0 && Convert.ToByte(e["flStatus"]) != 9) || now.Subtract((DateTime)e["dtExecucaoIni"]).TotalHours < 48)
                                    .Select(e => new MonitorCargasBoot
                                    {
                                        idLogCargaDetalhe = Convert.ToInt32(e["idLogCargaDetalhe"]),
                                        dtExecucaoIni = (DateTime)e["dtExecucaoIni"],
                                        dtExecucaoFim = e["dtExecucaoFim"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtExecucaoFim"],
                                        flStatus = Convert.ToByte(e["flStatus"]),
                                        dsMensagem = Convert.ToString(e["dsMensagem"]),
                                        dsModalidade = Convert.ToString(e["dsModalidade"]),
                                        qtTransacoes = Convert.ToInt32(e["qtTransacoes"]),
                                        vlTotalProcessado = Convert.ToDecimal(e["vlTotalProcessado"]),
                                        qtTransacoesCS = Convert.ToInt32(e["qtTransacoes"]),
                                        vlTotalProcessadoCS = Convert.ToDecimal(e["vlTotalProcessado"]),
                                        txAuditoria = Convert.ToString(e["txAuditoria"]),
                                        tbLogCarga = new tbLogCargaMonitor
                                        {
                                            idLogCarga = Convert.ToInt32(e["idLogCarga"]),
                                            dtCompetencia = (DateTime)e["dtCompetencia"],
                                            flStatusPagosAntecipacao = Convert.ToBoolean(e["flStatusPagosAntecipacao"]),
                                            flStatusPagosCredito = Convert.ToBoolean(e["flStatusPagosCredito"]),
                                            flStatusPagosDebito = Convert.ToBoolean(e["flStatusPagosDebito"]),
                                            flStatusReceber = Convert.ToBoolean(e["flStatusReceber"]),
                                            flStatusVendasCredito = Convert.ToBoolean(e["flStatusVendasCredito"]),
                                            flStatusVendasDebito = Convert.ToBoolean(e["flStatusVendasDebito"]),
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
                                        tbAdquirente = new tbAdquirenteMonitor
                                        {
                                            cdAdquirente = Convert.ToInt32(e["cdAdquirente"]),
                                            nmAdquirente = Convert.ToString(e["nmAdquirente"])
                                        }
                                    })
                                    .ToList<MonitorCargasBoot>();

                    alterouFiltro = false;

                    //semaforo.Release(1);
                }
            }
            catch (Exception e)
            {
                // ...
            }
            finally
            {
                semaforoExecucao.Release(1);
            }
        }



        private List<dynamic> getListaAgrupadaEOrdenada(List<MonitorCargasBoot> lista, SqlNotificationInfo Info = SqlNotificationInfo.Unknown)
        {
            if (lista == null) return null;

            if (lista.Count == 0) return new List<dynamic>();

            //semaforo.WaitOne();

            // Agrupa
            List<MonitorCargasBootAgrupado> newList = lista
                        // Refazer agrupamento => Empresa/Adquirente -> LogCarga
                        .GroupBy(e => new { e.empresa.nu_cnpj, e.tbAdquirente.cdAdquirente })
                        .Select(e => new MonitorCargasBootAgrupado
                        {
                            tbLogCargas = e.GroupBy(x => x.tbLogCarga.idLogCarga)
                            .Select(x => new tbLogCargasMonitor
                            {
                                idLogCarga = x.Key,
                                dtCompetencia = x.Select(d => d.tbLogCarga.dtCompetencia).FirstOrDefault(),
                                flStatusPagosAntecipacao = x.Select(d => d.tbLogCarga.flStatusPagosAntecipacao).FirstOrDefault(),
                                flStatusPagosCredito = x.Select(d => d.tbLogCarga.flStatusPagosCredito).FirstOrDefault(),
                                flStatusPagosDebito = x.Select(d => d.tbLogCarga.flStatusPagosDebito).FirstOrDefault(),
                                flStatusReceber = x.Select(d => d.tbLogCarga.flStatusReceber).FirstOrDefault(),
                                flStatusVendasCredito = x.Select(d => d.tbLogCarga.flStatusVendasCredito).FirstOrDefault(),
                                flStatusVendasDebito = x.Select(d => d.tbLogCarga.flStatusVendasDebito).FirstOrDefault(),
                                tbLogCargasDetalheMonitor = x.OrderBy(d => d.dsModalidade)
                                                             .GroupBy(d => d.dsModalidade)
                                .Select(d => new tbLogCargaDetalheMonitor
                                {
                                    dsMensagem = d.OrderByDescending(f => f.dtExecucaoIni).Select(f => f.dsMensagem).FirstOrDefault(),
                                    dsModalidade = d.Key,
                                    dtExecucaoFim = d.OrderByDescending(f => f.dtExecucaoIni).Select(f => f.dtExecucaoFim).FirstOrDefault(),
                                    dtExecucaoIni = d.OrderByDescending(f => f.dtExecucaoIni).Select(f => f.dtExecucaoIni).FirstOrDefault(),
                                    flStatus = d.OrderByDescending(f => f.dtExecucaoIni).Select(f => f.flStatus).FirstOrDefault(),
                                    idLogCargaDetalhe = d.OrderByDescending(f => f.dtExecucaoIni).Select(f => f.idLogCargaDetalhe).FirstOrDefault(),
                                    qtTransacoes = d.OrderByDescending(f => f.dtExecucaoIni).Select(f => f.qtTransacoes).FirstOrDefault(),
                                    vlTotalProcessado = d.OrderByDescending(f => f.dtExecucaoIni).Select(f => f.vlTotalProcessado).FirstOrDefault(),
                                    qtTransacoesCS = d.OrderByDescending(f => f.dtExecucaoIni).Select(f => f.qtTransacoesCS).FirstOrDefault(),
                                    vlTotalProcessadoCS = d.OrderByDescending(f => f.dtExecucaoIni).Select(f => f.vlTotalProcessadoCS).FirstOrDefault(),
                                    txAuditoria = d.OrderByDescending(f => f.dtExecucaoIni).Select(f => f.txAuditoria).FirstOrDefault(),
                                }).ToList<tbLogCargaDetalheMonitor>(),
                            }).OrderBy(x => x.dtCompetencia).ToList<tbLogCargasMonitor>(),
                            ultimaDataExecucaoFim = e.OrderByDescending(x => x.dtExecucaoFim)
                                                     .Select(x => x.dtExecucaoFim)
                                                     .FirstOrDefault(),
                            prioridade = e.Where(x => x.flStatus == 0 || x.flStatus == 9).Count() > 0 ? 1 : 0,
                            grupoempresa = e.Select(x => x.grupoempresa).FirstOrDefault(),
                            empresa = e.Select(x => x.empresa).FirstOrDefault(),
                            tbAdquirente = e.Select(x => x.tbAdquirente).FirstOrDefault(),
                        })
                        .OrderByDescending(e => e.prioridade)
                        .ThenByDescending(e => e.ultimaDataExecucaoFim)
                        .ThenBy(e => e.empresa.ds_fantasia)
                        .ThenBy(e => e.empresa.filial)
                        .ThenBy(e => e.tbAdquirente.nmAdquirente)
                        .ToList<MonitorCargasBootAgrupado>();

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
            List<dynamic> mudancas = new List<dynamic>();

            if (filtro.Token == null || !Permissoes.Autenticado(filtro.Token))
                return mudancas;

            semaforo.WaitOne();

            List<MonitorCargasBoot> oldList = list != null ? list : new List<MonitorCargasBoot>();

            initList();

            DateTime dtOut = DateTime.Now;

            if (list != null)
            {
                try
                {
                    if (Info.Equals(SqlNotificationInfo.Delete))
                        // Delete
                        mudancas = getListaAgrupadaEOrdenada(oldList
                                                                .Where(e => !list.Any(l => l.idLogCargaDetalhe == e.idLogCargaDetalhe)) //l.empresa.nu_cnpj == e.empresa.nu_cnpj && l.tbAdquirente.cdAdquirente == e.tbAdquirente.cdAdquirente))
                                                                .ToList<MonitorCargasBoot>(), Info);
                    else
                        // Insert, Update
                        mudancas = getListaAgrupadaEOrdenada(list
                                                                .Where(e => !oldList.Any(l => l.idLogCargaDetalhe == e.idLogCargaDetalhe && l.flStatus.Equals(e.flStatus) &&
                                                                                         ((l.dtExecucaoFim == null && e.dtExecucaoFim == null) ||
                                                                                          (l.dtExecucaoFim != null && e.dtExecucaoFim != null && l.dtExecucaoFim.Equals(e.dtExecucaoFim)))))
                                                                .ToList<MonitorCargasBoot>(), Info);
                }
                catch (Exception e)
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
            semaforo.Release(1);
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
                if (!Permissoes.Autenticado(filtro.Token))
                    return;
                
                // Só envia se de fato tiveram mudanças para o filtro selecionado
                List<dynamic> mudancas = obtemListaComMudancas(e.Info);
                if (mudancas.Count > 0) context.Clients.Client(connectionId).enviaMudancas(mudancas[0]);
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
