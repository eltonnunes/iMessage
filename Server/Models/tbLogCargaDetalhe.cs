﻿using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class tbLogCargaDetalhe
    {
        public int idLogCargaDetalhe { get; set; }
        public int idLogCarga { get; set; }
        public System.DateTime dtExecucaoIni { get; set; }
        public Nullable<System.DateTime> dtExecucaoFim { get; set; }
        public byte flStatus { get; set; }
        public string dsMensagem { get; set; }
        public string dsModalidade { get; set; }
        public Nullable<int> qtTransacoes { get; set; }
        public Nullable<decimal> vlTotalProcessado { get; set; }
        public Nullable<int> qtTransacoesCS { get; set; }
        public Nullable<decimal> vlTotalProcessadoCS { get; set; }
        public string txAuditoria { get; set; }
        public virtual tbLogCarga tbLogCargas { get; set; }
    }
}