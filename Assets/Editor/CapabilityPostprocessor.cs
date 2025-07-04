using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

/// <summary>
/// Unity iOSビルド後にUrlSchemeを追加する為のPostprocessor
/// </summary>
public class CapabilityPostprocessor
{
    /// <summary>
    /// ビルド後の処理( Android / iOS共通 )
    /// </summary>
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS) return;

        // get project info
        string pbxPath = PBXProject.GetPBXProjectPath(path);
        var proj = new PBXProject();
        proj.ReadFromFile(pbxPath);
        var guid = proj.GetUnityMainTargetGuid();

        // get entitlements path
        string[] idArray = Application.identifier.Split('.');
        var entitlementsPath = $"Unity-iPhone/melody.entitlements";

        // create capabilities manager
        var capManager = new ProjectCapabilityManager(pbxPath, entitlementsPath, null, guid);

        // Add necessary capabilities
        capManager.AddPushNotifications(true);
        capManager.AddAssociatedDomains(new string[] {"applinks:melody-jpop.onelink.me"});

        // Write to file
        capManager.WriteToFile();
    }
}