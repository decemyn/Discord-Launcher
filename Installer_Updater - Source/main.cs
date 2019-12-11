//INSTALLER MAIN CODE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Security;
using System.Reflection;
using IWshRuntimeLibrary;
using System.Net;
using System.Drawing.Text;
namespace DiscordLauncherInstaller
{

    public partial class installer_menu : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private string default_path = @"C:\Program Files (x86)";
        private string install_path = @"C:\Program Files (x86)"; //PATH INITIALIZATION
        private string registry_path;
        private bool from_uninstall_close_check = false;
        //SHORTCUT STATE
        private bool shortcut_state = true;
        //INSTALLER ASSEMBLY VERSION FILE
        private string installation_version;

        //GUI UPDATE METHODS
        private void failure_state_update_ui()
        {
            notok_1.Visible = true;
            notok_2.Visible = true;
            second_step.Text = "FAILURE";
            second_step.Visible = true;
            failure_panel.Location = new Point(225, 30);
            failure_panel.Visible = true;
        }
        private void completion_state_update_ui()
        {
            ok_1.Visible = true;
            ok_2.Visible = true;
            second_step.Text = "COMPLETED";
            second_step.Visible = true;
            done_ui_pic.Visible = true;
            completion_panel.Location = new Point(225, 30);
            completion_panel.Visible = true;
        }

        private void CreateShortcut(string install_path)
        {
            bool ok_operation = false;
            string combined_path = Path.Combine(install_path, "Discord Launcher");
            string link = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                + Path.DirectorySeparatorChar + Application.ProductName + ".lnk";
            var shell = new WshShell();
            var shortcut = shell.CreateShortcut(link) as IWshShortcut;

            //GET INSTALLED APPLICATION ASSEMBLY FROM INSTALL PATH
            DirectoryInfo installation_directory = new DirectoryInfo(combined_path);
            foreach(FileInfo install_file in installation_directory.GetFiles())
            {
                if(install_file.Extension==".exe")
                {
                    AssemblyName launcher_assembly = AssemblyName.GetAssemblyName(install_file.FullName);
                    if (launcher_assembly.Name == "DiscordLauncher")
                    {
                        ok_operation = true;
                        shortcut.TargetPath = install_file.FullName;
                        shortcut.WorkingDirectory = combined_path;
                        shortcut.Save();
                        break;
                    }
                }
            }
            if(ok_operation==false)
            {
                MessageBox.Show("An error ocurred when creating a desktop shortcut!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public installer_menu()
        {
            InitializeComponent();
        }



        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            install_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.install_button_click;
        }

        private void install_button_MouseUp(object sender, MouseEventArgs e)
        {
            install_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.install_button;
        }

        private void install_button_Click(object sender, EventArgs e)
        {
            path_panel.Visible = false;
            bool failure=false;
            try
            {

                DirectoryInfo installation_directory = new DirectoryInfo(install_path);//GET RAW INSTALL DIRECTORY(BEFORE SUBDIRECTORY CREATION)

                installation_directory.CreateSubdirectory("Discord Launcher");//CREATE INSTALL SUBDIRECTORY

                DirectoryInfo installation_directory_subdirectory = new DirectoryInfo(Path.Combine(installation_directory.FullName, "Discord Launcher"));//GET SUBDIRECTORY

                DirectoryInfo installer_assembly_directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory); //GET INSTALLER ASSEMBLY DIRECTORY

                AssemblyName installer_assembly = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location);


                foreach (FileInfo launcher_file in installer_assembly_directory.GetFiles())
                {
                    bool not_exe = false;
                    try
                    {
                        AssemblyName launcher_file_temp_assembly = AssemblyName.GetAssemblyName(launcher_file.FullName);
                        if (launcher_file_temp_assembly.Name != installer_assembly.Name)
                        {
                            try
                            {
                                launcher_file.CopyTo(Path.Combine(installation_directory_subdirectory.FullName, launcher_file.Name), true);
                            }
                            catch (IOException)
                            {
                                failure_state_update_ui();
                                MessageBox.Show("Installer failed to copy a file!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                failure = true;
                                break;
                            }
                        }
                    }
                    catch (BadImageFormatException)
                    {
                        not_exe = true;
                    }
                    if (not_exe == true)
                    {
                        try
                        {
                            if (launcher_file.Extension == ".config")
                                launcher_file.CopyTo(Path.Combine(installation_directory_subdirectory.FullName, launcher_file.Name), true);
                        }
                        catch (IOException)
                        {
                            failure_state_update_ui();
                            MessageBox.Show("Installer failed to copy a file!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            failure = true;
                            break;
                        }
                    }
                }

                if (failure == false)
                {
                    bool registry_failure = false;
                    //HANDLING CREATION OF THE REGISTRY PATH KEY
                    try
                    {
                        RegistryKey discord_launcher_registry_key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey("Software", true); ;
                        RegistryKey final_key = discord_launcher_registry_key.CreateSubKey("DiscordLauncher");
                        if (final_key != null)
                        {
                            final_key.SetValue("path", installation_directory_subdirectory.FullName, RegistryValueKind.String);
                        }
                        else
                        {
                            registry_failure = true;
                            failure_state_update_ui();
                            MessageBox.Show("Error in managing registry keys!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch(ObjectDisposedException)
                    {
                        registry_failure = true;
                        failure_state_update_ui();
                        MessageBox.Show("Error in managing registry keys!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if(registry_failure==false)
                    {
                        completion_state_update_ui();
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                failure_state_update_ui();
                MessageBox.Show("The installer requires Administrator privileges!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(SecurityException)
            {
                failure_state_update_ui();
                MessageBox.Show("The installer requires Administrator privileges!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_MouseDown_1(object sender, MouseEventArgs e)
        {
            default_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.default_button_click;
        }

        private void default_button_MouseUp(object sender, MouseEventArgs e)
        {
            default_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.default_button;
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            browse_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.browse_button_click;
        }

        private void browse_button_MouseUp(object sender, MouseEventArgs e)
        {
            browse_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.browse_button;
        }

        private void default_button_Click(object sender, EventArgs e)
        {
            install_path = default_path;
            string display_path = Path.Combine(install_path, "Discord Launcher");
            installation_path_control.Text = display_path;
        }

        private void browse_button_MouseDown(object sender, MouseEventArgs e)
        {
            browse_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.browse_button_click;
        }

        private void browse_button_MouseUp_1(object sender, MouseEventArgs e)
        {
            browse_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.browse_button;
        }

        private void browse_button_Click(object sender, EventArgs e)
        {
            install_path_dialog.ShowDialog();
            install_path = install_path_dialog.SelectedPath;
            string display_path = Path.Combine(install_path, "Discord Launcher");
            installation_path_control.Text = display_path;
        }

        private void close_button_MouseDown(object sender, MouseEventArgs e)
        {
            close_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.close_button_click;
        }

        private void close_button_MouseUp(object sender, MouseEventArgs e)
        {
            close_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.close_button;
        }

        private bool installation_integrity_check(string installation_path)
        {
            DirectoryInfo installation_directory = new DirectoryInfo(installation_path);
            bool found_executable = false;
            bool found_config = false;
            foreach (FileInfo installation_file in installation_directory.GetFiles())
            {
                if (installation_file.Extension == ".exe")
                {
                    AssemblyName temp_assembly = AssemblyName.GetAssemblyName(installation_file.FullName);
                    if (temp_assembly.Name == "DiscordLauncher")
                    {
                        installation_version = temp_assembly.Version.ToString();
                        found_executable = true;
                    }
                }
                if (installation_file.Extension == ".config")
                    found_config = true;
            }
            if (found_executable == true && found_config == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string installer_assembly_version_get()
        {
            DirectoryInfo installer_directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            foreach(FileInfo installer_file in installer_directory.GetFiles())
            {
                if(installer_file.Extension==".exe")
                {
                    AssemblyName temp_assembly = AssemblyName.GetAssemblyName(installer_file.FullName);
                    if(temp_assembly.Name=="DiscordLauncher")
                    {
                        return temp_assembly.Version.ToString();
                    }
                }
            }
            return String.Empty;
        }
        private string github_get_latest_version()
        {
            string github_Latest_Release_path = "https://github.com/decemyn/Discord-Launcher/releases/latest";
            WebRequest github_request = WebRequest.Create(github_Latest_Release_path);
            WebResponse github_response = github_request.GetResponse();
            string github_response_path = github_response.ResponseUri.ToString();
            string healty_response_path = "https://github.com/decemyn/Discord-Launcher/releases/tag/v";
            try
            {
                if (github_response_path.Substring(0, github_response_path.Length - 7) == healty_response_path)
                {
                    try
                    {
                        string github_latest_version = github_response_path.Substring(github_response_path.LastIndexOf('/') + 2, 7);
                        return github_latest_version;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                    }
                    github_response.Close();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return string.Empty;
            }
            return string.Empty;
        }
        private void installer_menu_Load(object sender, EventArgs e)
        {
            this.Size = new Size(800, 500);
            side_block.Location = new Point(0, 30);
            //DETERMINE INSTALLER FUNCTION
            RegistryKey discord_launcher_registry_key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey("Software", true);
            RegistryKey final_key = discord_launcher_registry_key.OpenSubKey("DiscordLauncher");
            if(final_key==null)
            {
                path_panel.Location = new Point(225, 30);
                path_panel.Visible = true;
                first_step.Text = "CHOOSE A PATH";
            }
            else
            {
                try
                {
                    if (Directory.Exists(final_key.GetValue("path").ToString()))
                    {
                        //CHECK INSTALLED FILES FOR INTEGRITY
                        if(installation_integrity_check(final_key.GetValue("path").ToString())==true)
                        {
                            if (AppDomain.CurrentDomain.BaseDirectory != final_key.GetValue("path").ToString()+@"\")
                            {
                                //UPDATE MODULE
                                string latest_version = github_get_latest_version();
                                if (latest_version != string.Empty)
                                {
                                    if (latest_version != installation_version)
                                    {
                                        string installer_file_version = installer_assembly_version_get();
                                        if (latest_version==installer_file_version)
                                        {
                                            //UPDATE SAFE
                                            registry_path = final_key.GetValue("path").ToString();
                                            first_step.Text = "UPDATE";
                                            current_version.Text += " " + installation_version;
                                            latest_version_label.Text += " " + latest_version;
                                            update_panel.Location = new Point(225, 30);
                                            update_panel.Visible = true;
                                        }
                                        else
                                        {
                                            //UPDATE NOT UPLOADED YET CASE(BUG FIX)
                                            //UNINSTALL MODULE
                                            registry_path = final_key.GetValue("path").ToString();
                                            first_step.Text = "UNINSTALL";
                                            uninstall_tag.Text += " " + installation_version;
                                            uninstall_panel.Location = new Point(225, 30);
                                            uninstall_panel.Visible = true;
                                        }
                                    }
                                    else
                                    {
                                        //UNINSTALL MODULE
                                        registry_path = final_key.GetValue("path").ToString();
                                        first_step.Text = "UNINSTALL";
                                        uninstall_tag.Text += " " + installation_version;
                                        uninstall_panel.Location = new Point(225, 30);
                                        uninstall_panel.Visible = true;
                                    }
                                }
                                else
                                {
                                    //UPDATE UNSAFE
                                    MessageBox.Show("Couldn't detect latest GitHub version! Update files couldn't be verified!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    registry_path = final_key.GetValue("path").ToString();
                                    first_step.Text = "UPDATE";
                                    current_version.Text += " " + "N/A";
                                    latest_version_label.Text += " " + "N/A";
                                    update_panel.Location = new Point(225, 30);
                                    update_panel.Visible = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("The application installer can't reside in the installation directory!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Application.Exit();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Installation integrity check fail! Reinstall the program by running the installer again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            try
                            {
                                discord_launcher_registry_key.DeleteSubKeyTree("DiscordLauncher");
                            }
                            catch (ArgumentException)
                            {
                                ;
                            }
                            path_panel.Location = new Point(225, 30);
                            path_panel.Visible = true;
                            first_step.Text = "CHOOSE A PATH";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Installation integrity check fail! Reinstall the program by running the installer again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        try
                        {
                            discord_launcher_registry_key.DeleteSubKeyTree("DiscordLauncher");
                        }
                        catch (ArgumentException)
                        {
                            ;
                        }
                        path_panel.Location = new Point(225, 30);
                        path_panel.Visible = true;
                        first_step.Text = "CHOOSE A PATH";
                    }
                }
                catch(NullReferenceException)
                {
                    MessageBox.Show("Installation integrity check fail! Reinstall the program by running the installer again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    try
                    {
                        discord_launcher_registry_key.DeleteSubKeyTree("DiscordLauncher");
                    }
                    catch (ArgumentException)
                    {
                        ;
                    }
                    path_panel.Location = new Point(225, 30);
                    path_panel.Visible = true;
                    first_step.Text = "CHOOSE A PATH";
                }
            }
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            if (from_uninstall_close_check == false)
                if (shortcut_state == true)
                    CreateShortcut(install_path);
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(shortcut_state==true)
            {
                shortcut_state = false;
                shortcut_box.Image = DiscordLauncherInstaller.Properties.Resources.check_box_off;
            }
            else
            {
                shortcut_state = true;
                shortcut_box.Image = DiscordLauncherInstaller.Properties.Resources.check_box_on;
            }
        }

        private void close_button_fail_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void close_button_fail_MouseDown(object sender, MouseEventArgs e)
        {
            close_button_fail.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.close_button_click;
        }

        private void close_button_fail_MouseUp(object sender, MouseEventArgs e)
        {
            close_button_fail.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.close_button;
        }

        private void update_button_Click(object sender, EventArgs e)
        {
            update_panel.Visible = false;
            bool failure = false;
            try
            {

                DirectoryInfo installation_directory = new DirectoryInfo(registry_path);

                DirectoryInfo installer_assembly_directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

                AssemblyName installer_assembly = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location);


                foreach (FileInfo launcher_file in installer_assembly_directory.GetFiles())
                {
                    bool not_exe = false;
                    try
                    {
                        AssemblyName launcher_file_temp_assembly = AssemblyName.GetAssemblyName(launcher_file.FullName);
                        if (launcher_file_temp_assembly.Name != installer_assembly.Name)
                        {
                            try
                            {
                                launcher_file.CopyTo(Path.Combine(installation_directory.FullName, launcher_file.Name), true);
                            }
                            catch (IOException)
                            {
                                failure_state_update_ui();
                                failure = true;
                                MessageBox.Show("Installer failed to copy a file!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                    }
                    catch (BadImageFormatException)
                    {
                        not_exe = true;
                    }
                    if (not_exe == true)
                    {
                        try
                        {
                            if (launcher_file.Extension == ".config")
                                launcher_file.CopyTo(Path.Combine(installation_directory.FullName, launcher_file.Name), true);
                        }
                        catch (IOException)
                        {
                            failure_state_update_ui();
                            failure = true;
                            MessageBox.Show("Installer failed to copy a file!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                failure_state_update_ui();
                failure = true;
                MessageBox.Show("The installer requires Administrator privileges!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SecurityException)
            {
                failure_state_update_ui();
                failure = true;
                MessageBox.Show("The installer requires Administrator privileges!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if(failure==false)
            completion_state_update_ui();
        }

        private void update_button_MouseDown(object sender, MouseEventArgs e)
        {
            update_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.update_button_click;
        }

        private void update_button_MouseUp(object sender, MouseEventArgs e)
        {
            update_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.update_button;
        }

        private void uninstall_button_MouseDown(object sender, MouseEventArgs e)
        {
            uninstall_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.uninstall_button_click;
        }

        private void uninstall_button_MouseUp(object sender, MouseEventArgs e)
        {
            uninstall_button.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.uninstall_button;
        }

        private void uninstall_button_2_MouseDown(object sender, MouseEventArgs e)
        {
            uninstall_button_2.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.uninstall_button_click;
        }

        private void uninstall_button_2_MouseUp(object sender, MouseEventArgs e)
        {
            uninstall_button_2.BackgroundImage = DiscordLauncherInstaller.Properties.Resources.uninstall_button;
        }
        //UNINSTALL APPLICATION MODULE
        private void uninstall_button_2_Click(object sender, EventArgs e)
        {
            uninstall_panel.Visible = false;
            from_uninstall_close_check = true;
            bool installation_fail = false;
            DirectoryInfo installation_directory = new DirectoryInfo(registry_path);
            RegistryKey uninstall_key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey("Software", true);
            try
            {
                uninstall_key.DeleteSubKeyTree("DiscordLauncher");
            }
            catch (ArgumentException)
            {
                ;
            }
            try
            {
                try
                {
                    foreach (FileInfo installation_file in installation_directory.GetFiles())
                    {
                        installation_file.Delete();
                    }
                    installation_directory.Delete();
                }
                catch
                {
                    installation_fail = true;
                }
            }
            catch(DirectoryNotFoundException)
            {
                ;
            }
            if(installation_fail==false)
            {
                shortcut_label.Text = "Application uninstallation completed successfully!";
                shortcut_label.Location = new Point(10, 150);
                shortcut_box.Visible = false;
                completion_state_update_ui();
            }
        }

        private void uninstall_button_Click(object sender, EventArgs e)
        {
            update_panel.Visible = false;
            first_step.Text = "UNINSTALL";
            uninstall_panel.Visible = false;
            from_uninstall_close_check = true;
            bool installation_fail = false;
            DirectoryInfo installation_directory = new DirectoryInfo(registry_path);
            RegistryKey uninstall_key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey("Software", true);
            try
            {
                uninstall_key.DeleteSubKeyTree("DiscordLauncher");
            }
            catch (ArgumentException)
            {
                ;
            }
            try
            {
                try
                {
                    foreach (FileInfo installation_file in installation_directory.GetFiles())
                    {
                        installation_file.Delete();
                    }
                    installation_directory.Delete();
                }
                catch
                {
                    installation_fail = true;
                }
            }
            catch (DirectoryNotFoundException)
            {
                ;
            }
            if (installation_fail == false)
            {
                shortcut_label.Text = "Application uninstallation completed successfully!";
                shortcut_label.Location = new Point(10, 150);
                shortcut_box.Visible = false;
                completion_state_update_ui();
            }
        }
    }
}
