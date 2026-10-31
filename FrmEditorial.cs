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
using System.Windows.Forms;

namespace Biblioteca
{
    public partial class FrmEditorial : FrmBase, IPanelCRUD
    {
        // Actividad 2.7: editorial temporal que acumula los libros (detalle)
        // mientras el usuario captura. Usa la colección List<Libros> del modelo.
        private Editorial _editorialTemp = new Editorial();

        // Indica si el DataGridView está mostrando solo los libros activos
        private bool _filtrado = false;

        // BindingSource estable para el DataGridView: evita el error
        // "El índice -1 no tiene un valor" al seleccionar filas tras re-enlazar la lista.
        private BindingSource _bsDetalle = new BindingSource();

        public FrmEditorial()
        {
            InitializeComponent();
            ConfigurarValidaciones();

            // Eventos de los controles de detalle (agregados en el diseñador)
            // Configuración del DataGridView de detalle
            dgv_Detalle_frmEdi.ReadOnly = true;
            dgv_Detalle_frmEdi.AllowUserToAddRows = false;
            dgv_Detalle_frmEdi.MultiSelect = false;
            dgv_Detalle_frmEdi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_Detalle_frmEdi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Se enlaza una sola vez al BindingSource (columnas generadas automáticamente)
            dgv_Detalle_frmEdi.Columns.Clear();
            dgv_Detalle_frmEdi.AutoGenerateColumns = true;
            _bsDetalle.DataSource = _editorialTemp.ListaLibros;
            dgv_Detalle_frmEdi.DataSource = _bsDetalle;

            this.Load += FrmEditorial_Load;
            btnAgregarDetalle.Click += btnAgregarDetalle_Click;
            btnQuitarDetalle.Click += btnQuitarDetalle_Click;
            btnBuscarDetalle.Click += btnBuscarDetalle_Click;
            btnFiltrarDetalle.Click += btnFiltrarDetalle_Click;
            btnVaciarDetalle.Click += btnVaciarDetalle_Click;
        }

        // ================== ACTIVIDAD 3.1: EVENTOS DE VALIDACIÓN ==================
        // Conecta los manejadores heredados de FrmBase a los controles de este formulario.
        private void ConfigurarValidaciones()
        {
            // Nombre y país: solo letras + obligatorios
            txb_Nombre_frmEdi.KeyPress += SoloLetras_KeyPress;
            txb_Nombre_frmEdi.Validating += TextoObligatorio_Validating;
            txb_Pais_frmEdi.KeyPress += SoloLetras_KeyPress;
            txb_Pais_frmEdi.Validating += TextoObligatorio_Validating;

            // Correo: sin espacios + formato válido
            txb_Correo_frmEdi.KeyPress += SinEspacios_KeyPress;
            txb_Correo_frmEdi.Validating += Correo_Validating;

            // Estado: debe haber una opción seleccionada
            cmb_Estado_frmEdi.Validating += ComboObligatorio_Validating;

            // Se evita pegar texto con Ctrl+V en los campos filtrados
            txb_Nombre_frmEdi.ShortcutsEnabled = false;
            txb_Pais_frmEdi.ShortcutsEnabled = false;
        }

        // ================== ACTIVIDAD 2.7: DETALLE (LIBROS) ==================

        private void FrmEditorial_Load(object? sender, EventArgs e)
        {
            CargarComboLibros();
            ActualizarDetalle();
        }

        // Llena el ComboBox con los libros ya registrados en el sistema.
        private void CargarComboLibros()
        {
            List<Libros> disponibles = new List<Libros>();

            // Se consultan los IDs del 1 al 50 con el CRUD del modelo Libros (Actividad 2.3).
            for (int i = 1; i <= 50; i++)
            {
                try
                {
                    Libros? libro = new Libros().ConsultarRegistro(i.ToString()) as Libros;
                    if (libro != null)
                        disponibles.Add(libro);
                }
                catch (Exception)
                {
                    // ID sin registro: se ignora y continúa con el siguiente
                }
            }

            cmb_Libros_frmEdi.DataSource = null;
            cmb_Libros_frmEdi.DataSource = disponibles;
            cmb_Libros_frmEdi.DisplayMember = "Titulo";
        }

        // Refresca el DataGridView y el contador con el estado actual de la lista.
        private void ActualizarDetalle()
        {
            _bsDetalle.DataSource = _editorialTemp.ListaLibros;
            _bsDetalle.ResetBindings(false);
            lbl_Contador_frmEdi.Text = "Libros: " + _editorialTemp.ContarLibros();
            _filtrado = false;
            btnFiltrarDetalle.Text = "Filtrar activos";
        }

        // Devuelve el libro seleccionado en la tabla (fila completa) o null.
        private Libros? ObtenerLibroSeleccionado()
        {
            if (dgv_Detalle_frmEdi.SelectedRows.Count > 0)
                return dgv_Detalle_frmEdi.SelectedRows[0].DataBoundItem as Libros;

            if (dgv_Detalle_frmEdi.CurrentRow != null)
                return dgv_Detalle_frmEdi.CurrentRow.DataBoundItem as Libros;

            return null;
        }

        private void btnAgregarDetalle_Click(object? sender, EventArgs e)
        {
            Libros? seleccionado = cmb_Libros_frmEdi.SelectedItem as Libros;
            if (seleccionado == null)
            {
                txb_Info_frmEdi.Text = "Seleccione un libro para agregar.";
                return;
            }

            // Función 4: Validar existencia. Si el ID ya está en la colección, se rechaza el duplicado.
            if (_editorialTemp.ValidarExistencia(seleccionado.Id.ToString()))
            {
                txb_Info_frmEdi.Text = "Ese libro ya está en la lista.";
                MessageBox.Show("El libro con ID " + seleccionado.Id + " ya existe en la colección. No se agregó de nuevo.");
                return;
            }

            _editorialTemp.RegistrarLibro(seleccionado);
            ActualizarDetalle();
            txb_Info_frmEdi.Text = "Libro agregado a la editorial.";
        }

        // Función 5: Eliminar por ID. Si se escribe un ID en el cuadro, se elimina ese libro;
        // si el cuadro está vacío, se usa el libro seleccionado en la tabla.
        private void btnQuitarDetalle_Click(object? sender, EventArgs e)
        {
            string id = txtBuscarDetalle.Text.Trim();

            if (id == "")
            {
                Libros? seleccionado = ObtenerLibroSeleccionado();
                if (seleccionado == null)
                {
                    MessageBox.Show("Escriba un ID o seleccione un libro de la tabla.");
                    return;
                }
                id = seleccionado.Id.ToString();
            }

            // Función 4: se valida que el ID exista antes de intentar quitarlo
            if (!_editorialTemp.ValidarExistencia(id))
            {
                MessageBox.Show("No hay un libro con el ID " + id + " en la colección.");
                return;
            }

            if (_editorialTemp.EliminarLibroPorId(id))
            {
                ActualizarDetalle();
                txtBuscarDetalle.Clear();
                txb_Info_frmEdi.Text = "Libro con ID " + id + " quitado de la lista.";
            }
            else
            {
                MessageBox.Show("No se pudo quitar el libro.");
            }
        }

        // Función 3: Buscar por ID
        private void btnBuscarDetalle_Click(object? sender, EventArgs e)
        {
            string id = txtBuscarDetalle.Text.Trim();
            if (id == "")
            {
                MessageBox.Show("Escriba el ID del libro a buscar.");
                return;
            }

            Libros? encontrado = _editorialTemp.BuscarLibroPorId(id);
            if (encontrado == null)
            {
                MessageBox.Show("No hay un libro con ese ID en la colección.");
                return;
            }

            // Se resalta en la tabla el libro encontrado
            foreach (DataGridViewRow fila in dgv_Detalle_frmEdi.Rows)
            {
                if (ReferenceEquals(fila.DataBoundItem, encontrado))
                {
                    dgv_Detalle_frmEdi.ClearSelection();
                    fila.Selected = true; // con FullRowSelect se resalta toda la fila
                    break;
                }
            }

            MessageBox.Show("Libro encontrado: " + encontrado.Titulo);
        }

        // Función 6: Filtrar por estado (activos). Al pulsar otra vez se muestran todos.
        private void btnFiltrarDetalle_Click(object? sender, EventArgs e)
        {
            if (_filtrado)
            {
                ActualizarDetalle();
                return;
            }

            List<Libros> activos = _editorialTemp.FiltrarLibrosActivos();

            // Si ninguno está activo, se avisa y se deja la tabla como estaba
            if (activos.Count == 0)
            {
                MessageBox.Show("Ningún libro de la colección tiene el estado Activo.");
                return;
            }

            _bsDetalle.DataSource = activos;
            _bsDetalle.ResetBindings(false);
            lbl_Contador_frmEdi.Text = "Activos: " + activos.Count;
            btnFiltrarDetalle.Text = "Ver todos";
            _filtrado = true;
        }

        // Función 7: Vaciar lista
        private void btnVaciarDetalle_Click(object? sender, EventArgs e)
        {
            _editorialTemp.VaciarLibros();
            ActualizarDetalle();
            txb_Info_frmEdi.Text = "Se vació la colección de libros.";
        }

        // ================== CRUD (IPanelCRUD) ==================

        public void EjecutarGuardar()
        {
            // Actividad 3.1: ValidateChildren() fuerza que TODOS los controles ejecuten su evento Validating.
            // Después se revisa con una bandera lógica si el ErrorProvider tiene algún error activo.
            bool hayErrores = !this.ValidateChildren() || HayErroresActivos(this);

            if (hayErrores)
            {
                txb_Info_frmEdi.Text = "⚠️ Hay campos con errores.\r\nCorrija los campos marcados con el icono rojo antes de guardar.";
                return;   // se bloquea el empaquetado hacia el Modelo
            }

            try
            {
                if (!int.TryParse(txtId.Text.Trim(), out int id))
                {
                    txb_Info_frmEdi.Text = "⚠️ ERROR DE VALIDACIÓN:\r\nPor favor, ingrese un código ID numérico válido.";
                    return;
                }

                string nombre = txb_Nombre_frmEdi.Text.Trim();
                string pais = txb_Pais_frmEdi.Text.Trim();
                int anioFundacion = dtp_AñoFunda_frmEdi.Value.Year;
                string correo = txb_Correo_frmEdi.Text.Trim();
                string rutaImagen = txb_RutaIma_frmEdi.Text.Trim();
                bool estado = cmb_Estado_frmEdi.SelectedItem?.ToString() == "Activo";

                // Se empaquetan los datos del formulario junto con la colección del DataGridView
                Editorial nuevaEditorial = new Editorial(id, nombre, pais, anioFundacion, correo, rutaImagen, estado, _editorialTemp.ListaLibros);
                nuevaEditorial.InsertarRegistro(nuevaEditorial);

                txb_Info_frmEdi.Text = "La editorial ha sido guardada exitosamente.";
                LimpiarCampos();
            }
            catch (Exception)
            {
                txb_Info_frmEdi.Text = "Ocurrió un error al guardar: Error de Persistencia";
            }
        }

        public void EjecutarBuscar(string id, ErrorProvider alerta)
        {
            alerta.Clear();
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(id))
            {
                alerta.SetError(txtId, "Ingrese un ID de editorial para realizar la búsqueda.");
                return;
            }

            try
            {
                if (!int.TryParse(id.Trim(), out int idBuscado))
                {
                    alerta.SetError(txtId, "El ID debe ser un número entero.");
                    return;
                }

                Editorial? editorialEncontrada = new Editorial().ConsultarRegistro(id) as Editorial;

                if (editorialEncontrada != null)
                {
                    txtId.Text = editorialEncontrada.Id.ToString();
                    txb_Nombre_frmEdi.Text = editorialEncontrada.Nombre;
                    txb_Pais_frmEdi.Text = editorialEncontrada.Pais;
                    dtp_AñoFunda_frmEdi.Value = new DateTime(editorialEncontrada.AnioFundacion, 1, 1);
                    txb_Correo_frmEdi.Text = editorialEncontrada.CorreoContacto;
                    txb_RutaIma_frmEdi.Text = editorialEncontrada.RutaImagen;
                    cmb_Estado_frmEdi.SelectedItem = editorialEncontrada.EsActivo ? "Activo" : "Inactivo";

                    // Se cargan en pantalla los libros de la editorial encontrada
                    _editorialTemp.VaciarLibros();
                    foreach (Libros libro in editorialEncontrada.ListaLibros)
                    {
                        _editorialTemp.RegistrarLibro(libro);
                    }
                    ActualizarDetalle();
                }
                else
                {
                    txb_Info_frmEdi.Text = "No se encontró ninguna editorial registrada con el ID especificado.";
                }
            }
            catch (Exception ex)
            {
                alerta.SetError(txtId, $"Error de búsqueda: {ex.Message}");
            }
        }

        public void EjecutarActualizar()
        {
            // Actividad 3.1: mismas validaciones que al guardar
            if (!FormularioValido())
                return;

            try
            {
                if (!int.TryParse(txtId.Text.Trim(), out int id))
                {
                    txb_Info_frmEdi.Text = "Ingrese un ID numérico válido para actualizar.";
                    return;
                }

                Editorial editorialActualizada = new Editorial(
                    id,
                    txb_Nombre_frmEdi.Text.Trim(),
                    txb_Pais_frmEdi.Text.Trim(),
                    dtp_AñoFunda_frmEdi.Value.Year,
                    txb_Correo_frmEdi.Text.Trim(),
                    txb_RutaIma_frmEdi.Text.Trim(),
                    cmb_Estado_frmEdi.SelectedItem?.ToString() == "Activo",
                    _editorialTemp.ListaLibros
                );

                editorialActualizada.ActualizarRegistro(editorialActualizada);
                LimpiarCampos();
                txb_Info_frmEdi.Text = "Los datos de la editorial han sido actualizados correctamente.";
            }
            catch (Exception)
            {
                txb_Info_frmEdi.Text = "Error al actualizar";
            }
        }

        public void EjecutarEliminar(StatusStrip barraEstado)
        {
            try
            {
                string idAEliminar = txtId.Text.Trim();

                if (string.IsNullOrWhiteSpace(idAEliminar))
                {
                    txb_Info_frmEdi.Text = "Debe especificar el ID de la editorial a eliminar.";
                    return;
                }

                DialogResult respuesta = MessageBox.Show($"¿Está seguro de eliminar la editorial con ID {idAEliminar}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes)
                    return;   // Actividad 3.3: el usuario eligió "No", se aborta de inmediato

                if (respuesta == DialogResult.Yes)
                {
                    Editorial editorialAuxiliar = new Editorial();
                    editorialAuxiliar.EliminarRegistro(idAEliminar);

                    if (barraEstado != null && barraEstado.Items.Count > 0)
                    {
                        barraEstado.Items[0].Text = $"Editorial con ID [{idAEliminar}] fue eliminada correctamente.";
                    }

                    LimpiarCampos();
                    txb_Info_frmEdi.Text = "El registro ha sido eliminado.";
                }
            }
            catch (Exception)
            {
                txb_Info_frmEdi.Text = "Error al eliminar";
            }
        }

        private void LimpiarCampos()
        {
            errorProvider1.Clear();   // quita los iconos de error al limpiar la pantalla
            txtId.Clear();
            txb_Nombre_frmEdi.Clear();
            txb_Pais_frmEdi.Clear();
            dtp_AñoFunda_frmEdi.Value = DateTime.Now;
            txb_Correo_frmEdi.Clear();
            txb_RutaIma_frmEdi.Clear();
            cmb_Estado_frmEdi.SelectedIndex = 0;

            // Se reinicia la colección y el contador vuelve a cero
            _editorialTemp.VaciarLibros();
            ActualizarDetalle();
        }
    }
} //Final