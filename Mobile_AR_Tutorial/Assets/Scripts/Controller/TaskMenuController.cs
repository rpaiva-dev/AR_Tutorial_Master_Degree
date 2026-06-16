using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TaskMenuController : MonoBehaviour
{
    private Canvas activeCanvas;
    private Button btn_AddTask;
    private Button btn_ReturnTask;
    private Button btn_SaveTask;

    private TutorialRepository tutorialRepository;

    public GameObject itemListTask;
    private GameObject listTaksParent;
    private TMP_InputField input_ResearchTasks;

    private TextMeshProUGUI txt_MainText;

    void Awake()
    {
        TutorialModel.listTasksEddited = TutorialModel.CurrentTutorial.Tasks;


        activeCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        tutorialRepository = GameObject.Find("MainScripts").GetComponent<TutorialRepository>();

        txt_MainText = GameObject.Find("txt_MainText").GetComponent<TextMeshProUGUI>();

        btn_AddTask = GameObject.Find("btn_AddTask").GetComponent<Button>();
        btn_AddTask.onClick.AddListener(() => modalAddTask());

        btn_ReturnTask = GameObject.Find("btn_ReturnTask").GetComponent<Button>();
        btn_ReturnTask.onClick.AddListener(() => modalReturnTask());

        btn_SaveTask = GameObject.Find("btn_SaveTask").GetComponent<Button>();
        btn_SaveTask.onClick.AddListener(() => modalSaveTask());

        listTaksParent = GameObject.Find("listTaksParent");

        input_ResearchTasks = GameObject.Find("input_ResearchTasks").GetComponent<TMP_InputField>();
        input_ResearchTasks.onValueChanged.AddListener((search) => {
            FilterTaskByName(search);
        });
    }

    private void FilterTaskByName(string search)
    {
        List<TaskModel> newTaskList;

        if (string.IsNullOrEmpty(search))
        {
            newTaskList = TutorialModel.listTasksEddited;
        }
        else
        {
            newTaskList = TutorialModel.listTasksEddited
                .Where(tut =>
                    tut.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();
        }

        InstantiateTaskList(newTaskList);
    }

    public void Start()
    {
        if (TutorialModel.CurrentTutorial != null)
            InstantiateTaskAndConfigureScreen();
        else
            SceneManager.LoadScene("Tutorial_Menu");
    }

    private void InstantiateTaskAndConfigureScreen()
    {
        txt_MainText.text = TutorialModel.CurrentTutorial.Name;
        InstantiateTaskList(TutorialModel.listTasksEddited);
    }

    private void InstantiateTaskList(List<TaskModel> taskModels)
    {
        foreach (Transform list in listTaksParent.transform)
        {
            Destroy(list.gameObject);
        }

        for (int i = 0; i < taskModels.Count; i++)
        {
            GameObject taskPrefabInstantiate = Instantiate(itemListTask, listTaksParent.transform);

            ItemListTaskController taskListController = taskPrefabInstantiate.GetComponent<ItemListTaskController>();
            taskListController.ItemListTaskControllerConstructor(taskModels[i]);

            RectTransform rectTransform = listTaksParent.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }


    public GameObject modalAddTaskPrefab;
    private void modalAddTask()
    {
        GameObject modalAddTask = Instantiate(modalAddTaskPrefab, activeCanvas.transform);
        ModalAddTaskController modalController = modalAddTask.GetComponent<ModalAddTaskController>();
        modalController.ModalAddTaskControllerConstructor(
            async (task) => {
                TutorialModel.listTasksEddited.Add(task);
                TutorialModel.indexTaskEdited = TutorialModel.listTasksEddited.Count - 1;
                showSucessMessage("Tarefa criada", "A câmera irá abrir e você poderá adicionar objetos virtuais, aguarde...", 5f);
                await Task.Delay(4000);
                SceneManager.LoadScene("Task_Create");
            }
        );
    }


    public GameObject modalReturnTaskPrefab;
    private void modalReturnTask()
    {
        GameObject modalReturnTask = Instantiate(modalReturnTaskPrefab, activeCanvas.transform);
        ModalReturnTask modalController = modalReturnTask.GetComponent<ModalReturnTask>();
        modalController.ModalReturnTaskConstructor(
            () => {
                TutorialModel.CurrentTutorial = null;
                SceneManager.LoadScene("Tutorial_Menu");
            }
        );
    }

    public GameObject modalFinishTaskPrefab;
    private void modalSaveTask()
    {
        GameObject modalFinishTask = Instantiate(modalFinishTaskPrefab, activeCanvas.transform);
        ModalFinishTask modalController = modalFinishTask.GetComponent<ModalFinishTask>();
        modalController.ModalFinishTaskConstructor(
            async () => {
                TutorialModel.CurrentTutorial.Tasks = TutorialModel.listTasksEddited;

                await tutorialRepository.editTutorial(
                TutorialModel.CurrentTutorial.Id.ToString(),
                TutorialModel.CurrentTutorial,
                (isSuccess, message) =>
                {
                    if (isSuccess)
                    {
                        TutorialModel.CurrentTutorial = null;
                        SceneManager.LoadScene("Tutorial_Menu");
                    }
                    else
                    {
                        showAlertMessage("Erro ao criar tarefa!", message, 3f);
                    }
                });
            }
        );
    }

    public GameObject modalSucessPrefab;
    private void showSucessMessage(string header, string message, float time)
    {
        GameObject modalSucessPrefabInstantiate = Instantiate(modalSucessPrefab, activeCanvas.transform);
        ModalSucessController modalSucessController = modalSucessPrefabInstantiate.GetComponent<ModalSucessController>();
        modalSucessController.showMessage(header, message, time);
    }


    public GameObject modalAlertPrefab;

    private void showAlertMessage(string header, string message, float time)
    {
        GameObject modalInfoPrefabInstantiate = Instantiate(modalAlertPrefab, activeCanvas.transform);
        ModalAlertController modalInfoController = modalInfoPrefabInstantiate.GetComponent<ModalAlertController>();
        modalInfoController.showMessage(header, message, time);
    }
}
