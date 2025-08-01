#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

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

            sw.WriteLine("installer.pods_project.targets.each do |target|");
            sw.WriteLine("if target.name == 'BoringSSL-GRPC'");
            sw.WriteLine("target.source_build_phase.files.each do |file|");
            sw.WriteLine("if file.settings && file.settings['COMPILER_FLAGS']");
            sw.WriteLine("flags = file.settings['COMPILER_FLAGS'].split");
            sw.WriteLine("flags.reject! { |flag| flag == '-GCC_WARN_INHIBIT_ALL_WARNINGS' }");
            sw.WriteLine("file.settings['COMPILER_FLAGS'] = flags.join(' ')");
            sw.WriteLine("end\nend\nend\nend");

            sw.WriteLine("installer.generated_projects.each do |project|");
            sw.WriteLine("project.targets.each do |target|");
            sw.WriteLine("target.build_configurations.each do |config|");
            sw.WriteLine("config.build_settings[\"DEVELOPMENT_TEAM\"] = \"VJ4WHTV2TN\"");
            sw.WriteLine("config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '13.0'");
            sw.WriteLine("end\nend\nend\nend");
        }
    }
}
#endif