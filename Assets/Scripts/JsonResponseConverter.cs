using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JsonResponseConverter<T> : IResponseDataConverter<T>
{
    public T Convert(DownloadHandler handler)
    {
        return JsonUtility.FromJson<T>(handler.text);
    }
}
