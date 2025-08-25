using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GoogleSheetsAppender : MonoBehaviour
{
    private string backendUrl = "https://unity-sheets-backend.vercel.app/api/sheets";

    void Start()
    {
        AppendToColumnA("Test Value");
    }

    /// <summary>
    /// Call this function to add a string to the next open cell in column A
    /// </summary>
    public void AppendToColumnA(string value)
    {
        StartCoroutine(SendToSheet(value));
    }

    private IEnumerator SendToSheet(string value)
    {
        // Prepare JSON body matching the backend format
        SheetRowData data = new SheetRowData(value);
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
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

    // Helper class to serialize JSON for the backend
    [System.Serializable]
    private class SheetRowData
    {
        public string[] values;

        public SheetRowData(string firstColumn)
        {
            values = new string[] { firstColumn };
        }
    }
}
