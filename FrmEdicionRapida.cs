/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes:
 * 1. CONTRERAS Rodriguez Janis Isabel
 * 2. MACÍAS Cruz Meredith Miranda
 * 3. TORRES Barrera Jose Angel
 */
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Biblioteca
{
    // Actividad 3.3: cuadro de diálogo personalizado (modal).
    // Se abre con ShowDialog() y devuelve su estado al formulario padre mediante DialogResult:
    // Aceptar -> DialogResult.OK, Cancelar -> DialogResult.Cancel (asignados en el diseñador).
    public partial class FrmEdicionRapida : Form
    {
        // Datos que el formulario padre lee cuando el resultado es OK
        public int IdLibro { get; private set; }
        public decimal NuevoPrecio { get; private set; }

        public FrmEdicionRapida()
        {
            InitializeComponent();

            txtIdLibro.ShortcutsEnabled = false;
            txtIdLibro.KeyPress += SoloNumeros_KeyPress;
            txtNuevoPrecio.ShortcutsEnabled = false;
            txtNuevoPrecio.KeyPress += Decimal_KeyPress;
            this.FormClosing += FrmEdicionRapida_FormClosing;
        }

        private void SoloNumeros_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void Decimal_KeyPress(object? sender, KeyPressEventArgs e)
        {
            TextBox txt = (TextBox)sender!;

            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                return;

            if (e.KeyChar == '.' && (!txt.Text.Contains('.') || txt.SelectedText.Contains('.')))
                return;

            e.Handled = true;
            System.Media.SystemSounds.Beep.Play();
        }

        // Al pulsar Aceptar el formulario intenta cerrarse con DialogResult.OK.
        // Aquí se valida: si hay errores se cancela el cierre y la ventana sigue abierta.
        private void FrmEdicionRapida_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
                return;   // Cancelar o la "X": se cierra sin validar

            errorProvider1.Clear();
            bool valido = true;

            if (!int.TryParse(txtIdLibro.Text.Trim(), out int id) || id <= 0)
            {
                errorProvider1.SetError(txtIdLibro, "Ingrese un ID numérico mayor que cero.");
                valido = false;
            }

            if (!decimal.TryParse(txtNuevoPrecio.Text.Trim(), out decimal precio) || precio <= 0)
            {
                errorProvider1.SetError(txtNuevoPrecio, "Ingrese un precio válido mayor que cero (ejemplo: 25.50).");
                valido = false;
            }

            if (!valido)
            {
                e.Cancel = true;
                this.DialogResult = DialogResult.None;
                return;
            }

            IdLibro = id;
            NuevoPrecio = precio;
        }
    }
}
