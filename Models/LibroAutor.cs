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
    // Tabla intermedia Libro-Autor (relación muchos a muchos).
    public class LibroAutor
    {
        private static List<LibroAutor> _vinculos = new List<LibroAutor>();

        public int IdLibro { get; }
        public int IdAutor { get; }

        public LibroAutor(int idLibro, int idAutor)
        {
            IdLibro = idLibro;
            IdAutor = idAutor;
        }

        public static void Vincular(int idLibro, int idAutor)
        {
            foreach (LibroAutor v in _vinculos)
            {
                if (v.IdLibro == idLibro && v.IdAutor == idAutor)
                    return;   // ya existe, no se duplica
            }
            _vinculos.Add(new LibroAutor(idLibro, idAutor));
        }

        public static void QuitarVinculosDeLibro(int idLibro)
        {
            _vinculos.RemoveAll(v => v.IdLibro == idLibro);
        }

        public static List<Autores> AutoresDeLibro(int idLibro)
        {
            List<Autores> resultado = new List<Autores>();
            foreach (LibroAutor v in _vinculos)
            {
                if (v.IdLibro != idLibro) continue;
                foreach (Autores a in Autores.ObtenerTodos())
                {
                    if (a.Id == v.IdAutor) { resultado.Add(a); break; }
                }
            }
            return resultado;
        }

        public static List<Libros> LibrosDeAutor(int idAutor)
        {
            List<Libros> resultado = new List<Libros>();
            foreach (LibroAutor v in _vinculos)
            {
                if (v.IdAutor != idAutor) continue;
                foreach (Libros l in Libros.ObtenerTodos())
                {
                    if (l.Id == v.IdLibro) { resultado.Add(l); break; }
                }
            }
            return resultado;
        }
    }
}
