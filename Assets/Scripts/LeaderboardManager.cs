using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class LeaderboardManager : MonoBehaviour
{
    private string backendUrl = "https://unity-sheets-backend.vercel.app/api/sheets";

    public void AppendToColumnA(string value)
    {
        StartCoroutine(SendToSheet(value));
    }

    private IEnumerator SendToSheet(string value)
    {
        SheetRowData data = new SheetRowData(value);
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to write to sheet: " + request.error);
            }
            else
            {
                Debug.Log("Successfully added: " + value);
            }
        }
    }

    public void ReadFirst10ColumnA(Action<List<string>> onComplete)
    {
        StartCoroutine(ReadSheetCoroutine(onComplete));
    }

    private IEnumerator ReadSheetCoroutine(Action<List<string>> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(backendUrl))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to read from sheet: " + request.error);
                onComplete?.Invoke(new List<string>());
            }
            else
            {
                string json = request.downloadHandler.text;
                SheetReadResponse response = JsonUtility.FromJson<SheetReadResponse>(json);
                onComplete?.Invoke(response.values ?? new List<string>());
            }
        }
    }

    [Serializable]
    private class SheetRowData
    {
        public string[] values;
        public SheetRowData(string firstColumn)
        {
            values = new string[] { firstColumn };
        }
    }

    [Serializable]
    private class SheetReadResponse
    {
        public List<string> values;
    }
}
