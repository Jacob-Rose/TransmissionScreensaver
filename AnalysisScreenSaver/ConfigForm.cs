using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisScreenSaver
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
            FormClosed += ConfigForm_FormClosed;
        }

        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\TransmissionScreenSaver");
            if (checkBox1.Checked)
            {
                
                key.SetValue("useGenericFonts", true);
            }
            else
            {
                key.SetValue("useGenericFonts", false);
            }
        }
    }
}
