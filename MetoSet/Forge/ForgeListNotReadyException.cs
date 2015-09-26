﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MetoCraft.Lang;

namespace MetoCraft.Forge
{
    class ForgeListNotReadyException:Exception
    {
        public override string Message
        {
            get
            {
                return LangManager.GetLangFromResource("ForgeNotReady");
            }
        }
        public override string ToString()
        {
            return this.Message;
        }
    }
}
