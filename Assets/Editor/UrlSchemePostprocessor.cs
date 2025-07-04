using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

/// <summary>
/// Unity iOSビルド後にUrlSchemeを追加する為のPostprocessor
/// </summary>
public class UrlSchemePostprocessor
{
    /// <summary>
    /// ビルド後の処理( Android / iOS共通 )
    /// </summary>
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        // iOSじゃなければ処理しない
        if (target != BuildTarget.iOS)
        {
            return;
        }

        AddUrlScheme(path);
    }

    /// <summary>
    /// URLスキームを追加します
    /// </summary>
    /// <param name="path"> 出力先のパス </param>
    public static void AddUrlScheme(string path)
    {
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();

        // 読み込み
        plist.ReadFromFile(plistPath);

        var urlTypes = plist.root.CreateArray("CFBundleURLTypes");
        var dict = urlTypes.AddDict();
        dict.SetString("CFBundleURLName", "jp.genit.musicquiz");
        var urlSchemes = dict.CreateArray("CFBundleURLSchemes");
        urlSchemes.AddString("melody");

        plist.root.SetBoolean("FirebaseAppStoreReceiptURLCheckEnabled", false);

        // 書き込み
        plist.WriteToFile(plistPath);
    }
}
