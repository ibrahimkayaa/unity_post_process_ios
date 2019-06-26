using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using UnityEngine;


[CreateAssetMenu(fileName ="setup", menuName ="Post Process/Setup")]
public class SujiPostProcessSetup : ScriptableObject {

    public enum CapabilityTypes
    {
        GameCenter,
        InAppPurchase,
        PushNotifications
    }

    public UnityEditor.DefaultAsset entitlementFile;
    public bool bitCodeEnable;
    public string developmentTeam;
    public string developmentID;
    public List<AppleInfoPlistDictionary> plistDictonary = new List<AppleInfoPlistDictionary>();
    public List<AppleInfoPlistBoolDictionary> plistBoolDiconary = new List<AppleInfoPlistBoolDictionary>();
    public List<CapabilityTypes> capabilities = new List<CapabilityTypes>();

    public string GetBitCodeEnable()
    {
        return (bitCodeEnable) ? "YES" : "NO";
    }
    public PBXCapabilityType GetCapabilityType(SujiPostProcessSetup.CapabilityTypes cap)
    {
        PBXCapabilityType returnType = PBXCapabilityType.PushNotifications;
        switch (cap)
        {
            case CapabilityTypes.GameCenter: returnType = PBXCapabilityType.GameCenter; break;
            case CapabilityTypes.InAppPurchase: returnType = PBXCapabilityType.InAppPurchase; break;
            case CapabilityTypes.PushNotifications: returnType = PBXCapabilityType.PushNotifications; break;
            default:
                break;
        }

        return returnType;
    }
}

[System.Serializable]
public class AppleInfoPlistDictionary
{
    public string key;
    public string value;
}
[System.Serializable]
public class AppleInfoPlistBoolDictionary
{
    public string key;
    public bool value;
}
