namespace BatePapoRedes2Nota3
{
    partial class BatePapo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.dgvBatePapo = new System.Windows.Forms.DataGridView();
            this.enviadapor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mensagem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtMensagem = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.opçõesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirOutroBatePapoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtAviso = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatePapo)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEnviar
            // 
            this.btnEnviar.Location = new System.Drawing.Point(502, 257);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(79, 106);
            this.btnEnviar.TabIndex = 1;
            this.btnEnviar.Text = "Enviar";
            this.btnEnviar.UseVisualStyleBackColor = true;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);
            // 
            // dgvBatePapo
            // 
            this.dgvBatePapo.AllowUserToAddRows = false;
            this.dgvBatePapo.AllowUserToDeleteRows = false;
            this.dgvBatePapo.AllowUserToResizeColumns = false;
            this.dgvBatePapo.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBatePapo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBatePapo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBatePapo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.enviadapor,
            this.mensagem});
            this.dgvBatePapo.Location = new System.Drawing.Point(13, 63);
            this.dgvBatePapo.Name = "dgvBatePapo";
            this.dgvBatePapo.RowHeadersVisible = false;
            this.dgvBatePapo.Size = new System.Drawing.Size(568, 188);
            this.dgvBatePapo.TabIndex = 2;
            // 
            // enviadapor
            // 
            this.enviadapor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.enviadapor.HeaderText = "Enviada Por";
            this.enviadapor.Name = "enviadapor";
            this.enviadapor.ReadOnly = true;
            this.enviadapor.Width = 119;
            // 
            // mensagem
            // 
            this.mensagem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.mensagem.HeaderText = "Mensagem";
            this.mensagem.Name = "mensagem";
            this.mensagem.ReadOnly = true;
            // 
            // txtMensagem
            // 
            this.txtMensagem.Location = new System.Drawing.Point(13, 257);
            this.txtMensagem.MaxLength = 1000;
            this.txtMensagem.Multiline = true;
            this.txtMensagem.Name = "txtMensagem";
            this.txtMensagem.Size = new System.Drawing.Size(483, 106);
            this.txtMensagem.TabIndex = 3;
            this.txtMensagem.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtMensagem_KeyUp);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opçõesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(594, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // opçõesToolStripMenuItem
            // 
            this.opçõesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abrirOutroBatePapoToolStripMenuItem});
            this.opçõesToolStripMenuItem.Name = "opçõesToolStripMenuItem";
            this.opçõesToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.opçõesToolStripMenuItem.Text = "Opções";
            // 
            // abrirOutroBatePapoToolStripMenuItem
            // 
            this.abrirOutroBatePapoToolStripMenuItem.Name = "abrirOutroBatePapoToolStripMenuItem";
            this.abrirOutroBatePapoToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.abrirOutroBatePapoToolStripMenuItem.Text = "Abrir Outro Bate Papo";
            this.abrirOutroBatePapoToolStripMenuItem.Click += new System.EventHandler(this.abrirOutroBatePapoToolStripMenuItem_Click);
            // 
            // txtAviso
            // 
            this.txtAviso.Location = new System.Drawing.Point(70, 31);
            this.txtAviso.Name = "txtAviso";
            this.txtAviso.ReadOnly = true;
            this.txtAviso.Size = new System.Drawing.Size(511, 26);
            this.txtAviso.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Aviso:";
            // 
            // BatePapo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 374);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtAviso);
            this.Controls.Add(this.txtMensagem);
            this.Controls.Add(this.dgvBatePapo);
            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BatePapo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bate Papo - ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BatePapo_FormClosing);
            this.Load += new System.EventHandler(this.BatePapo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatePapo)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.DataGridView dgvBatePapo;
        private System.Windows.Forms.DataGridViewTextBoxColumn enviadapor;
        private System.Windows.Forms.DataGridViewTextBoxColumn mensagem;
        private System.Windows.Forms.TextBox txtMensagem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem opçõesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abrirOutroBatePapoToolStripMenuItem;
        private System.Windows.Forms.TextBox txtAviso;
        private System.Windows.Forms.Label label1;
    }
}

