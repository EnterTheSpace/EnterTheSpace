using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
 
[CustomPropertyDrawer(typeof(HideAttribute))]
public class HidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HideAttribute condHAtt = (HideAttribute)attribute;
        bool enabled = GetHideAttributeResult(condHAtt, property);
 
        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (!condHAtt.HideInInspector || enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
 
        GUI.enabled = wasEnabled;
    }
 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        HideAttribute condHAtt = (HideAttribute)attribute;
        bool enabled = GetHideAttributeResult(condHAtt, property);
 
        if (!condHAtt.HideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
 
    private bool GetHideAttributeResult(HideAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
 
        if (sourcePropertyValue != null)
        {
            enabled = sourcePropertyValue.boolValue;
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }
 
        return enabled;
    }
}