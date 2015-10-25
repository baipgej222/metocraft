﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management;
using Microsoft.Win32;
using System.Runtime.Serialization;
using System.IO;
using System.Windows;
using System.Drawing;

namespace MetoCraft
{
    [DataContract]
    public class Config : ICloneable
    {
        [DataMember]
        public string Javaw;
        [DataMember]
        public string MCPath;
        [DataMember]
        public string Javaxmx;
        [DataMember]
        public string LastPlayVer;
        [DataMember]
        public string ExtraJvmArg;
        [DataMember]
        public string Lang;
        [DataMember]
        public bool Autostart, Report,CheckUpdate;
        [DataMember]
        public double WindowTransparency;
        [DataMember]
        public string BackGround;
        [DataMember]
        public Color color;
        [DataMember]
        public int DownloadSource;
        [DataMember]
        public Dictionary<string, object> PluginConfig = new Dictionary<string, object>();

        [DataMember] public string GUID;

        public Config()
        {
            Javaw = GetJavaDir() ?? "javaw.exe";
            MCPath = MeCore.BaseDirectory + ".minecraft";
            Javaxmx = (GetMemory() / 4).ToString(CultureInfo.InvariantCulture);
            Autostart = false;
            ExtraJvmArg = " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true";
            WindowTransparency = 1;
            BackGround = "default";
            color = Color.FromArgb(255,255,255);
            Report = true;
            DownloadSource = 0;
            Lang = GetValidLang();
            CheckUpdate = true;
            PluginConfig = null;
            GUID = GetGuid();
        }
        public string GetValidLang() {
            if (CultureInfo.CurrentUICulture.Parent.Name != "zh-CHT" && CultureInfo.CurrentUICulture.Parent.Name != "zh-CHS"
                && CultureInfo.CurrentUICulture.Parent.Name != "en") {
                return "en";
            }
            return CultureInfo.CurrentUICulture.Parent.Name;
        }
        public object GetPluginConfig(string key)
        {
            if (PluginConfig.ContainsKey(key))
            {
                return PluginConfig[key];
            }
            return null;
        }

        public void SetPluginConfig(string key, object value)
        {
            if (PluginConfig.ContainsKey(key))
            {
                PluginConfig[key] = value;
            }
            else
            {
                PluginConfig.Add(key, value);
            }
        }

        public object Clone()
        {
            return (Config)this.MemberwiseClone();
        }

        public static Config Load(string file)
        {
            if (!System.IO.File.Exists(file))
                return new Config();
            try
            {
                var fs = new FileStream(file, FileMode.Open);
                var ser = new DataContractSerializer(typeof(Config));
                var cfg = (Config)ser.ReadObject(fs);
                fs.Close();
                if (cfg.GUID == null)
                {
                    cfg.GUID = GetGuid();
                }
                return cfg;
            }
            catch
            {
                MessageBox.Show("加载配置文件遇到错误，使用默认配置");
                return new Config();
            }
        }
        public static void Save(Config cfg = null ,string file = null)
        {
            if (cfg == null)
            {
                cfg = MeCore.Config;
            }
            if (file == null)
            {
                file = MeCore.BaseDirectory + "metocraft.xml";
            }
            var fs = new FileStream(file, FileMode.Create);
            var ser = new DataContractSerializer(typeof(Config));
            ser.WriteObject(fs, cfg);
            fs.Close();
        }

        public void Save(string file = null)
        {
            Save(this, file);
        }
        /// <summary>
        /// 读取注册表，寻找安装的java路径
        /// </summary>
        /// <returns>javaw.exe路径</returns>
        public static string GetJavaDir()
        {
            try
            {
                RegistryKey reg = Registry.LocalMachine;
                var openSubKey = reg.OpenSubKey("SOFTWARE");
                if (openSubKey != null)
                {
                    var registryKey = openSubKey.OpenSubKey("JavaSoft");
                    if (registryKey != null)
                        reg = registryKey.OpenSubKey("Java Runtime Environment");
                }
                if (reg != null)
                    foreach (string ver in reg.GetSubKeyNames())
                    {
                        try
                        {
                            RegistryKey command = reg.OpenSubKey(ver);
                            if (command != null)
                            {
                                string str = command.GetValue("JavaHome").ToString();
                                if (str != "")
                                    return str + @"\bin\javaw.exe";
                            }
                        }
                        catch { return null; }
                    }
                return null;
            }
            catch { return null; }

        }
        /// <summary>
        /// 获取系统物理内存大小
        /// </summary>
        /// <returns>系统物理内存大小，支持64bit,单位MB</returns>
        public static ulong GetMemory()
        {
            try
            {
                double capacity = 0.0;
                var cimobject1 = new ManagementClass("Win32_PhysicalMemory");
                ManagementObjectCollection moc1 = cimobject1.GetInstances();
                foreach (var o in moc1)
                {
                    var mo1 = (ManagementObject) o;
                    capacity += ((Math.Round(Int64.Parse(mo1.Properties["Capacity"].Value.ToString()) / 1024 / 1024.0, 1)));
                }
                moc1.Dispose();
                cimobject1.Dispose();
                UInt64 qmem = Convert.ToUInt64(capacity.ToString(CultureInfo.InvariantCulture));
                return qmem;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Logger.error("获取内存失败");
                Logger.error(ex);
                return ulong.MaxValue;

            }
        }

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
