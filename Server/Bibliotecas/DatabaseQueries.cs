using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Bibliotecas
{
    public class DatabaseQueries
    {
        public static string GetDate(DateTime data)
        {
            if (data == null) return "";
            string ano = data.Year.ToString("0000");
            string mes = data.Month.ToString("00");
            string dia = data.Day.ToString("00");
            return ano + "-" + mes + "-" + dia;
        }

        public static int GetBoolean(bool valor)
        {
            return valor ? 1 : 0;
        }
    }
}