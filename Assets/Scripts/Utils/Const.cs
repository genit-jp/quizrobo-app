using System.Collections.Generic;
using UnityEngine;
public class Const
{
    public static bool IsDebug
    {
        get
        {
            if (Application.identifier.Contains("dev"))
            {
                return true;
            }
#if UNITY_DEBUG || UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }
    
    public static string MakeApiUrl(string path)
    {
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }

        var API_VERSION = "/v1";
        
        // if (isDebug)
        // {
        //     API_VERSION = "/dev";
        // }

        return HOSTNAME + API_VERSION + path;
    }
    
    public static string HOSTNAME
    {
        get
        {
            // if (isDebug)
            // {
            //     return "https://s3.ap-northeast-1.amazonaws.com/"; 
            // }

            return "https://s3.ap-northeast-1.amazonaws.com/kidsquiz.assets"; //開発用(仮)
        }
    }
    
    // public static string MASTER_JSON_URL => MakeApiUrl("/master-0.6.0.json");
    
    public static string GetMasterJsonUrl(string masterVersion)
    {
        if (masterVersion == "0.6.0")
        {
            return MakeApiUrl("/master-0.6.0.json");
        }
        else
        {
            return MakeApiUrl("/master.json");
        }
    }
    
    public static Dictionary<int, Dictionary<string, int>> 
        SubjectRatioInGoodQuizFor060 = new Dictionary<int, Dictionary<string, int>>()
    {
        {1, new Dictionary<string, int>()
        {
            {"音楽", 20},
            {"算数", 20},
            {"国語", 20},
            {"その他", 40},
        }},
        {2, new Dictionary<string, int>()
        {
            {"音楽", 30},
            {"算数", 20},
            {"国語", 20},
            {"その他", 30},
        }},
        {3, new Dictionary<string, int>()
        {
            {"音楽", 30},
            {"国語", 10},
            {"社会", 10},
            {"理科", 30},
            {"その他", 20},
        }},
        {4, new Dictionary<string, int>()
        {
            {"音楽", 30},
            {"国語", 10},
            {"社会", 10},
            {"理科", 30},
            {"その他", 20},
        }},
        {5, new Dictionary<string, int>()
        {
            {"音楽", 20},
            {"国語", 10},
            {"社会", 10},
            {"理科", 20},
            {"英語", 20},
            {"その他", 20},
        }},
        {6, new Dictionary<string, int>()
        {
            {"音楽", 20},
            {"国語", 10},
            {"社会", 10},
            {"理科", 20},
            {"英語", 20},
            {"その他", 20},
        }},
    };
    
    public static Dictionary<int, Dictionary<string, int>> 
        SubjectRatioInGoodQuizFor120 = new Dictionary<int, Dictionary<string, int>>()
        {
            {1, new Dictionary<string, int>()
            {
                {"雑学", 20},
                {"算数", 20},
                {"国語", 20},
                {"その他", 40},
            }},
            {2, new Dictionary<string, int>()
            {
                {"雑学", 30},
                {"算数", 20},
                {"国語", 20},
                {"その他", 30},
            }},
            {3, new Dictionary<string, int>()
            {
                {"雑学", 30},
                {"国語", 10},
                {"社会", 10},
                {"理科", 30},
                {"その他", 20},
            }},
            {4, new Dictionary<string, int>()
            {
                {"雑学", 30},
                {"国語", 10},
                {"社会", 10},
                {"理科", 30},
                {"その他", 20},
            }},
            {5, new Dictionary<string, int>()
            {
                {"雑学", 20},
                {"国語", 10},
                {"社会", 10},
                {"理科", 20},
                {"英語", 20},
                {"その他", 20},
            }},
            {6, new Dictionary<string, int>()
            {
                {"雑学", 20},
                {"国語", 10},
                {"社会", 10},
                {"理科", 20},
                {"英語", 20},
                {"その他", 20},
            }},
        };
    
    public static Dictionary<int, List<string>> OthersSubjects = new Dictionary<int, List<string>>
    {
        {1, new List<string>(){"保険・体育", "図書", "図工", "時事", "情報", "家庭科", "生活", "なぞなぞ"}},
        {2, new List<string>(){"保険・体育", "図書", "図工", "時事", "情報", "家庭科", "生活", "なぞなぞ"}},
        {3, new List<string>(){"保険・体育", "図書", "図工", "時事", "情報", "家庭科", "なぞなぞ"}},
        {4, new List<string>(){"図書", "図工", "時事", "情報", "家庭科", "なぞなぞ"}},
        {5, new List<string>(){"図書", "図工", "時事", "情報", "家庭科", "なぞなぞ"}},
        {6, new List<string>(){"図書", "図工", "時事", "情報", "家庭科", "なぞなぞ"}},
    };
    
    public static Dictionary<int, List<int>> 
        StageToLevelRatio = new Dictionary<int, List<int>>()
        {
            {1, new List<int>(){50, 50, 0, 0, 0}},
            {2, new List<int>(){50, 30, 20, 0, 0}},
            {3, new List<int>(){40, 40, 20, 0, 0}},
            {4, new List<int>(){20, 45, 25, 10, 0}},
            {5, new List<int>(){15, 35, 40, 10, 0}},
            {6, new List<int>(){15, 20, 40, 15, 10}},
            {7, new List<int>(){10, 20, 30, 20, 20}},
        };
    
    public enum PlayMode
    {
        Normal,
        Calculation,
        Subject,
    }
    
    public static string MasterVersion = "master_version";
}