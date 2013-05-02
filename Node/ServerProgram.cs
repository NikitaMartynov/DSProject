using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;


namespace DSProject
{
    static class ServerProgram
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Node node = new Node();

            try
            {
                Application.Run(new NodeForm(node));
            }
            catch (Exception ex) 
            {
                //string str = "sdg";
                MessageBox.Show("sdg: " + ex);
            }
        }
    }
}
