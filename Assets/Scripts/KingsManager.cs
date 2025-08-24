using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class KingsManager : MonoBehaviour
{
    public List<ShadowText> kingTexts;

    private string sheetId = "1tsL6ZFkJv1CmdJY4UqX2SYtt_I1tBje2KwVvVbhUqko";
    private string range = "A1:A10";
    private string apiKey = "AIzaSyBvCu16k9h7uMWE2_tf_vT7kSq-GvgVSEs";

    void Start()
    {
        RefreshKings();
    }
    
    // Call this to start refreshing kings every 60 seconds
    public void RefreshKings()
    {
        StartCoroutine(PeriodicKingsRefreshCoroutine());
    }

    private IEnumerator PeriodicKingsRefreshCoroutine()
    {
        while (true)
        {
            LoadKingsFromGoogleSheet();
            yield return new WaitForSeconds(60f);
        }
    }
    

    // Call this method to load king names from a Google Sheet
    public void LoadKingsFromGoogleSheet()
    {
        StartCoroutine(FetchKingsCoroutine(sheetId, range, apiKey));
    }

    private IEnumerator FetchKingsCoroutine(string sheetId, string range, string apiKey)
    {
        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{sheetId}/values/{range}?key={apiKey}";
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to fetch kings: {www.error}");
                yield break;
            }

            // Manually parse the 'values' array from the JSON response
            var json = www.downloadHandler.text;
            var values = new List<string>();
            var matches = System.Text.RegularExpressions.Regex.Matches(json, @"\[\s*""(.*?)""\s*\]");
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    values.Add(match.Groups[1].Value);
                }
            }
            
            for (int i = 0; i < kingTexts.Count; i++)
            {
                ShadowText thisText = kingTexts[i];
                thisText.SetText("???");
                thisText.SetDull();

                if (values.Count > i && values[i].ToString().Length > 0)
                {
                    thisText.SetText(values[i].Substring(0, 3).ToUpper());
                    thisText.SetBright();
                }
            }
        }
    }

    // Helper class for Google Sheets API response
    [System.Serializable]
    private class GoogleSheetResponse
    {
        public List<List<string>> values;
    }
}
