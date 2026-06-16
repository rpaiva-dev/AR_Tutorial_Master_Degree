using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MongoDB.Bson;

public class TutorialRepository : MonoBehaviour
{
    private TutorialsServices tutorialsServices;

    void Awake()
    {
        tutorialsServices = GetComponent<TutorialsServices>();
    }

    public async Task CreateTutorialAsync(TutorialModel newTutorial, Action<bool, string, TutorialModel> callback)
    {
        try
        {
            bool isSaveSuccessful = await tutorialsServices.CreateNewTutorial(newTutorial, (a, b) => { });
            if (isSaveSuccessful)
            {
                callback(true, "", newTutorial);
            }
            else
            {
                callback(false, "Failed to create tutorial.", null);
            }
        }
        catch (Exception ex)
        {
            callback(false, ex.Message, null);
        }
    }

    public async Task GetTutorialByName(string tutorialName, Action<bool, string, TutorialModel> callback)
    {
        
        if (string.IsNullOrEmpty(tutorialName))
        {
            Debug.LogError("O nome do tutorial é nulo ou vazio.");
            callback(false, "O nome do tutorial não pode ser nulo ou vazio.", null);
            return;
        }

        TutorialModel tutorial = await tutorialsServices.GetTutorialByName(tutorialName);

        if (tutorial != null)
        {
            Debug.Log("Tutorial encontrado: " + tutorial.Name);
            callback(true, "Tutorial encontrado", tutorial);
        }
        else
        {
            Debug.Log("Tutorial não encontrado: " + tutorialName);
            callback(true, "Tutorial não encontrado.", null);
        }
    }


    public async Task GetTaskInTutorialByName(ObjectId id, string taskname, Action<bool, string, TaskModel> callback)
    {
        try
        {
            var task = await tutorialsServices.GetTaskInTutorialByName(id, taskname);

            if (task != null)
                callback(true, "", task);
            else
                callback(true, "Tarefa não encontrado.", null);
        }
        catch (Exception ex)
        {
            Debug.LogError("Erro ao obter Tarefa por nome: " + ex.Message);
            callback(false, "Erro ao buscar Tarefa: " + ex.Message, null);
        }
    }

    public async Task GetTutorialByIdAsync(ObjectId tutorialId, Action<bool, string, TutorialModel> callback)
    {
        try
        {
            var tutorial = await tutorialsServices.GetTutorialById(tutorialId);
            callback(tutorial != null, tutorial != null ? "" : "Tutorial not found", tutorial);
        }
        catch (Exception ex)
        {
            callback(false, ex.Message, null);
        }
    }

    public async Task GetAllTutorialsAsync(Action<bool, string, List<TutorialModel>> callback)
    {
        try
        {
            var tutorialsList = await tutorialsServices.GetAllTutorials();
            if (tutorialsList != null && tutorialsList.Count > 0)
            {
                callback(true, "", tutorialsList);
            }
        }
        catch (Exception ex)
        {
            callback(false, "Erro ao buscar tutoriais: " + ex.Message, null);
        }
    }

    public async Task CreateTaskInTutorialAsync(ObjectId idTutorial, TaskModel currentTask, Action<bool, string, TaskModel> callback)
    {
        try
        {
            bool isSaveSuccessful = await tutorialsServices.CreateNewTask(idTutorial, currentTask);
            callback(isSaveSuccessful, isSaveSuccessful ? "" : "Failed to create task", currentTask);
        }
        catch (Exception ex)
        {
            callback(false, ex.Message, null);
        }
    }

    public async Task EditTaskInTutorialAsync(ObjectId tutorialId, int indexTask, TaskModel currentTask, Action<bool, string, TaskModel> callback)
    {
        try
        {
            bool isSaveSuccessful = await tutorialsServices.EditTask(tutorialId, indexTask, currentTask);
            callback(true,  "Success!!!", currentTask);
        }
        catch (Exception ex)
        {
            callback(false, ex.Message, currentTask);
        }
    }

    public async Task SetNameTutorialAsync(string tutorialId, string newName, Action<bool, string> callback)
    {
        try
        {
            bool isSaveSuccessful = await tutorialsServices.SetNameTutorial(tutorialId, newName);
            callback(isSaveSuccessful, isSaveSuccessful ? "Success!!!" : "Failed to set tutorial name.");
        }
        catch (Exception ex)
        {
            callback(false, ex.Message);
        }
    }

    public async Task SetNameTaskAsync(ObjectId tutorialId, int index, string newName, Action<bool, string> callback)
    {
        try
        {
            bool isSaveSuccessful = await tutorialsServices.SetTaskNameAsync(tutorialId, index, newName);
            callback(isSaveSuccessful, isSaveSuccessful ? "Success!!!" : "Failed to set task name.");
        }
        catch (Exception ex)
        {
            callback(false, ex.Message);
        }
    }



    public async Task DeleteTutorialAsync(string tutorialId, Action<bool, string> callback)
    {
        try
        {
            bool isSaveSuccessful = await tutorialsServices.DeleteTutorial(tutorialId);
            callback(isSaveSuccessful, isSaveSuccessful ? "Success!!!" : "Failed to delete tutorial.");
        }
        catch (Exception ex)
        {
            callback(false, ex.Message);
        }
    }

    public async Task DeleteTaskAsync(ObjectId tutorialId, string taskname, Action<bool, string> callback)
    {
        try
        {
            bool isSaveSuccessful = await tutorialsServices.DeleteTask(tutorialId, taskname);
            callback(isSaveSuccessful, isSaveSuccessful ? "Success!!!" : "Failed to delete task.");
        }
        catch (Exception ex)
        {
            callback(false, ex.Message);
        }
    }

    public async Task editTutorial(string id, TutorialModel currentTutorial, Action<bool, string> callback)
    {
        try
        {
            bool isSaveSuccessful = await tutorialsServices.editTutorialAsync(id.ToString(), currentTutorial);
            callback(true, "Success!!!");
        }
        catch (Exception ex)
        {
            callback(false, ex.Message);
        }
    }



    /*
    public async Task PutTutorialsOnListObjec()
    {
        await LoadAndDisplayTutorials();
    }

    private async Task LoadAndDisplayTutorials()
    {
        bool isSaveSuccessful = false;
        string errorMessage = "";
        List<TutorialModel> tutorialList = null;

        // Get all tutorials
        try
        {
            tutorialList = await tutorialsServices.GetAllTutorials();
            isSaveSuccessful = tutorialList != null;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }

        // Display tutorials
        if (isSaveSuccessful)
        {
            foreach (TutorialModel tutorial in tutorialList)
            {
                GameObject newListTutorial = Instantiate(listTutorial, listSlidingArea.transform);
                GameObject.Find("btnEdit").GetComponent<Button>().onClick.AddListener(() => { OnEditTutorialClicked(tutorial); });
                GameObject.Find("btnStart").GetComponent<Button>().onClick.AddListener(() => { OnStartTutorialClicked(tutorial); });
                newListTutorial.GetComponentInChildren<TextMeshProUGUI>().text = $"{tutorial.Name}: {tutorial.Description}";
            }
        }
        else
        {
            await ShowErrorPanel($"Failed to load tutorials: {errorMessage}", 3f);
        }
    }

    private void OnStartTutorialClicked(TutorialModel tutorial)
    {
        TutorialModel.CurrentTutorial = tutorial;
        SceneManager.LoadScene("View");
    }

    private void OnEditTutorialClicked(TutorialModel tutorial)
    {
        TutorialModel.CurrentTutorial = tutorial;
        SceneManager.LoadScene("Edit");
    }

    private async Task ShowErrorPanel(string message, float time)
    {
        GameObject errorPanel = Instantiate(Resources.Load("errorPanel") as GameObject);
        errorPanel.GetComponentInChildren<TextMeshProUGUI>().text = message;

        // Ensure the error panel is a child of the canvas, if necessary
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            errorPanel.transform.SetParent(canvas.transform, false);
        }

        await Task.Delay(TimeSpan.FromSeconds(time));

        // Destroy the error panel
        Destroy(errorPanel);
    }
    */
}
