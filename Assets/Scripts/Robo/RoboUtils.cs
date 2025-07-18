using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomUtils
{
    public static string GetPartId(string partType, UserDataManager.RoboCustomData data)
    {
        return partType switch
        {
            "Head" => data.headId,
            "Body" => data.bodyId,
            "Arms" => data.armsId,
            "Legs" => data.legsId,
            "Tail" => data.tailId,
            _ => null
        };
    }
    
    public static void SetPartId(string partType, UserDataManager.RoboCustomData data, string roboId)
    {
        switch (partType)
        {
            case "Head": data.headId = roboId; break;
            case "Body": data.bodyId = roboId; break;
            case "Arms": data.armsId = roboId; break;
            case "Legs": data.legsId = roboId; break;
            case "Tail": data.tailId = roboId; break;
        }
    }


}
