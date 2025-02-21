using System;
using UnityEditor;
using UnityEngine;
using static PlasticGui.WorkspaceWindow.Merge.MergeInProgress;

namespace OmicronFSM.Editor
{
    [CustomPropertyDrawer(typeof(StateMachine))]
    public class StateMachineDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            GUI.enabled = Application.isPlaying;
            if (GUI.Button(position, "Display Graph"))
            {
                StateMachine machine = fieldInfo.GetValue(property.serializedObject.targetObject) as StateMachine;
                GraphWindow.Display(machine, property.displayName);
            }
            GUI.enabled = true;

            EditorGUI.EndProperty();
        }
    }
}