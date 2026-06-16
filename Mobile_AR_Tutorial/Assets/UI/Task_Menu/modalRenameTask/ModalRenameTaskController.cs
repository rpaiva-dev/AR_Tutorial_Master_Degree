using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ModalRenameTaskController : MonoBehaviour
{
    private Button btn_Close;
    private TMP_InputField input_TaskName;
    private TextMeshProUGUI txt_ExplanationText;
    private Button btn_RenameTask;
    private Action<string> renameTaskAction;

    private void Awake()
    {
        Transform modal = this.gameObject.GetComponentsInChildren<Transform>()
                       .FirstOrDefault(txt => txt.name == "modal");

        btn_Close = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_Close");
        btn_Close.onClick.AddListener(() => Destroy(this.gameObject));

        Transform modalContent = this.gameObject.GetComponentsInChildren<Transform>()
                       .FirstOrDefault(txt => txt.name == "modalContent");

        input_TaskName = modalContent.GetComponentsInChildren<TMP_InputField>()
                        .FirstOrDefault(txt => txt.name == "input_TaskName");
        input_TaskName.text = String.Empty;


        txt_ExplanationText = modalContent.GetComponentsInChildren<TextMeshProUGUI>()
                        .FirstOrDefault(txt => txt.name == "txt_ExplanationText");

        btn_RenameTask = modalContent.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_RenameTask");
        btn_RenameTask.onClick.AddListener(() => RenameTask());
    }
    private void Start()
    {
        input_TaskName.ActivateInputField();
    }
    private void RenameTask()
    {
        if (string.IsNullOrEmpty(input_TaskName.text.Trim()))
        {
            ShowMessageError("Nome em branco! Coloque um nome.");
            return;
        }

        string newTaskName = input_TaskName.text.Trim();
        bool isNameTaken = TutorialModel.CurrentTutorial.Tasks
            .Any(task => string.Equals(task.Name, newTaskName, StringComparison.OrdinalIgnoreCase));

        if (isNameTaken)
        {
            ShowMessageError("Esse nome de Task é inválido, ele já existe! Tente outro nome.");
        }
        else
        {
            renameTaskAction?.Invoke(newTaskName);
            Destroy(gameObject);
        }
    }


    private void ShowMessageError(string Message)
    {
        txt_ExplanationText.text = Message;
        txt_ExplanationText.color = Color.red;
        txt_ExplanationText.fontStyle = FontStyles.Bold;
    }

    public void ModalRenameTaskControllerConstructor(string taskname, Action<string> renameTaskAction)
    {
        input_TaskName.text = taskname;
        this.renameTaskAction = renameTaskAction;
    }

}
