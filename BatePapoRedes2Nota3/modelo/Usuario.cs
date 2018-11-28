using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace BatePapoRedes2Nota3.modelo
{
    public class Usuario
    {
        public String login { get; set; }
        public String senha { get; set; }

        public Usuario()
        {

        }

        public Usuario(String login)
        {
            this.login = login;
        }

        public Usuario[] CarregaUsuarios()
        {
            Usuario[] usuarios = null;
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var usuariosDicionario = serializer.Deserialize<Dictionary<String, Usuario[]>>(File.ReadAllText("../../modelo/usuarios.json"));

            foreach (KeyValuePair<string, Usuario[]> entrada in usuariosDicionario)
            {
                usuarios = entrada.Value;
            }

            return usuarios;
        }

        public void CarregaUsuarioPorLogin(String login)
        {
            Usuario[] usuarios = CarregaUsuarios();

            foreach(Usuario usuario in usuarios)
            {
                if(usuario.login.Equals(login))
                {
                    this.login = usuario.login;
                    break;
                }
            }
        }

        public Boolean Autenticar(String login, String senha)
        {
            Usuario[] usuarios = CarregaUsuarios();

            foreach(Usuario usuario in usuarios)
            {
                if(usuario.login.Equals(login))
                {
                    if(usuario.senha.Equals(senha))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
