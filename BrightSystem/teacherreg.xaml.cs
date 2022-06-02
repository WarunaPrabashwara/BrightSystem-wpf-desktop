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
    /// Interaction logic for teacherreg.xaml
    /// </summary>
    public partial class teacherreg : Window
    {
        public teacherreg() 
        {
            InitializeComponent();
            passwordBox1.Text = "25";
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        public void Reset()
        {
            textBoxFirstName.Text = "";
            textBoxLastName.Text = "";
            passwordBox1.Text = "25";
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
                teachers student2 = new teachers();
                student2.name = textBoxFirstName.Text;
                student2.phone = textBoxLastName.Text;
                student2.percentageOfHall = Int32.Parse(passwordBox1.Text) ;
                if (passwordBox1.Text.Length == 0)
                {
                    errormessage.Text = "Enter percentage that the hall require .";
                    passwordBox1.Focus();
                }
                else
                {
                    errormessage.Text = "";
 ;

                    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                    {
                        try
                        {

                            var output = cnn.Query<teachers>("select * from teachers where name = '" + student2.name + "';", new DynamicParameters());
                            List<teachers> nebuals = new List<teachers>();
                            nebuals = output.ToList();

                            if (nebuals.Count > 0)
                            {
                                MessageBox.Show("දැනටමත් ගුරුවරයාගේ නම අතුලත් කර ඇත ");
                            }
                            else
                            {
                                cnn.Execute("insert into teachers (name , phone , percentageOfHall  ) values( @name ,@phone , @percentageOfHall )", student2);
                                MessageBox.Show("ගුරුවරයා ඇතුලු කලා ");
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
}
