using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TutorialsServices : MonoBehaviour
{
    private string fileName = "tutorials_data.json";
    private string filePath;

    private TutorialListWrapper data;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        LoadData();
    }

    // =========================
    // LOAD / SAVE
    // =========================

    private void LoadData()
    {
        if (!File.Exists(filePath))
        {
            TextAsset json = Resources.Load<TextAsset>("Data/tutorials_data");

            if (json != null)
                File.WriteAllText(filePath, json.text);
            else
                File.WriteAllText(filePath, "{\"tutorials\":[]}");
        }

        string jsonText = File.ReadAllText(filePath);

        if (string.IsNullOrEmpty(jsonText))
            jsonText = "{\"tutorials\":[]}";

        data = JsonUtility.FromJson<TutorialListWrapper>(jsonText);

        if (data == null)
            data = new TutorialListWrapper();

        if (data.tutorials == null)
            data.tutorials = new List<TutorialModel>();
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    // =========================
    // TUTORIAL
    // =========================

    public Task<List<TutorialModel>> GetAllTutorials()
    {
        return Task.FromResult(data.tutorials);
    }

    public Task<TutorialModel> GetTutorialByName(string tutorialName)
    {
        if (string.IsNullOrWhiteSpace(tutorialName))
            return Task.FromResult<TutorialModel>(null);

        var tutorial = data?.tutorials?
            .FirstOrDefault(t =>
                !string.IsNullOrEmpty(t?.Name) &&
                t.Name.Equals(tutorialName, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(tutorial);
    }

    public Task<TutorialModel> GetTutorialById(ObjectId tutorialId)
    {
        var tutorial = data.tutorials
            .FirstOrDefault(t => t.Id.ToString() == tutorialId.ToString());

        return Task.FromResult(tutorial);
    }

    public Task<bool> CreateNewTutorial(TutorialModel newTutorial, Action<bool, string> callback)
    {
        if (data == null)
            data = new TutorialListWrapper();

        if (data.tutorials == null)
            data.tutorials = new List<TutorialModel>();

        if (string.IsNullOrEmpty(newTutorial.Id.ToString()))
            newTutorial.Id = ObjectId.GenerateNewId().ToString();

        newTutorial.Tasks ??= new List<TaskModel>();

        data.tutorials.Add(newTutorial);
        SaveData();

        callback?.Invoke(true, "");
        return Task.FromResult(true);
    }

    public Task<bool> DeleteTutorial(string tutorialId)
    {
        var tutorial = data.tutorials.FirstOrDefault(t => t.Id.ToString() == tutorialId.ToString());
        if (tutorial == null) return Task.FromResult(false);

        data.tutorials.Remove(tutorial);
        SaveData();
        return Task.FromResult(true);
    }

    public Task<bool> editTutorialAsync(string id, TutorialModel currentTutorial)
    {
        int index = data.tutorials.FindIndex(t => t.Id.ToString() == id.ToString());
        if (index == -1) return Task.FromResult(false);

        data.tutorials[index] = currentTutorial;
        SaveData();
        return Task.FromResult(true);
    }

    public Task<bool> SetNameTutorial(string tutorialId, string newName)
    {
        var tutorial = data.tutorials.FirstOrDefault(t => t.Id.ToString() == tutorialId.ToString());
        if (tutorial == null) return Task.FromResult(false);

        tutorial.Name = newName;
        SaveData();
        return Task.FromResult(true);
    }

    // =========================
    // TASK
    // =========================

    public Task<bool> CreateNewTask(ObjectId tutorialId, TaskModel taskModel)
    {
        var tutorial = data.tutorials.FirstOrDefault(t => t.Id.ToString() == tutorialId.ToString());
        if (tutorial == null) return Task.FromResult(false);

        tutorial.Tasks ??= new List<TaskModel>();
        tutorial.Tasks.Add(taskModel);
        SaveData();
        return Task.FromResult(true);
    }

    public Task<bool> EditTask(ObjectId tutorialId, int indexTask, TaskModel currentTask)
    {
        var tutorial = data.tutorials.FirstOrDefault(t => t.Id.ToString() == tutorialId.ToString());
        if (tutorial == null || tutorial.Tasks == null) return Task.FromResult(false);

        if (indexTask < 0 || indexTask >= tutorial.Tasks.Count)
            return Task.FromResult(false);

        tutorial.Tasks[indexTask] = currentTask;
        SaveData();
        return Task.FromResult(true);
    }

    public Task<bool> DeleteTask(ObjectId tutorialId, string taskName)
    {
        var tutorial = data.tutorials.FirstOrDefault(t => t.Id.ToString() == tutorialId.ToString());
        if (tutorial == null || tutorial.Tasks == null) return Task.FromResult(false);

        var task = tutorial.Tasks.FirstOrDefault(t => t.Name == taskName);
        if (task == null) return Task.FromResult(false);

        tutorial.Tasks.Remove(task);
        SaveData();
        return Task.FromResult(true);
    }

    public Task<bool> SetTaskNameAsync(ObjectId tutorialId, int index, string newTaskName)
    {
        var tutorial = data.tutorials.FirstOrDefault(t => t.Id.ToString() == tutorialId.ToString());
        if (tutorial == null || tutorial.Tasks == null) return Task.FromResult(false);

        if (index < 0 || index >= tutorial.Tasks.Count)
            return Task.FromResult(false);

        tutorial.Tasks[index].Name = newTaskName;
        SaveData();
        return Task.FromResult(true);
    }

    public Task<TaskModel> GetTaskInTutorialByName(ObjectId id, string taskname)
    {
        var tutorial = data.tutorials.FirstOrDefault(t => t.Id.ToString() == id.ToString());
        if (tutorial == null || tutorial.Tasks == null)
            return Task.FromResult<TaskModel>(null);

        var task = tutorial.Tasks
            .FirstOrDefault(t => t.Name.Equals(taskname, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(task);
    }
}




//public class TutorialsServices : MonoBehaviour
//{
//    private string fileName = "tutorials_data.json";
//    private string filePath;

//    private TutorialListWrapper data;

//    void Awake()
//    {
//        filePath = Path.Combine(Application.persistentDataPath, fileName);
//        LoadData();
//    }

//    // ==========================
//    // LOAD / SAVE
//    // ==========================

//    private void LoadData()
//    {
//        if (!File.Exists(filePath))
//        {
//            // Copia o JSON inicial do Resources
//            TextAsset jsonFromResources = Resources.Load<TextAsset>("Data/tutorials_data");
//            File.WriteAllText(filePath, jsonFromResources.text);
//        }

//        string json = File.ReadAllText(filePath);
//        data = JsonUtility.FromJson<TutorialListWrapper>(json);

//        if (data == null)
//            data = new TutorialListWrapper();
//    }

//    private void SaveData()
//    {
//        string json = JsonUtility.ToJson(data, true);
//        File.WriteAllText(filePath, json);
//    }

//    // ==========================
//    // CRUD TUTORIAL
//    // ==========================

//    public Task<List<TutorialModel>> GetAllTutorials()
//    {
//        return Task.FromResult(data.tutorials);
//    }

//    public Task<TutorialModel> GetTutorialByName(string name)
//    {
//        TutorialModel tutorial = data.tutorials
//            .Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

//        return Task.FromResult(tutorial);
//    }

//    public Task<bool> CreateNewTutorial(TutorialModel newTutorial)
//    {
//        data.tutorials.Add(newTutorial);
//        SaveData();
//        return Task.FromResult(true);
//    }

//    public Task<bool> DeleteTutorial(string tutorialName)
//    {
//        var tutorial = data.tutorials.Find(t => t.Name == tutorialName);
//        if (tutorial == null) return Task.FromResult(false);

//        data.tutorials.Remove(tutorial);
//        SaveData();
//        return Task.FromResult(true);
//    }

//    public Task<bool> EditTutorial(TutorialModel updatedTutorial)
//    {
//        int index = data.tutorials.FindIndex(t => t.Id == updatedTutorial.Id);
//        if (index == -1) return Task.FromResult(false);

//        data.tutorials[index] = updatedTutorial;
//        SaveData();
//        return Task.FromResult(true);
//    }

//    // ==========================
//    // TASKS
//    // ==========================

//    public Task<bool> CreateNewTask(string tutorialId, TaskModel task)
//    {
//        var tutorial = data.tutorials.Find(t => t.Id.ToString() == tutorialId);
//        if (tutorial == null) return Task.FromResult(false);

//        tutorial.Tasks ??= new List<TaskModel>();
//        tutorial.Tasks.Add(task);
//        SaveData();
//        return Task.FromResult(true);
//    }

//    public Task<bool> DeleteTask(string tutorialId, string taskName)
//    {
//        var tutorial = data.tutorials.Find(t => t.Id.ToString() == tutorialId);
//        if (tutorial == null) return Task.FromResult(false);

//        var task = tutorial.Tasks.Find(t => t.Name == taskName);
//        if (task == null) return Task.FromResult(false);

//        tutorial.Tasks.Remove(task);
//        SaveData();
//        return Task.FromResult(true);
//    }
//}




















//using MongoDB.Bson;
//using MongoDB.Bson.IO;
//using MongoDB.Driver;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Unity.VisualScripting;
//using UnityEngine;

//public class TutorialsServices : MonoBehaviour
//{
//    private string connectionString = "mongodb+srv://user:user@cluster0.ogxbu.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";

//    private MongoClient mongoClient;
//    private static IMongoDatabase database;
//    private IMongoCollection<TutorialModel> tutorialCollection;

//    private bool isConnect = false;
//    //private Canvas activeCanvas;

//    public void Awake()
//    {
//        //activeCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

//        isConnect = configConnection();
//        if (!isConnect)
//        {
//            showMessage("Erro ao conectar ao banco de dados.", 3f);
//        }
//    }

//    private void Update()
//    {
//        if (!isConnect) { }
//    }

//    public bool configConnection()
//    {
//        try
//        {
//            mongoClient = new MongoClient(connectionString);
//            database = mongoClient.GetDatabase("ARToolTutorials");

//            if (database != null)
//            {
//                tutorialCollection = database.GetCollection<TutorialModel>("Tutorials");

//                if (tutorialCollection != null)
//                {
//                    Debug.Log("Coleção de tutoriais encontrada.");
//                    //showMessage("Coleção de tutoriais encontrada.", 2f);
//                    return true;
//                }
//                else
//                {
//                    //Debug.LogError("E("Erro: A coleção 'Tutorials' não foi encontrada no banco de dados.");
//                    //showMessage("Erro: A coleção 'Tutorials' não foi encontrada.", 3f);
//                    return false;
//                }
//            }
//            else
//            {
//                //Debug.LogError("E("Erro: O banco de dados 'ARToolTutorials' não foi encontrado.");
//                showMessage("Erro: O banco de dados 'ARToolTutorials' não foi encontrado.", 3f);
//                return false;
//            }
//        }
//        catch (Exception e)
//        {
//            //Debug.LogError("E("Erro na inicialização do MongoDB: " + e.Message + "\n" + e.StackTrace);
//            showMessage("Erro na inicialização do MongoDB: " + e.Message, 3f);
//            return false;
//        }
//    }

//    public async Task<List<TutorialModel>> GetAllTutorials()
//    {
//        if (!isConnect)
//        {
//            showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//            isConnect = configConnection();

//            // Verifique se a conexão falhou
//            if (!isConnect)
//            {
//                showMessage("Erro ao tentar se Conectar. Verifique a internet", 3f);
//                ////Debug.LogError("E("Erro ao tentar se reconectar ao banco de dados.");
//                return null;
//            }
//        }

//        try
//        {
//            // Verifique se a coleção foi corretamente inicializada
//            if (tutorialCollection == null)
//            {
//                //Debug.LogError("A coleção de tutoriais não foi inicializada.");
//                showMessage("A coleção de tutoriais não foi inicializada.", 3f);
//                return null;
//            }

//            // Buscar todos os tutoriais sem filtros
//            var filter = Builders<TutorialModel>.Filter.Empty;
//            var tutorials = await tutorialCollection.Find(filter).ToListAsync();

//            // Verificar se a lista de tutoriais está vazia
//            if (tutorials == null || tutorials.Count == 0)
//            {
//                Debug.LogWarning("Nenhum tutorial encontrado.");
//                //showMessage("Nenhum tutorial encontrado.", 3f);
//            }

//            return tutorials;
//        }
//        catch (Exception e)
//        {
//            // Log detalhado do erro para depuração
//            Debug.LogError("Erro ao buscar tutoriais: " + e.Message + " - StackTrace: " + e.StackTrace);
//            showMessage("Erro ao buscar tutoriais: " + e.Message, 3f);
//            return null;
//        }
//    }

//    public async Task<TutorialModel> GetTutorialByName(string tutorialName)
//    {
//        if (string.IsNullOrEmpty(tutorialName))
//        {
//            Debug.LogWarning("O nome do tutorial está vazio ou nulo.");
//            return null;
//        }

//        if (!isConnect)
//        {
//            showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//            isConnect = configConnection();

//            if (!isConnect)
//            {
//                showMessage("Erro ao tentar se reconectar ao banco de dados.", 3f);
//                return null;
//            }
//        }

//        try
//        {
//            // Filtro para busca case-insensitive
//            var filter = Builders<TutorialModel>.Filter.Regex(
//                t => t.Name,
//                new MongoDB.Bson.BsonRegularExpression($"^{tutorialName}$", "i")
//            );

//            // Busca no banco
//            TutorialModel tutorial = await tutorialCollection.Find(filter).FirstOrDefaultAsync();
//            return tutorial; // Retorna null se não encontrar
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Erro ao buscar tutorial pelo nome: " + e.Message);
//            return null; // Retorna null em caso de erro
//        }
//    }



//    public async Task<bool> CreateNewTutorial(TutorialModel newTutorial, Action<bool, string> callback)
//    {
//        if (!isConnect)
//        {
//            showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//            isConnect = configConnection();

//            if (!isConnect)
//            {
//                showMessage("Erro ao tentar se reconectar ao banco de dados.", 3f);
//                //Debug.LogError("E("Erro ao tentar se reconectar ao banco de dados.");
//            }
//        }

//        try
//        {
//            await tutorialCollection.InsertOneAsync(newTutorial);
//            Debug.Log("Tutorial created with ID: " + newTutorial.Id);
//            return true;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error creating tutorial: " + e.Message);
//            return false;
//        }
//    }

//    public async Task<bool> SetNameTutorial(ObjectId tutorialId, string newName)
//    {
//        if (!isConnect)
//        {
//            showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//            isConnect = configConnection();

//            if (!isConnect)
//            {
//                showMessage("Erro ao tentar se reconectar ao banco de dados.", 3f);
//                //Debug.LogError("E("Erro ao tentar se reconectar ao banco de dados.");
//            }
//        }

//        try
//        {
//            if (tutorialId == ObjectId.Empty || string.IsNullOrWhiteSpace(newName))
//            {
//                //Debug.LogError("E("Invalid tutorial ID or name.");
//                return false;
//            }

//            var filter = Builders<TutorialModel>.Filter.Eq(t => t.Id, tutorialId);
//            var update = Builders<TutorialModel>.Update.Set(t => t.Name, newName);
//            var result = await tutorialCollection.UpdateOneAsync(filter, update);

//            if (result.ModifiedCount > 0)
//            {
//                Debug.Log($"Tutorial with ID {tutorialId} updated successfully.");
//                return true;
//            }
//            else
//            {
//                Debug.LogWarning($"No tutorial was updated. Tutorial ID {tutorialId} may not exist.");
//                return false;
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"Error updating tutorial with ID {tutorialId}: {e.Message}");
//            return false;
//        }
//    }

//    public async Task<bool> SetTaskNameAsync(ObjectId tutorialId, int index, string newTaskName)
//    {
//        try
//        {
//            var tutorial = await GetTutorialById(tutorialId);
//            if (tutorial == null)
//            {
//                return false;
//            }

//            if (tutorial.Tasks == null || tutorial.Tasks.Count == 0)
//            {
//                return false;
//            }

//            var task = tutorial.Tasks[index];
//            if (task == null)
//            {
//                return false;
//            }

//            task.Name = newTaskName;

//            var filter = Builders<TutorialModel>.Filter.Eq(t => t.Id, tutorialId);
//            var update = Builders<TutorialModel>.Update.Set(t => t.Tasks, tutorial.Tasks);
//            var result = await tutorialCollection.UpdateOneAsync(filter, update);

//            if (result.ModifiedCount > 0)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
//        catch (Exception ex)
//        {
//            print("erro: "+ex);
//            return false;
//        }
//    }


//    public async Task<bool> EditTask(ObjectId tutorialId, int indexTask, TaskModel currentTask)
//    {

//        if (!isConnect)
//        {
//            showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//            isConnect = configConnection();

//            if (!isConnect)
//            {
//                showMessage("Erro ao tentar se reconectar ao banco de dados.", 3f);
//                //Debug.LogError("E("Erro ao tentar se reconectar ao banco de dados.");
//            }
//        }

//        try
//        {
//            var filter = Builders<TutorialModel>.Filter.Eq(t => t.Id, tutorialId);
//            var update = Builders<TutorialModel>.Update.Set($"Tasks.{indexTask}", currentTask);
//            var result = await tutorialCollection.UpdateOneAsync(filter, update);

//            if (result.ModifiedCount > 0)
//            {
//                Debug.Log("Task updated successfully.");
//                return true;
//            }
//            else
//            {
//                Debug.LogWarning("No task was updated.");
//                return false;
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error updating task: " + e.Message);
//            return false;
//        }
//    }

//    public async Task<bool> ChangeTaskIndexInTutorial(ObjectId tutorialId, int index1, int index2)
//    {
//        if (!isConnect)
//        {
//            showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//            isConnect = configConnection();

//            if (!isConnect)
//            {
//                showMessage("Erro ao tentar se reconectar ao banco de dados.", 3f);
//                //Debug.LogError("E("Erro ao tentar se reconectar ao banco de dados.");
//            }
//        }

//        try
//        {
//            var tutorial = await GetTutorialById(tutorialId);
//            if (tutorial == null || index1 < 0 || index2 < 0 || index1 >= tutorial.Tasks.Count || index2 >= tutorial.Tasks.Count)
//            {
//                Debug.LogWarning("Invalid task indices.");
//                return false;
//            }

//            var temp = tutorial.Tasks[index1];
//            tutorial.Tasks[index1] = tutorial.Tasks[index2];
//            tutorial.Tasks[index2] = temp;

//            var filter = Builders<TutorialModel>.Filter.Eq(t => t.Id, tutorialId);
//            var update = Builders<TutorialModel>.Update.Set(t => t.Tasks, tutorial.Tasks);
//            var result = await tutorialCollection.UpdateOneAsync(filter, update);

//            return result.ModifiedCount > 0;
//        }
//        catch (Exception e)
//        {
//            Debug.Log("Error swapping tasks: " + e.Message);
//            return false;
//        }
//    }

//    public async Task<bool> DeleteTask(ObjectId tutorialId, string taskName)
//    {
//        if (!isConnect)
//        {
//            showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//            isConnect = configConnection();

//            if (!isConnect)
//            {
//                showMessage("Erro ao tentar se reconectar ao banco de dados.", 3f);
//                //Debug.LogError("E("Erro ao tentar se reconectar ao banco de dados.");
//            }
//        }

//        try
//        {
//            var filter = Builders<TutorialModel>.Filter.Eq(t => t.Id, tutorialId);
//            var update = Builders<TutorialModel>.Update.PullFilter(t => t.Tasks, Builders<TaskModel>.Filter.Eq("Name", taskName));
//            var result = await tutorialCollection.UpdateOneAsync(filter, update);

//            if (result.ModifiedCount > 0)
//            {
//                Debug.Log("Task deleted successfully.");
//                return true;
//            }
//            else
//            {
//                Debug.LogWarning("No task was deleted.");
//                return false;
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error deleting task: " + e.Message);
//            return false;
//        }
//    }

//    public async Task<TutorialModel> GetTutorialById(ObjectId tutorialId)
//    {
//        if (!isConnect)
//        {
//            showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//            isConnect = configConnection();

//            if (!isConnect)
//            {
//                showMessage("Erro ao tentar se reconectar ao banco de dados.", 3f);
//                //Debug.LogError("E("Erro ao tentar se reconectar ao banco de dados.");
//            }
//        }

//        try
//        {
//            var filter = Builders<TutorialModel>.Filter.Eq(t => t.Id, tutorialId);
//            var tutorial = await tutorialCollection.Find(filter).FirstOrDefaultAsync();
//            return tutorial;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error fetching tutorial: " + e.Message);
//            return null;
//        }
//    }

//    public async Task<bool> DeleteTutorial(ObjectId tutorialId)
//    {
//        if (!isConnect)
//        {
//            showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//            isConnect = configConnection();

//            if (!isConnect)
//            {
//                showMessage("Erro ao tentar se reconectar ao banco de dados.", 3f);
//                //Debug.LogError("E("Erro ao tentar se reconectar ao banco de dados.");
//            }
//        }

//        try
//        {
//            var filter = Builders<TutorialModel>.Filter.Eq(t => t.Id, tutorialId);
//            var result = await tutorialCollection.DeleteOneAsync(filter);

//            if (result.DeletedCount > 0)
//            {
//                Debug.Log("Tutorial deleted successfully.");
//                return true;
//            }
//            else
//            {
//                Debug.LogWarning("No tutorial was deleted.");
//                return false;
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error deleting tutorial: " + e.Message);
//            return false;
//        }
//    }

//    public async Task<bool> CreateNewTask(ObjectId tutorialId, TaskModel taskModel)
//    {
//        if (!isConnect)
//        {
//            showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//            isConnect = configConnection();

//            if (!isConnect)
//            {
//                showMessage("Erro ao tentar se reconectar ao banco de dados.", 3f);
//                //Debug.LogError("E("Erro ao tentar se reconectar ao banco de dados.");
//            }
//        }

//        try
//        {
//            var filter = Builders<TutorialModel>.Filter.Eq(t => t.Id, tutorialId);

//            var tutorial = await tutorialCollection.Find(filter).FirstOrDefaultAsync();
//            if (tutorial == null)
//            {
//                //Debug.LogError("E("Tutorial not found.");
//                return false;
//            }

//            tutorial.Tasks ??= new List<TaskModel>();
//            tutorial.Tasks.Add(taskModel);

//            var update = Builders<TutorialModel>.Update.Set(t => t.Tasks, tutorial.Tasks);
//            var result = await tutorialCollection.UpdateOneAsync(filter, update);

//            if (result.ModifiedCount > 0)
//            {
//                Debug.Log("Task created successfully.");
//                return true;
//            }
//            else
//            {
//                Debug.LogWarning("No task was created.");
//                return false;
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error creating task: " + e.Message);
//            return false;
//        }
//    }

//    public async Task<TaskModel> GetTaskInTutorialByName(ObjectId id, string taskname)
//    {
//        try
//        {
//            var filter = Builders<TutorialModel>.Filter.Eq(t => t.Id, id);
//            var tutorial = await tutorialCollection.Find(filter).FirstOrDefaultAsync();
//            if (tutorial == null)
//            {   return null;    }

//            bool taskExists = tutorial.Tasks.Any(task => string.Equals(task.Name.ToUpper(), taskname.ToUpper(), StringComparison.OrdinalIgnoreCase));
//            TaskModel task = tutorial.Tasks.FirstOrDefault(x => x.Name.ToUpper() == taskname.ToUpper() );

//            return task;
//        }
//        catch (Exception ex)
//        {
//            print("erro: "+ex);
//            return null;
//        }
//    }


//    //public GameObject modalInfoPrefab;
//    private void showMessage(string message, float time)
//    {
//        //GameObject modalInfoPrefabInstantiate = Instantiate(modalInfoPrefab, activeCanvas.transform);
//        /*ModalInfoPrefabController modalInfoController = modalInfoPrefabInstantiate.GetComponent<ModalInfoPrefabController>();
//        modalInfoController.showMessage(message, time);*/
//    }

//    public async Task<bool> editTutorialAsync(ObjectId id, TutorialModel currentTutorial)
//    {
//        try
//        {
//            // Verifica a conexão com o banco de dados
//            if (!isConnect)
//            {
//                showMessage("Conexão não estabelecida. Tentando reconectar...", 3f);
//                isConnect = configConnection();

//                if (!isConnect)
//                {
//                    showMessage("Erro ao tentar se reconectar ao banco de dados.", 3f);
//                    Debug.LogError("Erro ao tentar se reconectar ao banco de dados.");
//                    return false;
//                }
//            }

//            // Define o filtro para encontrar o tutorial pelo id
//            var filter = Builders<TutorialModel>.Filter.Eq(t => t.Id, id);

//            // Define a atualização com os dados do `currentTutorial`
//            var update = Builders<TutorialModel>.Update
//                .Set(t => t.Name, currentTutorial.Name)
//                .Set(t => t.Tasks, currentTutorial.Tasks);

//            // Executa a atualização
//            var result = await tutorialCollection.UpdateOneAsync(filter, update);

//            // Retorna true se pelo menos um documento foi modificado
//            return result.ModifiedCount > 0;
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError("Erro ao atualizar tutorial: " + ex.Message);
//            showMessage("Erro ao atualizar tutorial: " + ex.Message, 3f);
//            return false;
//        }
//    }

//}
