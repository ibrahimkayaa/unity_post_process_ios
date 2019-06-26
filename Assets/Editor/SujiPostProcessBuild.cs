using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;


public class SujiPostProcessBuild : ScriptableObject {

#if UNITY_IOS
    //public DefaultAsset entitlementFile;
    public static SujiPostProcessSetup setup;

    [PostProcessBuild(200)]
    public static void ChangeXCodePlist(BuildTarget buildTarget, string pathToBuildProject){

        if(buildTarget == BuildTarget.iOS){

            //Plist File Change
            setup = (SujiPostProcessSetup)AssetDatabase.LoadAssetAtPath("Assets/Editor/setup.asset", typeof(SujiPostProcessSetup));
            if(setup == null)
            {
                return;
            }
            string plistPath = pathToBuildProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            PlistElementDict root = plist.root;


            foreach(var cap in setup.capabilities)
            {
                switch (cap)
                {
                    case SujiPostProcessSetup.CapabilityTypes.GameCenter:
                        PlistElementArray blabla = root.values["UIRequiredDeviceCapabilities"].AsArray();
                        blabla.AddString("gamekit");
                        break;
                    case SujiPostProcessSetup.CapabilityTypes.InAppPurchase:
                        break;
                    case SujiPostProcessSetup.CapabilityTypes.PushNotifications:
                        break;
                    default:
                        break;
                }
            }



            foreach (AppleInfoPlistDictionary dict in setup.plistDictonary)
            {
                root.SetString(dict.key, dict.value);
            }

            foreach(AppleInfoPlistBoolDictionary dictBool in setup.plistBoolDiconary)
            {
                root.SetBoolean(dictBool.key, dictBool.value);
            }

            File.WriteAllText(plistPath, plist.WriteToString());



            PrepareProject(pathToBuildProject);

        }
    }

    public static void PrepareProject(string buildPath){

        //Create entitlement for post process
        //var entitlement = ScriptableObject.CreateInstance<SujiPostProcessBuild>();
        //var file = entitlement.entitlementFile;
        var file = setup.entitlementFile;


        //Destroy temp scriptable object
        //DestroyImmediate(entitlement);

        if (file == null)
        {
            return;
        }

        var target_name = PBXProject.GetUnityTargetName();
        var projPath = Path.Combine(buildPath,"Unity-iPhone.xcodeproj/project.pbxproj" );
        var project = new PBXProject();
        project.ReadFromString(File.ReadAllText(projPath));

        var src = AssetDatabase.GetAssetPath(file);
        var file_name = Path.GetFileName(src);
        var dst = buildPath + "/" + target_name + "/" + file_name;
        FileUtil.CopyFileOrDirectory(src, dst);

        var target = project.TargetGuidByName("Unity-iPhone");
        project.AddFile(target_name + "/" + file_name, file_name);
        //project.
        project.AddBuildProperty(target,"ENABLE_BITCODE", setup.GetBitCodeEnable());
        project.AddBuildProperty(target, "CODE_SIGN_ENTITLEMENTS", target_name +"/" +file_name);
        project.AddBuildProperty(target, "DEVELOPMENT_TEAM", setup.developmentTeam);


        foreach(SujiPostProcessSetup.CapabilityTypes cap in setup.capabilities)
        {

            switch (cap)
            {
                case SujiPostProcessSetup.CapabilityTypes.GameCenter:
                    project.AddCapability(target, PBXCapabilityType.GameCenter);
                    project.AddFrameworkToProject(target, "GameKit.framework", true);

                    break;
                case SujiPostProcessSetup.CapabilityTypes.InAppPurchase:
                    project.AddCapability(target, PBXCapabilityType.InAppPurchase, target_name + "/" + file_name);
                    project.AddFrameworkToProject(target, "StoreKit.framework", true);
                    break;
                case SujiPostProcessSetup.CapabilityTypes.PushNotifications:
                    project.AddCapability(target, PBXCapabilityType.PushNotifications);
                    break;
                default:
                    break;
            }
           
        }
        project.SetTeamId(target, setup.developmentID);
        File.WriteAllText(projPath, project.WriteToString());


    }


#endif
}
