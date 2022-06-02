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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Configuration;
using System.Data.SQLite;
using System.Data;
using Dapper;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

namespace BrightSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            //ttt.Text = GenerateHash("E://@camzer");
            forwardfucn();
            // firs time  db eke log ekata hash karala dala tyenne integer 0 kiyana eka hash karala eka string ekak widihata 
            // eka parak log unama db eke log ekata hash karala dala tyenne integer 1 kiyana eka hash karala eka string ekak widihata 
            //   1 =  6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b
            //   0 =  5feceb66ffc86f38d952786c6d696c79c2dbc239dd4e91b46729d73a27fb57e9
        }
        string macAddr;
        private void forwardfucn()
        {
            macAddr =( from nic in NetworkInterface.GetAllNetworkInterfaces()  where nic.OperationalStatus == OperationalStatus.Up  select nic.GetPhysicalAddress().ToString()    ).FirstOrDefault();

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<manipulate>("select * from manipulate ", new DynamicParameters());
                    List<manipulate> teachers = new List<manipulate>();
                    teachers = output.ToList();
                    if (teachers.Count >0 )
                    {
                        // eka parak log unama recode eka wadinawa . nattham recode eka wadinne naha . so mekata enne eka parak log unanam 
                        if (teachers[0].log == "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b") // eka parak log wela nm 
                        {

                            if(AreEqual(macAddr , teachers[0].id)  )  // hash kala mac address eka db eke tyena mac ekata samananm 
                            {
                                forward();

                            }
                            else
                            {
                                MessageBox.Show("අන්තර්ජල පහසුකම් අවශය යි  ");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
        private void forwad_Click(object sender, RoutedEventArgs e)
        {
            macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                try
                {
                    var output = cnn.Query<manipulate>("select * from manipulate ", new DynamicParameters());
                    List<manipulate> teachers = new List<manipulate>();
                    teachers = output.ToList();
                    if (teachers.Count > 0)
                    {                         
                        if (AreEqual(paswdbox.Password.ToString(), teachers[0].hash)  ) // gahapu password eka harinam 
                        {
                            string hashmac = GenerateHash(macAddr) ;
                            string hashedOne = "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b";
                            string hashedZero = "5feceb66ffc86f38d952786c6d696c79c2dbc239dd4e91b46729d73a27fb57e9";
                            try
                            {
                                cnn.Execute("UPDATE manipulate SET id = '" + hashmac + "', log = '" + hashedOne + "' WHERE log = '" + hashedZero + "' ");

                            }
                            catch (Exception ex)
                            {

                            }
                            forward();
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

        }
        private void forward()
        {
            this.forwad.Visibility = Visibility.Hidden;
            paswdbox.Visibility = Visibility.Hidden;
            Main.Content = new secondpage();
        }
        public string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public string GenerateHash(string input, string salt = "aa")
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool AreEqual(string plainTextInput, string hashedInput, string salt="aa")
        {
            string newHashedPin = GenerateHash(plainTextInput );
            return newHashedPin.Equals(hashedInput);
        }
    }
}
