﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NFKDemoAdapter.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NFKDemoAdapter.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Windows Registry Editor Version 5.00
        ///
        ///[HKEY_CURRENT_USER\Software\Classes\ndmadapter\shell\open\command]
        ///@=&quot;{PROGRAM_EXE_PATH} \&quot;%1\&quot;&quot;
        ///
        ///[HKEY_CURRENT_USER\Software\Classes\.ndm]
        ///@=&quot;ndmadapter&quot;.
        /// </summary>
        internal static string file_reg {
            get {
                return ResourceManager.GetString("file_reg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] intro {
            get {
                object obj = ResourceManager.GetObject("intro", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Windows Registry Editor Version 5.00
        ///
        ///[HKEY_CURRENT_USER\Software\Classes\nfkdemo]
        ///&quot;URL Protocol&quot;=&quot;&quot;
        ///
        ///[HKEY_CURRENT_USER\Software\Classes\nfkdemo\shell\open\command]
        ///@=&quot;\&quot;{PROGRAM_EXE_PATH}\&quot; \&quot;%1\&quot;&quot;
        ///
        ///.
        /// </summary>
        internal static string link_reg {
            get {
                return ResourceManager.GetString("link_reg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // display size (init value with aspect ratio)
        ///r_mode 1280 720
        ///zoomwindow
        ///
        ///// game speed (0-40), default 20
        ///speeddemo 25
        ///
        ///// start play music from a playlist basenfk/music/mp3list.dat
        ///mp3volume 20
        ///mp3play
        ///
        ///// decrease game volume
        ///volume 15
        ///
        ///// show nicknames
        ///autoshownick 0
        ///shownick 1
        ///
        ///showhp 1
        ///showhp_colored 1
        ///showhp_x 30
        ///showhp_y 75
        ///
        ///drawtime 1
        ///allowmapschangebg 1
        ///barflash 1
        ///barposition 427
        ///showmapinfo 1
        ///shownickatsb 1
        ///ch_showfollowinglabel 0
        ///
        ///// do not draw unused bars
        ///cf_w [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ndmadapter {
            get {
                return ResourceManager.GetString("ndmadapter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] NFK {
            get {
                object obj = ResourceManager.GetObject("NFK", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
