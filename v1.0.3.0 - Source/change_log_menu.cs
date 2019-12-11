//CHANGE LOG MENU FORM CODE
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
    public partial class change_log : Form
    {
        public change_log()
        {
            InitializeComponent();
        }

        private void change_log_Load(object sender, EventArgs e)
        {
            Main_menu.change_log_activation_state = true;
        }

        private void change_log_FormClosed(object sender, FormClosedEventArgs e)
        {
            Main_menu.change_log_activation_state = false;
        }
    }
}
