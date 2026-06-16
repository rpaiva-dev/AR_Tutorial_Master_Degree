using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ModalFinishPreViewTutorial : MonoBehaviour
{
    private Button btn_Close;

    private Button btn_No;
    private Button btn_Yes;
    private Action finishTaskAction;

    void Awake()
    {
        btn_Close = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_Close");
        btn_Close.onClick.AddListener(() => Destroy(this.gameObject));

        btn_No = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_No");
        btn_No.onClick.AddListener(() => Destroy(this.gameObject));

        btn_Yes = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_Yes");
        btn_Yes.onClick.AddListener(() => { finishTaskAction.Invoke(); Destroy(this.gameObject); });
    }

    public void ModalFinishTaskConstructor(Action finishTaskAction)
    {
        this.finishTaskAction = finishTaskAction;
    }

}
