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
using System.Globalization;

namespace BrightSystem
{
    /// <summary>
    /// Interaction logic for teacherprofile.xaml
    /// </summary>
    public partial class teacherprofile : Window
    {
        teachers studentin = new teachers();
        public teacherprofile()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            editbut.Visibility = Visibility.Visible;
            textBoxLastName_Copy.Visibility = Visibility.Hidden;
            submitbut.Visibility = Visibility.Hidden;
        }

        private void editbut_Click(object sender, RoutedEventArgs e)
        {
            editbut.Visibility = Visibility.Hidden;
            textBoxLastName_Copy.Visibility = Visibility.Visible;
            submitbut.Visibility = Visibility.Visible;
            textBoxLastName_Copy.Text = "Enter new percentage";
        }

        private void submitbut_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxLastName_Copy.Text == "Enter new percentage")
            {
                MessageBox.Show("Enter new percentage");
            }
            else
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        int casee= Int16.Parse(textBoxLastName_Copy.Text);
                        cnn.Execute("UPDATE teachers SET percentageOfHall = '" + textBoxLastName_Copy.Text + "'  WHERE tid = '" + studentin.tid + "' ");
                        teacherprofile newWindow = new teacherprofile(studentin.tid, studentin.name, studentin.phone, casee );
                        Application.Current.MainWindow = newWindow;
                        newWindow.Show();
                        this.Close(); ;
                        
                    }
                    catch (Exception ex)
                    {

                    }
                    
                }

            }
        }

        public teacherprofile(int tid, string name, string phone, int percentageOfHall) : this()//you have to call the default constructor
        {
            this.studentin.tid =tid;
            this.studentin.name = name;
            this.studentin.phone = phone;
            this.studentin.percentageOfHall = percentageOfHall ;
            
            idc.Content = studentin.tid;
            namec.Content = studentin.name;
            phonec.Content = studentin.phone;
            baatch.Content = studentin.percentageOfHall;

            dbfunction();
            teacherpaymentshow();

        }
        List<teacherpayment> nebuals6 = new List<teacherpayment>();
        List<stuTeachClas> nebuals7 = new List<stuTeachClas>();
        List<studfeepaid> nebuals8 = new List<studfeepaid>();
        List<studfeepaid> teachertagewiyayuthu = new List<studfeepaid>();
        List<String> monthstopayfor = new List<String>();
        double gewiyayuthumudala = 0.00;
        double percentage =0.00 ;
        DateTime maxdatecurrent;
        public void teacherpaymentshow()
        {
            baatch_Copy.Content = "No any payments";
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                // methanion percentage eka hoyala denawa 
                try
                {
                    var output8 = cnn.Query<teachers>("select * from teachers where tid= '" + studentin.tid + "'", new DynamicParameters());
                    List<teachers> nebuals77 = new List<teachers>();
                    nebuals77 = output8.ToList();
                    percentage = (100 - nebuals77[0].percentageOfHall )*0.01;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error" + ex );
                }

                // methana teacher ta karala tyena payment set eka aran eken payment eka karala yena uparima date eka[masayak widihaa ] gannawa . payment ekak wath karala natham 20000101 gannw 
                // eeka maxdatecurrentvariable eka ta danwa 
                try
                {
                    var output8 = cnn.Query<teacherpayment>("select * from teacherpayment where tid=" + studentin.tid + "", new DynamicParameters());
                    nebuals6 = output8.ToList();
                    if (nebuals6.Count > 0) //teacher payment aragena eka parak hari 
                    {
                        maxdatecurrent = DateTime.ParseExact(nebuals6[0].datepaid, "yyyyMMdd", CultureInfo.InvariantCulture);
                        for (int l = 0; l < nebuals6.Count; l++)
                        {
                            if(maxdatecurrent < DateTime.ParseExact(nebuals6[l].datepaid, "yyyyMMdd", CultureInfo.InvariantCulture))
                            {
                                maxdatecurrent = DateTime.ParseExact(nebuals6[l].datepaid, "yyyyMMdd", CultureInfo.InvariantCulture);
                            }

                        }
                        maxdatecurrent = DateTime.ParseExact(maxdatecurrent.ToString("yyyy") + maxdatecurrent.ToString("MM"), "yyyyMM", CultureInfo.InvariantCulture);

                    }
                    else
                    {
                        maxdatecurrent = DateTime.ParseExact( "20000101" , "yyyyMMdd", CultureInfo.InvariantCulture); 
                    }

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var output = cnn.Query<stuTeachClas>("select * from stuTeachClas where tid=" + studentin.tid + "", new DynamicParameters());
                    nebuals7 = output.ToList(); // teacher ta adaala class okkoama gannwa 
                    if (nebuals7.Count > 0) // teacher ta classs tyenwa nm 
                    {
                        List <int> classlistofteacher = new List<int>();
                        // dan class tika unique karagamu 

                        for (int k = 0; k < nebuals7.Count; k++)
                        {
                            if (classlistofteacher.Contains(nebuals7[k].cid ))
                            {

                            }
                            else
                            {
                                classlistofteacher.Add(nebuals7[k].cid) ;
                            }

                        }

                        for (int k = 0; k < classlistofteacher.Count; k++) // hama class ekakatama iterae karanwa 
                        {
                            try
                            {
                                var output8 = cnn.Query<studfeepaid>("select * from studfeepaid where cid=" + classlistofteacher[k] + "", new DynamicParameters());
                                nebuals8 = output8.ToList();  // adaala class ekata [ loop eka athule di ] adaala payments tyenawada balanwa
                                if (nebuals8.Count >0) //student ge payments tyewna nm .. [ me masetama wenna oni naha . kalin resolve wela hari kamak naha . eka parak hari payment karala
                                {
                                    for (int l = 0; l < nebuals8.Count; l++)
                                    {
                                        DateTime formonthh = DateTime.ParseExact(nebuals8[l].paidon , "yyyyMMdd", CultureInfo.InvariantCulture) ;
                                        DateTime formoth2 = DateTime.ParseExact(formonthh.ToString("yyyy") + formonthh.ToString("MM"), "yyyyMM", CultureInfo.InvariantCulture);
                                        if (maxdatecurrent < formoth2 ) // teacher ta payment eka kala anthima masayata pasu thyyena siyalu student fee collections ekathu karala gannawa 
                                        {
                                            teachertagewiyayuthu.Add(nebuals8[l]);
                                            gewiyayuthumudala = gewiyayuthumudala + (Int16.Parse(nebuals8[l].amount))*percentage;
                                            if (monthstopayfor.Contains(formonthh.ToString("yyyy-MMM")))
                                            {

                                            }
                                            else
                                            {
                                                monthstopayfor.Add(formonthh.ToString("yyyy-MMM"));
                                            }
                                             
                                        }
                                        else
                                        {

                                        }

                                    }
                                    baatch_Copy.Content = gewiyayuthumudala ;
                                    phonec_Copy.Content =  String.Join(" ", monthstopayfor);
                                }
                                else
                                {
                                    
                                }

                            }
                            catch (Exception ex)
                            {
                            }
                    
                        }
                         
                    
                    }
                    else
                    {
                       
                    }
                }
                catch (Exception ex)
                {
                }
 
            }


        }
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }


        List<classes> nebuals = new List<classes>();
          
        public void dbfunction()
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<classes>("select * from classes where tid=" + studentin.tid + "" , new DynamicParameters());
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
                    cnameNtname = nebuals[i].cnameNtname,
                    cid = nebuals[i].cid ,
                    tid = nebuals[i].tid ,
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
                        cnameNtname = filtertext[i].cnameNtname,
                        cid = filtertext[i].cid,
                        tid = filtertext[i].tid,
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
                        cnameNtname = nebuals[i].cnameNtname,
                        cid = nebuals[i].cid,
                        tid = nebuals[i].tid,
                    });


                }


            }
        }




        private void delclick(object sender, RoutedEventArgs e)
        {
            classes st = datagridXml.SelectedItem as classes;
            int cid = st.cid;

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    cnn.Execute("DELETE FROM teachers  WHERE tid=" + cid + "");
                 }
                catch (Exception ex)
                {
                }
            }

            MessageBox.Show("student was deleted !  ");
        }

        private void viewclick(object sender, RoutedEventArgs e)
        {
            classes st = datagridXml.SelectedItem as classes;
            classesprofile classesprofile = new classesprofile(st.cid, st.cnameNtname, st.tid);
            classesprofile.ShowDialog();

        }

        private void refresh()
        {
            teacherprofile newWindow = new teacherprofile(studentin.tid , studentin.name , studentin.phone , studentin.percentageOfHall);
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }

        public void pay()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    cnn.Execute("insert into teacherpayment (tid , amount , datepaid ,formonth   ) values( '" + studentin.tid + "' , '" + gewiyayuthumudala + "' , '"+ DateTime.Now.ToString("yyyyMMdd") + "' , '" + String.Join(" ", monthstopayfor) + "'  )");
                    MessageBox.Show("ගෙවීම කලා");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error " + ex);
                }
            }

        }
        private void paybut(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("මෙම ගුරුවරයාට මුදල් ගෙවු බව තහවුරු කරන්න " + studentin.name, "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                MessageBoxResult result2 = MessageBox.Show("ගුරුවරයාට මෙම මාසයට මුදල් ගෙවූ පසු , සිසුන් නැවත මෙම මාසය තුල එම ගුරුවරයාට කරන කිසිදු ගෙවීමක් පරිගනකයේ සටහන් නොවේ . එම නිසා ගුරුවරයාට මුදල් ගෙවීමේදී මාසයේ අවසන් පන්ති දවසේ නම් ලකුනු කිරීමෙන් ද පසු මුදල් ගෙවීම සිදු කරන්න .  අද දිනය එම ගුරුවරයාගේ සියලු පන්ති වල මාසයේ අවසන් දිනය නම් සහ සියලු නම් ලකුනු කිරීම් අවසන් නම් පමනක් ok බොතම ඔබන්න  ", "WARNING!!", MessageBoxButton.YesNoCancel);
                if (result2 == MessageBoxResult.Yes)
                {
                    genReport genReport = new genReport();
                    genReport.RepGenerateFunc(studentin.tid, studentin.name);
                    genReport.createpdf(studentin.name);
                    MessageBox.Show(" රිපෝට් එක ගුරුවරයාගෙ නම  + මාසයේ නමින් රිපෝට් ෆෝල්ඩර් එක තුල ඩවුන්ලෝඩ් වී ඇත ");
                    pay();

                    refresh();
                    //     windowtoprint windowtoprint = new windowtoprint(studentin.tid, studentin.name );
                    //   windowtoprint.ShowDialog();
                }
                else if (result2 == MessageBoxResult.No)
                {

                }
                else
                {

                }


            
            }
            else if (result == MessageBoxResult.No)
            {

            }
            else
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            genReport genReport = new genReport();
            genReport.RepGenerateFunc(studentin.tid , studentin.name );
            genReport.createpdf(studentin.name);
            MessageBox.Show("රිපෝට් එක ගුරුවරයාගෙ නම  + මාසයේ නමින් රිපෝට් ෆෝල්ඩර් එක තුල ඩවුන්ලෝඩ් වී ඇත  ");
        }
    }

}
