using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gomoku
{
    public partial class SettingUI : Form
    {
        public SettingUI()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int ans = 0;
            if(int.TryParse(Text,out ans))
            {
                Settings.SEARCH_DEPTH = ans;
            }

        }
    }
}
