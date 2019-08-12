using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Akiba.Core
{
    static class Utilities
    {

        public const string GameExecutableName = "AkibaUU.exe";
        public const string ConfigExecutableName = "AkibaUU_Config.exe";
        public const string BackupConfigExecutableName = "AkibaUU_Config.Original.exe";
        public const string ConfigTriggerSwitch = "RediationNext_Config";
        public const string MonitorTriggerSwitch = "GameMonitor";

        public static bool AlterGameFramerate()
        {
            Process gameProcess = GetGameProcess();

            if (gameProcess == null)
            {
                return false;
            }

            int memoryAddress = GetFramerateMemoryAddress();

            if (memoryAddress == 0x00)
            {
                return false;
            }

            NativeMethods.WriteToProcessMemory(gameProcess, memoryAddress, Program.Config.FramesPerSecond);

            return true;
        }

        public static Process GetGameProcess()
        {
            return Process.GetProcessesByName(
                Path.GetFileNameWithoutExtension(GameExecutableName)
            ).FirstOrDefault();
        }

        public static void LaunchGame()
        {
            Process.Start(GameExecutableName);
        }

        private static int GetFramerateMemoryAddress()
        {
            Version systemVersion = Environment.OSVersion.Version;

            if (systemVersion.Major == 10)
            {
                if (GetWindowsReleaseId() >= 1809)//Keep working on 1903
                {
                    // Windows 10 (October 2018 Update)
                    return 0x0019FDEC;
                }

                // Windows 10
                return 0x0019FDFC;
            }
            else if (systemVersion.Major == 6 && systemVersion.Minor == 3)
            {
                // Windows 8.1
                return 0x0018FDFC;
            }
            else if (systemVersion.Major == 6 && systemVersion.Minor == 2)
            {
                // Windows 8
                return 0x0018FE00;
            }
            else if (systemVersion.Major == 6 && systemVersion.Minor == 1)
            {
                // Windows 7
                return 0x0018FE04;
            }
            else if (systemVersion.Major == 6 && systemVersion.Minor == 0)
            {
                // Windows Vista
                return 0x0017FE04;
            }

            return 0x00;
        }

        private static uint GetWindowsReleaseId()
        {
            return Convert.ToUInt32(
                (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "0")
            );
        }
    }
}
