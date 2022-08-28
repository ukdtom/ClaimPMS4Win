using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.NetworkInformation;



namespace ClaimPMS4Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static readonly HttpClient client = new HttpClient();

        private string FriendlyName;
        private string PlexOnlineMail;
        private string PlexOnlineToken;
        private string PlexOnlineUsername;
        private string PlexOnlineHome;

        private void addLog( string text)
        {
            this.rtbOut.AppendText(text + "\r\n");
        }

        private Boolean pmsPresent()
        {
            RegistryKey pmsKeys = Registry.CurrentUser.OpenSubKey(@"Software\Plex, Inc.\Plex Media Server", true);
            if (pmsKeys != null)
            {
                FriendlyName = (String)pmsKeys.GetValue("FriendlyName");
                PlexOnlineMail = (String)pmsKeys.GetValue("PlexOnlineMail");
                PlexOnlineToken = (String)pmsKeys.GetValue("PlexOnlineToken");
                PlexOnlineUsername = (String)pmsKeys.GetValue("PlexOnlineUsername");
                this.tbUsrName.Text = PlexOnlineUsername;
                PlexOnlineHome = pmsKeys.GetValue("PlexOnlineHome").ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> loginPlexTVAsync()
        {
            string url;
            if (this.tb2FA.Text != "")
            {
                url = String.Format("https://plex.tv/api/v2/users/signin?login={0}&password={1}&verificationCode={2}", Uri.EscapeDataString(this.tbUsrName.Text), Uri.EscapeDataString(this.tbPwd.Text), Uri.EscapeDataString(this.tb2FA.Text));
            } else
            {
                url = String.Format("https://plex.tv/api/v2/users/signin?login={0}&password={1}", Uri.EscapeDataString(this.tbUsrName.Text), Uri.EscapeDataString(this.tbPwd.Text));
            }

            var response = await client.PostAsync(url, null);
            var statusCode = response.StatusCode;
            if ((int)statusCode == 1029)
            {
                this.addLog("2FA token needed");


            }
            var responseMsg = await response.Content.ReadAsStringAsync();
            if (statusCode == System.Net.HttpStatusCode.OK)
            {
                this.addLog("Authenticated towards plex.tv okay");
            } else
            {
                this.addLog("We encountered an error!");


            }





            //      axios({
            //      method: 'POST',
            //url: url,
            //headers: wtutils.PMSHeader
            //          })



            return false;
        }

        private void setDefaultHeaders()
        {
            // Add some default headers for Web Requests
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("X-Plex-Device", "Windows");
            String firstMacAddress = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();
            client.DefaultRequestHeaders.Add("X-Plex-Client-Identifier", firstMacAddress);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string version = assembly.GetName().Version.ToString();
            // Also set as default header towards plex.tv
            client.DefaultRequestHeaders.Add("X-Plex-Version", version);
            string name = assembly.GetName().Name;
            // Also set as default header towards plex.tv
            client.DefaultRequestHeaders.Add("X-Plex-Product", name);
            this.setDefaultHeaders();
            this.Text = name + " v" + version;
            this.addLog("Velcome to " + name);
            this.addLog("");
            this.addLog("Note: When we say PMS, we mean the Plex Media Server");
            this.addLog("");
            this.addLog("This 3rd. Party tool will assist you in (Re-) Claiming a PMS");
            this.addLog("");
            this.addLog("This tool MUST be run on the PMS itself, and as the user that normally runs the PMS");
            this.addLog("");
            this.addLog("");
            if (pmsPresent())
            {
                this.addLog("We found an installed PMS, so good so far");
                this.addLog("");
                this.addLog("PMS Name detected as: " + FriendlyName);
                if (this.PlexOnlineUsername != null)
                {
                    this.btnClaim.Text = "Re-claim";
                    this.addLog("PMS detected as been claimed");
                } else
                {
                    this.addLog("PMS detected as not claimed");
                }
                this.addLog("");
                this.addLog("Enter credentials to the right, and press the button");
            } else
            {
                this.addLog("No PMS server found");
                this.addLog("You have to run this on the actual PMS server!");
                this.addLog("and with the correct user!");
                this.btnClaim.Enabled = false;
            }
        }

        private void btnClaim_Click(object sender, EventArgs e)
        {
            _ = this.loginPlexTVAsync();
        }
    }
}
