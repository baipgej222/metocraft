﻿using MTMCL.Lang;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace MTMCL
{
    public static class MeCore
    {
        public static string version;
        public static Config Config;
        //public static Server.ServerInfo ServerCfg;
        public static bool IsServerDedicated;
        public static Dictionary<string, object> Language = new Dictionary<string, object>();
        public static Dictionary<string, ResourceDictionary> Color = new Dictionary<string, ResourceDictionary>();
        public static string BaseDirectory = Environment.CurrentDirectory + '\\';
        public static string DataDirectory = Environment.CurrentDirectory + '\\' + "MTMCL" + '\\';
        private readonly static string Cfgfile = BaseDirectory + "mtmcl_config.json";
        private readonly static string Serverfile = BaseDirectory + "mtmcl_server_config.json";
        public static string DefaultBG = "pack://application:,,,/Resources/bg.png";
        //public static NotiIcon NIcon = new NotiIcon();
        public static MainWindow MainWindow = null;
        private static Application thisApplication = Application.Current;
        public static Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;
        static MeCore()
        {
            version = Application.ResourceAssembly.FullName.Split('=')[1];
            version = version.Substring(0, version.IndexOf(','));
            Logger.log("----------" + DateTime.Now.ToLongTimeString() + " launch log----------");
            Logger.log("MTMCL Ver." + version + " launching");
            /*if (File.Exists(Serverfile))
            {
                ServerCfg = Server.ServerInfo.Load(Serverfile);
                if (ServerCfg.Ignore)
                {
                    IsServerDedicated = false;
                    Logger.log("Launching normal version due to failure to read the server config");
                }
                else if (App.forceNonDedicate)
                {
                    IsServerDedicated = false;
                    Logger.log("Launching normal version due to the argument");
                }
                else
                {
                    IsServerDedicated = true;
                    Logger.log("Launching server-dedicated version");
                }
            }
            else
            {
                Logger.log("Launching normal version as the server config file is missing");
            }*/
            if (File.Exists(Cfgfile))
            {
                Config = Config.Load(Cfgfile);
                Logger.log(string.Format("loaded {0}", Cfgfile));
                Logger.log(Config.ToReadableLog());
                LoadLanguage();
                LoadColor();
                //ChangeLanguage(Config.Lang);
            }
            else
            {
                Config = new Config();
                Logger.log("loaded default config");
                LoadLanguage();
                LoadColor();
            }
            if (Config.Javaw == "autosearch")
            {
                Config.Javaw = KMCCC.Tools.SystemTools.FindValidJava().First();
            }
            if (Config.Javaxmx == -1)
            {
                Config.Javaxmx = (KMCCC.Tools.SystemTools.GetTotalMemory() / 4) / 1024 / 1024;
            }
            LangManager.UseLanguage(Config.Lang);
#if DEBUG
#else
            //ReleaseCheck();
#endif
        }
        private static void ReleaseCheck()
        {
            if (Config.CheckUpdate)
            {
                var updateChecker = new Update.Updater();
                updateChecker.CheckFinishEvent += UpdateCheckerOnCheckFinishEvent;
            }
        }
        private static void UpdateCheckerOnCheckFinishEvent(bool hasUpdate, string updateAddr, string updateinfo, int updateBuild)
        {
            if (hasUpdate)
            {
                var a = MessageBox.Show(MainWindow, updateinfo,"更新", MessageBoxButton.OKCancel,
                    MessageBoxImage.Information);
                if (a == MessageBoxResult.OK)
                {
                    var updater = new Update.TaskGui(updateBuild, updateAddr);
                    MainWindow.Hide();
                    updater.setTask("更新MTMCL").ShowDialog();
                }
                if (a == MessageBoxResult.No || a == MessageBoxResult.None)
                {
                    if (MessageBox.Show(MainWindow, updateinfo, "更新", MessageBoxButton.OKCancel,
                    MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        var updater = new Update.TaskGui(updateBuild, updateAddr);
                        MainWindow.Hide();
                        updater.setTask("更新MTMCL").ShowDialog();
                    }
                }
            }
        }
        public static void Invoke(Delegate invoke, object[] argObjects = null)
        {
            Dispatcher.Invoke(invoke, argObjects);
        }
        public static void ChangeLanguage(string lang)
        {
            LangManager.UseLanguage(lang);
        }
        private static void LoadLanguage()
        {
            ResourceDictionary lang = LangManager.LoadLangFromResource("pack://application:,,,/Lang/en.xaml");
            Language.Add((string)lang["DisplayName"], lang["LangName"]);
            LangManager.Add(lang["LangName"] as string, "pack://application:,,,/Lang/en.xaml");

            lang = LangManager.LoadLangFromResource("pack://application:,,,/Lang/zh-CHS.xaml");
            Language.Add((string)lang["DisplayName"], lang["LangName"]);
            LangManager.Add(lang["LangName"] as string, "pack://application:,,,/Lang/zh-CHS.xaml");

            lang = LangManager.LoadLangFromResource("pack://application:,,,/Lang/zh-CHT.xaml");
            Language.Add((string)lang["DisplayName"], lang["LangName"]);
            LangManager.Add(lang["LangName"] as string, "pack://application:,,,/Lang/zh-CHT.xaml");
            if (Directory.Exists(DataDirectory + "Lang"))
            {
                foreach (string langFile in Directory.GetFiles(DataDirectory + "Lang", "*.xaml", SearchOption.TopDirectoryOnly))
                {
                    lang = LangManager.LoadLangFromResource(langFile);
                    Language.Add((string)lang["DisplayName"], lang["LangName"]);
                    LangManager.Add(lang["LangName"] as string, langFile);
                }
            }
            else
            {
                Directory.CreateDirectory(DataDirectory + "Lang");
            }
        }
        private static void LoadColor()
        {
            ResourceDictionary color = new ResourceDictionary();
            string uri = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml";
            for (int i = 0; i < 23; i++)
            {
                string s = Enum.GetName(typeof(ColorScheme), i);
                color.Source = new Uri(string.Format(uri, s));
                Color.Add(s, color);
            }
            if (Directory.Exists(DataDirectory + "Color"))
            {
                foreach (string file in Directory.GetFiles(DataDirectory + "Color", "*.xaml", SearchOption.TopDirectoryOnly))
                {
                    color.Source = new Uri(file);
                    Color.Add(Path.GetFileNameWithoutExtension(file), color);
                    MahApps.Metro.ThemeManager.AddAccent(Path.GetFileNameWithoutExtension(file), color.Source);
                }
            }
            else
            {
                Directory.CreateDirectory(DataDirectory + "Color");
            }
        }
        private enum ColorScheme
        {
            Red, Green, Blue, Purple, Orange, Lime, Emerald, Teal, Cyan, Cobalt, Indigo, Violet, Pink, Magenta, Crimson, Amber, Yellow, Brown, Olive, Steel, Mauve, Taupe, Sienna
        }
        public static void Halt(int code = 0)
        {
            thisApplication.Shutdown(code);
        }

        public static void SingleInstance(Window window)
        {
            System.Threading.ThreadPool.RegisterWaitForSingleObject(App.ProgramStarted, OnAnotherProgramStarted, window, -1, false);
        }

        private static void OnAnotherProgramStarted(object state, bool timedout)
        {
            var window = state as Window;
            //NIcon.ShowBalloonTip(2000, LangManager.GetLangFromResource("MTMCLHiddenInfo"));
            if (window != null)
            {
                Dispatcher.Invoke(new Action(window.Show));
                Dispatcher.Invoke(new Action(() => { window.Activate(); }));
            }
        }
    }
}
