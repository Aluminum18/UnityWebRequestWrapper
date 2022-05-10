using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class RequestHelper
{
    public static UnityWebRequest GetUnityWebRequest(string url, RequestType requestType, string data, string token, params Tuple<string, string>[] customHeader)
    {
        switch (requestType)
        {
            case RequestType.GET:
                {
                    return CreateGetRequest(url, token, customHeader);
                }
            case RequestType.POST:
                {
                    return CreatePostRequest(url, data, token, customHeader);
                }
            default:
                {
                    return null;
                }
        }
    }

    private static UnityWebRequest CreateGetRequest(string url, string token, params Tuple<string, string>[] customHeader)
    {
        var request = UnityWebRequest.Get(url);

        if (token != null)
        {
            StringBuilder tokenSb = new StringBuilder();
            tokenSb.Append(token);
            request.SetRequestHeader("Authorization", tokenSb.ToString());
        }

        if (customHeader.Length != 0)
        {
            for (int i = 0; i < customHeader.Length - 1; i++)
            {
                var headerField = customHeader[i];
                request.SetRequestHeader(headerField.Item1, headerField.Item2);
            }
        }

        return request;
    }

    private static UnityWebRequest CreatePostRequest(string url, string requestBody, string token, params Tuple<string, string>[] customHeader)
    {
        byte[] rawBody = new UTF8Encoding().GetBytes(requestBody);
        return CreatePostRequest(url, rawBody, token, customHeader);
    }

    private static UnityWebRequest CreatePostRequest(string url, byte[] requestBody, string token, params Tuple<string, string>[] customHeader)
    {
        UnityWebRequest request = new UnityWebRequest
        {
            url = url,
            method = "POST"
        };

        request.downloadHandler = new DownloadHandlerBuffer();

        if (requestBody != null)
        {
            request.uploadHandler = new UploadHandlerRaw(requestBody);
        }

        if (token != null)
        {
            request.SetRequestHeader("Authorization", token);
        }

        if (customHeader.Length != 0)
        {
            for (int i = 0; i < customHeader.Length; i++)
            {
                var headerField = customHeader[i];
                request.SetRequestHeader(headerField.Item1, headerField.Item2);
            }
        }

        request.SetRequestHeader("Content-Type", "application/json");
        return request;
    }
}
