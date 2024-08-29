using System;
using System.Linq;
using PacketDotNet;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConstructingPackets;

    /// <summary>
    /// Example that shows how to construct a packet using packet constructors
    /// to build a tcp/ip ipv4 packet
    /// </summary>
    class MainClass
    {

        public static void Main(string[] args)
        {
            var udpPacket = new UdpPacket(10, 5);
            var ipSourceAddress = System.Net.IPAddress.Parse("192.168.1.1");
            var ipDestinationAddress = System.Net.IPAddress.Parse("192.168.1.0");
            udpPacket.PayloadData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };



            var option = new Rfc1770IPOption(new System.Net.IPAddress[]
            {
                System.Net.IPAddress.Parse("192.168.1.2"),
                System.Net.IPAddress.Parse("192.168.1.3"),
            });
            var ipPacket = new IPv4Packet(ipSourceAddress, ipDestinationAddress,option);

            /*const string sourceHwAddress = "90-90-90-90-90-90";
            var ethernetSourceHwAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(sourceHwAddress);
            const string destinationHwAddress = "80-80-80-80-80-80";
            var ethernetDestinationHwAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(destinationHwAddress);
            // NOTE: using EthernetType.None to illustrate that the ethernet
            //       protocol type is updated based on the packet payload that is
            //       assigned to that particular ethernet packet
            var ethernetPacket = new EthernetPacket(ethernetSourceHwAddress,
                                                    ethernetDestinationHwAddress,
                                                    EthernetType.None);*/

            // Now stitch all of the packets together
            ipPacket.PayloadPacket = udpPacket;
            //ethernetPacket.PayloadPacket = ipPacket;
            var data = ipPacket.Bytes;
            //Console.WriteLine(BitConverter.ToString(ethernetPacket.Bytes));
            var rawPacket = Packet.ParsePacket(LinkLayers.Raw, data);
            var newIPPacket = (IPv4Packet)rawPacket.PayloadPacket;
            Console.ReadLine();
        }
        public static void Main1(string[] args)
        {
            const ushort tcpSourcePort = 123;
            const ushort tcpDestinationPort = 321;
            var tcpPacket = new TcpPacket(tcpSourcePort, tcpDestinationPort);

            var ipSourceAddress = System.Net.IPAddress.Parse("192.168.1.1");
            var ipDestinationAddress = System.Net.IPAddress.Parse("192.168.1.2");
            var ipPacket = new IPv4Packet(ipSourceAddress, ipDestinationAddress);

            const string sourceHwAddress = "90-90-90-90-90-90";
            var ethernetSourceHwAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(sourceHwAddress);
            const string destinationHwAddress = "80-80-80-80-80-80";
            var ethernetDestinationHwAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(destinationHwAddress);
            // NOTE: using EthernetType.None to illustrate that the ethernet
            //       protocol type is updated based on the packet payload that is
            //       assigned to that particular ethernet packet
            var ethernetPacket = new EthernetPacket(ethernetSourceHwAddress,
                                                    ethernetDestinationHwAddress,
                                                    EthernetType.None);

            // Now stitch all of the packets together
            ipPacket.PayloadPacket = tcpPacket;
            ethernetPacket.PayloadPacket = ipPacket;

            // and print out the packet to see that it looks just like we wanted it to
            Console.WriteLine(ethernetPacket.ToString());
        }
    }