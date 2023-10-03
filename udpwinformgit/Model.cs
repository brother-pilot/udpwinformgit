using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;


//модель и бизнес-логика
namespace udpwinformgit
{
    public class Model
    {
        //данные которые потом будут передаваться в консольную часть
        public List<IPAddress> ipAddr { get; set; } = new List<IPAddress>();
        public List<PhysicalAddress> physicalAddr { get; set; } = new List<PhysicalAddress>();
    }
}
