namespace Ashita.View
{
    using System.Linq;
    using System.Windows;
    using MahApps.Metro;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var theme = System.Configuration.ConfigurationManager.AppSettings["theme"] ?? "light";
            var accentConfig = System.Configuration.ConfigurationManager.AppSettings["accent"] ?? "blue";
            var accent = ThemeManager.DefaultAccents.FirstOrDefault(a => a.Name.ToLower() == accentConfig) ?? ThemeManager.DefaultAccents.First(a => a.Name == "Blue");
            MahApps.Metro.ThemeManager.ChangeTheme(this, accent, (theme.ToLower() == "dark") ? Theme.Dark : Theme.Light);
        }
    }
}
