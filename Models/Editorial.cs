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
    public class Editorial : EntidadBase, IAlmacenamientoCRUD
    {
        // Simulación de persistencia en RAM (Actividad 2.3)
        private static List<Editorial> _listaEditoriales = new List<Editorial>();

        private string _nombre = string.Empty;
        private string _pais = string.Empty;
        private int _anioFundacion;
        private string _correoContacto = string.Empty;
        private string _rutaImagen = string.Empty;

        // Actividad 2.7: colección genérica tipada (relación Editorial 1 -> * Libros).
        // Es privada para que nadie pueda reemplazarla desde fuera.
        private List<Libros> _listaLibros;

        public string Nombre
        {
            get => _nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El nombre de la editorial no puede estar vacío.");
                _nombre = value.Trim();
            }
        }

        public string Pais
        {
            get => _pais;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El país no puede estar vacío.");
                _pais = value.Trim();
            }
        }

        public int AnioFundacion
        {
            get => _anioFundacion;
            set
            {
                if (value <= 0 || value > DateTime.Now.Year)
                    throw new ArgumentException("El año de fundación no es válido.");
                _anioFundacion = value;
            }
        }

        public string CorreoContacto
        {
            get => _correoContacto;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                    throw new ArgumentException("El correo de contacto no tiene un formato válido.");
                _correoContacto = value.Trim();
            }
        }

        public string RutaImagen
        {
            get => _rutaImagen;
            set => _rutaImagen = string.IsNullOrWhiteSpace(value) ? "editorial_default.png" : value.Trim();
        }

        // Propiedad de solo lectura: se puede consultar y llenar mediante
        // los métodos del modelo, pero no sobrescribir la lista completa.
        public List<Libros> ListaLibros
        {
            get { return _listaLibros; }
        }

        public Editorial() : base(1, DateTime.Now, true)
        {
            this.Nombre = "Editorial Sin Nombre";
            this.Pais = "Desconocido";
            this.AnioFundacion = DateTime.Now.Year;
            this.CorreoContacto = "contacto@editorial.com";
            this.RutaImagen = "editorial_default.png";
            _listaLibros = new List<Libros>(); // evita NullReferenceException
        }

        public Editorial(int idEditorial, string nombre, string pais, int anioFundacion, string correoContacto, string rutaImagen, bool estado)
            : base(idEditorial, DateTime.Now, estado)
        {
            this.Nombre = nombre;
            this.Pais = pais;
            this.AnioFundacion = anioFundacion;
            this.CorreoContacto = correoContacto;
            this.RutaImagen = rutaImagen;
            _listaLibros = new List<Libros>();
        }

        // Constructor parametrizado que recibe la colección capturada en el DataGridView.
        public Editorial(int idEditorial, string nombre, string pais, int anioFundacion, string correoContacto, string rutaImagen, bool estado, List<Libros> libros)
            : this(idEditorial, nombre, pais, anioFundacion, correoContacto, rutaImagen, estado)
        {
            if (libros != null)
            {
                foreach (Libros libro in libros)
                {
                    _listaLibros.Add(libro);
                }
            }
        }

        public int CalcularAntiguedad()
        {
            return DateTime.Now.Year - this.AnioFundacion;
        }

        public int CalcularAntiguedad(int anioReferencia)
        {
            if (anioReferencia < this.AnioFundacion)
                throw new ArgumentException("El año de referencia no puede ser anterior a la fundación.");
            return anioReferencia - this.AnioFundacion;
        }

        // ================== ACTIVIDAD 2.7: 7 FUNCIONES DE LA COLECCIÓN ==================
        // Sin lambdas ni predicados: solo ciclos foreach y condicionales if.

        // 1. Registrar elemento: inserta al final de la lista con el método nativo Add.
        public void RegistrarLibro(Libros libro)
        {
            if (libro == null)
                throw new ArgumentException("El libro no puede ser nulo.");
            _listaLibros.Add(libro);
        }

        // 2. Contar elementos: usa la propiedad nativa Count.
        public int ContarLibros()
        {
            return _listaLibros.Count;
        }

        // 3. Buscar por ID: recorre la lista y devuelve el objeto completo, o null si no existe.
        public Libros? BuscarLibroPorId(string id)
        {
            foreach (Libros libro in _listaLibros)
            {
                if (libro.Id.ToString() == id)
                    return libro;
            }
            return null;
        }

        // 4. Validar existencia: true si el ID ya está en la colección (evita duplicados).
        public bool ValidarExistencia(string id)
        {
            foreach (Libros libro in _listaLibros)
            {
                if (libro.Id.ToString() == id)
                    return true;
            }
            return false;
        }

        // 5. Eliminar por ID: busca la posición del elemento y lo extrae con RemoveAt(indice).
        public bool EliminarLibroPorId(string id)
        {
            for (int i = 0; i < _listaLibros.Count; i++)
            {
                if (_listaLibros[i].Id.ToString() == id)
                {
                    _listaLibros.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        // 6. Filtrar por estado: devuelve en una nueva List<Libros> solo los libros activos.
        public List<Libros> FiltrarLibrosActivos()
        {
            List<Libros> activos = new List<Libros>();
            foreach (Libros libro in _listaLibros)
            {
                if (libro.EsActivo == true)
                    activos.Add(libro);
            }
            return activos;
        }

        // 7. Vaciar lista: limpia la colección con Clear y el contador vuelve a cero.
        public void VaciarLibros()
        {
            _listaLibros.Clear();
        }

        // ================== CRUD (Actividad 2.3) ==================
        // Se reescribió con foreach para que el archivo no use lambdas.

        public void InsertarRegistro(object objeto)
        {
            if (objeto is Editorial editorial)
                _listaEditoriales.Add(editorial);
            else
                throw new ArgumentException("El objeto no es del tipo Editorial.");
        }

        public object ConsultarRegistro(string id)
        {
            int idBuscado = int.Parse(id);
            foreach (Editorial e in _listaEditoriales)
            {
                if (e.Id == idBuscado)
                    return e;
            }
            return null;
        }

        public void ActualizarRegistro(object objeto)
        {
            if (objeto is Editorial editorialActualizada)
            {
                for (int i = 0; i < _listaEditoriales.Count; i++)
                {
                    if (_listaEditoriales[i].Id == editorialActualizada.Id)
                    {
                        _listaEditoriales[i] = editorialActualizada;
                        return;
                    }
                }
                throw new ArgumentException("No se encontró la editorial a actualizar.");
            }
            else
            {
                throw new ArgumentException("El objeto no es del tipo Editorial.");
            }
        }

        public void EliminarRegistro(string id)
        {
            int idBuscado = int.Parse(id);
            foreach (Editorial e in _listaEditoriales)
            {
                if (e.Id == idBuscado)
                {
                    _listaEditoriales.Remove(e);
                    return;
                }
            }
            throw new ArgumentException("No se encontró la editorial a eliminar.");
        }

        public override string ToString()
        {
            string estadoStr = EsActivo ? "Activa" : "Inactiva";
            return $"[Editorial #{Id}] {Nombre} | País: {Pais} | Fundada: {AnioFundacion} | Estado: {estadoStr} | Libros: {ContarLibros()}";
        }
    }
}