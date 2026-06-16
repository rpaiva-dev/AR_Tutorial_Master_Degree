using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Networking;
public class AssetBundles_Services : MonoBehaviour {


    //public string BundleLocationURL = "https://raw.githubusercontent.com/paiva-rodrigo/UnityAssetBundles/main/AssetBundles/";
    //private string BundleLocationURL = "https://drive.google.com/uc?id=1q7BZuF3TVmvPLDHc6-POEcIOqh3JIwlT&export=download\r\n";

    void Start()
    {
        
    }

    /*public string GetBundleURL(string bundleName)
    {
        return $"{BundleLocationURL}{bundleName}";
    }*/

    private Dictionary<string, AssetBundle> assetBundleCache = new Dictionary<string, AssetBundle>();

    public IEnumerator GetAssetBundle(string idObjectName, Action<float, AssetBundle> onComplete, Action<string> onError)
    {
        string bundleURL = $"https://drive.google.com/uc?id={idObjectName}&export=download";
        float startDownloadTime = Time.realtimeSinceStartup; // Início do tempo de download

        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL))
        {
            yield return www.SendWebRequest();
            

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Network error: {www.error}");
                onError?.Invoke($"Network error: {www.error}");
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                if (bundle == null)
                {
                    onError?.Invoke("Erro ao carregar o AssetBundle.");
                    yield break;
                }

                bundle.Unload(false); // Faz unload do bundle
                float endDownloadTime = Time.realtimeSinceStartup; // Fim do tempo de download
                float downloadTime = (float)Math.Round(endDownloadTime - startDownloadTime, 4); // Tempo do download

                onComplete?.Invoke(downloadTime, bundle);
            }
        }
    }







}

