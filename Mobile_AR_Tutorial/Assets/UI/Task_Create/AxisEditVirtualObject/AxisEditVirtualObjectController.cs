using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AxisEditVirtualObjectController : MonoBehaviour
{
    private Action<Vector3> returnVectorAction;

    private GameObject X, Y, Z;
    private TextMeshProUGUI X_value, Y_value, Z_value;
    private Slider X_Slider, Y_Slider, Z_Slider;

    private Vector3 vector3Values;

    private int minValue;
    private int maxValue;

    // Método genérico para configurar sliders e valores
    private void ConfigureAxis(GameObject axis, ref TextMeshProUGUI valueText, ref Slider slider, string axisName, Action<float> onValueChanged)
    {
        axis = gameObject.GetComponentsInChildren<Transform>().FirstOrDefault(x => x.name == axisName)?.gameObject;
        if (axis == null) throw new Exception($"{axisName} axis GameObject not found!");

        valueText = axis.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(x => x.name == "txtValue");
        slider = axis.GetComponentsInChildren<Slider>().FirstOrDefault(x => x.name == "Slider");

        if (valueText == null || slider == null) throw new Exception($"{axisName} axis components not found!");

        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((value) => onValueChanged(value));
    }

    // Método genérico para configurar X, Y e Z
    private void ConfigureAxes()
    {
        ConfigureAxis(X, ref X_value, ref X_Slider, "X", newValue =>
        {
            X_value.text = $"X = {newValue:F2}";
            vector3Values.x = newValue;
            returnVectorAction?.Invoke(vector3Values);
        });

        ConfigureAxis(Y, ref Y_value, ref Y_Slider, "Y", newValue =>
        {
            Y_value.text = $"Y = {newValue:F2}";
            vector3Values.y = newValue;
            returnVectorAction?.Invoke(vector3Values);
        });

        ConfigureAxis(Z, ref Z_value, ref Z_Slider, "Z", newValue =>
        {
            Z_value.text = $"Z = {newValue:F2}";
            vector3Values.z = newValue;
            returnVectorAction?.Invoke(vector3Values);
        });
    }

    // Método genérico para inicializar construtores de Position, Rotation e Scale
    private void Initialize(Vector3 currentValue, int min, int max, Action<Vector3> returnAction)
    {
        minValue = min;
        maxValue = max;

        ConfigureAxes();

        // Inicializa os valores
        X_value.text = $"X = {currentValue.x:F2}";
        X_Slider.value = currentValue.x;

        Y_value.text = $"Y = {currentValue.y:F2}";
        Y_Slider.value = currentValue.y;

        Z_value.text = $"Z = {currentValue.z:F2}";
        Z_Slider.value = currentValue.z;

        vector3Values = currentValue;
        returnVectorAction = returnAction;
    }

    // Construtor para Position
    public void AxisEditVirtualObjectControllerConstructorPosition(Vector3 currentValue, Action<Vector3> returnVectorAction)
    {
        this.returnVectorAction = null;
        Initialize(currentValue, -5, 5, returnVectorAction);
    }

    // Construtor para Rotation
    public void AxisEditVirtualObjectControllerConstructorRotation(Vector3 currentValue, Action<Vector3> returnVectorAction)
    {
        this.returnVectorAction = null;
        Initialize(currentValue, 0, 360, returnVectorAction);
    }

    // Construtor para Scale
    public void AxisEditVirtualObjectControllerConstructorScale(Vector3 currentValue, Action<Vector3> returnVectorAction)
    {
        this.returnVectorAction = null;
        Initialize(currentValue, -5, 5, returnVectorAction);
    }
}
