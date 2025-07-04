using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

/// <summary>
/// Unity iOSビルド後にUrlSchemeを追加する為のPostprocessor
/// </summary>
public class SKAddNetworkPostProcessor
{
    private const string KEY_SK_ADNETWORK_ITEMS = "SKAdNetworkItems";

    private const string KEY_SK_ADNETWORK_ID = "SKAdNetworkIdentifier";

    private static string[] skAdNetworkIds =
    {
        "22mmun2rn5.skadnetwork",
        "238da6jt44.skadnetwork",
        "23zd986j2c.skadnetwork",
        "24t9a8vw3c.skadnetwork",
        "252b5q8x7y.skadnetwork",
        "275upjj5gd.skadnetwork",
        "2fnua5tdw4.skadnetwork",
        "2U9PT9HC89.skadnetwork",
        "32z4fx6l9h.skadnetwork",
        "3qcr597p9d.skadnetwork",
        "3qy4746246.skadnetwork",
        "3rd42ekr43.skadnetwork",
        "3sh42y64q3.skadnetwork",
        "424m5254lk.skadnetwork",
        "4468km3ulz.skadnetwork",
        "44jx6755aq.skadnetwork",
        "44n7hlldy6.skadnetwork",
        "47vhws6wlr.skadnetwork",
        "488r3q3dtq.skadnetwork",
        "4dzt52r2t5.skadnetwork",
        "4fzdc2evr5.skadnetwork",
        "4mn522wn87.skadnetwork",
        "4pfyvq9l8r.skadnetwork",
        "523jb4fst2.skadnetwork",
        "52fl2v3hgk.skadnetwork",
        "54nzkqm89y.skadnetwork",
        "5594blyghf.skadnetwork",
        "578prtvx9j.skadnetwork",
        "5a6flpkh64.skadnetwork",
        "5l3tpt7t6e.skadnetwork",
        "5lm9lj6jb7.skadnetwork",
        "5tjdwbrq8w.skadnetwork",
        "6g9af3uyq4.skadnetwork",
        "6xzpu9s2p8.skadnetwork",
        "737z793b9f.skadnetwork",
        "7953jerfzd.skadnetwork",
        "7rz58n8ntl.skadnetwork",
        "7ug5zh24hu.skadnetwork",
        "866k9ut3g3.skadnetwork",
        "89z7zv988g.skadnetwork",
        "8m87ys6875.skadnetwork",
        "8s468mfl3y.skadnetwork",
        "97r2b46745.skadnetwork",
        "9b89h5y424.skadnetwork",
        "9nlqeag3gk.skadnetwork",
        "9rd848q2bz.skadnetwork",
        "9t245vhmpl.skadnetwork",
        "9yg77x724h.skadnetwork",
        "av6w8kgt66.skadnetwork",
        "bvpn9ufa9b.skadnetwork",
        "c6k4g5qg8m.skadnetwork",
        "cg4yq2srnc.skadnetwork",
        "cj5566h2ga.skadnetwork",
        "cstr6suwn9.skadnetwork",
        "dzg6xy7pwj.skadnetwork",
        "e5fvkxwrpn.skadnetwork",
        "ecpz2srf59.skadnetwork",
        "eh6m2bh4zr.skadnetwork",
        "ejvt5qm6ak.skadnetwork",
        "f38h382jlk.skadnetwork",
        "f73kdq92p3.skadnetwork",
        "f7s53z58qe.skadnetwork",
        "feyaarzu9v.skadnetwork",
        "g28c52eehv.skadnetwork",
        "ggvn48r87g.skadnetwork",
        "glqzh8vgby.skadnetwork",
        "gta9lk7p23.skadnetwork",
        "gvmwg8q7h5.skadnetwork",
        "hb56zgv37p.skadnetwork",
        "hdw39hrw9y.skadnetwork",
        "hs6bdukanm.skadnetwork",
        "k674qkevps.skadnetwork",
        "kbd757ywx3.skadnetwork",
        "klf5c3l5u5.skadnetwork",
        "lr83yxwka7.skadnetwork",
        "ludvb6z3bs.skadnetwork",
        "m5mvw97r93.skadnetwork",
        "m8dbw4sv7c.skadnetwork",
        "mlmmfzh3r3.skadnetwork",
        "mls7yz5dvl.skadnetwork",
        "mtkv5xtk9e.skadnetwork",
        "n38lu8286q.skadnetwork",
        "n66cz3y3bx.skadnetwork",
        "n6fk4nfna4.skadnetwork",
        "n9x2a789qt.skadnetwork",
        "nzq8sh4pbs.skadnetwork",
        "p78axxw29g.skadnetwork",
        "ppxm28t8ap.skadnetwork",
        "prcb7njmu6.skadnetwork",
        "pu4na253f3.skadnetwork",
        "pwa73g5rt2.skadnetwork",
        "rvh3l7un93.skadnetwork",
        "rx5hdcabgc.skadnetwork",
        "s39g8k73mm.skadnetwork",
        "su67r6k2v3.skadnetwork",
        "t38b2kh725.skadnetwork",
        "tl55sbb4fm.skadnetwork",
        "u679fj5vs4.skadnetwork",
        "uw77j35x4d.skadnetwork",
        "v4nxqhlyqp.skadnetwork",
        "v72qych5uu.skadnetwork",
        "v79kvwwj4g.skadnetwork",
        "v9wttpbfk9.skadnetwork",
        "vutu7akeur.skadnetwork",
        "w9q455wk68.skadnetwork",
        "wg4vff78zm.skadnetwork",
        "wzmmz9fp6w.skadnetwork",
        "x44k69ngh6.skadnetwork",
        "x8uqf25wch.skadnetwork",
        "xy9t38ct57.skadnetwork",
        "y45688jllp.skadnetwork",
        "YCLNXRL5PM.skadnetwork",
        "ydx93a7ass.skadnetwork",
        "yrqqpx2mcb.skadnetwork",
        "z4gj7hsk7h.skadnetwork",
        "zmvfpc5aq8.skadnetwork",
    };

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

        var plistPath = Path.Combine(path, "Info.plist");
        var document = new PlistDocument();

        // 読み込み
        document.ReadFromFile(plistPath);

        PlistElementArray array = GetSKAdNetworkItemsArray(document);
        if (array != null)
        {
            foreach (string id in skAdNetworkIds)
            {
                if (!ContainsSKAdNetworkIdentifier(array, id))
                {
                    PlistElementDict added = array.AddDict();
                    added.SetString(KEY_SK_ADNETWORK_ID, id);
                }
            }
        }

        File.WriteAllText(plistPath, document.WriteToString());
    }

    private static bool ContainsSKAdNetworkIdentifier(PlistElementArray skAdNetworkItemsArray, string id)
    {
        foreach (PlistElement elem in skAdNetworkItemsArray.values)
        {
            try
            {
                PlistElementDict elemInDict = elem.AsDict();
                PlistElement value;
                bool identifierExists = elemInDict.values.TryGetValue(KEY_SK_ADNETWORK_ID, out value);

                if (identifierExists && value.AsString().Equals(id))
                {
                    return true;
                }
            }
#pragma warning disable 0168
            catch (Exception e)
#pragma warning restore 0168
            {
                // Do nothing
            }
        }

        return false;
    }

    private static PlistElementArray GetSKAdNetworkItemsArray(PlistDocument document)
    {
        PlistElementArray array;
        if (document.root.values.ContainsKey(KEY_SK_ADNETWORK_ITEMS))
        {
            try
            {
                PlistElement element;
                document.root.values.TryGetValue(KEY_SK_ADNETWORK_ITEMS, out element);
                array = element.AsArray();
            }
#pragma warning disable 0168
            catch (Exception e)
#pragma warning restore 0168
            {
                // The element is not an array type.
                array = null;
            }
        }
        else
        {
            array = document.root.CreateArray(KEY_SK_ADNETWORK_ITEMS);
        }

        return array;
    }
}