/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes:
 * 1. CONTRERAS Rodriguez Janis Isabel
 * 2. TORRES Barrera Jose Angel
 * 3. MACÍAS Cruz Meredith Miranda
 */
using Biblioteca.Models;
using System;
using System.Windows.Forms;

namespace Biblioteca
{
    // Ventana modal de inicio de sesión.
    // Controles (creados desde el Diseñador): txtCorreo, txtContrasena, btnIngresar, btnCancelar
    public partial class FrmLogin : Form
    {
        private int _intentos = 0;
        private const int MaxIntentos = 3;

        public FrmLogin()
        {
            InitializeComponent();

            txtContrasena.UseSystemPasswordChar = true;
            txtContrasena.ShortcutsEnabled = false;
            this.AcceptButton = btnIngresar;   // Enter = Ingresar
            this.CancelButton = btnCancelar;   // Esc = Cancelar

            btnIngresar.Click += btnIngresar_Click;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
        }

        private void btnIngresar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCorreo.Text) || string.IsNullOrEmpty(txtContrasena.Text))
            {
                MessageBox.Show("Ingrese su correo y su contraseña.", "Inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Usuarios? usuario = Sesion.IniciarSesion(txtCorreo.Text, txtContrasena.Text);

            if (usuario != null)
            {
                this.DialogResult = DialogResult.OK;
                return;
            }

            _intentos++;
            txtContrasena.Clear();

            if (_intentos >= MaxIntentos)
            {
                MessageBox.Show("Demasiados intentos fallidos. La aplicación se cerrará.", "Inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            // Mismo mensaje para correo inexistente, contraseña incorrecta o cuenta inactiva
            MessageBox.Show("Correo o contraseña incorrectos, o la cuenta está inactiva.", "Inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
