/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes:
 * 1. CONTRERAS Rodriguez Janis Isabel
 * 2. MACÍAS Cruz Meredith Miranda
 * 3. TORRES Barrera Jose Angel
 */
using Biblioteca.Models;
using System;
using System.Windows.Forms;

namespace Biblioteca
{
    // Actividad 3.3: ventana NO modal (se abre con Show()).
    // Muestra el listado de libros y permanece abierta mientras se usa el formulario principal.
    public partial class FrmConsultaLibros : Form
    {
        public FrmConsultaLibros()
        {
            InitializeComponent();
            Actualizar();
        }

        // Vuelve a llenar la lista con los libros registrados
        public void Actualizar()
        {
            lvLibros.BeginUpdate();
            lvLibros.Items.Clear();

            foreach (Libros l in Libros.ObtenerTodos())
            {
                ListViewItem fila = new ListViewItem(l.Id.ToString());
                fila.SubItems.Add(l.Titulo);
                fila.SubItems.Add(l.PrecioRentaDiaria.ToString("C"));
                fila.SubItems.Add(l.EsNovedad ? "Sí" : "No");
                fila.SubItems.Add(l.EsActivo ? "Activo" : "Inactivo");
                lvLibros.Items.Add(fila);
            }

            lvLibros.EndUpdate();
            lblTotal.Text = $"Libros registrados: {lvLibros.Items.Count}";
        }

        private void btnActualizar_Click(object? sender, EventArgs e)
        {
            Actualizar();
        }

        private void btnCerrar_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
