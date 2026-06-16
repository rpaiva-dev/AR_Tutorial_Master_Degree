using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

[Serializable]
public class TaskModel
{
    public string Name;

    public List<VirtualObjectModel> VirtualObjects;

    public TaskModel()
    {
        VirtualObjects = new List<VirtualObjectModel>();
    }

    public TaskModel(string name)
    {
        Name = name;
        VirtualObjects = new List<VirtualObjectModel>();
    }

    public TaskModel Clone()
    {
        return new TaskModel
        {
            Name = this.Name,
            VirtualObjects = this.VirtualObjects.Select(vo => vo.Clone()).ToList()
        };
    }

    public override bool Equals(object obj)
    {
        if (obj is not TaskModel other)
            return false;

        return Name == other.Name &&
               VirtualObjects.SequenceEqual(other.VirtualObjects);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, VirtualObjects);
    }
}



//public class TaskModel
//{
//    public string Name { get; set; }

//    [BsonElement("VirtualObjects")]
//    public List<VirtualObjectModel> VirtualObjects { get; set; }

//    public TaskModel()
//    {
//        VirtualObjects = new List<VirtualObjectModel>();
//    }

//    public TaskModel(string name)
//    {
//        Name = name;
//        VirtualObjects = new List<VirtualObjectModel>();
//    }

//    public TaskModel Clone()
//    {
//        return new TaskModel
//        {
//            Name = this.Name,
//            VirtualObjects = this.VirtualObjects.Select(vo => vo.Clone()).ToList()
//        };
//    }

//    public override bool Equals(object obj)
//    {
//        if (obj is not TaskModel other)
//            return false;

//        return Name == other.Name &&
//               VirtualObjects.SequenceEqual(other.VirtualObjects); // Compara objetos aninhados
//    }

//    public override int GetHashCode()
//    {
//        return HashCode.Combine(Name, VirtualObjects);
//    }
//}
