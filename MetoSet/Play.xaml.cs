﻿using MTMCL.Assets;
using MTMCL.Lang;
using MTMCL.Task;
using MTMCL.util;
using MTMCL.Versions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MTMCL
{
    public partial class Play : Grid
    {
        public VersionJson[] versions;
        public Play()
        {
            InitializeComponent();
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }
        private void Back()
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            string path = MeCore.Config.MCPath;
            if (MeCore.IsServerDedicated)
            {
                if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                {
                    path = path.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                }
            }
            if (!string.IsNullOrWhiteSpace(path))
            {
                //App.core = LauncherCore.Create(new LauncherCoreCreationOption(path, MeCore.Config.Javaw, new KMCCC.Modules.JVersion.NewJVersionLocator()));
                versions = VersionReader.GetVersion(MeCore.Config.MCPath);
                var dt = new DataTable();
                dt.Columns.Add("Version");
                dt.Columns.Add("Type");
                foreach (var version in versions)
                {
                    dt.Rows.Add(version.id, version.type);
                }
                listVer.SelectedIndex = -1;
                listVer.DataContext = dt;
                listVer.SelectedIndex = -1;
            }
            if (versions == null)
            {
                gridNoVersion.Visibility = Visibility.Visible;
                return;
            }
            if (versions.Length == 0)
            {
                gridNoVersion.Visibility = Visibility.Visible;
            }
        }
        List<LibraryUniversal> libs = new List<LibraryUniversal>();
        private async void listVer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* if (App.core != null)
             {*/
            var version = versions[listVer.SelectedIndex];
            var dtlib = new DataTable();
            dtlib.Columns.Add("Lib");
            dtlib.Columns.Add("Exist");
            libs.Clear();
            foreach (var item in version.libraries.ToUniversalLibrary())
            {
                libs.Add(item);
            }
            foreach (LibraryUniversal libfile in libs)
            {
                dtlib.Rows.Add(new object[] { libfile.name, FileHelper.IfFileVaild(libfile.path) });
            }
            listLib.DataContext = dtlib;
            await System.Threading.Tasks.Task.Factory.StartNew(new Action(delegate
             {
                 Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                 {
                     var _version = versions[listVer.SelectedIndex];
                     string indexpath = MeCore.Config.MCPath + "\\assets\\indexes\\" + _version.assets + ".json";
                     if (MeCore.IsServerDedicated)
                     {
                         if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                         {
                             indexpath = indexpath.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                         }
                     }
                     if (!File.Exists(indexpath))
                     {
                         FileHelper.CreateDirectoryForFile(indexpath);
                         string result = new WebClient().DownloadString(new Uri(MTMCL.Resources.UrlReplacer.getDownloadUrl() + "indexes/" + _version.assets + ".json"));
                         StreamWriter sw = new StreamWriter(indexpath);
                         LitJson.JsonWriter jw = new LitJson.JsonWriter(sw);
                         jw.PrettyPrint = true;
                         LitJson.JsonMapper.ToJson(LitJson.JsonMapper.ToObject<Assets.AssetIndex>(result), jw);
                         sw.Close();
                     }
                     var sr = new StreamReader(indexpath);
                     AssetIndex assets = LitJson.JsonMapper.ToObject<Assets.AssetIndex>(sr);
                     Logger.log(assets.objects.Count.ToString(CultureInfo.InvariantCulture), " assets in total");
                     try
                     {
                         var dt = new DataTable();
                         dt.Columns.Add("Assets");
                         dt.Columns.Add("Exist");
                         foreach (KeyValuePair<string, AssetsEntity> entity in assets.objects)
                         {
                             dt.Rows.Add(new object[] { entity.Key, assetExist(entity).ToString() });
                         }
                         listAsset.DataContext = dt;
                     }
                     catch
                     {

                     }
                 }));

             }));
            lblSelectVer.Content = version.id;
            //}
        }
        private bool assetExist(KeyValuePair<string, AssetsEntity> entity)
        {
            string path = MeCore.Config.MCPath;
            if (MeCore.IsServerDedicated)
            {
                if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                {
                    path = path.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                }
            }
            return File.Exists(path + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + @"\" + entity.Value.hash);
        }
        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            Launch.Login.IAuth auth;
            /*if (string.IsNullOrWhiteSpace(MeCore.Config.DefaultAuth))
            {
                ACSelect ac = new ACSelect();
                ac.ShowDialog();
                auth = ac.auth;
            }
            else
            {
                Config.SavedAuth dauth;
                MeCore.Config.SavedAuths.TryGetValue(MeCore.Config.DefaultAuth, out dauth);
                auth = dauth.AuthType.Equals("KMCCC.Yggdrasil") ? new KMCCC.Authentication.YggdrasilDebuggableRefresh(Guid.Parse(dauth.AccessToken), true, Guid.Parse(MeCore.Config.GUID)) as KMCCC.Authentication.IAuthenticator : new KMCCC.Authentication.WarpedAuhenticator(new KMCCC.Authentication.AuthenticationInfo { DisplayName = MeCore.Config.DefaultAuth, AccessToken = new Guid(dauth.AccessToken), UUID = new Guid(dauth.UUID), UserType = dauth.UserType, Properties = dauth.Properies }) as KMCCC.Authentication.IAuthenticator;
            }*/
            ACLogin ac = new ACLogin();
            ac.ShowDialog();
            auth = ac.auth;
            if (auth == null)
            {
                return;
            }
            MeCore.MainWindow._LaunchOptions = Launch.LaunchGameInfo.CreateInfo(MeCore.Config.MCPath, auth, versions[listVer.SelectedIndex], MeCore.Config.Javaw, (int)MeCore.Config.Javaxmx, CreateServerInfo());
            MeCore.MainWindow.launchFlyout.IsOpen = true;
            Back();
        }
        private Launch.ServerInfo CreateServerInfo()
        {
            if (MeCore.IsServerDedicated)
            {
                if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ServerIP))
                {
                    if (MeCore.Config.Server.ServerIP.IndexOf(':') != -1)
                    {
                        ushort port = 25565;
                        if (!ushort.TryParse(MeCore.Config.Server.ServerIP.Substring(MeCore.Config.Server.ServerIP.IndexOf(':')).Trim(':'), out port))
                        {
                            port = 25565;
                        }
                        return new Launch.ServerInfo
                        {
                            Ip = MeCore.Config.Server.ServerIP.Substring(0, MeCore.Config.Server.ServerIP.IndexOf(':')).Trim(':'),
                            Port = port
                        };
                    }
                }
            }
            return null;
        }

        private void butDLAssets_Click(object sender, RoutedEventArgs e)
        {
            TaskListBar task = new TaskListBar() { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/download-banner.jpg")) };
            var thGet = new Thread(new ThreadStart(delegate
            {
                WebClient _downer = new WebClient();
                VersionJson _version = null;
                do
                {
                    Dispatcher.Invoke(new Action(() => _version = versions[listVer.SelectedIndex]));
                } while (_version == null);
                string indexpath = MeCore.Config.MCPath + "\\assets\\indexes\\" + _version.assets + ".json";
                if (MeCore.IsServerDedicated)
                {
                    if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                    {
                        indexpath = indexpath.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                    }
                }
                if (!File.Exists(indexpath))
                {
                    FileHelper.CreateDirectoryForFile(indexpath);
                    string result = new WebClient().DownloadString(new Uri(MTMCL.Resources.UrlReplacer.getDownloadUrl() + "indexes/" + _version.assets + ".json"));
                    StreamWriter sw = new StreamWriter(indexpath);
                    LitJson.JsonWriter jw = new LitJson.JsonWriter(sw);
                    jw.PrettyPrint = true;
                    LitJson.JsonMapper.ToJson(LitJson.JsonMapper.ToObject<Assets.AssetIndex>(result), jw);
                    sw.Close();
                }
                var sr = new StreamReader(indexpath);
                AssetIndex assets = LitJson.JsonMapper.ToObject<Assets.AssetIndex>(sr);
                int i = 0;
                foreach (KeyValuePair<string, AssetsEntity> entity in assets.objects)
                {
                    i++;
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        task.setTaskStatus((((float)i) / assets.objects.Count * 100).ToString() + "%");
                    }));
                    string url = MTMCL.Resources.UrlReplacer.getResourceUrl() + entity.Value.hash.Substring(0, 2) + "/" + entity.Value.hash;
                    string file = MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + "\\" + entity.Value.hash;
                    if (MeCore.IsServerDedicated)
                    {
                        if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                        {
                            file = file.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                        }
                    }
                    FileHelper.CreateDirectoryForFile(file);
                    try
                    {
                        if (FileHelper.IfFileVaild(file, entity.Value.size)) continue;
                        //Downloader.DownloadFileAsync(new Uri(Url), File,Url);
                        _downer.DownloadFile(new Uri(url), file);
                        Logger.log(i.ToString(CultureInfo.InvariantCulture), "/", assets.objects.Count.ToString(CultureInfo.InvariantCulture), file.Substring(MeCore.Config.MCPath.Length), "下载完毕");
                        if (i == assets.objects.Count)
                        {
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                Logger.log("assets下载完毕");
                                //MeCore.NIcon.ShowBalloonTip(3000, LangManager.GetLangFromResource("SyncAssetsFinish"));
                            }));
                        }
                    }
                    catch (WebException ex)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            Logger.log(ex.Response.ResponseUri.ToString());
                            Logger.error(ex);
                        }));
                    }
                }
            }));
            MeCore.MainWindow.addTask("dl-assets", task.setThread(thGet).setTask(LangManager.GetLangFromResource("TaskDLAssets")));
        }
        private void butRDLAsset_Click(object sender, RoutedEventArgs e)
        {
            TaskListBar task = new TaskListBar() { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/download-banner.jpg")) };
            var thGet = new Thread(new ThreadStart(delegate
            {
                WebClient _downer = new WebClient();
                VersionJson _version = null;
                do
                {
                    Dispatcher.Invoke(new Action(() => _version = versions[listVer.SelectedIndex]));
                } while (_version == null);
                string indexpath = MeCore.Config.MCPath + "\\assets\\indexes\\" + _version.assets + ".json";
                if (MeCore.IsServerDedicated)
                {
                    if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                    {
                        indexpath = indexpath.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                    }
                }
                if (!File.Exists(indexpath))
                {
                    FileHelper.CreateDirectoryForFile(indexpath);
                    string result = new WebClient().DownloadString(new Uri(MTMCL.Resources.UrlReplacer.getDownloadUrl() + "indexes/" + _version.assets + ".json"));
                    StreamWriter sw = new StreamWriter(indexpath);
                    LitJson.JsonWriter jw = new LitJson.JsonWriter(sw);
                    jw.PrettyPrint = true;
                    LitJson.JsonMapper.ToJson(LitJson.JsonMapper.ToObject<Assets.AssetIndex>(result), jw);
                    sw.Close();
                }
                var sr = new StreamReader(indexpath);
                AssetIndex assets = LitJson.JsonMapper.ToObject<Assets.AssetIndex>(sr);
                int i = 0;
                foreach (KeyValuePair<string, AssetsEntity> entity in assets.objects)
                {
                    i++;
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        task.setTaskStatus((float)i / assets.objects.Count * 100 + "%");
                    }));
                    string url = MTMCL.Resources.UrlReplacer.getResourceUrl() + entity.Value.hash.Substring(0, 2) + "/" + entity.Value.hash;
                    string file = MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + "\\" + entity.Value.hash;
                    if (MeCore.IsServerDedicated)
                    {
                        if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                        {
                            file = file.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                        }
                    }
                    FileHelper.CreateDirectoryForFile(file);
                    try
                    {
                        if (FileHelper.IfFileVaild(file, entity.Value.size)) File.Delete(file);
                        _downer.DownloadFile(new Uri(url), file);
                        Logger.log(i.ToString(CultureInfo.InvariantCulture), "/", assets.objects.Count.ToString(CultureInfo.InvariantCulture), file.Substring(MeCore.Config.MCPath.Length), "下载完毕");
                        if (i == assets.objects.Count)
                        {
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                Logger.log("assets重新下载完毕");
                            }));
                        }
                    }
                    catch (WebException ex)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            Logger.log(ex.Response.ResponseUri.ToString());
                            Logger.error(ex);
                        }));
                    }
                }
            }));
            MeCore.MainWindow.addTask("dl-assets", task.setThread(thGet).setTask(LangManager.GetLangFromResource("TaskRDLAssets")));
        }

        private void butGoDLMC_Click(object sender, RoutedEventArgs e)
        {
            MeCore.MainWindow.ChangePage("download");
        }
    }
}
