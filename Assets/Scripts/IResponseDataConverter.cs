
using UnityEngine.Networking;

public interface IResponseDataConverter<T>
{
    T Convert(DownloadHandler handler);
}
