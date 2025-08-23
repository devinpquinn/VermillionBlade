using System.Collections;
using UnityEngine;

public class NameEntryManager : MonoBehaviour
{
    public ShadowText name_1;
    public ShadowText name_2;
    public ShadowText name_3;

    private char[] enteredLetters = new char[3];
    private int currentIndex = 0;
    private bool inputComplete = false;

    void Update()
    {
        if (inputComplete) return;

        if (currentIndex < 3)
        {
            // Check for letter key input
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (IsLetterKey(k) && Input.GetKeyDown(k))
                {
                    char letter = k.ToString()[0];
                    enteredLetters[currentIndex] = letter;
                    SetNameText(currentIndex, letter.ToString().ToUpper(), true);
                    currentIndex++;
                    break;
                }
            }
        }

        // Handle backspace
        if (Input.GetKeyDown(KeyCode.Backspace) && currentIndex > 0)
        {
            currentIndex--;
            enteredLetters[currentIndex] = '\0';
            SetNameText(currentIndex, "?", false);
        }

        // If all three letters entered, print name
        if (currentIndex == 3)
        {
            inputComplete = true;
            string name = new string(enteredLetters);
            EnterName(name);
        }
    }

    private bool IsLetterKey(KeyCode k)
    {
        // Accept only A-Z
        return k >= KeyCode.A && k <= KeyCode.Z;
    }

    private void SetNameText(int index, string text, bool bright)
    {
        switch (index)
        {
            case 0:
                name_1.SetText(text);
                if (bright) name_1.SetBright(); else name_1.SetDull();
                break;
            case 1:
                name_2.SetText(text);
                if (bright) name_2.SetBright(); else name_2.SetDull();
                break;
            case 2:
                name_3.SetText(text);
                if (bright) name_3.SetBright(); else name_3.SetDull();
                break;
        }
    }
    
    private void EnterName(string name)
    {
        Debug.Log($"Entered Name: {name}");
        PlayerPrefs.SetString("Name", name);
        
        StartCoroutine(WaitAndDisable());
    }
    
    IEnumerator WaitAndDisable()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
