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
using System.Windows.Shapes;


using System.Data;
using System.Configuration;
using Dapper;
using System.Data.SQLite;

namespace BrightSystem
{
    /// <summary>
    /// Interaction logic for batchmanagement.xaml
    /// </summary>
    public partial class batchmanagement : Window
    {
        List<batches> nebuals = new List<batches>();

        public batchmanagement()
        {
            InitializeComponent();

            dbfunction();
        }

        public void dbfunction()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<batches>("select * from batches", new DynamicParameters());
                    nebuals = output.ToList();
                }
                catch (Exception ex)
                {
                }
            }


            for (int i = 0; i < nebuals.Count; i++)
            {
                datagridXml.Items.Add(new batches()
                {
                    bid = nebuals[i].bid,
                    name = nebuals[i].name,

                });


            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        private void delclick(object sender, RoutedEventArgs e)
        {
            batches st = datagridXml.SelectedItem as batches;
            int bid = st.bid;

            MessageBoxResult result = MessageBox.Show("confirm to delete?"+ st.name , "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        cnn.Execute("DELETE FROM batches  WHERE bid=" + bid + "");
                        batchmanagement newWindow = new batchmanagement();
                        Application.Current.MainWindow = newWindow;
                        newWindow.Show();
                        this.Close();
                        MessageBox.Show("batch was deleted !  ");
                    }
                    catch (Exception ex)
                    {
                    }
                }

                
            }
            else if(result == MessageBoxResult.No)
{
                 
            }
            else
            {
                 
            }


            
           
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            batchreg classesprofile = new batchreg();
            classesprofile.ShowDialog();
            refresh2(); 
        }

        private void refresh2()
        {
            datagridXml.Items.Clear();
            datagridXml.Items.Refresh();
            dbfunction();
        }

        private void refresh(object sender, RoutedEventArgs e)
        {
            datagridXml.Items.Clear();
            datagridXml.Items.Refresh();

            dbfunction(); 

        }
    }
}
