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
}
