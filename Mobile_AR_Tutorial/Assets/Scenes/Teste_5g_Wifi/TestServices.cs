using MongoDB.Driver;
using System.Threading.Tasks;
using UnityEngine;

public class TestServices : MonoBehaviour
{
    private const string ConnectionString = "mongodb+srv://user:user@cluster0.ogxbu.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
    private const string DatabaseName = "TestResultsDB";

    private MongoClient _mongoClient;
    private IMongoDatabase _database;

    private void Awake()
    {
        try
        {
            _mongoClient = new MongoClient(ConnectionString);
            _database = _mongoClient.GetDatabase(DatabaseName);
            Debug.Log("Conectado ao banco de dados com sucesso.");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Erro na inicialização do MongoDB: " + e.Message);
        }
    }

    public async Task<bool> SaveTestResultsBySize(TestBySizeModel testResult)
    {
        try
        {
            var collection = _database.GetCollection<TestBySizeModel>("TestResultsBySize");
            await collection.InsertOneAsync(testResult);
            Debug.Log("Teste por tamanho salvo com sucesso.");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Erro ao salvar o teste por tamanho: " + e.Message);
            return false;
        }
    }

    public async Task<bool> SaveTestResultsByQuantity(TestByQuantityModel testResult)
    {
        try
        {
            var collection = _database.GetCollection<TestByQuantityModel>("TestResultsByQuantity");
            await collection.InsertOneAsync(testResult);
            Debug.Log("Teste por quantidade salvo com sucesso.");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Erro ao salvar o teste por quantidade: " + e.Message);
            return false;
        }
    }
}