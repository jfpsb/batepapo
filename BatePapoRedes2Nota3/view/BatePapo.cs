using BatePapoRedes2Nota3.controller;
using BatePapoRedes2Nota3.modelo;
using BatePapoRedes2Nota3.view;
using System;
using System.Windows.Forms;

namespace BatePapoRedes2Nota3
{
    /// <summary>
    /// View da tela de bate papo
    /// </summary>
    public partial class BatePapo : Form, IBatePapoView
    {
        public BatePapo()
        {
            InitializeComponent();
        }

        private BatePapoController batePapoController;

        private void BatePapo_Load(object sender, EventArgs e)
        {
            batePapoController.Conectar();
        }

        private void BatePapo_FormClosing(object sender, FormClosingEventArgs e)
        {
            batePapoController.FecharTelaBatePapo();
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            batePapoController.EnviarMensagem(txtMensagem.Text);
        }

        private void txtMensagem_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                batePapoController.EnviarMensagem(txtMensagem.Text);
            }
        }

        public void SetUsuario(Usuario usuario)
        {
            batePapoController = new BatePapoController(this);
            batePapoController.SetUsuario(usuario);
        }

        public void AlterarTextoTxtAviso(String texto)
        {
            txtAviso.Invoke(new MethodInvoker(() =>
            {
                txtAviso.Text = texto;
            }));
        }

        public void AdicionarComandoDataGridView(String usuario, String mensagem)
        {
            dgvBatePapo.Invoke(new MethodInvoker(() =>
            {
                dgvBatePapo.Rows.Add(usuario, mensagem);
                dgvBatePapo.FirstDisplayedScrollingRowIndex = dgvBatePapo.Rows.Count - 1;
            }));
        }

        public void LimparMensagem()
        {
            txtMensagem.Clear();
        }

        public void ConfiguraTituloBatePapo(string usuario)
        {
            Text += usuario;
        }

        public void Fechar()
        {
            Close();
        }

        private void abrirOutroBatePapoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            batePapoController.MenuStripAbrirOutroBatePapo();
        }
    }
}
