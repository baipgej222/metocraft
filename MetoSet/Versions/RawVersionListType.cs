﻿using System.Runtime.Serialization;

namespace MTMCL.Versions
{
    [DataContract]
    class RawVersionListType
    {
        [DataMember(Order = 0,IsRequired = true)]
        private RemoteVerType[] versions;
        [DataMember(Order = 1,IsRequired = true)]
        private latest latest;

        public RemoteVerType[] getVersions()
        {
            return versions;
        }

        public latest getLastestVersion()
        {
            return latest;
        }

        public RawVersionListType()
        {
            versions = null;
            latest = new latest();
        }
    }
    [DataContract]
    class latest
    {
        [DataMember(Order = 0, IsRequired = true)]
        string snapshot;
        [DataMember(Order = 1, IsRequired = true)]
        string release;

        public latest()
        {
            snapshot = "";

            release = "";
        }
    }
}
