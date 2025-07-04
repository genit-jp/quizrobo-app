using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;

public class LocalizationsPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.iOS)
        {
            AddLanguage(path, "ja");
            // 日本語のみの場合の例.
//            AddLanguage(path, "ja");
        }
    }

    /// <summary>
    /// 言語を追加.
    /// </summary>
    public static void AddLanguage(string path, params string[] languages)
    {
        var plistPath = System.IO.Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();

        plist.ReadFromFile(plistPath);

        var localizationKey = "CFBundleLocalizations";

        var localizations = plist.root.values
            .Where(kv => kv.Key == localizationKey)
            .Select(kv => kv.Value)
            .Cast<PlistElementArray>()
            .FirstOrDefault();

        if (localizations == null)
        {
            localizations = plist.root.CreateArray(localizationKey);
        }

        foreach (var language in languages)
        {
            if (localizations.values.Select(el => el.AsString()).Contains(language) == false)
            {
                localizations.AddString(language);
            }
        }

        plist.WriteToFile(plistPath);
    }
}

#endif