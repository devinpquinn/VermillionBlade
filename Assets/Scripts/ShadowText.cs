using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShadowText : MonoBehaviour
{
    public List<TextMeshProUGUI> texts;

    private Color brightBackground = new Color(0.4862745f, 0.09411766f, 0.2352941f, 1f);
    private Color brightForeground = new Color(0.8352942f, 0.2352941f, 0.4156863f, 1f);
    private Color dullBackground = new Color(0.2745098f, 0.05490196f, 0.1686275f, 1f);
    private Color dullForeground = new Color(0.4862745f, 0.09411766f, 0.2352941f, 1f);

    public void SetText(string text)
    {
        foreach (TextMeshProUGUI textComponent in texts)
        {
            textComponent.text = text;
        }
    }
    
    public void SetBright()
    {
        texts[0].color = brightBackground;
        texts[1].color = brightForeground;
    }

    public void SetDull()
    {
        texts[0].color = dullBackground;
        texts[1].color = dullForeground;
    }
}
