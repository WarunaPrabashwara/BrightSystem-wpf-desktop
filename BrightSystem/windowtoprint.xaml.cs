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
using System.Drawing;

namespace BrightSystem
{
    /// <summary>
    /// Interaction logic for windowtoprint.xaml
    /// </summary>
    public partial class windowtoprint : Window
    {
        public windowtoprint()
        {
            InitializeComponent();
        }
        public windowtoprint( int tid  , string tname ) : this()
        {
            RepGenerateFunc(tid, tname);
            namec.Content = tname;

            fillthegrid();
            printpage();
            filename.Content = tname + monthssss;
        }

        DateTime maxdatecurrent;
        reportmonths reportmonths = new reportmonths();
        string attendanceee = "";
        string monthssss;
        public void RepGenerateFunc(int tid, string tname)
        {
            string ifnopayments = "No any payments";

            reportmonths.percentage = 0.00;

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                // methanion percentage eka hoyala denawa 
                try
                {
                    var output8 = cnn.Query<teachers>("select * from teachers where tid= '" + tid + "'", new DynamicParameters());
                    List<teachers> nebuals77 = new List<teachers>();
                    nebuals77 = output8.ToList();
                    reportmonths.percentage = (100 - nebuals77[0].percentageOfHall) * 0.01;

                }
                catch (Exception ex)
                {
                    // MessageBox.Show("error" + ex);
                }

                // methana teacher ta karala tyena payment set eka aran eken payment eka karala yena uparima date eka[masayak widihaa ] gannawa . payment ekak wath karala natham 20000101 gannw 
                // eeka maxdatecurrentvariable eka ta danwa 
                try
                {
                    var output8 = cnn.Query<teacherpayment>("select * from teacherpayment where tid=" + tid + "", new DynamicParameters());
                    List<teacherpayment> nebuals6 = new List<teacherpayment>();
                    nebuals6 = output8.ToList();
                    if (nebuals6.Count > 0) //teacher payment aragena eka parak hari 
                    {
                        maxdatecurrent = DateTime.ParseExact(nebuals6[0].datepaid, "yyyyMMdd", CultureInfo.InvariantCulture);
                        for (int l = 0; l < nebuals6.Count; l++)
                        {
                            if (maxdatecurrent < DateTime.ParseExact(nebuals6[l].datepaid, "yyyyMMdd", CultureInfo.InvariantCulture))
                            {
                                maxdatecurrent = DateTime.ParseExact(nebuals6[l].datepaid, "yyyyMMdd", CultureInfo.InvariantCulture);
                            }

                        }
                        maxdatecurrent = DateTime.ParseExact(maxdatecurrent.ToString("yyyy") + maxdatecurrent.ToString("MM"), "yyyyMM", CultureInfo.InvariantCulture);

                    }
                    else
                    {
                        maxdatecurrent = DateTime.ParseExact("20000101", "yyyyMMdd", CultureInfo.InvariantCulture);
                    }

                }
                catch (Exception ex)
                {

                }

                try
                {
                    var output = cnn.Query<stuTeachClas>("select * from stuTeachClas where tid=" + tid + "", new DynamicParameters());
                    List<stuTeachClas> nebuals7 = new List<stuTeachClas>();
                    nebuals7 = output.ToList(); // teacher ta adaala class okkoama gannwa 
                    if (nebuals7.Count > 0) // teacher ta classs tyenwa nm 
                    {

                        List<int> classlistofteacher = new List<int>();
                        // dan class tika unique karagamu 

                        for (int k = 0; k < nebuals7.Count; k++)
                        {
                            if (classlistofteacher.Contains(nebuals7[k].cid))
                            {

                            }
                            else
                            {
                                classlistofteacher.Add(nebuals7[k].cid);
                            }

                        }
                        List<reportclasses> classes = new List<reportclasses>();
                        reportmonths.classes = classes;  // kklm  me word eka dala tyenen pahala class eke tyenwa podi desciption ekak . eeka point out karanna . mama dawasak ma hira wela hitiya logic ekak 
                        for (int k = 0; k < classlistofteacher.Count; k++) // hama class ekakatama iterae karanwa 
                        {

                            reportclasses reportclasses = new reportclasses();
                            reportclasses.classname = "test";

                            reportmonths.classes.Add(reportclasses);

                            try
                            {
                                var output11 = cnn.Query<classes>("select * from classes where cid=" + classlistofteacher[k] + "", new DynamicParameters());
                                List<classes> nebuals777 = new List<classes>();
                                nebuals777 = output11.ToList();
                                reportmonths.classes[k].classname = nebuals777[0].cnameNtname;
                            }
                            catch (Exception ex)
                            {

                            }


                            try
                            {
                                var output8 = cnn.Query<studfeepaid>("select * from studfeepaid where cid=" + classlistofteacher[k] + "", new DynamicParameters());
                                List<studfeepaid> nebuals8 = new List<studfeepaid>();
                                nebuals8 = output8.ToList();  // adaala class ekata [ loop eka athule di ] adaala payments tyenawada balanwa
                                if (nebuals8.Count > 0) //student ge payments tyewna nm .. [ me masetama wenna oni naha . kalin resolve wela hari kamak naha . eka parak hari payment karala
                                {
                                    List<reportstudents> paidstudents = new List<reportstudents>();
                                    reportmonths.classes[k].paidstudents = paidstudents; // kklm  me word eka dala tyenen pahala class eke tyenwa podi desciption ekak . eeka point out karanna . mama dawasak ma hira wela hitiya logic ekak 
                                    for (int l = 0; l < nebuals8.Count; l++)
                                    {
                                        DateTime formonthh = DateTime.ParseExact(nebuals8[l].paidon, "yyyyMMdd", CultureInfo.InvariantCulture);
                                        DateTime formoth2 = DateTime.ParseExact(formonthh.ToString("yyyy") + formonthh.ToString("MM"), "yyyyMM", CultureInfo.InvariantCulture);
                                        if (maxdatecurrent < formoth2) // teacher ta payment eka kala anthima masayata pasu thyyena siyalu student fee collections ekathu karala gannawa 
                                        {
                                            reportstudents reportstudents = new reportstudents();
                                            reportmonths.classes[k].paidstudents.Add(reportstudents);

                                            try
                                            {
                                                var output1111 = cnn.Query<student>("select * from student where s_id=" + nebuals8[l].sid + "", new DynamicParameters());
                                                List<student> nebuals77777 = new List<student>();
                                                nebuals77777 = output1111.ToList();
                                                reportmonths.classes[k].paidstudents[l].stuname = nebuals77777[0].name;

                                            }
                                            catch (Exception ex)
                                            {

                                            }


                                            reportmonths.classes[k].paidstudents[l].paidon = DateTime.ParseExact(nebuals8[l].paidon, "yyyyMMdd", CultureInfo.InvariantCulture);
                                            reportmonths.classes[k].paidstudents[l].paidfor = DateTime.ParseExact(nebuals8[l].formonth, "yyyyMM", CultureInfo.InvariantCulture);
                                            reportmonths.classes[k].paidstudents[l].amount = Int16.Parse(nebuals8[l].amount) * reportmonths.percentage;

                                            try
                                            {
                                                var output11111 = cnn.Query<studAtendance>("select * from studAtendance where sid='" + nebuals8[l].sid + "'", new DynamicParameters());
                                                List<studAtendance> nebuals77777 = new List<studAtendance>();
                                                nebuals77777 = output11111.ToList();
                                                for (int i = 0; i < nebuals77777.Count; i++)
                                                {
                                                    string monthofattend = DateTime.ParseExact(nebuals77777[i].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyyMM");

                                                    if (monthofattend == reportmonths.classes[k].paidstudents[l].paidfor.ToString("yyyyMM"))
                                                    {
                                                        string datee = DateTime.ParseExact(nebuals77777[i].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd");
                                                        // attendanceee = attendanceee + "  " + datee;
                                                        List<string> attendance = new List<string>();
                                                        reportmonths.classes[k].paidstudents[l].attendance = attendance;
                                                        if (reportmonths.classes[k].paidstudents[l].attendance.Contains(datee))
                                                        { }
                                                        else
                                                        {
                                                            reportmonths.classes[k].paidstudents[l].attendance.Add(datee);
                                                        }

                                                    }
                                                }


                                            }
                                            catch (Exception ex)
                                            {

                                            }


                                        }
                                        else
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
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                }

            }


        }

        public void fillthegrid()
        {
            phonec.Content = (reportmonths.percentage * 100).ToString()+"%";


            if (reportmonths.classes.Count > 0)
            {
                double totaltotal = 0.0;
                for (int j = 0; j < reportmonths.classes.Count; j++)
                {
                    datagridXml.Items.Add(new objectforgrid()
                    {   zero = "Class name : " ,
                        first = "",
                        second = reportmonths.classes[j].classname,
                        third = "",
                        fourth = "",
                        fifth = "",
                    });
                    datagridXml.Items.Add(new objectforgrid()
                    {   zero = "" ,
                        first = "Student name",
                        second = "Attendance",
                        third = "Paid on",
                        fourth = "Paid for",
                        fifth = "Amount without hall fee",
                    });;
 
                    if (reportmonths.classes[j].paidstudents.Count > 0)
                    {

                        double classtotal = 0.0;
                        for (int i = 0; i < reportmonths.classes[j].paidstudents.Count; i++)
                        {
                            datagridXml.Items.Add(new objectforgrid()
                            {
                                zero = "",
                                first = reportmonths.classes[j].paidstudents[i].stuname ,
                                second = string.Join("/", reportmonths.classes[j].paidstudents[i].attendance),
                                third = reportmonths.classes[j].paidstudents[i].paidon.ToString("yyyyMMMdd") ,
                                fourth = reportmonths.classes[j].paidstudents[i].paidfor.ToString("yyyyMMM"),
                                fifth = reportmonths.classes[j].paidstudents[i].amount.ToString(),
                            }); ;
                             
                            classtotal = classtotal + reportmonths.classes[j].paidstudents[i].amount;

                        }
                        datagridXml.Items.Add(new objectforgrid()
                        {
                            zero = "" ,
                            first = "",
                            second = "",
                            third = "",
                            fourth = "Class's Total ",
                            fifth = classtotal.ToString(),
                        });
                        totaltotal = totaltotal + classtotal;
                    }
                    else
                    {
                        datagridXml.Items.Add(new objectforgrid()
                        {
                            zero = "" ,
                            first = "neither atendance nor payments",
                            second = "",
                            third = "",
                            fourth = "",
                            fifth = "",
                        });
                    }

                }
                datagridXml.Items.Add(new objectforgrid()
                {
                    zero = "" ,
                    first = "",
                    second = "",
                    third = "",
                    fourth = "Total of all classes  Rs.",
                    fifth = totaltotal.ToString(),
                });
            }
            List<string> monthlist = new List<String>();
            if (reportmonths.classes.Count > 0)
            {
                for (int j = 0; j < reportmonths.classes.Count; j++)
                {
                    if (reportmonths.classes[j].paidstudents.Count > 0)
                    {
                        for (int i = 0; i < reportmonths.classes[j].paidstudents.Count; i++)
                        {
                            if (monthlist.Contains(reportmonths.classes[j].paidstudents[i].paidon.ToString("yyyyMMM")))
                            {
                            }
                            else
                            {
                                monthlist.Add(reportmonths.classes[j].paidstudents[i].paidon.ToString("yyyyMMM"));
                            }
                        }
                    }
                }
            }
            
            if (monthlist.Count > 0)
            {
                monthssss = String.Join(",", monthlist);
            }
            else
            {
                monthssss = "";
            }
        }

 
        public void printpage()
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(grid, "My First Print Job");
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }


    internal class objectforgrid
    {   public string zero { get; set; }
        public string first { get; set; }
        public string second { get; set; }
        public string third { get; set; }
        public string fourth { get; set; }
        public string fifth { get; set; }

    }
}
