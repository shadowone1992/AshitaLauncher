
namespace Ashita.View
{
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for LauncherView.xaml
    /// </summary>
    public partial class LauncherView : UserControl
    {
        public LauncherView()
        {
            InitializeComponent();
            
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Background = Brushes.Transparent;
            }
        }
    }
}
