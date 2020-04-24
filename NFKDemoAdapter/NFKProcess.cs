using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFKDemoAdapter
{
    public class NFKProcess : IDisposable
    {
        private Process p = null;
        private Stopwatch sw;
        private Thread t;
        public FormIntro Intro;
        public FormAutoClose StubForm;
        private LowLevelKeyboardHook kbh = null;
        System.Threading.Thread KeyThread;

        public Process GetProcess()
        {
            return p;
        }

        public bool GameRunning
        {
            get
            {
                return _gameRunning;
            }
            private set
            {
                _gameRunning = value;
                if (value == false)
                {
                    Dispose();
                }
            }
        }
        private bool _gameRunning = false;

        public NFKProcess()
        {
            sw = new Stopwatch();
            Intro = new FormIntro();
            // this form is for escape hook (application form must be running)
            StubForm = new FormAutoClose()
            {
                StartPosition = FormStartPosition.Manual,
                Left = -100,
                Top = -100,
                Height = 0,
                Width = 0
            };
            StubForm.Shown += (object _sender, EventArgs _e) => {
                StubForm.Hide();
            };

            // close nfk by escape
            kbh = new LowLevelKeyboardHook();
            kbh.OnKeyPressed += (object _sender, System.Windows.Forms.Keys _e) =>
            {
                if (_e == Keys.Escape)
                    Dispose();
            };
            kbh.HookKeyboard();
        }

        public void Run(string demoFile)
        {
            if (String.IsNullOrEmpty(NFKHelper.DemoFile))
            {
                return;
            }

            p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                FileName = NFKHelper.GameExePathTemp,
                Arguments = NFKHelper.GetGameStartArgs(demoFile),
                WorkingDirectory = NFKHelper.GetGameWorkingDir(),
                CreateNoWindow = true
            };

            p.Start();
            sw.Start();
            GameRunning = true;
        }


        /// <summary>
        /// Wait process until exit, or exit it by force if user went to game menu
        /// </summary>
        [STAThread]
        public void WatchForExit()
        {
            t = new Thread(watcher);
            t.Start();
        }

        /// <summary>
        /// Exit program when game process is closed or a demo is finished (game state in a menu)
        /// </summary>
        private void watcher()
        {
            var fullLoaded = false;

            int delay = 10;
            var pm = new ProcessMemory();
            try
            {
                pm = ProcessMemory.ForProcess(p);
            }
            catch { }

            bool windowOpened = false;
            // FIXME: this code inside while can throw exception if abort button was clicked in Form2 (cause process killed)
            while (p != null && !p.HasExited)
            {
                // loading screen is opened
                if (!windowOpened)
                {
                    windowOpened = true;
                    /*
                    var wnd = WinApi.FindWindow("Tloadi", "Need For Kill Console");
                    //var mwnd = WinApi.FindWindowEx(wnd, IntPtr.Zero, "TMemo", null);
                    //var wnd2 = WinApi.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Need For Kill Console");
                    //Debug.WriteLine("wnd {0}", wnd);
                    if (wnd != IntPtr.Zero)
                    {
                        //var windows = WinApi.GetProcessWindows(p.Id);
                        //if (windows.Length > 0)
                        //{
                        //    foreach (var w in windows)
                        //        WinApi.HideWindow(w);
                        //
                        //    windowOpened = true;
                        //}
                        WinApi.HideWindow(wnd);
                        //WinApi.HideWindow(wnd2);
                        //WinApi.HideWindow(mwnd);
                        windowOpened = true;
                    }
                    */
                }

                bool inMenu = false, gameLoaded = false;
                try
                {
                    // read data from the game memory
                    inMenu = pm.AtOffset(NFKHelper.IsMenuOffset).AsByte() == 1;
                    gameLoaded = pm.AtOffset(NFKHelper.IsGameLoadedOffset).AsByte() == 1;
                    //System.Diagnostics.Debug.WriteLine("{0} | {1} | {2} / {3}", p.Handle, p.MainWindowHandle, inMenu, gameLoaded);
                }
                catch { }

                if (!fullLoaded && !inMenu && gameLoaded)
                {
                    // close intro
                    Intro.CloseFlag = true;

                    fullLoaded = true;
                    sw.Stop();
                    // update load time
                    Properties.Settings.Default.GameLoadTime = (int)sw.ElapsedMilliseconds;
                    Properties.Settings.Default.Save();
                }

                // exit only if in menu + game loaded
                if (fullLoaded && inMenu /* && totalDelay > maxDelayTime*/)
                {
                    GameRunning = false;
                    break;
                }
                // check NFK memory every delay milliseconds
                Thread.Sleep(delay);
            }
            GameRunning = false;
        }



        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                try
                {
                    if (kbh != null)
                        kbh.UnHookKeyboard();

                    if (p != null)
                    {
                        p.Kill();
                        p.Close();
                        p = null;
                    }
                    if (Intro != null)
                    {
                        // close intro
                        Intro.CloseFlag = true;
                    }
                    StubForm.CloseFlag = true;
                    StubForm = null;

                    // delete temp exe
                    // FIXME: it may be not deleted if there are two instances of the program are running
                    File.Delete(Path.Combine(NFKHelper.GetGameWorkingDir(), NFKHelper.GAME_EXE_TEMP));
                }
                catch(Exception e) { }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~NFKProcess()
        {
          // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
          Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
