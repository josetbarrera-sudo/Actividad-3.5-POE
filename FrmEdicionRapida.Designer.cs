namespace Biblioteca
{
    partial class FrmEdicionRapida
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lblTitulo = new Label();
            lblId = new Label();
            txtIdLibro = new TextBox();
            lblPrecio = new Label();
            txtNuevoPrecio = new TextBox();
            btnAceptar = new Button();
            btnCancelar = new Button();
            lblAutoria = new Label();
            errorProvider1 = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.Location = new Point(20, 16);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(235, 28);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Edición rápida de precio";
            // 
            // lblId
            // 
            lblId.AutoSize = true;
            lblId.Location = new Point(20, 66);
            lblId.Name = "lblId";
            lblId.Size = new Size(79, 20);
            lblId.TabIndex = 1;
            lblId.Text = "ID del libro:";
            // 
            // txtIdLibro
            // 
            txtIdLibro.Location = new Point(150, 63);
            txtIdLibro.Name = "txtIdLibro";
            txtIdLibro.Size = new Size(190, 27);
            txtIdLibro.TabIndex = 0;
            // 
            // lblPrecio
            // 
            lblPrecio.AutoSize = true;
            lblPrecio.Location = new Point(20, 106);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new Size(104, 20);
            lblPrecio.TabIndex = 3;
            lblPrecio.Text = "Nuevo precio/día:";
            // 
            // txtNuevoPrecio
            // 
            txtNuevoPrecio.Location = new Point(150, 103);
            txtNuevoPrecio.Name = "txtNuevoPrecio";
            txtNuevoPrecio.Size = new Size(190, 27);
            txtNuevoPrecio.TabIndex = 1;
            // 
            // btnAceptar
            // 
            btnAceptar.DialogResult = DialogResult.OK;
            btnAceptar.Location = new Point(150, 150);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(94, 32);
            btnAceptar.TabIndex = 2;
            btnAceptar.Text = "Aceptar";
            btnAceptar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            btnCancelar.DialogResult = DialogResult.Cancel;
            btnCancelar.Location = new Point(250, 150);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 32);
            btnCancelar.TabIndex = 3;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // lblAutoria
            // 
            lblAutoria.BorderStyle = BorderStyle.FixedSingle;
            lblAutoria.Dock = DockStyle.Bottom;
            lblAutoria.Location = new Point(0, 205);
            lblAutoria.Name = "lblAutoria";
            lblAutoria.Size = new Size(364, 86);
            lblAutoria.TabIndex = 7;
            lblAutoria.Text = "Equipo 3\r\nCONTRERAS Rodríguez Janis Isabel\r\nMACÍAS Cruz Meredith Miranda\r\nTORRES Barrera José Ángel";
            lblAutoria.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // FrmEdicionRapida
            // 
            AcceptButton = btnAceptar;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancelar;
            ClientSize = new Size(364, 291);
            Controls.Add(lblAutoria);
            Controls.Add(btnCancelar);
            Controls.Add(btnAceptar);
            Controls.Add(txtNuevoPrecio);
            Controls.Add(lblPrecio);
            Controls.Add(txtIdLibro);
            Controls.Add(lblId);
            Controls.Add(lblTitulo);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmEdicionRapida";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Edición rápida";
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitulo;
        private Label lblId;
        private TextBox txtIdLibro;
        private Label lblPrecio;
        private TextBox txtNuevoPrecio;
        private Button btnAceptar;
        private Button btnCancelar;
        private Label lblAutoria;
        private ErrorProvider errorProvider1;
    }
}
