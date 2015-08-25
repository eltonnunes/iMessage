using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class Grupo
    {
        public Grupo()
        {
            this.usuarios = new List<usuario>();
        }

        public string Cod_Grupo { get; set; }
        public string Descr_Grupo { get; set; }
        public virtual ICollection<usuario> usuarios { get; set; }
    }
}
