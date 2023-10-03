using System;
using System.Windows.Forms;

//winform-console application
namespace udpwinformgit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var mod = new Model();
            var form = new Form1(mod);
            Application.Run(form);
            Console.WriteLine("stop!!!");
            Udp udp = new Udp();
            for (int i = 0; i < mod.ipAddr.Count; i++)
            {
                udp.UdpStart(mod.ipAddr[i], mod.physicalAddr[i]);
            }
        }
    }

    //TODO make only winform application 
}
