using BatePapoRedes2Nota3.controller.util;
using BatePapoRedes2Nota3.modelo;
using BatePapoRedes2Nota3.view;
using System;
using System.Net;
using System.Net.Sockets;

namespace BatePapoRedes2Nota3.controller
{
    class TelaLoginController
    {
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Usuario usuario = new Usuario();
        private ILoginView loginView;
        private byte[] recebido_do_servidor = new byte[100];
        private Usuario[] usuarios_cadastrados = new Usuario[10];

        public TelaLoginController(ILoginView loginView)
        {
            this.loginView = loginView;
            usuarios_cadastrados = usuario.CarregaUsuarios();
            loginView.PopulaDataGridView(usuarios_cadastrados);
        }

        public void Autenticar(String login, String senha, String host)
        {
            Boolean conectado = Conectar(host);

            if (conectado)
            {
                String requisicao_do_cliente = "login " + login + " " + senha;

                byte[] requisicao_ao_servidor = Util.RetornaEmByteArray(requisicao_do_cliente);

                socket.Send(requisicao_ao_servidor);

                int tamanho_recebido_do_servidor = socket.Receive(recebido_do_servidor);

                byte[] recebido_do_servidor_tamanho_exato = new byte[tamanho_recebido_do_servidor];

                Array.Copy(recebido_do_servidor, recebido_do_servidor_tamanho_exato, tamanho_recebido_do_servidor);

                String resposta_do_servidor = Util.RetornaEmString(recebido_do_servidor_tamanho_exato);

                switch (resposta_do_servidor)
                {
                    case "ok":
                        loginView.LoginAprovado(new Usuario(login));
                        break;
                    case "notok":
                        loginView.LoginRecusado("Usuário ou senha inválidos!");
                        break;
                    default:
                        loginView.LoginRecusado(resposta_do_servidor);
                        break;

                }
            }
        }

        public Boolean Conectar(String ip)
        {
            if (!socket.Connected)
            {
                try
                {
                    socket.Connect(IPAddress.Parse(ip), 3500);
                    return true;
                }
                catch (SocketException se)
                {
                    String mensagem = "Erro ao conectar com servidor da aplicação. Deseja tentar novamente?\n\n" + se.Message;
                    loginView.ResultadoConectar(mensagem);
                }

                return false;
            }

            return true;
        }
    }
}
