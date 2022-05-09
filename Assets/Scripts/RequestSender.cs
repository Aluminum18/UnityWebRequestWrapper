using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using Cysharp.Threading.Tasks;

public class RequestSender : MonoBehaviour
{
    public delegate void SendApiSuccessHandler<T>(T response);
    public delegate void SendApiSuccessHandler();
    public delegate void SendApiFailedHandler();
    public delegate void SendApiFailedByNetWorkHandler();

    

    /// <typeparam name="T">Type of response</typeparam>
    public void SendGetRequest<T>(string path,
                                SendApiSuccessHandler<T> successCallback,
                                SendApiFailedHandler failedCallback,
                                SendApiFailedByNetWorkHandler networkErrorCallback,
                                string token = null,
                                params Tuple<string, string>[] customHeader)
    {
        StartCoroutine(IE_SendRequest(path, RequestType.GET, successCallback, failedCallback, networkErrorCallback, null, token, customHeader));
    }

    public async UniTask<T> SendGetRequest<T>(string url, string token = null, params Tuple<string, string>[] customHeader)
    {
        return await SendRequestAsync<T>(url, RequestType.GET, null, token, customHeader);
    }

    public async UniTask<T> SendPostRequest<T>(string url, object data = null, string token = null, params Tuple<string, string>[] customHeader)
    {
        string dataStr = JsonUtility.ToJson(data);
        return await SendRequestAsync<T>(url, RequestType.POST, dataStr, token, customHeader);
    }

    /// <summary>
    /// Used this method when response data is not used
    /// </summary>
    public void SendPostRequest(string path,
                                SendApiSuccessHandler successCallback,
                                SendApiFailedHandler failedCallback,
                                SendApiFailedByNetWorkHandler networkErrorCallback,
                                object data = null,
                                string token = null,
                                params Tuple<string, string>[] customHeader)
    {
        string requestBody = JsonUtility.ToJson(data);
        StartCoroutine(IE_SendRequest(path, RequestType.POST, successCallback, failedCallback, networkErrorCallback, requestBody, token, customHeader));
    }

    /// <typeparam name="T">Type of response</typeparam>
    public void SendPostRequest<T>(string path,
                                SendApiSuccessHandler<T> successCallback,
                                SendApiFailedHandler failedCallback,
                                SendApiFailedByNetWorkHandler networkErrorCallback,
                                string data = null,
                                string token = null,
                                params Tuple<string, string>[] customHeader)
    {
        StartCoroutine(IE_SendRequest(path, RequestType.POST, successCallback, failedCallback, networkErrorCallback, data, token, customHeader));
    }

    /// <typeparam name="T">Type of response</typeparam>
    public void SendPostRequest<T>(string path,
                                SendApiSuccessHandler<T> successCallback,
                                SendApiFailedHandler failedCallback,
                                SendApiFailedByNetWorkHandler networkErrorCallback,
                                object data = null,
                                string token = null,
                                params Tuple<string, string>[] customHeader)
    {
        string requestBodyString = JsonUtility.ToJson(data);
        SendPostRequest(path, successCallback, failedCallback, networkErrorCallback, requestBodyString, token, customHeader);
    }


    private IEnumerator IE_SendRequest<T>(string path,
                                RequestType requestType,
                                SendApiSuccessHandler<T> successCallback,
                                SendApiFailedHandler failedCallback,
                                SendApiFailedByNetWorkHandler networkErrorCallback,
                                string data,
                                string token,
                                Tuple<string, string>[] customHeader)
    {
        using var request = RequestHelper.GetUnityWebRequest(path, requestType, data, token, customHeader);
        if (request == null)
        {
            yield break;
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            failedCallback?.Invoke();
            yield break;
        }
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            networkErrorCallback?.Invoke();
            yield break;
        }

        var response = JsonUtility.FromJson<T>(request.downloadHandler.text);
        successCallback?.Invoke(response);
    }

    private async UniTask<T> SendRequestAsync<T>(string url, RequestType requestType, string data, string token, Tuple<string, string>[] customHeader)
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

    private IEnumerator IE_SendRequest(string path,
                            RequestType requestType,
                            SendApiSuccessHandler successCallback,
                            SendApiFailedHandler failedCallback,
                            SendApiFailedByNetWorkHandler networkErrorCallback,
                            string data,
                            string token,
                            Tuple<string, string>[] customHeader)
    {
        using (var request = RequestHelper.GetUnityWebRequest(path, requestType, data, token, customHeader))
        {
            if (request == null)
            {
                yield break;
            }
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                failedCallback?.Invoke();
                yield break;
            }
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                networkErrorCallback?.Invoke();
                yield break;
            }
            successCallback?.Invoke();
        }
    }
}