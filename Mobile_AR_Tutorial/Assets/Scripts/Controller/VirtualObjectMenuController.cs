using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VirtualObjectMenuController : MonoBehaviour
{
    private GameObject header;

    private Canvas activeCanvas;
    private GameObject Axis3DObject;

    private Button btn_AddVirtualObject;
    private Button btn_FinishTask;
    private Action<VirtualObjectModel.TypeVirtualObject> instantiateVirtualObjectAction;
    private Action<VirtualObjectModel> selectedGameObjectToEditeAction;
    private Action<VirtualObjectModel> removeVirtualObjectAction;
    private TaskModel editedTask;
    private GameObject virtualObjectNameModal;
    private TextMeshProUGUI txt_VirtualObjectName;

    //Edit objuect Buttons
    private GameObject editButtons;

    //editButtons buttons
    private Button btnPosition;
    private Button btnRotation;
    private Button btnScale;
    private Button btnColor;
    private Button btn_RemoveObject;
    private Button btn_FinishObject;

    //edit part, modals and slider of colors, and text
    private GameObject EditAxisAndColors;
    private GameObject CUIColorPickerModal;
    private GameObject AxisEditVirtualObject;
    private GameObject EditTextComponents;
    private AxisEditVirtualObjectController controllerAxisEditModal;
    private CUIColorPickerController controllerColorModal;
    private EditTextComponentController controllerTextModal;


    private Camera mainCamera;
    private RectTransform clickableArea;

    private void Awake()
    {
        GameObject clickableAreaObject = GameObject.Find("clickableArea");
        if (clickableAreaObject != null)
        {
            clickableArea = clickableAreaObject.GetComponent<RectTransform>();
            if (clickableArea == null)
            {
                Debug.LogError("O GameObject 'clickableArea' foi encontrado, mas não possui um componente RectTransform.");
            }
        }
        else
        {
            Debug.LogError("O GameObject 'clickableArea' não foi encontrado na cena.");
        }

        // Inicializa a câmera principal
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Câmera principal (Camera.main) não foi encontrada. Verifique se há uma câmera na cena marcada como 'Main Camera'.");
        }

        // Inicialização restante do UI e outros componentes
        activeCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        Axis3DObject = GameObject.Find("Axis3DObject");
        Axis3DObject?.SetActive(false);

        header = GameObject.Find("header");

        virtualObjectNameModal = GameObject.Find("virtualObjectNameModal");
        txt_VirtualObjectName = virtualObjectNameModal?.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(x => x.name == "txt_VirtualObjectName");
        virtualObjectNameModal?.SetActive(false);

        btn_AddVirtualObject = GameObject.Find("btn_AddVirtualObject")?.GetComponent<Button>();
        btn_AddVirtualObject?.onClick.AddListener(() => modalAddVirtualObject());

        btn_FinishTask = GameObject.Find("btn_FinishTask").GetComponent<Button>();
        btn_FinishTask.onClick.AddListener(() => modaFinishTask());

        btn_RemoveObject = GameObject.Find("btn_RemoveObject")?.GetComponent<Button>();
        btn_FinishObject = GameObject.Find("btn_FinishObject")?.GetComponent<Button>();
        btn_RemoveObject?.gameObject.SetActive(false);
        btn_FinishObject?.gameObject.SetActive(false);

        CUIColorPickerModal = GameObject.Find("CUIColorPicker");
        CUIColorPickerModal?.SetActive(false);

        AxisEditVirtualObject = GameObject.Find("AxisEditVirtualObject");
        AxisEditVirtualObject?.SetActive(false);

        EditTextComponents = GameObject.Find("EditTextComponents");
        EditTextComponents?.SetActive(false);

        controllerAxisEditModal = AxisEditVirtualObject?.GetComponent<AxisEditVirtualObjectController>();
        controllerColorModal = CUIColorPickerModal?.GetComponent<CUIColorPickerController>();
        controllerTextModal = EditTextComponents?.GetComponent<EditTextComponentController>();

        editButtons = GameObject.Find("editButtons");
        btnPosition = editButtons?.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "btnPosition");
        btnRotation = editButtons?.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "btnRotation");
        btnScale = editButtons?.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "btnScale");
        btnColor = editButtons?.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "btnColor");
        editButtons?.SetActive(false);

        EditAxisAndColors = GameObject.Find("EditAxisAndColors");
        EditAxisAndColors?.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && header.activeSelf) getObjectByTouch();
    }

    private void getObjectByTouch()
    {
        if (clickableArea == null)
        {
            Debug.LogError("clickableArea não foi inicializado. Certifique-se de que o objeto existe e foi encontrado no método Awake.");
            return;
        }

        if (IsPointerOverClickableArea())
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.transform.gameObject;
                StartCoroutine(changeObjectClicked(clickedObject));

                GameObject parent = clickedObject.transform.parent.gameObject;
                if (parent.name.Length > 1)
                {
                    string indexStr = parent.name[1].ToString();
                    print("indexStr: " + indexStr);

                    if (Int32.TryParse(indexStr, out int indexOfVirtualObj))
                    {
                        if (indexOfVirtualObj >= 0 && indexOfVirtualObj < editedTask.VirtualObjects.Count)
                        {
                            VirtualObjectModel currentVirtualObject = editedTask.VirtualObjects[indexOfVirtualObj];
                            currentVirtualObject.GameObject = parent;
                            SetToEditObjectLayout(currentVirtualObject);
                        }
                        else
                        {
                            Debug.LogError($"Índice {indexOfVirtualObj} fora do intervalo da lista de objetos virtuais.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Falha ao converter o índice do objeto virtual.");
                    }
                }
                else
                {
                    Debug.LogError("O nome do objeto pai é muito curto para conter um índice.");
                }
            }
        }
        else
        {
            Debug.Log("Clique fora da área clicável.");
        }

        bool IsPointerOverClickableArea()
        {
            Vector2 localMousePosition = clickableArea.InverseTransformPoint(Input.mousePosition);
            return clickableArea.rect.Contains(localMousePosition);
        }
    }

    private IEnumerator changeObjectClicked(GameObject clickedObject)
    {
        TextMeshPro tmp = clickedObject.GetComponent<TextMeshPro>();
        MeshRenderer meshRenderer = clickedObject.GetComponentInChildren<MeshRenderer>();

        Color initialColor;

        if (tmp)
        {
            initialColor = tmp.color;
            tmp.color = Color.red;
        }
        else if (meshRenderer)
        {
            initialColor = meshRenderer.material.color;
            meshRenderer.material.color = Color.red;
        }
        else
        {
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        if (tmp)
        {
            tmp.color = initialColor;
        }
        else if (meshRenderer)
        {
            meshRenderer.material.color = initialColor;
        }
    }


    public void VirtualObjectMenuControllerConstructor(Action<VirtualObjectModel.TypeVirtualObject> instantiateVirtualObjectAction,
    Action<VirtualObjectModel> selectedGameObjectToEdit_,
    Action<VirtualObjectModel> removeVirtualObjectAction,
    TaskModel editedTask)
    {
        this.instantiateVirtualObjectAction = instantiateVirtualObjectAction;
        this.selectedGameObjectToEditeAction = selectedGameObjectToEdit_;
        this.removeVirtualObjectAction = removeVirtualObjectAction;
        this.editedTask = editedTask;
    }

    public void SetToEditObjectLayout(VirtualObjectModel virtualObj)
    {
        EditAxisAndColors.SetActive(true);
        Axis3DObject.SetActive(true);

        // Hide unnecessary UI
        btn_AddVirtualObject.gameObject.SetActive(false);
        btn_FinishTask.gameObject.SetActive(false);
        header.SetActive(false);

        // Display edit UI
        virtualObjectNameModal.SetActive(true);
        txt_VirtualObjectName.text = virtualObj.typeVirtualObject.ToString();

        btn_RemoveObject.gameObject.SetActive(true);
        btn_RemoveObject.onClick.RemoveAllListeners();
        btn_RemoveObject.onClick.AddListener(() => modalRemoveVirtualObject(virtualObj));

        btn_FinishObject.gameObject.SetActive(true);
        btn_FinishObject.onClick.RemoveAllListeners();
        btn_FinishObject.onClick.AddListener(ResetToAddObjectLayout);

        ConfigureEditButtons(virtualObj);
    }

    public void ConfigureEditButtons(VirtualObjectModel virtualObj)
    {
        //config Edit Buttons
        editButtons.SetActive(true);
        btnPosition.onClick.RemoveAllListeners();
        btnRotation.onClick.RemoveAllListeners();
        btnScale.onClick.RemoveAllListeners();
        btnColor.onClick.RemoveAllListeners();

        /*Configuração inicial*/
        if (virtualObj.typeVirtualObject == VirtualObjectModel.TypeVirtualObject.Texto3D)
        {
            AxisEditVirtualObject.SetActive(false);
            CUIColorPickerModal.SetActive(false);
            EditTextComponents.SetActive(true);

            selectThisButton(btnScale);
            controllerTextModal.EditTextComponentControllerConstructor(virtualObj.Text, virtualObj.FontSize,
                (texto, fontSize) => {
                    virtualObj.Text = texto;
                    virtualObj.GameObject.GetComponentInChildren<TextMeshPro>().text = texto;
                    virtualObj.FontSize = fontSize;
                    virtualObj.GameObject.GetComponentInChildren<TextMeshPro>().fontSize = fontSize;

                    selectedGameObjectToEditeAction(virtualObj);
                });
        }
        else
        {
            AxisEditVirtualObject.SetActive(true);
            CUIColorPickerModal.SetActive(false);
            EditTextComponents.SetActive(false);
            selectThisButton(btnPosition);
            controllerAxisEditModal.AxisEditVirtualObjectControllerConstructorPosition(virtualObj.Position, (Vector3 position) =>
            {
                virtualObj.Position = position;
                virtualObj.GameObject.transform.localPosition = position;
                selectedGameObjectToEditeAction(virtualObj);
            });
        }

        btnPosition.onClick.AddListener(() => {
            AxisEditVirtualObject.SetActive(true);
            CUIColorPickerModal.SetActive(false);
            EditTextComponents.SetActive(false);

            selectThisButton(btnPosition);
            controllerAxisEditModal.AxisEditVirtualObjectControllerConstructorPosition(virtualObj.Position, (Vector3 position) =>
            {
                virtualObj.Position = position;
                virtualObj.GameObject.transform.localPosition = position;
                selectedGameObjectToEditeAction(virtualObj);
            });
        });

        btnRotation.onClick.AddListener(() => {
            AxisEditVirtualObject.SetActive(true);
            CUIColorPickerModal.SetActive(false);
            EditTextComponents.SetActive(false);

            selectThisButton(btnRotation);
            controllerAxisEditModal.AxisEditVirtualObjectControllerConstructorRotation(virtualObj.Rotation, (Vector3 rotation) =>
            {
                print($"virtualObj.Rotation: {virtualObj.Rotation}");
                print($"rotation: {rotation}");
                virtualObj.Rotation = rotation;
                virtualObj.GameObject.transform.localRotation = Quaternion.Euler(rotation);
                selectedGameObjectToEditeAction(virtualObj);
            });
        });


        Color initialColorVirtualObject = new Color();
        if (virtualObj.typeVirtualObject == VirtualObjectModel.TypeVirtualObject.Texto3D)
        {
            btnScale.GetComponentInChildren<TextMeshProUGUI>().text = "Texto";
            btnScale.onClick.AddListener(() =>
            {
                selectThisButton(btnScale);

                AxisEditVirtualObject.SetActive(false);
                CUIColorPickerModal.SetActive(false);
                EditTextComponents.SetActive(true);

                controllerTextModal.EditTextComponentControllerConstructor(virtualObj.Text, virtualObj.FontSize,
                    (texto, fontSize) => {
                        virtualObj.Text = texto;
                        virtualObj.GameObject.GetComponentInChildren<TextMeshPro>().text = texto;
                        virtualObj.FontSize = fontSize;
                        virtualObj.GameObject.GetComponentInChildren<TextMeshPro>().fontSize = fontSize;

                        selectedGameObjectToEditeAction(virtualObj);
                    });
            }
            );

            btnColor.onClick.AddListener(() => {
                AxisEditVirtualObject.SetActive(false);
                CUIColorPickerModal.SetActive(true);
                EditTextComponents.SetActive(false);

                selectThisButton(btnColor);
                initialColorVirtualObject = virtualObj.GameObject.GetComponentInChildren<TextMeshPro>().color;

                controllerColorModal.constructorModalBtnColorObject(
                    initialColorVirtualObject,
                    (Color selectedColor) => {
                        virtualObj.GameObject.GetComponentInChildren<TextMeshPro>().color = selectedColor;

                        virtualObj.Color = selectedColor;
                        selectedGameObjectToEditeAction(virtualObj);
                    }
                );
            });
        }
        else
        {
            btnScale.GetComponentInChildren<TextMeshProUGUI>().text = "Tamanho";
            btnScale.onClick.AddListener(() => {
                AxisEditVirtualObject.SetActive(true);
                CUIColorPickerModal.SetActive(false);
                EditTextComponents.SetActive(false);

                selectThisButton(btnScale);
                controllerAxisEditModal.AxisEditVirtualObjectControllerConstructorScale(virtualObj.Scale, (Vector3 scale) =>
                {
                    virtualObj.Scale = scale;
                    virtualObj.GameObject.transform.localScale = scale;
                    selectedGameObjectToEditeAction(virtualObj);
                });
            });

            btnColor.onClick.AddListener(() => {
                AxisEditVirtualObject.SetActive(false);
                CUIColorPickerModal.SetActive(true);
                EditTextComponents.SetActive(false);

                selectThisButton(btnColor);
                initialColorVirtualObject = virtualObj.GameObject.GetComponentInChildren<MeshRenderer>().material.color;

                controllerColorModal.constructorModalBtnColorObject(
                    initialColorVirtualObject,
                    (Color selectedColor) => {
                        virtualObj.GameObject.GetComponentInChildren<MeshRenderer>().material.color = selectedColor;

                        virtualObj.Color = selectedColor;
                        selectedGameObjectToEditeAction(virtualObj);
                    }
                );
            });
        }

    }

    private Color textSelectColor = Color.white; // Branco
    private Color32 textUnselectColor = new Color32(36, 39, 54, 255); // #242736
    private Color32 imageSelectColor = new Color32(0, 119, 182, 255); // #0077B6
    private Color32 imageUnselectColor = new Color32(202, 240, 248, 255); // #CAF0F8
    private void selectThisButton(Button btnSelected)
    {
        btnPosition.GetComponentInChildren<TextMeshProUGUI>().color = textUnselectColor;
        btnPosition.GetComponentsInChildren<Image>().FirstOrDefault(x => x.name == "background").color = imageUnselectColor;
        btnRotation.GetComponentInChildren<TextMeshProUGUI>().color = textUnselectColor;
        btnRotation.GetComponentsInChildren<Image>().FirstOrDefault(x => x.name == "background").color = imageUnselectColor;
        btnScale.GetComponentInChildren<TextMeshProUGUI>().color = textUnselectColor;
        btnScale.GetComponentsInChildren<Image>().FirstOrDefault(x => x.name == "background").color = imageUnselectColor;
        btnColor.GetComponentInChildren<TextMeshProUGUI>().color = textUnselectColor;
        btnColor.GetComponentsInChildren<Image>().FirstOrDefault(x => x.name == "background").color = imageUnselectColor;

        btnSelected.GetComponentInChildren<TextMeshProUGUI>().color = textSelectColor;
        btnSelected.GetComponentsInChildren<Image>().FirstOrDefault(x => x.name == "background").color = imageSelectColor;
    }

    private void ResetToAddObjectLayout()
    {
        btn_AddVirtualObject.gameObject.SetActive(true);
        btn_FinishTask.gameObject.SetActive(true);

        header.SetActive(true);

        virtualObjectNameModal.SetActive(false);
        btn_RemoveObject.gameObject.SetActive(false);
        btn_FinishObject.gameObject.SetActive(false);
        EditAxisAndColors.SetActive(false);
        editButtons.SetActive(false);

        Axis3DObject.SetActive(false);
    }


    public GameObject modalFinishCreateTaskPrefab;
    private void modaFinishTask()
    {
        GameObject modalFinishCreateTask = Instantiate(modalFinishCreateTaskPrefab, activeCanvas.transform);
        ModalFinishCreateTask modalController = modalFinishCreateTask.GetComponent<ModalFinishCreateTask>();
        modalController.ModalFinishTaskConstructor(
            async () => {
                TutorialModel.listTasksEddited[TutorialModel.indexTaskEdited] = editedTask;
                showSucessMessage("Mudanças salvas", "Você será direcionado para o menu anterior", 2f);
                editedTask = null;
                await Task.Delay(2000);
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

    public GameObject modalAddObjectPrefab;
    private void modalAddVirtualObject()
    {
        GameObject modalAddObject = Instantiate(modalAddObjectPrefab, activeCanvas.transform);
        ModalAddObjectController modalController = modalAddObject.GetComponent<ModalAddObjectController>();
        modalController.ModalAddObjectControllerConstructor(
            (VirtualObjectModel.TypeVirtualObject type) => {
                instantiateVirtualObjectAction(type);
                Destroy(modalAddObject);
            }
        );
    }


    public GameObject modalRemoveObject;

    private void modalRemoveVirtualObject(VirtualObjectModel selectedGameObjectToDelete)
    {
        GameObject modalRemoveController = Instantiate(modalRemoveObject, activeCanvas.transform);
        ModalRemoveObjectController modalController = modalRemoveController.GetComponent<ModalRemoveObjectController>();
        modalController.ModalRemoveObjectConstructor(
            () =>
            {
                removeVirtualObjectAction(selectedGameObjectToDelete);
                ResetToAddObjectLayout();
            }
        );
    }
}
