using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundCloudBot
{
    class Program
    {
        public static MainForm GUI = new MainForm();

        static void Main(string[] args)
        {
            GUI.log("Application started successfully");
            Application.EnableVisualStyles();
            Application.Run(GUI);
        }
    }
}
