//using System;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = System.Random;
#if UNITY_ANDROID
using UnityEngine.Networking;
#endif

namespace Genit
{
    public static class Utils
    {
        private static readonly Random _random = new();

        private static Vector3 TouchPosition = Vector3.zero;

        public static int Rand(int max)
        {
            return _random.Next(0, max);
        }

        public static bool Rand(float percent)
        {
            var threashold = (int)(100.0f * percent);
            return _random.Next(0, 100) < threashold;
        }

        public static float SquaredDistance(Vector3 mousePos, double intersect_x, double intersect_y)
        {
            var dx = mousePos.x - intersect_x;
            var dy = mousePos.y - intersect_y;
            var squaredDistance = (dx * dx) + (dy * dy);
            return (float)squaredDistance;
        }

        public static string MakeStorageImagePath(string imgName)
        {
            if (!imgName.StartsWith("/"))
            {
                imgName = "/" + imgName;
            }

            return Application.persistentDataPath + imgName;
        }

        public static string MakeStreamingAssetsPath(string filename)
        {
            if (filename.StartsWith("/"))
            {
                filename = filename.Substring(1);
            }

            return Path.Combine(Application.streamingAssetsPath, filename);
        }
        

        public static string MakeTempFilePath(string filename)
        {
            if (filename.StartsWith("/"))
            {
                filename = filename.Substring(1);
            }

            return Path.Combine(Application.temporaryCachePath, filename);
        }

        public static string MakeTempDir()
        {
            return Application.temporaryCachePath;
        }

        public static string GenerateRandomString(int length)
        {
            var txt = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new Random();

            var result = "";

            for (var i = 0; i < length; i++)
            {
                result += txt[random.Next(txt.Length)]; // 文字数の範囲内で乱数を生成
            }

            return result;
        }

        public static string GetImagePath(string imgName)
        {
            var stragePath = MakeStorageImagePath(imgName);
            if (File.Exists(stragePath))
            {
                return stragePath;
            }

            var assetsPath = MakeStreamingAssetsPath(imgName);
            if (File.Exists(assetsPath))
            {
                return assetsPath;
            }

            return null;
        }

        public static async UniTask<byte[]> ReadAllBytes(string path)
        {
#if UNITY_ANDROID
        if (path.Contains("://"))
        {
            var www = UnityWebRequest.Get(path);
            await www.SendWebRequest();
            return www.downloadHandler.data;
        }
#endif
            if (path.StartsWith("http"))
            {
                var www = UnityWebRequest.Get(path);
                await www.SendWebRequest();
                return www.downloadHandler.data;
            }

            byte[] result;
            using (var SourceStream = new FileStream(
                       path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                result = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
            }

            return result;
        }


        public static async UniTask<byte[]> LoadLocalImage(string songImage)
        {
            var imagePath = GetImagePath(songImage);
            if (imagePath == null)
            {
                return null;
            }

            var byteData = await ReadAllBytes(imagePath);
            return byteData;
        }

        public static Texture2D ByteToTexture(byte[] byteData)
        {
            var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            texture.LoadImage(byteData);
            return texture;
        }

        public static void FixFontSize(Text text, float maxWidth)
        {
            var textWidth = text.preferredWidth;

            if (maxWidth < textWidth)
            {
                var fixNum = maxWidth / textWidth;

                text.GetComponent<RectTransform>().localScale = new Vector3(fixNum, 1.0f, 1.0f);
            }
        }

        public static long Clamp(long src, long min, long max)
        {
            var ret = src;
            if (src < min)
            {
                ret = min;
            }

            if (src > max)
            {
                ret = max;
            }

            return ret;
        }

        public static void MultiplyLocalScale(Transform transform, Vector3 vec3)
        {
            var scale = transform.localScale;
            transform.localScale = new Vector3(scale.x * vec3.x, scale.y * vec3.y, scale.z * vec3.z);
        }

        public static float LimitVal(float val, float min, float max)
        {
            if (val < min)
            {
                return min;
            }

            if (val > max)
            {
                return max;
            }

            return val;
        }

        public static void DestroyAllChildren(Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        /// <summary>
        ///     ダイアログにアニメーションを追加
        /// </summary>
        /// <param name="dialogPrefab">ダイアログのプレハブ名</param>
        /// <param name="parentTransform">アニメーションプレハブをセットする親オブジェクト</param>
        /// <return name="childObj">ダイアログオブジェクト</return>
        public static async UniTask<GameObject> OpenDialog(string dialogPrefab, Transform parentTransform, Color? color = null)
        {
            
            // アニメーションプレハブを生成し、親オブジェクトにセット
            var openDialogPrefabObj = (GameObject)await Resources.LoadAsync("Prefabs/Common/DialogBase");
            var parentObj =
                Object.Instantiate(openDialogPrefabObj, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            parentObj.transform.SetParent(parentTransform, false);

            // ダイアログプレハブを生成し、アニメーションプレハブにセット
            var dialogPrefabObj = (GameObject)await Resources.LoadAsync(dialogPrefab);
            var childObj = Object.Instantiate(dialogPrefabObj, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            dialogPrefabObj.transform.localPosition = new Vector3();
            var dialog = parentObj.transform.Find("Dialog");
            childObj.transform.SetParent(dialog.transform, false);

            var dialogBase = parentObj.GetComponent<DialogBase>();
            if (color != null)
            {
                dialogBase.SetColor(color.Value);
            }

            dialogBase.Closable = true;
            return childObj;
        }

        public static DirectoryInfo SafeCreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return null;
            }

            return Directory.CreateDirectory(path);
        }


        public static async UniTask<GameObject> InstantiatePrefab(string prefabPath, Transform parent = null)
        {
            var resource = (GameObject)await Resources.LoadAsync(prefabPath);
            if (resource == null)
            {
                Debug.LogError($"Prefab Resources load failed({prefabPath})");
            }

            var gameObject = Object.Instantiate(resource, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            if (parent)
            {
                gameObject.transform.SetParent(parent, false);
            }

            return gameObject;
        }

        public static async UniTask<GameObject> GetPrefabResource(string prefabPath)
        {
            return (GameObject)await Resources.LoadAsync(prefabPath);
        }

        public static GameObject InstantiatePrefabFromResource(GameObject resource, Transform parent = null)
        {
            if (resource == null)
            {
                Debug.LogError("Prefab Resources load failed");
            }

            var gameObject = Object.Instantiate(resource, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            if (parent)
            {
                gameObject.transform.SetParent(parent, false);
            }

            return gameObject;
        }

        public static Transform Clear(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }

            return transform;
        }

        public static Transform SetX(this Transform transform, float x)
        {
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
            return transform;
        }

        public static Transform SetY(this Transform transform, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
            return transform;
        }

        public static void AdjustScreenArea(RectTransform panel, Rect safeArea)
        {
            if (safeArea.x != 0)
            {
                var fixScreenWidth = safeArea.width / Screen.width;
                panel.localScale = new Vector3(fixScreenWidth, fixScreenWidth, 1);
            }
        }

        public static int GetIntFromXmlElement(XElement element, string key, int defaultVal = 0)
        {
            var val = element.Descendants(key).FirstOrDefault();
            if (val != null)
            {
                return int.Parse(val.Value);
            }

            return defaultVal;
        }

        public static int? GetIntFromXmlElementOrNull(XElement element, string key)
        {
            var val = element.Descendants(key).FirstOrDefault();
            if (val != null)
            {
                return int.Parse(val.Value);
            }

            return null;
        }
        
        public static long DateTimeToUnixTime(DateTime dt)
        {
            var dto = new DateTimeOffset(dt.ToUniversalTime().Ticks, new TimeSpan(+00, 00, 00));
            return dto.ToUnixTimeSeconds();
        }

        public static DateTime UnixTimeToDateTime(long unixTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
        }

        public static bool IsVisibleUnixTime(DateTime now, int unixTime)
        {
            var checkTime = UnixTimeToDateTime(unixTime).ToUniversalTime();
            checkTime = new DateTime(checkTime.Year, checkTime.Month, checkTime.Day, 0, 0, 0, DateTimeKind.Utc);
            var nowUT = DateTimeToUnixTime(now);
            // 4時以降をOKとするのでUTC時間で午後19時なので、5時間足して0時にする
            nowUT += 5 * 60 * 60;
            return nowUT >= DateTimeToUnixTime(checkTime);
        }

        public static bool IsBetweenVisibleUnixDate(DateTime now, int startDate, int endDate)
        {
            var checkStartTime = UnixTimeToDateTime(startDate).ToUniversalTime();
            checkStartTime = new DateTime(checkStartTime.Year, checkStartTime.Month, checkStartTime.Day, 0, 0, 0,
                DateTimeKind.Utc);

            var checkEndTime = UnixTimeToDateTime(endDate + (24 * 60 * 60)).ToUniversalTime();
            checkEndTime = new DateTime(checkEndTime.Year, checkEndTime.Month, checkEndTime.Day, 0, 0, 0,
                DateTimeKind.Utc);

            var nowUT = DateTimeToUnixTime(now);
            // 4時以降をOKとするのでUTC時間で午後19時なので、5時間足して0時にする
            nowUT += 5 * 60 * 60;
            now = UnixTimeToDateTime(nowUT).ToUniversalTime();
            return nowUT >= DateTimeToUnixTime(checkStartTime) && nowUT < DateTimeToUnixTime(checkEndTime);
        }

        public static void ReplaceStringInFile(string file, string target, string replaceText)
        {
            if (!File.Exists(file))
            {
                Debug.LogError($"file open error:{file}");
                return;
            }

            // 置換後のテキスト
            var processedContents = "";

            using (var stream = new StreamReader(file))
            {
                while (stream.Peek() >= 0)
                {
                    var line = stream.ReadLine();
                    processedContents += line.Replace(target, replaceText) + "\n";
                }
            }

            // 既存ファイルを削除し、置換後のテキストで新規作成
            File.Delete(file);

            using (var stream = File.CreateText(file))
            {
                stream.Write(processedContents);
            }
        }

        public static string ZenToHan(string s)
        {
            return ZenToHanAlpha(ZenToHanNum(s));
        }

        public static string ZenToHanNum(string s)
        {
            return Regex.Replace(s, "[０-９]", p => ((char)(p.Value[0] - '０' + '0')).ToString());
        }

        public static string ZenToHanAlpha(string s)
        {
            var str = Regex.Replace(s, "[ａ-ｚ]", p => ((char)(p.Value[0] - 'ａ' + 'a')).ToString());

            return Regex.Replace(str, "[Ａ-Ｚ]", p => ((char)(p.Value[0] - 'Ａ' + 'A')).ToString());
        }

        public static int TryStringToInt(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException();
            }

            if (!Int32.TryParse(s, out var result))
            {
                throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public static float AttackThresholdValue(int val)
        {
            return val / 10.0f;
        }


        public static int CountCharacters(string text)
        {
            float halfWidth = 0;
            var fullWidth = 0;
            foreach (var c in text)
            {
                if (c >= ' ' && c <= '~') // 半角文字
                {
                    halfWidth++;
                }
                else // 全角文字
                {
                    fullWidth++;
                }
            }

            return Mathf.CeilToInt(halfWidth / 2.0f) + fullWidth;
        }

        public static int ConvertToMirrorKey(int key)
        {
            switch (key % 12)
            {
                case 0:
                    return 3;
                case 1:
                    return 2;
                case 2:
                    return 1;
                case 3:
                    return 0;
                default:
                    return 0;
            }
        }
        

        public static Sprite GetSpriteSkillMark(string userSkillRank)
        {
            return Resources.Load<Sprite>($"SkillMarks/{userSkillRank}");
        }
    }
    
}
