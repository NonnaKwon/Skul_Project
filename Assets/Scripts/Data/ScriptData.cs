using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class ScriptData
{
    public string ScriptID;
    public string Talker;
    public string Script;
}


[Serializable]
public class ScriptDataList
{
    public static List<ScriptData> SList = MakeList();


    public static ScriptData GetData(string id)
    {
        for(int i =0;i<SList.Count;i++)
        {
            if (SList[i].ScriptID.Equals(id))
                return SList[i];
        }
        return null;
    }

    private static List<ScriptData> MakeList()
    {
        List<ScriptData> list = new List<ScriptData>();
        List<Dictionary<string, object>> tmp = CSVReader.Read("Data/ScriptData");
        foreach (Dictionary<string, object> dic in tmp)
        {
            ScriptData data = new ScriptData();
            data.ScriptID = dic["ScriptID"].ToString();
            data.Talker = dic["Talker"].ToString();
            data.Script = dic["Script"].ToString();
            list.Add(data);
        }
        return list;
    }
}
