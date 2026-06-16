using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardTutorialController : MonoBehaviour
{
    private Canvas activeCanvas;

    public GameObject optionsCardTutorialPrefab;

    private Button btnOptionsTutorial;
    private GameObject optionsCardTutorial;
    private GameObject overlay;

    private TutorialModel tutorial;
    private List<TutorialModel> listaTutoriais;
    private Action<List<TutorialModel>> ActionReload;
    private TextMeshProUGUI txt_TutorialName;
    private TextMeshProUGUI txt_numberTaks;

    private Button btn_view;
    public GameObject modalViewTutorialPrefab;

    private TutorialRepository tutorialRepository;


    private void Awake()
    {
        activeCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        tutorialRepository = GameObject.Find("MainScripts").GetComponent<TutorialRepository>();


        btnOptionsTutorial = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btnOptionsTutorial");
        btnOptionsTutorial.onClick.AddListener(() => optionsTutorial());

        txt_TutorialName = this.GetComponentsInChildren<TextMeshProUGUI>(true)
                        .FirstOrDefault(txt => txt.name == "txt_TutorialName");

        txt_numberTaks = this.GetComponentsInChildren<TextMeshProUGUI>(true)
                        .FirstOrDefault(txt => txt.name == "txt_numberTaks");

        btn_view = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_view");
        btn_view.onClick.AddListener(() => {
            GameObject modalViewTutorialInstantiate = Instantiate(modalViewTutorialPrefab, activeCanvas.transform);
            ModalViewTutorialController modalViewController = modalViewTutorialInstantiate.GetComponent<ModalViewTutorialController>();
            modalViewController.ModalViewTutorialConstructor(
                () => {
                    viewTutorial(tutorial);
                }
            );
        });
    }

    public void CardTutorialControllerConstructor(TutorialModel tutorialModel, List<TutorialModel> listaTutoriais, Action<List<TutorialModel>> ActionReload)
    {
        this.tutorial = tutorialModel;
        this.listaTutoriais = listaTutoriais;
        this.ActionReload = ActionReload;

        txt_TutorialName.text = tutorialModel.Name;
        txt_numberTaks.text = $"{tutorialModel.Tasks.Count} tarefas";
    }

    private void optionsTutorial()
    {
        GameObject currentObject = GameObject.Find("optionsCardTutorial(Clone)");
        if (currentObject != null)
        {
            Destroy(currentObject);
        }
        else
        {
            // Instancia o menu de opções
            optionsCardTutorial = Instantiate(optionsCardTutorialPrefab, activeCanvas.transform);
            CreateOverlay();
            optionsCardTutorial.transform.SetAsLastSibling();

            RectTransform menuOptionsTutorialRectTransform = optionsCardTutorial.GetComponent<RectTransform>();
            RectTransform btnMenuRectTransform = btnOptionsTutorial.GetComponent<RectTransform>();

            // Calcula a posição mundial do botão
            Vector3 btnMenuWorldPosition = btnMenuRectTransform.TransformPoint(btnMenuRectTransform.rect.center);

            // Calcula o deslocamento inicial
            Vector3 offset = new Vector3(
                -btnMenuRectTransform.rect.width / 2 - menuOptionsTutorialRectTransform.rect.width / 2,
                btnMenuRectTransform.rect.height / 2 - menuOptionsTutorialRectTransform.rect.height / 2,
                0
            );

            // Aplica o deslocamento inicial
            menuOptionsTutorialRectTransform.position = btnMenuWorldPosition + offset;

            // Verifica se o menu está fora dos limites do Canvas e ajusta sua posição
            RectTransform canvasRectTransform = activeCanvas.GetComponent<RectTransform>();
            Vector3[] menuWorldCorners = new Vector3[4];
            menuOptionsTutorialRectTransform.GetWorldCorners(menuWorldCorners);

            // Obtém os limites do Canvas em coordenadas mundiais
            Vector3 canvasMin = canvasRectTransform.TransformPoint(canvasRectTransform.rect.min);
            Vector3 canvasMax = canvasRectTransform.TransformPoint(canvasRectTransform.rect.max);

            // Calcula ajustes para os limites superior, inferior, esquerdo e direito
            float offsetX = 0f;
            float offsetY = 0f;

            // Ajuste para o limite inferior
            if (menuWorldCorners[0].y < canvasMin.y)
            {
                offsetY = canvasMin.y - menuWorldCorners[0].y + 20; // Adiciona espaçamento
            }

            // Ajuste para o limite superior
            if (menuWorldCorners[1].y > canvasMax.y)
            {
                offsetY = canvasMax.y - menuWorldCorners[1].y - 20;
            }

            // Ajuste para o limite esquerdo
            if (menuWorldCorners[0].x < canvasMin.x)
            {
                offsetX = canvasMin.x - menuWorldCorners[0].x + 20;
            }

            // Ajuste para o limite direito
            if (menuWorldCorners[2].x > canvasMax.x)
            {
                offsetX = canvasMax.x - menuWorldCorners[2].x - 20;
            }

            // Aplica os ajustes calculados
            menuOptionsTutorialRectTransform.position += new Vector3(offsetX, offsetY, 0);

            // Configuração das opções do menu
            OptionsCardTutorialController optionsCard = optionsCardTutorial.GetComponent<OptionsCardTutorialController>();
            optionsCard.constructorTutorialMenuController(
                this.tutorial,
                listaTutoriais,
                (tutorial) => viewTutorial(tutorial),
                (tutorial, listaTutoriais, new_name) => renameTutorial(tutorial, listaTutoriais, new_name, ActionReload),
                (tutorial) => editTutorial(tutorial),
                (tutorial, listaTutoriais) => duplicateTutorial(tutorial, listaTutoriais, ActionReload),
                (tutorial, listaTutoriais) => removeTutorial(tutorial, listaTutoriais, ActionReload)
            );
        }
    }


    private void viewTutorial(TutorialModel tutorial)
    {
        if (tutorial.Tasks.Count == 0)
        {
            showInfoMessage("Tutorial não possui tarefas", "Esse tutorial não tem nenhuma tarefa, clique em editar e adicione alguma tarefa", 3f);
        }
        else
        {
            TutorialModel.CurrentTutorial = tutorial;
            TutorialModel.indexTaskEdited = 0;
            SceneManager.LoadScene("Tutorial_View");
        }
    }

    private async void renameTutorial(TutorialModel tutorial, List<TutorialModel> currentTutorialList, string new_name, Action<List<TutorialModel>> ActionReload)
    {
        await tutorialRepository.SetNameTutorialAsync(tutorial.Id.ToString(), new_name,
            (isSucess, messaage) => {
                if (isSucess)
                {
                    showSucessMessage("Tutorial renomeado!","Tutorial renomeado com sucesso!", 1.5f);
                    currentTutorialList.Find(tut => tut.Id == tutorial.Id).Name = new_name;
                    ActionReload(currentTutorialList);
                }
                else
                {
                    showInfoMessage("Problema ao renomear o tutorial", "Ocorreu algum problema ao renomear o tutorial", 1.5f);
                }
            }
        );
    }

    private void editTutorial(TutorialModel tutorial)
    {
        TutorialModel.CurrentTutorial = tutorial;
        TutorialModel.indexTaskEdited = 0;
        SceneManager.LoadScene("Task_Menu");
    }

    public GameObject modalEditTutorialPrefab;
    private async void duplicateTutorial(TutorialModel tutorial, List<TutorialModel> currentTutorialList, Action<List<TutorialModel>> ActionReload)
    {
        if (tutorial != null)
        {
            TutorialModel dupliTut = new TutorialModel
            {
                Name = tutorial.Name + " (Duplicado)",
                Tasks = tutorial.Tasks
            };

            await tutorialRepository.CreateTutorialAsync(dupliTut,
            (isSucess, message, tutorialModel) => {
                if (isSucess)
                {
                    showSucessMessage("Tutorial duplicado!", $"Tutorial duplicado com sucesso, seu nome é {dupliTut.Name}", 2f);

                    Destroy(GameObject.Find("optionsCardTutorial(Clone)"));
                    Destroy(GameObject.Find("Overlay"));

                    int originalIndex = currentTutorialList.IndexOf(tutorial);
                    if (originalIndex != -1)
                    {
                        currentTutorialList.Insert(originalIndex + 1, dupliTut);
                    }

                    ActionReload(currentTutorialList);

                    GameObject modalEditInstantiate = Instantiate(modalEditTutorialPrefab, activeCanvas.transform);
                    ModalEditTutorialController modalEditController = modalEditInstantiate.GetComponent<ModalEditTutorialController>();
                    modalEditController.ModalEditTutorialConstructor(
                        () => {
                            editTutorial(tutorial);
                        }
                    );
                }
                else
                {
                    showInfoMessage("Erro ao duplicar Tutorial", "Ocorreu um erro ao renomear o tutorial", 1.5f);
                }
            });
        }
    }

    private async void removeTutorial(TutorialModel tutorial, List<TutorialModel> currentTutorialList, Action<List<TutorialModel>> ActionReload)
    {
        await tutorialRepository.DeleteTutorialAsync(tutorial.Id.ToString(),
        (isSuccess, messageString) => { });

        showSucessMessage("Tutorial Removido!", "Tutorial removido com sucesso", 1.5f);
        currentTutorialList.Remove(tutorial);
        ActionReload(currentTutorialList);
    }

    private void CreateOverlay()
    {
        // Cria o overlay
        overlay = new GameObject("Overlay");
        RectTransform overlayRectTransform = overlay.AddComponent<RectTransform>();
        overlayRectTransform.SetParent(activeCanvas.transform, false);
        overlayRectTransform.anchorMin = Vector2.zero;
        overlayRectTransform.anchorMax = Vector2.one;
        overlayRectTransform.sizeDelta = Vector2.zero;

        // Define a aparência e o comportamento do overlay
        Image overlayImage = overlay.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0); // Transparente

        overlay.AddComponent<GraphicRaycaster>();

        // Adiciona um EventTrigger para detectar cliques no overlay
        EventTrigger trigger = overlay.AddComponent<EventTrigger>();

        // Configura evento de PointerDown para destruir o menu de opções e o overlay
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown // Detecta apenas quando o clique começa (evita conflitos com arrasto)
        };
        entry.callback.AddListener((eventData) =>
        {
            // Certifique-se de destruir o menu e o overlay ao clicar no overlay
            if (optionsCardTutorial != null)
            {
                Destroy(optionsCardTutorial);
            }
            Destroy(overlay);
        });
        trigger.triggers.Add(entry);
    }


    public GameObject modalInfoPrefab;
    private void showInfoMessage(string header, string message, float time)
    {
        GameObject modalInfoPrefabInstantiate = Instantiate(modalInfoPrefab, activeCanvas.transform);
        ModalInfoController modalInfoController = modalInfoPrefabInstantiate.GetComponent<ModalInfoController>();
        modalInfoController.showMessage(header, message, time);

        //Resolvendo um bug
        GameObject modal_view = GameObject.Find("modalViewTutorial(Clone)").gameObject;
        if (modal_view)
            Destroy(modal_view);
    }


    public GameObject modalSucessPrefab;

    private void showSucessMessage(string header, string message, float time)
    {
        GameObject modalSucessPrefabInstantiate = Instantiate(modalSucessPrefab, activeCanvas.transform);
        ModalSucessController modalSucessController = modalSucessPrefabInstantiate.GetComponent<ModalSucessController>();
        modalSucessController.showMessage(header, message, time);
    }
}
