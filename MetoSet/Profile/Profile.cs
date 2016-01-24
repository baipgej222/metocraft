﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetoCraft.Profile
{
    public sealed class Profile
    {
        [LitJson.JsonPropertyName("oldName")]
        public string oldName { get; set; }
        [LitJson.JsonPropertyName("name")]
        public string name { get; set; }
        [LitJson.JsonPropertyName("version")]
        public string version { get; set; }
        [LitJson.JsonPropertyName("winSizeX")]
        public int winSizeX { get; set; }
        [LitJson.JsonPropertyName("winSizeY")]
        public int winSizeY { get; set; }
        [LitJson.JsonPropertyName("Xmx")]
        public int Xmx { get; set; }
        [Obsolete]
        public string auth { get; set; }
        [Obsolete]
        public string token { get; set; }
    }
}
