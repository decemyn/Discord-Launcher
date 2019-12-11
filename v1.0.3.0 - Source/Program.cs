using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
namespace PriorityTool
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string executable_path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            if (Directory.Exists(executable_path))
            {
                DirectoryInfo application_directory = new DirectoryInfo(executable_path);
                FileInfo[] application_files = application_directory.GetFiles();
                foreach (FileInfo file in application_files)
                {
                    if (file.Name == "DiscordLauncher.exe.config")
                    {
                        Main_menu.found_settings = true;
                        Process[] app_safety = Process.GetProcessesByName("discordlauncher");
                        if (app_safety.Length == 1)
                        {
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new Main_menu());
                        }
                        else
                        {
                            MessageBox.Show("Application is already running.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                if(Main_menu.found_settings==false)
                    MessageBox.Show("Application configuration file cannot be found! Run installer again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}