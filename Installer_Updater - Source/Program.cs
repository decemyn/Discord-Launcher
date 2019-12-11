using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Principal;
using System.IO;
using System.Reflection;
namespace DiscordLauncherInstaller
{
    static class Program
    {

        public static bool installer_integrity_check()
        {
            DirectoryInfo installer_directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            bool found_executable = false;
            bool found_config = false;
            foreach(FileInfo installer_file in installer_directory.GetFiles())
            {
                if(installer_file.Extension==".exe")
                {
                    AssemblyName temp_assembly = AssemblyName.GetAssemblyName(installer_file.FullName);
                    if(temp_assembly.Name=="DiscordLauncher")
                    {
                        found_executable = true;
                    }
                }
                if (installer_file.Extension == ".config")
                    found_config = true;
            }
            if(found_executable==true&&found_config==true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [STAThread]
        static void Main()
        {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (isElevated)
            {
                if (installer_integrity_check() == true)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new installer_menu());
                }
                else
                {
                    MessageBox.Show("Installer integrity check failed! Required installer files are missing!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("The application requires administrative privileges to run!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
