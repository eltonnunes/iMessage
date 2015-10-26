using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Object
{
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
}
