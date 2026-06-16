using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TaskCreateController : MonoBehaviour
{

    private TutorialRepository tutorialRepository;
    private VirtualObjectRepository virtualObjectRepository;
    private VirtualObjectMenuController virtualObjectMenuController;

    private Canvas activeCanvas;
    private TaskModel editedTask;

    private GameObject VirtualObjectParent;

    private GameObject header;
    private TextMeshProUGUI txt_TaskName;
    private Button btn_ReturnTask;

    /*private GameObject ImageTarget; 
    private DefaultObserverEventHandler imageTargetHandler;*/


    private void Awake()
    {
        activeCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        VirtualObjectParent = GameObject.Find("VirtualObjectParent");

        /*ImageTarget = GameObject.Find("ImageTarget");
        imageTargetHandler = ImageTarget.GetComponent<DefaultObserverEventHandler>();*/
        
        //header
        header = GameObject.Find("header");
        txt_TaskName = header.GetComponentsInChildren<TextMeshProUGUI>()
            .FirstOrDefault(x => x.name == "txt_TaskName");
        btn_ReturnTask = GameObject.Find("btn_ReturnTask").GetComponent<Button>();
        btn_ReturnTask.onClick.AddListener(() => modalReturnTask());
        

        //repository
        tutorialRepository = GameObject.Find("MainScripts").GetComponent<TutorialRepository>();
        virtualObjectRepository = GameObject.Find("MainScripts").GetComponent<VirtualObjectRepository>();

        if (TutorialModel.CurrentTutorial == null)
            SceneManager.LoadScene("Tutorial_Menu");
        else
            configureTaskCreateSetup();


        virtualObjectMenuController = GameObject.Find("virtualObjectMenu").GetComponent<VirtualObjectMenuController>();
        virtualObjectMenuController.VirtualObjectMenuControllerConstructor(
            (type) => {
                GameObject newObj = virtualObjectRepository.InstantiateVirtualObjectPrefabOnParent(type, VirtualObjectParent);
                VirtualObjectModel newVirtualObject = new VirtualObjectModel(newObj, type);

                virtualObjectMenuController.SetToEditObjectLayout(newVirtualObject);
                editedTask.VirtualObjects.Add(newVirtualObject);
                renameAllObjectsAndVirtualObjects();
            },
            (selectedVirtualObjectToEdit) => {
                //#preciso verificar aqui    
            },
            (selectedVirtualObjectToDelete) =>
            {
                editedTask.VirtualObjects.Remove(selectedVirtualObjectToDelete);
                Destroy(selectedVirtualObjectToDelete.GameObject);
                renameAllObjectsAndVirtualObjects();
            },
            this.editedTask
            );
    }

    private GameObject currentModalPrefab;
    public GameObject modalInfoPrefab;
    private void imageTargetFound(bool isFound)
    {
        // Evita recriar modais repetidamente
        if (currentModalPrefab != null)
        {
            Destroy(currentModalPrefab);
            currentModalPrefab = null;
        }

        if (isFound)
        {
            Debug.Log("Imagem alvo encontrada - Exibindo modal de sucesso.");
            currentModalPrefab = Instantiate(modalSucessPrefab, activeCanvas.transform);
            ModalSucessController modalSucessController = currentModalPrefab.GetComponent<ModalSucessController>();
            modalSucessController.showMessage(
                "Imagem alvo encontrada!",
                "Imagem alvo encontrada com sucesso!",
                2f
            );
        }
        else
        {
            Debug.Log("Imagem alvo perdida - Exibindo modal de alerta.");
            currentModalPrefab = Instantiate(modalInfoPrefab, activeCanvas.transform);
            ModalInfoController modalInfoController = currentModalPrefab.GetComponent<ModalInfoController>();
            modalInfoController.showMessage(
                "Imagem alvo perdida!",
                "Imagem alvo perdida, aponte a câmera para ela.",
                2f
            );
        }
    }


    private void configureTaskCreateSetup()
    {
        // Garante que os listeners sejam registrados corretamente
        /*imageTargetHandler.OnTargetFound.RemoveAllListeners();
        imageTargetHandler.OnTargetLost.RemoveAllListeners();*/

        // Listeners para eventos de encontrar/perder imagem alvo
        /*imageTargetHandler.OnTargetFound.AddListener(() => {
            Debug.Log("Evento: Imagem alvo encontrada!");
            imageTargetFound(true);
        });

        imageTargetHandler.OnTargetLost.AddListener(() => {
            Debug.Log("Evento: Imagem alvo perdida!");
            imageTargetFound(false);
        });*/

        if (TutorialModel.indexTaskEdited==0)
            showModalFindImageTarget();

        // Configura a tarefa e objetos virtuais
        editedTask = TutorialModel.listTasksEddited[TutorialModel.indexTaskEdited];
        txt_TaskName.text = editedTask.Name;
        virtualObjectRepository.InstantiateVirtualObjectsOnParent(editedTask.VirtualObjects, VirtualObjectParent);
    }


    public GameObject modalFindImageTargetPrefab;

    private void showModalFindImageTarget()
    {
        GameObject modalReturnTask = Instantiate(modalFindImageTargetPrefab, activeCanvas.transform);
        ModalFindImageTargetController modalController = modalReturnTask.GetComponent<ModalFindImageTargetController>();

        // Configura os listeners apenas ao confirmar no modal
        modalController.ModalFindImageTargetConstructor(() =>
        {
            /*imageTargetHandler.OnTargetFound.AddListener(() =>
            {
                Debug.Log("Imagem alvo encontrada no modal.");
                imageTargetFound(true);
            });
            imageTargetHandler.OnTargetLost.AddListener(() =>
            {
                Debug.Log("Imagem alvo perdida no modal.");
                imageTargetFound(false);
            });*/
        });
    }


    private void renameAllObjectsAndVirtualObjects()
    {
        for (int i = 0; i < VirtualObjectParent.transform.childCount; i++)
        {
            GameObject currentObject = VirtualObjectParent.transform.GetChild(i).gameObject;
            string cleanName = Regex.Replace(currentObject.name, @"^\(\d+\)\s*", "");
            currentObject.name = $"({i}) {cleanName}";

            if (i < editedTask.VirtualObjects.Count)
            {
                editedTask.VirtualObjects[i].Name = currentObject.name;
            }
        }
    }

    
    public GameObject modalReturnTaskPrefab;
    private void modalReturnTask()
    {
        GameObject modalReturnTask = Instantiate(modalReturnTaskPrefab, activeCanvas.transform);
        ModalReturnCreateTask modalController = modalReturnTask.GetComponent<ModalReturnCreateTask>();
        modalController.ModalReturnTaskConstructor(
            () => {
                TutorialModel.listTasksEddited.RemoveAt(TutorialModel.listTasksEddited.Count - 1);

                editedTask = null;
                TutorialModel.indexTaskEdited = 0;

                SceneManager.LoadScene("Task_Menu");
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
