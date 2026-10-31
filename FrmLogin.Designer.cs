namespace Biblioteca
{
    partial class FrmLogin
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
            lblTitulo = new Label();
            lblCorreo = new Label();
            lblContrasena = new Label();
            txtCorreo = new TextBox();
            txtContrasena = new TextBox();
            btnIngresar = new Button();
            btnCancelar = new Button();
            lblAutoria = new Label();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.Location = new Point(28, 22);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(182, 30);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Inicio de sesión";
            // 
            // lblCorreo
            // 
            lblCorreo.AutoSize = true;
            lblCorreo.Location = new Point(30, 74);
            lblCorreo.Name = "lblCorreo";
            lblCorreo.Size = new Size(57, 20);
            lblCorreo.TabIndex = 1;
            lblCorreo.Text = "Correo:";
            // 
            // txtCorreo
            // 
            txtCorreo.Location = new Point(30, 98);
            txtCorreo.Name = "txtCorreo";
            txtCorreo.Size = new Size(320, 27);
            txtCorreo.TabIndex = 2;
            // 
            // lblContrasena
            // 
            lblContrasena.AutoSize = true;
            lblContrasena.Location = new Point(30, 140);
            lblContrasena.Name = "lblContrasena";
            lblContrasena.Size = new Size(88, 20);
            lblContrasena.TabIndex = 3;
            lblContrasena.Text = "Contraseña:";
            // 
            // txtContrasena
            // 
            txtContrasena.Location = new Point(30, 164);
            txtContrasena.Name = "txtContrasena";
            txtContrasena.Size = new Size(320, 27);
            txtContrasena.TabIndex = 4;
            txtContrasena.UseSystemPasswordChar = true;
            // 
            // btnIngresar
            // 
            btnIngresar.Location = new Point(130, 214);
            btnIngresar.Name = "btnIngresar";
            btnIngresar.Size = new Size(104, 34);
            btnIngresar.TabIndex = 5;
            btnIngresar.Text = "Ingresar";
            btnIngresar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(246, 214);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(104, 34);
            btnCancelar.TabIndex = 6;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // lblAutoria
            // 
            lblAutoria.BorderStyle = BorderStyle.FixedSingle;
            lblAutoria.Dock = DockStyle.Bottom;
            lblAutoria.Location = new Point(0, 272);
            lblAutoria.Name = "lblAutoria";
            lblAutoria.Size = new Size(380, 86);
            lblAutoria.TabIndex = 7;
            lblAutoria.Text = "Equipo 3\r\nCONTRERAS Rodríguez Janis Isabel\r\nMACÍAS Cruz Meredith Miranda\r\nTORRES Barrera José Ángel";
            lblAutoria.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FrmLogin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(380, 358);
            Controls.Add(lblAutoria);
            Controls.Add(btnCancelar);
            Controls.Add(btnIngresar);
            Controls.Add(txtContrasena);
            Controls.Add(lblContrasena);
            Controls.Add(txtCorreo);
            Controls.Add(lblCorreo);
            Controls.Add(lblTitulo);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Biblioteca - Iniciar sesión";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitulo;
        private Label lblCorreo;
        private Label lblContrasena;
        private TextBox txtCorreo;
        private TextBox txtContrasena;
        private Button btnIngresar;
        private Button btnCancelar;
        private Label lblAutoria;
    }
}
