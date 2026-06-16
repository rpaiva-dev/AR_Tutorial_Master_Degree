using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[Serializable]
public class TutorialModel
{
    [BsonIgnore]
    public static TutorialModel CurrentTutorial;


    [BsonIgnore]
    public static List<TaskModel> listTasksEddited = new List<TaskModel>();
    [BsonIgnore]
    public static int indexTaskEdited;

    public enum UserState
    {
        FirstTime,
        Returning
    }

    [BsonIgnore]
    public static UserState CurrentUserState { get; set; } = UserState.FirstTime;


    //[BsonId]
    ////public ObjectId Id { get; set; }
    //public String Id { get; set; }

    //public string Name { get; set; }


    //[BsonElement("Tasks")]
    //public List<TaskModel> Tasks { get; set; }

    public string Id;
    public string Name;
    public List<TaskModel> Tasks;

    public TutorialModel(string name)
    {
        Name = name;
        Tasks = new List<TaskModel>();
        Id = ObjectId.GenerateNewId().ToString();
    }

    public TutorialModel()
    {
        Tasks = new List<TaskModel>();
        Id = ObjectId.GenerateNewId().ToString();
    }

}
