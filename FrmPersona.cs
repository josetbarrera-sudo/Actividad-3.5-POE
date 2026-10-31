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
    public partial class FrmPersona : FrmBase, IPanelCRUD
    {
        public FrmPersona()
        {
            InitializeComponent();
            ConfigurarValidaciones();
        }

        // ================== ACTIVIDAD 3.1: EVENTOS DE VALIDACIÓN ==================
        // Conecta los manejadores heredados de FrmBase a los controles de este formulario.
        private void ConfigurarValidaciones()
        {
            // Nombre: solo letras + obligatorio
            txb_Nombre_frmPerso.KeyPress += SoloLetras_KeyPress;
            txb_Nombre_frmPerso.Validating += TextoObligatorio_Validating;
            txb_Nombre_frmPerso.ShortcutsEnabled = false;

            // Edad: solo dígitos + obligatorio
            txb_Edad_frmPerso.KeyPress += SoloNumeros_KeyPress;
            txb_Edad_frmPerso.Validating += TextoObligatorio_Validating;
            txb_Edad_frmPerso.ShortcutsEnabled = false;

            // Correo: sin espacios + formato válido
            txb_Correo_frmPerso.KeyPress += SinEspacios_KeyPress;
            txb_Correo_frmPerso.Validating += Correo_Validating;

            // Estado: debe haber una opción seleccionada
            cmb_Estado_frmPerso.Validating += ComboObligatorio_Validating;
        }

        public void EjecutarGuardar()
        {
            // Actividad 3.1: ValidateChildren() fuerza que TODOS los controles ejecuten su evento Validating.
            // Después se revisa con una bandera lógica si el ErrorProvider tiene algún error activo.
            bool hayErrores = !this.ValidateChildren() || HayErroresActivos(this);

            if (hayErrores)
            {
                txb_Info_frmPerso.Text = "⚠️ Hay campos con errores.\r\nCorrija los campos marcados con el icono rojo antes de guardar.";
                return;   // se bloquea el empaquetado hacia el Modelo
            }

            try
            {
                if (!int.TryParse(txtId.Text.Trim(), out int id))
                {
                    txb_Info_frmPerso.Text = "⚠️ ERROR DE VALIDACIÓN:\r\nPor favor, ingrese un código ID numérico válido.";
                    return;
                }

                string nombre = txb_Nombre_frmPerso.Text.Trim();
                int edad = int.TryParse(txb_Edad_frmPerso.Text.Trim(), out int e) ? e : 18;
                string correo = txb_Correo_frmPerso.Text.Trim();

                // NOTA: RutaImagen y Estado (cmb_Estado_frmPerso) no se guardan porque
                // la clase Persona no tiene esas propiedades. Ver aclaración pendiente.

                Persona nuevaPersona = new Persona(id, nombre, edad, correo);
                nuevaPersona.InsertarRegistro(nuevaPersona);

                txb_Info_frmPerso.Text = "La persona ha sido guardada exitosamente.";
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                txb_Info_frmPerso.Text = "Ocurrió un error al guardar: Error de Persistencia";
            }
        }

        public void EjecutarBuscar(string id, ErrorProvider alerta)
        {
            alerta.Clear();
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(id))
            {
                alerta.SetError(txtId, "Ingrese un ID de persona para realizar la búsqueda.");
                return;
            }

            try
            {
                if (!int.TryParse(id.Trim(), out int idBuscado))
                {
                    alerta.SetError(txtId, "El ID debe ser un número entero.");
                    return;
                }

                Persona personaEncontrada = (Persona)new Persona().ConsultarRegistro(id);

                if (personaEncontrada != null)
                {
                    txtId.Text = personaEncontrada.Id.ToString();
                    txb_Nombre_frmPerso.Text = personaEncontrada.NombreCompleto;
                    txb_Edad_frmPerso.Text = personaEncontrada.Edad.ToString();
                    txb_Correo_frmPerso.Text = personaEncontrada.Correo;
                    // cmb_Estado_frmPerso y txb_RutaIma_frmPerso no se pueden llenar:
                    // el modelo no expone esos datos.
                }
                else
                {
                    txb_Info_frmPerso.Text = "No se encontró ninguna persona registrada con el ID especificado.";
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
                    txb_Info_frmPerso.Text = "Ingrese un ID numérico válido para actualizar.";
                    return;
                }

                Persona personaActualizada = new Persona(
                    id,
                    txb_Nombre_frmPerso.Text.Trim(),
                    int.TryParse(txb_Edad_frmPerso.Text.Trim(), out int e) ? e : 18,
                    txb_Correo_frmPerso.Text.Trim()
                );

                personaActualizada.ActualizarRegistro(personaActualizada);
                LimpiarCampos();
                txb_Info_frmPerso.Text = "Los datos de la persona han sido actualizados correctamente.";
            }
            catch (Exception ex)
            {
                txb_Info_frmPerso.Text = "Error al actualizar";
            }
        }

        public void EjecutarEliminar(StatusStrip barraEstado)
        {
            try
            {
                string idAEliminar = txtId.Text.Trim();

                if (string.IsNullOrWhiteSpace(idAEliminar))
                {
                    txb_Info_frmPerso.Text = "Debe especificar el ID de la persona a eliminar.";
                    return;
                }

                DialogResult respuesta = MessageBox.Show($"¿Está seguro de eliminar a la persona con ID {idAEliminar}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes)
                    return;   // Actividad 3.3: el usuario eligió "No", se aborta de inmediato

                if (respuesta == DialogResult.Yes)
                {
                    Persona personaAuxiliar = new Persona();
                    personaAuxiliar.EliminarRegistro(idAEliminar);

                    if (barraEstado != null && barraEstado.Items.Count > 0)
                    {
                        barraEstado.Items[0].Text = $"Persona con ID [{idAEliminar}] fue eliminada correctamente.";
                    }

                    LimpiarCampos();
                    txb_Info_frmPerso.Text = "El registro ha sido eliminado.";
                }
            }
            catch (Exception ex)
            {
                txb_Info_frmPerso.Text = "Error al eliminar";
            }
        }

        private void LimpiarCampos()
        {
            errorProvider1.Clear();   // quita los iconos de error al limpiar la pantalla
            txtId.Clear();
            txb_Nombre_frmPerso.Clear();
            txb_Edad_frmPerso.Clear();
            txb_Correo_frmPerso.Clear();
            if (cmb_Estado_frmPerso.Items.Count > 0)
                cmb_Estado_frmPerso.SelectedIndex = 0;
            txb_RutaIma_frmPerso.Clear();
        }
    }
}