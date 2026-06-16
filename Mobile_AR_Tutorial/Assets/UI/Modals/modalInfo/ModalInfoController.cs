using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ModalInfoController : MonoBehaviour
{

    private Button btn_Close;

    private TextMeshProUGUI txt_MainText;
    private TextMeshProUGUI txt_ExplanationText;

    private void Awake()
    {
        Transform modal = this.gameObject.GetComponentsInChildren<Transform>()
                       .FirstOrDefault(txt => txt.name == "modal");

        btn_Close = this.gameObject.GetComponentsInChildren<Button>()
                        .FirstOrDefault(txt => txt.name == "btn_Close");
        btn_Close.onClick.AddListener(() => Destroy(this.gameObject));

        Transform modalContent = this.gameObject.GetComponentsInChildren<Transform>()
                       .FirstOrDefault(txt => txt.name == "modalContent");

        txt_MainText = modalContent.GetComponentsInChildren<TextMeshProUGUI>()
                        .FirstOrDefault(txt => txt.name == "txt_MainText");
        txt_ExplanationText = modalContent.GetComponentsInChildren<TextMeshProUGUI>()
                        .FirstOrDefault(txt => txt.name == "txt_ExplanationText");
    }

    public void showMessage(string header, string message, float time)
    {
        StartCoroutine(showMessageCoroutine(header, message, time));
    }

    private IEnumerator showMessageCoroutine(string header, string message, float time)
    {
        txt_MainText.text = header;
        txt_ExplanationText.text = message;
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
