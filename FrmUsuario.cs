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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Biblioteca
{
    public partial class FrmUsuario : FrmBase, IPanelCRUD
    {
        public FrmUsuario()
        {
            InitializeComponent();
            ConfigurarCuenta();
        }

        // Controles nuevos (se agregan desde el Diseñador, ver guía):
        //   cmb_Rol_frmUsua, txb_Contrasena_frmUsua, txb_Area_frmUsua (reemplaza al antiguo txb_Nacionalidad_frmUsua)
        private void ConfigurarCuenta()
        {
            cmb_Rol_frmUsua.Items.Clear();
            cmb_Rol_frmUsua.Items.AddRange(new object[] { "Estudiante", "Docente", "Administrador" });
            cmb_Rol_frmUsua.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb_Rol_frmUsua.SelectedIndex = 0;

            txb_Contrasena_frmUsua.UseSystemPasswordChar = true;   // oculta lo que se escribe
            txb_Contrasena_frmUsua.ShortcutsEnabled = false;

            // Solo el administrador gestiona cuentas, roles y contraseñas
            if (!Sesion.EsAdministrador)
            {
                foreach (Control c in this.Controls)
                    BloquearControles(c);
            }
        }

        private void BloquearControles(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c is TextBox || c is ComboBox || c is CheckBox)
                    c.Enabled = false;
                BloquearControles(c);
            }
        }

        private static RolUsuario LeerRol(string? texto)
        {
            return Enum.TryParse(texto, out RolUsuario rol) ? rol : RolUsuario.Estudiante;
        }

        // Implementación obligatoria de los 4 métodos del contrato IPanelCRUD
        public void EjecutarGuardar()
        {
            try
            {
                if (!int.TryParse(txtId.Text.Trim(), out int id))
                {
                    txb_Info_frmUsua.Text = "⚠️ ERROR DE VALIDACIÓN:\r\nPor favor, ingrese un código ID numérico válido.";
                    return;
                }

                string nombre = txb_Nombre_frmUsua.Text.Trim();
                int edad = int.TryParse(txb_Edad_frmUsua.Text.Trim(), out int e) ? e : 18;
                string correo = txb_Correo_frmUsua.Text.Trim();
                string area = txb_Area_frmUsua.Text.Trim();
                int librosPrestados = int.TryParse(txb_LibPres_frmUsua.Text.Trim(), out int lp) ? lp : 0;
                bool esProfesor = ckb_EsProfe_frmUsua.Checked;
                bool estado = ckb_Activo_frmUsua.Checked;

                // Valores por defecto para atributos no presentes en la interfaz
                decimal multaAcumulada = 0m;
                string rutaImagen = "usuario_default.png";

                // La contraseña es obligatoria al crear la cuenta
                if (string.IsNullOrWhiteSpace(txb_Contrasena_frmUsua.Text))
                {
                    errorProvider1.SetError(txb_Contrasena_frmUsua, "Ingrese una contraseña de al menos 8 caracteres.");
                    txb_Info_frmUsua.Text = "Falta la contraseña de la cuenta.";
                    return;
                }
                errorProvider1.SetError(txb_Contrasena_frmUsua, "");

                // Instancia usando el constructor completo de la clase Usuarios
                Usuarios nuevoUsuario = new Usuarios(id, nombre, edad, correo, librosPrestados, multaAcumulada, esProfesor, rutaImagen, estado);
                nuevoUsuario.Rol = LeerRol(cmb_Rol_frmUsua.SelectedItem?.ToString());
                nuevoUsuario.AreaDepartamento = area;
                nuevoUsuario.EstablecerContrasena(txb_Contrasena_frmUsua.Text);   // se guarda solo el hash
                nuevoUsuario.InsertarRegistro(nuevoUsuario);

                txb_Info_frmUsua.Text = "El usuario ha sido guardado exitosamente.";
                LimpiarCampos();
            }
            catch (ArgumentException ex)
            {
                txb_Info_frmUsua.Text = ex.Message;
            }
            catch (Exception)
            {
                txb_Info_frmUsua.Text = "Ocurrió un error al guardar";
            }
        }

        public void EjecutarBuscar(string id, ErrorProvider alerta)
        {
            alerta.Clear();

            if (string.IsNullOrWhiteSpace(id))
            {
                alerta.SetError(txtId, "Ingrese un ID de usuario para realizar la búsqueda.");
                return;
            }

            try
            {
                if (!int.TryParse(id.Trim(), out int idBuscado))
                {
                    alerta.SetError(txtId, "El ID debe ser un número entero.");
                    return;
                }

                // Búsqueda directa sobre la colección estática de Usuarios
                Usuarios usuarioEncontrado = Usuarios.ObtenerTodos().Find(u => u.Id == idBuscado);

                if (usuarioEncontrado != null)
                {
                    txtId.Text = usuarioEncontrado.Id.ToString();
                    txb_Nombre_frmUsua.Text = usuarioEncontrado.NombreCompleto;
                    txb_Edad_frmUsua.Text = usuarioEncontrado.Edad.ToString();
                    txb_Correo_frmUsua.Text = usuarioEncontrado.Correo;
                    txb_LibPres_frmUsua.Text = usuarioEncontrado.LibrosPrestados.ToString();
                    ckb_EsProfe_frmUsua.Checked = usuarioEncontrado.EsProfesor;
                    ckb_Activo_frmUsua.Checked = usuarioEncontrado.EsActivo;
                    cmb_Rol_frmUsua.SelectedItem = usuarioEncontrado.Rol.ToString();
                    txb_Area_frmUsua.Text = usuarioEncontrado.AreaDepartamento;
                    txb_Contrasena_frmUsua.Clear();   // el hash nunca se muestra
                }
                else
                {
                    txb_Info_frmUsua.Text = "No se encontró ningún usuario registrado con el ID especificado.";
                }
            }
            catch (Exception ex)
            {
                alerta.SetError(txtId, $"Error de búsqueda: {ex.Message}");
            }
        }

        public void EjecutarActualizar()
        {
            try
            {
                if (!int.TryParse(txtId.Text.Trim(), out int id))
                {
                    txb_Info_frmUsua.Text = "Ingrese un ID numérico válido para actualizar.";
                    return;
                }

                // Se modifica el usuario existente para conservar su contraseña, multa e historial
                Usuarios? usuario = new Usuarios().ConsultarRegistro(id.ToString()) as Usuarios;
                if (usuario == null)
                {
                    txb_Info_frmUsua.Text = "No se encontró el usuario a actualizar.";
                    return;
                }

                usuario.NombreCompleto = txb_Nombre_frmUsua.Text.Trim();
                usuario.Edad = int.TryParse(txb_Edad_frmUsua.Text.Trim(), out int e) ? e : 18;
                usuario.Correo = txb_Correo_frmUsua.Text.Trim();
                usuario.LibrosPrestados = int.TryParse(txb_LibPres_frmUsua.Text.Trim(), out int lp) ? lp : 0;
                usuario.Rol = LeerRol(cmb_Rol_frmUsua.SelectedItem?.ToString());
                usuario.AreaDepartamento = txb_Area_frmUsua.Text.Trim();
                usuario.EsActivo = ckb_Activo_frmUsua.Checked;   // desactivar cuenta sin borrar historial

                // Solo se cambia la contraseña si se escribió una nueva
                if (!string.IsNullOrWhiteSpace(txb_Contrasena_frmUsua.Text))
                    usuario.EstablecerContrasena(txb_Contrasena_frmUsua.Text);

                LimpiarCampos();
                txb_Info_frmUsua.Text = "Los datos del usuario han sido actualizados correctamente.";
            }
            catch (ArgumentException ex)
            {
                txb_Info_frmUsua.Text = ex.Message;
            }
            catch (Exception)
            {
                txb_Info_frmUsua.Text = "Error al actualizar";
            }
        }

        public void EjecutarEliminar(StatusStrip barraEstado)
        {
            try
            {
                string idAEliminar = txtId.Text.Trim();

                if (string.IsNullOrWhiteSpace(idAEliminar))
                {
                    txb_Info_frmUsua.Text = "Debe especificar el ID del usuario a eliminar.";
                    return;
                }

                DialogResult respuesta = MessageBox.Show($"¿Está seguro de eliminar al usuario con ID {idAEliminar}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes)
                    return;   // Actividad 3.3: el usuario eligió "No", se aborta de inmediato

                if (respuesta == DialogResult.Yes)
                {
                    Usuarios usuarioAuxiliar = new Usuarios();
                    usuarioAuxiliar.EliminarRegistro(idAEliminar);

                    if (barraEstado != null && barraEstado.Items.Count > 0)
                    {
                        barraEstado.Items[0].Text = $"Usuario con ID [{idAEliminar}] fue eliminado correctamente.";
                    }

                    LimpiarCampos();
                    txb_Info_frmUsua.Text = "El registro ha sido eliminado.";
                }
            }
            catch (Exception ex)
            {
                txb_Info_frmUsua.Text = "Error al eliminar";
            }
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txb_Nombre_frmUsua.Clear();
            txb_Edad_frmUsua.Clear();
            txb_Correo_frmUsua.Clear();
            txb_Area_frmUsua.Clear();
            txb_Contrasena_frmUsua.Clear();
            cmb_Rol_frmUsua.SelectedIndex = 0;
            txb_LibPres_frmUsua.Clear();
            ckb_EsProfe_frmUsua.Checked = false;
            ckb_Activo_frmUsua.Checked = true;
        }
    }
}