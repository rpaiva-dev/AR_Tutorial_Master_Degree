using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class UserModel
{
    [JsonIgnore]
    public static UserModel currentUser;

    [JsonIgnore]
    public string Id;

    public string Name;
    public string Password;

    public List<string> idEditTutorials;

    public UserModel()
    {
    }

    public UserModel(string name, string password)
    {
        Name = name;
        Password = password;
        idEditTutorials = new List<string>(); // Inicializando a lista

    }
}