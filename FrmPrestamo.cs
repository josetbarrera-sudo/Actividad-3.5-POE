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
    public partial class FrmPrestamo : FrmBase, IPanelCRUD
    {
        // Instancia que conserva la pila de historial mientras el formulario está abierto
        private Prestamos _prestamoHistorial = new Prestamos();

        public FrmPrestamo()
        {
            InitializeComponent();

            // Enlace de los eventos Click de la pila (el -= evita que se ejecuten doble si el diseñador ya los enlazó)
            btnRegistrarEstado.Click -= btnRegistrarEstado_Click;
            btnRegistrarEstado.Click += btnRegistrarEstado_Click;
            btnDeshacer.Click -= btnDeshacer_Click;
            btnDeshacer.Click += btnDeshacer_Click;

            ConfigurarEstadoYDevolucion();
            ActualizarVistaHistorial();   // deja el botón Deshacer desactivado al iniciar
        }

        // Control nuevo (se agrega desde el Diseñador, ver guía): dtp_FechaDevol_frmPrest (DateTimePicker)
        private void ConfigurarEstadoYDevolucion()
        {
            // Estado detallado del préstamo: Activo / Devuelto / Vencido
            cmb_Status_frmPrest.Items.Clear();
            cmb_Status_frmPrest.Items.AddRange(Enum.GetNames(typeof(EstadoPrestamo)));
            cmb_Status_frmPrest.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb_Status_frmPrest.SelectedIndex = 0;

            // La casilla del DateTimePicker indica si ya hubo devolución (sin marcar = aún no devuelto)
            dtp_FechaDevol_frmPrest.ShowCheckBox = true;
            dtp_FechaDevol_frmPrest.Checked = false;

            // Alumnos y profesores solo consultan sus propios préstamos
            if (!Sesion.EsAdministrador)
            {
                foreach (Control c in this.Controls)
                    BloquearControles(c);
                txtId.Enabled = true;
            }
        }

        private void BloquearControles(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c is TextBox || c is ComboBox || c is Button || c is DateTimePicker)
                    c.Enabled = false;
                BloquearControles(c);
            }
        }

        private static Usuarios? BuscarUsuarioPorNombre(string nombre)
        {
            return Usuarios.ObtenerTodos().Find(u => string.Equals(u.NombreCompleto, nombre, StringComparison.OrdinalIgnoreCase));
        }

        private static Libros? BuscarLibroPorTitulo(string titulo)
        {
            return Libros.ObtenerTodos().Find(l => string.Equals(l.Titulo, titulo, StringComparison.OrdinalIgnoreCase));
        }

        private EstadoPrestamo EstadoSeleccionado()
        {
            return Enum.TryParse(cmb_Status_frmPrest.SelectedItem?.ToString(), out EstadoPrestamo e) ? e : EstadoPrestamo.Activo;
        }

        // ===================== CONTROL VISUAL DE LA PILA =====================

        // Botón "Registrar": apila un PRESTAMO o una DEVOLUCION según el combo de Status
        private void btnRegistrarEstado_Click(object sender, EventArgs e)
        {
            string tipo = "PRESTAMO";
            if (cmb_Status_frmPrest.SelectedItem?.ToString() == "Devuelto")
                tipo = "DEVOLUCION";

            _prestamoHistorial.RegistrarAccion(tipo);
            SincronizarStatus();
            ActualizarVistaHistorial();
            txb_Info_frmPrest.Text = "Acción registrada en el historial: " + tipo;
        }

        // Botón "Deshacer Último": desapila y revierte la última acción
        private void btnDeshacer_Click(object sender, EventArgs e)
        {
            string revertido = _prestamoHistorial.DeshacerUltimo();

            if (revertido != null)
            {
                SincronizarStatus();
                txb_Info_frmPrest.Text = "Se revirtió: " + revertido;
            }

            ActualizarVistaHistorial();
        }

        // Refresca ListBox, etiqueta de la cima y habilita/deshabilita Deshacer
        private void ActualizarVistaHistorial()
        {
            lstHistorial.Items.Clear();

            string[] datos = _prestamoHistorial.VolcarAArreglo();
            for (int i = 0; i < datos.Length; i++)
            {
                lstHistorial.Items.Add(datos[i]);
            }

            lblCima.Text = "Cima: " + _prestamoHistorial.VerCima();
            btnDeshacer.Enabled = _prestamoHistorial.ContarHistorial() > 0;
        }

        // Refleja en el combo el Status que dejó el modelo tras registrar o deshacer
        private void SincronizarStatus()
        {
            cmb_Status_frmPrest.SelectedItem = _prestamoHistorial.Estado.ToString();

            if (_prestamoHistorial.FechaDevolucionReal.HasValue)
            {
                dtp_FechaDevol_frmPrest.Value = _prestamoHistorial.FechaDevolucionReal.Value;
                dtp_FechaDevol_frmPrest.Checked = true;
            }
            else
            {
                dtp_FechaDevol_frmPrest.Checked = false;
            }
        }

        // =====================================================================

        // Implementación obligatoria de los 4 métodos del contrato IPanelCRUD
        public void EjecutarGuardar()
        {
            try
            {
                if (!int.TryParse(txtId.Text.Trim(), out int id))
                {
                    txb_Info_frmPrest.Text = "⚠️ ERROR DE VALIDACIÓN:\r\nPor favor, ingrese un código ID numérico válido.";
                    return;
                }

                // El usuario y el libro deben estar registrados
                Usuarios? usr = BuscarUsuarioPorNombre(txb_Usuario_frmPrest.Text.Trim());
                if (usr == null)
                {
                    txb_Info_frmPrest.Text = "El usuario no está registrado. Regístrelo primero en el módulo Usuarios.";
                    return;
                }
                if (!usr.EsActivo)
                {
                    txb_Info_frmPrest.Text = "La cuenta de este usuario está inactiva.";
                    return;
                }

                Libros? libro = BuscarLibroPorTitulo(txb_Libro_frmPrest.Text.Trim());
                if (libro == null)
                {
                    txb_Info_frmPrest.Text = "El libro no está registrado. Regístrelo primero en el módulo Libros.";
                    return;
                }

                EstadoPrestamo estadoSel = EstadoSeleccionado();
                if (estadoSel != EstadoPrestamo.Devuelto && dtp_FechaDevol_frmPrest.Checked)
                {
                    txb_Info_frmPrest.Text = "Solo se registra fecha de devolución cuando el estado es Devuelto.";
                    return;
                }

                string rutaImagen = txb_RutaIma_frmPrest.Text.Trim();
                bool estado = cmb_Estado_frmPrest.SelectedItem?.ToString() == "Activo";

                // Instancia usando el constructor parametrizado de la clase Prestamos
                // (al agregar el libro se toma una copia física concreta, si es físico)
                Prestamos nuevoPrestamo = new Prestamos(id, usr, libro, rutaImagen, estado);
                nuevoPrestamo.FechaEntrega = dtp_FechaEntr_frmPrest.Value;

                if (estadoSel == EstadoPrestamo.Devuelto)
                    nuevoPrestamo.RegistrarDevolucion(dtp_FechaDevol_frmPrest.Checked ? dtp_FechaDevol_frmPrest.Value : DateTime.Now);
                else
                {
                    nuevoPrestamo.Estado = estadoSel;
                    nuevoPrestamo.ActualizarEstado();
                }

                nuevoPrestamo.InsertarRegistro(nuevoPrestamo);

                // Al guardar definitivamente, se vacía el historial de la pila
                _prestamoHistorial.VaciarHistorial();
                ActualizarVistaHistorial();

                txb_Info_frmPrest.Text = "El préstamo ha sido guardado exitosamente.";
                LimpiarCampos();
            }
            catch (InvalidOperationException ex)
            {
                txb_Info_frmPrest.Text = ex.Message;   // por ejemplo: no hay copias disponibles
            }
            catch (Exception)
            {
                txb_Info_frmPrest.Text = "Ocurrió un error al guardar: Error de Persistencia";
            }
        }

        public void EjecutarBuscar(string id, ErrorProvider alerta)
        {
            alerta.Clear();

            if (string.IsNullOrWhiteSpace(id))
            {
                alerta.SetError(txtId, "Ingrese un ID de préstamo para realizar la búsqueda.");
                return;
            }

            try
            {
                if (!int.TryParse(id.Trim(), out int idBuscado))
                {
                    alerta.SetError(txtId, "El ID debe ser un número entero.");
                    return;
                }

                // Búsqueda del préstamo invocando a ConsultarRegistro
                Prestamos aux = new Prestamos();
                object resultado = aux.ConsultarRegistro(idBuscado.ToString());

                if (resultado is Prestamos prestamoEncontrado)
                {
                    // Alumnos y profesores solo ven sus propios préstamos
                    if (!Sesion.EsAdministrador && prestamoEncontrado.Usuario.Id != Sesion.UsuarioActual?.Id)
                    {
                        txb_Info_frmPrest.Text = "No tiene permiso para consultar este préstamo.";
                        return;
                    }

                    prestamoEncontrado.ActualizarEstado();
                    txtId.Text = prestamoEncontrado.Id.ToString();
                    txb_Usuario_frmPrest.Text = prestamoEncontrado.Usuario.Nombre;

                    if (prestamoEncontrado.LibrosPrestados != null && prestamoEncontrado.LibrosPrestados.Length > 0 && prestamoEncontrado.LibrosPrestados[0] != null)
                    {
                        txb_Libro_frmPrest.Text = prestamoEncontrado.LibrosPrestados[0].Titulo;
                    }

                    dtp_FechaEntr_frmPrest.Value = prestamoEncontrado.FechaEntrega;
                    cmb_Status_frmPrest.SelectedItem = prestamoEncontrado.Estado.ToString();

                    if (prestamoEncontrado.FechaDevolucionReal.HasValue)
                    {
                        dtp_FechaDevol_frmPrest.Value = prestamoEncontrado.FechaDevolucionReal.Value;
                        dtp_FechaDevol_frmPrest.Checked = true;
                    }
                    else
                    {
                        dtp_FechaDevol_frmPrest.Checked = false;
                    }
                    cmb_Estado_frmPrest.SelectedItem = prestamoEncontrado.EsActivo ? "Activo" : "Inactivo";
                    txb_RutaIma_frmPrest.Text = prestamoEncontrado.RutaImagen;

                    if (prestamoEncontrado.DevolvioATiempo == true)
                        txb_Info_frmPrest.Text = "Devuelto a tiempo.";
                    else if (prestamoEncontrado.DiasRetraso > 0)
                        txb_Info_frmPrest.Text = $"Retraso de {prestamoEncontrado.DiasRetraso} día(s).";
                    else
                        txb_Info_frmPrest.Text = $"Fecha límite: {prestamoEncontrado.FechaEntrega:dd/MM/yyyy}.";
                }
                else
                {
                    txb_Info_frmPrest.Text = "No se encontró ningún préstamo registrado con el ID especificado.";
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
                    txb_Info_frmPrest.Text = "Ingrese un ID numérico válido para actualizar.";
                    return;
                }

                // Se modifica el préstamo existente para conservar sus libros y ejemplares
                Prestamos? prestamo = new Prestamos().ConsultarRegistro(id.ToString()) as Prestamos;
                if (prestamo == null)
                {
                    txb_Info_frmPrest.Text = "No se encontró el préstamo a actualizar.";
                    return;
                }

                EstadoPrestamo estadoSel = EstadoSeleccionado();

                if (prestamo.Estado == EstadoPrestamo.Devuelto && estadoSel != EstadoPrestamo.Devuelto)
                {
                    txb_Info_frmPrest.Text = "Un préstamo devuelto no se puede reactivar. Registre un préstamo nuevo.";
                    return;
                }
                if (estadoSel != EstadoPrestamo.Devuelto && dtp_FechaDevol_frmPrest.Checked)
                {
                    txb_Info_frmPrest.Text = "Solo se registra fecha de devolución cuando el estado es Devuelto.";
                    return;
                }

                prestamo.FechaEntrega = dtp_FechaEntr_frmPrest.Value;
                prestamo.RutaImagen = txb_RutaIma_frmPrest.Text.Trim();
                prestamo.EsActivo = cmb_Estado_frmPrest.SelectedItem?.ToString() == "Activo";

                if (estadoSel == EstadoPrestamo.Devuelto && prestamo.Estado != EstadoPrestamo.Devuelto)
                {
                    // Compara la fecha real con la fecha límite y libera las copias
                    prestamo.RegistrarDevolucion(dtp_FechaDevol_frmPrest.Checked ? dtp_FechaDevol_frmPrest.Value : DateTime.Now);
                }
                else if (estadoSel != EstadoPrestamo.Devuelto)
                {
                    prestamo.Estado = estadoSel;
                    prestamo.ActualizarEstado();
                }

                LimpiarCampos();
                txb_Info_frmPrest.Text = "Los datos del préstamo han sido actualizados correctamente.";
            }
            catch (Exception ex)
            {
                txb_Info_frmPrest.Text = "Error al actualizar";
            }
        }

        public void EjecutarEliminar(StatusStrip barraEstado)
        {
            try
            {
                string idAEliminar = txtId.Text.Trim();

                if (string.IsNullOrWhiteSpace(idAEliminar))
                {
                    txb_Info_frmPrest.Text = "Debe especificar el ID del préstamo a eliminar.";
                    return;
                }

                DialogResult respuesta = MessageBox.Show($"¿Está seguro de eliminar el préstamo con ID {idAEliminar}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes)
                    return;   // Actividad 3.3: el usuario eligió "No", se aborta de inmediato

                if (respuesta == DialogResult.Yes)
                {
                    Prestamos prestamoAuxiliar = new Prestamos();
                    prestamoAuxiliar.EliminarRegistro(idAEliminar);

                    if (barraEstado != null && barraEstado.Items.Count > 0)
                    {
                        barraEstado.Items[0].Text = $"Préstamo con ID [{idAEliminar}] fue eliminado correctamente.";
                    }

                    LimpiarCampos();
                    txb_Info_frmPrest.Text = "El registro ha sido eliminado.";
                }
            }
            catch (Exception ex)
            {
                txb_Info_frmPrest.Text = "Error al eliminar";
            }
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txb_Usuario_frmPrest.Clear();
            txb_Libro_frmPrest.Clear();
            dtp_FechaEntr_frmPrest.Value = DateTime.Now.AddDays(7);
            dtp_FechaDevol_frmPrest.Checked = false;
            txb_RutaIma_frmPrest.Clear();

            if (cmb_Status_frmPrest.Items.Count > 0)
                cmb_Status_frmPrest.SelectedIndex = 0;

            if (cmb_Estado_frmPrest.Items.Count > 0)
                cmb_Estado_frmPrest.SelectedIndex = 0;
        }
    }
}