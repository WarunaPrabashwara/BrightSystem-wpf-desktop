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
    /// Interaction logic for classMain.xaml
    /// </summary>
    public partial class classMain : Page
    {
        List<classes> nebuals = new List<classes>();

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
        public classMain()
        {
            InitializeComponent();
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<classes>("select * from classes", new DynamicParameters());
                    nebuals = output.ToList();
                }
                catch (Exception ex)
                {
                }
            }


            for (int i = 0; i < nebuals.Count; i++)
            {
                datagridXml.Items.Add(new classes()
                {
                    cid = nebuals[i].cid,
                    tid = nebuals[i].tid,
                    cnameNtname = nebuals[i].cnameNtname,
                     
                });


            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            var tbx = sender as TextBox;
            if (tbx.Text != "")
            {


                var filtertext = nebuals.Where(x => x.cnameNtname.ToLower().Contains(tbx.Text.ToLower())).ToList();

                var filtertext2 = nebuals.Where(x => x.cid.ToString().Contains(tbx.Text)).ToList();
                filtertext.AddRange(filtertext2);
                filtertext = filtertext.Distinct().ToList();

                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();


                for (int i = 0; i < filtertext.Count; i++)
                {
                    datagridXml.Items.Add(new classes()
                    {
                        cid = filtertext[i].cid,
                        tid = filtertext[i].tid,
                        cnameNtname = filtertext[i].cnameNtname,

                    });



                }




            }
            else
            {
                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();

                for (int i = 0; i < nebuals.Count; i++)
                {
                    datagridXml.Items.Add(new classes()
                    {
                        cid = nebuals[i].cid,
                        tid = nebuals[i].tid,
                        cnameNtname = nebuals[i].cnameNtname,

                    });



                }


            }
        }

        private void updateeclick(object sender, RoutedEventArgs e)
        {

        }


        private void delclick(object sender, RoutedEventArgs e)
        {
            classes st = datagridXml.SelectedItem as classes;
            int cid = st.cid;

            MessageBoxResult result = MessageBox.Show("confirm deletion " + st.cnameNtname , "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        cnn.Execute("DELETE FROM classes  WHERE cid=" + cid + "");
                        this.NavigationService.Refresh();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                MessageBox.Show("student was deleted !  ");
            }
            else if(result == MessageBoxResult.No)
            {
                 
            }
            else
            {
                  
            }


            
        }

        private void viewclick(object sender, RoutedEventArgs e)
        {
            classes st = datagridXml.SelectedItem as classes;
            classesprofile classesprofile = new classesprofile(st.cid, st.cnameNtname, st.tid);
            classesprofile.ShowDialog();

        }

        private void newclass(object sender, RoutedEventArgs e)
        {
            classreg classreg = new classreg();
            classreg.ShowDialog();
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

    }
}
