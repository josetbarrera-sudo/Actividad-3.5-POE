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

namespace Biblioteca.Models
{
    public class Prestamos : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Prestamos> _listaPrestamos = new List<Prestamos>();

        private Usuarios _usuario = new Usuarios();
        private List<DetallePrestamo> _detalles = new List<DetallePrestamo>();
        private DateTime _fechaEntrega;
        private DateTime? _fechaDevolucionReal;
        private EstadoPrestamo _estado = EstadoPrestamo.Activo;
        private string _rutaImagen = string.Empty;

        // Pila LIFO: historial de préstamos y devoluciones (el último en registrarse es el primero en deshacerse)
        private Stack<string> _historialCambios;

        public Usuarios Usuario { get => _usuario; set => _usuario = value ?? new Usuarios(); }

        // Detalle del préstamo (tabla intermedia): un préstamo puede tener varios libros
        public IReadOnlyList<DetallePrestamo> Detalles => _detalles;

        public Libros[] LibrosPrestados
        {
            get
            {
                Libros[] libros = new Libros[_detalles.Count];
                for (int i = 0; i < _detalles.Count; i++)
                    libros[i] = _detalles[i].Libro;
                return libros;
            }
        }

        // Fecha límite (estimada) de entrega
        public DateTime FechaEntrega { get => _fechaEntrega; set => _fechaEntrega = value; }

        // Momento real en que se devolvió; null mientras no se haya devuelto
        public DateTime? FechaDevolucionReal { get => _fechaDevolucionReal; set => _fechaDevolucionReal = value; }

        public EstadoPrestamo Estado { get => _estado; set => _estado = value; }

        // true si se devolvió en la fecha límite o antes; false si fue con retraso; null si aún no se devuelve
        public bool? DevolvioATiempo
        {
            get
            {
                if (_fechaDevolucionReal == null) return null;
                return _fechaDevolucionReal.Value.Date <= _fechaEntrega.Date;
            }
        }

        public int DiasRetraso
        {
            get
            {
                DateTime referencia = _fechaDevolucionReal ?? DateTime.Now;
                int dias = (referencia.Date - _fechaEntrega.Date).Days;
                return dias > 0 ? dias : 0;
            }
        }

        // Solo lectura: nadie fuera de la clase puede reemplazar o modificar la pila directamente
        public Stack<string> HistorialCambios { get { return _historialCambios; } }

        public string RutaImagen
        {
            get => _rutaImagen;
            set => _rutaImagen = string.IsNullOrWhiteSpace(value) ? "prestamo_default.png" : value.Trim();
        }

        public Prestamos() : base(1, DateTime.Now, true)
        {
            this._historialCambios = new Stack<string>();
            this._usuario = new Usuarios();
            this.FechaEntrega = DateTime.Now.AddDays(7);
            this.Estado = EstadoPrestamo.Activo;
            this.RutaImagen = "prestamo_default.png";
        }

        public Prestamos(int id, Usuarios usuario, Libros libroInicial, string rutaImagen, bool estado)
            : base(id, DateTime.Now, estado)
        {
            this._historialCambios = new Stack<string>();
            this._usuario = usuario ?? new Usuarios();
            this.FechaEntrega = DateTime.Now.AddDays(7);
            this.Estado = EstadoPrestamo.Activo;
            this.RutaImagen = rutaImagen;
            if (libroInicial != null)
                AgregarLibro(libroInicial);
        }

        // Agrega un libro al préstamo. Si es físico toma un ejemplar concreto; si es digital no lo necesita.
        public void AgregarLibro(Libros libro)
        {
            Ejemplar? ejemplar = null;
            if (libro.TipoRecurso == TipoRecurso.Fisico)
            {
                ejemplar = libro.PrestarEjemplar();
                if (ejemplar == null)
                    throw new InvalidOperationException($"No hay copias disponibles de \"{libro.Titulo}\".");
            }
            _detalles.Add(new DetallePrestamo(Id, libro, ejemplar));
        }

        // Marca el préstamo como devuelto y libera las copias físicas prestadas
        public void RegistrarDevolucion(DateTime fechaReal)
        {
            this.FechaDevolucionReal = fechaReal;
            this.Estado = EstadoPrestamo.Devuelto;
            foreach (DetallePrestamo d in _detalles)
            {
                if (d.Ejemplar != null)
                    d.Libro.DevolverEjemplar(d.Ejemplar);
            }
        }

        // Si sigue activo y ya pasó la fecha límite, pasa a Vencido
        public void ActualizarEstado()
        {
            if (Estado == EstadoPrestamo.Activo && DateTime.Now.Date > FechaEntrega.Date)
                Estado = EstadoPrestamo.Vencido;
        }

        // ===================== FUNCIONES DE CONTROL DE LA PILA =====================

        // 1. Apilar acción (Push): tipo = "PRESTAMO" o "DEVOLUCION"
        public void RegistrarAccion(string tipo)
        {
            string registro = tipo + " | " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            _historialCambios.Push(registro);

            // Se actualiza el estado del préstamo según la acción
            if (tipo == "DEVOLUCION")
            {
                this.Estado = EstadoPrestamo.Devuelto;
                this.FechaDevolucionReal = DateTime.Now;
            }
            else
            {
                this.Estado = EstadoPrestamo.Activo;
                this.FechaDevolucionReal = null;
                ActualizarEstado();
            }
        }

        // 2. Desapilar y revertir (Pop): valida primero que haya elementos y aplica el cambio inverso
        public string DeshacerUltimo()
        {
            if (_historialCambios.Count == 0)
                return null;

            string ultimo = _historialCambios.Pop();

            if (ultimo.StartsWith("DEVOLUCION"))
            {
                this.Estado = EstadoPrestamo.Activo;   // se deshace la devolución: vuelve a estar activo
                this.FechaDevolucionReal = null;
                ActualizarEstado();
            }
            else
            {
                this.Estado = EstadoPrestamo.Devuelto; // se deshace el préstamo: ya no está activo
                this.FechaDevolucionReal ??= DateTime.Now;
            }

            return ultimo;
        }

        // 3. Inspeccionar cima (Peek)
        public string VerCima()
        {
            if (_historialCambios.Count > 0)
                return _historialCambios.Peek();
            return "(sin acciones)";
        }

        // 4. Conteo de historial (Count)
        public int ContarHistorial()
        {
            return _historialCambios.Count;
        }

        // 5. Validar existencia de estado (ciclo foreach tradicional, sin LINQ ni lambdas)
        public bool ExisteEstado(string estado)
        {
            foreach (string registro in _historialCambios)
            {
                if (registro.StartsWith(estado))
                    return true;
            }
            return false;
        }

        // 6. Vaciar historial (Clear)
        public void VaciarHistorial()
        {
            _historialCambios.Clear();
        }

        // 7. Volcado a arreglo lineal: del cambio más nuevo al más viejo, sin alterar la pila original
        public string[] VolcarAArreglo()
        {
            string[] arreglo = new string[_historialCambios.Count];
            int i = 0;
            foreach (string registro in _historialCambios)
            {
                arreglo[i] = registro;
                i++;
            }
            return arreglo;
        }

        // ===========================================================================

        public string ImprimirP()
        {
            return $"Ticket: {Id} | Usuario: {Usuario.Nombre} | Estado: {Estado.ToString().ToUpper()}";
        }

        public string ImprimirP(object parametroExtra)
        {
            return ImprimirP();
        }

        public void InsertarRegistro(object objeto)
        {
            if (objeto is Prestamos prestamo)
                _listaPrestamos.Add(prestamo);
            else
                throw new ArgumentException("El objeto no es del tipo Prestamos.");
        }

        public object ConsultarRegistro(string id)
        {
            int idBuscado = int.Parse(id);
            return _listaPrestamos.Find(p => p.Id == idBuscado);
        }

        public void ActualizarRegistro(object objeto)
        {
            if (objeto is Prestamos prestamoActualizado)
            {
                Prestamos existente = _listaPrestamos.Find(p => p.Id == prestamoActualizado.Id);
                if (existente != null)
                {
                    int indice = _listaPrestamos.IndexOf(existente);
                    _listaPrestamos[indice] = prestamoActualizado;
                }
                else
                {
                    throw new ArgumentException("No se encontró el préstamo a actualizar.");
                }
            }
            else
            {
                throw new ArgumentException("El objeto no es del tipo Prestamos.");
            }
        }

        public void EliminarRegistro(string id)
        {
            int idBuscado = int.Parse(id);
            Prestamos existente = _listaPrestamos.Find(p => p.Id == idBuscado);
            if (existente != null)
                _listaPrestamos.Remove(existente);
            else
                throw new ArgumentException("No se encontró el préstamo a eliminar.");
        }

        public static List<Prestamos> ObtenerTodos() => _listaPrestamos;

        public override string ToString() => ImprimirP();
    }
}