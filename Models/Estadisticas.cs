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
    // Estadísticas globales de uso de la biblioteca (vista del administrador).
    public static class Estadisticas
    {
        public static string Resumen()
        {
            int activos = 0, devueltos = 0, vencidos = 0;
            foreach (Prestamos p in Prestamos.ObtenerTodos())
            {
                p.ActualizarEstado();
                if (p.Estado == EstadoPrestamo.Activo) activos++;
                else if (p.Estado == EstadoPrestamo.Devuelto) devueltos++;
                else vencidos++;
            }

            int fisicos = 0, digitales = 0;
            foreach (Libros l in Libros.ObtenerTodos())
            {
                if (l.TipoRecurso == TipoRecurso.Digital) digitales++; else fisicos++;
            }

            int estudiantes = 0, docentes = 0, admins = 0, inactivos = 0;
            foreach (Usuarios u in Usuarios.ObtenerTodos())
            {
                if (u.Rol == RolUsuario.Estudiante) estudiantes++;
                else if (u.Rol == RolUsuario.Docente) docentes++;
                else admins++;
                if (!u.EsActivo) inactivos++;
            }

            return $"Libros: {fisicos} físicos, {digitales} digitales\r\n" +
                   $"Usuarios: {estudiantes} estudiantes, {docentes} docentes, {admins} administradores ({inactivos} inactivos)\r\n" +
                   $"Préstamos: {activos} activos, {devueltos} devueltos, {vencidos} vencidos";
        }
    }
}
