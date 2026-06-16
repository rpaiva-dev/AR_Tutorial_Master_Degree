using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreViewTutorialController : MonoBehaviour
{
    private Canvas activeCanvas;
    private VirtualObjectRepository virtualObjectRepository;
    private GameObject VirtualObjectParent;

    private TextMeshProUGUI txt_TaskName;
    private TextMeshProUGUI txt_TaskIndex;

    private Button btn_ReturnTask;
    private Button btn_FinishTask;

    private Button btn_PreviousTask;
    private Button btn_NextTask;
    private Button btn_EditTask;


    private void Awake()
    {
        activeCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        virtualObjectRepository = GameObject.Find("MainScripts").GetComponent<VirtualObjectRepository>();
        VirtualObjectParent = GameObject.Find("VirtualObjectParent");


        txt_TaskName = GameObject.Find("txt_TaskName").GetComponent<TextMeshProUGUI>();
        txt_TaskIndex = GameObject.Find("txt_TaskIndex").GetComponent<TextMeshProUGUI>();

        btn_ReturnTask = GameObject.Find("btn_ReturnTask").GetComponent<Button>();
        btn_ReturnTask.onClick.AddListener(() => modalReturnTask());
        btn_FinishTask = GameObject.Find("btn_FinishTask").GetComponent<Button>();
        btn_FinishTask.onClick.AddListener(() => modaFinishTask());


        btn_PreviousTask = GameObject.Find("btn_PreviousTask").GetComponent<Button>();
        btn_PreviousTask.onClick.AddListener(() => changeTask(-1));

        btn_NextTask = GameObject.Find("btn_NextTask").GetComponent<Button>();
        btn_NextTask.onClick.AddListener(() => changeTask(1));

        btn_EditTask = GameObject.Find("btn_EditTask").GetComponent<Button>();
        btn_EditTask.onClick.AddListener(() => 
            modaEditTask()
        );

        if (TutorialModel.CurrentTutorial == null)
            SceneManager.LoadScene("Tutorial_Menu");
        else
            changeTask(0);
    }

    private void changeTask(int change)
    {
        TutorialModel.indexTaskEdited += change;

        txt_TaskName.text = TutorialModel.
            listTasksEddited[TutorialModel.indexTaskEdited].Name;
        txt_TaskIndex.text = $"Tarefa {TutorialModel.indexTaskEdited+1} de {TutorialModel.CurrentTutorial.Tasks.Count}";

        virtualObjectRepository.InstantiateVirtualObjectsOnParent(TutorialModel.
            listTasksEddited[TutorialModel.indexTaskEdited].VirtualObjects, VirtualObjectParent);

        if (TutorialModel.indexTaskEdited == 0)
        {
            btn_PreviousTask.gameObject.SetActive(false);
            btn_NextTask.gameObject.SetActive(true);
            btn_ReturnTask.gameObject.SetActive(true);
            btn_FinishTask.gameObject.SetActive(false);
        }
        else if (TutorialModel.indexTaskEdited == TutorialModel.CurrentTutorial.Tasks.Count - 1)
        {
            btn_PreviousTask.gameObject.SetActive(true);
            btn_NextTask.gameObject.SetActive(false);
            btn_FinishTask.gameObject.SetActive(true);
            btn_ReturnTask.gameObject.SetActive(false);
        }
        else
        {
            btn_PreviousTask.gameObject.SetActive(true);
            btn_NextTask.gameObject.SetActive(true);
            btn_ReturnTask.gameObject.SetActive(true);
            btn_FinishTask.gameObject.SetActive(false);
        }
    }

    public GameObject modalEditPreviewTutorialPrefab;
    private void modaEditTask()
    {
        // Verifica se o prefab está atribuído
        if (modalEditPreviewTutorialPrefab == null)
        {
            Debug.LogError("❌ modalEditPreviewTutorialPrefab está nulo! Atribua o prefab no Inspector.");
            return;
        }

        // Verifica se o canvas está atribuído
        if (activeCanvas == null)
        {
            Debug.LogError("❌ activeCanvas está nulo! Verifique se o objeto foi atribuído.");
            return;
        }

        // Instancia o modal
        GameObject modalFinishCreateTask = Instantiate(modalEditPreviewTutorialPrefab, activeCanvas.transform);

        // Tenta obter o componente do modal
        ModalEditPreviewTutorialController modalController = modalFinishCreateTask.GetComponent<ModalEditPreviewTutorialController>();

        // Verifica se o componente realmente existe
        if (modalController == null)
        {
            Debug.LogError("❌ O prefab não possui o componente ModalEditPreviewTutorialController!");
            Destroy(modalFinishCreateTask); // Destroi o modal criado incorretamente
            return;
        }

        // Se tudo está ok, executa a função normalmente
        modalController.ModalEditTaskConstructor(() =>
        {
            if (TutorialModel.CurrentTutorial == null)
            {
                Debug.LogError("❌ TutorialModel.CurrentTutorial está nulo!");
                return;
            }

            Debug.Log("🟩 TutorialModel: " + TutorialModel.CurrentTutorial);
            SceneManager.LoadScene("Task_Create");
        });
    }

    public GameObject modalFinishPreviewTutorialPrefab;
    private void modaFinishTask()
    {
        GameObject modalFinishCreateTask = Instantiate(modalFinishPreviewTutorialPrefab, activeCanvas.transform);
        ModalFinishPreViewTutorial modalController = modalFinishCreateTask.GetComponent<ModalFinishPreViewTutorial>();
        modalController.ModalFinishTaskConstructor(
            () => {
                TutorialModel.indexTaskEdited = 0;
                SceneManager.LoadScene("Task_Menu");
            }
        );
    }


    public GameObject modalReturnPreviewTutorialPrefab;

    private void modalReturnTask()
    {
        GameObject modalReturnTask = Instantiate(modalReturnPreviewTutorialPrefab, activeCanvas.transform);
        ModalReturnPreViewTutorial modalController = modalReturnTask.GetComponent<ModalReturnPreViewTutorial>();
        modalController.ModalReturnTaskConstructor(
            () => {
                TutorialModel.indexTaskEdited = 0;
                SceneManager.LoadScene("Task_Menu");
            }
        );
    }
}
