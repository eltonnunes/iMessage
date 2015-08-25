using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class tbExecucaoLog
    {
        public int cdExecucaoLog { get; set; }
        public int cdLoginAdquirenteEmpresa { get; set; }
        public System.DateTime dtTransacaoFiltroInicio { get; set; }
        public System.DateTime dtTransacaoFiltroFinal { get; set; }
        public int qtTransacao { get; set; }
        public decimal vlTransacaoTotal { get; set; }
        public byte stExecucao { get; set; }
        public Nullable<System.DateTime> dtExecucaoInicio { get; set; }
        public Nullable<System.DateTime> dtExecucaoFim { get; set; }
        public System.DateTime dtExecucaoProxima { get; set; }
        public Nullable<int> idLogExecution { get; set; }
        public virtual tbLoginAdquirenteEmpresa tbLoginAdquirenteEmpresa { get; set; }
    }
}
