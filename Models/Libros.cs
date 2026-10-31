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
    public class Libros : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Libros> _listaLibros = new List<Libros>();

        private string _titulo = string.Empty;
        private decimal _precioRentaDiaria;
        private bool _esNovedad;
        private string _rutaImagen = string.Empty;
        private TipoRecurso _tipoRecurso = TipoRecurso.Fisico;
        private string _rutaArchivoDigital = string.Empty;
        private List<Ejemplar> _ejemplares = new List<Ejemplar>();

        public string Titulo
        {
            get => _titulo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El título no puede estar vacío.");
                _titulo = value.Trim();
            }
        }

        public decimal PrecioRentaDiaria
        {
            get => _precioRentaDiaria;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("El precio de renta debe ser mayor a cero.");
                _precioRentaDiaria = value;
            }
        }

        // Físico o digital
        public TipoRecurso TipoRecurso
        {
            get => _tipoRecurso;
            set => _tipoRecurso = value;
        }

        // Ruta local o enlace del archivo (PDF / libro electrónico). Solo aplica a recursos digitales.
        public string RutaArchivoDigital
        {
            get => _rutaArchivoDigital;
            set
            {
                string limpio = value?.Trim() ?? string.Empty;
                if (TipoRecurso == TipoRecurso.Digital && limpio.Length == 0)
                    throw new ArgumentException("Un recurso digital necesita la ruta o enlace del archivo.");
                _rutaArchivoDigital = limpio;
            }
        }

        // Copias físicas individuales, cada una con su código único
        public IReadOnlyList<Ejemplar> Ejemplares => _ejemplares;

        // Total de copias físicas
        public int TotalEjemplares => _ejemplares.Count;

        // Copias físicas que no están prestadas en este momento
        public int CopiasDisponibles
        {
            get
            {
                int disponibles = 0;
                foreach (Ejemplar e in _ejemplares)
                {
                    if (e.Disponible) disponibles++;
                }
                return disponibles;
            }
        }

        public bool EsNovedad
        {
            get => _esNovedad;
            set => _esNovedad = value;
        }

        public string RutaImagen
        {
            get => _rutaImagen;
            set => _rutaImagen = string.IsNullOrWhiteSpace(value) ? "libro_default.png" : value.Trim();
        }

        public Libros() : base(1, DateTime.Now, true)
        {
            this.Titulo = "Sin Título";
            this.PrecioRentaDiaria = 20.0m;
            this.EsNovedad = false;
            this.RutaImagen = "libro_default.png";
            AjustarEjemplares(1);
        }

        // Constructor original (libro físico). Crea tantos ejemplares como copias se indiquen.
        public Libros(int id, string titulo, decimal precioRentaDiaria, int copias, bool esNovedad, string rutaImagen, bool estado)
            : this(id, titulo, precioRentaDiaria, copias, esNovedad, rutaImagen, estado, TipoRecurso.Fisico, string.Empty)
        {
        }

        public Libros(int id, string titulo, decimal precioRentaDiaria, int copias, bool esNovedad, string rutaImagen, bool estado,
                      TipoRecurso tipoRecurso, string rutaArchivoDigital)
            : base(id, DateTime.Now, estado)
        {
            this.Titulo = titulo;
            this.PrecioRentaDiaria = precioRentaDiaria;
            this.EsNovedad = esNovedad;
            this.RutaImagen = rutaImagen;
            this.TipoRecurso = tipoRecurso;
            this.RutaArchivoDigital = rutaArchivoDigital;

            if (tipoRecurso == TipoRecurso.Fisico)
                AjustarEjemplares(copias);
        }

        // Deja exactamente "cantidad" ejemplares; solo se quitan copias que no estén prestadas.
        public void AjustarEjemplares(int cantidad)
        {
            if (cantidad < 0)
                throw new ArgumentException("Las copias no pueden ser negativas.");

            while (_ejemplares.Count < cantidad)
                _ejemplares.Add(new Ejemplar(Id, _ejemplares.Count + 1));

            while (_ejemplares.Count > cantidad)
            {
                Ejemplar ultimo = _ejemplares[_ejemplares.Count - 1];
                if (!ultimo.Disponible)
                    throw new InvalidOperationException("No se pueden quitar copias que están prestadas.");
                _ejemplares.RemoveAt(_ejemplares.Count - 1);
            }
        }

        // Toma la primera copia libre y la marca como prestada (null si no hay).
        public Ejemplar? PrestarEjemplar()
        {
            foreach (Ejemplar e in _ejemplares)
            {
                if (e.Disponible)
                {
                    e.Disponible = false;
                    return e;
                }
            }
            return null;
        }

        public void DevolverEjemplar(Ejemplar ejemplar)
        {
            ejemplar.Disponible = true;
        }

        public List<Autores> ObtenerAutores() => LibroAutor.AutoresDeLibro(Id);

        public decimal CalcularCostoRenta(int dias)
        {
            if (dias <= 0)
                throw new ArgumentException("Los días deben ser mayores a cero.");

            return dias * this.PrecioRentaDiaria;
        }

        public decimal CalcularCostoRenta(int dias, decimal porcentajeDescuento)
        {
            if (porcentajeDescuento < 0 || porcentajeDescuento > 100)
                throw new ArgumentOutOfRangeException(nameof(porcentajeDescuento), "El descuento debe ser entre 0 y 100.");

            decimal costoBase = CalcularCostoRenta(dias);
            decimal descuento = costoBase * (porcentajeDescuento / 100m);
            return costoBase - descuento;
        }

        public void InsertarRegistro(object objeto)
        {
            if (objeto is Libros libro)
                _listaLibros.Add(libro);
            else
                throw new ArgumentException("El objeto no es del tipo Libros.");
        }

        public object? ConsultarRegistro(string id)
        {
            int idBuscado = int.Parse(id);
            foreach (Libros l in _listaLibros)
            {
                if (l.Id == idBuscado)
                    return l;
            }
            return null;
        }

        public void ActualizarRegistro(object objeto)
        {
            if (objeto is Libros libroActualizado)
            {
                Libros existente = _listaLibros.Find(l => l.Id == libroActualizado.Id);
                if (existente != null)
                {
                    int indice = _listaLibros.IndexOf(existente);
                    _listaLibros[indice] = libroActualizado;
                }
                else
                {
                    throw new ArgumentException("No se encontró el libro a actualizar.");
                }
            }
            else
            {
                throw new ArgumentException("El objeto no es del tipo Libros.");
            }
        }

        public void EliminarRegistro(string id)
        {
            int idBuscado = int.Parse(id);
            Libros existente = _listaLibros.Find(l => l.Id == idBuscado);
            if (existente != null)
            {
                _listaLibros.Remove(existente);
                LibroAutor.QuitarVinculosDeLibro(idBuscado);
            }
            else
                throw new ArgumentException("No se encontró el libro a eliminar.");
        }

        public static List<Libros> ObtenerTodos() => _listaLibros;

        public override string ToString()
        {
            string novedadStr = EsNovedad ? " (Novedad)" : "";
            string estadoStr = EsActivo ? "Disponible" : "No disponible";
            string existencia = TipoRecurso == TipoRecurso.Digital
                ? "Digital"
                : $"Copias: {CopiasDisponibles}/{TotalEjemplares}";
            return $"[ID: {Id}] {Titulo}{novedadStr} | {existencia} | Precio/Día: ${PrecioRentaDiaria:F2} | Estado: {estadoStr}";
        }
    }
}
