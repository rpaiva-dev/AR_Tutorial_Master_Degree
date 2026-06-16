using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Teste5G_Wifi_Controller : MonoBehaviour
{
    private AssetBundles_Services assetBundleServices;
    private TestServices testServices;

    private TextMeshProUGUI txt_Result;
    private Button btn_QuantidadeObj;
    private Button btn_TamanhoObj;
    private Toggle toogle_5G;
    private string connectionType;

    void Awake()
    {
        txt_Result = GameObject.Find("txt_Result").GetComponent<TextMeshProUGUI>();
        btn_QuantidadeObj = GameObject.Find("btn_QuantidadeObj").GetComponent<Button>();
        btn_QuantidadeObj.onClick.AddListener(() => StartCoroutine(StartDownloadMultipleVirtualObjects()));
        btn_TamanhoObj = GameObject.Find("btn_TamanhoObj").GetComponent<Button>();
        btn_TamanhoObj.onClick.AddListener(() => StartCoroutine(DownloadMultipleObjectsSequence()));
        assetBundleServices = GameObject.Find("MainScripts").GetComponent<AssetBundles_Services>();
        testServices = GameObject.Find("MainScripts").GetComponent<TestServices>();
        toogle_5G = GameObject.Find("toogle_5G").GetComponent<Toggle>();
        toogle_5G.onValueChanged.AddListener((value) => connectionType = value ? "5G" : "WIFI");

        if (toogle_5G.isOn)
            connectionType = "5G";
        else
            connectionType = "WIFI";

    }

    private IEnumerator DownloadMultipleObjectsSequence()
    {
        txt_Result.text = "Testes de Objetos de Tamanho diferente:\n";

        Dictionary<string, string> objects = new Dictionary<string, string>
    {
        { "64 KB", "1iWOM_jwkNetDMC4KC8SoXV9OxudXdMoW" },
        { "128 KB", "1kK18cFSYGkrgldBzhjdgQejmXJNWlpb3" },
        { "256 KB", "1sbykLqetufO-SDxBGXwMosK2vCJmumZv" },
        { "512 KB", "1k4UQSXx6q7_ecgs98hbjNCMZn6zHpRF-" },
        { "1 MB", "1dtP_liV_fVa1BI2gWPlO6_QVwOH3xyEm" },
        { "2 MB", "1m39nRu5AM6Q-f0b8poRbnyyR50joxNIC" },
        { "4 MB", "1eVlHmT-bQhKeyUt8pEVFGqVJqM1DJklC" },
        { "8 MB", "1Bh0fo_Cp1ZTBDBUOv1d_T2imq2KmPXQ-" },
        { "16 MB", "1hitbkSV7M2LziPBff-jzP91TLTOXMl8b" },
        { "32 MB", "1R3n8uvL3v1Ljp4tkWcnaGmd55s_ukvcw" },
        { "64 MB", "17ZVPIa4E0LDMNgV46NiOzw8IbEoOzmaq" }
    };

        for (int exec = 1; exec <= 33; exec++)
        {
            txt_Result.text = $"Teste em andamento: Conexão {connectionType}, Execução {exec}/33\n";

            foreach (var obj in objects)
            {
                string objectName = obj.Key;
                string objectId = obj.Value;

                bool downloadCompleted = false;
                float downloadTime = 0;

                yield return StartCoroutine(assetBundleServices.GetAssetBundle(
                    objectId,
                    async (downloadT, assetBundle) =>
                    {
                        downloadTime = downloadT;

                        txt_Result.text += $"{objectName}: {downloadTime} segundos\n";

                        bool saved = await testServices.SaveTestResultsBySize(new TestBySizeModel(exec, connectionType, objectName, downloadTime));
                        if (!saved) Debug.LogWarning("Erro ao salvar no banco de dados.");

                        downloadCompleted = true;
                    },
                    (error) =>
                    {
                        txt_Result.text += $"Erro ao baixar {objectName}: {error}\n";
                        downloadCompleted = true;
                    }
                ));

                yield return new WaitUntil(() => downloadCompleted);

                // Aguarda um pequeno tempo entre os downloads para evitar congestionamento de rede
                yield return new WaitForSeconds(1f);
            }
        }

        txt_Result.text += "\nTodos os downloads foram concluídos.";
    }


    private IEnumerator StartDownloadMultipleVirtualObjects()
    {
        txt_Result.text = "Testes de Objetos de Quantidades Diferentes:\n";
        int[] objectCounts = { 1, 10, 100 };
        string id64kb= "1iWOM_jwkNetDMC4KC8SoXV9OxudXdMoW";

        for (int exec = 1; exec <= 33; exec++)
        {
            txt_Result.text = $"Teste em andamento: Conexão {connectionType}, Execução {exec}/33\n";

            foreach (int count in objectCounts)
            {
                float startTime = Time.time;
                for (int i = 0; i < count; i++)
                {
                    bool downloadCompleted = false;
                    yield return StartCoroutine(assetBundleServices.GetAssetBundle(
                        id64kb,
                        (downloadTime, assetBundle) =>
                        {
                            //assetBundle?.Unload(false);
                            downloadCompleted = true;
                        },
                        (error) =>
                        {
                            txt_Result.text += $"Erro ao baixar objeto {i + 1} no grupo de {count}: {error}\n";
                            downloadCompleted = true;
                        }
                    ));
                    yield return new WaitUntil(() => downloadCompleted);
                }
                float totalDownloadTime = Time.time - startTime;
                txt_Result.text += $"Download de {count} objetos: {totalDownloadTime:F4} segundos\n";

                // Usando uma corrotina para salvar no banco de dados
                yield return StartCoroutine(SaveTestResultsByQuantityCoroutine(exec, connectionType, count, totalDownloadTime));
            }
        }
        txt_Result.text += "\nTodos os downloads de múltiplos objetos foram concluídos.";
    }

    private IEnumerator SaveTestResultsByQuantityCoroutine(int teste, string connectionType, int count, float totalDownloadTime)
    {
        //bool saved = false;
        Task<bool> saveTask = testServices.SaveTestResultsByQuantity(new TestByQuantityModel(teste, connectionType, count, totalDownloadTime));
        yield return new WaitUntil(() => saveTask.IsCompleted);

        if (saveTask.Result)
        {
            Debug.Log("Teste por quantidade salvo com sucesso.");
        }
        else
        {
            Debug.LogWarning("Erro ao salvar no banco de dados.");
        }
    }

}