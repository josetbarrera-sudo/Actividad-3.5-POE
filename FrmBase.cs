/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes:
 * 1. CONTRERAS Rodriguez Janis Isabel
 * 2. TORRES Barrera Jose Angel
 * 3. MACÍAS Cruz Meredith Miranda
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Biblioteca
{
    // Actividad 3.1: FrmBase concentra los manejadores de eventos reutilizables.
    // Los formularios heredados solo los conectan a sus controles (DRY).
    public partial class FrmBase : Form
    {
        public FrmBase()
        {
            InitializeComponent();

            // El ID es común a todos los formularios heredados:
            // solo dígitos, sin pegar texto (Ctrl+V) y obligatorio.
            txtId.ShortcutsEnabled = false;
            txtId.KeyPress += SoloNumeros_KeyPress;
            txtId.Validating += TextoObligatorio_Validating;
        }

        // ================== KEYPRESS (filtros de teclado) ==================

        // Solo letras: acepta letras (con acentos y ñ), espacio y teclas de control (Backspace, etc.).
        protected void SoloLetras_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;                        // descarta la pulsación
                System.Media.SystemSounds.Beep.Play();   // sonido del sistema
            }
        }

        // Solo números enteros: acepta dígitos y teclas de control.
        protected void SoloNumeros_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        // Números con precisión: dígitos, teclas de control y UN solo punto decimal.
        protected void Decimal_KeyPress(object? sender, KeyPressEventArgs e)
        {
            TextBox txt = (TextBox)sender!;

            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                return;

            // El punto solo se admite si aún no existe uno (o si el texto seleccionado lo contiene y será reemplazado)
            if (e.KeyChar == '.' && (!txt.Text.Contains('.') || txt.SelectedText.Contains('.')))
                return;

            e.Handled = true;
            System.Media.SystemSounds.Beep.Play();
        }

        // Campos sin espacios (por ejemplo, correo electrónico).
        protected void SinEspacios_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        // ================== VALIDATING (presencia y consistencia) ==================

        // TextBox obligatorio: si está vacío, cancela la salida del control y muestra el error.
        protected void TextoObligatorio_Validating(object? sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox)sender!;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txt, "Este campo es obligatorio.");
            }
            else
            {
                errorProvider1.SetError(txt, "");   // limpieza obligatoria del error
            }
        }

        // ComboBox obligatorio: el índice -1 significa que no se seleccionó nada.
        protected void ComboObligatorio_Validating(object? sender, CancelEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender!;

            if (cmb.SelectedIndex == -1)
            {
                e.Cancel = true;
                errorProvider1.SetError(cmb, "Seleccione una opción de la lista.");
            }
            else
            {
                errorProvider1.SetError(cmb, "");
            }
        }

        // Decimal obligatorio: no vacío, con formato numérico válido y mayor que cero.
        protected void DecimalObligatorio_Validating(object? sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox)sender!;
            string mensaje = "";

            if (string.IsNullOrWhiteSpace(txt.Text))
                mensaje = "Este campo es obligatorio.";
            else if (!decimal.TryParse(txt.Text.Trim(), out decimal valor))
                mensaje = "Ingrese un número válido (ejemplo: 150.50).";
            else if (valor <= 0)
                mensaje = "El valor debe ser mayor que cero.";

            if (mensaje != "")
            {
                e.Cancel = true;
                errorProvider1.SetError(txt, mensaje);
            }
            else
            {
                errorProvider1.SetError(txt, "");
            }
        }

        // Correo obligatorio: no vacío, con "@" y un punto después de la arroba.
        protected void Correo_Validating(object? sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox)sender!;
            string correo = txt.Text.Trim();
            string mensaje = "";

            if (correo == "")
            {
                mensaje = "Este campo es obligatorio.";
            }
            else
            {
                int arroba = correo.IndexOf('@');
                if (arroba <= 0 || correo.IndexOf('.', arroba) <= arroba + 1 || correo.EndsWith("."))
                    mensaje = "Ingrese un correo válido (ejemplo: nombre@dominio.com).";
            }

            if (mensaje != "")
            {
                e.Cancel = true;
                errorProvider1.SetError(txt, mensaje);
            }
            else
            {
                errorProvider1.SetError(txt, "");
            }
        }

        // NumericUpDown con monto obligatorio mayor que cero.
        protected void MontoPositivo_Validating(object? sender, CancelEventArgs e)
        {
            NumericUpDown nup = (NumericUpDown)sender!;

            if (nup.Value <= 0)
            {
                e.Cancel = true;
                errorProvider1.SetError(nup, "El monto debe ser mayor que cero.");
            }
            else
            {
                errorProvider1.SetError(nup, "");
            }
        }


        // ================== CONEXIÓN AUTOMÁTICA PARA TODAS LAS PESTAÑAS ==================
        // Al cargar cualquier formulario heredado, se recorren sus controles y se conectan los
        // eventos según el nombre del control (txb_Nombre_..., txb_Edad_..., cmb_Estado_..., etc.).
        // Si un control ya fue conectado a mano, se quita y se vuelve a conectar (nunca queda duplicado).
        // Para excluir un control: Tag = "sinvalidar" o agregarlo a _excluidos.

        private static readonly string[] _letras = { "nombre", "apellido", "pais", "nacionalidad", "motivo", "descripcion" };
        private static readonly string[] _numeros = { "edad", "prestados", "copias", "cantidad", "anio", "telefono", "dias" };
        private static readonly string[] _decimales = { "costo", "precio", "monto", "salario", "tarifa" };
        private static readonly string[] _correos = { "correo", "email", "mail" };
        private static readonly HashSet<string> _excluidos = new HashSet<string> { "cmb_Libros_frmEdi", "txtBuscarDetalle" };

        private bool _validacionesConectadas = false;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!_validacionesConectadas)
            {
                _validacionesConectadas = true;
                ConectarValidacionesAutomaticas(this);
                ConectarDialogoImagen(this);   // Actividad 3.3
            }

            AjustarParaMdi();   // Actividad 3.5
        }

        // ================== ACTIVIDAD 3.5: FORMULARIO COMO VENTANA HIJA MDI ==================
        // El panel base está acoplado a la izquierda con un ancho fijo; dentro de una ventana MDI más
        // pequeña quedaría recortado. Se desacopla y se activa AutoScroll para que la ventana hija
        // muestre barras de desplazamiento en lugar de cortar el contenido.
        private void AjustarParaMdi()
        {
            if (MdiParent == null)
                return;

            int alto = 0;
            foreach (Control c in pnlFormularioBase.Controls)
                alto = Math.Max(alto, c.Bottom);

            int ancho = pnlFormularioBase.Width;

            pnlFormularioBase.Dock = DockStyle.None;
            pnlFormularioBase.Location = new System.Drawing.Point(0, 0);
            pnlFormularioBase.Size = new System.Drawing.Size(ancho, alto + 10);
            AutoScroll = true;
        }

        private static bool Contiene(string nombre, string[] claves)
        {
            foreach (string clave in claves)
                if (nombre.Contains(clave)) return true;
            return false;
        }

        private void ConectarValidacionesAutomaticas(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c.HasChildren)
                    ConectarValidacionesAutomaticas(c);

                if (c == txtId || (c.Tag as string) == "sinvalidar" || _excluidos.Contains(c.Name))
                    continue;

                string n = c.Name.ToLowerInvariant();

                if (c is TextBox txt)
                {
                    // Cajas de solo lectura (info, ruta de imagen) y de búsqueda no se validan
                    if (txt.ReadOnly || n.Contains("info") || n.Contains("rutaima") || n.Contains("buscar"))
                        continue;

                    // ----- KeyPress: filtro de teclado según el tipo de dato -----
                    txt.KeyPress -= SoloLetras_KeyPress;
                    txt.KeyPress -= SoloNumeros_KeyPress;
                    txt.KeyPress -= Decimal_KeyPress;
                    txt.KeyPress -= SinEspacios_KeyPress;

                    bool esCorreo = Contiene(n, _correos);
                    bool esDecimal = Contiene(n, _decimales);

                    if (esCorreo)
                    {
                        txt.KeyPress += SinEspacios_KeyPress;
                    }
                    else if (esDecimal)
                    {
                        txt.KeyPress += Decimal_KeyPress;
                        txt.ShortcutsEnabled = false;
                    }
                    else if (Contiene(n, _numeros))
                    {
                        txt.KeyPress += SoloNumeros_KeyPress;
                        txt.ShortcutsEnabled = false;
                    }
                    else if (Contiene(n, _letras))
                    {
                        txt.KeyPress += SoloLetras_KeyPress;
                        txt.ShortcutsEnabled = false;
                    }

                    // ----- Validating: todos los TextBox editables son obligatorios -----
                    txt.Validating -= TextoObligatorio_Validating;
                    txt.Validating -= DecimalObligatorio_Validating;
                    txt.Validating -= Correo_Validating;

                    if (esCorreo)
                        txt.Validating += Correo_Validating;
                    else if (esDecimal)
                        txt.Validating += DecimalObligatorio_Validating;
                    else
                        txt.Validating += TextoObligatorio_Validating;
                }
                else if (c is ComboBox cmb)
                {
                    // ComboBox: índice -1 (sin selección) no es válido
                    cmb.Validating -= ComboObligatorio_Validating;
                    cmb.Validating += ComboObligatorio_Validating;
                }
                else if (c is NumericUpDown nup && n.Contains("mon"))
                {
                    // Montos: deben ser mayores que cero
                    nup.Validating -= MontoPositivo_Validating;
                    nup.Validating += MontoPositivo_Validating;
                }
            }
        }

        // ================== ACTIVIDAD 3.3: DIÁLOGO DE ARCHIVO (OpenFileDialog) ==================
        // Cada formulario heredado tiene un botón "Imagen" y un TextBox "RutaIma".
        // Aquí se conectan entre sí una sola vez para toda la jerarquía (DRY).

        private void ConectarDialogoImagen(Control contenedor)
        {
            Button? boton = null;
            TextBox? ruta = null;
            BuscarControlesImagen(contenedor, ref boton, ref ruta);

            if (boton == null || ruta == null)
                return;

            boton.Tag = ruta;                       // el botón recuerda en qué TextBox escribir
            boton.Click -= BotonImagen_Click;
            boton.Click += BotonImagen_Click;
        }

        private void BuscarControlesImagen(Control contenedor, ref Button? boton, ref TextBox? ruta)
        {
            foreach (Control c in contenedor.Controls)
            {
                string n = c.Name.ToLowerInvariant();

                if (c is Button b && n.Contains("imagen"))
                    boton = b;
                else if (c is TextBox t && n.Contains("rutaima"))
                    ruta = t;

                if (c.HasChildren)
                    BuscarControlesImagen(c, ref boton, ref ruta);
            }
        }

        private void BotonImagen_Click(object? sender, EventArgs e)
        {
            if (sender is not Button boton || boton.Tag is not TextBox ruta)
                return;

            using OpenFileDialog dialogo = new OpenFileDialog();
            dialogo.Title = "Seleccionar imagen";
            dialogo.Filter = "Imágenes (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos (*.*)|*.*";
            dialogo.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dialogo.CheckFileExists = true;

            // La ruta solo se captura si el usuario confirmó con "Abrir" (DialogResult.OK)
            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                ruta.Text = dialogo.FileName;
                errorProvider1.SetError(ruta, "");
            }
        }

        // ================== APOYO PARA GUARDAR / ACTUALIZAR ==================

        // Valida todo el formulario: ValidateChildren() + bandera de errores activos.
        // Si hay errores, avisa en el cuadro de información y devuelve false.
        protected bool FormularioValido()
        {
            bool hayErrores = !this.ValidateChildren() || HayErroresActivos(this);

            if (hayErrores)
                MostrarErrorValidacion();

            return !hayErrores;
        }

        // Escribe el aviso de errores en el cuadro de información del formulario (txb_Info_...).
        protected void MostrarErrorValidacion()
        {
            TextBox? info = BuscarCuadroInfo(this);
            if (info != null)
                info.Text = "⚠️ Hay campos con errores.\r\nCorrija los campos marcados con el icono rojo antes de continuar.";
        }

        private TextBox? BuscarCuadroInfo(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c is TextBox t && t.Name.StartsWith("txb_Info"))
                    return t;

                TextBox? encontrado = BuscarCuadroInfo(c);
                if (encontrado != null)
                    return encontrado;
            }
            return null;
        }

        // ================== APOYO PARA EjecutarGuardar() ==================

        // Recorre todos los controles (incluidos los anidados en paneles) y devuelve true
        // si alguno tiene un error activo en el ErrorProvider.
        protected bool HayErroresActivos(Control contenedor)
        {
            foreach (Control control in contenedor.Controls)
            {
                if (errorProvider1.GetError(control) != "")
                    return true;

                if (control.HasChildren && HayErroresActivos(control))
                    return true;
            }
            return false;
        }
    }
}