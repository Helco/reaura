using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Aura
{
    [CustomEditor(typeof(AuraSOVariableSet))]
    public class AuraSOVariableSetEditor : Editor
    {
        private ReorderableList list;
        private AuraSOVariableSet VariableSet => target as AuraSOVariableSet;
        private List<AuraSOVariableSet.Variable> copiedList;

        private void OnEnable()
        {
            copiedList = VariableSet.AsEnumerable.ToList();
            list = new ReorderableList(copiedList, typeof(AuraSOVariableSet.Variable), false, true, false, false);
            list.drawHeaderCallback += r => GUI.Label(r, "Variables");
            list.drawElementCallback += DrawElement;
        }

        private static readonly float padding = 8;
        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            Rect left = new Rect(rect.x, rect.y, (rect.width - padding) / 2, rect.height);
            Rect right = new Rect(rect.x + (rect.width + padding) / 2, rect.y, (rect.width - padding) / 2, rect.height);

            string variableName = copiedList[index].Name;
            bool prevEnabled = GUI.enabled;
            GUI.enabled = false;
            GUI.TextField(left, variableName);
            GUI.enabled = prevEnabled;
            EditorGUI.BeginChangeCheck();
            int newValue = EditorGUI.IntField(right, copiedList[index].Value);
            if (EditorGUI.EndChangeCheck())
            {
                copiedList[index] = new AuraSOVariableSet.Variable(variableName, newValue);
                VariableSet[variableName] = newValue;
            }
        }

        public override void OnInspectorGUI()
        {
            list.DoLayoutList();
        }
    }
}
