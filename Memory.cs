using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Csharp_cstest
{
    class Memory
    {
        //Om OpenProcess te gebruiken
        [DllImport("kernel32.dll")]
        private static extern int OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        //Om RPM te gebruiken
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead);

        //Om WPM te gebruiken
        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, int size, ref int lpNumberOfBytesWrite);

        //store base adress voor process
        public static int ProcAdress;
        //store base adress voor module
        public static int ClientBase;
        //store process (voor module)
        private static Process TargetProc;

        public static bool GetProcess(string process)
        {
            //hex getal voor alle toegang
            uint AllAccess = 0x001F0FFF;

            //Loop door alle processen op de pc
            foreach (Process proc in Process.GetProcesses())
            {
                //Als process naam goed is
                if (proc.ProcessName == process)
                {
                    //set processadress
                    ProcAdress = OpenProcess(AllAccess, false, proc.Id);
                    //set target process
                    TargetProc = proc;
                    //process found
                    return true;
                }
            }
            //process not found
            return false;
        }

        public static bool GetModule(string module)
        {
            if (TargetProc != null)
            {
                //Loop door alle modules in het process
                foreach (ProcessModule modules in TargetProc.Modules)
                {
                    //als de module naam client.dll is
                    if (modules.ModuleName == module)
                    {
                        //set Client baseadress
                        ClientBase = (int)(modules.BaseAddress);
                        return true;
                    }
                }
            }
            return false;
        }

        public static int Readint(int adress, int len = 4)
        {
            //buffer
            byte[] buffer = new byte[len];
            //read memory van ProcAdress
            ReadProcessMemory(ProcAdress, adress, buffer, len, 0);
            //convert buffer naar int
            return BitConverter.ToInt32(buffer, 0);
        }

        public static void Writeint(int adress, int value, int len = 4)
        {
            //buffer
            byte[] buffer = new byte[len];
            //convert value to byte array
            buffer = BitConverter.GetBytes(value);
            int lbp = 1;
            //write memory naar adress
            WriteProcessMemory((IntPtr)ProcAdress, (IntPtr)adress, buffer, buffer.Length, ref lbp);
        }

        public static void Writefloat(int adress, float value, int len = 4)
        {
            //buffer
            byte[] buffer = new byte[len];
            //convert value to byte array
            buffer = BitConverter.GetBytes(value);
            int lbp = 1;
            //write memory naar adress
            WriteProcessMemory((IntPtr)ProcAdress, (IntPtr)adress, buffer, buffer.Length, ref lbp);
        }
    }
}
