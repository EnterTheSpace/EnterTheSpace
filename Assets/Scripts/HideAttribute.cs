using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
 
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class HideAttribute : PropertyAttribute
{
    public string ConditionalSourceField;
    public bool HideInInspector;
    
    public HideAttribute(string conditionalSourceField)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = true;
    }

    public HideAttribute(string conditionalSourceField, bool other)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = other;
    }
}