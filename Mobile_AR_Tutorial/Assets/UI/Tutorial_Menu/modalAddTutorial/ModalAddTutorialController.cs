using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ModalAddTutorialController : MonoBehaviour
{
    private Button btn_Close;
    private TMP_InputField input_TutorialName;
    private TextMeshProUGUI txt_ExplanationText;
    private Button btn_CreateTutorial;
    private Action<TutorialModel> createTutorialAction;

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

        txt_ExplanationText = modalContent.GetComponentsInChildren<TextMeshProUGUI>()
                        .FirstOrDefault(txt => txt.name == "txt_ExplanationText");

        btn_CreateTutorial = modalContent.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_CreateTutorial");
        btn_CreateTutorial.onClick.AddListener(() => CreateTutorial());
    }

    private void Start()
    {
        input_TutorialName.ActivateInputField();
    }

    private async void CreateTutorial()
    {
        TutorialModel newTutorial = new TutorialModel();

        newTutorial.Name = input_TutorialName.text;
        input_TutorialName.onValueChanged.AddListener((string newValue) => newTutorial.Name = newValue);

        if (string.IsNullOrEmpty(input_TutorialName.text.Trim()))
        {
            ShowMessageError("Nome em branco! Coloque um nome");
        }
        else
        {
            TutorialRepository tutorialRepository = GameObject.Find("MainScripts").GetComponent<TutorialRepository>();
            await tutorialRepository.GetTutorialByName(newTutorial.Name.Trim(),
                (isSucess, message, tutorialReturned) => {
                    if (tutorialReturned != null)
                    {
                        ShowMessageError("Esse nome de tutorial é inválido, ele já existe! tente outro nome!");
                    }
                    else
                    {
                        createTutorialAction?.Invoke(newTutorial);
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

    public void ModalAddTutorialControllerConstructor(Action<TutorialModel> createTutorialAction)
    {
        this.createTutorialAction = createTutorialAction;
    }

}
