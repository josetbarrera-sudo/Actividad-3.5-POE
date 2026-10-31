/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes:
 * 1. CONTRERAS Rodriguez Janis Isabel
 * 2. TORRES Barrera Jose Angel
 * 3. MACÍAS Cruz Meredith Miranda
 */
using System.Globalization;

namespace Biblioteca
{
    // Datos que viajan del hilo secundario al hilo de la interfaz mediante IProgress<T>.
    public sealed record ProgresoReporte(int Indice, int Total, string Usuario, string Libro,
                                         int DiasRetraso, decimal Multa, string Estado);

    // ACTIVIDAD 3.4 - Procesos concurrentes e hilos de ejecución.
    //
    // Proceso pesado identificado en la lógica de negocio: "Reporte masivo de circulación",
    // que calcula días de retraso y multa de N préstamos (simulación de procesamiento masivo).
    //
    //  * btnEjecutarLineal : ejecuta el proceso en el hilo de la interfaz (UI Thread) -> la ventana se congela.
    //  * btnIniciar        : ejecuta el MISMO proceso con async/await + Task.Run (segundo plano) -> la ventana responde.
    //  * btnCancelar       : cancela con CancellationTokenSource de forma segura (sin matar el hilo).
    //
    // Sincronización segura (nunca se usa CheckForIllegalCrossThreadCalls = false):
    //  - IProgress<T> / Progress<T>  -> barra de progreso, porcentaje y filas del DataGridView.
    //  - InvokeRequired + BeginInvoke -> texto del Label de estado (ActualizarEstadoSeguro).
    public partial class FrmProcesosHilos : Form
    {
        private const int PausaPorRegistroMs = 20;   // simula el costo de cada registro (E/S, consulta, etc.)

        private static readonly string[] Usuarios =
        {
            "Ana López", "Luis Hernández", "María Gómez", "Carlos Ramírez", "Sofía Torres",
            "Diego Vargas", "Valeria Cruz", "Jorge Medina", "Paola Núñez", "Mateo Salazar"
        };

        private static readonly string[] Libros =
        {
            "Cien años de soledad", "El principito", "Don Quijote de la Mancha", "Pedro Páramo",
            "Rayuela", "La sombra del viento", "Fahrenheit 451", "El túnel"
        };

        private CancellationTokenSource? _cts;
        private bool _ejecutando;
        private bool _cerrando;
        private int _clics;

        public FrmProcesosHilos()
        {
            InitializeComponent();
            tmrReloj_Tick(this, EventArgs.Empty);
            lblEstado.Text = $"Listo. Hilo de la interfaz: #{Environment.CurrentManagedThreadId}.";
        }

        // ================== PASO 2: TAREA EN SEGUNDO PLANO (async/await + Task.Run) ==================
        private async void btnIniciar_Click(object? sender, EventArgs e)
        {
            if (_ejecutando)
                return;

            int total = (int)nudRegistros.Value;
            PrepararEjecucion(total);

            _cts = new CancellationTokenSource();
            CancellationTokenSource cts = _cts;
            CancellationToken token = cts.Token;

            // Progress<T> se crea en el hilo de la interfaz: su callback (MostrarProgreso)
            // siempre se ejecuta de vuelta en el UI Thread, por eso es seguro tocar controles ahí.
            IProgress<ProgresoReporte> progreso = new Progress<ProgresoReporte>(MostrarProgreso);

            try
            {
                // Todo el código pesado (ciclo y cálculos) vive dentro de Task.Run -> hilo del ThreadPool.
                decimal totalMultas = await Task.Run(() => GenerarReporte(total, progreso, token), token);

                lblEstado.Text = $"Proceso terminado: {total} registros. Total de multas: {totalMultas.ToString("C2", CultureInfo.CurrentCulture)}.";
            }
            catch (OperationCanceledException)
            {
                lblEstado.Text = "Proceso cancelado de forma segura por el usuario.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error en el proceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblEstado.Text = "El proceso terminó con un error.";
            }
            finally
            {
                _cts = null;
                cts.Dispose();
                FinalizarEjecucion();
            }
        }

        // ================== PASO 1: COMPORTAMIENTO LINEAL (demostración del congelamiento) ==================
        // Solo sirve para documentar el problema: el proceso corre en el hilo de la interfaz,
        // por lo que la ventana no se puede mover ni responde a otros botones hasta que termina.
        private void btnEjecutarLineal_Click(object? sender, EventArgs e)
        {
            if (_ejecutando)
                return;

            int total = (int)nudRegistros.Value;
            double segundos = total * PausaPorRegistroMs / 1000.0;

            DialogResult r = MessageBox.Show(
                $"Esta es una DEMOSTRACIÓN del problema.\n\nEl proceso se ejecutará en el hilo de la interfaz y la ventana se congelará unos {segundos:N0} segundos.\n\n¿Continuar?",
                "Ejecución lineal (sin hilos)", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (r != DialogResult.OK)
                return;

            PrepararEjecucion(total);
            IProgress<ProgresoReporte> progreso = new Progress<ProgresoReporte>(MostrarProgreso);

            // Llamada SÍNCRONA en el UI Thread: nadie atiende los mensajes de Windows mientras dura.
            decimal totalMultas = GenerarReporte(total, progreso, CancellationToken.None);

            lblEstado.Text = $"Proceso lineal terminado ({total} registros). Total de multas: {totalMultas.ToString("C2", CultureInfo.CurrentCulture)}.";
            FinalizarEjecucion();
        }

        // ================== PASO 4: CANCELACIÓN SEGURA ==================
        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            if (_cts == null || _cts.IsCancellationRequested)
                return;

            _cts.Cancel();                      // solo solicita la cancelación (cooperativa)
            btnCancelar.Enabled = false;
            lblEstado.Text = "Cancelando...";
        }

        // ================== CÓDIGO PESADO (se ejecuta SOLO en segundo plano) ==================
        // Corre en un hilo del ThreadPool cuando se llama desde Task.Run. NO toca controles directamente.
        private decimal GenerarReporte(int total, IProgress<ProgresoReporte> progreso, CancellationToken token)
        {
            Random azar = new Random(2026);
            decimal acumulado = 0m;

            for (int i = 1; i <= total; i++)
            {
                token.ThrowIfCancellationRequested();   // punto de cancelación cooperativa

                // Cálculo matemático pesado (simula el procesamiento de cada préstamo).
                double suma = 0;
                for (int k = 1; k <= 40000; k++)
                    suma += Math.Sqrt(k) * Math.Sin(k);

                string usuario = Usuarios[(int)(Math.Abs(suma) * 1000 % Usuarios.Length)];
                string libro = Libros[azar.Next(Libros.Length)];
                int dias = azar.Next(0, 16);
                decimal multa = dias * 5m;
                string estado = dias == 0 ? "A tiempo" : dias <= 7 ? "Retraso leve" : "Retraso grave";
                acumulado += multa;

                // Actualización segura vía IProgress<T> (el callback corre en el hilo de la interfaz).
                progreso.Report(new ProgresoReporte(i, total, usuario, libro, dias, multa, estado));

                // Actualización segura vía InvokeRequired / BeginInvoke.
                if (i % 25 == 0 || i == total)
                    ActualizarEstadoSeguro($"Procesando registro {i} de {total}... (ejecutándose en el hilo #{Environment.CurrentManagedThreadId})");

                // Espera que se interrumpe al instante si se cancela (mejor que Thread.Sleep).
                if (token.WaitHandle.WaitOne(PausaPorRegistroMs))
                    token.ThrowIfCancellationRequested();
            }

            return acumulado;
        }

        // ================== PASO 3: ACTUALIZACIÓN SEGURA DE CONTROLES (THREAD-SAFE) ==================
        // Patrón InvokeRequired + BeginInvoke: si se llama desde un hilo externo, se delega al hilo de la interfaz.
        private void ActualizarEstadoSeguro(string texto)
        {
            if (IsDisposed || Disposing)
                return;

            try
            {
                if (lblEstado.InvokeRequired)
                {
                    lblEstado.BeginInvoke(new Action<string>(ActualizarEstadoSeguro), texto);
                    return;
                }

                lblEstado.Text = texto;
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }   // el formulario se cerró mientras corría el hilo
        }

        // Callback de Progress<T>: se ejecuta siempre en el hilo de la interfaz.
        private void MostrarProgreso(ProgresoReporte p)
        {
            if (IsDisposed)
                return;

            pbProgreso.Value = Math.Min(p.Indice, pbProgreso.Maximum);
            lblPorcentaje.Text = $"{p.Indice * 100 / p.Total} %";

            int fila = dgvResultados.Rows.Add(p.Indice, p.Usuario, p.Libro, p.DiasRetraso,
                                              p.Multa.ToString("C2", CultureInfo.CurrentCulture), p.Estado);
            dgvResultados.FirstDisplayedScrollingRowIndex = fila;
        }

        // ================== AUXILIARES DE INTERFAZ ==================
        private void PrepararEjecucion(int total)
        {
            dgvResultados.Rows.Clear();
            pbProgreso.Minimum = 0;
            pbProgreso.Maximum = total;
            pbProgreso.Value = 0;
            lblPorcentaje.Text = "0 %";

            btnIniciar.Enabled = false;
            btnEjecutarLineal.Enabled = false;
            nudRegistros.Enabled = false;
            btnCancelar.Enabled = true;
            _ejecutando = true;
        }

        private void FinalizarEjecucion()
        {
            _ejecutando = false;
            btnIniciar.Enabled = true;
            btnEjecutarLineal.Enabled = true;
            nudRegistros.Enabled = true;
            btnCancelar.Enabled = false;

            if (_cerrando)
                Close();
        }

        // ================== PRUEBA DE INTERFAZ RESPONSIVA ==================
        private void btnContador_Click(object? sender, EventArgs e)
        {
            _clics++;
            lblClics.Text = $"Clics: {_clics}";
        }

        private void tmrReloj_Tick(object? sender, EventArgs e)
        {
            lblReloj.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);
        }

        private void btnCerrar_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void FrmProcesosHilos_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Si se cierra con el proceso activo: se cancela de forma segura y se cierra al terminar.
            if (_ejecutando && _cts != null)
            {
                _cerrando = true;
                _cts.Cancel();
                e.Cancel = true;
            }
        }
    }
}
