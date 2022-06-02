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
    /// Interaction logic for kidstoclass.xaml
    /// </summary>
    public partial class kidstoclass : Window
    {
         
        stuTeachClas stduent2 = new stuTeachClas();
        
        public kidstoclass()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
        }
        public kidstoclass  (int cid, int tid, string cnameNtname ) : this()
        {
            this.stduent2.cid = cid;
            this.stduent2.tid = tid;
           
            textBoxLastName.Content = cnameNtname ;
            dbquery();

        }

        List<student> students = new List<student>();


        public void dbquery()
        {



            
 
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        var output = cnn.Query<student>("select * from student ", new DynamicParameters());
                        students = output.ToList();
                    }
                    catch (Exception ex)
                    {
                    }
                }
 

            for (int i = 0; i < students.Count; i++)
            {
                datagridXml.Items.Add(new student()
                {
                    s_id = students[i].s_id,
                    name = students[i].name,
                    locofFingerPrint = students[i].locofFingerPrint,
                    batch = students[i].batch,
                    phone = students[i].phone,
                });
            }


        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            var tbx = sender as TextBox;
            if (tbx.Text != "")
            {


                var filtertext = students.Where(x => x.name.ToLower().Contains(tbx.Text.ToLower())).ToList();

                var filtertext2 = students.Where(x => x.s_id.ToString().Contains(tbx.Text)).ToList();
                filtertext.AddRange(filtertext2);
                filtertext = filtertext.Distinct().ToList();

                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();


                for (int i = 0; i < filtertext.Count; i++)
                {
                    datagridXml.Items.Add(new student()
                    {
                        s_id = filtertext[i].s_id,
                        name = filtertext[i].name,
                        locofFingerPrint = filtertext[i].locofFingerPrint,
                        batch = filtertext[i].batch,
                        phone = filtertext[i].phone,
                    });

                }




            }
            else
            {
                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();

                for (int i = 0; i < students.Count; i++)
                {
                    datagridXml.Items.Add(new student()
                    {
                        s_id = students[i].s_id,
                        name = students[i].name,
                        locofFingerPrint = students[i].locofFingerPrint,
                        batch = students[i].batch,
                        phone = students[i].phone,
                    });

                }


            }
        }



        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        private void addclick(object sender, RoutedEventArgs e)
        {
            stuTeachClas student3 = new stuTeachClas();
            student st = datagridXml.SelectedItem as student;
            this.stduent2.sid = st.s_id ;
             
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        var output = cnn.Query<stuTeachClas>("select * from stuTeachClas where cid=" + stduent2.cid + ""+ " and tid=" + stduent2.tid + ""+ " and sid=" + st.s_id + "", new DynamicParameters());
                        List <stuTeachClas> nebuals = new List<stuTeachClas>(); 
                        nebuals = output.ToList();
                        if(nebuals.Count > 0)
                        {
                            MessageBox.Show(" දැනටමත් මොහු පන්තියට ඇතුලු කර ඇත  ");
                        }
                        else
                        {

                        student3.sid = stduent2.sid ;
                        student3.tid = stduent2.tid ;    
                        student3.cid = stduent2.cid ;

                            cnn.Execute("insert into stuTeachClas (cid , tid , sid ) values( @cid ,@tid , @sid )", student3 );
                            MessageBox.Show("student was entered sucessfully");  
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error"+ ex);
                    }
                }


            
        }



    }
}
