#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.iOS.Xcode;

public static class CloudBuildPostProcessScript {
    // PostExport method for Unity Cloud Build
    public static void PostExport(string exportPath) {
        // Path to the Xcode project's .pbxproj file
        string pbxProjectPath = PBXProject.GetPBXProjectPath(exportPath);
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(pbxProjectPath);

        // Get the main target GUID
        string targetGuid = pbxProject.GetUnityMainTargetGuid();
        // Add In-App Purchase capability
        pbxProject.AddCapability(targetGuid, PBXCapabilityType.InAppPurchase);
        
        // Write updates to the .pbxproj file
        pbxProject.WriteToFile(pbxProjectPath);

        // Modify the Info.plist file for App Transport Security
        string plistPath = Path.Combine(exportPath, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Add App Transport Security settings
        PlistElementDict rootDict = plist.root;
        PlistElementDict ats = rootDict.CreateDict("NSAppTransportSecurity");
        ats.SetBoolean("NSAllowsArbitraryLoads", true);

        // Add SKAdNetwork identifier for Unity Ads
        PlistElementArray skAdNetworkItems = rootDict.CreateArray("SKAdNetworkItems");
        PlistElementDict adNetwork = skAdNetworkItems.AddDict();
        adNetwork.SetString("SKAdNetworkIdentifier", "4dzt52r2t5.skadnetwork");

        // Add an exception for Unity Ads domain to allow HTTP loads
        PlistElementDict exceptionDomains = ats.CreateDict("NSExceptionDomains");
        PlistElementDict unityAdsDomain = exceptionDomains.CreateDict("unity3d.com");
        unityAdsDomain.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);
        
        pbxProject.AddFrameworkToProject(targetGuid, "StoreKit.framework", false);
        ats.SetBoolean("NSAllowsArbitraryLoadsInWebContent", true);

        // Write the modified plist back to the file
        plist.WriteToFile(plistPath);
    }
}
#endif