﻿using MTMCL.JsonClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows.Controls;

namespace MTMCL.Forge
{
    class ForgeVersionList
    {
        public delegate void ForgePageReadyHandle();
        public event ForgePageReadyHandle ForgePageReadyEvent;
        private readonly DataContractJsonSerializer _forgeVerJsonParse = new DataContractJsonSerializer(typeof(ForgeVersion));
        private ForgeVersion _forge;
        public Dictionary<string, string> ForgeDownloadUrl = new Dictionary<string, string>(),
            ForgeChangeLogUrl = new Dictionary<string, string>();
        public void GetVersion()
        {
            var webClient = new WebClient();
            webClient.DownloadStringAsync(new Uri(Resources.UrlReplacer.getForgeMaven("http://files.minecraftforge.net/maven/net/minecraftforge/forge/json")));
            webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Logger.log("获取forge列表失败");
                Logger.error(e.Error);
            }
            else
            {
                _forge = LitJson.JsonMapper.ToObject<ForgeVersion>(e.Result);
//                    _forgeVerJsonParse.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(e.Result))) as ForgeVersion;
                Logger.log("获取Forge列表成功");
            }
            ForgePageReadyEvent?.Invoke();
        }

        public object[] GetNew()
        {
            ArrayList arrayList = new ArrayList(_forge.promos.Count);
            ForgeVersion forge = _forge;
            foreach (var item in forge.promos) {
                ForgeVersion.Version ver = null;
                if (forge.number.TryGetValue(item.Value.ToString(), out ver)) {
                    bool install = false;
                    string version = ver.mcversion + "-" + ver.version;
                    if (!string.IsNullOrWhiteSpace(ver.branch))
                    {
                        version = version + "-" + ver.branch;
                    }
                    for (int i = 0; i < ver.files.GetLength(0); i++)
                    {
                        if (ver.files[i][1].Equals("changelog"))
                        {
                            ForgeChangeLogUrl[ver.version] = forge.webpath + "/" + version + "/" + forge.artifact + "-" +version + "-" + ver.files[i][1] + "." + ver.files[i][0];
                        }
                        if (ver.files[i][1].Equals("installer"))
                        {
                            install = true;
                            ForgeDownloadUrl[ver.version] = forge.webpath + "/" + version + "/" + forge.artifact + "-" + version + "-" + ver.files[i][1] + "." + ver.files[i][0];
                        }
                    }
                    if (!install)
                    {
                        Logger.log("MinecraftForge " + ver.version, " for ", ver.mcversion, " does not have installer");
                    }
                    arrayList.Add(new object[] { ver.version, ver.mcversion, DateTime.SpecifyKind(util.TimeHelper.UnixTimeStampToDateTime((double)ver.modified), DateTimeKind.Local), item.Key.Contains("latest") ? "latest" : (item.Key.Contains("recommended") ? "recommended" : "") });
                    Logger.log("获取Forge", ver.version);
                }
            }

            return arrayList.ToArray();
        }
        public TreeViewItem[] Get()
        {
            ArrayList arrayList = new ArrayList(_forge.number.Count);
            TreeViewItem treeViewItem = new TreeViewItem();
            arrayList.Add(treeViewItem);
            ForgeVersion forge = _forge;
            foreach (var item in forge.number.Values)
            {
                bool install = false;
                for (int i = 0; i < item.files.GetLength(1); i++)
                {
                    if (item.files[i][2].Equals("changelog"))
                    {
                        ForgeChangeLogUrl[item.version] = forge.artifact + "-" + item.mcversion + "-" + item.version + "-" + item.files[i][1] + "." + item.files[i][0];
                    }
                    if (item.files[i][2].Equals("installer"))
                    {
                        install = true;
                        ForgeDownloadUrl[item.version] = forge.artifact + "-" + item.mcversion + "-" + item.version + "-" + item.files[i][1] + "." + item.files[i][0];
                    }
                }
                if (!install)
                {
                    Logger.log("MinecraftForge" + item.version, " for ", item.mcversion, "does not have installer");
                }
                if (treeViewItem.Header == null)
                {
                    treeViewItem.Header = item.mcversion;
                }
                else
                {
                    if (treeViewItem.Header.ToString() != item.mcversion)
                    {
                        treeViewItem = new TreeViewItem();
                        arrayList.Add(treeViewItem);
                        treeViewItem.Header = item.mcversion;
                    }
                }
                Logger.log(treeViewItem.Header.ToString());
                Logger.log(item.mcversion);

                treeViewItem.Items.Add(item.version);
                Logger.log("获取Forge", item.version);
            }

            return arrayList.ToArray(typeof(TreeViewItem)) as TreeViewItem[];
        }

    }
}

