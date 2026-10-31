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
using System.Data;
using System.Drawing;
using System.Text;
using Biblioteca.Models;
using System.Windows.Forms;

namespace Biblioteca
{
    public partial class FrmPrincipal : Form
    {
        // Actividad 3.5 (MDI): la vista activa ya no se guarda en un campo; es la ventana hija
        // que tiene el foco dentro del contenedor. Si no es un módulo CRUD (p. ej. la consulta), es null.
        private IPanelCRUD? VistaActiva => this.ActiveMdiChild as IPanelCRUD;
        public FrmPrincipal()
        {
            InitializeComponent();
            AplicarPermisos();
        }

        // ================== CONTROL DE ACCESOS POR ROL ==================
        // Controles nuevos (se agregan desde el Diseñador, ver guía): btnEstadisticas y btnMisPrestamos
        private void AplicarPermisos()
        {
            Usuarios? actual = Sesion.UsuarioActual;
            if (actual == null)
                return;

            this.Text = $"Gestión de Biblioteca - {actual.NombreCompleto} ({actual.Rol})";

            bool esAdmin = Sesion.EsAdministrador;

            // Solo el administrador ve las estadísticas y los módulos de administración
            btnEstadisticas.Visible = esAdmin;
            btnEstadisticas.Click += btnEstadisticas_Click;

            // Actividad 3.4: el reporte masivo (proceso en segundo plano) es una función del administrador
            btnProcesoMasivo.Visible = esAdmin;

            // Alumnos y docentes ven sus propios préstamos y la fecha límite
            btnMisPrestamos.Visible = !esAdmin;
            btnMisPrestamos.Click += btnMisPrestamos_Click;

            if (esAdmin)
                return;

            // Alumnos y docentes: catálogo (Libros), consulta de préstamos y descarga de material digital
            btnAutores.Visible = false;
            btnUsuarios.Visible = false;
            btnPersona.Visible = false;
            btnGenero.Visible = false;
            btnReserva.Visible = false;
            btnMulta.Visible = false;
            btnEditorial.Visible = false;
            btnAdministrador.Visible = false;

            btnMasterGuardar.Visible = false;
            btnMasterActualizar.Visible = false;
            btnMasterEliminar.Visible = false;

            btnEdicionRapida.Visible = false;
        }

        private void btnEstadisticas_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(Estadisticas.Resumen(), "Estadísticas de la biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Actividad 3.4 / 3.5: la ventana del proceso concurrente ahora es una hija MDI de instancia única
        private void btnProcesoMasivo_Click(object? sender, EventArgs e)
        {
            AbrirHijo<FrmProcesosHilos>("Reporte masivo (hilos)");
        }

        private void btnMisPrestamos_Click(object? sender, EventArgs e)
        {
            Usuarios? actual = Sesion.UsuarioActual;
            if (actual == null)
                return;

            StringBuilder sb = new StringBuilder();
            foreach (Prestamos p in Prestamos.ObtenerTodos())
            {
                if (p.Usuario.Id != actual.Id)
                    continue;

                p.ActualizarEstado();
                sb.AppendLine($"Préstamo #{p.Id} | {p.Estado} | Fecha límite: {p.FechaEntrega:dd/MM/yyyy}");
            }

            if (sb.Length == 0)
                sb.Append("No tiene préstamos registrados.");

            MessageBox.Show(sb.ToString(), "Mis préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ================== ACTIVIDAD 3.5: ARQUITECTURA MDI ==================
        // FrmPrincipal es el formulario contenedor (IsMdiContainer = true, se configura en el Diseñador).
        // Cada módulo se abre como ventana hija (MdiParent = this) y solo puede existir UNA instancia.
        private void AbrirHijo<T>(string titulo) where T : Form, new()
        {
            // Algoritmo de instancia única: se recorre la colección de ventanas hijas abiertas
            foreach (Form hijo in this.MdiChildren)
            {
                if (hijo.GetType() == typeof(T))
                {
                    // Ya existe: se restaura si estaba minimizada, se trae al frente y NO se crea otra
                    if (hijo.WindowState == FormWindowState.Minimized)
                        hijo.WindowState = FormWindowState.Normal;

                    hijo.Activate();
                    return;
                }
            }

            T nuevo = new T();
            nuevo.Text = titulo;
            nuevo.StartPosition = FormStartPosition.WindowsDefaultLocation;   // cada ventana nueva se escalona
            nuevo.MdiParent = this;   // obligatorio ANTES de Show(): la ventana queda atrapada dentro del contenedor
            nuevo.Show();

            AjustarAlAreaMdi(nuevo);
        }

        // Los formularios se diseñaron grandes (1145 x 883); se reducen para que quepan en el área MDI.
        // Si el contenido es más grande, la propia ventana hija muestra barras de desplazamiento (ver FrmBase).
        private void AjustarAlAreaMdi(Form hijo)
        {
            MdiClient? area = this.Controls.OfType<MdiClient>().FirstOrDefault();
            if (area == null)
                return;

            int ancho = Math.Min(hijo.Width, Math.Max(400, area.ClientSize.Width - 40));
            int alto = Math.Min(hijo.Height, Math.Max(300, area.ClientSize.Height - 40));
            hijo.Size = new Size(ancho, alto);
        }

        private void mnuCascada_Click(object? sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void mnuMosaicoVertical_Click(object? sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void mnuMosaicoHorizontal_Click(object? sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        //Botones que llaman a las otras clases / ventanas
        private void btnAutores_Click(object sender, EventArgs e)
        {
            AbrirHijo<FrmAutor>("Autores");
        }
        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            AbrirHijo<FrmUsuario>("Usuarios");
        }

        private void btnLibros_Click(object sender, EventArgs e)
        {
            AbrirHijo<FrmLibros>("Libros");
        }

        private void btnPrestamos_Click(object sender, EventArgs e)
        {
            AbrirHijo<FrmPrestamo>("Préstamos");
        }

        private void btnPersona_Click(object sender, EventArgs e)
        {
            AbrirHijo<FrmPersona>("Persona");
        }

        private void btnGenero_Click(object sender, EventArgs e)
        {
            AbrirHijo<FrmGenero>("Género");
        }

        private void btnReserva_Click(object sender, EventArgs e)
        {
            AbrirHijo<FrmReserva>("Reserva");
        }

        private void btnMulta_Click(object sender, EventArgs e)
        {
            AbrirHijo<FrmMulta>("Multa");
        }

        private void btnEditorial_Click(object sender, EventArgs e)
        {
            AbrirHijo<FrmEditorial>("Editorial");
        }

        private void btnAdministrador_Click(object sender, EventArgs e)
        {
            AbrirHijo<FrmAdministrador>("Administrador");
        }


        //Botones maestros
        private void btnMasterGuardar_Click(object sender, EventArgs e)
        {
            if (VistaActiva != null)
            {
                VistaActiva.EjecutarGuardar();// Llamada limpia y polimórfica
            }
            else
            {
                MessageBox.Show("Abra y seleccione un módulo antes de guardar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnMasterActualizar_Click(object sender, EventArgs e)
        {
            if (VistaActiva != null)
            {
                DialogResult respuesta = MessageBox.Show("¿Desea actualizar los datos del registro?", "Confirmar actualización", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (respuesta != DialogResult.Yes)
                    return;

                VistaActiva.EjecutarActualizar();
            }
            else
            {
                MessageBox.Show("Abra y seleccione un módulo antes de actualizar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnMasterEliminar_Click(object sender, EventArgs e)
        {
            if (VistaActiva != null)
            {
                VistaActiva.EjecutarEliminar(statusStrip1);
            }
            else
            {
                MessageBox.Show("Abra y seleccione un módulo antes de eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnMasterBuscar_Click(object sender, EventArgs e)
        {
            if (VistaActiva != null)
            {
                VistaActiva.EjecutarBuscar(txtIdBusqueda.Text.Trim(), errorProvider1);
            }
            else
            {
                MessageBox.Show("Abra y seleccione un módulo antes de buscar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
