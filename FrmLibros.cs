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
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Biblioteca
{
    public partial class FrmLibros : FrmBase, IPanelCRUD
    {
        public FrmLibros()
        {
            InitializeComponent();
            ConfigurarValidaciones();
            ConfigurarRecurso();
        }

        // ================== CORRECCIÓN 4.x: recursos físicos y digitales ==================
        // Controles nuevos que se agregan desde el Diseñador (ver guía):
        //   cmb_Tipo_frmLibros, txb_Copias_frmLibros, txb_Archivo_frmLibros, btn_Archivo_frmLibros,
        //   btn_Descargar_frmLibros, clb_Autores_frmLibros
        private void ConfigurarRecurso()
        {
            cmb_Tipo_frmLibros.Items.Clear();
            cmb_Tipo_frmLibros.Items.AddRange(new object[] { "Físico", "Digital" });
            cmb_Tipo_frmLibros.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb_Tipo_frmLibros.SelectedIndex = 0;
            cmb_Tipo_frmLibros.SelectedIndexChanged += (s, e) => ActualizarCamposSegunTipo();

            txb_Copias_frmLibros.KeyPress += SoloNumeros_KeyPress;
            txb_Copias_frmLibros.ShortcutsEnabled = false;
            txb_Archivo_frmLibros.ReadOnly = true;

            btn_Archivo_frmLibros.Click += btn_Archivo_Click;
            btn_Descargar_frmLibros.Click += btn_Descargar_Click;

            CargarAutoresDisponibles();
            ActualizarCamposSegunTipo();

            // Alumnos y profesores: solo consulta y descarga
            if (!Sesion.EsAdministrador)
                AplicarSoloLectura();
        }

        private void ActualizarCamposSegunTipo()
        {
            bool esDigital = cmb_Tipo_frmLibros.SelectedItem?.ToString() == "Digital";
            txb_Copias_frmLibros.Enabled = !esDigital;
            txb_Archivo_frmLibros.Enabled = esDigital;
            btn_Archivo_frmLibros.Enabled = esDigital;
            if (esDigital) txb_Copias_frmLibros.Clear();
        }

        private void CargarAutoresDisponibles()
        {
            clb_Autores_frmLibros.Items.Clear();
            foreach (Autores a in Autores.ObtenerTodos())
                clb_Autores_frmLibros.Items.Add(a);   // se muestra con ToString() del autor
            clb_Autores_frmLibros.CheckOnClick = true;
        }

        private void AplicarSoloLectura()
        {
            foreach (Control c in this.Controls)
                BloquearControles(c);
            txtId.Enabled = true;                    // se conserva para consultar por ID
            btn_Descargar_frmLibros.Enabled = true;  // pueden descargar o leer el material digital
        }

        private void BloquearControles(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c is TextBox || c is ComboBox || c is Button || c is CheckedListBox)
                    c.Enabled = false;
                BloquearControles(c);
            }
        }

        private void btn_Archivo_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialogo = new OpenFileDialog();
            dialogo.Title = "Seleccionar archivo digital";
            dialogo.Filter = "Libros digitales (*.pdf;*.epub)|*.pdf;*.epub|Todos los archivos (*.*)|*.*";
            if (dialogo.ShowDialog() == DialogResult.OK)
                txb_Archivo_frmLibros.Text = dialogo.FileName;
        }

        // Abre el PDF/libro electrónico (ruta local o enlace web) con la aplicación del sistema
        private void btn_Descargar_Click(object? sender, EventArgs e)
        {
            string ruta = txb_Archivo_frmLibros.Text.Trim();
            if (cmb_Tipo_frmLibros.SelectedItem?.ToString() != "Digital" || ruta.Length == 0)
            {
                txb_Info_frmLibros.Text = "Este libro no tiene un archivo digital disponible.";
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
            }
            catch (Exception)
            {
                txb_Info_frmLibros.Text = "No se pudo abrir el archivo. Verifique que la ruta o el enlace sean correctos.";
            }
        }

        // Validaciones propias del tipo de recurso
        private bool RecursoValido()
        {
            bool valido = true;
            errorProvider1.SetError(txb_Copias_frmLibros, "");
            errorProvider1.SetError(txb_Archivo_frmLibros, "");

            if (cmb_Tipo_frmLibros.SelectedItem?.ToString() == "Digital")
            {
                if (string.IsNullOrWhiteSpace(txb_Archivo_frmLibros.Text))
                {
                    errorProvider1.SetError(txb_Archivo_frmLibros, "Seleccione el archivo o enlace del recurso digital.");
                    valido = false;
                }
            }
            else if (!int.TryParse(txb_Copias_frmLibros.Text.Trim(), out int copias) || copias < 1)
            {
                errorProvider1.SetError(txb_Copias_frmLibros, "Indique cuántas copias físicas hay (mínimo 1).");
                valido = false;
            }

            if (!valido)
                txb_Info_frmLibros.Text = "⚠️ Hay campos con errores.\r\nCorrija los campos marcados con el icono rojo antes de continuar.";
            return valido;
        }

        private void VincularAutoresSeleccionados(int idLibro)
        {
            LibroAutor.QuitarVinculosDeLibro(idLibro);
            foreach (object item in clb_Autores_frmLibros.CheckedItems)
            {
                if (item is Autores autor)
                    LibroAutor.Vincular(idLibro, autor.Id);
            }
        }

        // ================== ACTIVIDAD 3.1: EVENTOS DE VALIDACIÓN ==================
        // Conecta los manejadores heredados de FrmBase a los controles de este formulario.
        private void ConfigurarValidaciones()
        {
            // Título: obligatorio (puede incluir números, por ejemplo "1984", por eso no se filtra a solo letras)
            txb_Titulo_frmLibros.Validating += TextoObligatorio_Validating;

            // Costo: dígitos y un único punto decimal + valor válido mayor que cero
            txb_Costo_frmLibros.KeyPress += Decimal_KeyPress;
            txb_Costo_frmLibros.Validating += DecimalObligatorio_Validating;
            txb_Costo_frmLibros.ShortcutsEnabled = false;

            // Novedad y estado: debe haber una opción seleccionada
            cmb_Novedad_frmLibros.Validating += ComboObligatorio_Validating;
            cmb_Estado_frmLibros.Validating += ComboObligatorio_Validating;
        }

        // El combo de Novedad puede tener "Si" o "Sí"; se aceptan ambos.
        private static bool EsSi(string? texto)
        {
            return texto == "Si" || texto == "Sí";
        }

        // Implementación obligatoria de los 4 métodos del contrato IPanelCRUD
        public void EjecutarGuardar()
        {
            // Actividad 3.1: ValidateChildren() fuerza que TODOS los controles ejecuten su evento Validating.
            // Después se revisa con una bandera lógica si el ErrorProvider tiene algún error activo.
            bool hayErrores = !this.ValidateChildren() || HayErroresActivos(this);

            if (hayErrores)
            {
                txb_Info_frmLibros.Text = "⚠️ Hay campos con errores.\r\nCorrija los campos marcados con el icono rojo antes de guardar.";
                return;   // se bloquea el empaquetado hacia el Modelo
            }

            if (!RecursoValido())
                return;

            try
            {
                if (!int.TryParse(txtId.Text.Trim(), out int id))
                {
                    txb_Info_frmLibros.Text = "⚠️ ERROR DE VALIDACIÓN:\r\nPor favor, ingrese un código ID numérico válido.";
                    return;
                }

                string titulo = txb_Titulo_frmLibros.Text.Trim();
                decimal costo = decimal.TryParse(txb_Costo_frmLibros.Text.Trim(), out decimal c) ? c : 20.0m;
                bool esNovedad = EsSi(cmb_Novedad_frmLibros.SelectedItem?.ToString());

                // CORRECCIÓN: antes se comparaba contra "Disponible", que no existe en el combo,
                // por eso todos los libros se guardaban como inactivos.
                bool estado = cmb_Estado_frmLibros.SelectedItem?.ToString() == "Activo";
                string rutaImagen = txb_RutaIma_frmLibros.Text.Trim();

                bool esDigital = cmb_Tipo_frmLibros.SelectedItem?.ToString() == "Digital";
                TipoRecurso tipo = esDigital ? TipoRecurso.Digital : TipoRecurso.Fisico;
                int copias = esDigital ? 0 : int.Parse(txb_Copias_frmLibros.Text.Trim());
                string archivo = esDigital ? txb_Archivo_frmLibros.Text.Trim() : string.Empty;

                // Instancia usando el constructor completo de la clase Libros
                Libros nuevoLibro = new Libros(id, titulo, costo, copias, esNovedad, rutaImagen, estado, tipo, archivo);
                nuevoLibro.InsertarRegistro(nuevoLibro);
                VincularAutoresSeleccionados(id);

                txb_Info_frmLibros.Text = "El libro ha sido guardado exitosamente.";
                LimpiarCampos();
            }
            catch (Exception)
            {
                txb_Info_frmLibros.Text = "Ocurrió un error al guardar: Error de Persistencia";
            }
        }

        public void EjecutarBuscar(string id, ErrorProvider alerta)
        {
            alerta.Clear();
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(id))
            {
                alerta.SetError(txtId, "Ingrese un ID de libro para realizar la búsqueda.");
                return;
            }

            try
            {
                if (!int.TryParse(id.Trim(), out int idBuscado))
                {
                    alerta.SetError(txtId, "El ID debe ser un número entero.");
                    return;
                }

                // Búsqueda directa sobre la colección estática de Libros
                Libros? libroEncontrado = null;
                foreach (Libros libro in Libros.ObtenerTodos())
                {
                    if (libro.Id == idBuscado)
                    {
                        libroEncontrado = libro;
                        break;
                    }
                }

                if (libroEncontrado != null)
                {
                    txtId.Text = libroEncontrado.Id.ToString();
                    txb_Titulo_frmLibros.Text = libroEncontrado.Titulo;
                    txb_Costo_frmLibros.Text = libroEncontrado.PrecioRentaDiaria.ToString();
                    cmb_Novedad_frmLibros.SelectedItem = libroEncontrado.EsNovedad ? "Sí" : "No";
                    cmb_Estado_frmLibros.SelectedItem = libroEncontrado.EsActivo ? "Activo" : "Inactivo";
                    txb_RutaIma_frmLibros.Text = libroEncontrado.RutaImagen;

                    cmb_Tipo_frmLibros.SelectedItem = libroEncontrado.TipoRecurso == TipoRecurso.Digital ? "Digital" : "Físico";
                    txb_Copias_frmLibros.Text = libroEncontrado.TipoRecurso == TipoRecurso.Fisico ? libroEncontrado.TotalEjemplares.ToString() : "";
                    txb_Archivo_frmLibros.Text = libroEncontrado.RutaArchivoDigital;

                    CargarAutoresDisponibles();
                    List<Autores> autoresDelLibro = libroEncontrado.ObtenerAutores();
                    for (int i = 0; i < clb_Autores_frmLibros.Items.Count; i++)
                    {
                        if (clb_Autores_frmLibros.Items[i] is Autores a && autoresDelLibro.Exists(x => x.Id == a.Id))
                            clb_Autores_frmLibros.SetItemChecked(i, true);
                    }

                    txb_Info_frmLibros.Text = libroEncontrado.TipoRecurso == TipoRecurso.Digital
                        ? "Recurso digital."
                        : $"Copias disponibles: {libroEncontrado.CopiasDisponibles} de {libroEncontrado.TotalEjemplares}.";
                }
                else
                {
                    txb_Info_frmLibros.Text = "No se encontró ningún libro registrado con el ID especificado.";
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
            if (!FormularioValido() || !RecursoValido())
                return;

            try
            {
                if (!int.TryParse(txtId.Text.Trim(), out int id))
                {
                    txb_Info_frmLibros.Text = "Ingrese un ID numérico válido para actualizar.";
                    return;
                }

                // Se modifica el libro existente (no se reemplaza) para no perder el estado de sus ejemplares
                Libros? libro = new Libros().ConsultarRegistro(id.ToString()) as Libros;
                if (libro == null)
                {
                    txb_Info_frmLibros.Text = "No se encontró el libro a actualizar.";
                    return;
                }

                bool esDigital = cmb_Tipo_frmLibros.SelectedItem?.ToString() == "Digital";
                libro.Titulo = txb_Titulo_frmLibros.Text.Trim();
                libro.PrecioRentaDiaria = decimal.TryParse(txb_Costo_frmLibros.Text.Trim(), out decimal c) ? c : 20.0m;
                libro.EsNovedad = EsSi(cmb_Novedad_frmLibros.SelectedItem?.ToString());
                libro.RutaImagen = txb_RutaIma_frmLibros.Text.Trim();
                libro.EsActivo = cmb_Estado_frmLibros.SelectedItem?.ToString() == "Activo";
                libro.TipoRecurso = esDigital ? TipoRecurso.Digital : TipoRecurso.Fisico;
                libro.RutaArchivoDigital = esDigital ? txb_Archivo_frmLibros.Text.Trim() : string.Empty;
                libro.AjustarEjemplares(esDigital ? 0 : int.Parse(txb_Copias_frmLibros.Text.Trim()));
                VincularAutoresSeleccionados(id);

                LimpiarCampos();
                txb_Info_frmLibros.Text = "Los datos del libro han sido actualizados correctamente.";
            }
            catch (Exception)
            {
                txb_Info_frmLibros.Text = "Error al actualizar";
            }
        }

        public void EjecutarEliminar(StatusStrip barraEstado)
        {
            try
            {
                string idAEliminar = txtId.Text.Trim();

                if (string.IsNullOrWhiteSpace(idAEliminar))
                {
                    txb_Info_frmLibros.Text = "Debe especificar el ID del libro a eliminar.";
                    return;
                }

                DialogResult respuesta = MessageBox.Show($"¿Está seguro de eliminar el libro con ID {idAEliminar}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes)
                    return;   // Actividad 3.3: el usuario eligió "No", se aborta de inmediato

                if (respuesta == DialogResult.Yes)
                {
                    Libros libroAuxiliar = new Libros();
                    libroAuxiliar.EliminarRegistro(idAEliminar);

                    if (barraEstado != null && barraEstado.Items.Count > 0)
                    {
                        barraEstado.Items[0].Text = $"Libro con ID [{idAEliminar}] fue eliminado correctamente.";
                    }

                    LimpiarCampos();
                    txb_Info_frmLibros.Text = "El registro ha sido eliminado.";
                }
            }
            catch (Exception)
            {
                txb_Info_frmLibros.Text = "Error al eliminar";
            }
        }

        private void LimpiarCampos()
        {
            errorProvider1.Clear();   // quita los iconos de error al limpiar la pantalla
            txtId.Clear();
            txb_Titulo_frmLibros.Clear();
            txb_Costo_frmLibros.Clear();
            txb_RutaIma_frmLibros.Clear();
            txb_Copias_frmLibros.Clear();
            txb_Archivo_frmLibros.Clear();
            cmb_Tipo_frmLibros.SelectedIndex = 0;
            CargarAutoresDisponibles();

            if (cmb_Novedad_frmLibros.Items.Count > 0)
                cmb_Novedad_frmLibros.SelectedIndex = 0;

            if (cmb_Estado_frmLibros.Items.Count > 0)
                cmb_Estado_frmLibros.SelectedIndex = 0;
        }
    }
}