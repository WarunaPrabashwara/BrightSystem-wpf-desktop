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
    /// Interaction logic for studentprofile.xaml
    /// </summary>
    public partial class studentprofile : Window
    {
        studentmodal studentin = new studentmodal();
        stuTeachClas stuTeachClas = new stuTeachClas();
        List<stuTeachClas> stuTeachClasAray = new List<stuTeachClas>();
        classes  classes = new classes();
        List<classes> classAray = new List<classes>();
        List<List<classes>> arrOfclassAray = new List<List<classes>>();
        studfeepaid studfeepaid = new studfeepaid();
        List<studfeepaid> studfeepaidAray = new List<studfeepaid>();

        string monthtext = DateTime.Now.ToString("MMM yyyy");

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }


        public  studentprofile(int s_id, string name, string phone, string batch, string locofFingerPrint) : this()//you have to call the default constructor
        {
            this.studentin.s_id = s_id;
            this.studentin.name = name;
            this.studentin.phone = phone;
            this.studentin.batch = batch;
            this.studentin.locofFingerPrint = locofFingerPrint;
            idc.Content = studentin.s_id;
            namec.Content = studentin.name;
            phonec.Content = studentin.phone;
            baatch.Content = studentin.batch;

            dbfunction();
            hastopayformonth();
            displaydata();

        }

        public studentprofile( )
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            addfinger.Visibility = Visibility.Visible;
            changefinger.Visibility = Visibility.Hidden;


        }


        public void displaydata()
            {
            if(arrOfclassAray.Count >0)
            {
                for (int i = 0; i < arrOfclassAray.Count; i++)
                {
                    datagridXml.Items.Add(new classes()
                    {
                        cnameNtname = arrOfclassAray[i][0].cnameNtname,
                        cid = arrOfclassAray[i][0].cid,
                        tid = arrOfclassAray[i][0].tid,
                        hastopayfor = arrOfclassAray[i][0].hastopayfor,
                    });
                }
            }
            

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            var tbx = sender as TextBox;
            if (tbx.Text != "")
            {


                var filtertext = arrOfclassAray.Where(x => x[0].cnameNtname.ToLower().Contains(tbx.Text.ToLower())).ToList();

                var filtertext2 = arrOfclassAray.Where(x => x[0].cid.ToString().Contains(tbx.Text)).ToList();
                filtertext.AddRange(filtertext2);
                filtertext = filtertext.Distinct().ToList();

                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();


                for (int i = 0; i < filtertext.Count; i++)
                {
                    datagridXml.Items.Add(new classes()
                    {
                        cnameNtname = filtertext[i][0].cnameNtname,
                        cid = filtertext[i][0].cid,
                        tid = filtertext[i][0].tid,
                        hastopayfor = arrOfclassAray[i][0].hastopayfor,
                    });


                }




            }
            else
            {
                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();

                for (int i = 0; i < arrOfclassAray.Count; i++)
                {
                    datagridXml.Items.Add(new classes()
                    {
                        cnameNtname = arrOfclassAray[i][0].cnameNtname,
                        cid = arrOfclassAray[i][0].cid,
                        tid = arrOfclassAray[i][0].tid,
                        hastopayfor = arrOfclassAray[i][0].hastopayfor,
                    });
                    

                }


            }
        }




        public void presentclick(object sender, RoutedEventArgs e)
        {
            classes st = datagridXml.SelectedItem as classes;

            MessageBoxResult result = MessageBox.Show("නම ලකුනු කරන්න ද? ", "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                var todayDate = DateTime.Today;
                string strToday = todayDate.ToString("yyyyMMdd");

                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {


                        var output4 = cnn.Query<studAtendance>("select * from studAtendance where sid = '" + studentin.s_id + "'" + " and cid='" + st.cid +"'" + " and dateofatt= '" + strToday +  "'" , new DynamicParameters());
                        List<studAtendance> nebuals4 = new List<studAtendance>();
                        nebuals4 = output4.ToList();

                        if (nebuals4.Count > 0)
                        {
                            MessageBox.Show("අද දවස ට මේ ලමයගෙ නම ලකුනු කරල තියෙන්නෙ  ");
                        }
                        else
                        {
                            cnn.Execute("insert into studAtendance (sid , cid ,dateofatt ) values('" + studentin.s_id + "', '" + st.cid  + "' , '" + strToday + "' )" );
                             
                            MessageBox.Show("නම ලකුනු කලා ");
                        }



                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error " + ex);
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


        
        string hastopayfor;
        DateTime minimummm;
        DateTime max2;
        private void hastopayformonth()
        {

            string mindatetocal = "20220401";

            DateTime parsedDate = DateTime.ParseExact(mindatetocal, "yyyyMMdd", CultureInfo.InvariantCulture);

            for (int i = 0; i < arrOfclassAray.Count; i++)
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        var output4 = cnn.Query<studAtendance>("select * from studAtendance where sid = '" + studentin.s_id + "'" + " and cid='" + arrOfclassAray[i][0].cid + "'", new DynamicParameters());
                        List<studAtendance> nebuals4 = new List<studAtendance>();
                        nebuals4 = output4.ToList();

                        if (nebuals4.Count > 0) // student attend welada eka parak hari 
                        {

                            using (IDbConnection cnn2 = new SQLiteConnection(LoadConnectionString()))
                            {
                                try
                                {
                                    var output5 = cnn2.Query<studfeepaid>("select * from studfeepaid where sid = '" + studentin.s_id + "'" + " and cid='" + arrOfclassAray[i][0].cid + "'", new DynamicParameters());
                                    List<studfeepaid> nebuals5 = new List<studfeepaid>();
                                    nebuals5 = output5.ToList();

                                    if (nebuals5.Count > 0) // studen eka parak hari fee gewal hari free month ekak offer karal hari tyenwada 
                                    {
                                        max2 = DateTime.ParseExact(nebuals5[0].formonth, "yyyyMM", CultureInfo.InvariantCulture);
                                        for( int m = 0; m < nebuals5.Count; m++)
                                        {
                                            if (max2 < DateTime.ParseExact(nebuals5[m].formonth, "yyyyMM", CultureInfo.InvariantCulture) )
                                            {
                                                max2 = DateTime.ParseExact(nebuals5[m].formonth, "yyyyMM", CultureInfo.InvariantCulture);
                                            }
                                        }
                                        string monthoif = max2.ToString("MM");
                                        string yeartoif = max2.ToString("yyyy");
                                        DateTime max2date = DateTime.ParseExact(yeartoif + monthoif , "yyyyMM", CultureInfo.InvariantCulture);
                                        string yearnow = System.DateTime.Now.ToString("yyyy");
                                        string monthnow = System.DateTime.Now.ToString("MM");
                                        DateTime nowdate = DateTime.ParseExact(yearnow + monthnow, "yyyyMM", CultureInfo.InvariantCulture);

                                        if (max2date < nowdate ) //anthimata pay kala maasaya me maseta wada aduda balanawa
                                        {
                                            DateTime monthtopay   = DateTime.ParseExact(nebuals4[0].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture);
                                            List<DateTime> paykaladinatawadawadiatt = new List<DateTime>();
                                            for(int n =0; n < nebuals4.Count; n++)
                                            {
                                                 
                                                DateTime max2date2 = max2 ;
                                                string yearnow2 = DateTime.ParseExact(nebuals4[n].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy");
                                                string monthnow2 = DateTime.ParseExact(nebuals4[n].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM");
                                                if (max2date2 < DateTime.ParseExact(yearnow2 + monthnow2, "yyyyMM", CultureInfo.InvariantCulture) )
                                                {
                                                    paykaladinatawadawadiatt.Add(DateTime.ParseExact(nebuals4[n].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture));
                                                }
                                            }
                                            if( paykaladinatawadawadiatt.Count > 0)
                                            {
                                                DateTime minimumdate = paykaladinatawadawadiatt[0];
                                                for (int l = 0; l < paykaladinatawadawadiatt.Count; l++)
                                                {
                                                    if (minimumdate > paykaladinatawadawadiatt[l])
                                                    {
                                                        minimumdate = paykaladinatawadawadiatt[l];
                                                    }
                                                }
                                                arrOfclassAray[i][0].hastopayfor = minimumdate.ToString("yyyy/MM");
                                            }
                                            else
                                            {
                                                arrOfclassAray[i][0].hastopayfor = System.DateTime.Now.ToString("yyyy/MM");
                                            }
                                        }
                                        else // anthimata pay kala masaya me maseta wada wadida balanwa 
                                        {
                                            arrOfclassAray[i][0].hastopayfor = System.DateTime.Now.AddMonths(1).ToString("yyyy/MM");
                                        }
                                    }
                                    else //studen attend unata eka parkwath fee gewala naha - e nisa attend weccha mul dawasa penwamu
                                    {
                                        minimummm = DateTime.ParseExact(nebuals4[0].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture);
                                        for (int j = 0; j < nebuals4.Count; j++)
                                        {
                                            if (minimummm > DateTime.ParseExact(nebuals4[j].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture))
                                            {
                                                minimummm = DateTime.ParseExact(nebuals4[j].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture);
                                            }
                                        }
                                        arrOfclassAray[i][0].hastopayfor = minimummm.ToString("yyyy/MM");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("error " + ex);
                                }

                            }

                        }
                        else // student eka parak wath attend wela nadda
                        {
                            arrOfclassAray[i][0].hastopayfor =  "No attendance";
                        }



                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error " + ex);
                    }

                }
            }
        }

        public void payfee(object sender, RoutedEventArgs e)
        {
            classes st = datagridXml.SelectedItem as classes;
            MessageBoxResult result = MessageBox.Show("මේ සිසුවා අද මුදල් ගෙවන බව තහවුරු කරන්න ?" + st.cnameNtname  , "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                using (IDbConnection cnn5 = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        var output5 = cnn5.Query<studAtendance>("select * from studAtendance where sid = '" + studentin.s_id + "'" + " and cid='" + st.cid + "'", new DynamicParameters());
                        List<studAtendance> nebuals5 = new List<studAtendance>();
                        nebuals5 = output5.ToList();
                        if(nebuals5.Count > 0)
                        {
                            DateTime checkdateforpay = DateTime.ParseExact(st.hastopayfor, "yyyy/MM", CultureInfo.InvariantCulture);

                            if (DateTime.Today.AddMonths(1).Month == checkdateforpay.Month)
                            {
                                MessageBox.Show("ඊලග මාසය ට දැන්ම ගෙවන්න බැහැ . ඊලග මාසය එනකන් ඉන්න  ", "My App");
                            }
                            else
                            {
                                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                                {
                                    try
                                    {
                                        DateTime payformonth = DateTime.ParseExact(st.hastopayfor, "yyyy/MM", CultureInfo.InvariantCulture);
                                        string paidon = System.DateTime.Now.ToString("yyyyMMdd");

                                        var output4 = cnn.Query<feePrice>("select * from feePrice where classid = '" + st.cid + "'", new DynamicParameters());
                                        List<feePrice> nebuals4 = new List<feePrice>();
                                        nebuals4 = output4.ToList();
                                        int feeamoint = nebuals4[0].fee;
                                        string notchargedda = "charged";
                                        cnn.Execute("insert into studfeepaid (sid , cid ,formonth ,paidon ,amount , notchaged ) values('" + studentin.s_id + "', '" + st.cid + "' , '" + payformonth.ToString("yyyyMM") + "', '" + paidon + "', '" + feeamoint + "', '" + notchargedda + "' )");
                                        MessageBox.Show("paid succesfully");
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("error " + ex);
                                    }
                                }

                                //anthimata refrsh karanne 
                                refresh2();
                            }
                        }
                        else
                        {
                            MessageBox.Show(" ලමයා එක දවසක් වත් ඇවිල්ල නැති නිසා එයා ට ගෙවන්න බැහැ ");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error " + ex);
                    }
                }

                    

                 
            }
            else if (result == MessageBoxResult.No)
            {

            }
            else
            {

            }



        }
        public void notchargedclick(object sender, RoutedEventArgs e)
        {
            classes st = datagridXml.SelectedItem as classes;
            MessageBoxResult result = MessageBox.Show("ලමයා අදාල මාසයට ගෙවන්නෙ නැති බව තහවුරු කරන්න ? :  පන්තියේ නම " + st.cnameNtname, "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {

                using (IDbConnection cnn5 = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        var output6 = cnn5.Query<studAtendance>("select * from studAtendance where sid = '" + studentin.s_id + "'" + " and cid='" + st.cid + "'", new DynamicParameters());
                        List<studAtendance> nebuals6 = new List<studAtendance>();
                        nebuals6 = output6.ToList();
                        if (nebuals6.Count > 0)
                        {
                            DateTime checkdateforpay = DateTime.ParseExact(st.hastopayfor, "yyyy/MM", CultureInfo.InvariantCulture);

                            if (DateTime.Today.AddMonths(1).Month == checkdateforpay.Month)
                            {
                                MessageBox.Show("ඊලග මාසය ට දැන්ම ගෙවන්න බැහැ . ඊලග මාසය එනකන් ඉන්න ", " ");
                            }
                            else
                            {
                                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                                {
                                    try
                                    {
                                        DateTime payformonth = DateTime.ParseExact(st.hastopayfor, "yyyy/MM", CultureInfo.InvariantCulture);
                                        string paidon = System.DateTime.Now.ToString("yyyyMMdd");


                                        int feeamoint = 0;
                                        string notchargedda = "notcharged";
                                        cnn.Execute("insert into studfeepaid (sid , cid ,formonth ,paidon ,amount , notchaged ) values('" + studentin.s_id + "', '" + st.cid + "' , '" + payformonth.ToString("yyyyMM") + "', '" + paidon + "', '" + feeamoint + "', '" + notchargedda + "' )");
                                        MessageBox.Show("updated  succesfully");
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("error " + ex);
                                    }
                                }
                                //anthimata refresh karanne 
                                refresh2();
                            }

                        }
                        else
                        {
                            MessageBox.Show("සිසුවා එක දවසක් වත් ඇවිල්ල නැහැ ");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error " + ex);
                    }
                }

                


                
            }
            else if (result == MessageBoxResult.No)
            {

            }
            else
            {

            }

        }
        public void attendance(object sender, RoutedEventArgs e)
        {
            classes st = datagridXml.SelectedItem as classes;
            studentatt studentatt = new studentatt(st.cid , studentin.s_id , studentin.name , st.cnameNtname);
            studentatt.ShowDialog();
        }


        private void delclick(object sender, RoutedEventArgs e)
        {
            classes st = datagridXml.SelectedItem as classes;
            int cid = st.cid;


            MessageBoxResult result = MessageBox.Show("remove class " + st.cnameNtname +" from student "+ studentin.name, "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        cnn.Execute("DELETE FROM stuTeachClas  WHERE cid= '" + cid + "' and tid= '" + st.tid + "' and sid= '" + studentin.s_id + "'");

                        studentprofile newWindow = new studentprofile(studentin.s_id, studentin.name, studentin.phone, studentin.batch, studentin.locofFingerPrint);
                        Application.Current.MainWindow = newWindow;
                        newWindow.Show();
                        this.Close();
                        MessageBox.Show("class was removed from student !  ");
                    }
                    catch (Exception ex)
                    {
                    }
                }


            }
            else if (result == MessageBoxResult.No)
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



        public void dbfunction()
        {

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<stuTeachClas>("select * from stuTeachClas where sid = " + this.studentin.s_id, new DynamicParameters());
                    stuTeachClasAray = output.ToList();
                }
                catch (Exception ex)
                {
                }
            }


            for (int i = 0; i < stuTeachClasAray.Count; i++)
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    try
                    {
                        var output2 = cnn.Query<classes>("select * from classes where cid =' " + stuTeachClasAray[i].cid + "'", new DynamicParameters());
                        classAray = output2.ToList();
                        arrOfclassAray.Add(classAray);    
                       
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

             

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output3 = cnn.Query<studfeepaid>("select * from studfeepaid where sid =' " + this.studentin.s_id + "'", new DynamicParameters());
                    studfeepaidAray = output3.ToList();
                }
                catch (Exception ex)
                {
                }
            }


        }

        private void classtokid(object sender, RoutedEventArgs e)
        {
            classtokid classtokid  = new classtokid(studentin.s_id , studentin.name);
            classtokid.ShowDialog();
            refresh2();
        }

        private void refresh2()
        {
            studentprofile newWindow = new studentprofile(studentin.s_id, studentin.name, studentin.phone, studentin.batch, studentin.locofFingerPrint);
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }

        private void refreshbut(object sender, RoutedEventArgs e)
        {
            studentprofile newWindow = new studentprofile(studentin.s_id, studentin.name, studentin.phone, studentin.batch, studentin.locofFingerPrint);
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }
    }
}
