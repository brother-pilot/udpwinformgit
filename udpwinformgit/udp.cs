using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using PacketDotNet;
using SharpPcap;
using System.Security.Cryptography;

namespace udpwinformgit
{
    class Udp
    {
        PhysicalAddress physicalAddr;
        byte[] packetBytes;
        int sizePacketDataBytes = 1400;
        long timeDelay = 0;
        private Int32 _elapsedTime;
        private UInt64 _totalSentBytes_old = 0;
        private UInt64 _totalSentDatagrams_old = 0;
        private UInt64 totalSentDatagrams = 0;
        private UInt64 TotalSentBytes { set; get; }

        private UInt64 _totalRecieveBytes_old = 0;
        private UInt64 _totalRecieveDatagrams_old = 0;
        private UInt64 totalRecieveDatagrams = 0;
        private UInt64 TotalRecieveBytes { set; get; }



        public void UdpStart(IPAddress ipAddr, PhysicalAddress physicalAddr)
        {

                this.physicalAddr = physicalAddr;
                CaptureDeviceList deviceList = CaptureDeviceList.Instance;
                //первое устройство для отправки пакетов
                //второе устройство для приема пакетов
                ICaptureDevice device1 = deviceList[0];
                var device2 = CaptureDeviceList.New()[0];
                // регистрируем событие, которое срабатывает, когда пришел новый пакет
                device2.OnPacketArrival += new PacketArrivalEventHandler(Program_OnPacketArrival);
                device2.Open(DeviceMode.Promiscuous, 100);
                //ставим фильтр на входящие пакеты
                device2.Filter = "dst host " + ipAddr + " and udp";
                Console.WriteLine("timer is started");
                Timer timStatistics = new Timer(UpdateStatistics, null, 0, 1000);
                device2.StartCapture();
                Start(device1, ipAddr, physicalAddr);
                Console.WriteLine("Stop sending");
                device2.StopCapture();
                device2.Close();
        }

        private void Start(ICaptureDevice device, IPAddress ipAddr, PhysicalAddress physicalAddr)
        {
                TotalSentBytes = 0;
                device.Open(DeviceMode.Promiscuous, 100);
                MakePacket(ipAddr, physicalAddr);
                ConsoleKeyInfo cki;
                bool flag = true;
                while (flag)
                {
                    device.SendPacket(packetBytes);
                    TotalSentBytes += (ulong)packetBytes.Length;
                    totalSentDatagrams++;
                    if (Console.KeyAvailable)
                    {
                        cki = Console.ReadKey(true);
                        switch (cki.KeyChar)
                        {
                            case 'x':
                                flag = false;
                                break;
                            case 'q':
                                if (sizePacketDataBytes < 1400)
                                {
                                    sizePacketDataBytes += 100;
                                    MakePacket(ipAddr, physicalAddr);
                                }
                                else
                                    Console.WriteLine("Max packet size in program is 1400 byte");
                                break;

                            case 'w':
                                if (sizePacketDataBytes > 100)
                                {
                                    sizePacketDataBytes -= 100;
                                    MakePacket(ipAddr, physicalAddr);
                                }
                                else
                                {
                                    Console.WriteLine("Min packet size in program is 100 byte");
                                }
                                break;
                            case 'a':
                                {
                                    timeDelay += 500;
                                }
                                break;
                            case 's':
                                if (timeDelay > 499)
                                {
                                    timeDelay -= 500;
                                }
                                else
                                    Console.WriteLine("Min time delay");
                                break;
                        }
                    }
                }
        }

        private void UpdateStatistics(object sender)
        {
            UInt64 totalSentBytes_new;
            UInt64 totalSentDatagrams_new;
            UInt64 totalRecieveBytes_new;
            UInt64 totalRecieveDatagrams_new;
            Int32 lostDatagrams = 0;
            totalSentBytes_new = TotalSentBytes;
            totalSentDatagrams_new = totalSentDatagrams;
            totalRecieveBytes_new = TotalRecieveBytes;
            totalRecieveDatagrams_new = totalRecieveDatagrams;
            lostDatagrams = (Int32)(totalSentDatagrams - totalRecieveDatagrams);
            _elapsedTime += (1000 / 1000);
            Console.Clear();
            Console.WriteLine("press key x for stop sending!");
            Console.WriteLine("press key q for max speed using change packet size");
            Console.WriteLine("press key w for min speed using change packet size");
            Console.WriteLine("press key a for max speed using change time delay");
            Console.WriteLine("press key s for min speed using change time delay");
            Console.WriteLine("packet size (byte) is " + sizePacketDataBytes);
            Console.WriteLine("timeDelay (tics) is: " + timeDelay + "\n");
            Console.WriteLine("elapsedTime " + _elapsedTime.ToString());
            Console.WriteLine("totalSentDatagrams per s" + (totalSentDatagrams - _totalSentDatagrams_old).ToString() +
                "     totalRecieveDatagrams per s" + (totalRecieveDatagrams - _totalRecieveDatagrams_old).ToString());
            Console.WriteLine("totalSentBytes " + (totalSentBytes_new).ToString() +
                "     totalRecieveBytes " + (totalRecieveBytes_new).ToString());
            Console.WriteLine("SendRate " + ((double)(totalSentBytes_new - _totalSentBytes_old) * 8 / (1024 * 1024)).ToString("F2") + " MBit/s" +
                "     RecieveRate " + ((double)(totalRecieveBytes_new - _totalRecieveBytes_old) * 8 / (1024 * 1024)).ToString("F2") + " MBit/s");
            Console.WriteLine("SentMegabytes " + ((double)totalSentBytes_new / (1024 * 1024)).ToString("F3") +
                "     RecieveMegabytes " + ((double)totalRecieveBytes_new / (1024 * 1024)).ToString("F3"));
            Console.WriteLine("LostDatagrams " + lostDatagrams.ToString() + "\n");
            _totalSentBytes_old = totalSentBytes_new;
            _totalSentDatagrams_old = totalSentDatagrams_new;
            _totalRecieveBytes_old = totalRecieveBytes_new;
            _totalRecieveDatagrams_old = totalRecieveDatagrams_new;
        }




        void MakePacket(IPAddress ipAddr, PhysicalAddress physicalAddr)
        {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] dataByte = new byte[sizePacketDataBytes];
                rng.GetBytes(dataByte);
                //просоединяем данные к пакету
                ushort udpSourcePort = 123;
                ushort udpDestinationPort = 333;
                var udpPacket = new UdpPacket(udpSourcePort, udpDestinationPort);
                //кодируем данные в байты 
                var ipSourceAddress = System.Net.IPAddress.Parse("192.168.1.1");
                var ipDestinationAddress = ipAddr;
                var ipPacket = new IPv4Packet(ipSourceAddress, ipDestinationAddress);
                var sourceHwAddress = "9A-90-90-90-90-90";
                var ethernetSourceHwAddress = System.Net.NetworkInformation
                    .PhysicalAddress.Parse(sourceHwAddress);
                var destinationHwAddress = physicalAddr;
                var ethernetPacket = new EthernetPacket(ethernetSourceHwAddress,
                    destinationHwAddress, EthernetType.None);
                //встраиваем пакеты в верхний уровень
                udpPacket.PayloadData = dataByte;
                ipPacket.PayloadPacket = udpPacket;
                ethernetPacket.PayloadPacket = ipPacket;
                packetBytes = ethernetPacket.Bytes;
        }

        void Program_OnPacketArrival(object sender, CaptureEventArgs e)
        //перехватывает еще и отправленные пакеты
        {
            // парсинг всего пакета
            Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            if (packet is EthernetPacket)
            {
                EthernetPacket ethernetPacket = (EthernetPacket)packet;
                if (ethernetPacket.DestinationHardwareAddress.Equals(physicalAddr)
                    && ethernetPacket.SourceHardwareAddress.Equals(PhysicalAddress.Parse("909090909090")))
                {
                    var udp = (UdpPacket)packet.Extract<UdpPacket>();
                    if (udp != null)
                    {
                        TotalRecieveBytes += (ulong)packetBytes.Length;
                        totalRecieveDatagrams++;
                    }
                }

            }
        }


    }
}
