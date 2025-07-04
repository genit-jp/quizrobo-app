using System;
using System.Collections.Generic; 
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CategoryDialog : DialogBaseListener
{
    [SerializeField] private RubyTextMeshProUGUI _description;
    [SerializeField] private GameObject _selectContainer;
    private Action<int, string> _onSelectedCategory;

    public async void Setup(int grade, Action<int, string> onSelectedCategory)
    {
        Debug.Log("grade: " + grade);
        _onSelectedCategory = onSelectedCategory;
        var categoryButtonObj = (GameObject)await Resources.LoadAsync("Prefabs/Select/CategoryButton");

        if (grade == 0)
        {
            _description.uneditedText = "<r=がくねん>学年</r>をえらんでね";
            for (int index = 1; index <= 6; index++)
            {
                int gradeIndex = index;
                var gameObj = Instantiate(categoryButtonObj, _selectContainer.transform, false);
                string text = $"{gradeIndex}年生";
                var categoryButton = gameObj.GetComponent<CategoryButton>();
                categoryButton.Setup(text, async () =>
                {
                    Vector4 blockerColor = new Color(255f / 255f, 246f / 255f, 230f / 255f, 1.0f);
                    var categoryDialogObj = await Genit.Utils.OpenDialog("Prefabs/Select/CategoryDialog", transform, blockerColor);
                    var categoryDialog = categoryDialogObj.GetComponent<CategoryDialog>();
                    categoryDialog.Setup(gradeIndex, _onSelectedCategory);
                });
            }
        }
        else
        {
            _description.uneditedText = "<r=きょうか>教科</r>をえらんでね";

            // 教科リストをgradeに応じて動的に選択
            Dictionary<string, string> subjectList;
            switch (grade)
            {
                case 1:
                    subjectList = new Dictionary<string, string> {{"こくご", "国語"}, {"さんすう", "算数"}};
                    break;
                case 2:
                    subjectList = new Dictionary<string, string> {{"国語", "国語"}, {"算数", "算数"}};
                    break;
                case 3:
                    subjectList = new Dictionary<string, string> {{"国語", "国語"}, {"算数", "算数"}, {"理科", "理科"}, {"社会", "社会"}};
                    break;
                case 4:
                    subjectList = new Dictionary<string, string> {{"国語", "国語"}, {"算数", "算数"}, {"理科", "理科"}, {"社会", "社会"}};
                    break;
                case 5:
                    subjectList = new Dictionary<string, string> {{"国語", "国語"}, {"算数", "算数"}, {"理科", "理科"}, {"社会", "社会"}};
                    break;
                case 6:
                    subjectList = new Dictionary<string, string> {{"国語", "国語"}, {"算数", "算数"}, {"理科", "理科"}, {"社会", "社会"}};
                    break;
                default:
                    subjectList = new Dictionary<string, string>(); // 既定値またはエラー処理
                    break;
            }

            foreach (var item in subjectList)
            {
                var gameObj = Instantiate(categoryButtonObj, _selectContainer.transform, false);
                var categoryButton = gameObj.GetComponent<CategoryButton>();
                string displayText = item.Key;
                string internalValue = item.Value;
                categoryButton.Setup(displayText, async () =>
                {
                    Debug.Log("grade: " + grade + " subject: " + internalValue);
                    _onSelectedCategory?.Invoke(grade, internalValue);
                });
            }
        }
    }

    public override bool OnClickBlocker()
    {
        return false;
    }
}
