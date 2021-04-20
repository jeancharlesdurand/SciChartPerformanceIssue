using System.Windows;
using SciChart_RealtimewithCursors;

namespace SciChartExport
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SurfaceWindow win = new SurfaceWindow();
            win.Show();
        }
    }
}
