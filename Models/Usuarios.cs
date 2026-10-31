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
using System.Security.Cryptography;

namespace Biblioteca.Models
{
    public class Usuarios : Persona, IAlmacenamientoCRUD
    {
        private static List<Usuarios> _listaUsuarios = new List<Usuarios>();

        private int _librosPrestados;
        private decimal _multaAcumulada;
        private RolUsuario _rol = RolUsuario.Estudiante;
        private string _areaDepartamento = string.Empty;
        private string _passwordHash = string.Empty;
        private string _rutaImagen = string.Empty;

        private const int TamanoSal = 16;
        private const int TamanoHash = 32;
        private const int Iteraciones = 210000;

        public string Nombre
        {
            get => NombreCompleto;
            set => NombreCompleto = value;
        }

        public int LibrosPrestados
        {
            get => _librosPrestados;
            set
            {
                if (value < 0)
                    throw new ArgumentException("La cantidad de libros no puede ser negativa.");
                _librosPrestados = value;
            }
        }

        public decimal MultaAcumulada
        {
            get => _multaAcumulada;
            set
            {
                if (value < 0)
                    throw new ArgumentException("La multa acumulada no puede ser negativa.");
                _multaAcumulada = value;
            }
        }

        // Rol de acceso: define qué pantallas puede ver el usuario
        public RolUsuario Rol
        {
            get => _rol;
            set => _rol = value;
        }

        // Se mantiene por compatibilidad: ahora se deriva del rol
        public bool EsProfesor
        {
            get => _rol == RolUsuario.Docente;
            set
            {
                if (_rol == RolUsuario.Administrador) return;   // no degradar a un administrador
                _rol = value ? RolUsuario.Docente : RolUsuario.Estudiante;
            }
        }

        // Área (docentes) o carrera (estudiantes): campo general
        public string AreaDepartamento
        {
            get => _areaDepartamento;
            set => _areaDepartamento = value?.Trim() ?? string.Empty;
        }

        // Nunca se guarda la contraseña en texto plano: solo "sal:hash" (PBKDF2-SHA256, en Base64)
        public string PasswordHash => _passwordHash;

        public bool TieneContrasena => _passwordHash.Length > 0;

        // Estado de la cuenta (activa/inactiva) = EsActivo heredado de EntidadBase.
        // Una cuenta inactiva no puede iniciar sesión, pero conserva su historial de préstamos.
        public bool CuentaActiva
        {
            get => EsActivo;
            set => EsActivo = value;
        }

        public void EstablecerContrasena(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena) || contrasena.Length < 8)
                throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");

            byte[] sal = RandomNumberGenerator.GetBytes(TamanoSal);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);
            _passwordHash = Convert.ToBase64String(sal) + ":" + Convert.ToBase64String(hash);
        }

        public bool VerificarContrasena(string contrasena)
        {
            if (!TieneContrasena || string.IsNullOrEmpty(contrasena))
                return false;

            string[] partes = _passwordHash.Split(':');
            byte[] sal = Convert.FromBase64String(partes[0]);
            byte[] esperado = Convert.FromBase64String(partes[1]);
            byte[] calculado = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);
            return CryptographicOperations.FixedTimeEquals(esperado, calculado);
        }

        public string RutaImagen
        {
            get => _rutaImagen;
            set => _rutaImagen = string.IsNullOrWhiteSpace(value) ? "usuario_default.png" : value.Trim();
        }

        public Usuarios() : base()
        {
            this.LibrosPrestados = 0;
            this.MultaAcumulada = 0m;
            this.EsProfesor = false;
            this.RutaImagen = "usuario_default.png";
        }

        public Usuarios(int idUsuario, string nombre, int librosPrestados, decimal multaAcumulada, bool esProfesor)
            : base(idUsuario, nombre, 18, "correo@ejemplo.com")
        {
            this.LibrosPrestados = librosPrestados;
            this.MultaAcumulada = multaAcumulada;
            this.EsProfesor = esProfesor;
            this.RutaImagen = "usuario_default.png";
        }

        public Usuarios(int idPersona, string nombreCompleto, int edad, string correo, int librosPrestados, decimal multaAcumulada, bool esProfesor, string rutaImagen, bool estado)
            : base(idPersona, nombreCompleto, edad, correo)
        {
            this.LibrosPrestados = librosPrestados;
            this.MultaAcumulada = multaAcumulada;
            this.EsProfesor = esProfesor;
            this.RutaImagen = rutaImagen;
            this.EsActivo = estado;
        }

        public decimal CalcularMultaTotal()
        {
            return this.MultaAcumulada;
        }

        public decimal CalcularMultaTotal(int diasRetrasoNuevos)
        {
            if (diasRetrasoNuevos < 0)
                throw new ArgumentException("Los días de retraso no pueden ser negativos.");

            decimal tarifaDiariaPorMora = this.EsProfesor ? 10.0m : 15.0m;
            return this.MultaAcumulada + (diasRetrasoNuevos * tarifaDiariaPorMora);
        }

        public void InsertarRegistro(object objeto)
        {
            if (objeto is Usuarios usuario)
                _listaUsuarios.Add(usuario);
            else
                throw new ArgumentException("El objeto no es del tipo Usuarios.");
        }

        public object ConsultarRegistro(string id)
        {
            int idBuscado = int.Parse(id);
            return _listaUsuarios.Find(u => u.Id == idBuscado);
        }

        public void ActualizarRegistro(object objeto)
        {
            if (objeto is Usuarios usuarioActualizado)
            {
                Usuarios existente = _listaUsuarios.Find(u => u.Id == usuarioActualizado.Id);
                if (existente != null)
                {
                    int indice = _listaUsuarios.IndexOf(existente);
                    _listaUsuarios[indice] = usuarioActualizado;
                }
                else
                {
                    throw new ArgumentException("No se encontró el usuario a actualizar.");
                }
            }
            else
            {
                throw new ArgumentException("El objeto no es del tipo Usuarios.");
            }
        }

        public void EliminarRegistro(string id)
        {
            int idBuscado = int.Parse(id);
            Usuarios existente = _listaUsuarios.Find(u => u.Id == idBuscado);
            if (existente != null)
                _listaUsuarios.Remove(existente);
            else
                throw new ArgumentException("No se encontró el usuario a eliminar.");
        }

        public static List<Usuarios> ObtenerTodos() => _listaUsuarios;

        public override string ToString()
        {
            string tipoUsuario = Rol.ToString();
            string estadoStr = EsActivo ? "Activo" : "Inactivo";
            return $"[Usuario #{Id}] {NombreCompleto} ({tipoUsuario}) | Libros: {LibrosPrestados} | Multa: ${MultaAcumulada:F2} | Estado: {estadoStr}";
        }
    }
}