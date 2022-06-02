using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Dapper;
using System.Data.SQLite;
using System.Globalization;

using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using Syncfusion.Pdf.Grid;
using Size = System.Drawing.Size;

namespace BrightSystem
{
    internal class genReport
    {
        DateTime maxdatecurrent;
        reportmonths reportmonths = new reportmonths();
        
        string attendanceee = "";
        public void RepGenerateFunc(int tid, string tname)
        {
            string ifnopayments = "No any payments";
            List<reportclasses> classes = new List<reportclasses>();
            reportmonths.classes = classes;  // kklm  me word eka dala tyenen pahala class eke tyenwa podi desciption ekak . eeka point out karanna . mama dawasak ma hira wela hitiya logic ekak 

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

                            // methanin pament kala tudents la ekathu karanwa 
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

            nonattendendstif();
        }

        public void nonattendendstif() // mehemai mulin puka iragena kala eka waradi . eeka thma uba mulin paid student arn dewaniyata eeken attendance gatthu eka . karanna oni mulin attendance aran dewaniyata paid un ganna eka . mokada payment nokala un enne naha palaweni widihata logic eka giyoth . ee wagema thawa case ekak . list[ list.Count -1 ] kiyala liyanna epa . e wenuwata i kiyala variable ekak aragena eeken sellama karapan mokada report generation ekata kela une oi widihata liyala . dawas gaanak solution eka hoyanna bari una 
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                ///////////////////
                ///mehemai ekama code eken atended studens la saha not attended students la hoyanna giyaama case . code eke logic eka awul . e nisa mama apahu ara object eke iterate wenna baluwe 
                ///

                if (reportmonths.classes != null)
                {

                    for (int i = 0; i < reportmonths.classes.Count; i++)
                    {
                        List<reportstudents> notpaidstudents = new List<reportstudents>();
                        reportmonths.classes[i].notpaidstudents = notpaidstudents;
                        int cid = 0;
                        int lss = 0;
                        // cid eka ganna try eka meka
                        try
                        {
                            var output8 = cnn.Query<classes>("select * from classes where cnameNtname='" + reportmonths.classes[i].classname + "'", new DynamicParameters());
                            List<classes> nebuals6 = new List<classes>();
                            nebuals6 = output8.ToList();
                            if (nebuals6.Count > 0)
                            {
                                cid = nebuals6[0].cid;
                            }
                            else
                            {

                            }

                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {
                            var output11111 = cnn.Query<studAtendance>("select * from studAtendance where cid ='" + cid + "'", new DynamicParameters());
                            List<studAtendance> nebuals77777 = new List<studAtendance>();
                            nebuals77777 = output11111.ToList();
                            if (nebuals77777.Count > 0)
                            {
                                List<int> sidlist = new List<int>();
                                for (int l = 0; l < nebuals77777.Count; l++)
                                {
                                    if (sidlist.Contains(nebuals77777[l].sid))
                                    {

                                    }
                                    else
                                    {
                                        sidlist.Add(nebuals77777[l].sid);
                                    }
                                }
                                for(int l = 0; l < sidlist.Count; l++)
                                {
                                    List< DateTime > studenttattendance = new List<DateTime>();
                                    for(int kk = 0; kk < nebuals77777.Count; kk++)
                                    {
                                        if (sidlist[l] == nebuals77777[kk].sid) // meeka athulata enne adaala studeent ge attendance witharai
                                        {
                                            studenttattendance.Add(DateTime.ParseExact(nebuals77777[kk].dateofatt, "yyyyMMdd", CultureInfo.InvariantCulture));

                                        }
                                    }
                                    DateTime maxpaidformonth = DateTime.ParseExact("200001", "yyyyMM", CultureInfo.InvariantCulture);
                                    try
                                    {
                                        var output8 = cnn.Query<studfeepaid>("select * from studfeepaid where cid='" + cid + "' and sid='" + sidlist[l] +"'", new DynamicParameters());
                                        List<studfeepaid> nebuals8 = new List<studfeepaid>();
                                        nebuals8 = output8.ToList();
                                        if (nebuals8.Count > 0)
                                        {
                                            for(int kk = 0; kk < nebuals8.Count; kk++)
                                            {
                                                if (maxpaidformonth < DateTime.ParseExact(nebuals8[kk].formonth, "yyyyMM", CultureInfo.InvariantCulture))
                                                {
                                                    maxpaidformonth = DateTime.ParseExact(nebuals8[kk].formonth, "yyyyMM", CultureInfo.InvariantCulture);
                                                }
                                            }
                                            
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    List<String> notpaidatt = new List<string>();    
                                    for(int mm=0; mm< studenttattendance.Count; mm++)
                                    {
                                        if(maxpaidformonth < DateTime.ParseExact(studenttattendance[mm].ToString("yyyy") + studenttattendance[mm].ToString("MM"), "yyyyMM", CultureInfo.InvariantCulture) )
                                        {
                                            notpaidatt.Add(studenttattendance[mm].ToString("MMMdd"));
                                        }
                                    }
                                     ;
                                    string studname = "";
                                    try
                                    {
                                        var output9 = cnn.Query<student>("select * from student where s_id='" + sidlist[l] + "'", new DynamicParameters());
                                        List<student> nebuals6 = new List<student>();
                                        nebuals6 = output9.ToList();
                                        if (nebuals6.Count > 0)
                                        {
                                            studname = nebuals6[0].name;

                                        }
                                        else
                                        {

                                        }

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    if (notpaidatt.Count > 0)
                                    {
                                        
                                        reportstudents reportstudents = new reportstudents();
                                        reportmonths.classes[i].notpaidstudents.Add(reportstudents);
                                        reportmonths.classes[i].notpaidstudents[lss].stuname = studname;
                                        reportmonths.classes[i].notpaidstudents[lss].attendance = notpaidatt;
                                         
                                        lss = lss + 1;
                                    }
                                    


                                }

                            }
                            else
                            {

                            }
                        }
                        catch
                        {
                        }

                      

                    }
                }


                ///////////////
                ///


            }
        }

        public void createpdf(string teachername)
        {
            using (PdfDocument document = new PdfDocument())
            {
                document.PageSettings.Size = new Size(600, 50000  );
                //Add a page to the document

                //Create PDF graphics for a page

                int pagecount = 0 ; 
                List<PdfPage> pagess = new List<PdfPage>();
                PdfPage page2 = document.Pages.Add();
                pagess.Add(page2);
                List<PdfGraphics> graphics = new List<PdfGraphics>();
                graphics.Add(pagess[pagecount].Graphics);
                 
                //Set the standard font
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
                PdfFont font3 = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                PdfFont font2 = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
                int wid = 0; 
                //wid  yanna content okkogema total height eka wge
                double apagessize = pagess[pagecount].Size.Height -150 ;
                int pageekaathuleusa = 0;
                graphics[pagecount].DrawString("Bright Education Center  ", font, PdfBrushes.Black, new PointF(200, wid));
                
                wid = wid + 15;
                pageekaathuleusa = pageekaathuleusa + 15;
                graphics[pagecount].DrawString("First table has students who paid or obtained free cards [ marked as Rs.0 paid ]  , Second table has students who didn't pay for attended dates " + Environment.NewLine + " If a student didn't attend for a full month , those students are not included to tables . We don't charge for those months . "  , font2, PdfBrushes.Black, new PointF(0, pageekaathuleusa));
                graphics[pagecount].DrawString(Environment.NewLine, font, PdfBrushes.Black, new PointF(0, 0));
                //Draw the text
                wid = wid + 30;
                pageekaathuleusa = pageekaathuleusa + 30;
                graphics[pagecount].DrawString(  Environment.NewLine + "Teacher's name: " + teachername + "   Teacher's percentage : " + (reportmonths.percentage * 100).ToString() + "% ", font2, PdfBrushes.Black, new PointF(300, pageekaathuleusa));
                wid = wid +60;
                pageekaathuleusa = pageekaathuleusa + 60;
                if (reportmonths.classes.Count > 0)
                {
                    if (pageekaathuleusa > apagessize )
                    {
                      //  pageekaathuleusa = pageekaathuleusa% (int)Math.Round(apagessize);
 
                   //     pagess.Add(document.Pages.Add());
                   //     pagecount = pagecount + 1;
                     //   graphics.Add(pagess[pagecount].Graphics);
                    }
                    double totaltotal = 0.0;
                    for (int j = 0; j < reportmonths.classes.Count; j++)
                    {
                        wid = wid + 10;
                        pageekaathuleusa = pageekaathuleusa + 10;
                        graphics[pagecount].DrawString("Class name : " + reportmonths.classes[j].classname, font3, PdfBrushes.Black, new PointF(0, pageekaathuleusa ));
                        graphics[pagecount].DrawString(Environment.NewLine, font, PdfBrushes.Black, new PointF(0, 0));
                        wid = wid + 15;
                        pageekaathuleusa = pageekaathuleusa + 15;
                        PdfGrid pdfGrid = new PdfGrid();
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Student name");
                        dataTable.Columns.Add("Attendance of paid for month");
                        dataTable.Columns.Add("Date paid");
                        dataTable.Columns.Add("Paid for");
                        dataTable.Columns.Add("Teacher amount");

                        PdfGrid pdfGrid2 = new PdfGrid();
                        DataTable dataTable2 = new DataTable();
                        dataTable2.Columns.Add("Student name");
                        dataTable2.Columns.Add("Attendance without payment");


                        if (reportmonths.classes[j].paidstudents != null )
                        {
                            if (reportmonths.classes[j].paidstudents.Count > 0)
                            {
                                double classtotal = 0.0;
                                for (int i = 0; i < reportmonths.classes[j].paidstudents.Count; i++)
                                {
                                    if (reportmonths.classes[j].paidstudents[i].attendance != null)
                                    {
                                        dataTable.Rows.Add(new object[] { reportmonths.classes[j].paidstudents[i].stuname, string.Join("/", reportmonths.classes[j].paidstudents[i].attendance), reportmonths.classes[j].paidstudents[i].paidon.ToString("yyyyMMMdd"), reportmonths.classes[j].paidstudents[i].paidfor.ToString("yyyyMMM"), reportmonths.classes[j].paidstudents[i].amount.ToString() });
                                        classtotal = classtotal + reportmonths.classes[j].paidstudents[i].amount;
                                    }    
                                }
                                if (pageekaathuleusa > apagessize)
                                {
                                //    pageekaathuleusa = pageekaathuleusa % (int)Math.Round(apagessize);

                                 //   pagess.Add(document.Pages.Add());
                                 //   pagecount = pagecount + 1;
                                 //   graphics.Add(pagess[pagecount].Graphics);
                                }
                                dataTable.Rows.Add(" ", " ", " ", "Total of class : Rs.", classtotal);
                                totaltotal = totaltotal + classtotal;
                                pdfGrid.DataSource = dataTable;
                                graphics[pagecount].DrawString("Students who paid or got free cards [ paid Rs.0 ] ", font2, PdfBrushes.Black, new PointF(10, pageekaathuleusa));

                                wid = wid + 1;
                                pageekaathuleusa = pageekaathuleusa + 1;
                                pdfGrid.Style.AllowHorizontalOverflow = true;
                                PdfLayoutResult result = pdfGrid.Draw(pagess[pagecount], new PointF(10, pageekaathuleusa + 15));
                                float height = result.Bounds.Height;
                                int floattoint = (int)height;
                                wid = wid + floattoint + 20;
                                pageekaathuleusa = pageekaathuleusa + floattoint + 20;
                                
                            }

                        }



                        if (reportmonths.classes[j].notpaidstudents != null )
                        {
                            if (reportmonths.classes[j].notpaidstudents.Count > 0)
                            {
                                for (int i = 0; i < reportmonths.classes[j].notpaidstudents.Count; i++)
                                {
                                    if (reportmonths.classes[j].notpaidstudents[i].attendance != null)
                                    {
                                        dataTable2.Rows.Add(new object[] { reportmonths.classes[j].notpaidstudents[i].stuname, string.Join("/", reportmonths.classes[j].notpaidstudents[i].attendance) });
                                    }    
                                }
                                if (pageekaathuleusa > apagessize)
                                {
                                //    pageekaathuleusa = pageekaathuleusa % (int)Math.Round(apagessize);

                                //    pagess.Add(document.Pages.Add());
                                //    pagecount = pagecount + 1;
                                //    graphics.Add(pagess[pagecount].Graphics);
                                }
                                if(  reportmonths.classes[j].notpaidstudents.Count != 0 ){
                                    pdfGrid2.DataSource = dataTable2;
                                    graphics[pagecount].DrawString("Students who didn't pay for attendance", font2, PdfBrushes.Black, new PointF(10, pageekaathuleusa));
                                    wid = wid + 1;
                                    pageekaathuleusa = pageekaathuleusa + 1;
                                    pdfGrid2.Style.AllowHorizontalOverflow = true;
                                    PdfLayoutResult result = pdfGrid2.Draw(pagess[pagecount], new PointF(10, pageekaathuleusa + 15));
                                    float height = result.Bounds.Height;
                                    int floattoint = (int)height;
                                    wid = wid + floattoint + 20;
                                    pageekaathuleusa = pageekaathuleusa + floattoint + 20;
                                }
                                
                                
                            }
                        }

                        

                         

                    }
                    wid = wid + 30;
                    pageekaathuleusa = pageekaathuleusa + 30;
                    graphics[0].DrawString("You have payment of Total : Rs." + totaltotal.ToString() + "    - Description below ", font, PdfBrushes.Black, new PointF(100, 75) );
                    wid = wid + 100;
                    pageekaathuleusa = pageekaathuleusa + 100;
                    graphics[pagecount].DrawString(Environment.NewLine, font, PdfBrushes.Black, new PointF(0, 0));
                }



                List<string> monthlist = new List<String>();
                if (reportmonths.classes.Count > 0)
                {
                    for (int j = 0; j < reportmonths.classes.Count; j++)
                    {
                        if (reportmonths.classes[j].paidstudents!= null && reportmonths.classes[j].paidstudents.Count > 0)
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
                string monthssss;
                if (monthlist.Count > 0)
                {
                    monthssss = String.Join(",", monthlist);
                }
                else
                {
                    monthssss = "";
                }


                PdfFont font7 = new PdfStandardFont(PdfFontFamily.Helvetica, 14);
                PdfFont font8 = new PdfStandardFont(PdfFontFamily.ZapfDingbats , 14);
                graphics[pagecount].DrawString("A Grade software solutions   +94713705748", font7 , PdfBrushes.Blue, new PointF(220, pageekaathuleusa));

                 
                graphics[pagecount].DrawString("------------------------------------------------------------------------------------------------------------------------------------------------END------------------------------------------------------------------------------------------------------------------------------------------------", font2, PdfBrushes.Black, new PointF(0, pageekaathuleusa +50));


                //Save the document
                document.Save("reports/" + teachername + " " + monthssss + ".pdf");
            }

        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }



    }

    internal class reportstudents
    {
        public string stuname { get; set; }
        public DateTime paidon { get; set; }
        public DateTime paidfor { get; set; }
        public double amount { get; set; }

        public List<string> attendance { get; set; }

    }
    internal class reportclasses
    {
        public List<reportstudents> paidstudents { get; set; } // mehema kalata ita passe wenama eliye list ekak hadala eeka mmekata equal kale nattham exception ekak enwa . mama dawas gaanak eeke hira wela hitiya .  "kklm" kiyala comment ekak dala athi . search karalap ee key word eka . ethakota ubata terei mokadda seen eka kiyala 
        public List<reportstudents> notpaidstudents { get; set; }
        public string classname { get; set; }

    }
    internal class reportmonths
    {
        public List<reportclasses> classes { get; set; }

        public string teachername { get; set; }
        public double percentage { get; set; }

    }
}
