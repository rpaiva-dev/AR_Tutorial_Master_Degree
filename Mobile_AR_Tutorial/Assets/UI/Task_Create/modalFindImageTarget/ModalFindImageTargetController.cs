using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ModalFindImageTargetController : MonoBehaviour
{

    private Button btn_Close;

    private Button btn_Yes;
    private Action removeTaskAction;

    void Awake()
    {
        btn_Close = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_Close");
        btn_Close.onClick.AddListener(() => {
            removeTaskAction.Invoke();
            Destroy(this.gameObject);
        });

        btn_Yes = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_Yes");
        btn_Yes.onClick.AddListener(() => {
            removeTaskAction.Invoke();
            Destroy(this.gameObject); 
        });
    }

    public void ModalFindImageTargetConstructor(Action removeTaskAction)
    {
        this.removeTaskAction = removeTaskAction;
    }

}
