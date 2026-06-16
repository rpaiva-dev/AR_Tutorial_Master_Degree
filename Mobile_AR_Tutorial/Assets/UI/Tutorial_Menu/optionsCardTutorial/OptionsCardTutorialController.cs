using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsCardTutorialController : MonoBehaviour
{

    private Canvas activeCanvas;

    private Button btn_view;
    private Button btn_rename;
    private Button btn_edit;
    private Button btn_duplicate;
    private Button btn_remove;

    public GameObject modalViewTutorialPrefab;
    public GameObject modalRenameTutorialPrefab;
    public GameObject modalEditTutorialPrefab;
    public GameObject modalDuplicateTutorialPrefab;
    public GameObject modalRemoveTutorialPrefab;


    //private GameObject errorObject;
    //private TextMeshProUGUI txtError;

    void Awake()
    {
        activeCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        btn_view = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_view");

        btn_rename = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_rename");

        btn_edit = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_edit");

        btn_duplicate = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_duplicate");

        btn_remove = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_remove");
    }

    public void constructorTutorialMenuController(
       TutorialModel tutorial,
       List<TutorialModel> currentTutoriaisList,
       Action<TutorialModel> viewAction,
       Action<TutorialModel, List<TutorialModel>, string> renameAction,
       Action<TutorialModel> editAction,
       Action<TutorialModel, List<TutorialModel>> duplicateTutorialAction,
       Action<TutorialModel, List<TutorialModel>> removeAction
    )
    {
        btn_view.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsCardTutorial(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            GameObject modalViewTutorialInstantiate = Instantiate(modalViewTutorialPrefab, activeCanvas.transform);
            ModalViewTutorialController modalViewController = modalViewTutorialInstantiate.GetComponent<ModalViewTutorialController>();
            modalViewController.ModalViewTutorialConstructor(
                () => {
                    viewAction(tutorial);
                }
            );
        });

        btn_rename.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsCardTutorial(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            GameObject modalRenameInstantiate = Instantiate(modalRenameTutorialPrefab, activeCanvas.transform);
            ModalRenameTutorialController modalRenameController = modalRenameInstantiate.GetComponent<ModalRenameTutorialController>();
            modalRenameController.ModalRenameTutorialControllerConstructor(
                (tutorial, new_name) => {
                    renameAction(tutorial, currentTutoriaisList, new_name);
                },
                tutorial
            );
        });

        btn_edit.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsCardTutorial(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            GameObject modalEditInstantiate = Instantiate(modalEditTutorialPrefab, activeCanvas.transform);
            ModalEditTutorialController modalEditController = modalEditInstantiate.GetComponent<ModalEditTutorialController>();
            modalEditController.ModalEditTutorialConstructor(
                () => {
                    editAction(tutorial);
                }
            );
        });

        btn_duplicate.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsCardTutorial(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            GameObject modalDuplicateInstantiate = Instantiate(modalDuplicateTutorialPrefab, activeCanvas.transform);
            ModalDuplicateTutorial modalDuplicateController = modalDuplicateInstantiate.GetComponent<ModalDuplicateTutorial>();
            modalDuplicateController.ModalDuplicateTutorialConstructor(
                () => { duplicateTutorialAction(tutorial, currentTutoriaisList); }
            );
        });

        btn_remove.onClick.AddListener(() => {
            Destroy(GameObject.Find("optionsCardTutorial(Clone)"));
            Destroy(GameObject.Find("Overlay"));

            GameObject modalRemoveInstantiate = Instantiate(modalRemoveTutorialPrefab, activeCanvas.transform);
            ModalRemoveTutorialController modalRemoveController = modalRemoveInstantiate.GetComponent<ModalRemoveTutorialController>();
            modalRemoveController.ModalRemoveTutorialConstructor(
                () => { removeAction(tutorial, currentTutoriaisList); }
            );
        });

    }

}
