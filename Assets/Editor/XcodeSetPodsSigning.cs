#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class XcodeSetPodsSigning : MonoBehaviour
{
    [PostProcessBuild(45)] //must be between 40 and 50 to ensure that it's not overriden by Podfile generation (40) and that it's added before "pod install" (50)
    public static void FixPodFile(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        using (StreamWriter sw = File.AppendText(buildPath + "/Podfile"))
        {
            sw.WriteLine("post_install do |installer|");
            sw.WriteLine("installer.generated_projects.each do |project|");
            sw.WriteLine("project.targets.each do |target|");
            sw.WriteLine("target.build_configurations.each do |config|");
            sw.WriteLine("config.build_settings[\"DEVELOPMENT_TEAM\"] = \"VJ4WHTV2TN\"");
            sw.WriteLine("end\nend\nend\nend");
        }
    }
}
#endif