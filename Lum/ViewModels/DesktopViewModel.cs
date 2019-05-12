using System;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

namespace Lum.ViewModels
{
    public class DesktopViewModel
    {
        public DesktopViewModel()
        {
            var info = AnalyticsInfo.VersionInfo;
            SystemFamily = info.DeviceFamily;

            var version = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            var v = ulong.Parse(version);
            var v1 = (int) (v & 0xFFFF000000000000L) >> 48;
            var v2 = (int) (v & 0x0000FFFF00000000L) >> 32;
            var v3 = (int) (v & 0x00000000FFFF0000L) >> 16;
            var v4 = (int) (v & 0x000000000000FFFFL);
            SystemVersion = new Version(v1, v2, v3, v4);

            var package = Package.Current;
            SystemArchitecture = package.Id.Architecture.ToString();
            AppName = package.DisplayName;

            var pv = package.Id.Version;
            ApplicationVersion = new Version(pv.Major, pv.Minor, pv.Build, pv.Revision);

            var eas = new EasClientDeviceInformation();
            SystemManufacturer = eas.SystemManufacturer;
            SystemModel = eas.SystemProductName;
            SystemName = eas.FriendlyName;
            SystemOperatingSystem = eas.OperatingSystem;
        }

        public string SystemOperatingSystem { get; }

        public string SystemName { get; }

        public string SystemModel { get;  }

        public string SystemManufacturer { get;  }

        public Version ApplicationVersion { get; }

        public string AppName { get;  }

        public string SystemArchitecture { get;  }

        public Version SystemVersion { get;  }

        public string SystemFamily { get; }

        public string FullOperatingSystem => $"{SystemOperatingSystem} ({SystemArchitecture}) Version {SystemVersion}";

        public string SystemMakeAndModel => $"{SystemManufacturer} {SystemModel}";
    }
}