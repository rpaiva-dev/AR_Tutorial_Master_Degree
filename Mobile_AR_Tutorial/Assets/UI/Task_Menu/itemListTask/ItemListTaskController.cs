using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemListTaskController : MonoBehaviour
{
    private Canvas activeCanvas;
    private TaskModel taskModel;

    public GameObject optionsListTaskPrefab;

    private Button btnOptionsTask;
    private GameObject optionsListTask;
    private GameObject overlay;

    private TextMeshProUGUI txt_TaskName;
    private TextMeshProUGUI txt_TaskNumber;

    private Button btn_preview;
    public GameObject modalViewTaskPrefab;


    private void Awake()
    {
        activeCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        btnOptionsTask = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btnOptionsTask");
        btnOptionsTask.onClick.AddListener(() => optionsTutorial());

        txt_TaskName = this.GetComponentsInChildren<TextMeshProUGUI>(true)
                        .FirstOrDefault(txt => txt.name == "txt_TaskName");

        txt_TaskNumber = this.GetComponentsInChildren<TextMeshProUGUI>(true)
                        .FirstOrDefault(txt => txt.name == "txt_TaskNumber");

        btn_preview = this.GetComponentsInChildren<Button>(true)
                        .FirstOrDefault(txt => txt.name == "btn_preview");
        btn_preview.onClick.AddListener(() => {
            GameObject modalViewTutorialInstantiate = Instantiate(modalViewTaskPrefab, activeCanvas.transform);
            ModalViewTaskController modalViewController = modalViewTutorialInstantiate.GetComponent<ModalViewTaskController>();
            modalViewController.ModalViewTaskConstructor(
                () => {
                    previewTask(this.taskModel);
                }
            );
        });
    }

    public void ItemListTaskControllerConstructor(TaskModel taskModel)
    {
        this.taskModel = taskModel;

        txt_TaskName.text = taskModel.Name;
        txt_TaskNumber.text = $"{TutorialModel.listTasksEddited.IndexOf(taskModel) + 1}";
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
            optionsListTask = Instantiate(optionsListTaskPrefab, activeCanvas.transform);
            CreateOverlay();
            optionsListTask.transform.SetAsLastSibling();

            RectTransform menuOptionsTutorialRectTransform = optionsListTask.GetComponent<RectTransform>();
            RectTransform btnMenuRectTransform = btnOptionsTask.GetComponent<RectTransform>();

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
            OptionsListTaskController optionsList = optionsListTask.GetComponent<OptionsListTaskController>();
            optionsList.constructorTaskMenuController(
                this.taskModel,
                (Task) => previewTask(Task),
                (taskname) => renameTaskAsync(taskname),
                (Task) => editTask(Task),
                (Task) => duplicateTask(Task),
                (Task) => moveUpTask(Task),
                (Task) => moveDownTask(Task),
                (Task) => removeTask(Task)
            );
        }
    }


    private void moveDownTask(TaskModel task)
    {
        int currentIndex = TutorialModel.listTasksEddited.IndexOf(task);

        if (currentIndex < TutorialModel.listTasksEddited.Count - 1)
        {
            var temp = TutorialModel.listTasksEddited[currentIndex + 1];
            TutorialModel.listTasksEddited[currentIndex + 1] = task;
            TutorialModel.listTasksEddited[currentIndex] = temp;
        }

        StartCoroutine(DelayedReloadScene(0.5f));
    }

    private void moveUpTask(TaskModel task)
    {
        int currentIndex = TutorialModel.listTasksEddited.IndexOf(task);

        if (currentIndex > 0)
        {
            var temp = TutorialModel.listTasksEddited[currentIndex - 1];
            TutorialModel.listTasksEddited[currentIndex - 1] = task;
            TutorialModel.listTasksEddited[currentIndex] = temp;
        }

        StartCoroutine(DelayedReloadScene(0.5f));
    }

    private void previewTask(TaskModel task)
    {
        TutorialModel.indexTaskEdited = TutorialModel.listTasksEddited.IndexOf(task);
        SceneManager.LoadScene("Tutorial_PreView");
    }

    private void renameTaskAsync(String newName)
    {
        TutorialModel.listTasksEddited[TutorialModel.listTasksEddited.IndexOf(this.taskModel)].Name = newName;

        showSucessMessage("Tarefa renomeada!", "Tarefa renomeada com sucesso!", 1.5f);
        StartCoroutine(DelayedReloadScene(2f));
    }

    private void editTask(TaskModel task)
    {
        TutorialModel.indexTaskEdited = TutorialModel.listTasksEddited.IndexOf(task);
        SceneManager.LoadScene("Task_Create");
    }

    public GameObject modalEditTutorialPrefab;
    private void duplicateTask(TaskModel Task)
    {
        TaskModel dupliTask = new TaskModel
        {
            Name = Task.Name + " (Duplicada)"
        };

        int originalIndex = TutorialModel.listTasksEddited.IndexOf(Task);
        TutorialModel.listTasksEddited.Insert(originalIndex + 1, dupliTask);

        showSucessMessage("Task duplicado!", $"Task duplicado com sucesso, seu nome é {dupliTask.Name}", 1.5f);
        StartCoroutine(DelayedReloadScene(2f));
    }

    private void removeTask(TaskModel Task)
    {
        showSucessMessage("Tarefa Removida!", "Tarefa removida com sucesso", 1.5f);
        TutorialModel.listTasksEddited.Remove(Task);

        StartCoroutine(DelayedReloadScene(2f));
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
            if (optionsListTask != null)
            {
                Destroy(optionsListTask);
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
    }

    public GameObject modalSucessPrefab;

    private void showSucessMessage(string header, string message, float time)
    {
        GameObject modalSucessPrefabInstantiate = Instantiate(modalSucessPrefab, activeCanvas.transform);
        ModalSucessController modalSucessController = modalSucessPrefabInstantiate.GetComponent<ModalSucessController>();
        modalSucessController.showMessage(header, message, time);
    }

    private IEnumerator DelayedReloadScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
