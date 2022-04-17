using System;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using System.Text; 
using System.Security.Permissions;
using Maca134.Arma.DllExport;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web.Script.Serialization;
using Microsoft.Win32;


namespace BEGuid
{
    public class DllEntry
    {
        public static string[] stringSeparators = { ":" };

        //Entry Point
        [ArmaDllExport]
        public static string Invoke(string inputStr, int maxSize) => run(inputStr);

        //Main Function Handle
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static string run(string input)
        {
            string result = "";
            try
            {
                string[] inputSplit = input.Split(stringSeparators, StringSplitOptions.None);
                if(inputSplit.Length > 1) {
                    switch (inputSplit[0])
                    {
                        //'BEGuid' callExtension "get:76561198276956558"
                        case "get":
                            result = A3Functions.GetBEGuid(inputSplit[1]);
                            break;
                        //'BEGuid' callExtension "check:76561198276956558"
                        case "check":
                            result = A3Functions.CheckBEbanned(inputSplit[1]);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (input)
                    { 
                        //'BEGuid' callExtension "test"
                        case "test":
                            result = "1";
                            break;
                        //'BEGuid' callExtension "hwid"
                        case "hwid":
                            result = A3Functions.GetHWID();
                            break;
                        default:
                            break;
                    }
                }
               
            }catch (Exception e){ 
                Environment.Exit(-1);
            }
            return result; 
        } 
    }

    public static class A3Functions
    { 

        //Get HardwareID
        public static string GetHWID()
        {
            using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey registryKey2 = registryKey1.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography"))
                {
                    //No key Exception
                    if (registryKey2 == null) throw new KeyNotFoundException(string.Format("Key Not Found: {0}", (object)"SOFTWARE\\Microsoft\\Cryptography"));

                    //Get the GUID Key
                    object Key = registryKey2.GetValue("MachineGuid");

                    //No Index Exception
                    if (Key == null) throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", (object)"MachineGuid"));

                    //Return The Key As A String
                    return Key.ToString().ToUpper();
                }
            }
        }
        public static string GetBEGuid(string SteamID) => CreateRequestString(SteamID);
        public static string CheckBEbanned(string SteamID) => CheckSteamID(SteamID);

        private static string CheckSteamID(string SteamID)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.ReceiveTimeout = 1000;
            sock.SendTimeout = 1000;
            //All domains lead to the same server, so it should work for Arma3/DayZ as well
            //Arma 2: arma2oa1.battleye.com Port:2324
            //Arma 3: arma31.battleye.com Port:2344
            //DayZ SA: dayz1.battleye.com Port:2354
            string domain = "arma31.battleye.com";
            IPEndPoint endPoint = new IPEndPoint(Dns.GetHostAddresses(domain)[0], 2344);
            byte[] send_buffer = Encoding.ASCII.GetBytes(CreateRequestString(SteamID));
            try
            {
                sock.SendTo(send_buffer, endPoint);
                byte[] receive_buffer = new byte[1024];
                EndPoint endpnt = (EndPoint)endPoint;
                int recv = sock.ReceiveFrom(receive_buffer, ref endpnt);
                string response = Encoding.ASCII.GetString(receive_buffer, 0, recv).Remove(0, 4);
                return string.IsNullOrEmpty(response) ? "Clean" : response;
            }
            catch (Exception) { }
            return "Unknown Error";
        }

        private static bool IsValidSteamID(string SteamIDIn)
        {
            long steamID;
            bool status = long.TryParse(SteamIDIn, out steamID);
            if (status && IsValidSteamID(steamID)) return true;
            return false;
        }
        private static bool IsValidSteamID(long steamID)
        {
            //TODO: Add steamkit dll and verify
            if (steamID.ToString().Length != 17) return false;
            return true;
        }

        private static Random rnd = new Random();
        private static string CreateRequestString(string SteamIDIn)
        {
            if (IsValidSteamID(SteamIDIn))
            {
                byte[] bArray = new byte[5];
                rnd.NextBytes(bArray);
                bArray[4] = 1;
                long steamID;
                bool status = long.TryParse(SteamIDIn, out steamID);

                //main method
                if (status && IsValidSteamID(steamID)) return CreateBEGuid(steamID);

                //fail safe method
                return GetMD5Hash("BE" + GetMD5Hash(SteamIDIn));
            }

            return "";
        }

        /// <summary>
        /// Creates BEGUID from steamID
        /// </summary>
        /// <param name="SteamId"></param>
        /// <returns></returns>
        private static string CreateBEGuid(long SteamId)
        {
            byte[] parts = { 0x42, 0x45, 0, 0, 0, 0, 0, 0, 0, 0 };
            byte counter = 2;

            do
            {
                parts[counter++] = (byte)(SteamId & 0xFF);
            } while ((SteamId >>= 8) > 0);

            return GetMD5Hash(parts);
        }

        /// <summary>
        /// Computes MD5 HASH from string
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string</returns>
        private static string GetMD5Hash(string value) => GetMD5Hash(Encoding.UTF8.GetBytes(value));

        /// <summary>
        /// Computes MD5 HASH from bytes
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string</returns>
        private static string GetMD5Hash(byte[] value)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(value);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

    }
}