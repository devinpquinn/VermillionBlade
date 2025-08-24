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
        LoadKingsFromGoogleSheet();
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

            Debug.Log($"Fetched kings: {www.downloadHandler.text}");

            // Parse JSON response
            var json = www.downloadHandler.text;
            var sheetData = JsonUtility.FromJson<GoogleSheetResponse>(json);
            if (sheetData != null && sheetData.values != null)
            {
                for (int i = 0; i < kingTexts.Count && i < sheetData.values.Count; i++)
                {
                    // Assuming each row is a list of strings, and king name is in the first column
                    // TODO: Replace with correct method/property for ShadowText
                    kingTexts[i].SetText(sheetData.values[i][0]);
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
