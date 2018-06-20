using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorControllerParameter = UnityEngine.AnimatorControllerParameter;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

[CustomPropertyDrawer(typeof(ConditionEvaluator))]
public class ConditionSourceDrawer : PropertyDrawer
{
    private bool _initialized;
    private ReorderableList _reorderableList;
    private SerializedProperty _listProp;
    //private ParameterCollection _parameters;

    protected static List<ConditionMode> IntModes;
    protected static List<ConditionMode> FloatModes;

    private ConditionEvaluator _conditionEvaluator;
    private float _listHeight;

    private float _spaceAboveControl = EditorGUIUtility.singleLineHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!_initialized)
        {
            Initialize(property);
        }   
        _listHeight = _reorderableList?.GetHeight() ?? _listHeight;        
        return base.GetPropertyHeight(property, label) + _listHeight + _spaceAboveControl;
    }

    public void Initialize(SerializedProperty property)
    {
        _listProp = property.FindPropertyRelative(nameof(ConditionEvaluator.Items));
        _conditionEvaluator = fieldInfo.GetValue(property.serializedObject.targetObject) as ConditionEvaluator;
        _reorderableList = new ReorderableList(property.serializedObject, _listProp, true, true, true, true);

        if (_conditionEvaluator.Items == null)
        {
            _conditionEvaluator.Items = new List<AnimationCondition>();
        }

        //_parameters = _conditionEvaluator.Parameters;
        _listHeight = _reorderableList.GetHeight();

        _reorderableList.drawElementCallback = (rect, index, isActive, isFocused) => 
        {
            EditorGUI.PropertyField(rect, _listProp.GetArrayElementAtIndex(index));
        };

        if (IntModes == null)
        {
            IntModes = new List<ConditionMode>
            {
                ConditionMode.Greater,
                ConditionMode.Less,
                ConditionMode.Equals,
                ConditionMode.NotEqual
            };
        }

        if (FloatModes == null)
        {
            FloatModes = new List<ConditionMode>
            {
                ConditionMode.Greater,
                ConditionMode.Less

            };
        }

        _reorderableList.drawHeaderCallback = rect =>
        {
            GUI.Label(rect, "Conditions");
        };

        _reorderableList.onAddCallback = list =>
        {
            var _parametersCollection = _conditionEvaluator.Parameters;
            string parameter = "";
            ConditionMode mode = ConditionMode.Greater;
            if (_parametersCollection != null)
            {
                AnimatorControllerParameter[] parameters = _parametersCollection.Items;
                if (parameters.Length > 0)
                {
                    parameter = parameters[0].name;
                    mode = parameters[0].type == AnimatorControllerParameterType.Float || parameters[0].type == AnimatorControllerParameterType.Int 
                        ? ConditionMode.Greater 
                        : ConditionMode.If;
                }
            }

            _conditionEvaluator.Items.Add(new AnimationCondition
            {
                ConditionMode = mode,
                Parameter = parameter,
                Threshold = 0f
            });
        };

        _reorderableList.drawElementCallback = (rect, index, active, focused) =>
        {
            var _parametersCollection = _conditionEvaluator.Parameters;
            SerializedProperty arrayElementAtIndex = _listProp.GetArrayElementAtIndex(index);
            ConditionMode animatorConditionMode = (ConditionMode)arrayElementAtIndex.FindPropertyRelative(nameof(AnimationCondition.ConditionMode)).intValue;
            int num1 = 3;
            Rect rect1 = new Rect(rect.x, rect.y + 2f, rect.width, rect.height - 5f);
            Rect position1 = rect1;
            position1.xMax -= rect1.width / 2f + (float)num1;
            Rect position2 = rect1;
            position2.xMin += rect1.width / 2f + (float)num1;
            Rect position3 = position2;
            position3.xMax -= position2.width / 2f + (float)num1;
            Rect position4 = position2;
            position4.xMin += position2.width / 2f + (float)num1;
            string stringValue = arrayElementAtIndex.FindPropertyRelative("Parameter").stringValue;

            int index1 = _parametersCollection?.IndexOfParameter(stringValue) ?? -1;
            bool flag = false;
            List<string> stringList = new List<string>();

            AnimatorControllerParameter[] controllerParameterArray = null;
            if (_parametersCollection != null)
            {
                controllerParameterArray = _parametersCollection.Items;
                for (int index2 = 0; index2 < controllerParameterArray.Length; ++index2)
                    stringList.Add(controllerParameterArray[index2].name);
            }

            string name = DelayedTextFieldDropDown(position1, stringValue, stringList.ToArray());
            if (stringValue != name)
            {
                index1 = _parametersCollection.IndexOfParameter(name);
                arrayElementAtIndex.FindPropertyRelative(nameof(AnimationCondition.Parameter)).stringValue = name;
                animatorConditionMode = ConditionMode.Greater;
                arrayElementAtIndex.FindPropertyRelative(nameof(AnimationCondition.ConditionMode)).intValue = (int)animatorConditionMode;
                flag = true;
            }

            AnimatorControllerParameterType controllerParameterType = index1 == -1 
                ? (AnimatorControllerParameterType)(-1)
                : controllerParameterArray[index1].type;

            if (index1 != -1 && (controllerParameterType == AnimatorControllerParameterType.Float || controllerParameterType == AnimatorControllerParameterType.Int))
            {
                List<ConditionMode> animatorConditionModeList = controllerParameterType != AnimatorControllerParameterType.Float 
                    ? IntModes 
                    : FloatModes;

                string[] displayedOptions = new string[animatorConditionModeList.Count];
                for (int index2 = 0; index2 < displayedOptions.Length; ++index2)
                    displayedOptions[index2] = animatorConditionModeList[index2].ToString();
                int selectedIndex = -1;
                for (int index2 = 0; index2 < displayedOptions.Length; ++index2)
                {
                    if (animatorConditionMode.ToString() == displayedOptions[index2])
                        selectedIndex = index2;
                }
                if (selectedIndex == -1)
                {
                    Vector2 vector2 = GUI.skin.label.CalcSize(Styles.ErrorIcon);
                    Rect position5 = position3;
                    position5.xMax = position5.xMin + vector2.x;
                    position3.xMin += vector2.x;
                    GUI.Label(position5, Styles.ErrorIcon);
                }
                EditorGUI.BeginChangeCheck();
                int index3 = EditorGUI.Popup(position3, selectedIndex, displayedOptions);
                if (EditorGUI.EndChangeCheck() || flag)
                    arrayElementAtIndex.FindPropertyRelative(nameof(AnimationCondition.ConditionMode)).intValue = (int)animatorConditionModeList[index3];
                EditorGUI.BeginChangeCheck();
                float floatValue = arrayElementAtIndex.FindPropertyRelative(nameof(AnimationCondition.Threshold)).floatValue;
                float num2 = controllerParameterType != AnimatorControllerParameterType.Float ? (float)EditorGUI.IntField(position4, Mathf.FloorToInt(floatValue)) : EditorGUI.FloatField(position4, floatValue);
                if (!EditorGUI.EndChangeCheck() && !flag)
                    return;
                arrayElementAtIndex.FindPropertyRelative(nameof(AnimationCondition.Threshold)).floatValue = num2;
            }
            else if (index1 != -1 && controllerParameterType == AnimatorControllerParameterType.Bool)
            {
                string[] displayedOptions = new string[2]
                {
                      "true",
                      "false"
                };
                int selectedIndex = animatorConditionMode != ConditionMode.IfNot ? 0 : 1;
                EditorGUI.BeginChangeCheck();
                int num2 = EditorGUI.Popup(position2, selectedIndex, displayedOptions);
                if (!EditorGUI.EndChangeCheck() && !flag)
                    return;
                arrayElementAtIndex.FindPropertyRelative(nameof(AnimationCondition.ConditionMode)).intValue = num2 != 0 ? 2 : 1;
            }
            else if (controllerParameterType == AnimatorControllerParameterType.Trigger)
            {
                if (!flag)
                    return;
                arrayElementAtIndex.FindPropertyRelative(nameof(AnimationCondition.ConditionMode)).intValue = 1;
            }
            else
                EditorGUI.LabelField(position2, "Parameter does not exist in Controller");
        };

        _initialized = true;
    }

    internal static string DelayedTextFieldDropDown(Rect position, string text, string[] dropDownElement)
    {
        return DelayedTextFieldDropDown(position, GUIContent.none, text, dropDownElement);
    }

    internal static string DelayedTextFieldDropDown(Rect position, GUIContent label, string text, string[] dropDownElement)
    {
        int controlId = 0;
        return DoTextFieldDropDown(EditorGUI.PrefixLabel(position, controlId, label), controlId, text, dropDownElement, true);
    }

    internal static string DoTextFieldDropDown(Rect rect, int id, string text, string[] dropDownElements, bool delayed)
    {
        Rect position1 = new Rect(rect.x, rect.y, rect.width - Styles.TextFieldDropDown.fixedWidth, rect.height);
        Rect rect1 = new Rect(position1.xMax, position1.y, Styles.TextFieldDropDown.fixedWidth, rect.height);

        text = !delayed 
            ? EditorGUI.TextField(position1, text, Styles.TextFieldDropDownText) 
            : EditorGUI.DelayedTextField(position1, text, Styles.TextFieldDropDownText);

        EditorGUI.BeginChangeCheck();
        int indentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        Rect position2 = rect1;
        string label = "";
        int selectedIndex = -1;
        string[] displayedOptions;
        if (dropDownElements.Length > 0)
            displayedOptions = dropDownElements;
        else
            displayedOptions = new string[1] { "--empty--" };
        GUIStyle textFieldDropDown = Styles.TextFieldDropDown;
        int index = EditorGUI.Popup(position2, label, selectedIndex, displayedOptions, textFieldDropDown);
        if (EditorGUI.EndChangeCheck() && dropDownElements.Length > 0)
            text = dropDownElements[index];
        EditorGUI.indentLevel = indentLevel;
        return text;
    }

    private static class Styles
    {
        public static readonly GUIStyle TextFieldDropDownText = (GUIStyle)"TextFieldDropDownText";
        public static readonly GUIStyle TextFieldDropDown = (GUIStyle)"TextFieldDropDown";
        public static GUIContent ErrorIcon = EditorGUIUtility.IconContent("console.erroricon.sml");
    }
 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();

        if (!_initialized)
        {
            Initialize(property);
        }

        position.y += _spaceAboveControl;
        position.height += _spaceAboveControl;

        _reorderableList.DoList(position);

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();             
        }

    }
}