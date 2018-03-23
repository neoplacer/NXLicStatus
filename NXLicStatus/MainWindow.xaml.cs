using System;
using System.Collections.Generic;
using System.Collections; 
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Diagnostics;

namespace NXLicStatus
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        //string LicData; 
        ArrayList partlist;
        public MainWindow()
        {
            InitializeComponent();
            getItem();
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            getItem();
        }

        private void getItem()
        {
            this.LicenseOverview.Items.Clear();
            this.partlist = new ArrayList(); 
           // this.LicData = String.Empty;

            Process cmd = new Process();           
            cmd.StartInfo.FileName = Environment.GetEnvironmentVariable("UGII_BASE_DIR") + "\\UGFLEXLM\\lmutil.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = "lmstat -f -c 28000@license";
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.ErrorDataReceived += build_ErrorDataReceived;
            cmd.OutputDataReceived += build_ErrorDataReceived;
            cmd.Start();
            cmd.BeginOutputReadLine();
            cmd.BeginErrorReadLine();
            cmd.WaitForExit();
            string PatternLicenseUse = @"^Users of (\w+|_): +\(Total of (\d+) license\w* issued; +Total of (\d+) license\w* in use";
            string PatternLicenseUsers = @"(\w+) (\S+) \S+ \(v(\d+.\d+)\) \(\S+\/\d+ \d+\), start (\w+ \d+/\d+ \d+:\d+)";
            TreeViewItem retval = new 	TreeViewItem();
            foreach (string eachline in partlist)
            {

                TreeViewItem License = new TreeViewItem();
                TreeViewItem LicenseUser = new TreeViewItem();
               // Debug.Print(eachline);
                 foreach(Match match in Regex.Matches( eachline, PatternLicenseUse))
                {
                    
                    Debug.Print("{0}   {1} / {2}  ",
                       match.Groups[1].Value,match.Groups[2].Value,  match.Groups[3]);
                     License.Header=String.Format("{0}   {1} / {2}  ", match.Groups[1].Value, match.Groups[2].Value,  match.Groups[3]);
                     this.LicenseOverview.Items.Add(License);
                }
                 foreach (Match match in Regex.Matches(eachline, PatternLicenseUsers))
                {
                       
                    //Username
                    Debug.Print("Found '{0}' at position {1}",
                         match.Groups[1].Value, match.Index);
                    // PC
                    Debug.Print("Found '{0}' at position {1}",
                       match.Groups[2].Value, match.Index);
                    //Datetime
                    Debug.Print("Found '{0}' at position {1}",
                      match.Groups[4].Value, match.Index);
                     LicenseUser.Header=String.Format("User: {0} PC: {1} um {2}  ", match.Groups[1].Value, match.Groups[2].Value,  match.Groups[4]);

                     TreeViewItem myItem = (TreeViewItem)this.LicenseOverview.Items.GetItemAt(this.LicenseOverview.Items.Count-1);
                     this.LicenseOverview.Items.RemoveAt(this.LicenseOverview.Items.Count-1);
                     myItem.Items.Add(LicenseUser);
                     this.LicenseOverview.Items.Add(myItem);

                }
      
            }
           
        }

        void build_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null) if (e.Data.Length > 2) partlist.Add(e.Data);
        }
    }
}
