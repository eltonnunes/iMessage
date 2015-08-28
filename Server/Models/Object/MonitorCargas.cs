using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Object
{
    public class GrupoEmpresaMonitor
    {
        private Int32 id;
        public Int32 id_grupo
        {
            get { return id; }
            set { id = value; }
        }

        private string nome;
        public string ds_nome
        {
            get { return nome; }
            set { nome = value; }
        }
    }

    public class EmpresaMonitor
    {
        private string cnpj;
        public string nu_cnpj
        {
            get { return cnpj; }
            set { cnpj = value; }
        }

        private string fantasia;
        public string ds_fantasia
        {
            get { return fantasia; }
            set { fantasia = value; }
        }

        private string filialnum;
        public string filial
        {
            get { return filialnum; }
            set { filialnum = value; }
        }
    }

    public class OperadoraMonitor
    {
        private Int32 idOp;
        public Int32 id
        {
            get { return idOp; }
            set { idOp = value; }
        }

        private string nome;
        public string nmOperadora
        {
            get { return nome; }
            set { nome = value; }
        }
    }

    public class LoginOperadoraMonitor
    {
        private Int32 idLogOp;
        public Int32 id
        {
            get { return idLogOp; }
            set { idLogOp = value; }
        }

        private bool stat;
        public bool status
        {
            get { return stat; }
            set { stat = value; }
        }
    }

    public class LogExecutionMonitor
    {
        private Int32 idLog;
        public Int32 id
        {
            get { return idLog; }
            set { idLog = value; }
        }

        private DateTime dtaFiltro;
        public DateTime dtaFiltroTransacoes
        {
            get { return dtaFiltro; }
            set { dtaFiltro = value; }
        }

        private string status;
        public string statusExecution
        {
            get { return status; }
            set { status = value; }
        }

        private Nullable<DateTime> dtaExecucaoF;
        public Nullable<DateTime> dtaExecucaoFim
        {
            get { return dtaExecucaoF; }
            set { dtaExecucaoF = value; }
        }

        private Nullable<DateTime> dtaExecucaoP;
        public Nullable<DateTime> dtaExecucaoProxima
        {
            get { return dtaExecucaoP; }
            set { dtaExecucaoP = value; }
        }
    }

    public class MonitorCargas
    {
        private Int32 idLogExecution;
        public Int32 id
        {
            get { return idLogExecution; }
            set { idLogExecution = value; }
        }

        private DateTime dtaFiltro;
        public DateTime dtaFiltroTransacoes
        {
            get { return dtaFiltro; }
            set { dtaFiltro = value; }
        }

        private string status;
        public string statusExecution
        {
            get { return status; }
            set { status = value; }
        }

        private Nullable<DateTime> dtaExecucaoF;
        public Nullable<DateTime> dtaExecucaoFim
        {
            get { return dtaExecucaoF; }
            set { dtaExecucaoF = value; }
        }

        private Nullable<DateTime> dtaExecucaoP;
        public Nullable<DateTime> dtaExecucaoProxima
        {
            get { return dtaExecucaoP; }
            set { dtaExecucaoP = value; }
        }

        private LoginOperadoraMonitor logOp;
        public LoginOperadoraMonitor loginOperadora
        {
            get { return logOp; }
            set { logOp = value; }
        }

        private Nullable<DateTime> ultimaDataExec;
        public Nullable<DateTime> ultimaDataExecucaoFim
        {
            get { return ultimaDataExec; }
            set { ultimaDataExec = value; }
        }

        private GrupoEmpresaMonitor grupo;
        public GrupoEmpresaMonitor grupoempresa
        {
            get { return grupo; }
            set { grupo = value; }
        }

        private EmpresaMonitor filial;
        public EmpresaMonitor empresa
        {
            get { return filial; }
            set { filial = value; }
        }

        private OperadoraMonitor adquirente;
        public OperadoraMonitor operadora
        {
            get { return adquirente; }
            set { adquirente = value; }
        }
    }


    public class MonitorCargasAgrupado
    {
        private Int32 Id;
        public Int32 id
        {
            get { return Id; }
            set { Id = value; }
        }

        private bool stat;
        public bool status
        {
            get { return stat; }
            set { stat = value; }
        }

        private List<LogExecutionMonitor> logExec;
        public List<LogExecutionMonitor> logExecution
        {
            get { return logExec; }
            set { logExec = value; }
        }

        private int prioridadeStatus;
        public int prioridade
        {
            get { return prioridadeStatus; }
            set { prioridadeStatus = value; }
        }

        private Nullable<DateTime> ultimaDataExec;
        public Nullable<DateTime> ultimaDataExecucaoFim
        {
            get { return ultimaDataExec; }
            set { ultimaDataExec = value; }
        }

        private GrupoEmpresaMonitor grupo;
        public GrupoEmpresaMonitor grupoempresa
        {
            get { return grupo; }
            set { grupo = value; }
        }

        private EmpresaMonitor filial;
        public EmpresaMonitor empresa
        {
            get { return filial; }
            set { filial = value; }
        }

        private OperadoraMonitor adquirente;
        public OperadoraMonitor operadora
        {
            get { return adquirente; }
            set { adquirente = value; }
        }
    }
}
