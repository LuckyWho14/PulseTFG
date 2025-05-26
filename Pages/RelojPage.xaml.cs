using System.Timers;

namespace PulseTFG.Pages;

public partial class RelojPage : ContentPage
{
    private DateTime inicio;
    private bool enEjecucion = false;
    private System.Timers.Timer cronometro;

    private int remainingMilliseconds;
    private bool isRunning = false;
    private bool isPaused = false;
    private CancellationTokenSource cts;



    public RelojPage()
    {
        InitializeComponent();

        cronometro = new System.Timers.Timer(10);
        cronometro.Elapsed += OnCronometroTick;
    }

    private void OnStartClicked(object sender, EventArgs e)
    {
        inicio = DateTime.Now;
        enEjecucion = true;
        cronometro.Start();
    }

    private void OnPauseClicked(object sender, EventArgs e)
    {
        enEjecucion = false;
    }

    private void OnStopClicked(object sender, EventArgs e)
    {
        enEjecucion = false;
        cronometro.Stop();
        TimeLabel.Text = "00:00.00";
    }

    private void OnCronometroTick(object sender, ElapsedEventArgs e)
    {
        if (!enEjecucion) return;

        var tiempo = DateTime.Now - inicio;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            TimeLabel.Text = tiempo.ToString(@"mm\:ss\.ff");
        });
    }

    // temporizador

    private void OnPlay10Min(object sender, EventArgs e) => IniciarTemporizador(10);
    private void OnPlay5Min(object sender, EventArgs e) => IniciarTemporizador(5);
    private void OnPlay3Min(object sender, EventArgs e) => IniciarTemporizador(3);
    private void OnPlay1Min(object sender, EventArgs e) => IniciarTemporizador(1);

    private void IniciarTemporizador(int minutos)
    {
        StopTimer(); // cancela si había uno activo
        StartTimer(minutos * 60 * 1000);
    }

    private void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
    {
        LabelTiempoPersonalizado.Text = $"{(int)e.NewValue} min";
    }

    private async void StartTimer(int totalMilliseconds)
    {
        remainingMilliseconds = totalMilliseconds;
        isRunning = true;
        isPaused = false;

        cts?.Cancel(); // cancela temporizador anterior si existía
        cts = new CancellationTokenSource();
        var token = cts.Token;

        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        while (remainingMilliseconds > 0 && !token.IsCancellationRequested)
        {
            if (!isPaused)
            {
                var remaining = totalMilliseconds - (int)stopwatch.ElapsedMilliseconds;
                if (remaining <= 0) break;

                int minutes = (remaining / 60000);
                int seconds = (remaining / 1000) % 60;
                int centiseconds = (remaining % 1000) / 10; // solo 2 dígitos

                TimerLabel.Text = $"{minutes:D2}:{seconds:D2}:{centiseconds:D2}";
            }

            await Task.Delay(50);
        }

        stopwatch.Stop();

        if (!token.IsCancellationRequested)
        {
            TimerLabel.Text = "00:00.00";
            isRunning = false;
        }
    }

    private void StopTimer()
    {
        cts?.Cancel();
        isRunning = false;
        isPaused = false;
        TimerLabel.Text = "00:00.00";
    }

    private void PauseTimer()
    {
        if (isRunning)
        {
            isPaused = !isPaused;
        }
    }

    // Eventos de botones

    private void OnPlayPersonalizado(object sender, EventArgs e)
    {
        int minutos = (int)StepperMinutos.Value;
        IniciarTemporizador(minutos);
    }

    private void OnPauseClickedT(object sender, EventArgs e)
    {
        PauseTimer();
    }

    private void OnStopClickedT(object sender, EventArgs e)
    {
        StopTimer();
    }
}
