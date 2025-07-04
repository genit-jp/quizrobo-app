using TMPro;
using UnityEngine;

public class UIFlicker : MonoBehaviour
{
    //RubyTextMeshProUGUIのalpha値を変化させるスクリプト(https://zenn.dev/daichi_gamedev/articles/62db8cf6765a26)
    
    public float speed = 1.0f;

    private RubyTextMeshProUGUI tapToStartText;
    private float time;
    
    void Start()
    {
        tapToStartText = this.gameObject.GetComponent<RubyTextMeshProUGUI>();
    }
    
    void Update()
    {
        tapToStartText.color = GetAlphaColor(tapToStartText.color);
    }
    
    Color GetAlphaColor(Color color)
    {
        time += Time.deltaTime * 5.0f * speed;
        color.a = Mathf.Sin(time) * 0.5f + 0.5f;

        return color;
    }
}