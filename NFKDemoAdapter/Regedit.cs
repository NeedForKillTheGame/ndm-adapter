using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFKDemoAdapter
{
    /*
    
    Windows Registry Editor Version 5.00

    ; Infamous capabilities:

    [HKEY_LOCAL_MACHINE\SOFTWARE\MyApp\Capabilities]
    "ApplicationDescription"="MyApp"
    "ApplicationIcon"="C:\\MyApp\\MyApp.exe,0"
    "ApplicationName"="MyApp"

    [HKEY_LOCAL_MACHINE\SOFTWARE\MyApp\Capabilities\FileAssociations]
    ".htm"="MyAppURL"
    ".html"="MyAppURL"
    ".shtml"="MyAppURL"
    ".xht"="MyAppURL"
    ".xhtml"="MyAppURL"

    [HKEY_LOCAL_MACHINE\SOFTWARE\MyApp\Capabilities\URLAssociations]
    "ftp"="MyAppURL"
    "http"="MyAppURL"
    "https"="MyAppURL"

    ; Register to Default Programs

    [HKEY_LOCAL_MACHINE\SOFTWARE\RegisteredApplications]
    "MyApp"="Software\\MyApp\\Capabilities"

    ; MyAppURL HANDLER:

    [HKEY_LOCAL_MACHINE\Software\Classes\MyAppURL]
    @="MyApp Document"
    "FriendlyTypeName"="MyApp Document"

    [HKEY_LOCAL_MACHINE\Software\Classes\MyAppURL\shell]

    [HKEY_LOCAL_MACHINE\Software\Classes\MyAppURL\shell\open]

    [HKEY_LOCAL_MACHINE\Software\Classes\MyAppURL\shell\open\command]
    @="\"C:\\MyApp\\MyApp.exe\" \"%1\""

    */

    public class Regedit
    {
        public Regedit()
        {

        }

        public bool HasLinkHandler()
        {
            using (var key = Registry.CurrentUser.OpenSubKey("Software\\Classes\\nfkdemo", RegistryKeyPermissionCheck.ReadSubTree))
            {
                return key != null;
            }
        }

        public bool HasFileHandler()
        {
            using (var key = Registry.CurrentUser.OpenSubKey("Software\\Classes\\.ndm", RegistryKeyPermissionCheck.ReadSubTree))
            {
                if (key != null)
                {
                    using (var key2 = Registry.CurrentUser.OpenSubKey("Software\\Classes\\ndmadapter\\shell\\open\\command", RegistryKeyPermissionCheck.ReadSubTree))
                    {
                        if (key2 != null)
                        {
                            return key2 != null;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/aa767914(v=vs.85)?redirectedfrom=MSDN
        /// </summary>
        public bool RegisterHandler(Handler handler)
        {
            try
            {
                // save temp reg file
                var tmpFileName = "ndmadapter.reg";
                var regData = handler == Handler.Link
                    ? Properties.Resources.link_reg
                    : Properties.Resources.file_reg;
                regData = regData.Replace("{PROGRAM_EXE_PATH}", NFKHelper.ProgramExePath.Replace("\\", "\\\\"));
                var tmpFilePath = Path.Combine(Path.GetTempPath(), tmpFileName);
                File.WriteAllText(tmpFilePath, regData);

                // exec reg file as administrator
                var p = Process.Start(new ProcessStartInfo()
                {
                    FileName = "reg.exe",
                    Arguments = "IMPORT \"" + tmpFilePath + "\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
                p.WaitForExit();

                File.Delete(tmpFilePath);
            }
            catch { }
            return handler == Handler.Link
                ? HasLinkHandler()
                : HasFileHandler();
        }

        public bool UnregisterHandler(Handler handler)
        {
            try
            {
                if (handler == Handler.Link)
                {
                    var p = Process.Start(new ProcessStartInfo()
                    {
                        FileName = "reg.exe",
                        Arguments = "DELETE HKEY_CURRENT_USER\\Software\\Classes\\nfkdemo /f",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                    p.WaitForExit();
                }
                else
                {
                    var p1 = Process.Start(new ProcessStartInfo()
                    {
                        FileName = "reg.exe",
                        Arguments = "DELETE HKEY_CURRENT_USER\\Software\\Classes\\.ndm /f",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                    var p2 = Process.Start(new ProcessStartInfo()
                    {
                        FileName = "reg.exe",
                        Arguments = "DELETE HKEY_CURRENT_USER\\Software\\Classes\\ndmadapter\\shell\\open\\command /f",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                    p1.WaitForExit();
                    p2.WaitForExit();
                }

                //[HKEY_CURRENT_USER\Software\Classes\.ndm]
                //[HKEY_CURRENT_USER\Software\Classes\ndmadapter\shell\open\command]
                //

            }
            catch { }
            return !(handler == Handler.Link
                ? HasLinkHandler()
                : HasFileHandler());
        }

        public enum Handler
        {
            Link,
            File
        }

        public bool RegisterFileHandler()
        {
            try
            {

            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
