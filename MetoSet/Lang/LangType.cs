﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MetoSet.Lang
{
    class LangType
    {
        public string Name;
        public ResourceDictionary Language = new ResourceDictionary();
        public LangType(string Name,string Path)
        {
            this.Name = Name;
            Language.Source = new Uri(Path);
        }
    }
}
