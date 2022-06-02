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
using System.Configuration;
using System.Data.SQLite;
using System.Data;
using Dapper;

namespace BrightSystem
{
    /// <summary>
    /// Interaction logic for batchreg.xaml
    /// </summary>
    public partial class batchreg : Window
    {
        public batchreg()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        public void Reset()
        {
            textBoxFirstName.Text = "";
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }



        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxFirstName.Text.Length == 0)
            {
                errormessage.Text = "Enter name.";
                textBoxFirstName.Focus();
            }
            
            else
            {
                batches student2 = new batches();
                student2.name = textBoxFirstName.Text;

                    errormessage.Text = "";


                    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                    {
                        try
                        {
                            var output = cnn.Query<batches>("select * from batches where name='" + student2.name + "';" , new DynamicParameters());
                            List<batches> nebuals = new List<batches>();
                            nebuals = output.ToList();
                            if (nebuals.Count > 0)
                            {
                                MessageBox.Show("මෙම නම දැනටමත් ඇතුලු කර ඇහ ");
                            }
                            else
                            {
                            cnn.Execute("insert into batches (name  ) values( @name )", student2);
                            MessageBox.Show("ඇතුලු කරන ලදි ");
                            }

                        
                        }
                        catch (Exception ex)
                        {

                        }
                    }



                    Reset();
                
            }
        }

    }

}
