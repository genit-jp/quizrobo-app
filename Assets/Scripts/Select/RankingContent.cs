// using System;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityEngine.UI;
//
// public class RankingContent: MonoBehaviour
// {
//     [SerializeField] private RubyTextMeshProUGUI _nameText;
//     [SerializeField] private Text _rankText;
//     
//     public void Setup(UserDataManager.UserData userData, int rank)
//     {
//         if(!String.IsNullOrEmpty(userData.playerName))
//         {
//             _nameText.uneditedText = userData.playerName;
//         }
//         else
//         {
//             _nameText.uneditedText = "ゲスト";
//         }
//         if (rank != -1)
//         {
//             _rankText.text = rank.ToString();
//         }
//         else
//         {
//             _rankText.text = "--";
//         }
//     }
// }