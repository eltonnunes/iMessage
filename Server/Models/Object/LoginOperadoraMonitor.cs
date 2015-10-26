using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Object
{
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
}
