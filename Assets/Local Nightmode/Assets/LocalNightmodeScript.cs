#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
#if VRC_SDK_VRCSDK3
    using VRC.SDK3.Avatars.Components;
    using VRC.SDK3.Editor;
    using VRC.SDKBase.Editor;
    using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
    using ExpressionParameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter;
    using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
    using ExpressionControl = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control;
#endif

public class LocalNightmodeScript : MonoBehaviour
{
    const string additionalName = "_NightMode";
    const string menuName = "NightMode";
    const string floatParamName = "NightModeF";
    const string namePrefab3 = "3.0_LocalNightmode";
    const string namePrefab2 = "2.0_LocalNightmode";
    const string pathPrefab3 = "Assets/Local Nightmode/Assets/3.0_LocalNightmode.prefab";
    const string pathPrefab2 = "Assets/Local Nightmode/Assets/2.0_LocalNightmode.prefab";
    const string pathDefaultExpressionParameters = "Assets/VRCSDK/Examples3/Expressions Menu/DefaultExpressionParameters.asset";
    const string pathDefaultExpressionsMenu = "Assets/VRCSDK/Examples3/Expressions Menu/DefaultExpressionsMenu.asset";
    const string pathDefaultAnimationController = "Assets/VRCSDK/Examples3/Animation/Controllers/vrc_AvatarV3FaceLayer.controller";
    const string pathAnim0 = "Assets/Local Nightmode/Assets/3.0_LocalNightmodeAnim_0.anim";
    const string pathAnim1 = "Assets/Local Nightmode/Assets/3.0_LocalNightmodeAnim_1.anim";
    const string pathParamNightmode = "Assets/expressionParam_Nightmode.asset";
    const string pathMenuNightmode = "Assets/expressionMenu_Nightmode.asset";
    const string pathControllerNightmode = "Assets/expressionAnimator_Nightmode.asset";
    const int maxExPropMemory = 128;

    [MenuItem("GameObject/Setup Local Nightmode", false, 20)]
    static void AddLocalNightmode()
    {
        GameObject gameObject = Selection.activeGameObject;
        if(gameObject == null) return;

        string sOK;
        string sNotAvatarTitle;
        string sNotAvatarMessage;
        string sTooManyParamTitle;
        string sTooManyParamMessage;
        string sTooManyMenuItemTitle;
        string sTooManyMenuItemMessage;
        string sCompleteTitle;
        string sCompleteMessage;

        if(Application.systemLanguage == SystemLanguage.Japanese)
        {
            sOK = "OK";
            sNotAvatarTitle = "このGameObjectはアバターではありません";
            sNotAvatarMessage = "Local Nightmodeはアバターにのみインストールできます";
            sTooManyParamTitle = "パラメーターが多すぎます";
            sTooManyParamMessage = "未使用のパラメーターをExpressionParametersから削除してください";
            sTooManyMenuItemTitle = "メニューの項目が多すぎます";
            sTooManyMenuItemMessage = "未使用のアイテムをExpressionsMenuから削除してください";
            sCompleteTitle = "Local Nightmode";
            sCompleteMessage = "インストールが完了しました";
        }
        else
        {
            sOK = "OK";
            sNotAvatarTitle = "This GameObject is not avatar";
            sNotAvatarMessage = "Local Nightmode can only be installed on avatars";
            sTooManyParamTitle = "Too many parameters";
            sTooManyParamMessage = "Please delete unused parameter from ExpressionParameters";
            sTooManyMenuItemTitle = "There are too many menu items";
            sTooManyMenuItemMessage = "Please remove any unused menu items from ExpressionsMenu";
            sCompleteTitle = "Local Nightmode";
            sCompleteMessage = "Installation is complete";
        }

        #if VRC_SDK_VRCSDK3
            VRCAvatarDescriptor avatarDescriptor = gameObject.GetComponent(typeof(VRCAvatarDescriptor)) as VRCAvatarDescriptor;
            if(avatarDescriptor == null)
            {
                EditorUtility.DisplayDialog(sNotAvatarTitle,sNotAvatarMessage,sOK);
                return;
            }

            GameObject copiedObject = (GameObject)Instantiate(gameObject);
            copiedObject.name = gameObject.name + additionalName;
            gameObject = copiedObject;
            avatarDescriptor = copiedObject.GetComponent(typeof(VRCAvatarDescriptor)) as VRCAvatarDescriptor;
            if(avatarDescriptor.expressionParameters != null)
            {
                string paramPath = AssetDatabase.GetAssetPath(avatarDescriptor.expressionParameters);
                ExpressionParameters copiedParam = (ExpressionParameters)Instantiate(avatarDescriptor.expressionParameters);
                string paramSavePath = Path.GetDirectoryName(paramPath) + "/" + Path.GetFileNameWithoutExtension(paramPath) + additionalName + ".asset";
                EditorUtility.SetDirty(copiedParam);
                AssetDatabase.CreateAsset(copiedParam, paramSavePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(paramSavePath, ImportAssetOptions.ForceUpdate);
                avatarDescriptor.expressionParameters = (ExpressionParameters)AssetDatabase.LoadAssetAtPath(paramSavePath, typeof(ExpressionParameters));
            }
            if(avatarDescriptor.expressionsMenu != null)
            {
                string menuPath = AssetDatabase.GetAssetPath(avatarDescriptor.expressionsMenu);
                string menuSavePath = Path.GetDirectoryName(menuPath) + "/" + Path.GetFileNameWithoutExtension(menuPath) + additionalName + ".asset";
                ExpressionsMenu copiedMenu = (ExpressionsMenu)Instantiate(avatarDescriptor.expressionsMenu);
                EditorUtility.SetDirty(copiedMenu);
                AssetDatabase.CreateAsset(copiedMenu, menuSavePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(menuSavePath, ImportAssetOptions.ForceUpdate);
                avatarDescriptor.expressionsMenu = (ExpressionsMenu)AssetDatabase.LoadAssetAtPath(menuSavePath, typeof(ExpressionsMenu));
            }
            for(int i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++)
            {
                if (avatarDescriptor.baseAnimationLayers[i].type == VRCAvatarDescriptor.AnimLayerType.FX)
                {
                    if(avatarDescriptor.baseAnimationLayers[i].animatorController != null)
                    {
                        string controllerPath = AssetDatabase.GetAssetPath(avatarDescriptor.baseAnimationLayers[i].animatorController);
                        string controllerSavePath = Path.GetDirectoryName(controllerPath) + "/" + Path.GetFileNameWithoutExtension(controllerPath) + additionalName + ".controller";
                        AnimatorController copiedController = (AnimatorController)Instantiate(avatarDescriptor.baseAnimationLayers[i].animatorController);
                        EditorUtility.SetDirty(copiedController);
                        AssetDatabase.CreateAsset(copiedController, controllerSavePath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(controllerSavePath, ImportAssetOptions.ForceUpdate);
                        avatarDescriptor.baseAnimationLayers[i].animatorController = (AnimatorController)AssetDatabase.LoadAssetAtPath(controllerSavePath, typeof(AnimatorController));
                    }
                }
            }

            // Parameter
            avatarDescriptor.customExpressions = true;
            if(avatarDescriptor.expressionParameters == null)
            {
                ExpressionParameters defaultParam = (ExpressionParameters)AssetDatabase.LoadAssetAtPath(pathDefaultExpressionParameters, typeof(ExpressionParameters));
                ExpressionParameters newParam = (ExpressionParameters)Instantiate(defaultParam);
                EditorUtility.SetDirty(newParam);
                AssetDatabase.CreateAsset(newParam, pathParamNightmode);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(pathParamNightmode, ImportAssetOptions.ForceUpdate);
                avatarDescriptor.expressionParameters = (ExpressionParameters)AssetDatabase.LoadAssetAtPath(pathParamNightmode, typeof(ExpressionParameters));
            }
            ExpressionParameters origParam = avatarDescriptor.expressionParameters;
            int memoryCount = 0;
            for(int i = 0; i < origParam.parameters.Length; i++)
            {
                if(origParam.parameters[i].valueType == ExpressionParameters.ValueType.Bool)
                {
                    memoryCount += 1;
                }
                else
                {
                    memoryCount += 8;
                }

                if(origParam.parameters[i].name == floatParamName || origParam.parameters[i].name == string.Empty)
                {
                    origParam.parameters[i].name = floatParamName;
                    origParam.parameters[i].valueType = ExpressionParameters.ValueType.Float;
                    origParam.parameters[i].saved = true;
                    origParam.parameters[i].defaultValue = 0.0f;
                    i = 256;
                }
                else if(i == origParam.parameters.Length - 1)
                {
                    if((memoryCount + 8) > maxExPropMemory)
                    {
                        EditorUtility.DisplayDialog(sTooManyParamTitle,sTooManyParamMessage,sOK);
                        return;
                    }
                    else
                    {
                        Array.Resize(ref origParam.parameters, i+2);
                        ExpressionParameters.Parameter nightmodeParam = new ExpressionParameters.Parameter();
                        nightmodeParam.name = floatParamName;
                        nightmodeParam.valueType = ExpressionParameters.ValueType.Float;
                        nightmodeParam.saved = true;
                        nightmodeParam.defaultValue = 0.0f;
                        origParam.parameters[i+1] = nightmodeParam;
                        i = 256;
                    }
                }
            }
            EditorUtility.SetDirty(origParam);

            // Menu
            if(avatarDescriptor.expressionsMenu == null)
            {
                ExpressionsMenu defaultMenu = (ExpressionsMenu)AssetDatabase.LoadAssetAtPath(pathDefaultExpressionsMenu, typeof(ExpressionsMenu));
                ExpressionsMenu newMenu = (ExpressionsMenu)Instantiate(defaultMenu);
                EditorUtility.SetDirty(newMenu);
                AssetDatabase.CreateAsset(newMenu, pathMenuNightmode);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(pathMenuNightmode, ImportAssetOptions.ForceUpdate);
                avatarDescriptor.expressionsMenu = (ExpressionsMenu)AssetDatabase.LoadAssetAtPath(pathMenuNightmode, typeof(ExpressionsMenu));
            }
            ExpressionsMenu origMenu = avatarDescriptor.expressionsMenu;
            for(int i = 0; i < origMenu.controls.Count; i++)
            {
                if(origMenu.controls[i].name == menuName)
                {
                    origMenu.controls.RemoveAt(i);
                }
            }
            if(origMenu.controls.Count == 8)
            {
                EditorUtility.DisplayDialog(sTooManyMenuItemTitle,sTooManyMenuItemMessage,sOK);
                return;
            }
            ExpressionControl control = new ExpressionControl();
            control.name = menuName;
            control.type = ExpressionControl.ControlType.RadialPuppet;
            Array.Resize(ref control.subParameters, 1);
            control.subParameters[0] = new ExpressionsMenu.Control.Parameter();
            control.subParameters[0].name = floatParamName;
            origMenu.controls.Add(control);
            EditorUtility.SetDirty(origMenu);

            // Animator
            for(int i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++)
            {
                if (avatarDescriptor.baseAnimationLayers[i].type == VRCAvatarDescriptor.AnimLayerType.FX)
                {
                    avatarDescriptor.baseAnimationLayers[i].isDefault = false;
                    if(avatarDescriptor.baseAnimationLayers[i].animatorController == null)
                    {
                        AnimatorController defaultCont = (AnimatorController)AssetDatabase.LoadAssetAtPath(pathDefaultAnimationController, typeof(AnimatorController));
                        AnimatorController newCont = (AnimatorController)Instantiate(defaultCont);
                        EditorUtility.SetDirty(newCont);
                        AssetDatabase.CreateAsset(newCont, pathControllerNightmode);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(pathControllerNightmode, ImportAssetOptions.ForceUpdate);
                        avatarDescriptor.baseAnimationLayers[i].animatorController = (AnimatorController)AssetDatabase.LoadAssetAtPath(pathControllerNightmode, typeof(AnimatorController));
                    }
                    AnimatorController controller = (AnimatorController)avatarDescriptor.baseAnimationLayers[i].animatorController;
                    for(int j = 0; j < controller.parameters.Length; j++)
                    {
                        if(controller.parameters[j].name == floatParamName)
                        {
                            controller.RemoveParameter(j);
                        }
                    }
                    for(int j = 0; j < controller.layers.Length; j++)
                    {
                        if(controller.layers[j].name == "NightMode")
                        {
                            controller.RemoveLayer(j);
                        }
                    }
                    controller.AddParameter(floatParamName, AnimatorControllerParameterType.Float);
                    BlendTree blendTree = new BlendTree();
                    AnimationClip nightmodeAnim0 = (AnimationClip)AssetDatabase.LoadAssetAtPath(pathAnim0, typeof(AnimationClip));
                    AnimationClip nightmodeAnim1 = (AnimationClip)AssetDatabase.LoadAssetAtPath(pathAnim1, typeof(AnimationClip));
                    blendTree.name = "NightModeBlendTree";
                    blendTree.hideFlags = HideFlags.HideInHierarchy;
                    blendTree.AddChild(nightmodeAnim0);
                    blendTree.AddChild(nightmodeAnim1);
                    blendTree.blendParameter = floatParamName;
                    AnimatorStateMachine nightmodeStateMachine = new AnimatorStateMachine();
                    nightmodeStateMachine.name = "NightModeStateMachine";
                    nightmodeStateMachine.hideFlags = HideFlags.HideInHierarchy;
                    nightmodeStateMachine.AddState("NightModeState");
                    nightmodeStateMachine.states[0].state.motion = blendTree;
                    nightmodeStateMachine.states[0].state.writeDefaultValues = false;
                    string controllerPath = AssetDatabase.GetAssetPath(avatarDescriptor.baseAnimationLayers[i].animatorController);
                    AssetDatabase.AddObjectToAsset(blendTree, controllerPath);
                    AssetDatabase.AddObjectToAsset(nightmodeStateMachine, controllerPath);
                    AssetDatabase.AddObjectToAsset(nightmodeStateMachine.states[0].state, controllerPath);
                    AnimatorControllerLayer layer = new AnimatorControllerLayer
                    {
                        defaultWeight = 1.0f,
                        name = "NightMode",
                        stateMachine = nightmodeStateMachine
                    };
                    controller.AddLayer(layer);
                }
            }
            AssetDatabase.SaveAssets();

            // Object
            if(gameObject.transform.Find(namePrefab3) != null)
            {
                DestroyImmediate(gameObject.transform.Find(namePrefab3).gameObject);
            }
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathPrefab3, typeof(GameObject));
            Vector3 pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y-1.0f, gameObject.transform.position.z);
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.identity;
            obj.name = namePrefab3;
            obj.transform.parent = gameObject.transform;

            EditorUtility.DisplayDialog(sCompleteTitle,sCompleteMessage,sOK);
        #else
            if(gameObject.transform.Find(namePrefab2) != null)
            {
                DestroyImmediate(gameObject.transform.Find(namePrefab2).gameObject);
            }
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathPrefab2, typeof(GameObject));
            Vector3 pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y-1.0f, gameObject.transform.position.z);
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.identity;
            obj.name = namePrefab2;
            obj.transform.parent = gameObject.transform;

            EditorUtility.DisplayDialog(sCompleteTitle,sCompleteMessage,sOK);
        #endif
    }
}
#endif