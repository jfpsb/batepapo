using BatePapoRedes2Nota3.controller;
using BatePapoRedes2Nota3.modelo;
using BatePapoRedes2Nota3.view;
using System;
using System.Windows.Forms;

namespace BatePapoRedes2Nota3
{
    public partial class Login : Form, ILoginView
    {
        public Login()
        {
            InitializeComponent();
        }

        private TelaLoginController telaLoginController;

        public Usuario Usuario;

        private void Login_Load(object sender, EventArgs e)
        {
            telaLoginController = new TelaLoginController(this);
        }

        private void btnAutenticar_Click(object sender, EventArgs e)
        {
            String login = txtLogin.Text;
            String senha = txtSenha.Text;
            String host = txtIp.Text;

            telaLoginController.Autenticar(login, senha, host);
        }

        public void LoginRecusado(string resultado)
        {
            MessageBox.Show(resultado);
        }

        public void LoginAprovado(Usuario usuario)
        {
            Usuario = usuario;
            DialogResult = DialogResult.OK;
        }

        public void PopulaDataGridView(Usuario[] usuarios)
        {
            foreach (Usuario u in usuarios)
            {
                dgvLogin.Rows.Add(u.login, u.senha);
            }
        }

        public void ResultadoConectar(String mensagem)
        {
            DialogResult result = MessageBox.Show(mensagem, "Erro", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                String host = txtIp.Text;
                telaLoginController.Conectar(host);
            }
        }
    }
}
