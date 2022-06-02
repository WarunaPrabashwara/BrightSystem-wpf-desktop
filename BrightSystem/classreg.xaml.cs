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
    /// Interaction logic for classreg.xaml
    /// </summary>
    public partial class classreg : Window
    {
        string yourstringname;
        List<string> dropdownar = new List<string>();
        public classreg()
        {
            InitializeComponent();
            cmbColors.SelectedIndex = -1;
            textBoxLastName.Text = "ex : 2002 maths ";
            yourstringname = "";
            cmbColors.ItemsSource = dropdownar;
            dbfunction();

        }
        public void dbfunction()
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<teachers>("select * from teachers ", new DynamicParameters());
                    List<teachers> teachers = new List<teachers>();
                    teachers = output.ToList();
                    for (int i = 0; i < teachers.Count; i++)
                    {
                        dropdownar.Add(teachers[i].tid + "__" + teachers[i].name);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void cmbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            yourstringname = (string)(cmbColors).SelectedItem;

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        public void Reset()
        {
             
            textBoxLastName.Text = "ex : 2002 maths " ;
            cmbColors.SelectedIndex = -1;
            yourstringname = "";
            textBoxLastName_Copy.Text = "";
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
            if (textBoxLastName.Text == "ex : 2002 maths " )
            {
                errormessage.Text = "Enter name.";
                textBoxLastName.Focus();
            }
            else if (yourstringname == "")
            {
                errormessage.Text = "Select the teacher.";
                cmbColors.Focus();

            }
            else if(textBoxLastName_Copy.Text == "")
            {
                errormessage.Text = "enter the fee.";
                textBoxLastName_Copy.Focus();

            }
            else
            {
                classes student2 = new classes();
                student2.cnameNtname = textBoxLastName.Text + "__" + yourstringname.Split("__")[1];
                student2.tid = int.Parse( yourstringname.Split("__")[0] );
                feePrice fee = new feePrice();
                fee.fee = int.Parse(textBoxLastName_Copy.Text);
                 

                errormessage.Text = "";
                    

                    using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                    {
                        try
                        {


                            var output = cnn.Query<classes>("select * from classes where cnameNtname = '" + student2.cnameNtname + "';", new DynamicParameters());
                            List<classes> nebuals = new List<classes>();
                            nebuals = output.ToList();

                            if (nebuals.Count > 0)
                            {
                                MessageBox.Show("පන්තිය දැනටමත් ඇතුල් කර ඇත ");
                            }
                            else
                            {
                                cnn.Execute("insert into classes (cnameNtname , tid  ) values( @cnameNtname ,@tid )", student2);
                                  var output2 = cnn.Query<classes>("select * from classes where cnameNtname = '" + student2.cnameNtname + "';", new DynamicParameters());
                                  List<classes> nebuals2 = new List<classes>();
                                  nebuals2 =  output2.ToList();
                            fee.classid = nebuals2[0].cid;
                            cnn.Execute("insert into feePrice (classid , fee  ) values( @classid ,@fee )", fee);
                                MessageBox.Show("class was entered sucessfully  ");
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
