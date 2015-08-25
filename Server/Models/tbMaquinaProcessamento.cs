using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class tbMaquinaProcessamento
    {
        public int idLoginOperadora { get; set; }
        public string ip { get; set; }
        public virtual LoginOperadora LoginOperadora { get; set; }
    }
}
