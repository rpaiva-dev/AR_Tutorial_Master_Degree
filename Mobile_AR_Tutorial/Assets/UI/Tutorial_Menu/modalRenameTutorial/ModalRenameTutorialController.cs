using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ModalRenameTutorialController : MonoBehaviour
{
    private Button btn_Close;
    private TMP_InputField input_TutorialName;
    private TextMeshProUGUI txt_ExplanationText;
    private Button btn_RenameTutorial;
    private TutorialModel tutorial;
    private Action<TutorialModel, String> renameTutorialAction;

    private void Awake()
    {
        Transform modal = this.gameObject.GetComponentsInChildren<Transform>()
                       .FirstOrDefault(txt => txt.name == "modal");

        btn_Close = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_Close");
        btn_Close.onClick.AddListener(() => Destroy(this.gameObject));

        Transform modalContent = this.gameObject.GetComponentsInChildren<Transform>()
                       .FirstOrDefault(txt => txt.name == "modalContent");

        input_TutorialName = modalContent.GetComponentsInChildren<TMP_InputField>()
                        .FirstOrDefault(txt => txt.name == "input_TutorialName");
        input_TutorialName.text = String.Empty;
        input_TutorialName.onValueChanged.AddListener((string newValue) => this.tutorial.Name = newValue);


        txt_ExplanationText = modalContent.GetComponentsInChildren<TextMeshProUGUI>()
                        .FirstOrDefault(txt => txt.name == "txt_ExplanationText");

        btn_RenameTutorial = modalContent.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_RenameTutorial");
        btn_RenameTutorial.onClick.AddListener(() => RenameTutorial());
    }
    private void Start()
    {
        input_TutorialName.ActivateInputField();
    }
    private async void RenameTutorial()
    {

        if (string.IsNullOrEmpty(input_TutorialName.text.Trim()))
        {
            ShowMessageError("Nome em branco! Coloque um nome");
        }
        else
        {
            TutorialRepository tutorialRepository = GameObject.Find("MainScripts").GetComponent<TutorialRepository>();
            await tutorialRepository.GetTutorialByName(input_TutorialName.text.Trim(),
                (isSucess, message, tutorialReturned) => {
                    if (tutorialReturned != null)
                    {
                        ShowMessageError($"Esse nome de tutorial é inválido, ele já existe! tente outro nome! {input_TutorialName.text.Trim()}");
                    }
                    else
                    {
                        renameTutorialAction?.Invoke(tutorial, input_TutorialName.text.Trim());
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

    public void ModalRenameTutorialControllerConstructor(Action<TutorialModel, String> renameTutorialAction, TutorialModel tutorial)
    {
        this.tutorial = tutorial;
        input_TutorialName.text = tutorial.Name;
        this.renameTutorialAction = renameTutorialAction;
    }

}
