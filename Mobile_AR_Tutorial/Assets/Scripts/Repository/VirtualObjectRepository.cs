using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class VirtualObjectRepository : MonoBehaviour
{
    private Canvas canvasActive;
    private Color selectionColor = Color.red;
    private Color originalColorSelection = Color.clear;


    private void Awake()
    {
        canvasActive = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public void changeSelectObjectColor(GameObject selectionObject)
    {
        TextMeshPro isText = selectionObject.GetComponent<TextMeshPro>();
        if (isText)
        {
            originalColorSelection = isText.color;
            isText.color = selectionColor;
        }
        else
        {
            Material mat = selectionObject.GetComponent<MeshRenderer>().material;
            originalColorSelection = mat.color;
            mat.color = selectionColor;
        }

    }

    public void changeDeselectObjectColor(GameObject deselectionObject)
    {
        if (deselectionObject)
        {
            TextMeshPro textMeshPro = deselectionObject.GetComponent<TextMeshPro>();
            if (textMeshPro)//caso for texto
            {
                if (textMeshPro.color == selectionColor)
                    textMeshPro.color = originalColorSelection;
            }
            else //caso nao for texto
            {
                Material mat = deselectionObject.GetComponent<MeshRenderer>().material;
                if (mat.color == selectionColor)
                    mat.color = originalColorSelection;
            }
        }
    }

   /* public List<VirtualObjectModel> getVirtualObjectFromParent(GameObject parent)
    {
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            gameObjects.Add(child.gameObject);
        }

        List<VirtualObjectModel> virtualObjects = new List<VirtualObjectModel>();
        foreach (GameObject gameObjet in gameObjects)
        {
            VirtualObjectModel virtualObjectModel = new VirtualObjectModel(gameObjet);
            virtualObjects.Add(virtualObjectModel);
        }

        return virtualObjects;
    }*/

    public void InstantiateVirtualObjectsOnParent(List<VirtualObjectModel> virtualObjects,GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (VirtualObjectModel VirtualObj in virtualObjects)
        {
            print($"VirtualObj: {VirtualObj.Name}");
            print($"Position: {VirtualObj.Position} JsonPosition: {VirtualObj.PositionJson}");
            print($"Rotation: {VirtualObj.Rotation} JsonRotation: {VirtualObj.RotationJson}");
            print($"Scale: {VirtualObj.Scale} JsonPosition: {VirtualObj.ScaleJson}");

            string path = "VirtualObjectPrefabs/" + VirtualObj.typeVirtualObject;
            GameObject prefabObj = Resources.Load(path) as GameObject;
            GameObject gameObj = Instantiate(prefabObj, parent.transform);
            VirtualObjectModel.ApplyPropertiesToGameObject(gameObj, VirtualObj);
            VirtualObj.GameObject = gameObj;
        }
    }

    //add
    public GameObject InstantiateVirtualObjectPrefabOnParent(VirtualObjectModel.TypeVirtualObject typeVirtual, GameObject parent)
    {
        string path = $"VirtualObjectPrefabs/{typeVirtual.ToString()}";
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            print($"Prefab não encontrado: {typeVirtual.ToString()}");
            return null;
        }

        GameObject objectInstance = Instantiate(prefab, parent.transform);
        string nameObj = objectInstance.name.Replace("(Clone)", "");
        objectInstance.name = nameObj;

        /*objectInstance.transform.localPosition = Vector3.zero;
        objectInstance.transform.localRotation = Quaternion.identity;
        objectInstance.transform.localScale = Vector3.one;*/

        return objectInstance;
    }

    //list
    public void listGameObjectsActiveObjectsReferenceInListMenu(GameObject activeGameObjectsParent, GameObject parent, Action<GameObject> editAction)
    {
        DestroyChildsIn(parent);

        string pathPrefab = "SideMenu/ListVirtualObject/ListVirtualObject";
        GameObject listVirtualObjectPrefab = Resources.Load<GameObject>(pathPrefab);

        /*foreach (Transform child in activeGameObjectsParent.transform)
        {
            GameObject listVirtualObject = Instantiate(listVirtualObjectPrefab, parent.transform);
            ListVirtualObject ListVirtualObject = listVirtualObject.GetComponent<ListVirtualObject>();
            ListVirtualObject.ListVirtualObjectConstructor(
                child.name,
                () => editAction(child.gameObject),
                () => ConfigureDeleteGameObject(child.gameObject, ListVirtualObject, parent));
        }*/
    }

    public void DestroyChildsIn(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if(child.name != "3DAxis") Destroy(child.gameObject);
        }
    }

    /*private void ConfigureDeleteGameObject(GameObject gameObject, GameObject listVirtualObject, GameObject parent)
    {
        GameObject modalChooseTutorialDelete = Instantiate(modalChoosePrefab, canvasActive.transform);
        ModalChoose manager = modalChooseTutorialDelete.GetComponent<ModalChoose>();
        manager.ModalChooseConstructor(ModalChoose.ConfigurationModal.YesOrNo, $"Do you want delete the object {gameObject.name}?",
            () => { }, 
            () => { 
                Destroy(gameObject); 
                Destroy(listVirtualObject);
            });
    }*/
    
    /*public void setTransformSelectecObjectBySlider(GameObject selectedObj, sideMenuScript.SliderTypeEnum sliderType, sideMenuScript.SliderRelativeAxis sliderAxis, float value)
    {
        Vector3 newValue = selectedObj.transform.localPosition;

        switch (sliderType)
        {
            case sideMenuScript.SliderTypeEnum.SliderPosition:
                newValue = selectedObj.transform.localPosition;
                break;
            case sideMenuScript.SliderTypeEnum.SliderRotation:
                newValue = selectedObj.transform.localEulerAngles;
                break;
            case sideMenuScript.SliderTypeEnum.SliderScale:
                newValue = selectedObj.transform.localScale;
                break;
        }

        switch (sliderAxis)
        {
            case sideMenuScript.SliderRelativeAxis.X:
                newValue.x = value;
                break;
            case sideMenuScript.SliderRelativeAxis.Y:
                newValue.y = value;
                break;
            case sideMenuScript.SliderRelativeAxis.Z:
                newValue.z = value;
                break;
        }

        switch (sliderType)
        {
            case sideMenuScript.SliderTypeEnum.SliderPosition:
                selectedObj.transform.localPosition = newValue;
                break;
            case sideMenuScript.SliderTypeEnum.SliderRotation:
                selectedObj.transform.localEulerAngles = newValue;
                break;
            case sideMenuScript.SliderTypeEnum.SliderScale:
                selectedObj.transform.localScale = newValue;
                break;
        }
    }

    private void showPanelError(string message, float time)
    {
        GameObject modalError = Instantiate(modalErrorPrefab, canvasActive.transform);
        ModalError manager = modalError.GetComponent<ModalError>();
        manager.showErroPanel(message, time);
    }*/

    public Color getOriginalColorSelectedObject()
    {
        return originalColorSelection;
    }

}
