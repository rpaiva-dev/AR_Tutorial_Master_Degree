using System;

public class TestBySizeModel
{
    public int teste { get; set; }
    public string ConnectionType { get; set; }
    public string ObjectName { get; set; }
    public float DownloadTime { get; set; }

    public TestBySizeModel(int teste, string connectionType, string objectName, float downloadTime)
    {
        this.teste = teste;
        ConnectionType = connectionType;
        ObjectName = objectName;
        DownloadTime = downloadTime;
    }
}

public class TestByQuantityModel
{
    public int teste { get; set; }
    public string ConnectionType { get; set; }
    public int ObjectCount { get; set; }
    public float DownloadTime { get; set; }

    public TestByQuantityModel(int teste, string connectionType, int objectCount, float downloadTime)
    {
        this.teste = teste;
        ConnectionType = connectionType;
        ObjectCount = objectCount;
        DownloadTime = downloadTime;
    }
}