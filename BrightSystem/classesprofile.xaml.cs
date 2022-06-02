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
    /// Interaction logic for classesprofile.xaml
    /// </summary>
    public partial class classesprofile : Window
    {
        classes classin = new classes();
        List<teachers> teacher = new List<teachers>();
        List<feePrice> fee = new List<feePrice>(); 

        public classesprofile()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            editbut.Visibility = Visibility.Visible;
            textBoxLastName_Copy.Visibility = Visibility.Hidden;
            submitbut.Visibility = Visibility.Hidden;
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        List<student> students = new List<student>();
        public classesprofile(int cid , string cnameNtname , int tid) : this()
        {
            this.classin.cid = cid;
            this.classin.cnameNtname = cnameNtname;
            this.classin.tid = tid;
           


            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<teachers>("select * from teachers where tid= ' " + classin.tid  + "'" , new DynamicParameters());
                    teacher = output.ToList();
                }
                catch (Exception ex)
                {
                }
            }

            
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<feePrice>("select * from feePrice where classid=" + classin.cid + "", new DynamicParameters());
                    fee = output.ToList();
                }
                catch (Exception ex)
                {
                }
            }

            phonec_Copy.Content = classin.cnameNtname ;
            for (int i = 0; i < teacher.Count; i++)
            {
                tname.Content = teacher[i].name;
            }
            for (int i = 0; i < fee.Count; i++)
            {
                feess.Content = fee[0].fee;
            }
             


            List<stuTeachClas> stc = new List<stuTeachClas>();
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<stuTeachClas>("select * from stuTeachClas where cid=" + classin.cid + "", new DynamicParameters());
                    stc = output.ToList();
                }
                catch (Exception ex)
                {
                }
            }

            
            for (int i = 0; i < stc.Count; i++)
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        var output = cnn.Query<student>("select * from student where s_id=" + stc[i].sid + "", new DynamicParameters());
                        students.Add(output.ToList()[0] );   
                    }
                    catch (Exception ex)
                    {
                    }
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



        private void delclick(object sender, RoutedEventArgs e)
        {
            student st = datagridXml.SelectedItem as student;
            int s_id = st.s_id;


            MessageBoxResult result = MessageBox.Show("confirm deletion " + st.name + " from class", "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        cnn.Execute("DELETE FROM stuTeachClas  WHERE sid='" + s_id + "' and tid= '" + classin.tid + "' and cid= '" + classin.cid + "' ");
                        classesprofile newWindow = new classesprofile(classin.cid , classin.cnameNtname , classin.tid);
                        Application.Current.MainWindow = newWindow;
                        newWindow.Show();
                        this.Close();

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
            student st = datagridXml.SelectedItem as student;
            studentprofile studentprofile = new studentprofile(st.s_id, st.name, st.phone, st.batch, st.locofFingerPrint);
            studentprofile.ShowDialog();
        }

        private void addsttoclas(object sender, RoutedEventArgs e)
        {
            kidstoclass classesprofile = new kidstoclass(classin.cid , classin.tid , classin.cnameNtname );
            classesprofile.ShowDialog();
            refresh2();

        }

        private void refresh2()
        {
            classesprofile newWindow = new classesprofile(classin.cid, classin.cnameNtname, classin.tid);
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }

        private void refreshclock(object sender, RoutedEventArgs e)
        {
            classesprofile newWindow = new classesprofile(classin.cid , classin.cnameNtname , classin.tid );
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }

        private void editbut_Click(object sender, RoutedEventArgs e)
        {
            editbut.Visibility = Visibility.Hidden;
            textBoxLastName_Copy.Visibility = Visibility.Visible;
            submitbut.Visibility = Visibility.Visible;
            textBoxLastName_Copy.Text = "Enter new fee";
        }

        private void submitbut_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxLastName_Copy.Text == "Enter new fee")
            {
                MessageBox.Show("Enter new fee");
            }
            else
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        cnn.Execute("UPDATE feePrice SET fee = '" + textBoxLastName_Copy.Text + "'  WHERE classid = '" + classin.cid + "' ");
                    }
                    catch (Exception ex)
                    {

                    }
                    refresh2();
                }

            }    
        }
    }

}


