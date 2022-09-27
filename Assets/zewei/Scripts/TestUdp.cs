using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEditor;

namespace zewei.Scripts
{
    public class TestUdp : MonoBehaviour
    {

        public IPEndPoint otherComputerConfigIpep;
        public void InitConfigFile(string filename = "otherConfig.txt")
        {
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    string[] arr = File.ReadAllText(filename).Split(':');
                    otherComputerConfigIpep = new IPEndPoint(IPAddress.Parse(arr[0]), int.Parse(arr[1]));
                }
                else
                {
                    otherComputerConfigIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15656);
                    File.WriteAllText(filename, "127.0.0.1:15656");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        

        public void SendToOtherComputer(UdpClient udpClient, string cmd)
        {
            if (otherComputerConfigIpep != null)
            {
                try
                {
                    var bts = Encoding.UTF8.GetBytes(cmd);
                    udpClient.Send(bts, bts.Length, otherComputerConfigIpep);
                }
                catch (Exception e)
                {
                    
                }
            }
        }
        
        
        
    }
}