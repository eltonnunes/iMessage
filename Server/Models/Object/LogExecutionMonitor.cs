using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Object
{
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
}
