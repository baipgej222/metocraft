﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTMCL.Update
{
    public class Updater
    {
        public delegate void CheckFinishEventHandler(bool hasUpdate, string updateAddr, int updateBuild);

        public event CheckFinishEventHandler CheckFinishEvent;

        protected virtual void OnCheckFinishEvent(bool hasupdate, string updateaddr, int updateBuild)
        {
            CheckFinishEventHandler handler = CheckFinishEvent;
            if (handler != null) MeCore.Invoke(new Action(() => handler(hasupdate, updateaddr, updateBuild)));
        }
        private const string CheckFile = @"http://cvronmin.github.io/mtmcl-version.json";
        public bool HasUpdate { get; private set; }
        public string DownloadUrl { get; private set; }
        public Updater() {
            var thread = new Thread(Run);
            thread.Start();
        }
        private void Run() {
            try
            {
                string[] builds = MeCore.version.Split('.');
                var req = (HttpWebRequest)WebRequest.Create(CheckFile);
                req.Method = "GET";
                var res = (HttpWebResponse)req.GetResponse();
                UpdateJson updatejs = LitJson.JsonMapper.ToObject<UpdateJson>(new LitJson.JsonReader(new StreamReader(res.GetResponseStream())));
                string[] latest = updatejs.versions[0].version.Split('.');
                if (updatejs == null | updatejs.versions.Length == 0)
                {
                    Logger.log("failed to deserialize update json", Logger.LogType.Error);
                    HasUpdate = false;
                    return;
                }
                if (Convert.ToInt32(builds[0]) < Convert.ToInt32(latest[0]))
                {
                    HasUpdate = true;
                    DownloadUrl = updatejs.versions[0].url;
                    Logger.log("new version found, version is: " + updatejs.versions[0].version);
                    Logger.log("download url is: " + DownloadUrl);
                }
                else if (Convert.ToInt32(builds[1]) < Convert.ToInt32(latest[1]))
                {
                    HasUpdate = true;
                    DownloadUrl = updatejs.versions[0].url;
                    Logger.log("new minor version found, version is: " + updatejs.versions[0].version);
                    Logger.log("download url is: " + DownloadUrl);
                }
                else if (Convert.ToInt32(builds[2]) < Convert.ToInt32(latest[2]))
                {
                    HasUpdate = true;
                    DownloadUrl = updatejs.versions[0].url;
                    Logger.log("new minor version found, version is: " + updatejs.versions[0].version);
                    Logger.log("download url is: " + DownloadUrl);
                }
                else if (Convert.ToInt32(builds[3]) < Convert.ToInt32(latest[3]))
                {
                    HasUpdate = true;
                    DownloadUrl = updatejs.versions[0].url;
                    Logger.log("new build found, version is: " + updatejs.versions[0].version);
                    Logger.log("download url is: " + DownloadUrl);
                }
                else
                {
                    HasUpdate = false;
                    Logger.log("no update.");
                }
                OnCheckFinishEvent(HasUpdate, DownloadUrl, Convert.ToInt32(latest[3]));
            }
            catch (Exception e)
            {
                Logger.log(e);
                HasUpdate = false;
            }
        }
    }
}
