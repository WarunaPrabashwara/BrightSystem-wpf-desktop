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
    /// Interaction logic for classtokid.xaml
    /// </summary>
    public partial class classtokid : Window
    {
        string yourstringname = "";
        string studentname = "";
        int sid  ;
        List<string> dropdownar = new List<string>();
        public classtokid()
        {
            InitializeComponent();

            
        }
        public classtokid(int s_id, string name ) : this()
        {
            sid = s_id;
            studentname  = name;
            cmbColors.SelectedIndex = -1;
            textBoxLastName.Content = studentname;
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
                    var output = cnn.Query<classes>("select * from classes ", new DynamicParameters());
                    List<classes> teachers = new List<classes>();
                    teachers = output.ToList();
                    for (int i = 0; i < teachers.Count; i++)
                    {
                        dropdownar.Add(teachers[i].cid + "__" + teachers[i].cnameNtname + "__"+ teachers[i].tid );
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

            textBoxLastName.Content = studentname ;
            cmbColors.SelectedIndex = -1;
            yourstringname = "";

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
            if (yourstringname == ""   )
            {
                errormessage.Text = "Select a class.";
                cmbColors.Focus();
            }
 
            else
            {
                stuTeachClas student2 = new stuTeachClas();
                student2.cid = int.Parse(yourstringname.Split("__")[0])  ;
                student2.tid = int.Parse(yourstringname.Split("__")[3])  ;
                student2.sid = sid;

                errormessage.Text = "";
                errormessage.Text = "You have Registered successfully .  ";

                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        var output = cnn.Query<stuTeachClas>("select * from stuTeachClas where cid=" + student2.cid + ""+ " and tid=" + student2.tid + ""+ " and sid=" + student2.sid + "", new DynamicParameters());
                        List <stuTeachClas> nebuals = new List<stuTeachClas>(); 
                        nebuals = output.ToList();
                        if(nebuals.Count > 0)
                        {
                            errormessage.Text = "මේ ලමයා මේ පන්තියේ දැනටම ඉන්නවා .";
                        }
                        else
                        {
                            cnn.Execute("insert into stuTeachClas (cid , tid , sid ) values( @cid ,@tid , @sid )", student2);
                            errormessage.Text = "ලමයාව පන්තියට ඇතුල් කලා ";

                        }
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error" + ex);
                    }
                }



                Reset();

            }
        }


    }
}
