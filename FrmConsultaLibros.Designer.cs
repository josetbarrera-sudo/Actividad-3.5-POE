namespace Biblioteca
{
    partial class FrmConsultaLibros
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
            pnlSuperior = new Panel();
            lblTotal = new Label();
            btnCerrar = new Button();
            btnActualizar = new Button();
            lvLibros = new ListView();
            colId = new ColumnHeader();
            colTitulo = new ColumnHeader();
            colPrecio = new ColumnHeader();
            colNovedad = new ColumnHeader();
            colEstado = new ColumnHeader();
            lblAutoria = new Label();
            pnlSuperior.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSuperior
            // 
            pnlSuperior.Controls.Add(lblTotal);
            pnlSuperior.Controls.Add(btnCerrar);
            pnlSuperior.Controls.Add(btnActualizar);
            pnlSuperior.Dock = DockStyle.Top;
            pnlSuperior.Location = new Point(0, 0);
            pnlSuperior.Name = "pnlSuperior";
            pnlSuperior.Size = new Size(640, 56);
            pnlSuperior.TabIndex = 0;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(240, 18);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(150, 20);
            lblTotal.TabIndex = 2;
            lblTotal.Text = "Libros registrados: 0";
            // 
            // btnCerrar
            // 
            btnCerrar.Location = new Point(120, 14);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(94, 29);
            btnCerrar.TabIndex = 1;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // btnActualizar
            // 
            btnActualizar.Location = new Point(12, 14);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.Size = new Size(100, 29);
            btnActualizar.TabIndex = 0;
            btnActualizar.Text = "Actualizar";
            btnActualizar.UseVisualStyleBackColor = true;
            btnActualizar.Click += btnActualizar_Click;
            // 
            // lvLibros
            // 
            lvLibros.Columns.AddRange(new ColumnHeader[] { colId, colTitulo, colPrecio, colNovedad, colEstado });
            lvLibros.Dock = DockStyle.Fill;
            lvLibros.FullRowSelect = true;
            lvLibros.GridLines = true;
            lvLibros.Location = new Point(0, 56);
            lvLibros.Name = "lvLibros";
            lvLibros.Size = new Size(640, 258);
            lvLibros.TabIndex = 1;
            lvLibros.UseCompatibleStateImageBehavior = false;
            lvLibros.View = View.Details;
            // 
            // colId
            // 
            colId.Text = "ID";
            colId.Width = 60;
            // 
            // colTitulo
            // 
            colTitulo.Text = "Título";
            colTitulo.Width = 260;
            // 
            // colPrecio
            // 
            colPrecio.Text = "Precio/día";
            colPrecio.Width = 100;
            // 
            // colNovedad
            // 
            colNovedad.Text = "Novedad";
            colNovedad.Width = 90;
            // 
            // colEstado
            // 
            colEstado.Text = "Estado";
            colEstado.Width = 100;
            // 
            // lblAutoria
            // 
            lblAutoria.BorderStyle = BorderStyle.FixedSingle;
            lblAutoria.Dock = DockStyle.Bottom;
            lblAutoria.Location = new Point(0, 314);
            lblAutoria.Name = "lblAutoria";
            lblAutoria.Size = new Size(640, 86);
            lblAutoria.TabIndex = 2;
            lblAutoria.Text = "Equipo 3\r\nCONTRERAS Rodríguez Janis Isabel\r\nMACÍAS Cruz Meredith Miranda\r\nTORRES Barrera José Ángel";
            lblAutoria.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FrmConsultaLibros
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 400);
            Controls.Add(lvLibros);
            Controls.Add(lblAutoria);
            Controls.Add(pnlSuperior);
            MinimumSize = new Size(520, 360);
            Name = "FrmConsultaLibros";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Consulta de libros";
            pnlSuperior.ResumeLayout(false);
            pnlSuperior.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlSuperior;
        private Label lblTotal;
        private Button btnCerrar;
        private Button btnActualizar;
        private ListView lvLibros;
        private ColumnHeader colId;
        private ColumnHeader colTitulo;
        private ColumnHeader colPrecio;
        private ColumnHeader colNovedad;
        private ColumnHeader colEstado;
        private Label lblAutoria;
    }
}
