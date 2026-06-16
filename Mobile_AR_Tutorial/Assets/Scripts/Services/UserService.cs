using System.Collections;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

public class UserService : MonoBehaviour
{
    private string BASE_URL = "https://data.mongodb-api.com/app/data-burjt/endpoint/data/v1/action/";
    private string API_KEY = "FWp4pMPZYSDgU84hHmNzdA24gnuJDXbbZZyhNizSzYJ5wNbPOAhOtm0LSRgTtFIJ";

    public IEnumerator GetUserBtNameAndPassword(Action<bool, string, UserModel> callback)
    {
        bool isSuccess = false;
        string errorMessage = "";
        UserModel userModel = null;

        var stringToGet = $@"{{
            ""dataSource"": ""ClusterUnity"",
            ""database"": ""ARToolTutorials"",
            ""collection"": ""Tutorials"",
            ""filter"": {{}}
        }}";

        var getRequest = CreateRequestWithString(BASE_URL + "find", API_RequestType.POST, stringToGet);
        AttachHeader(getRequest, "api-key", API_KEY);
        yield return getRequest.SendWebRequest();

        if (getRequest.result == UnityWebRequest.Result.Success)
        {
            isSuccess = true;
            var responseText = getRequest.downloadHandler.text;
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);
        }
        else
        {
            isSuccess = false;
            errorMessage = getRequest.error;
        }

        callback(isSuccess, errorMessage, userModel);
    }

    public IEnumerator AddTutorialInUser(string userId, string tutorialId, Action<bool, string> callback)
    {
        bool isSuccess = false;
        string errorMessage = "";

        var stringToPost = $@"{{
        ""dataSource"": ""ClusterUnity"",
        ""database"": ""ARToolTutorials"",
        ""collection"": ""Users"",
        ""filter"": {{ ""_id"": {{ ""$oid"": ""{userId}"" }} }},
        ""update"": {{
                ""$push"": {{
                    ""idEditTutorials"": ""{tutorialId}""
                }}
            }}
        }}";

        // Configura a requisição POST
        var postRequest = CreateRequestWithString(BASE_URL + "updateOne", API_RequestType.POST, stringToPost);
        AttachHeader(postRequest, "api-key", API_KEY);
        yield return postRequest.SendWebRequest();

        if (postRequest.result == UnityWebRequest.Result.Success)
        {
            isSuccess = true;
            Debug.Log("Add Tutorial ID successfully.");
        }
        else
        {
            isSuccess = false;
            errorMessage = postRequest.error;
            Debug.LogError("Error adding tutorial: " + errorMessage);
            Debug.LogError("Response Code: " + postRequest.responseCode);
            Debug.LogError("Response: " + postRequest.downloadHandler.text);
        }

        callback(isSuccess, errorMessage);
    }

    public IEnumerator CreateNewUser(UserModel newUser, Action<bool, string, UserModel> callback)
    {
        bool isSuccess = false;
        string errorMessage = "";


        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var stringToPost = $@"{{
            ""dataSource"": ""ClusterUnity"",
            ""database"": ""ARToolTutorials"",
            ""collection"": ""Users"",
            ""document"": {JsonConvert.SerializeObject(newUser, settings)}
        }}";

        // Configura a requisição POST
        var postRequest = CreateRequestWithString(BASE_URL + "insertOne", API_RequestType.POST, stringToPost);
        AttachHeader(postRequest, "api-key", API_KEY);
        yield return postRequest.SendWebRequest();

        if (postRequest.result == UnityWebRequest.Result.Success)
        {
            isSuccess = true;

            // Analisa a resposta para obter o ID do documento inserido
            var responseText = postRequest.downloadHandler.text;
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);

            if (responseData.TryGetValue("insertedId", out object insertedId))
            {
                newUser.Id = insertedId.ToString();
            }
            else
            {
                Debug.LogWarning("No 'insertedId' found in the response.");
            }
        }
        else
        {
            isSuccess = false;
            errorMessage = postRequest.error;
            Debug.LogError("Error adding tutorial: " + errorMessage);
            Debug.LogError("Response Code: " + postRequest.responseCode);
            Debug.LogError("Response: " + postRequest.downloadHandler.text);
        }

        callback(isSuccess, errorMessage, newUser);
    }

    public IEnumerator GetAllUser(Action<bool, string, List<UserModel>> callback)
    {
        bool isSuccess = false;
        string errorMessage = "";
        List<UserModel> users = null;

        var stringToGet = $@"{{
        ""dataSource"": ""ClusterUnity"",
        ""database"": ""ARToolTutorials"",
        ""collection"": ""Users"",
        ""filter"": {{}}
    }}";

        var getRequest = CreateRequestWithString(BASE_URL + "find", API_RequestType.POST, stringToGet);
        AttachHeader(getRequest, "api-key", API_KEY);
        yield return getRequest.SendWebRequest();

        if (getRequest.result == UnityWebRequest.Result.Success)
        {
            isSuccess = true;
            var responseText = getRequest.downloadHandler.text;
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);

            if (responseData != null && responseData.TryGetValue("documents", out object documentsObj))
            {
                if (documentsObj is JArray documents)
                {
                    users = new List<UserModel>();

                    foreach (var document in documents)
                    {
                        try
                        {
                            var user = document.ToObject<UserModel>();

                            // Verificar se _id é um objeto com $oid ou um valor simples
                            var idToken = document["_id"];
                            if (idToken is JObject idObject && idObject.TryGetValue("$oid", out JToken oid))
                            {
                                user.Id = oid.ToString();
                            }
                            else
                            {
                                user.Id = idToken.ToString();
                            }

                            users.Add(user);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("Error parsing user document: " + ex.Message);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Documents field is not a JArray.");
                    isSuccess = false;
                    errorMessage = "Documents field is not a JArray.";
                }
            }
            else
            {
                Debug.LogError("Response data does not contain 'documents' or is null.");
                isSuccess = false;
                errorMessage = "Response data does not contain 'documents' or is null.";
            }
        }
        else
        {
            isSuccess = false;
            errorMessage = getRequest.error;
            Debug.LogError("Error fetching users: " + errorMessage);
        }

        callback(isSuccess, errorMessage, users);
    }


    public IEnumerator EditUser(string id, UserModel newUser, Action<bool, string, UserModel> callback)
    {
        bool isSuccess = false;
        string errorMessage = "";

        var stringToUpdate = $@"{{
            ""dataSource"": ""ClusterUnity"",
            ""database"": ""ARToolTutorials"",
            ""collection"": ""Users"",
            ""filter"": {{ ""_id"": {{ ""$oid"": ""{id}"" }} }},
            ""update"": {{
                ""$set"": {{
                    ""Name"": ""{newUser.Name}"",
                    ""Password"": ""{newUser.Password}""
                }}
            }}
        }}";

        // Configura a requisição POST
        var postRequest = CreateRequestWithString(BASE_URL + "updateOne", API_RequestType.POST, stringToUpdate);
        AttachHeader(postRequest, "api-key", API_KEY);
        yield return postRequest.SendWebRequest();

        if (postRequest.result == UnityWebRequest.Result.Success)
        {
            isSuccess = true;

            // Analisa a resposta para obter o ID do documento inserido
            var responseText = postRequest.downloadHandler.text;
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);

            newUser.Id = id;
        }
        else
        {
            isSuccess = false;
            errorMessage = postRequest.error;
            Debug.LogError("Error adding tutorial: " + errorMessage);
            Debug.LogError("Response Code: " + postRequest.responseCode);
            Debug.LogError("Response: " + postRequest.downloadHandler.text);
        }

        callback(isSuccess, errorMessage, newUser);
    }

    public IEnumerator getUserModelByNameAndPassword(string name, string password, Action<bool, string, UserModel> callback)
    {
        bool isSuccess = false;
        string errorMessage = "";
        UserModel newUserModel = null;

        // Construir a string JSON para a solicitação
        var stringToGet = $@"{{
            ""dataSource"": ""ClusterUnity"",
            ""database"": ""ARToolTutorials"",
            ""collection"": ""Users"",
            ""filter"": {{ 
                ""Name"": ""{name}"", 
                ""Password"": ""{password}""
            }}
        }}";

        var postRequest = CreateRequestWithString(BASE_URL + "findOne", API_RequestType.POST, stringToGet);
        AttachHeader(postRequest, "api-key", API_KEY);
        yield return postRequest.SendWebRequest();

        if (postRequest.result == UnityWebRequest.Result.Success)
        {
            try
            {
                // Parse the JSON response using Newtonsoft.Json
                var responseText = postRequest.downloadHandler.text;
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);

                if (responseData.ContainsKey("document"))
                {
                    var document = responseData["document"] as Newtonsoft.Json.Linq.JObject;
                    newUserModel = document.ToObject<UserModel>();
                    newUserModel.Id = document["_id"].ToString();
                    isSuccess = true;
                    Debug.Log("User retrieved successfully.");
                }
                else
                {
                    errorMessage = "User not found.";
                    Debug.LogWarning(errorMessage);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                errorMessage = "Error parsing response: " + ex.Message;
                Debug.LogWarning(errorMessage);
            }
        }
        else
        {
            isSuccess = false;
            errorMessage = postRequest.error;
            Debug.LogError("Error retrieving user: " + errorMessage);
            Debug.LogError("Response Code: " + postRequest.responseCode);
            Debug.LogError("Response: " + postRequest.downloadHandler.text);
        }

        callback(isSuccess, errorMessage, newUserModel);
    }

    public IEnumerator DeleteTutorialInUser(string userId, string idTutorial, Action<bool, string> callback)
    {
        bool isSuccess = true;
        string errorMessage = "";

        var stringToPull = $@"{{
            ""dataSource"": ""ClusterUnity"",
            ""database"": ""ARToolTutorials"",
            ""collection"": ""Users"",
            ""filter"": {{ ""_id"": {{ ""$oid"": ""{userId}"" }} }},
            ""update"": {{
                ""$pull"": {{ ""idEditTutorials"": ""{idTutorial}"" }}
            }}
        }}";

        var pullRequest = CreateRequestWithString(BASE_URL + "updateOne", API_RequestType.POST, stringToPull);
        AttachHeader(pullRequest, "api-key", API_KEY);
        yield return pullRequest.SendWebRequest();

        if (pullRequest.result == UnityWebRequest.Result.Success)
        {
            var responseText = pullRequest.downloadHandler.text;
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);

            if (responseData.TryGetValue("matchedCount", out object matchedCount) && (long)matchedCount > 0)
            {
                Debug.Log("Tutorial deleted successfully from user.");
            }
            else
            {
                Debug.Log("Tutorial was not found in the user's list, but no error occurred.");
            }
        }
        else
        {
            // Even if there's an error, we'll just log it but continue without setting isSuccess to false.
            Debug.LogError("Error deleting tutorial: " + pullRequest.error);
            Debug.LogError("Response Code: " + pullRequest.responseCode);
            Debug.LogError("Response: " + pullRequest.downloadHandler.text);
        }

        callback(isSuccess, errorMessage);
    }


    private UnityWebRequest CreateRequestWithString(string path, API_RequestType type, string data)
    {
        var request = new UnityWebRequest(path, type.ToString());

        if (data != null)
        {
            var bodyRaw = Encoding.UTF8.GetBytes(data);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    private void AttachHeader(UnityWebRequest request, string key, string value)
    {
        request.SetRequestHeader(key, value);
    }

    public enum API_RequestType
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3
    }

}

