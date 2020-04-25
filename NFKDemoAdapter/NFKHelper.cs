using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFKDemoAdapter
{
    public static class NFKHelper
    {
        public const string GAME_EXE_TEMP = "NFKDEMO.exe";
        public const string GAME_CONFIG = "ndmadapter.cfg";
        public static Process P;
        /// <summary>
        /// Default game load time
        /// After first start it updates in program settings
        /// </summary>
        public const float GAME_LOAD_TIME_DEFAULT = 12000; // milliseconds
        /// <summary>
        /// Video intro duration
        /// </summary>
        public const float INTRO_TIME = 4000; // milliseconds

        public static int IsMenuOffset = 0x005547CC;
        public static int IsGameLoadedOffset = 0x0054BF2C;

        public const int GAME_WIDTH = 1280;
        public const int GAME_HEIGHT = 720;

        /// <summary>
        /// Path to demo file, it must be set after the file download
        /// </summary>
        public static string DemoFile { get; set; }

        /// <summary>
        /// Must be set before execute other functions
        /// </summary>
        public static string GameExePath
        {
            get
            {
                // TODO: load from registry if empty
                if (string.IsNullOrEmpty(_gameExePath))
                {
                    _gameExePath = Properties.Settings.Default.GameExePath;
                }
                // if still empty
                if (string.IsNullOrEmpty(_gameExePath) || !File.Exists(_gameExePath))
                {
                    Common.ShowError("Error", "Incorrect path to NFK\n" + _gameExePath);
                    // start the program without parameters
                    Process.Start(ProgramExePath);

                    // exit current instance
                    Environment.Exit(1);
                }
                return _gameExePath;
            }
        }
        private static string _gameExePath = string.Empty;

        /// <summary>
        /// NFK.exe in temporary path
        /// Put it from resources if not exists
        /// </summary>
        public static string GameExePathTemp
        {
            get
            {
                var gameExe = Path.Combine(GetGameWorkingDir(), GAME_EXE_TEMP);
                // if not exists
                if (!File.Exists(gameExe))
                {
                    try
                    {

                        File.WriteAllBytes(gameExe, NFKDemoAdapter.Properties.Resources.NFK);
                        var ch = new DirectoryInfo(gameExe);
                        ch.Attributes = FileAttributes.Hidden;
                    }
                    catch (Exception e)
                    {
                        Common.ShowError("Error on NFK.exe extraction", e.Message);
                        Environment.Exit(1);
                    }
                }
                return gameExe;
            }
        }

        /// <summary>
        /// Check if demo file is valid NFK demo
        /// </summary>
        /// <param name="demoFile"></param>
        /// <returns></returns>
        public static bool IsValidDemoFile(string demoFile)
        {
            byte[] buf = new byte[7];
            if (!File.Exists(demoFile))
                return false;

            try
            {
                using (var f = File.OpenRead(demoFile))
                {
                    f.Read(buf, 0, 7);
                }
            }
            catch
            {
                return false;
            }
 
            if (Encoding.ASCII.GetString(buf) != "NFKDEMO")
                return false;

            return true;
        }

        /// <summary>
        /// Return current executable full path
        /// </summary>
        public static string ProgramExePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            }
        }

        /// <summary>
        /// Return NFK start arguments
        /// </summary>
        /// <param name="demoFile"></param>
        /// <returns></returns>
        public static string GetGameStartArgs(string demoFile)
        {
            //demoFile = Path.GetFileNameWithoutExtension(demoFile);
            var configFile = Path.GetFileNameWithoutExtension(GAME_CONFIG);
            return string.Format("+gowindow +dontsavecfg +exec {0} +demo \"{1}\"", configFile, demoFile);
        }

        /// <summary>
        /// Game root dir
        /// </summary>
        /// <returns>c:/games/nfk</returns>
        public static string GetGameWorkingDir()
        {
            return Path.GetDirectoryName(GameExePath);
        }

        /// <summary>
        /// Game basenfk dir
        /// </summary>
        /// <returns>c:/games/nfk/basenfk</returns>
        public static string GetBasenfkPath()
        {
            return Path.Combine(GetGameWorkingDir(), "basenfk");
        }

        /// <summary>
        /// Return file path for config which would be loaded automatically with the game
        /// Create it in c:/games/nfk/basenfk with predefined contents if not exists
        /// </summary>
        /// <returns>c:/games/nfk/basenfk/ndmadapter.cfg</returns>
        public static string GetOrCreateConfigFile()
        {
            var filePath = Path.Combine(GetBasenfkPath(), GAME_CONFIG);
            if (!File.Exists(filePath))
            {
                try
                {
                    var contents = Properties.Resources.ndmadapter;
                    int new_width, new_height;
                    calcResizedWindow(Screen.PrimaryScreen.Bounds.Width,
                        Screen.PrimaryScreen.Bounds.Height, 
                        out new_width, 
                        out new_height);
                    contents = contents.Replace("r_mode 1280 720", string.Format("r_mode {0} {1}", new_width, new_height));
                    File.WriteAllText(filePath, contents);

                    setSoftwareSound();
                    Task.Run(() => createMp3list());
                }
                catch (Exception e)
                {
                    Common.ShowError("Error on extraction", e.Message);
                    Environment.Exit(1);
                }
            }
            return filePath;
        }

        /// <summary>
        /// Replace hardware(2) to sound software(1) if set
        /// Hardware sound disappears on late Windows versions. Software works better.
        /// </summary>
        private static void setSoftwareSound()
        {
            var nfkConfigIni = Path.Combine(GetBasenfkPath(), "nfksetup.ini");
            var contents = File.ReadAllText(nfkConfigIni);
            contents = contents.Replace("soundtype=1", "soundtype=2");
            File.WriteAllText(nfkConfigIni, contents);
        }

        /// <summary>
        /// Download and place wind.mpw into music directory to play via "mp3play" command
        /// </summary>
        private static void createMp3list()
        {
            var musicPath = Path.Combine(GetBasenfkPath(), "music");
            var mp3File = Path.Combine(musicPath, "wind.mp3");
            var mp3listFile = Path.Combine(musicPath, "mp3list.dat");

            try
            {
                if (!Directory.Exists(musicPath))
                    Directory.CreateDirectory(musicPath);

                // if playlist is not empty then backup
                if (File.Exists(mp3listFile) && (new FileInfo(mp3listFile)).Length > 0)
                    File.Copy(mp3listFile, mp3listFile + ".bkp");

                // write new mp3list
                var contents = "music/wind.mp3\n";
                File.WriteAllText(mp3listFile, contents);

                // if music file not exists then download
                if (!File.Exists(mp3File) || (new FileInfo(mp3File)).Length == 0)
                {
                    using (var wc = new WebClient())
                    {
                        wc.DownloadFile("https://nfk.harpywar.com/download/music/wind.mp3", mp3File);
                    }
                }
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// Get new width and height proportional to original but less
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static void calcResizedWindow(int width, int height, out int new_width, out int new_height)
        {
            var screen_ratio = decimal.Divide(width, height);
            new_width = width > height
                ? (width > GAME_WIDTH ? GAME_WIDTH : width)
                : (height > GAME_HEIGHT ? GAME_HEIGHT : height);
            new_height = width > height
                ? (int)Math.Ceiling(new_width / screen_ratio)
                : (int)Math.Ceiling(new_width * screen_ratio);
        }


        /// <summary>
        /// Return demos download path
        /// Create the directory if not exists
        /// </summary>
        /// <returns>c:/games/nfk/basenfk/demos/download</returns>
        public static string GetOrCreateDemoDownloadDir()
        {
            var path = Path.Combine(GetBasenfkPath(), "demos", "download");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }





    }
}
