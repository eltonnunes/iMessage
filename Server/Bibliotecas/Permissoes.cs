using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Bibliotecas
{
    public class Permissoes
    {

        public static bool Autenticado(string token)
        {
            if (token == null) return false;
 
            using (var _db = new painel_taxservices_dbContext())
            {

                _db.Configuration.ProxyCreationEnabled = false;

                var verify = _db.LoginAutenticacaos.Where(v => v.token.Equals(token)).Select(v => v).FirstOrDefault();

                if (verify != null)
                    return true;
            }
            return false;
        }


        public static webpages_Users GetUser(string token)
        {
            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                return _db.LoginAutenticacaos.Where(v => v.token.Equals(token))
                            .Select(v => v.webpages_Users)
                            .FirstOrDefault<webpages_Users>();
            }
            //return _db.LoginAutenticacaos.Where(v => v.token.Equals(token)).Select(v => v.webpages_Users).FirstOrDefault();
        }

        public static Int32 GetIdGrupo(string token)
        {
            webpages_Users user = GetUser(token);
            if (user != null && user.id_grupo != null) return (Int32)user.id_grupo;
            return 0;
        }

        public static string GetCNPJEmpresa(string token)
        {
            webpages_Users user = GetUser(token);
            if (user != null && user.id_grupo != null && user.nu_cnpjEmpresa != null) return user.nu_cnpjEmpresa;
            return "";
        }

        /**
         * Retorna true se o privilégio do usuário logado é de vendedor da ATOS
         */
        public static bool isAtosRoleVendedor(string token)
        {
            webpages_Users user = GetUser(token);
            if (user == null) return false;

            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                webpages_Roles role =  _db.webpages_UsersInRoles
                                        .Where(r => r.UserId == user.id_users)
                                        .Where(r => r.RoleId > 50)
                                        .Select(r => r.webpages_Roles)
                                        .FirstOrDefault();

                if (role == null) return false;

                return role.RoleLevel >= 0 && role.RoleLevel <= 2 && role.RoleName.ToUpper().Equals("COMERCIAL");
            }
        }

        public static List<Int32> GetIdsGruposEmpresasVendedor(string token)
        {
            List<Int32> lista = new List<Int32>();

            webpages_Users user = GetUser(token);

            if (user == null) return lista;

            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                lista = _db.grupo_empresa
                        .Where(g => g.id_vendedor == user.id_users)
                        .Select(g => g.id_grupo)
                        .ToList<Int32>();

                return lista;
            }
        }
    }
}
