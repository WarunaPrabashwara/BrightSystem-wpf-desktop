using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrightSystem
{
    /// <summary>
    /// Interaction logic for secondpage.xaml
    /// </summary>
    public partial class secondpage : Page
    {
        public secondpage()
        {
            InitializeComponent();
            MessageBoxResult result = MessageBox.Show(" අද දිනය " + System.DateTime.Now.ToString("dd/MMM/yyyy") + " නොවේ නම් පරිගනකයේ දිනය වෙනස් කරන්න " , "Confirmation");
        }
        private void student_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new stdMain();
        }

        private void teacher_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new teachMain();
        }

        private void classes_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new classMain();
        }

        private void extra_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new extraMain();
        }


    }
}
