using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsListTaskController : MonoBehaviour
{

    private Canvas activeCanvas;

    private Button btn_preview;
    private Button btn_rename;
    private Button btn_edit;
    private Button btn_duplicate;
    private Button btn_moveUp;
    private Button btn_moveDown;
    private Button btn_remove;

    public GameObject modalViewTaskPrefab;
    public GameObject modalRenameTaskPrefab;
    public GameObject modalEditTaskPrefab;
    public GameObject modalDuplicateTaskPrefab;
    public GameObject modalRemoveTaskPrefab;
    private TaskModel taskModel;


    //private GameObject errorObject;
    //private TextMeshProUGUI txtError;

    void Awake()
    {
        activeCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        btn_preview = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_preview");

        btn_rename = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_rename");

        btn_edit = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_edit");

        btn_duplicate = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_duplicate");

        btn_moveUp = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_moveUp");

        btn_moveDown = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_moveDown");

        btn_remove = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_remove");
    }

    public void constructorTaskMenuController(
       TaskModel task,
       Action<TaskModel> viewAction,
       Action<string> renameAction,
       Action<TaskModel> editAction,
       Action<TaskModel> duplicateTaskAction,
       Action<TaskModel> moveUpAction,
       Action<TaskModel> moveDownAction,
       Action<TaskModel> removeAction
    )
    {
        this.taskModel = task;

        btn_preview.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsListTask(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            GameObject modalViewTaskInstantiate = Instantiate(modalViewTaskPrefab, activeCanvas.transform);
            ModalViewTaskController modalViewController = modalViewTaskInstantiate.GetComponent<ModalViewTaskController>();
            modalViewController.ModalViewTaskConstructor(
                () => {
                    viewAction(task);
                }
            );
        });

        btn_rename.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsListTask(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            GameObject modalRenameInstantiate = Instantiate(modalRenameTaskPrefab, activeCanvas.transform);
            ModalRenameTaskController modalRenameController = modalRenameInstantiate.GetComponent<ModalRenameTaskController>();
            modalRenameController.ModalRenameTaskControllerConstructor(
                this.taskModel.Name,
                (taskname) => {
                    renameAction(taskname);
                }
            );
        });

        btn_edit.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsListTask(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            GameObject modalEditInstantiate = Instantiate(modalEditTaskPrefab, activeCanvas.transform);
            ModalEditTaskController modalEditController = modalEditInstantiate.GetComponent<ModalEditTaskController>();
            modalEditController.ModalEditTaskConstructor(
                () => {
                    editAction(this.taskModel);
                }
            );
        });

        btn_duplicate.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsListTask(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            GameObject modalDuplicateInstantiate = Instantiate(modalDuplicateTaskPrefab, activeCanvas.transform);
            ModalDuplicateTask modalDuplicateController = modalDuplicateInstantiate.GetComponent<ModalDuplicateTask>();
            modalDuplicateController.ModalDuplicateTaskConstructor(
                () => { duplicateTaskAction(this.taskModel); }
            );
        });

        btn_moveUp.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsListTask(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            moveUpAction(this.taskModel);
        });

        btn_moveDown.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsListTask(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            moveDownAction(this.taskModel);
        });

        btn_remove.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsListTask(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            GameObject modalRemoveInstantiate = Instantiate(modalRemoveTaskPrefab, activeCanvas.transform);
            ModalRemoveTaskController modalRemoveController = modalRemoveInstantiate.GetComponent<ModalRemoveTaskController>();
            modalRemoveController.ModalRemoveTaskConstructor(
                () => { removeAction(this.taskModel); }
            );
        });

    }

}
