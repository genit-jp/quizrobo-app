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
        if (IsDebug)
        {
            // 開発モードでは常に指定のURLを返す
            return "https://script.google.com/macros/s/AKfycbzlwBXUxsVAi2KoXyIjvjzimSW5n_JZc5WjvdTZUmEnNWuM-Hln6aRc2p8ykqFeVqJH/exec";
        }

        
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
            if (IsDebug)
            {
                return "https://script.google.com/macros/s"; 
            }


            return "https://s3.ap-northeast-1.amazonaws.com/kidsquiz.assets"; //開発用(仮)
        }
    }
    
    // public static string MASTER_JSON_URL => MakeApiUrl("/master-0.6.0.json");
    
    public static string GetMasterJsonUrl(string masterVersion)
    {
        if (IsDebug)
        {
            return "https://script.google.com/macros/s/AKfycbzlwBXUxsVAi2KoXyIjvjzimSW5n_JZc5WjvdTZUmEnNWuM-Hln6aRc2p8ykqFeVqJH/exec";
        }

        if (masterVersion == "0.6.0")
        {
            return MakeApiUrl("/master-0.6.0.json");
        }
        else
        {
            return MakeApiUrl("/master.json");
        }
    }
    
    public static readonly Dictionary<string, string> DEFAULT_CHALLENGE_LEVELS = new()
    {
        { "Japanese", "easy" },
        { "Math", "easy" },
        { "Science", "easy" },
        { "SocialStudies", "easy" },
        { "English", "easy" }
    };
    
    public static readonly Dictionary<string, string> DIFFICULTY_NAME_MAP = new Dictionary<string, string>
    {
        {"easy", "かんたん"},
        {"normal", "ふつう"},
        {"hard", "むずかしい"}
    };
    
    public static readonly Dictionary<string, string> SUBJECT_NAME_MAP = new()
    {
        { "国語", "Japanese" },
        { "算数", "Math" },
        { "理科", "Science" },
        { "社会", "SocialStudies" },
        { "英語", "English" }
    };
    
    public static readonly Dictionary<string, string> PART_NAMES = new Dictionary<string, string>
    {
        { "Head", "あたま" },
        { "Body", "からだ" },
        { "Arms", "うで" },
        { "Legs", "あし" },
        { "Tail", "しっぽ" }
    };
    
    public static readonly Dictionary<string, string> DefaultPartImageMap = new Dictionary<string, string>
    {
        { "あたま", "default_head" },
        { "からだ", "default_body" },
        { "うで",   "default_arms" },
        { "あし",   "default_legs" },
        { "しっぽ", "default_tail" }
    };
    
    public static readonly Dictionary<char, int> enemyHpTable = new()
    {
        { 'A', 10 },
        { 'B', 60 },
        { 'C', 360 },
        { 'D', 2000 }
    };
    
    public static class GameSceneParam
    {
        public static int ChapterNumber;
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
    
    
    public static string MasterVersion = "master_version";
}