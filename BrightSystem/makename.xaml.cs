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
    /// Interaction logic for makename.xaml
    /// </summary>
    public partial class makename : Window
    {
         
        List<studentmodal> nebuals = new List<studentmodal>();


        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public makename()
        {

            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<studentmodal>("select * from student", new DynamicParameters());
                    nebuals = output.ToList();
                }
                catch (Exception ex)
                {
                }
            }


            for (int i = 0; i < nebuals.Count; i++)
            {
                datagridXml.Items.Add(new studentmodal()
                {
                    name = nebuals[i].name,
                    phone = nebuals[i].phone,
                    s_id = nebuals[i].s_id,
                    batch = nebuals[i].batch,
                });


            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            var tbx = sender as TextBox;
            if (tbx.Text != "")
            {


                var filtertext = nebuals.Where(x => x.name.ToLower().Contains(tbx.Text.ToLower())).ToList();

                var filtertext2 = nebuals.Where(x => x.s_id.ToString().Contains(tbx.Text)).ToList();
                filtertext.AddRange(filtertext2);
                filtertext = filtertext.Distinct().ToList();

                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();


                for (int i = 0; i < filtertext.Count; i++)
                {
                    datagridXml.Items.Add(new studentmodal()
                    {
                        name = filtertext[i].name,
                        phone = filtertext[i].phone,
                        s_id = filtertext[i].s_id,
                        batch = filtertext[i].batch,
                    });

                }




            }
            else
            {
                datagridXml.Items.Clear();
                datagridXml.Items.Refresh();

                for (int i = 0; i < nebuals.Count; i++)
                {
                    datagridXml.Items.Add(new studentmodal()
                    {
                        name = nebuals[i].name,
                        phone = nebuals[i].phone,
                        s_id = nebuals[i].s_id,
                        batch = nebuals[i].batch,
                    });

                }


            }
        }



 





        private void viewclick(object sender, RoutedEventArgs e)
        {
            studentmodal st = datagridXml.SelectedItem as studentmodal;
            studentprofile studentprofile = new studentprofile(st.s_id, st.name, st.phone, st.batch, st.locofFingerPrint);
            studentprofile.ShowDialog();
        }

    }
}
