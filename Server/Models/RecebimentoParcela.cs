using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class RecebimentoParcela
    {
        public int idRecebimento { get; set; }
        public int numParcela { get; set; }
        public decimal valorParcelaBruta { get; set; }
        public Nullable<decimal> valorParcelaLiquida { get; set; }
        public System.DateTime dtaRecebimento { get; set; }
        public decimal valorDescontado { get; set; }
    }
}