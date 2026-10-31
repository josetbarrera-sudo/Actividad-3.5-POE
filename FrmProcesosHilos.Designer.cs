namespace Biblioteca
{
    partial class FrmProcesosHilos
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
            lblDescripcion = new Label();
            lblRegistros = new Label();
            nudRegistros = new NumericUpDown();
            btnIniciar = new Button();
            btnCancelar = new Button();
            btnEjecutarLineal = new Button();
            pbProgreso = new ProgressBar();
            lblPorcentaje = new Label();
            lblEstado = new Label();
            dgvResultados = new DataGridView();
            colNumero = new DataGridViewTextBoxColumn();
            colUsuario = new DataGridViewTextBoxColumn();
            colLibro = new DataGridViewTextBoxColumn();
            colDias = new DataGridViewTextBoxColumn();
            colMulta = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            grpResponsiva = new GroupBox();
            txtPrueba = new TextBox();
            btnContador = new Button();
            lblClics = new Label();
            lblReloj = new Label();
            btnCerrar = new Button();
            lblAutoria = new Label();
            tmrReloj = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)nudRegistros).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvResultados).BeginInit();
            grpResponsiva.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.Location = new Point(20, 12);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Reporte masivo de circulación (proceso en segundo plano)";
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new Point(20, 48);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.TabIndex = 1;
            lblDescripcion.Text = "Calcula días de retraso y multa de N préstamos (simulación de procesamiento masivo).";
            // 
            // lblRegistros
            // 
            lblRegistros.AutoSize = true;
            lblRegistros.Location = new Point(20, 90);
            lblRegistros.Name = "lblRegistros";
            lblRegistros.TabIndex = 2;
            lblRegistros.Text = "Registros a procesar:";
            // 
            // nudRegistros
            // 
            nudRegistros.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            nudRegistros.Location = new Point(185, 87);
            nudRegistros.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            nudRegistros.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            nudRegistros.Name = "nudRegistros";
            nudRegistros.Size = new Size(90, 27);
            nudRegistros.TabIndex = 3;
            nudRegistros.Value = new decimal(new int[] { 300, 0, 0, 0 });
            // 
            // btnIniciar
            // 
            btnIniciar.Location = new Point(300, 82);
            btnIniciar.Name = "btnIniciar";
            btnIniciar.Size = new Size(190, 36);
            btnIniciar.TabIndex = 4;
            btnIniciar.Text = "Iniciar (segundo plano)";
            btnIniciar.UseVisualStyleBackColor = true;
            btnIniciar.Click += btnIniciar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(500, 82);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(120, 36);
            btnCancelar.TabIndex = 5;
            btnCancelar.Text = "Detener";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Enabled = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnEjecutarLineal
            // 
            btnEjecutarLineal.Location = new Point(630, 82);
            btnEjecutarLineal.Name = "btnEjecutarLineal";
            btnEjecutarLineal.Size = new Size(330, 36);
            btnEjecutarLineal.TabIndex = 6;
            btnEjecutarLineal.Text = "Demo: sin hilos (la ventana se congela)";
            btnEjecutarLineal.UseVisualStyleBackColor = true;
            btnEjecutarLineal.Click += btnEjecutarLineal_Click;
            // 
            // pbProgreso
            // 
            pbProgreso.Location = new Point(20, 132);
            pbProgreso.Name = "pbProgreso";
            pbProgreso.Size = new Size(940, 28);
            pbProgreso.TabIndex = 7;
            // 
            // lblPorcentaje
            // 
            lblPorcentaje.AutoSize = true;
            lblPorcentaje.Location = new Point(20, 168);
            lblPorcentaje.Name = "lblPorcentaje";
            lblPorcentaje.TabIndex = 8;
            lblPorcentaje.Text = "0 %";
            // 
            // lblEstado
            // 
            lblEstado.Location = new Point(100, 168);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(860, 20);
            lblEstado.TabIndex = 9;
            lblEstado.Text = "Listo.";
            // 
            // dgvResultados
            // 
            dgvResultados.AllowUserToAddRows = false;
            dgvResultados.AllowUserToDeleteRows = false;
            dgvResultados.AllowUserToResizeRows = false;
            dgvResultados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResultados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvResultados.Columns.AddRange(new DataGridViewColumn[] { colNumero, colUsuario, colLibro, colDias, colMulta, colEstado });
            dgvResultados.Location = new Point(20, 198);
            dgvResultados.Name = "dgvResultados";
            dgvResultados.ReadOnly = true;
            dgvResultados.RowHeadersVisible = false;
            dgvResultados.RowHeadersWidth = 51;
            dgvResultados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResultados.Size = new Size(940, 290);
            dgvResultados.TabIndex = 10;
            // 
            // colNumero
            // 
            colNumero.FillWeight = 30F;
            colNumero.HeaderText = "#";
            colNumero.MinimumWidth = 6;
            colNumero.Name = "colNumero";
            colNumero.ReadOnly = true;
            // 
            // colUsuario
            // 
            colUsuario.FillWeight = 90F;
            colUsuario.HeaderText = "Usuario";
            colUsuario.MinimumWidth = 6;
            colUsuario.Name = "colUsuario";
            colUsuario.ReadOnly = true;
            // 
            // colLibro
            // 
            colLibro.FillWeight = 110F;
            colLibro.HeaderText = "Libro";
            colLibro.MinimumWidth = 6;
            colLibro.Name = "colLibro";
            colLibro.ReadOnly = true;
            // 
            // colDias
            // 
            colDias.FillWeight = 60F;
            colDias.HeaderText = "Días de retraso";
            colDias.MinimumWidth = 6;
            colDias.Name = "colDias";
            colDias.ReadOnly = true;
            // 
            // colMulta
            // 
            colMulta.FillWeight = 60F;
            colMulta.HeaderText = "Multa";
            colMulta.MinimumWidth = 6;
            colMulta.Name = "colMulta";
            colMulta.ReadOnly = true;
            // 
            // colEstado
            // 
            colEstado.FillWeight = 70F;
            colEstado.HeaderText = "Estado";
            colEstado.MinimumWidth = 6;
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            // 
            // grpResponsiva
            // 
            grpResponsiva.Controls.Add(txtPrueba);
            grpResponsiva.Controls.Add(btnContador);
            grpResponsiva.Controls.Add(lblClics);
            grpResponsiva.Controls.Add(lblReloj);
            grpResponsiva.Location = new Point(20, 500);
            grpResponsiva.Name = "grpResponsiva";
            grpResponsiva.Size = new Size(940, 110);
            grpResponsiva.TabIndex = 11;
            grpResponsiva.TabStop = false;
            grpResponsiva.Text = "Prueba de interfaz responsiva (la ventana NO debe congelarse)";
            // 
            // txtPrueba
            // 
            txtPrueba.Location = new Point(15, 40);
            txtPrueba.Name = "txtPrueba";
            txtPrueba.PlaceholderText = "Escribe aquí mientras corre el proceso";
            txtPrueba.Size = new Size(380, 27);
            txtPrueba.TabIndex = 0;
            // 
            // btnContador
            // 
            btnContador.Location = new Point(410, 35);
            btnContador.Name = "btnContador";
            btnContador.Size = new Size(140, 36);
            btnContador.TabIndex = 1;
            btnContador.Text = "Clic +1";
            btnContador.UseVisualStyleBackColor = true;
            btnContador.Click += btnContador_Click;
            // 
            // lblClics
            // 
            lblClics.AutoSize = true;
            lblClics.Location = new Point(565, 44);
            lblClics.Name = "lblClics";
            lblClics.TabIndex = 2;
            lblClics.Text = "Clics: 0";
            // 
            // lblReloj
            // 
            lblReloj.AutoSize = true;
            lblReloj.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblReloj.Location = new Point(700, 43);
            lblReloj.Name = "lblReloj";
            lblReloj.TabIndex = 3;
            lblReloj.Text = "00/00/0000 00:00:00";
            // 
            // btnCerrar
            // 
            btnCerrar.Location = new Point(840, 622);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(120, 36);
            btnCerrar.TabIndex = 12;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // lblAutoria
            // 
            lblAutoria.BorderStyle = BorderStyle.FixedSingle;
            lblAutoria.Dock = DockStyle.Bottom;
            lblAutoria.Location = new Point(0, 674);
            lblAutoria.Name = "lblAutoria";
            lblAutoria.Size = new Size(980, 86);
            lblAutoria.TabIndex = 13;
            lblAutoria.Text = "Equipo 3\r\nCONTRERAS Rodríguez Janis Isabel\r\nMACÍAS Cruz Meredith Miranda\r\nTORRES Barrera José Ángel";
            lblAutoria.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tmrReloj
            // 
            tmrReloj.Enabled = true;
            tmrReloj.Interval = 1000;
            tmrReloj.Tick += tmrReloj_Tick;
            // 
            // FrmProcesosHilos
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 760);
            Controls.Add(lblAutoria);
            Controls.Add(btnCerrar);
            Controls.Add(grpResponsiva);
            Controls.Add(dgvResultados);
            Controls.Add(lblEstado);
            Controls.Add(lblPorcentaje);
            Controls.Add(pbProgreso);
            Controls.Add(btnEjecutarLineal);
            Controls.Add(btnCancelar);
            Controls.Add(btnIniciar);
            Controls.Add(nudRegistros);
            Controls.Add(lblRegistros);
            Controls.Add(lblDescripcion);
            Controls.Add(lblTitulo);
            AutoScroll = true;
            AutoScrollMargin = new Size(0, 90);
            Name = "FrmProcesosHilos";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Actividad 3.4 - Procesos concurrentes e hilos de ejecución";
            FormClosing += FrmProcesosHilos_FormClosing;
            ((System.ComponentModel.ISupportInitialize)nudRegistros).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvResultados).EndInit();
            grpResponsiva.ResumeLayout(false);
            grpResponsiva.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitulo;
        private Label lblDescripcion;
        private Label lblRegistros;
        private NumericUpDown nudRegistros;
        private Button btnIniciar;
        private Button btnCancelar;
        private Button btnEjecutarLineal;
        private ProgressBar pbProgreso;
        private Label lblPorcentaje;
        private Label lblEstado;
        private DataGridView dgvResultados;
        private DataGridViewTextBoxColumn colNumero;
        private DataGridViewTextBoxColumn colUsuario;
        private DataGridViewTextBoxColumn colLibro;
        private DataGridViewTextBoxColumn colDias;
        private DataGridViewTextBoxColumn colMulta;
        private DataGridViewTextBoxColumn colEstado;
        private GroupBox grpResponsiva;
        private TextBox txtPrueba;
        private Button btnContador;
        private Label lblClics;
        private Label lblReloj;
        private Button btnCerrar;
        private Label lblAutoria;
        private System.Windows.Forms.Timer tmrReloj;
    }
}
