using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
using System;
using UnityEditor;
#endif


#if UNITY_EDITOR
public class Builder
{
    public static void iOSBuild()
    {
        string outpubDirKey = "-output-dir";

        IEnumerable<string> scenePaths = GetBuildScenePaths();
        string outputDir = GetParameterFrom(outpubDirKey);
        BuildTarget buildTarget = BuildTarget.iOS;
        BuildOptions buildOptions = BuildOptions.None;

        BuildReport buildReport = BuildPipeline.BuildPlayer(scenePaths.ToArray(), outputDir, buildTarget, buildOptions);

        BuildSummary summary = buildReport.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Success");
        }
        else
        {
            Debug.Log("Error");
        }
    }

    public static void BuildAndroidAabProd()
    {
        BuildAndroidInternal(true, BuildType.Prod);
    }

    public static void BuildAndroidAabDev()
    {
        BuildAndroidInternal(true, BuildType.Dev);
    }

    public static void BuildAndroidApkProd()
    {
        BuildAndroidInternal(false, BuildType.Prod);
    }

    public static void BuildAndroidApkDev()
    {
        BuildAndroidInternal(false, BuildType.Dev);
    }

    private static void BuildAndroidInternal(bool isAab, BuildType buildType)
    {
        string outputPath = GetParameterFrom("-output-path");
        if (string.IsNullOrEmpty(outputPath))
        {
            Debug.LogError("-output-path パラメータが必要です");
            return;
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable("KEYSTORE_PASS");
        PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable("KEYSTORE_PASS");
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        EditorUserBuildSettings.buildAppBundle = isAab;

        // 元の設定を保存
        string originalProductName = PlayerSettings.productName;
        string originalBundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);

        SetPlayerSettings(BuildTargetGroup.Android, buildType);

        IEnumerable<string> scenePaths = GetBuildScenePaths();
        BuildReport buildReport =
            BuildPipeline.BuildPlayer(scenePaths.ToArray(), outputPath, BuildTarget.Android, BuildOptions.None);

        OutputBuildResult(buildReport);

        // 設定を元に戻す
        PlayerSettings.productName = originalProductName;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, originalBundleId);
    }

    public static void BuildIOSProd()
    {
        BuildIOSInternal(BuildType.Prod);
    }

    public static void BuildIOSDev()
    {
        BuildIOSInternal(BuildType.Dev);
    }

    private static void BuildIOSInternal(BuildType buildType)
    {
        // 元の設定を保存
        string originalProductName = PlayerSettings.productName;
        string originalBundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);

        SetPlayerSettings(BuildTargetGroup.iOS, buildType);
        iOSBuild();
        OutputBuildResult(null); // iOSBuild内でBuildReportを返す場合は差し替え

        // 設定を元に戻す
        PlayerSettings.productName = originalProductName;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, originalBundleId);
    }

    private static void OutputBuildResult(BuildReport buildReport)
    {
        if (buildReport == null)
        {
            Debug.Log("ビルド完了（BuildReport未取得）");
            return;
        }

        BuildSummary summary = buildReport.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Success");
        }
        else
        {
            Debug.Log("Error");
        }
    }

    private static void SetPlayerSettings(BuildTargetGroup target, BuildType buildType)
    {
        string baseBundleId = target == BuildTargetGroup.Android ? "jp.genit.quizrobo" : "jp.genit.quizrobo";
        string bundleId = buildType == BuildType.Dev ? baseBundleId + ".dev" : baseBundleId;
        PlayerSettings.productName = buildType == BuildType.Dev ? "開発用" : "クイズロボ";
        PlayerSettings.SetApplicationIdentifier(target, bundleId);
    }

    public static void BuildSimulator()
    {
        try
        {
            // SimulatorビルドのためにTarget設定を変更
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
            BuildIOSDev();
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        }
        finally
        {
            // 必ず元のTarget設定に戻す
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        }
    }

    private static IEnumerable<string> GetBuildScenePaths()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        return scenes.Where(arg => arg.enabled).Select(arg => arg.path);
    }

    private static string GetParameterFrom(string key)
    {
        string[] args = Environment.GetCommandLineArgs();
        int index = args.ToList().FindIndex(arg => arg == key);
        int paramIndex = index + 1;

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

    private enum BuildType
    {
        Dev,
        Prod
    }
}
#endif
