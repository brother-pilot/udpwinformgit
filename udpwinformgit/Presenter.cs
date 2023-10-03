using IPAddressControlLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace udpwinformgit
{
    public class Presenter
    {
        #region
        private Model mod;
        //	АА3. Презентер взаимодействует с View путем использования специального интерфейса,
        //реализованного представлением
        private IView view;//вместо таскания формы делаем так
        #endregion

        public Presenter(IView View, Model mod)
        {
            this.view = View;
            this.mod = mod;
        }

        public void TextChangedIP(object sender, EventArgs e)
        {
            if (!IsValidIP(view.Ip))
            {
                view.Status = "Please, enter correct IP with all numbers";
            }
            else
            {
                view.Status = "Valid IP address was conformed!";
            }
        }

        public void TextChangedMAC(object sender, EventArgs e)
        {
            if (!IsValidMAC(view.Mac))
            {
                view.Status = "Invalid MAC address specified. One or more invalid characters detected.\n" +
                    "Only 0123456789ABCDEF may be used! \n" +
                    "or Invalid mac address specified. Must be exactly \n" +
                    " 12 valid alphanumeric characters";
            }
            else
            {
                view.Status = "Valid MAC address was conformed!";
            }
        }

        public void ClearAndSave(object sender, EventArgs e)
        {
                if (IsValidIP(view.Ip) && IsValidMAC(view.Mac))
                {
                    var ipAddres = IPAddress.Parse(view.Ip);
                    var macAddr = System.Net.NetworkInformation.PhysicalAddress.Parse(view.Mac);
                    mod.ipAddr.Add(ipAddres);
                    mod.physicalAddr.Add(macAddr);
                    view.Ip = null;
                    view.Mac = null;
                    //ClearIpAndMac();
                    view.Status = "Please, enter new IP and MAC";
                }
                else
                    view.Status = "Please, enter correct IP on patten 000-000-000-000\n" +
                    "Please, enter correct MAC on patten 00-00-00-00-00-00\n"+
                    "Only 0123456789ABCDEF may be used for MAC! \n";

        }

        public bool FinishIPAndMAC()
        {
            if (mod.ipAddr.Count != 0 && mod.physicalAddr.Count != 0)
                return true;
            else
            {
                view.Status = "Enter IP and MAC! Press button Add!";
                return false;
            }
        }

        // <summary>
        /// Method to validate an IP address
        /// using regular expressions. The pattern
        /// being used will validate an ip address
        /// with the range of 1.0.0.0 to 255.255.255.255
        /// </summary>
        public static bool IsValidIP(string addr)
        {
            //create our match pattern
            string pattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            //create our Regular Expression object
            Regex check = new Regex(pattern);
            //boolean variable to hold the status
            bool valid = false;
            //check to make sure an ip address was provided
            if (addr == "")
            {
                //no address provided so return false
                valid = false;
            }
            else
            {
                //address provided so use the IsMatch Method
                //of the Regular Expression object
                valid = check.IsMatch(addr, 0);
            }
            //return the results
            return valid;
        }

        // <summary>
        // Method to validate an MAC address
        // </summary>
        public static bool IsValidMAC(string address)
        {
            address = address.Replace("-", "").Replace(":", "").Replace(" ", "").ToUpper().Trim();
            string validCharacters = "0123456789ABCDEF";
            var count = (from c in address
                         where !validCharacters.Contains(c)
                         select c).Count();
            if (count > 0 || address.Length != 12)
                return false;
            return true;
        }
    }
}
