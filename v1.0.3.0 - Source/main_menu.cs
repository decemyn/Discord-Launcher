//MAIN MENU FORM CODE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using System.Security;
using Microsoft.Win32;
using System.IO;
using System.Configuration;
using System.Runtime.InteropServices;
namespace PriorityTool
{
    public partial class Main_menu : Form
    {
        //IMPORT TITLE BAR CONTROL
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        //RESIZE MODULE
        private const int
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;

        const int _ = 10;

        Rectangle Top { get { return new Rectangle(0, 0, this.ClientSize.Width, _); } }
        Rectangle Left { get { return new Rectangle(0, 0, _, this.ClientSize.Height); } }
        Rectangle Bottom { get { return new Rectangle(0, this.ClientSize.Height - _, this.ClientSize.Width, _); } }
        Rectangle Right { get { return new Rectangle(this.ClientSize.Width - _, 0, _, this.ClientSize.Height); } }

        Rectangle TopLeft { get { return new Rectangle(0, 0, _, _); } }
        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - _, 0, _, _); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - _, _, _); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - _, this.ClientSize.Height - _, _, _); } }


        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == 0x84)
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (Top.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (Left.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (Right.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (Bottom.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }
    //DEBUG STATE(IMPORTANT)
    private protected bool debug_state = false;
        //
        private Process[] discord_processes;
        private delegate void safe_call_updater();
        private delegate void safe_ui_updater();
        //DISCORD LISTENER CONSTANTS
        private bool state; //MAIN LISTENER KILL STATE
        private bool realtime_state;
        private bool performance_state;
        //GUI FLOW SAFETY
        static public bool change_log_activation_state = false;
        static public bool settings_menu_activation_state = false;
        //APP FIRST START FOR BALLON TOOLTIP
        private bool first_start_tooltip=true;
        //UPDATE CONSTANTS (IMPORTANT)
        private string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private bool currentlyChecking_update=false;
        private string github_global_version=string.Empty;
        //SETTINGS MENU CONSTANTS
        static public bool found_settings = false;
        private bool update_required;
        public Main_menu()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            //FORM RESIZE
            form_resize();
        }
        //THE MAIN DISCORD LISTENER/ PRIORITY CHANGER
        public void discord_listener()
        {
            while (state)
            {
                discord_processes = Process.GetProcessesByName("discord");
                update_run_tag();
                foreach(Process discord in discord_processes)
                {
                    try
                    {
                        if(realtime_state==true)
                        discord.PriorityClass = ProcessPriorityClass.High;
                        if(performance_state == true)
                        discord.PriorityClass = ProcessPriorityClass.BelowNormal;
                    }
                    catch(InvalidOperationException)
                    {
                        ;
                    }
                }
                Thread.Sleep(500);
            }
        }
        //GUI DISCORD APP STATE UPDATE METHOD
        public void update_run_tag()
        {
            if(discord_processes.Length!=0)
            {
                if (state_run_tag.InvokeRequired)
                {
                    safe_call_updater discord_delegate = new safe_call_updater(update_run_tag);
                    try
                    {
                        state_run_tag.Invoke(discord_delegate);
                    }
                    catch
                    {
                        state = false;
                    }
                }
                else
                {
                    state_run_tag.Image = DiscordLauncher.Properties.Resources.running_state_img;
                }
            }
            if (discord_processes.Length == 0)
            {
                if (state_run_tag.InvokeRequired)
                {
                    safe_call_updater discord_delegate = new safe_call_updater(update_run_tag);
                    try
                    {
                        state_run_tag.Invoke(discord_delegate);
                    }
                    catch
                    {
                        state = false;
                    }
                }
                else
                {
                    state_run_tag.Image = DiscordLauncher.Properties.Resources.not_running_state_img;
                }
            }
        }
        //TOOL STRIP ITEMS(SHOW/EXIT)
        //SHOW
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        //EXIT
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            state = false;
            if (DiscordLauncher.Properties.Settings.Default.kill_discord_process_app_close == true)
            {
                try
                {
                    Process[] catch_discord = Process.GetProcessesByName("discord");
                    foreach (Process discord in catch_discord)
                    {
                        discord.Kill();
                    }
                }
                catch (NullReferenceException)
                {
                    ;
                }
                catch (Win32Exception)
                {
                    ;
                }
            }
            Application.Exit();
        }
        //MINIMIZE APP HANDLING
        private void Main_menu_Move(object sender, EventArgs e)
        {
            if(this.WindowState==FormWindowState.Minimized)
            {
                this.Hide();
                if(DiscordLauncher.Properties.Settings.Default.disable_running_in_background_message == false)
                DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "Running in background.", ToolTipIcon.None);
                if (DiscordLauncher.Properties.Settings.Default.minimize_on_startup == true && DiscordLauncher.Properties.Settings.Default.start_discord_on_load == false && first_start_tooltip == true)
                {
                    first_start_tooltip = false;
                    DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "Running in background.", ToolTipIcon.None);
                }
            }

        }
        //SHOW APP AGAIN AFTER BEING MINIMIZED
        private void DiscordIconTray_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        //APP ON CLOSE HANDLER
        private void Main_menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            //SAVE FORM SIZE
            DiscordLauncher.Properties.Settings.Default.Width = this.Width;
            DiscordLauncher.Properties.Settings.Default.Height = this.Height;
            DiscordLauncher.Properties.Settings.Default.Save();
            state = false;
            if (DiscordLauncher.Properties.Settings.Default.kill_discord_process_app_close == true)
            {
                try
                {
                    Process[] catch_discord = Process.GetProcessesByName("discord");
                    foreach(Process discord in catch_discord)
                    {
                        discord.Kill();
                    }
                }
                catch (NullReferenceException)
                {
                    ;
                }
                catch (Win32Exception)
                {
                    ;
                }
            }
        }
        //MINIMIZE APP STARTUP METHOD
        private void minimize_on_startup()
        {
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            this.ShowInTaskbar = false;
        }
        //PULL DISCORD PATH REGISTRY
        private string registry_pull_discord_path()
        {
            string discord_path = string.Empty;
            try
            {
                RegistryKey discord_app_key = Registry.ClassesRoot.OpenSubKey(@"Discord\shell\open\command");
                if (discord_app_key != null)
                {
                    string discord_path_raw = discord_app_key.GetValue(string.Empty).ToString();
                    try
                    {
                        discord_path = discord_path_raw.Substring(discord_path_raw.IndexOf(@"C:\Users"), discord_path_raw.IndexOf(".exe") + 3);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return string.Empty;
                    }
                    if (discord_path != string.Empty)
                    {
                        return discord_path;
                    }
                }
            }
            catch (SecurityException)
            {
                return string.Empty;
            }
            return string.Empty;
        }
        //START DISCORD STARTUP METHOD
        private void start_discord_on_load()
        {
            Process[] throwaway = Process.GetProcessesByName("discord");
            if (throwaway.Length == 0)
            {
                if (DiscordLauncher.Properties.Settings.Default.discord_app_path == string.Empty)
                {
                    string discord_path = registry_pull_discord_path();
                    if (discord_path != string.Empty) 
                    {
                        DiscordLauncher.Properties.Settings.Default.discord_app_path = discord_path;
                        DiscordLauncher.Properties.Settings.Default.Save();
                    }
                    else
                    {
                        MessageBox.Show("Discord Launcher couldn't detect Discord's application executable! Please select it manually!", "Select path", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        OpenFileDialog discord_executable_selector = new OpenFileDialog();
                        discord_executable_selector.Title = "Select Discord's executable";
                        discord_executable_selector.Filter = "|*.exe";
                        discord_executable_selector.ShowDialog();
                        if(discord_executable_selector.FileName!=string.Empty)
                        {
                            DiscordLauncher.Properties.Settings.Default.discord_app_path = discord_executable_selector.FileName;
                            DiscordLauncher.Properties.Settings.Default.Save();
                        }
                    }
                }
                try
                {
                    FileInfo discord_file = new FileInfo(DiscordLauncher.Properties.Settings.Default.discord_app_path);
                    if (discord_file.Exists != false)
                    {
                        Process.Start(DiscordLauncher.Properties.Settings.Default.discord_app_path);
                        Process[] snoop_start = Process.GetProcessesByName("discord");
                        if (snoop_start.Length == 0)
                        {
                            MessageBox.Show("Wrong executable selected! Resetting path to default!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DiscordLauncher.Properties.Settings.Default.discord_app_path = string.Empty;
                            DiscordLauncher.Properties.Settings.Default.Save();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Discord Launcher couldn't detect Discord's application executable! Please select it manually!", "Select path", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        OpenFileDialog discord_executable_selector = new OpenFileDialog();
                        discord_executable_selector.Title = "Select Discord's executable";
                        discord_executable_selector.Filter = "|*.exe";
                        discord_executable_selector.ShowDialog();
                        if (discord_executable_selector.FileName != string.Empty)
                        {
                            DiscordLauncher.Properties.Settings.Default.discord_app_path = discord_executable_selector.FileName;
                            DiscordLauncher.Properties.Settings.Default.Save();
                        }
                    }
                }
                catch(ArgumentException)
                {
                    MessageBox.Show("Discord Launcher couldn't open Discord.exe!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DiscordLauncher.Properties.Settings.Default.discord_app_path = string.Empty;
                    DiscordLauncher.Properties.Settings.Default.Save();
                }
                catch(InvalidOperationException)
                {
                    MessageBox.Show("Discord Launcher couldn't open Discord.exe!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DiscordLauncher.Properties.Settings.Default.discord_app_path = string.Empty;
                    DiscordLauncher.Properties.Settings.Default.Save();
                }
                catch(Win32Exception)
                {
                    MessageBox.Show("Discord Launcher couldn't open Discord.exe!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DiscordLauncher.Properties.Settings.Default.discord_app_path = string.Empty;
                    DiscordLauncher.Properties.Settings.Default.Save();
                }
                catch(SecurityException)
                {
                    MessageBox.Show("Discord Launcher couldn't open Discord.exe!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DiscordLauncher.Properties.Settings.Default.discord_app_path = string.Empty;
                    DiscordLauncher.Properties.Settings.Default.Save();
                }
            }
            else
            {
                DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "Running in background.", ToolTipIcon.None);
            }
        }
        //DISCORD LISTENER THREAD AT STARTUP METHOD
        private void discord_listener_thread_auto_startup()
        {
            Thread discord_listener_thread_on_load = new Thread(discord_listener);
            discord_listener_thread_on_load.Start();
        }
        private void update_listener_thread_auto_startup()
        {
            Thread update_listener_thread = new Thread(update_listener_startup);
            update_listener_thread.Start();
        }
        //VERSION GUI UPDATE
        private void version_gui_update()
        {
            version.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        //UPDATE PERFORMANCE SETTINGS GUI ON LOAD
        private void performance_settings_gui()
        {
            if (DiscordLauncher.Properties.Settings.Default.low_state == true)
            {
                performance_state = true;
                realtime_state = false;
                low_state_button_ui.Image = DiscordLauncher.Properties.Resources.low_state_img_select;
            }
            if (DiscordLauncher.Properties.Settings.Default.high_state == true)
            {
                performance_state = false;
                realtime_state = true;
                high_state_button_ui.Image = DiscordLauncher.Properties.Resources.high_state_img_select;
            }
        }
        //SETTINGS UPGRADE HANDLER ON LOAD
        private void settings_upgrade_handler()
        {
            DiscordLauncher.Properties.Settings.Default.force_create = true;
            DiscordLauncher.Properties.Settings.Default.Save();
            string current_exe_settings_path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            DirectoryInfo configuration_directory = new DirectoryInfo(current_exe_settings_path);
            try
            {
                configuration_directory = configuration_directory.Parent.Parent;
            }
            catch(NullReferenceException)
            {

            }
            try
            {
                DirectoryInfo[] configuration_directory_files = configuration_directory.GetDirectories();
                foreach (DirectoryInfo version_number in configuration_directory_files)
                {

                    if (version_number.Name != currentVersion)
                    {
                        update_required = true;
                    }
                }
                if (update_required == true)
                {
                    DiscordLauncher.Properties.Settings.Default.Upgrade();
                    DiscordLauncher.Properties.Settings.Default.Save();
                    foreach (DirectoryInfo version_number in configuration_directory_files)
                    {
                        if (version_number.Name != currentVersion)
                        {
                            version_number.Delete(true);
                        }
                    }
                }
            }
            catch(DirectoryNotFoundException)
            {
                MessageBox.Show("Failed to retrieve old settings", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch(IOException)
            {
                MessageBox.Show("Failed to retrieve old settings", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //FORM RESIZE FROM SETTINGS
        private void form_resize()
        {
            this.Width = DiscordLauncher.Properties.Settings.Default.Width;
            this.Height = DiscordLauncher.Properties.Settings.Default.Height;
        }
        //APP ON LOAD HANDLER
        private void Main_menu_Load(object sender, EventArgs e)
        {
            //SETINGS UPGRADE
            settings_upgrade_handler();
            //PERFORMANCE BUTTONS UPDATE
            performance_settings_gui();
            version_gui_update();
            if (debug_state == false)
            {
                if (DiscordLauncher.Properties.Settings.Default.start_discord_on_load == true)
                    start_discord_on_load();
                if (DiscordLauncher.Properties.Settings.Default.update_listener_thread_auto_startup == true)
                    update_listener_thread_auto_startup();
                if (DiscordLauncher.Properties.Settings.Default.minimize_on_startup == true)
                    minimize_on_startup();
                if (DiscordLauncher.Properties.Settings.Default.discord_listener_thread_auto_startup == true)
                {
                    state = true;
                    start_button_ui.Image = DiscordLauncher.Properties.Resources.stop_img;
                    discord_state_label.Visible = true;
                    state_run_tag.Visible = true;
                    discord_listener_thread_auto_startup();
                }
                else
                {
                    state = false;
                }
            }
        }
        //TOOL STRIP ITEM(CHANGE LOG)
        private void changeLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (change_log_activation_state == false)
            {
                change_log change_log_instance = new change_log();
                change_log_instance.Show();
            }
            else
            {
                Form change_log_reference = Application.OpenForms["change_log"];
                if(change_log_reference!=null)
                change_log_reference.Focus();
            }
        }
        //UPDATE SECTION

        //UPDATE LISTENER ON CLICK
        public void update_listener()
        {
            if (currentlyChecking_update == false)
            {
                currentlyChecking_update = true;
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
                            if (github_latest_version == currentVersion)
                            {
                                    DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "No updates found!", ToolTipIcon.None);
                            }
                            else
                            {
                                DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "Updates found!", ToolTipIcon.None);
                                github_global_version = github_latest_version;
                                safe_ui_updater sync_delegate = new safe_ui_updater(sync_button_ui_update);
                                sync_button_ui.Invoke(sync_delegate);
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "GitHub servers problem. Try again at a later date!", ToolTipIcon.None);
                        }
                        github_response.Close();
                    }
                    else
                    {
                        DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "GitHub servers problem. Try again at a later date!", ToolTipIcon.None);
                    }
                }
                catch(ArgumentOutOfRangeException)
                {
                    DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "GitHub servers problem. Try again at a later date!", ToolTipIcon.None);
                }
            }
            if (currentlyChecking_update == true)
                currentlyChecking_update = false;
        }
        //UPDATE LISTENER STARTUP MODIFIED
        public void update_listener_startup()
        {
            if (currentlyChecking_update == false)
            {
                currentlyChecking_update = true;
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
                            if (github_latest_version == currentVersion)
                            {
                                ;
                            }
                            else
                            {
                                DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "Updates found!", ToolTipIcon.None);
                                github_global_version = github_latest_version;
                                safe_ui_updater sync_delegate = new safe_ui_updater(sync_button_ui_update);
                                sync_button_ui.Invoke(sync_delegate);
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "GitHub servers problem. Try again at a later date!", ToolTipIcon.None);
                        }
                        github_response.Close();
                    }
                    else
                    {
                        DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "GitHub servers problem. Try again at a later date!", ToolTipIcon.None);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    DiscordIconTray.ShowBalloonTip(3000, "Discord Launcher", "GitHub servers problem. Try again at a later date!", ToolTipIcon.None);
                }
            }
            if (currentlyChecking_update == true)
                currentlyChecking_update = false;
        }
        //TOOL STRIP ITEM(CHECK FOR UPDATES)
        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentlyChecking_update == false)
            {
                Thread update_listener_thread = new Thread(update_listener);
                update_listener_thread.Start();
            }
        }
        //TOOL STRIP MENU(SETTINGS)
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (settings_menu_activation_state == false)
            {
                settings_menu settings_menu_instance = new settings_menu();
                settings_menu_instance.Show();
            }
            else
            {
                Form settings_menu_reference =Application.OpenForms["settings_menu"];
                if(settings_menu_reference!=null)
                settings_menu_reference.Focus();
            }
        }
        #region GUI MODULE

        //UPDATE BUTTON
        private void update_button_ui_MouseEnter(object sender, EventArgs e)
        {
            update_button_ui.Image = DiscordLauncher.Properties.Resources.update_img_hover;
        }
        private void update_button_ui_MouseLeave(object sender, EventArgs e)
        {
            update_button_ui.Image = DiscordLauncher.Properties.Resources.update_img;
        }
        private void update_button_ui_MouseDown(object sender, MouseEventArgs e)
        {
            update_button_ui.Image = DiscordLauncher.Properties.Resources.update_img_click;
        }
        //UPDATE BUTTON CLICK HANDLING
        private void update_button_ui_Click(object sender, EventArgs e)
        {
            if (currentlyChecking_update == false)
            {
                Thread update_listener_thread = new Thread(update_listener);
                update_listener_thread.Start();
            }
        }

        //SYNC BUTTON
        private void sync_button_ui_update()
        {
            sync_button_ui.Visible = true;
        }
        private void sync_button_ui_MouseEnter(object sender, EventArgs e)
        {
            sync_button_ui.Image = DiscordLauncher.Properties.Resources.sync_img_hover;
        }

        private void sync_button_ui_MouseLeave(object sender, EventArgs e)
        {
            sync_button_ui.Image = DiscordLauncher.Properties.Resources.sync_img;
        }

        private void sync_button_ui_MouseDown(object sender, MouseEventArgs e)
        {
            sync_button_ui.Image = DiscordLauncher.Properties.Resources.sync_img_click;
        }
        //SYNC BUTTON CLICK HANDLING
        private void sync_button_ui_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/decemyn/Discord-Launcher/archive/v" + github_global_version + ".zip");
        }

        //START BUTTON
        private void start_button_ui_MouseEnter(object sender, EventArgs e)
        {
            if (state == false)
                start_button_ui.Image = DiscordLauncher.Properties.Resources.start_img_hover;
            else
                start_button_ui.Image = DiscordLauncher.Properties.Resources.stop_img_hover;
        }
        private void start_button_ui_MouseLeave(object sender, EventArgs e)
        {
            if (state == false)
                start_button_ui.Image = DiscordLauncher.Properties.Resources.start_img;
            else
                start_button_ui.Image = DiscordLauncher.Properties.Resources.stop_img;
        }
        private void start_button_ui_MouseDown(object sender, MouseEventArgs e)
        {
            if (state == false)
                start_button_ui.Image = DiscordLauncher.Properties.Resources.start_img_click;
            else
                start_button_ui.Image = DiscordLauncher.Properties.Resources.stop_img_click;
        }
        //START BUTTON CLICK HANDLING
        private void start_button_ui_Click(object sender, EventArgs e)
        {
            //DISCORD THREAD
            Thread discord_listener_thread = new Thread(discord_listener);
            //ON BUTTON
            if (state == false)
            {
                state = true;
                start_button_ui.Image = DiscordLauncher.Properties.Resources.stop_img;
                discord_state_label.Visible = true;
                state_run_tag.Visible = true;
                discord_listener_thread.Start();
            }
            //OFF BUTTON
            else if (state == true)
            {
                state = false;
                start_button_ui.Image = DiscordLauncher.Properties.Resources.start_img;
                discord_state_label.Visible = false;
                state_run_tag.Visible = false;
                discord_listener_thread.Abort();
            }
        }

        //VERSION BUTTON
        private void version_MouseEnter(object sender, EventArgs e)
        {
            version.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
        }
        private void version_MouseLeave(object sender, EventArgs e)
        {
            version.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(130)))), ((int)(((byte)(130)))));
        }
        private void version_MouseDown(object sender, MouseEventArgs e)
        {
            version.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
        }
        //VERSION BUTTON CLICK HANDLER
        private void version_MouseClick(object sender, MouseEventArgs e)
        {
            if (change_log_activation_state == false)
            {
                change_log change_log_instance = new change_log();
                change_log_instance.Show();
            }
            else
            {
                Form change_log_reference = Application.OpenForms["change_log"];
                if (change_log_reference != null)
                    change_log_reference.Focus();
            }
        }

        //LOW STATE BUTTON
        private void low_state_button_ui_MouseEnter(object sender, EventArgs e)
        {
            if (performance_state == false)
                low_state_button_ui.Image = DiscordLauncher.Properties.Resources.low_state_img_hover;
            else
                low_state_button_ui.Image = DiscordLauncher.Properties.Resources.low_state_img_select_hover;
        }
        private void low_state_button_ui_MouseLeave(object sender, EventArgs e)
        {
            if (performance_state == false)
                low_state_button_ui.Image = DiscordLauncher.Properties.Resources.low_state_img;
            else
                low_state_button_ui.Image = DiscordLauncher.Properties.Resources.low_state_img_select;
        }
        private void low_state_button_ui_MouseDown(object sender, MouseEventArgs e)
        {
            if (performance_state == false)
                low_state_button_ui.Image = DiscordLauncher.Properties.Resources.low_state_img_click;
            else
                low_state_button_ui.Image = DiscordLauncher.Properties.Resources.low_state_img_select_click;
        }
        //LOW  STATE BUTTON CLICK HANDLING
        private void low_state_button_ui_Click(object sender, EventArgs e)
        {
            performance_state = true;
            realtime_state = false;
            low_state_button_ui.Image = DiscordLauncher.Properties.Resources.low_state_img_select;
            high_state_button_ui.Image = DiscordLauncher.Properties.Resources.high_state_img;
        }

        //HIGH STATE BUTTON
        private void high_state_button_ui_MouseEnter(object sender, EventArgs e)
        {
            if (realtime_state == false)
                high_state_button_ui.Image = DiscordLauncher.Properties.Resources.high_state_img_hover;
            else
                high_state_button_ui.Image = DiscordLauncher.Properties.Resources.high_state_img_select_hover;
        }
        private void high_state_button_ui_MouseLeave(object sender, EventArgs e)
        {
            if (realtime_state == false)
                high_state_button_ui.Image = DiscordLauncher.Properties.Resources.high_state_img;
            else
                high_state_button_ui.Image = DiscordLauncher.Properties.Resources.high_state_img_select;
        }
        private void high_state_button_ui_MouseDown(object sender, MouseEventArgs e)
        {
            if (realtime_state == false)
                high_state_button_ui.Image = DiscordLauncher.Properties.Resources.high_state_img_click;
            else
                high_state_button_ui.Image = DiscordLauncher.Properties.Resources.high_state_img_select_click;
        }
        //HIGH STATE BUTTON CLICK HANDLING
        private void high_state_button_ui_MouseClick(object sender, MouseEventArgs e)
        {
            performance_state = false;
            realtime_state = true;
            high_state_button_ui.Image = DiscordLauncher.Properties.Resources.high_state_img_select;
            low_state_button_ui.Image = DiscordLauncher.Properties.Resources.low_state_img;
        }

        //SETTINGS BUTTON
        private void settings_button_ui_MouseEnter(object sender, EventArgs e)
        {
            settings_button_ui.Image = DiscordLauncher.Properties.Resources.settings_img_over;
        }
        private void settings_button_ui_MouseLeave(object sender, EventArgs e)
        {
            settings_button_ui.Image = DiscordLauncher.Properties.Resources.settings_img;
        }
        private void settings_button_ui_MouseDown(object sender, MouseEventArgs e)
        {
            settings_button_ui.Image = DiscordLauncher.Properties.Resources.settings_img_click;
        }
        //SETTINGS BUTTON CLICK HANDLING
        private void settings_button_ui_Click(object sender, EventArgs e)
        {
            if (settings_menu_activation_state == false)
            {
                settings_menu settings_menu_instance = new settings_menu();
                settings_menu_instance.ShowDialog();
            }
            else
            {
                Form settings_menu_reference = Application.OpenForms["settings_menu"];
                if (settings_menu_reference != null)
                    settings_menu_reference.Focus();
            }
        }

        //TITLE BAR 
        private void title_bar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        //TITLE BAR BUTTONS
        //EXIT
        private void exit_button_Click(object sender, EventArgs e)
        {
            state = false;
            if (DiscordLauncher.Properties.Settings.Default.kill_discord_process_app_close == true)
            {
                try
                {
                    Process[] catch_discord = Process.GetProcessesByName("discord");
                    foreach (Process discord in catch_discord)
                    {
                        discord.Kill();
                    }
                }
                catch (NullReferenceException)
                {
                    ;
                }
                catch (Win32Exception)
                {
                    ;
                }
            }
            Application.Exit();
        }
        //MINIMIZE
        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion
    }
}
