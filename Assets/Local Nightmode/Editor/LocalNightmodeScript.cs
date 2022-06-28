#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

using Object = UnityEngine.Object;
using ValueType = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.ValueType;

namespace lilLocalNightmode
{
    public class LocalNightmode : MonoBehaviour
    {
        private const string MENU_TITLE = "GameObject/Setup Local Nightmode";
        private const int MENU_PRIORITY = 20;
        private const string ADD_NAME = "_NightMode";
        private const string MENU_NAME = "NightMode";
        private const string LAYER_NAME = "NightMode";
        private const string PARAM_NAME = "NightModeF";
        private const string PREFAB_NAME = "3.0_LocalNightmode";
        private const string PATH_DEFAULT_PARAM = "Assets/expressionParam_Nightmode.asset";
        private const string PATH_DEFAULT_MENU = "Assets/expressionMenu_Nightmode.asset";
        private const string PATH_DEFAULT_CONT = "Assets/expressionAnimator_Nightmode.asset";

        private const string TEXT_DIALOG_TITLE = "LocalNightmode";
        private static readonly string[] TEXT_OK = new[] {"OK", "OK"};
        private static readonly string[] TEXT_MESSAGE_NOT_AVATAR = new[] {
            "This GameObject is not avatar. Local Nightmode can only be installed on avatars.",
            "このGameObjectはアバターではありません。Local Nightmodeはアバターにのみインストールできます。"
        };
        private static readonly string[] TEXT_MESSAGE_TOO_MANY_PARAMS = new[] {
            "There are too many parameters. Please delete unused parameter from ExpressionParameters.",
            "パラメーターが多すぎます。未使用のパラメーターをExpressionParametersから削除してください。"
        };
        private static readonly string[] TEXT_MESSAGE_TOO_MANY_ITEM = new[] {
            "There are too many menu items. Please remove any unused menu items from ExpressionsMenu.",
            "メニューの項目が多すぎます。未使用のアイテムをExpressionsMenuから削除してください。"
        };
        private static readonly string[] TEXT_MESSAGE_COMPLETE = new[] {
            "Complete!",
            "完了しました"
        };

        private static string GetPrefab3Path()                      { return GUIDToPath("e137359a31fecb845b61940f99fc4251"); } // "Assets/Local Nightmode/Runtime/3.0_LocalNightmode.prefab"
        private static string GetAnim0Path()                        { return GUIDToPath("f5474b94da1f37247aad5f76ca4be939"); } // "Assets/Local Nightmode/Runtime/3.0_LocalNightmodeAnim_0.anim"
        private static string GetAnim1Path()                        { return GUIDToPath("6feec18cc39376149b0cb0a60477b9b6"); } // "Assets/Local Nightmode/Runtime/3.0_LocalNightmodeAnim_1.anim"
        private static string GetSampleExpressionParametersPath()   { return GUIDToPath("03a6d797deb62f0429471c4e17ea99a7"); } // "Assets/Sample/VRChat SDK - Avatars/3.0.9/AV3 Demo Assets/Expressions Menu/DefaultExpressionParameters.asset"
        private static string GetSampleExpressionsMenuPath()        { return GUIDToPath("024fb8ef5b3988c46b446863c92f4522"); } // "Assets/Sample/VRChat SDK - Avatars/3.0.9/AV3 Demo Assets/Expressions Menu/DefaultExpressionsMenu.asset"
        private static string GetSampleAnimationControllerPath()    { return GUIDToPath("d40be620cf6c698439a2f0a5144919fe"); } // "Assets/Sample/VRChat SDK - Avatars/3.0.9/AV3 Demo Assets/Animation/Controllers/vrc_AvatarV3FaceLayer.controller"
        private static string GUIDToPath(string GUID)               { return AssetDatabase.GUIDToAssetPath(GUID); }

        private static int lang = 0;

        [MenuItem(MENU_TITLE, false, MENU_PRIORITY)]
        private static void SetupLocalNightmode()
        {
            lang = Application.systemLanguage == SystemLanguage.Japanese ? 1 : 0;

            if(Selection.activeGameObject.GetComponent(typeof(VRCAvatarDescriptor)) == null)
            {
                EditorUtility.DisplayDialog(TEXT_DIALOG_TITLE, TEXT_MESSAGE_NOT_AVATAR[lang], TEXT_OK[lang]);
                return;
            }

            // Copy
            GameObject gameObject = Selection.activeGameObject;
            if(!gameObject.name.Contains(ADD_NAME))
            {
                gameObject = Instantiate(gameObject);
                gameObject.name += ADD_NAME;
            }
            var avatarDescriptor = (VRCAvatarDescriptor)gameObject.GetComponent(typeof(VRCAvatarDescriptor));
            avatarDescriptor.customExpressions = true;
            SetupParams(avatarDescriptor);
            SetupMenu(avatarDescriptor);
            SetupAnimator(avatarDescriptor);
            AssetDatabase.SaveAssets();

            // Object
            if(gameObject.transform.Find(PREFAB_NAME) != null)
            {
                DestroyImmediate(gameObject.transform.Find(PREFAB_NAME).gameObject);
            }
            var prefab = (GameObject)AssetDatabase.LoadAssetAtPath(GetPrefab3Path(), typeof(GameObject));
            var obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            obj.name = PREFAB_NAME;
            obj.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y-1.0f, gameObject.transform.position.z);
            obj.transform.rotation = Quaternion.identity;
            obj.transform.parent = gameObject.transform;

            EditorUtility.DisplayDialog(TEXT_DIALOG_TITLE, TEXT_MESSAGE_COMPLETE[lang], TEXT_OK[lang]);
        }

        [MenuItem(MENU_TITLE, true, MENU_PRIORITY)]
        private static bool CheckGameObject()
        {
            return Selection.activeGameObject != null;
        }

        private static void SetupParams(VRCAvatarDescriptor avatarDescriptor)
        {
            var pars = (VRCExpressionParameters)CreateInstance(avatarDescriptor.expressionParameters, PATH_DEFAULT_PARAM, GetSampleExpressionParametersPath(), typeof(VRCExpressionParameters));
            avatarDescriptor.expressionParameters = pars;
            var param = new VRCExpressionParameters.Parameter
            {
                name = PARAM_NAME,
                valueType = ValueType.Float,
                saved = false,
                defaultValue = 0.0f
            };
            int memoryCount = 0;
            for(int i = 0; i < pars.parameters.Length; i++)
            {
                memoryCount += VRCExpressionParameters.TypeCost(pars.parameters[i].valueType);

                if(pars.parameters[i].name == PARAM_NAME || string.IsNullOrEmpty(pars.parameters[i].name))
                {
                    pars.parameters[i] = param;
                    break;
                }
                else if(i == pars.parameters.Length - 1)
                {
                    if((memoryCount + VRCExpressionParameters.TypeCost(ValueType.Float)) > VRCExpressionParameters.MAX_PARAMETER_COST)
                    {
                        EditorUtility.DisplayDialog(TEXT_DIALOG_TITLE, TEXT_MESSAGE_TOO_MANY_PARAMS[lang], TEXT_OK[lang]);
                        return;
                    }
                    else
                    {
                        Array.Resize(ref pars.parameters, i+2);
                        pars.parameters[i+1] = param;
                        break;
                    }
                }
            }
        }

        private static void SetupMenu(VRCAvatarDescriptor avatarDescriptor)
        {
            VRCExpressionsMenu menu = (VRCExpressionsMenu)CreateInstance(avatarDescriptor.expressionsMenu, PATH_DEFAULT_MENU, GetSampleExpressionsMenuPath(), typeof(VRCExpressionsMenu));
            avatarDescriptor.expressionsMenu = menu;
            for(int i = menu.controls.Count - 1; i >= 0; i--)
            {
                if(menu.controls[i].name == MENU_NAME) menu.controls.RemoveAt(i);
            }
            if(menu.controls.Count == VRCExpressionsMenu.MAX_CONTROLS)
            {
                EditorUtility.DisplayDialog(TEXT_DIALOG_TITLE, TEXT_MESSAGE_TOO_MANY_ITEM[lang], TEXT_OK[lang]);
                return;
            }
            var control = new VRCExpressionsMenu.Control
            {
                name = MENU_NAME,
                type = VRCExpressionsMenu.Control.ControlType.RadialPuppet
            };
            Array.Resize(ref control.subParameters, 1);
            control.subParameters[0] = new VRCExpressionsMenu.Control.Parameter{ name = PARAM_NAME };
            menu.controls.Add(control);
            EditorUtility.SetDirty(menu);
        }

        private static void SetupAnimator(VRCAvatarDescriptor avatarDescriptor)
        {
            AnimatorController cont = null;
            for(int i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++)
            {
                if(avatarDescriptor.baseAnimationLayers[i].type == VRCAvatarDescriptor.AnimLayerType.FX)
                {
                    cont = (AnimatorController)CreateInstance(avatarDescriptor.baseAnimationLayers[i].animatorController, PATH_DEFAULT_CONT, GetSampleAnimationControllerPath(), typeof(AnimatorController));
                    avatarDescriptor.baseAnimationLayers[i].animatorController = cont;
                    avatarDescriptor.baseAnimationLayers[i].isDefault = false;
                    break;
                }
            }

            for(int i = cont.parameters.Length - 1; i >= 0; i--)
            {
                if(cont.parameters[i].name == PARAM_NAME) cont.RemoveParameter(i);
            }
            for(int i = cont.layers.Length - 1; i >= 0; i--)
            {
                if(cont.layers[i].name == LAYER_NAME) cont.RemoveLayer(i);
            }
            cont.AddParameter(PARAM_NAME, AnimatorControllerParameterType.Float);
            var blendTree = new BlendTree();
            AnimationClip nightmodeAnim0 = (AnimationClip)AssetDatabase.LoadAssetAtPath(GetAnim0Path(), typeof(AnimationClip));
            AnimationClip nightmodeAnim1 = (AnimationClip)AssetDatabase.LoadAssetAtPath(GetAnim1Path(), typeof(AnimationClip));
            blendTree.name = "NightModeBlendTree";
            blendTree.hideFlags = HideFlags.HideInHierarchy;
            blendTree.AddChild(nightmodeAnim0);
            blendTree.AddChild(nightmodeAnim1);
            blendTree.blendParameter = PARAM_NAME;
            var nightmodeStateMachine = new AnimatorStateMachine
            {
                name = "NightModeStateMachine",
                hideFlags = HideFlags.HideInHierarchy
            };
            nightmodeStateMachine.AddState("NightModeState");
            nightmodeStateMachine.states[0].state.motion = blendTree;
            nightmodeStateMachine.states[0].state.writeDefaultValues = false;
            string controllerPath = AssetDatabase.GetAssetPath(cont);
            AssetDatabase.AddObjectToAsset(blendTree, controllerPath);
            AssetDatabase.AddObjectToAsset(nightmodeStateMachine, controllerPath);
            AssetDatabase.AddObjectToAsset(nightmodeStateMachine.states[0].state, controllerPath);
            var layer = new AnimatorControllerLayer
            {
                defaultWeight = 1.0f,
                name = LAYER_NAME,
                stateMachine = nightmodeStateMachine
            };
            cont.AddLayer(layer);
            EditorUtility.SetDirty(cont);
        }

        private static Object CreateInstance(Object obj, string path, string origPath, Type type)
        {
            if(obj == null) obj = AssetDatabase.LoadAssetAtPath(origPath, type);
            else if(AssetDatabase.GetAssetPath(obj).Contains(ADD_NAME)) return obj;
            else path = AssetDatabase.GetAssetPath(obj).Replace(".asset", ADD_NAME + ".asset").Replace(".controller", ADD_NAME + ".controller");
            if(!File.Exists(path))
            {
                Object instance = Instantiate(obj);
                EditorUtility.SetDirty(instance);
                AssetDatabase.CreateAsset(instance, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            return AssetDatabase.LoadAssetAtPath(path, type);
        }
    }
}
#endif