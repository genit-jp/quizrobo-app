using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


#if UNITY_EDITOR
    public class Builder
    {
        public static void iOSBuild()
        {
            var outpubDirKey = "-output-dir";
            
            var scenePaths = GetBuildScenePaths();
            var outputDir = GetParameterFrom(outpubDirKey);
            var buildTarget = BuildTarget.iOS;
            var buildOptions = BuildOptions.None;
            
            var buildReport = BuildPipeline.BuildPlayer(scenePaths.ToArray(), outputDir, buildTarget, buildOptions);
            
            var summary = buildReport.summary;

            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log("Success");
            }
            else
            {
                Debug.Log("Error");
            }
        }
        
        public static void BuildDev()
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "jp.genit.kidsquiz.dev");
            PlayerSettings.productName = "開発用";
            iOSBuild();
        }
        
        public static void BuildProd()
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "jp.genit.kidsquiz");
            PlayerSettings.productName = "キッズクイズ";
            iOSBuild();
        }
        
        private static IEnumerable<string> GetBuildScenePaths()
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            return scenes.Where((arg) => arg.enabled).Select((arg) => arg.path);
        }

        private static string GetParameterFrom(string key)
        {
            var args = System.Environment.GetCommandLineArgs();
            var index = args.ToList().FindIndex((arg) => arg == key);
            var paramIndex = index + 1;
            
            if (index < 0 || args.Length <= paramIndex)
            {
                return null;
            }

            return args[paramIndex];
        }

        public static string GetAppVersion()
        {
            return PlayerSettings.bundleVersion;
        }
    }
#endif
