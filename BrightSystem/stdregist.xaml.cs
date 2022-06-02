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
    /// Interaction logic for stdregist.xaml
    /// </summary>
    public partial class stdregist : Window
    {
        string yourstringname;
        List<string> dropdownar = new List<string>() ;

        public stdregist()
        {
            yourstringname = ""; 
            InitializeComponent();
            cmbColors.ItemsSource = dropdownar;
            cmbColors.SelectedIndex = -1 ;
            dbfunction();
        }

        public void dbfunction()
        {
             
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<batches>("select * from batches " , new DynamicParameters());
                    List<batches> batchess = new List<batches>();
                    batchess = output.ToList();
                    for(int i = 0; i < batchess.Count; i++)
                    {
                        dropdownar.Add(batchess[i].name );        
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

            private void cmbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            yourstringname =  (string)(cmbColors).SelectedItem   ;

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        public void Reset()
        {
            textBoxFirstName.Text = "";
            textBoxLastName.Text = "";
            cmbColors.SelectedIndex = -1;
            yourstringname = "";
             
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private static string LoadConnectionString( string id ="Default")
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
            else if (yourstringname == "" )
            {
                errormessage.Text = "Select the batch.";
                cmbColors.Focus();

            }
            else
            {
                studentmodal student2 = new studentmodal();
                student2.name  = textBoxFirstName.Text;
                student2.phone = textBoxLastName.Text;
                student2.batch = yourstringname;
                 

                    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                    {
                        try
                        {

                            var output = cnn.Query<student>("select * from student where name = '" + textBoxFirstName.Text + "';" , new DynamicParameters());
                            List<student> nebuals = new List<student>();
                            nebuals = output.ToList();
                              
                                if (nebuals.Count > 0)
                                {
                                    MessageBox.Show("සිසුවා දැනටමත් ඇතුලත් කර ඇත ");
                                }
                                else
                                {
                                    cnn.Execute("insert into student (name , phone , batch  ) values( @name ,@phone , @batch  )", student2);
                                    MessageBox.Show("student was entered sucessfully ");
                                }



                                                    
                        }
                        catch (Exception ex)
                        {
                        MessageBox.Show("error " + ex) ;
                        }
                    }



                    Reset();
                
            }
        }
        
    }

}
