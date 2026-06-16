using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ModalAddTaskController : MonoBehaviour
{
    private Button btn_Close;
    private TMP_InputField input_TaskName;
    private TextMeshProUGUI txt_ExplanationText;
    private Button btn_CreateTask;
    private Action<TaskModel> createTaskAction;

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

        btn_CreateTask = modalContent.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_CreateTask");
        btn_CreateTask.onClick.AddListener(() => CreateTask());
    }

    private void Start()
    {
        input_TaskName.ActivateInputField();
    }

    private async void CreateTask()
    {
        TaskModel newTask = new TaskModel();

        newTask.Name = input_TaskName.text;
        input_TaskName.onValueChanged.AddListener((string newValue) => newTask.Name = newValue);

        if (string.IsNullOrEmpty(input_TaskName.text.Trim()))
        {
            ShowMessageError("Nome em branco! Coloque um nome");
        }
        else
        {
            TutorialRepository tutorialRepository = GameObject.Find("MainScripts").GetComponent<TutorialRepository>();
            await tutorialRepository.GetTutorialByName(newTask.Name.Trim(),
                (isSucess, message, tutorialReturned) => {
                    if (tutorialReturned != null)
                    {
                        ShowMessageError("Esse nome de tarefa é inválido, ele já existe! tente outro nome!");
                    }
                    else
                    {
                        createTaskAction?.Invoke(newTask);
                        Destroy(gameObject);
                    }
                });
        }
    }

    private void ShowMessageError(string Message)
    {
        txt_ExplanationText.text = Message;
        txt_ExplanationText.color = Color.red;
        txt_ExplanationText.fontStyle = FontStyles.Bold;
    }

    public void ModalAddTaskControllerConstructor(Action<TaskModel> createTaskAction)
    {
        this.createTaskAction = createTaskAction;
    }

}
