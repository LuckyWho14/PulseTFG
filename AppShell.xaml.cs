namespace PulseTFG
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            GoToAsync("//LoginPage").ConfigureAwait(false);

        }

    }
}
