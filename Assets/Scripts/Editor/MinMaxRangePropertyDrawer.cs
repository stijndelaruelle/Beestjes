using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//http://www.grapefruitgames.com/blog/2013/11/a-min-max-range-for-unity/

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        MinMaxRangeAttribute range = attribute as MinMaxRangeAttribute;
        SerializedProperty minProperty = property.FindPropertyRelative("m_Min");
        SerializedProperty maxProperty = property.FindPropertyRelative("m_Max");

        // Calculate rects
        Rect minRect = new Rect(position.x, position.y, 50, position.height);
        Rect maxRect = new Rect(position.x + position.width - 50, position.y, 50, position.height);
        Rect sliderRect = new Rect(position.x + 55, position.y, position.width - 110, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        float minValue = EditorGUI.FloatField(minRect, minProperty.floatValue);
        float maxValue = EditorGUI.FloatField(maxRect, maxProperty.floatValue);
        EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, range.MinLimit, range.MaxLimit);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();

        minProperty.floatValue = minValue;
        maxProperty.floatValue = maxValue;
    }
}