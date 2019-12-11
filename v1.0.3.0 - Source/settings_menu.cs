//SETTINGS MENU FORM CODE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using Microsoft.Win32;
namespace PriorityTool
{
    public partial class settings_menu : Form
    {
        static public bool helper_menu_activation_state=false;
        public settings_menu()
        {
            InitializeComponent();
        }
        //SETTINGS READER UPDATE GUI
        private void read_settings_from_file_gui()
        {
            if (DiscordLauncher.Properties.Settings.Default.low_state == true)
            {
                performance_state_button.BackColor = System.Drawing.Color.Green;
                max_state_button.BackColor = System.Drawing.Color.Gray;
            }
            else
            {
                performance_state_button.BackColor = System.Drawing.Color.Gray;
                max_state_button.BackColor = System.Drawing.Color.Green;
            }
            if(DiscordLauncher.Properties.Settings.Default.start_discord_on_load==true)
            {
                first_setting_on.BackColor = System.Drawing.Color.Green;
                first_setting_off.BackColor = System.Drawing.Color.Gray;
            }
            else
            {
                first_setting_on.BackColor = System.Drawing.Color.Gray;
                first_setting_off.BackColor = System.Drawing.Color.Green;
            }
            if (DiscordLauncher.Properties.Settings.Default.minimize_on_startup == true)
            {
                second_setting_on.BackColor = System.Drawing.Color.Green;
                second_setting_off.BackColor = System.Drawing.Color.Gray;
            }
            else
            {
                second_setting_on.BackColor = System.Drawing.Color.Gray;
                second_setting_off.BackColor = System.Drawing.Color.Green;
            }
            if (DiscordLauncher.Properties.Settings.Default.update_listener_thread_auto_startup == true)
            {
                third_setting_on.BackColor = System.Drawing.Color.Green;
                third_setting_off.BackColor = System.Drawing.Color.Gray;
            }
            else
            {
                third_setting_on.BackColor = System.Drawing.Color.Gray;
                third_setting_off.BackColor = System.Drawing.Color.Green;
            }
            if (DiscordLauncher.Properties.Settings.Default.disable_running_in_background_message == true)
            {
                fourth_setting_on.BackColor = System.Drawing.Color.Green;
                fourth_setting_off.BackColor = System.Drawing.Color.Gray;
            }
            else
            {
                fourth_setting_on.BackColor = System.Drawing.Color.Gray;
                fourth_setting_off.BackColor = System.Drawing.Color.Green;
            }
            if (DiscordLauncher.Properties.Settings.Default.kill_discord_process_app_close == true)
            {
                fifth_setting_on.BackColor = System.Drawing.Color.Green;
                fifth_setting_off.BackColor = System.Drawing.Color.Gray;
            }
            else
            {
                fifth_setting_on.BackColor = System.Drawing.Color.Gray;
                fifth_setting_off.BackColor = System.Drawing.Color.Green;
            }
            if (DiscordLauncher.Properties.Settings.Default.discord_listener_thread_auto_startup == true)
            {
                sixth_setting_on.BackColor = System.Drawing.Color.Green;
                sixth_setting_off.BackColor = System.Drawing.Color.Gray;
            }
            else
            {
                sixth_setting_on.BackColor = System.Drawing.Color.Gray;
                sixth_setting_off.BackColor = System.Drawing.Color.Green;
            }
        }
        private void settings_menu_Load(object sender, EventArgs e)
        {
            Main_menu.settings_menu_activation_state = true;
            read_settings_from_file_gui();
        }

        private void settings_menu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form help_menu_reference = Application.OpenForms["help_settings"];
            try
            {
                help_menu_reference.Close();
            }
            catch(NullReferenceException)
            {

            }
            Main_menu.settings_menu_activation_state = false;
        }

        private void first_setting_off_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.start_discord_on_load = false;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void first_setting_on_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.start_discord_on_load = true;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void second_setting_off_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.minimize_on_startup = false;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void second_setting_on_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.minimize_on_startup = true;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void third_setting_off_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.update_listener_thread_auto_startup = false;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void third_setting_on_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.update_listener_thread_auto_startup = true;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void fourth_setting_off_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.disable_running_in_background_message = false;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void fourth_setting_on_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.disable_running_in_background_message = true;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void fifth_setting_off_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.kill_discord_process_app_close = false;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void fifth_setting_on_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.kill_discord_process_app_close = true;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void sixth_setting_off_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.discord_listener_thread_auto_startup = false;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void sixth_setting_on_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.discord_listener_thread_auto_startup = true;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void reset_default_Click(object sender, EventArgs e)
        {
            string temp_discord_path = DiscordLauncher.Properties.Settings.Default.discord_app_path;
            DiscordLauncher.Properties.Settings.Default.Reset();
            if (temp_discord_path != string.Empty)
            {
               // DiscordLauncher.Properties.Settings.Default.discord_app_path = temp_discord_path;
               // DiscordLauncher.Properties.Settings.Default.Save();
            }
            read_settings_from_file_gui();
        }

        private void performance_state_button_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.high_state = false;
            DiscordLauncher.Properties.Settings.Default.low_state = true;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void max_state_button_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.high_state = true;
            DiscordLauncher.Properties.Settings.Default.low_state = false;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void one_click_Click(object sender, EventArgs e)
        {
            DiscordLauncher.Properties.Settings.Default.start_discord_on_load = true;
            DiscordLauncher.Properties.Settings.Default.minimize_on_startup = true;
            DiscordLauncher.Properties.Settings.Default.disable_running_in_background_message = true;
            DiscordLauncher.Properties.Settings.Default.kill_discord_process_app_close = true;
            DiscordLauncher.Properties.Settings.Default.discord_listener_thread_auto_startup = true;
            DiscordLauncher.Properties.Settings.Default.Save();
            read_settings_from_file_gui();
        }

        private void help_button_Click(object sender, EventArgs e)
        {
            if (helper_menu_activation_state == false)
            {
                help_settings help_settings_instance = new help_settings();
                help_settings_instance.Show();
            }
            else
            {
                Form help_menu_reference = Application.OpenForms["help_settings"];
                if (help_menu_reference != null)
                    help_menu_reference.Focus();
            }
        }
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
        private void path_selector_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Path resets when pressing \"Reset to default\" button!", "Executable selector for the Discord application", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult input_result;
            input_result=MessageBox.Show("Automatically detect path?", "Executable selector for the Discord application", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if(input_result.ToString()=="Yes")
            {
                string discord_path = registry_pull_discord_path();
                if(discord_path!=string.Empty)
                {
                     DiscordLauncher.Properties.Settings.Default.discord_app_path = discord_path;
                     DiscordLauncher.Properties.Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("Discord Launcher couldn't detect Discord's application executable! Please select it manually!", "Executable selector for the Discord application", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if(input_result.ToString() == "No")
            {
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
    }
}
