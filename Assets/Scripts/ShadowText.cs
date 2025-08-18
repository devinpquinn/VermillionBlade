using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShadowText : MonoBehaviour
{
    public List<TextMeshProUGUI> texts;
    
    public void SetText(string text)
    {
        foreach (var textComponent in texts)
        {
            textComponent.text = text;
        }
    }
}
