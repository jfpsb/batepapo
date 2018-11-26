using System;

namespace BatePapoRedes2Nota3.data
{
    public class Usuario
    {
        public String login { get; set; }
        public String senha { get; set; }

        public Usuario()
        {

        }

        public Usuario(String login, String senha)
        {
            this.login = login;
            this.senha = senha;
        }
    }
}
