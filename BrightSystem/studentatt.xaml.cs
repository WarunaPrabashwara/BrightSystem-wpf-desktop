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
    /// Interaction logic for studentatt.xaml
    /// </summary>
    public partial class studentatt : Window
    {
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
        public studentatt()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
        }

        public studentatt(int cid,  int sid  , string studentname , string classname  ) : this()
        {
            namec.Content = studentname;
            phonec.Content = classname;

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output4 = cnn.Query<studAtendance>("select * from studAtendance where sid = '" + sid + "'" + " and cid='" + cid + "'"  , new DynamicParameters());
                    List<studAtendance> nebuals4 = new List<studAtendance>();
                    nebuals4 = output4.ToList();
                    if (nebuals4.Count > 0)
                    {
                        for (int i = 0; i < nebuals4.Count; i++)
                        {
                            string formonthhh = DateTime.ParseExact(nebuals4[nebuals4.Count -1 -i].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy-MMM-dd");
                            attendancelistbox.Items.Add(formonthhh );
                        }
                    }
                    else
                    {
                        attendancelistbox.Items.Add("No attendance ");
                    }

                    var output5 = cnn.Query<studfeepaid>("select * from studfeepaid where sid = '" + sid + "'" + " and cid='" + cid + "'", new DynamicParameters());
                    List<studfeepaid> nebuals5 = new List<studfeepaid>();
                    nebuals5 = output5.ToList();
                    if (nebuals5.Count > 0)
                    {
                        for (int i = 0; i < nebuals5.Count; i++)
                        {
                            string formonthh = DateTime.ParseExact( nebuals5[nebuals5.Count -1 -i].formonth , "yyyyMM", CultureInfo.InvariantCulture).ToString("yyyy-MMM");
                            string paidonn = DateTime.ParseExact(nebuals5[nebuals5.Count - 1 - i].paidon, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy-MMM-dd");
                            feepaidlistbox.Items.Add("For "+  formonthh + " paid on "+ paidonn +" Rs."+ nebuals5[i].amount +" ("+ nebuals5[i].notchaged +")" );
                        }
                    }
                    else
                    {
                        feepaidlistbox.Items.Add("No payments yet ");
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show("error " + ex);
                }
            }

            
             
        }

    }
}
