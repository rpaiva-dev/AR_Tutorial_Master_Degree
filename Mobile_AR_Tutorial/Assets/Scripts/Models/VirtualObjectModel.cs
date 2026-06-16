using MongoDB.Bson.Serialization.Attributes;
using System;
using TMPro;
using UnityEngine;

[Serializable]
public class VirtualObjectModel
{
    public string Name; //  { get; set; }
    public string Text; //  { get; set; }
    public int FontSize; //  { get; set; }
    public TypeVirtualObject typeVirtualObject; //  { get; set; }

    //[BsonIgnore]
    public Vector3 Position; //  { get; set; }

    //[BsonElement("PositionJson")]
    public string PositionJson
    {
        get => JsonUtility.ToJson(Position);
        set => Position = JsonUtility.FromJson<Vector3>(value);
    }

    //[BsonIgnore]
    public Vector3 Rotation; //  { get; set; }

    //[BsonElement("RotationJson")]
    public string RotationJson
    {
        get => JsonUtility.ToJson(Rotation);
        set => Rotation = JsonUtility.FromJson<Vector3>(value);
    }

    //[BsonIgnore]
    public Vector3 Scale; //  { get; set; }

    //[BsonElement("ScaleJson")]
    public string ScaleJson
    {
        get => JsonUtility.ToJson(Scale);
        set => Scale = JsonUtility.FromJson<Vector3>(value);
    }

    public enum TypeVirtualObject
    {
        Seta, Rotacao, Chave, Texto3D, Certo, Errado, Proibido, Atencao, Martelo, Furadeira
    }

    //[BsonIgnore]
    public Color Color; //  { get; set; }

    //[BsonElement("ColorJson")]
    public string ColorJson
    {
        get => JsonUtility.ToJson(Color);
        set => Color = JsonUtility.FromJson<Color>(value);
    }

    //[BsonIgnore]
    public GameObject GameObject; //  { get; set; }

    public VirtualObjectModel() { }

    public VirtualObjectModel(GameObject gameObj, TypeVirtualObject type)
    {
        Name = gameObj.name;
        typeVirtualObject = type;
        Position = gameObj.transform.localPosition;
        Rotation = gameObj.transform.localEulerAngles;
        Scale = gameObj.transform.localScale;

        GameObject = gameObj;

        if (type != TypeVirtualObject.Texto3D)
        {
            MeshRenderer renderer = gameObj.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                Color = renderer.material.color;
            }
            else
            {
                Color = Color.clear;
            }
        }
        else
        {
            TextMeshPro tmp = gameObj.GetComponentInChildren<TextMeshPro>();
            Color = tmp.color;
            FontSize = Int32.Parse(tmp.fontSize.ToString());
        }
    }

    public VirtualObjectModel Clone()
    {
        return new VirtualObjectModel
        {
            Name = this.Name,
            typeVirtualObject = this.typeVirtualObject,
            Position = this.Position,
            Rotation = this.Rotation,
            Scale = this.Scale,
            Color = this.Color
        };
    }

    public static GameObject ApplyPropertiesToGameObject(GameObject prefabObject, VirtualObjectModel virtualObject)
    {
        prefabObject.name = virtualObject.Name;
        prefabObject.transform.localPosition = virtualObject.Position;
        prefabObject.transform.localEulerAngles = virtualObject.Rotation;

        if (virtualObject.typeVirtualObject != VirtualObjectModel.TypeVirtualObject.Texto3D)
        {
            var renderer = prefabObject.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = virtualObject.Color;
            }
            prefabObject.transform.localScale = virtualObject.Scale;
        }
        else
        {
            //TextMeshPro tmp = prefabObject.GetComponentInChildren<TextMeshPro>();
            prefabObject.GetComponentInChildren<TextMeshPro>().text = virtualObject.Text;
            prefabObject.GetComponentInChildren<TextMeshPro>().color = virtualObject.Color;
            prefabObject.GetComponentInChildren<TextMeshPro>().fontSize = virtualObject.FontSize;
        }

        return prefabObject;
    }

    public override bool Equals(object obj)
    {
        if (obj is not VirtualObjectModel other)
            return false;

        return Name == other.Name &&
               Position.Equals(other.Position) &&
               Rotation.Equals(other.Rotation) &&
               Scale.Equals(other.Scale) &&
               Text == other.Text &&
               FontSize == other.FontSize &&
               Color.Equals(other.Color);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Position, Rotation, Scale, Text, FontSize, Color);
    }
}


