using Microsoft.Win32;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;
using System.Management;

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
        private string authToken;
        private string claimToken;
        private string exchangeToken;

        private void addLog( string text)
        {
            this.rtbOut.AppendText(text + "\r\n");
        }

        private Boolean pmsPresent()
        {
            RegistryKey pmsKeys = Registry.CurrentUser.OpenSubKey(@"Software\Plex, Inc.\Plex Media Server", false);
            if (pmsKeys != null)
            {
                FriendlyName = (String)pmsKeys.GetValue("FriendlyName");
                PlexOnlineMail = (String)pmsKeys.GetValue("PlexOnlineMail");
                PlexOnlineToken = (String)pmsKeys.GetValue("PlexOnlineToken");
                PlexOnlineUsername = (String)pmsKeys.GetValue("PlexOnlineUsername");
                //this.tbUsrName.Text = PlexOnlineUsername;
                PlexOnlineHome = pmsKeys.GetValue("PlexOnlineHome", 0).ToString();
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
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Created:
                    // We authenticated ok
                    var responseMsg = await response.Content.ReadAsStringAsync();
                    var jo = JObject.Parse(responseMsg);

                    if (this.PlexOnlineUsername != jo["username"].ToString())
                    {
                        DialogResult dialogResult = MessageBox.Show("The entered credentials are different from what PMS used to be claimed as. Do you want to continue?", "Please confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.No)
                        {
                            return false;
                        }
                    }
                    authToken = jo["authToken"].ToString();
                    PlexOnlineMail = jo["email"].ToString();
                    this.addLog("Logged in okay");
                    return true;
                default:
                    // code block
                    this.addLog("We encountered an error!");
                    this.addLog("Status code was:" + statusCode.ToString());
                    this.addLog("Please try again");
                    return false;
            }
        }

        private async Task<bool> getClaimToken()
        {
            // Get Claim Token from plex.tv
            string url = String.Format("https://plex.tv/api/v2/claim/token?X-Plex-Token={0}", Uri.EscapeDataString(this.authToken));
            var response = await client.GetAsync(url);
            var statusCode = response.StatusCode;
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var responseMsg = await response.Content.ReadAsStringAsync();
                    var jo = JObject.Parse(responseMsg);
                    claimToken = jo["token"].ToString();
                    return true;
                default:
                    // code block
                    this.addLog("We encountered an error!");
                    this.addLog("Status code was:" + statusCode.ToString());
                    this.addLog("Please try again");
                    return false;
            }
        }

        private async Task<bool> getExchangeToken()
        {
            // Get Claim Token from plex.tv
            string url = String.Format("https://plex.tv/api/v2/claim/exchange?token={0}", Uri.EscapeDataString(this.claimToken));
            var response = await client.PostAsync(url, null);
            var statusCode = response.StatusCode;
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var responseMsg = await response.Content.ReadAsStringAsync();
                    var jo = JObject.Parse(responseMsg);
                    exchangeToken = jo["token"].ToString();
                    return true;
                default:
                    // code block
                    this.addLog("We encountered an error!");
                    this.addLog("Status code was:" + statusCode.ToString());
                    this.addLog("Please try again");
                    return false;
            }
        }

        private Task<bool> updateRegistry()
        {
            // We now need to update the registry
            RegistryKey pmsKeys = Registry.CurrentUser.OpenSubKey(@"Software\Plex, Inc.\Plex Media Server", true);
            if (pmsKeys != null)
            {
                pmsKeys.SetValue("PlexOnlineHome", 1, RegistryValueKind.DWord);
                pmsKeys.SetValue("PlexOnlineMail", this.PlexOnlineMail, RegistryValueKind.String);
                pmsKeys.SetValue("PlexOnlineToken", this.exchangeToken, RegistryValueKind.String);
                pmsKeys.SetValue("PlexOnlineUsername", this.PlexOnlineUsername, RegistryValueKind.String);
                pmsKeys.Close();
                return Task.FromResult(true);
            } else { return Task.FromResult(false); }
        }

        private Task<bool> restartPMS()
        {
            // Check if PMS is running
            Process[] processes = Process.GetProcessesByName("Plex Media Server");
            if (processes.Length > 0)
            {
                this.addLog("Restarting PMS");
                var filename = processes[0].MainModule.FileName;
                processes[0].Kill();
                // Make sure we don't start as a child process
                using (var managementClass = new ManagementClass("Win32_Process"))
                {
                    var processInfo = new ManagementClass("Win32_ProcessStartup");
                    processInfo.Properties["CreateFlags"].Value = 0x00000008;

                    var inParameters = managementClass.GetMethodParameters("Create");
                    inParameters["CommandLine"] = filename;
                    inParameters["ProcessStartupInformation"] = processInfo;

                    var result = managementClass.InvokeMethod("Create", inParameters, null);
                    if ((result != null) && ((uint)result.Properties["ReturnValue"].Value != 0))
                    {
                        Console.WriteLine("Process ID: {0}", result.Properties["ProcessId"].Value);
                    }
                }
            }
            else
            {
                this.addLog("PMS Not running");
                this.addLog("Locating PMS Install directory");
                // We need to find install dir
                RegistryKey pmsKeys = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\PlexUpdateService", false);
                string updatesvc = (string)pmsKeys.GetValue("ImagePath");
                pmsKeys.Close();
                var path = System.IO.Path.GetDirectoryName(updatesvc);
                string pmsPath = path + "\\Plex Media Server.exe";
                // Make sure we don't start as a child process
                using (var managementClass = new ManagementClass("Win32_Process"))
                {
                    var processInfo = new ManagementClass("Win32_ProcessStartup");
                    processInfo.Properties["CreateFlags"].Value = 0x00000008;

                    var inParameters = managementClass.GetMethodParameters("Create");
                    inParameters["CommandLine"] = pmsPath;
                    inParameters["ProcessStartupInformation"] = processInfo;

                    var result = managementClass.InvokeMethod("Create", inParameters, null);
                    if ((result != null) && ((uint)result.Properties["ReturnValue"].Value != 0))
                    {
                        Console.WriteLine("Process ID: {0}", result.Properties["ProcessId"].Value);
                    }
                }
                this.addLog("PMS has been started");
            }
            return Task.FromResult(true);
        }

        private async void claimIt()
        {
            if ( await this.loginPlexTVAsync())
            {
                // Logged in okay, let's get a claim token
                if (await this.getClaimToken())
                {
                    this.addLog("Got a claim token");
                    // Now we need an exchange token (The real deal )
                    if ( await this.getExchangeToken())
                    {
                        this.addLog("Got an exchange token");
                        this.addLog("Updating Registry");
                        if ( await this.updateRegistry() )
                        {
                            this.addLog("Registry updated");
                            if ( await this.restartPMS())
                            {
                                this.addLog("All Done");
                            }
                        } else
                        {
                            this.addLog("Failed to update the registry");
                        }
                    } else
                    {
                        this.addLog("Error getting an exchange token");
                    }
                } else
                {
                    this.addLog("Error getting a claim token");
                }
            }
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
            this.addLog("Welcome to " + name);
            this.addLog("");
            this.addLog("Note: PMS = Plex Media Server");
            this.addLog("");
            this.addLog("This 3rd. Party tool will assist you in (Re-) Claiming a PMS");
            this.addLog("");
            this.addLog("This tool MUST be run on the PMS machine, and as the user that normally runs the PMS");
            this.addLog("");
            this.addLog("");
            if (pmsPresent())
            {
                this.addLog("We found an installed PMS, so far, so good");
                this.addLog("");
                this.addLog("PMS Name detected as: " + FriendlyName);
                if (this.PlexOnlineToken != "")
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
            this.addLog("Start login towards plex.tv");
            this.claimIt();
        }
    }
}
