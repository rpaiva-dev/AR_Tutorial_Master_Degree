using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ModalAddObjectController : MonoBehaviour
{

    private Button btn_Close;

    private Button btn_arrow;
    private Button btn_rotation;
    private Button btn_check;
    private Button btn_wrong;
    private Button btn_attention;

    private Button btn_prohibited;
    private Button btn_text3d;
    private Button btn_screwdriver;
    private Button btn_hammer;
    private Button btn_drill;


    private Action<VirtualObjectModel.TypeVirtualObject> actionInstanciateObject;

    private void Awake()
    {
        Transform modal = this.gameObject.GetComponentsInChildren<Transform>()
                       .FirstOrDefault(txt => txt.name == "modal");

        btn_Close = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_Close");
        btn_Close.onClick.AddListener(() => Destroy(this.gameObject));

        Transform modalContent = this.gameObject.GetComponentsInChildren<Transform>()
                       .FirstOrDefault(txt => txt.name == "modalContent");

        btn_arrow = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_arrow");
        btn_arrow.onClick.AddListener(() => actionInstanciateObject.Invoke(VirtualObjectModel.TypeVirtualObject.Seta));

        btn_rotation = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_rotation");
        btn_rotation.onClick.AddListener(() => actionInstanciateObject.Invoke(VirtualObjectModel.TypeVirtualObject.Rotacao));

        btn_check = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_check");
        btn_check.onClick.AddListener(() => actionInstanciateObject.Invoke(VirtualObjectModel.TypeVirtualObject.Certo));

        btn_wrong = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_wrong");
        btn_wrong.onClick.AddListener(() => actionInstanciateObject.Invoke(VirtualObjectModel.TypeVirtualObject.Errado));

        btn_attention = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_attention");
        btn_attention.onClick.AddListener(() => actionInstanciateObject.Invoke(VirtualObjectModel.TypeVirtualObject.Atencao));

        btn_prohibited = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_prohibited");
        btn_prohibited.onClick.AddListener(() => actionInstanciateObject.Invoke(VirtualObjectModel.TypeVirtualObject.Proibido));

        btn_text3d = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_text3d");
        btn_text3d.onClick.AddListener(() => actionInstanciateObject.Invoke(VirtualObjectModel.TypeVirtualObject.Texto3D));

        btn_screwdriver = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_screwdriver");
        btn_screwdriver.onClick.AddListener(() => actionInstanciateObject.Invoke(VirtualObjectModel.TypeVirtualObject.Chave));

        btn_hammer = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_hammer");
        btn_hammer.onClick.AddListener(() => actionInstanciateObject.Invoke(VirtualObjectModel.TypeVirtualObject.Martelo));

        btn_drill = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_drill");
        btn_drill.onClick.AddListener(() => actionInstanciateObject.Invoke(VirtualObjectModel.TypeVirtualObject.Furadeira));
    }

    public void ModalAddObjectControllerConstructor(Action<VirtualObjectModel.TypeVirtualObject> actionInstanciateObject)
    {
        this.actionInstanciateObject = actionInstanciateObject;
    }
}
