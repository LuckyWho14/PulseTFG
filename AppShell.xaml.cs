namespace PulseTFG
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            GoToAsync("//InicioPage").ConfigureAwait(false);

        }

    }
}
