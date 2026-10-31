/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes:
 * 1. CONTRERAS Rodriguez Janis Isabel
 * 2. TORRES Barrera Jose Angel
 * 3. MACÍAS Cruz Meredith Miranda
 */
using System;

namespace Biblioteca.Models
{
    // Copia física individual de un libro (identificación única por ejemplar).
    public class Ejemplar
    {
        public string CodigoEjemplar { get; }
        public int IdLibro { get; }
        public bool Disponible { get; set; } = true;

        public Ejemplar(int idLibro, int numero)
        {
            if (idLibro <= 0)
                throw new ArgumentException("El ID del libro debe ser mayor a 0.");
            IdLibro = idLibro;
            CodigoEjemplar = $"L{idLibro:D4}-E{numero:D3}";
        }

        public override string ToString() => $"{CodigoEjemplar} ({(Disponible ? "Disponible" : "Prestado")})";
    }
}
