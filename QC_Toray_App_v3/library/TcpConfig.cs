using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QC_Toray_App_v3.library
{
    public static class TcpConfig
    {
        public static string TcpServerIp
        {
            get => Properties.Settings.Default.ServerIp;
            set 
            { 
                Properties.Settings.Default.ServerIp = value;
                Properties.Settings.Default.Save();
            }
        }

        public static int TcpServerPort
        {
            get => Properties.Settings.Default.ServerPort;
            set
            {
                Properties.Settings.Default.ServerPort = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
