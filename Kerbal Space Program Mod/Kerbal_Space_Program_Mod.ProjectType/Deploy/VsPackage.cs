namespace KSP4VS.Deploy
{
    using System;
    
    /// <summary>
    /// Helper class that exposes all GUIDs used across VS Package.
    /// </summary>
    internal sealed partial class PackageGuids
    {
        public const string guidVsPackageString = "28b896d8-186e-48b6-850f-3c9505e78ca9";
        public const string guidVsPackageCmdSetString = "f61a30ca-328c-4a2c-b394-f5f4a5eefcd4";
        public const string guidImagesString = "16a2c1c2-393d-4852-82a5-641a7986c148";
        public static Guid guidVsPackage = new Guid(guidVsPackageString);
        public static Guid guidVsPackageCmdSet = new Guid(guidVsPackageCmdSetString);
        public static Guid guidImages = new Guid(guidImagesString);
    }
    /// <summary>
    /// Helper class that encapsulates all CommandIDs uses across VS Package.
    /// </summary>
    internal sealed partial class PackageIds
    {
        public const int DeployConfigurationWindowCommandId = 0x0100;
        public const int bmpPic1 = 0x0001;
        public const int bmpPic2 = 0x0002;
        public const int bmpPicSearch = 0x0003;
        public const int bmpPicX = 0x0004;
        public const int bmpPicArrows = 0x0005;
        public const int bmpPicStrikethrough = 0x0006;
    }
}
