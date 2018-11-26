using BatePapoRedes2Nota3.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace BatePapoRedes2Nota3
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private byte[] recebido_do_servidor = new byte[100];
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Usuario[] usuarios_cadastrados = new Usuario[10];
        private String Login_Usuario;

        private void Login_Load(object sender, EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var resultado = serializer.Deserialize<Dictionary<String, Usuario[]>>(File.ReadAllText("../../data/usuarios.json"));

            foreach (KeyValuePair<string, Usuario[]> entry in resultado)
            {
                usuarios_cadastrados = entry.Value;
            }

            foreach(Usuario u in usuarios_cadastrados)
            {
                dgvLogin.Rows.Add(u.login, u.senha);
            }
        }

        private Boolean Conectar(String ip)
        {
            if (! socket.Connected)
            {
                try
                {
                    socket.Connect(IPAddress.Parse(ip), 3500);
                    return true;
                }
                catch (SocketException se)
                {
                    DialogResult result = MessageBox.Show("Erro ao conectar com servidor da aplicação. Deseja tentar novamente?\n\n" + se.Message, "Erro", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Conectar(ip);
                    }
                    else
                    {
                        Close();
                    }
                }

                return false;
            }

            return true;
        }

        private void btnAutenticar_Click(object sender, EventArgs e)
        {
            Boolean conectado = Conectar(txtIp.Text);

            if (conectado)
            {
                String login = txtLogin.Text;
                String senha = txtSenha.Text;

                String requisicao_do_cliente = "login " + login + " " + senha;

                byte[] requisicao_ao_servidor = Servidor.RetornaEmByteArray(requisicao_do_cliente);

                socket.Send(requisicao_ao_servidor);

                int tamanho_recebido_do_servidor = socket.Receive(recebido_do_servidor);

                byte[] recebido_do_servidor_tamanho_exato = new byte[tamanho_recebido_do_servidor];

                Array.Copy(recebido_do_servidor, recebido_do_servidor_tamanho_exato, tamanho_recebido_do_servidor);

                String resposta_do_servidor = Servidor.RetornaEmString(recebido_do_servidor_tamanho_exato);

                switch(resposta_do_servidor)
                {
                    case "ok":
                        Login_Usuario = login;
                        DialogResult = DialogResult.OK;
                        break;
                    case "usuarionaoexiste":
                        MessageBox.Show("Usuário não encontrado");
                        break;
                    case "senhaerrada":
                        MessageBox.Show("Senha errada");
                        break;
                    default:
                        MessageBox.Show(resposta_do_servidor);
                        break;

                }
            }
        }

        public String LoginUsuario()
        {
            return Login_Usuario;
        }
    }
}
