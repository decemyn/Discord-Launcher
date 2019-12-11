//HELP MENU FORM CODE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriorityTool
{
    public partial class help_settings : Form
    {
        public help_settings()
        {
            InitializeComponent();
        }

        private void help_settings_Load(object sender, EventArgs e)
        {
            settings_menu.helper_menu_activation_state = true;
        }

        private void help_settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            settings_menu.helper_menu_activation_state = false;
        }
    }
}
