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
    // Controla quién inició sesión y qué puede hacer según su rol.
    public static class Sesion
    {
        public static Usuarios? UsuarioActual { get; private set; }

        public static bool HaySesion => UsuarioActual != null;
        public static bool EsAdministrador => UsuarioActual?.Rol == RolUsuario.Administrador;

        // Devuelve null si el correo/contraseña no coinciden o la cuenta está inactiva.
        public static Usuarios? IniciarSesion(string correo, string contrasena)
        {
            foreach (Usuarios u in Usuarios.ObtenerTodos())
            {
                if (!string.Equals(u.Correo, correo.Trim(), StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!u.EsActivo || !u.VerificarContrasena(contrasena))
                    return null;

                UsuarioActual = u;
                return u;
            }
            return null;
        }

        public static void CerrarSesion() => UsuarioActual = null;

        // Crea el primer administrador para poder entrar por primera vez.
        public static void SembrarAdministradorInicial()
        {
            if (Usuarios.ObtenerTodos().Exists(u => u.Rol == RolUsuario.Administrador))
                return;

            Usuarios admin = new Usuarios(9999, "Administrador", 30, "admin@biblioteca.com", 0, 0m, false, "usuario_default.png", true);
            admin.Rol = RolUsuario.Administrador;
            admin.AreaDepartamento = "Biblioteca";
            admin.EstablecerContrasena("Admin1234");   // cambiarla después desde el módulo Usuarios
            admin.InsertarRegistro(admin);
        }
    }
}
