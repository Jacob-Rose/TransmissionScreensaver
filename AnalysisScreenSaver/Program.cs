using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalysisScreenSaver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            Application.SetCompatibleTextRenderingDefault(true);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
            {
                string firstArgument = args[0].ToLower().Trim();
                string secondArgument = null;

                // Handle cases where arguments are separated by colon.
                // Examples: /c:1234567 or /P:1234567
                if (firstArgument.Length > 2)
                {
                    secondArgument = firstArgument.Substring(3).Trim();
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (args.Length > 1)
                    secondArgument = args[1];

                if (firstArgument == "/c")           // Configuration mode
                {
                    ConfigForm form = new ConfigForm();
                    form.ShowDialog();
                }
                else if (firstArgument == "/p")      // Preview mode
                {
                    return;
                }
                else if (firstArgument == "/s")      // Full-screen mode
                {
                    ShowScreenSaver();
                    Application.Run();
                }
                else    // Undefined argument
                {
                    MessageBox.Show("Sorry, but the command line argument \"" + firstArgument +
                        "\" is not valid.", "ScreenSaver",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else    // No arguments - treat like /c
            {
                // TODO
            }
        }

        static void ShowScreenSaver()
        {
            bool debugMode = true;
            int count = 0;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Primary || !debugMode)
                {
                    System.Threading.Thread.Sleep(2);
                    count++;
                    AnalysisScreenForm screenForm = new AnalysisScreenForm(new Rectangle(0, 0, screen.Bounds.Width, screen.Bounds.Height), count, screen.Primary);
                    screenForm.DesktopLocation = screen.Bounds.Location;
                    screenForm.DesktopBounds = screen.Bounds;
                    screenForm.Activate();
                    screenForm.Show();
                }
            }
            
        }

    }
}
