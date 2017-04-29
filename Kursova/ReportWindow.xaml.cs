using System.ComponentModel;
using System.Windows;

namespace Kursova
{
    /// <summary>
    ///     Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private readonly MainWindow window;

        public ReportWindow(MainWindow form)
        {
            InitializeComponent();
            window = form;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            window.IsEnabled = true;
        }
    }
}