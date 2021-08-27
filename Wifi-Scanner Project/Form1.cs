using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Scan()
        {
            string output;
            string line;
            int BSSIDNumber = 0;
            int NetworkIndex = -1;
            string[,] Networks = new string[100, 12]; //array
            Process proc = new Process(); //from sys.Diagnostics
            proc.StartInfo.CreateNoWindow = true; //taa k nayi window na banaye ye cmd ki
            proc.StartInfo.FileName = "netsh"; //builtin command hoti hy windows ki....For more, type netsh in the cmd
            proc.StartInfo.Arguments = "wlan show networks mode=bssid"; //then type wlan show networks mode=bssid in cmd
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false; // false if the process should be created directly from the executable file
            proc.Start();
            output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            StringReader sr = new StringReader(output.ToString());
            line = null;

            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("General Failure"))
                {
                    // Wifi disconnected or not installed
                    break;
                }
                if (line.StartsWith("SSID"))
                {
                    NetworkIndex++;
                    for (int i = 0; i < 9; i++)
                    {
                        // prevent exception finding null on search
                        Networks[NetworkIndex, i] = " ";
                    }
                    // prevent exception for trim
                    Networks[NetworkIndex, 3] = "0%";
                    // reset the BSSID number
                    BSSIDNumber = 0;
                    Networks[NetworkIndex, 1] = line.Substring(line.IndexOf(":") + 1).TrimEnd(' ').TrimStart(' ');
                    continue;
                }

                if (line.IndexOf("Authentication") > 0)
                {
                    Networks[NetworkIndex, 4] = line.Substring(line.IndexOf(":") + 1).TrimStart(' ').TrimEnd(' ');
                    continue;
                }

                if (line.IndexOf("Encryption") > 0)
                {
                    Networks[NetworkIndex, 5] = line.Substring(line.IndexOf(":") + 1).TrimStart(' ').TrimEnd(' ');
                    continue;
                }


                if (line.IndexOf("Network type") > 0)
                {
                    Networks[NetworkIndex, 9] = line.Substring(line.IndexOf(":") + 1).TrimStart(' ').TrimEnd(' ');
                    continue;
                }

                if (line.IndexOf("BSSID") > 0)
                {
                    if ((Convert.ToInt32(line.IndexOf("BSSID" + 6)) > BSSIDNumber))
                    {
                        BSSIDNumber = Convert.ToInt32(line.IndexOf("BSSID" + 6));
                        NetworkIndex++;


                    }
                    Networks[NetworkIndex, 0] = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }



                if (line.IndexOf("Signal") > 0)
                {
                    Networks[NetworkIndex, 3] = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }
                if (line.IndexOf("Channel") > 0)
                {
                    Networks[NetworkIndex, 2] = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }


                if (line.IndexOf("Basic rates (Mbps)") > 0)
                {

                    Networks[NetworkIndex, 8] = line.Substring(line.IndexOf(":"));
                    if (Networks[NetworkIndex, 8] == ":")
                    {
                        Networks[NetworkIndex, 8] = "not shown";
                        continue;
                    }
                    Networks[NetworkIndex, 8] = Networks[NetworkIndex, 8].TrimStart(':').TrimStart(' ').TrimEnd(' ');
                    for (int i = Networks[NetworkIndex, 8].Length - 1; i > 0; i--)
                    {
                        if (Networks[NetworkIndex, 8].Substring(i, 1) == " ")
                        {
                            Networks[NetworkIndex, 8] = Networks[NetworkIndex, 8].Substring(i + 1, Networks[NetworkIndex, 8].Length - 1 - i);
                            break;
                        }
                    }
                }
               

                if (line.IndexOf("Radio type") > 0)
                {
                    Networks[NetworkIndex, 7] = line.Substring(line.IndexOf(":") + 1).TrimStart(' ').TrimEnd(' ');
                    continue;
                }
            }


            for (int i = 0; i < NetworkIndex + 1; i++)
            {
                ListViewItem SearchItem = new ListViewItem();

                if (Networks[i, 0] == " ")
                    // don't search if no valid MAC Address !
                    continue;
                SearchItem = listView1.FindItemWithText(Networks[i, 0]);
                if (SearchItem == null)
                {
                    // New discovery - add it to the list

                    SystemSounds.Hand.Play();

                    listView1.Items.Add(Networks[i, 1]);// MAC Address
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 0]);      // SSID
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 2]);      // Channel
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 3]);      // Signal
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 4]);      // Security type
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 8]);      //Basic rates
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 5]);      //Encryption
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 7]);      //Radio type
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(Networks[i, 9]);      //Network Type
                }
                else
                {
                    //  Already in list - update Signal and other details that may change

                    listView1.Items[SearchItem.Index].SubItems[3].Text = Networks[i, 3];      // Signal

                    // Don't change any details if blank 
                    if (Networks[i, 1] != null) listView1.Items[SearchItem.Index].SubItems[1].Text = Networks[i, 1];      // SSID
                    if (Networks[i, 4] != null) listView1.Items[SearchItem.Index].SubItems[4].Text = Networks[i, 4];      // Authenticatiopn
                    if (Networks[i, 5] != null) listView1.Items[SearchItem.Index].SubItems[5].Text = Networks[i, 5];      // Encryption



                }
            }
            
        }

        
        private void bunifuGradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bunifuFlatButton3_Click(object sender, EventArgs e)
        {
            Scan();
        }
        public int count;
        

       

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {

            
        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
