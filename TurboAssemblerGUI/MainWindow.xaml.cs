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
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
namespace TurboAssemblerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private static string compilerPath;
        private static string name;
        private static string exePath;
        private static string filePath;

        public MainWindow()
        {
            Closing += MainWindow_Closing;


            bool is64bit = !string.IsNullOrEmpty(
    Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"));
            if (is64bit)
            {
                System.Windows.Forms.MessageBox.Show(@"You have a 64-bit operation system."+'\n'+@"You'l have to install DOS-BOX in order to compile x86 TASM projects." + '\n' + @"Then, Type:"+'\n'+@"cd 'ta-gui.exe-File-Location'\Exes\ (Without the '')"+'\n'+@"Then just type the .exe file to run it, or the turbo-debugger path to debug it."+'\n'+@"I know it's kind of messy, and I am working on a solution for this."+'\n'+@"                              Check for updates at www.C0LDSH3LL.com", "64-Bit OS Notice");
                System.Diagnostics.Process.Start(@"C:\Program Files (x86)\DOSBox-0.74\DOSBox.exe");
            }

            try
            {
                exePath = System.Windows.Forms.Application.ExecutablePath;
                exePath = exePath.Substring(0, exePath.LastIndexOf(@"\") + 1);

                if (!System.IO.Directory.Exists(exePath + @"\Exes"))
                    System.IO.Directory.CreateDirectory(exePath + @"\Exes");

                if (!File.Exists(exePath + @"\Projects\cPath.xml"))
                {

                    string startupPath = System.Windows.Forms.Application.StartupPath;
                    using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                    {
                        dialog.Description = "Please Choose Your Compiler's Folder Location";
                        dialog.ShowNewFolderButton = false;
                        dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                        dialog.ShowDialog();
                        compilerPath = dialog.SelectedPath;

                    }
                    System.IO.Directory.CreateDirectory(exePath + @"Projects\");
                    FileStream fs = File.Create(exePath + @"Projects\cPath.xml");
                    fs.Close();
                    System.IO.File.WriteAllText(exePath + @"Projects\cPath.xml", compilerPath);
                }

                else
                    compilerPath = System.IO.File.ReadAllText(exePath + @"Projects\cPath.xml");
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            InitializeComponent();

            int lineNumber = 0001;
            for (int i = 0; i < 9999; i++)
            {
                if (lineNumber < 10)
                    LinesNBox.Text += "000" + lineNumber.ToString() + "\n";
                if (lineNumber < 100 && lineNumber >= 10)
                    LinesNBox.Text += "00" + lineNumber.ToString() + "\n";
                if (lineNumber < 1000 && lineNumber >= 100)
                    LinesNBox.Text += "0" + lineNumber.ToString() + "\n";
                if (lineNumber >= 1000)
                    LinesNBox.Text += lineNumber.ToString() + "\n";
                lineNumber++;
            }


        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process[] prs = Process.GetProcesses();
            foreach (Process pr in prs)
            {
                if (pr.ProcessName == "DOSBox")
                {

                    pr.Kill();
                    break;
                }

            }
        }

        private void scrollViewerLeft_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            scrollViewerRight.ScrollToVerticalOffset((sender as ScrollViewer).VerticalOffset);
        }

        private void Menu_Help_Click(object sender, RoutedEventArgs e)
        {
            HelpScreen hs = new HelpScreen();
            hs.Show();
        }

        private void Menu_Report_Click(object sender, RoutedEventArgs e)
        {
            ReportScreen2 rs = new ReportScreen2();
            rs.Show();
        }

        public static int ExecuteCommand(string Command)
        {
            try
            {
                ProcessStartInfo ProcessInfo;
                Process Process;

                ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + Command);
                ProcessInfo.CreateNoWindow = true;
                ProcessInfo.UseShellExecute = true;
                Process = Process.Start(ProcessInfo);
                Process.Close();

                return 0;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return 1;
            }
        }

        private void Menu_Compile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string exeProg = name.Substring(0, name.LastIndexOf('.')) + ".exe";
                string tasm = compilerPath + @"\bin\tasm " + exePath + @"Projects\" + name;
                ExecuteCommand(tasm);
                System.Threading.Thread.Sleep(100);
                string linkProg = name.Substring(0, name.LastIndexOf('.')) + ".OBJ";
                string maplocation = name.Substring(0, name.LastIndexOf('.')) + ".MAP";
                string tlink = compilerPath + @"\bin\tlink " + exePath + linkProg;
                ExecuteCommand(tlink);
                System.Threading.Thread.Sleep(300);
                File.Delete(exePath + linkProg);
                File.Delete(exePath + maplocation);
                File.Move(exePath + exeProg, exePath + @"\Exes\" + exeProg);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("You Have To Save The Project Or Open One First!");
            }
        }

        private void Menu_Donate_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("PAYPAL CONNECTION HERE");
        
        }

        private void Menu_Open_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "File"; // Default file name
                dlg.DefaultExt = ".asm"; // Default file extension
                dlg.Filter = "Text documents (.asm)|*.asm"; // Filter files by extension 

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    string filename = dlg.FileName;
                    filePath = filename;
                    string text = System.IO.File.ReadAllText(filename);// ReadAllLines(filename);
                    CodeBox.IsEnabled = true;
                    name = filename.Substring(filename.LastIndexOf(@"\") + 1);
                    Title = "Turbo Assembler GUI - " + name;
                    CodeBox.Text = text;
                    NewProjLBL.Visibility = Visibility.Hidden;
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Menu_Debug_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string exeProg = name.Substring(0, name.LastIndexOf('.')) + ".exe";
                string td = compilerPath + @"\bin\td " + exePath + @"Exes\" + exeProg;
                ExecuteCommand(td);
            }
            catch (NullReferenceException)
            {
                System.Windows.MessageBox.Show("You Have To Save The Project Or Open One First!");
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Menu_CompilerPathChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string startupPath = System.Windows.Forms.Application.StartupPath;
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "Open a folder which contains the xml output";
                    dialog.ShowNewFolderButton = false;
                    dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                    dialog.ShowDialog();
                    compilerPath = dialog.SelectedPath;

                }
                System.IO.File.WriteAllText(exePath + @"Projects\cPath.xml", compilerPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Menu_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            Try2:
                name = Microsoft.VisualBasic.Interaction.InputBox("Save My Project As..", "Save Project As..", "___.asm");
                if (!name.Contains(".asm"))
                    name = name + ".asm";

                if (name.Length > 12)
                {
                    System.Windows.Forms.MessageBox.Show("Projects Name-Length In TASM Must To Be Less Or Equal To 12 Characters! Please Try Again..");
                    goto Try2;
                }
                FileStream fs = System.IO.File.Create(exePath + name);
                fs.Close();

                string[] s = CodeBox.Text.Split(new char[] { '\n' });
                File.WriteAllLines(exePath + @"Projects\" + name, s);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are You Sure That You Want To Exit?\nMake Sure That You Saved The Project First!", "Exit", MessageBoxButtons.YesNo);
                if (dialogResult.ToString() == "Yes")
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Menu_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (name != null)
                {
                    string[] s = CodeBox.Text.Split(new char[] { '\n' });

                    if (filePath == null)
                        File.WriteAllLines(exePath + @"Projects\" + name, s);
                    else
                        File.WriteAllLines(filePath, s);
                }
                else
                {

                    System.Windows.MessageBox.Show("You Have To Save The Project Or Open One First!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }


        private void Menu_New_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            Try3:
                name = Microsoft.VisualBasic.Interaction.InputBox("Please Choose A Name For Your Assembler Project", "Create New Project", "___.asm");
                if (!name.Contains(".asm"))
                    name = name + ".asm";

                if (name.Length > 12)
                {
                    System.Windows.Forms.MessageBox.Show("Projects Name-Length In TASM Must To Be Less Or Equal To 12 Characters! Please Try Again..");
                    goto Try3;
                }
                if (name == "___.asm" || name == "" || name == ".asm")
                {
                    goto Cancled;
                }
                if (!System.IO.Directory.Exists(exePath + @"\Projects"))
                    System.IO.Directory.CreateDirectory(exePath + @"\Projects");

                if (!System.IO.Directory.Exists(exePath + @"\Exes"))
                    System.IO.Directory.CreateDirectory(exePath + @"\Exes");

                FileStream fs = System.IO.File.Create(exePath + @"Projects\" + name);
                fs.Close();
                CodeBox.IsEnabled = true;
                string[] s = CodeBox.Text.Split(new char[] { '\n' });
                File.WriteAllLines(exePath + @"Projects\" + name, s);
                NewProjLBL.Visibility = Visibility.Hidden;
                Title = "Turbo Assembler GUI - " + name;

            Cancled:
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void NewProjLBL_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Menu_New_Click(sender, e);
        }

        private void CodeBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            if ((System.Windows.Forms.Control.ModifierKeys & Keys.S) == 0)
            {
                Menu_Save_Click(sender, e);
                Title = "Turbo Assembler GUI - " + name;
            }
        }


        private void CodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Title = "Turbo Assembler GUI - *" + name + "*";
        }

        private void Hyperlink_RequestNavigate_1(object sender, RequestNavigateEventArgs e)
        {

        }

    }
}