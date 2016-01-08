using Microsoft.AspNet.SignalR;
using Server.Bibliotecas;
using Server.Hubs;
using Server.Models.Object;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;

namespace Server.Negocios.SignalR
{
    public class GatewayFilaBootICard
    {

        private Semaphore semaforo;
        private IHubContext context;
        private Semaphore semaforoExecucao;
        private SqlConnection connection;
        private SqlCommand command;
        private string script;
        private string connectionId;
        private string token;

        private List<FilaBootICard> list;


        public GatewayFilaBootICard(string connectionId, string token)
        {
            setConnection(connectionId, token);

            context = GlobalHost.ConnectionManager.GetHubContext<ServerHub>();

            semaforo = new Semaphore(1, 1);
            semaforoExecucao = new Semaphore(1, 1);

            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

            script = @" SELECT
	                        F.id,
	                        F.dsModalidade,
	                        F.dtInicio,
	                        F.dtFim,
	                        F.stProcessamento,
	                        F.dtProcessamento,
                            F.dtInsert,
	                        G.id_grupo,
	                        G.ds_nome,
	                        E.nu_cnpj,
	                        E.ds_fantasia,
	                        E.filial,
	                        A.cdAdquirente,
	                        A.nmAdquirente,
	                        uI.id_users as id_usersInsert,
	                        uI.ds_login as ds_loginInsert,
                            F.cdUser
	                        

                        FROM
	                        card.tbFilaBootICard F
                        INNER JOIN cliente.empresa E ON E.nu_cnpj = F.nrCNPJ
                        INNER JOIN cliente.grupo_empresa G ON E.id_grupo = G.id_grupo
                        INNER JOIN card.tbAdquirente A ON A.cdAdquirente = F.cdAdquirente
                        INNER JOIN dbo.webpages_Users uI ON uI.id_users = F.cdUserInsert
                        
                        WHERE
	                        F.stProcessamento = 2";

            command = new SqlCommand(script, connection);
        }

        public void setConnection(string connectionId, string token)
        {
            this.connectionId = connectionId;
            this.token = token;
        }


        public void initList()
        {
            if (token == null || !Permissoes.Autenticado(token))
                return;

            semaforoExecucao.WaitOne();

            try
            {
                if (connection == null)
                    connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);

                // Obtém novo comando
                command = new SqlCommand(script, connection);

                // Registra evento de notificação
                registraEventoNotificacao();


                if (!connection.State.Equals(ConnectionState.Open))
                    connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    list = reader.Cast<IDataRecord>()
                                    .Select(e => new FilaBootICard
                                    {
                                        id = Convert.ToInt32(e["id"]),
                                        empresa = new FilaBootICard.FilaEmpresa
                                        {
                                            nu_cnpj = Convert.ToString(e["nu_cnpj"]),
                                            ds_fantasia = Convert.ToString(e["ds_fantasia"])
                                        },
                                        grupo_empresa = new FilaBootICard.FilaGrupoEmpresa
                                        {
                                            id_grupo = Convert.ToInt32(e["id_grupo"]),
                                            ds_nome = Convert.ToString(e["ds_nome"])
                                        },
                                        tbAdquirente = new FilaBootICard.FilaTbAdquirente
                                        {
                                            cdAdquirente = Convert.ToInt32(e["cdAdquirente"]),
                                            nmAdquirente = Convert.ToString(e["nmAdquirente"])
                                        },
                                        cdUser = e["cdUser"].Equals(DBNull.Value) ? (int?)null : Convert.ToInt32(e["cdUser"]),
                                        //webpages_Users = e["id_users"].Equals(DBNull.Value) ? (FilaBootICard.FilaWebpagesUsers)null :
                                        //                 new FilaBootICard.FilaWebpagesUsers
                                        //                 {
                                        //                     id_users = Convert.ToInt32(e["id_users"]),
                                        //                     ds_login = Convert.ToString(e["ds_login"])
                                        //                 },
                                        webpages_UsersInsert = new FilaBootICard.FilaWebpagesUsers
                                                         {
                                                             id_users = Convert.ToInt32(e["id_usersInsert"]),
                                                             ds_login = Convert.ToString(e["ds_loginInsert"])
                                                         },
                                        dsModalidade = Convert.ToString(e["dsModalidade"]),
                                        dtInicio = (DateTime)e["dtInicio"],
                                        dtFim = (DateTime)e["dtFim"],
                                        stProcessamento = Convert.ToByte(e["stProcessamento"]),
                                        dtProcessamento = e["dtProcessamento"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtProcessamento"],
                                        dtInsert = (DateTime)e["dtInsert"],
                                    })
                                    .OrderBy(e => e.dtInsert)
                                    .ThenBy(e => e.grupo_empresa.cdPrioridade)
                                    .ThenBy(e => e.grupo_empresa.ds_nome)
                                    .ThenBy(e => e.tbAdquirente.nmAdquirente)
                                    .ThenBy(e => e.dsModalidade)
                                    .ThenBy(e => e.dtInicio)
                                    .ToList<FilaBootICard>();
                }
            }
            catch (Exception e)
            {
                string erro = e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message;
            }

            semaforoExecucao.Release(1);

        }


        private List<dynamic> obtemListaComMudancas(SqlNotificationInfo Info = SqlNotificationInfo.Unknown)
        {
            List<dynamic> mudancas = new List<dynamic>();

            if (token == null || !Permissoes.Autenticado(token))
                return mudancas;

            semaforo.WaitOne();

            List<FilaBootICard> oldList = list != null ? list : new List<FilaBootICard>();

            initList();

            DateTime dtOut = DateTime.Now;

            if (list != null)
            {
                try
                {
                    if (Info.Equals(SqlNotificationInfo.Delete))
                        // Delete
                        mudancas.Add(new { NotificationInfo = Info.ToString().ToUpper(),
                                           objetos = oldList.Where(e => !list.Any(l => l.id == e.id))
                                                            .ToList<FilaBootICard>()
                                          });
                    else
                        // Insert, Update
                        mudancas.Add(new { NotificationInfo = Info.ToString().ToUpper(),
                                           objetos = list.Where(e => !oldList.Any(l => l.id == e.id && l.stProcessamento == e.stProcessamento &&
                                                                                       l.dtInsert == e.dtInsert && l.webpages_UsersInsert.id_users == e.webpages_UsersInsert.id_users &&
                                                                                      ((l.cdUser == null && e.cdUser == null) || (l.cdUser != null && e.cdUser != null && l.cdUser.Value == e.cdUser.Value)) &&//((l.webpages_Users == null && e.webpages_Users == null) || (l.webpages_Users != null && e.webpages_Users != null && l.webpages_Users.id_users == e.webpages_Users.id_users)) &&
                                                                                      ((l.dtProcessamento == null && e.dtProcessamento == null) || (l.dtProcessamento != null && e.dtProcessamento != null && l.dtProcessamento.Value.Equals(e.dtProcessamento.Value)))))
                                                         .ToList<FilaBootICard>()
                                         });
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
            if(list != null)
                context.Clients.Client(connectionId).enviaLista(list);
            semaforo.Release(1);
        }



        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency dep = (SqlDependency)sender;
            dep.OnChange -= dependency_OnChange;

            registraEventoNotificacao();

            if (e.Info.Equals(SqlNotificationInfo.Insert) ||
                e.Info.Equals(SqlNotificationInfo.Update) ||
                e.Info.Equals(SqlNotificationInfo.Delete))
            {
                // Só envia se de fato tiveram mudanças para o filtro selecionado
                List<dynamic> mudancas = obtemListaComMudancas(e.Info);
                if (mudancas.Count > 0)
                {
                    semaforo.WaitOne();
                    context.Clients.Client(connectionId).enviaLista(list);//enviaMudancas(mudancas[0]);
                    semaforo.Release(1);
                }
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