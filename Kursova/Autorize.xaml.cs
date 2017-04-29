using System.Linq;
using System.Windows;

namespace Kursova
{
    /// <summary>
    ///     Interaction logic for Autorize.xaml
    /// </summary>
    public partial class Autorize : Window
    {
        private readonly RepairEntities repair = new RepairEntities();

        public Autorize()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var is_find = false;
            var list = repair.Employees.ToList();
            foreach (var emp in list)
            {
                if (textBox1.Text.ToLower() == emp.SurName.ToLower() && textBox2.Text.ToLower() == emp.Phone.ToLower())
                {
                    is_find = true;
                    var window = new MainWindow(emp, this);
                    IsEnabled = false;
                    window.Show();
                    break;
                }
            }
            if (is_find == false)
                MessageBox.Show("Такого працівника не знайдено");
        }
    }
}