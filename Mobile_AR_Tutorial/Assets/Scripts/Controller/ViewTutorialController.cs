using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ViewTutorialController : MonoBehaviour
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


        if (TutorialModel.CurrentTutorial == null)
            SceneManager.LoadScene("Tutorial_Menu");
        else
            changeTask(0);
    }

    private void changeTask(int change)
    {
        TutorialModel.indexTaskEdited += change;

        txt_TaskName.text = TutorialModel.
            CurrentTutorial.Tasks[TutorialModel.indexTaskEdited].Name;
        txt_TaskIndex.text = $"Tarefa {TutorialModel.indexTaskEdited + 1} de {TutorialModel.CurrentTutorial.Tasks.Count}";

        virtualObjectRepository.InstantiateVirtualObjectsOnParent(TutorialModel.
            CurrentTutorial.Tasks[TutorialModel.indexTaskEdited].VirtualObjects, VirtualObjectParent);

        if(TutorialModel.indexTaskEdited == 0)
        {
            if (TutorialModel.CurrentTutorial.Tasks.Count == 1)
            {
                btn_PreviousTask.gameObject.SetActive(false);
                btn_NextTask.gameObject.SetActive(false);
                btn_ReturnTask.gameObject.SetActive(false);
                btn_FinishTask.gameObject.SetActive(true);
            }
            else 
            {
                btn_PreviousTask.gameObject.SetActive(false);
                btn_NextTask.gameObject.SetActive(true);
                btn_ReturnTask.gameObject.SetActive(true);
                btn_FinishTask.gameObject.SetActive(false);
            }
            
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

    public GameObject modalFinishViewTutorialPrefab;
    private void modaFinishTask()
    {
        GameObject modalFinishCreateTask = Instantiate(modalFinishViewTutorialPrefab, activeCanvas.transform);
        ModalFinishViewTutorial modalController = modalFinishCreateTask.GetComponent<ModalFinishViewTutorial>();
        modalController.ModalFinishTaskConstructor(
            () => {
                TutorialModel.CurrentTutorial = null;
                TutorialModel.indexTaskEdited = 0;
                SceneManager.LoadScene("Tutorial_Menu");
            }
        );
    }


    public GameObject modalReturnViewTutorialPrefab;

    private void modalReturnTask()
    {
        GameObject modalReturnTask = Instantiate(modalReturnViewTutorialPrefab, activeCanvas.transform);
        ModalReturnViewTutorial modalController = modalReturnTask.GetComponent<ModalReturnViewTutorial>();
        modalController.ModalReturnTaskConstructor(
            () => {
                TutorialModel.indexTaskEdited = 0;
                SceneManager.LoadScene("Task_Menu");
            }
        );
    }
}
