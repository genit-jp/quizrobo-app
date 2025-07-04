using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class PostProcessiOS
{
    [PostProcessBuild(0)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.iOS)
        {
            var filePath = Path.Combine(pathToBuiltProject, "Classes/UnityAppController.mm");
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);

                // 一時的な置換文字列を使用して、直接の入れ替えを避ける
                var tempString = "TEMP_METHOD_NAME";
                text = text.Replace("applicationDidEnterBackground", tempString);
                text = text.Replace("applicationWillResignActive", "applicationDidEnterBackground");
                text = text.Replace(tempString, "applicationWillResignActive");

                File.WriteAllText(filePath, text);
                Debug.Log(
                    "Successfully swapped 'applicationDidEnterBackground' and 'applicationWillResignActive' in UnityAppController.mm");
            }
            else
            {
                Debug.LogError("UnityAppController.mm not found at path: " + filePath);
            }
        }
    }
}
