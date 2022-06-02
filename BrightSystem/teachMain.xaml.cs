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
    /// Interaction logic for teachMain.xaml
    /// </summary>
    public partial class teachMain : Page
    {
        List<teachers> nebuals = new List<teachers>();


        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }



        public teachMain()
        {
            InitializeComponent();

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<teachers>("select * from teachers", new DynamicParameters());
                    nebuals = output.ToList();
                }
                catch (Exception ex)
                {
                }
            }


            for (int i = 0; i < nebuals.Count; i++)
            {
                datagridXml.Items.Add(new teachers()
                {
                    name = nebuals[i].name,
                    phone = nebuals[i].phone,
                    tid = nebuals[i].tid,
                    percentageOfHall = nebuals[i].percentageOfHall,
                });
            }


            }

 

        private void b1click(object sender, RoutedEventArgs e)
        {
            teachers st = datagridXml.SelectedItem as teachers;
            String studentname = st.name;
            MessageBox.Show(studentname);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            teacherreg teacherreg = new teacherreg();
            teacherreg.ShowDialog();
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
            teachers st = datagridXml.SelectedItem as teachers;
            int tid = st.tid;

            MessageBoxResult result = MessageBox.Show("confirm deletion " + st.name + " from teachers", "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        cnn.Execute("DELETE FROM teachers  WHERE tid=" + tid + "");
                        this.NavigationService.Refresh();
                        MessageBox.Show("teacher was deleted !  ");

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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            var tbx = sender as TextBox;
            if (tbx.Text != "")
            {


                var filtertext = nebuals.Where(x => x.name.ToLower().Contains(tbx.Text.ToLower())).ToList();

                var filtertext2 = nebuals.Where(x => x.tid.ToString().Contains(tbx.Text)).ToList();
                filtertext.AddRange(filtertext2);
                filtertext = filtertext.Distinct().ToList();

                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();


                for (int i = 0; i < filtertext.Count; i++)
                {
                    datagridXml.Items.Add(new teachers()
                    {
                        name = filtertext[i].name,
                        phone = filtertext[i].phone,
                        tid = filtertext[i].tid,
                        percentageOfHall = filtertext[i].percentageOfHall,
                    });

                }




            }
            else
            {
                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();

                for (int i = 0; i < nebuals.Count; i++)
                {
                    datagridXml.Items.Add(new teachers()
                    {
                        name = nebuals[i].name,
                        phone = nebuals[i].phone,
                        tid = nebuals[i].tid,
                        percentageOfHall = nebuals[i].percentageOfHall,
                    });

                }


            }
        }


        private void viewclick(object sender, RoutedEventArgs e)
        {
            teachers st = datagridXml.SelectedItem as teachers ;
            teacherprofile teacherprofile = new teacherprofile(st.tid, st.name, st.phone, st.percentageOfHall );
            teacherprofile.ShowDialog();
        }

        
 
    }
}
