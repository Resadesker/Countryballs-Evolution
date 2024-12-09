#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class PostProcessBuildScript {
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path) {
        if (buildTarget == BuildTarget.iOS) {
            string pbxProjectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(pbxProjectPath);

            string targetGuid = pbxProject.GetUnityMainTargetGuid();
            pbxProject.AddCapability(targetGuid, PBXCapabilityType.InAppPurchase);

            pbxProject.WriteToFile(pbxProjectPath);
            
            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Add App Transport Security settings to allow network access for Unity Ads and IAPs
            PlistElementDict rootDict = plist.root;
            PlistElementDict ats = rootDict.CreateDict("NSAppTransportSecurity");
            ats.SetBoolean("NSAllowsArbitraryLoads", true); // Allow network connections
            
            PlistElementArray skAdNetworkItems = rootDict.CreateArray("SKAdNetworkItems");
            PlistElementDict adNetwork = skAdNetworkItems.AddDict();
            adNetwork.SetString("SKAdNetworkIdentifier", "4dzt52r2t5.skadnetwork"); // Unity's identifier
            
            PlistElementDict exceptionDomains = ats.CreateDict("NSExceptionDomains");
            PlistElementDict unityAdsDomain = exceptionDomains.CreateDict("unity3d.com");
            unityAdsDomain.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);

            plist.WriteToFile(plistPath);
        }
    }
}
#endif