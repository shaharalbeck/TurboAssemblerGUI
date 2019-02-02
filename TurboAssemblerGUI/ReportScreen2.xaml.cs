using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace TurboAssemblerGUI
{
    /// <summary>
    /// Interaction logic for ReportScreen2.xaml
    /// </summary>
    public partial class ReportScreen2 : Window
    {
        public ReportScreen2()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TextBox NameB = NameBox;
            TextBox DescriptionB = DescBox1;
            Button btn = SendButton;
            string name = NameB.Text;
            string description = DescriptionB.Text;
            SendMail(name, description, btn);
        }

        private static void SendMail(string name, string description, Button btn)
        {
  
            var fromAddress = new MailAddress("from@gmail.com", "ta-gui Reporter");
            var toAddress = new MailAddress("to@Gmail.Com", "To Name");
            const string fromPassword = "PW";
            const string subject = "ta-gui Bug Report";
            //  const string body = "LOG";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = "A Bug Report From "+name+": \n\r"+description
            })
            {
                try
                {
                    smtp.Send(message);
                    System.Windows.Forms.MessageBox.Show("Report Sent! Thank You!");
                    btn.Content = "Thank You!";
                    btn.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Exception Occurred ->"+ex.Message);
                }
            }

        }
    }
}
