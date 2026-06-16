using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialMenuController : MonoBehaviour
{
    private Canvas activeCanvas;
    private Button btn_AddTutorial;

    private TutorialRepository tutorialRepository;

    public GameObject cardsTutorial;
    private GameObject listTutorialCardsParent;
    private TMP_InputField input_ResearchTutoriais;

    List<TutorialModel> listTutorialModels;

    void Awake()
    {
        //canvas
        activeCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        //repository
        tutorialRepository = GameObject.Find("MainScripts").GetComponent<TutorialRepository>();

        //btn to add tutorials
        btn_AddTutorial = GameObject.Find("btn_AddTutorial").GetComponent<Button>();
        btn_AddTutorial.onClick.AddListener(() => modalAddTutorial());

        //parent to instantiate cards
        listTutorialCardsParent = GameObject.Find("listTutorialCardsParent");

        input_ResearchTutoriais = GameObject.Find("input_ResearchTutoriais").GetComponent<TMP_InputField>();
        input_ResearchTutoriais.onValueChanged.AddListener((search) => {
            FilterTutorialByName(search);
        });

        listTutorialModels = new List<TutorialModel>();
    }

    private void FilterTutorialByName(string search)
    {
        List<TutorialModel> newTutorialList;

        if (string.IsNullOrEmpty(search))
        {
            newTutorialList = listTutorialModels;
        }
        else
        {
            newTutorialList = listTutorialModels
                .Where(tut =>
                    tut.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();
        }

        InstantiateTutorialCards(newTutorialList);
    }

    public void Start()
    {

        //if (TutorialModel.CurrentUserState == TutorialModel.UserState.FirstTime)
        //confirmationUserXperience();
        GetAndInstantiateTutorials();
    }

    private async void GetAndInstantiateTutorials()
    {
        await tutorialRepository.GetAllTutorialsAsync((isSuccess, message, listaTutorial) =>
        {
            if (isSuccess)
            {
                this.listTutorialModels = listaTutorial;
                InstantiateTutorialCards(listaTutorial);
            }
            else
            {
                //showMessage(message, 3f);
            }
        });
    }

    private void InstantiateTutorialCards(List<TutorialModel> tutorialModels)
    {
        foreach (Transform cards in listTutorialCardsParent.transform)
        {
            Destroy(cards.gameObject);
        }

        for (int i = 0; i < tutorialModels.Count; i += 2)
        {
            List<TutorialModel> newlist = new List<TutorialModel>();
            newlist.Add(tutorialModels[i]);

            if (i + 1 < tutorialModels.Count)
            {
                newlist.Add(tutorialModels[i + 1]);
            }

            GameObject tutorialPrefabInstantiate = Instantiate(cardsTutorial, listTutorialCardsParent.transform);

            GroupCardsTutorialController tutorialCardsController = tutorialPrefabInstantiate.GetComponent<GroupCardsTutorialController>();
            tutorialCardsController.GroupCardsTutorialControllerConstructor(newlist, tutorialModels, (tutorialModels) => {
                this.listTutorialModels = tutorialModels;
                InstantiateTutorialCards(listTutorialModels);
            });
        }

        RectTransform rectTransform = listTutorialCardsParent.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

    }


    public GameObject modalAddTutorialPrefab;
    private void modalAddTutorial()
    {
        GameObject modalAddTutorial = Instantiate(modalAddTutorialPrefab, activeCanvas.transform);
        ModalAddTutorialController modalController = modalAddTutorial.GetComponent<ModalAddTutorialController>();
        modalController.ModalAddTutorialControllerConstructor(
            (tutorial) => { createTutorial(tutorial); }
        );
    }

    private async void createTutorial(TutorialModel tutorialModel)
    {
        await tutorialRepository.CreateTutorialAsync(tutorialModel,
            async (isSuccess, message, newTutorial) =>
            {
                if (isSuccess)
                {
                    TutorialModel.CurrentTutorial = newTutorial;
                    showSucessMessage("Tutorial criado", "Você será direcionado para as tarefas", 2f);

                    await Task.Delay(2000);
                    SceneManager.LoadScene("Task_Menu");
                }
                else
                {
                    showAlertMessage("Erro ao criar tutorial!", message, 3f);
                }
            });
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
