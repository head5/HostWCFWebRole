using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Management.Instrumentation;
//using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace WCFWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : ISystem
    {

        OperatingSystem OS;
        DriveInfo[] driveinfo = DriveInfo.GetDrives();

        /// <summary>
        /// This Method Will return the OS Details of the System.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Systemdata GetSystemInfo(int value)
        {
            OS = Environment.OSVersion;
            Systemdata sysdata = new Systemdata();
            // Fill Version Details
            Version vs = OS.Version;
            sysdata.VersionInfo.Version_String = OS.VersionString;

            sysdata.VersionInfo.Major_Version = vs.Major;
            sysdata.VersionInfo.Minor_Version = vs.Minor;
            sysdata.VersionInfo.Build_Version = vs.Build;

            //Fill Computer Details
            sysdata.ComputerInfo.ComputerName = SystemInformation.ComputerName;
            sysdata.ComputerInfo.DomainName = SystemInformation.UserDomainName;
            sysdata.ComputerInfo.UserName = SystemInformation.UserName;
            //sysdata.ComputerInfo.IpAddress=Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
            //  sysdata.ComputerInfo.IpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString(); 

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addr = host.AddressList;
            foreach (IPAddress ipaddr in addr)
            {
                if (ipaddr.AddressFamily == AddressFamily.InterNetwork)
                    sysdata.ComputerInfo.IpAddress = ipaddr.ToString();
            }

            //return host;
            //return host.AddressList[1].ToString();

            //Calculate Ram Usable
            ulong ramInByte = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            double ram_UsableInGB = (ramInByte / 1024d) / (1024 * 1024);
            sysdata.ComputerInfo.Ram_Usable = ram_UsableInGB;

            // Get All Drive Details..                 
            Drive drive;
            List<Drive> driveList = new List<Drive>();
            for (int i = 0; i < driveinfo.Length; i++)
            {
                if (driveinfo[i].IsReady)
                {
                    drive = new Drive();
                    double totalSpace = (driveinfo[i].TotalSize / 1024d) / (1024 * 1024);
                    double freeSpace = (driveinfo[i].TotalFreeSpace / 1024d) / (1024 * 1204);
                    double usedSpace = totalSpace - freeSpace;

                    drive.DriveName = driveinfo[i].Name;
                    drive.TotalSpace = String.Format("{0:N2} GB", totalSpace);
                    drive.UsedSpace = String.Format("{0:N2} GB", usedSpace);
                    drive.FreeSpace = String.Format("{0:N2} GB", freeSpace);
                    driveList.Add(drive);
                }
            }

            sysdata.DriveInfo = driveList;   // Add all drive info to the list

            // Get 1 Process which is consuming Memory.
            Process[] process = Process.GetProcesses();
            var query = Process.GetProcesses().OrderByDescending(p => p.PrivateMemorySize64).Take(1).ToList();
            foreach (var item in query)
            {
                sysdata.ProcessConsumeMemory.Process_Name = item.ProcessName;
                sysdata.ProcessConsumeMemory.Process_ID = item.Id;
            }
            return sysdata;
            //  return null;
        }

       
    }
}
