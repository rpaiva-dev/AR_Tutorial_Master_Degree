using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static MongoDB.Driver.WriteConcern;

public class EditTextComponentController : MonoBehaviour
{
    private Action<string, int> returnTextAction;
    private string message;
    private int fontSize;

    private TMP_InputField Input_Text;

    private TextMeshProUGUI txt_Font_Size;
    private Slider Slider;


    private void construtor()
    {
        txt_Font_Size = this.GetComponentsInChildren<Transform>().FirstOrDefault(n => n.name == "txt_Font_Size")
            .GetComponent<TextMeshProUGUI>();

        Input_Text = this.GetComponentsInChildren<Transform>().FirstOrDefault(n => n.name== "Input_Text")
            .GetComponent<TMP_InputField>();
        
        Slider = this.GetComponentsInChildren<Slider>().FirstOrDefault(n => n.name == "Slider");
    }


    public void EditTextComponentControllerConstructor(string message, int fontSize , Action<string, int> returnTextAction) {
        construtor();

        this.message = String.IsNullOrEmpty(message) ? "Digite o texto..." : message;
        this.fontSize = fontSize;
        txt_Font_Size.text = $"Tamanho da fonte: {fontSize}";

        this.Slider.value = fontSize;
        this.Input_Text.text = message;

        Input_Text.Select();
        Input_Text.ActivateInputField();

        this.returnTextAction = returnTextAction;

        Slider.onValueChanged.AddListener((newValue) =>
        {
            txt_Font_Size.text = $"Tamanho da fonte: {newValue}";
            this.fontSize = Int32.Parse(newValue.ToString());
            returnTextAction(this.message, this.fontSize);
        });
        Input_Text.onValueChanged.AddListener((newValue) =>
        {
            this.message = newValue;
            returnTextAction(this.message, this.fontSize);
        });
    }

}
