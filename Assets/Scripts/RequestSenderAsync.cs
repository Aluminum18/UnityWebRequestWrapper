using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using Cysharp.Threading.Tasks;

public class RequestSenderAsync
{
    public static async UniTask<T> SendGetRequest<T>(string url, string token = null, params Tuple<string, string>[] customHeader)
    {
        return await SendRequestAsync<T>(url, RequestType.GET, null, token, customHeader);
    }

    public static async UniTask<T> SendPostRequest<T>(string url, object data = null, string token = null, params Tuple<string, string>[] customHeader)
    {
        string dataStr = JsonUtility.ToJson(data);
        return await SendRequestAsync<T>(url, RequestType.POST, dataStr, token, customHeader);
    }

    public static async UniTask SendPostRequest(string url, object data = null, string token = null, params Tuple<string, string>[] customHeader)
    {
        string dataStr = JsonUtility.ToJson(data);
        await SendRequestAsync(url, RequestType.POST, dataStr, token, customHeader);
    }

    private static async UniTask<T> SendRequestAsync<T>(string url, RequestType requestType, string data, string token, Tuple<string, string>[] customHeader)
    {
        UnityWebRequest request = RequestHelper.GetUnityWebRequest(url, requestType, data, token, customHeader);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Request [{url}] got error, error code [{request.responseCode}]");
            return default;
        }

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError($"Request [{url}] got error, cannot connect to server");
            return default;
        }

        return JsonUtility.FromJson<T>(request.downloadHandler.text);
    }

    private static async UniTask SendRequestAsync(string url, RequestType requestType, string data, string token, Tuple<string, string>[] customHeader)
    {
        UnityWebRequest request = RequestHelper.GetUnityWebRequest(url, requestType, data, token, customHeader);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Request [{url}] got error, error code [{request.responseCode}]");
            return;
        }

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError($"Request [{url}] got error, cannot connect to server");
            return;
        }
    }
}