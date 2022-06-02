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
using System.Data;
using System.Configuration;
using Dapper;
using System.Data.SQLite;

namespace BrightSystem
{
    /// <summary>
    /// Interaction logic for stdMain.xaml
    /// </summary>
    public partial class stdMain : Page
    {

        List<studentmodal> nebuals = new List<studentmodal>();


        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public stdMain()
        {

            InitializeComponent();
            

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            { 
                try
                {
                    var output = cnn.Query<studentmodal>("select * from student", new DynamicParameters());
                    nebuals = output.ToList();
                }
                catch (Exception ex)
                {
                }
            }


            for (int i = 0; i < nebuals.Count; i++)
            {
                datagridXml.Items.Add(new studentmodal()
                {
                    name = nebuals[i].name,
                    phone = nebuals[i].phone,
                    s_id = nebuals[i].s_id,
                    batch = nebuals[i].batch,
                });
          
            }
            
        }



        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            var tbx = sender as TextBox;
            if( tbx.Text != "")
            {


                var filtertext = nebuals.Where(x=> x.name.ToLower().Contains(tbx.Text.ToLower()) ).ToList();

                var filtertext2 = nebuals.Where(x => x.s_id.ToString().Contains(tbx.Text)).ToList();
                filtertext.AddRange(filtertext2);
                filtertext = filtertext.Distinct().ToList();

                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();


                for (int i = 0; i < filtertext.Count; i++)
                {
                    datagridXml.Items.Add(new studentmodal()
                    {
                        name = filtertext[i].name,
                        phone = filtertext[i].phone,
                        s_id = filtertext[i].s_id,
                        batch = filtertext[i].batch,
                    });

                }




            }
            else
            {
                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();

                for (int i = 0; i < nebuals.Count; i++)
                {
                    datagridXml.Items.Add(new studentmodal()
                    {
                        name = nebuals[i].name,
                        phone = nebuals[i].phone,
                        s_id = nebuals[i].s_id,
                        batch = nebuals[i].batch,
                    });

                }


            }
        }

 
        private void b1click(object sender, RoutedEventArgs e)
        {
            studentmodal st = datagridXml.SelectedItem as studentmodal;
            String studentname = st.name;
            MessageBox.Show(studentname);
        }

        private void newstudent(object sender, RoutedEventArgs e)
        {
            stdregist stdregister = new stdregist();
            stdregister.ShowDialog();
            refresh2();
        }
        private void refresh2()
        {
            this.NavigationService.Refresh();
        }

        private void refreshbut(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Refresh();

        }

        private void delclick(object sender, RoutedEventArgs e)
        {
            studentmodal st = datagridXml.SelectedItem as studentmodal;
            int s_id = st.s_id;

            MessageBoxResult result = MessageBox.Show("confirm deletion " + st.name + " from students", "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        cnn.Execute("DELETE FROM student  WHERE s_id=" + s_id + "");
                        this.NavigationService.Refresh();
                        MessageBox.Show("student was deleted !  ");
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else if (result == MessageBoxResult.No)
            {

            }
            else
            {

            }

            

            
        }

        private void viewclick(object sender, RoutedEventArgs e)
        {
            studentmodal st = datagridXml.SelectedItem as studentmodal;
            studentprofile studentprofile = new studentprofile(st.s_id  , st.name , st.phone , st.batch , st.locofFingerPrint );
            studentprofile.ShowDialog();
        }

        private void namemark_Click(object sender, RoutedEventArgs e)
        {
            makename markname  = new makename();
            markname.ShowDialog();
            refresh2();
        }

         
    
    }
}
