using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace Narazaka.VRChat.AddPBCollider.Editor
{
    [CustomEditor(typeof(AddPBCollider))]
    public class AddPBColliderEditor : UnityEditor.Editor
    {
        SerializedProperty boneReference;
        SerializedProperty subPath;
        SerializedProperty pbIndex;
        SerializedProperty colliders;

        void OnEnable()
        {
            boneReference = serializedObject.FindProperty(nameof(AddPBCollider.boneReference));
            subPath = serializedObject.FindProperty(nameof(AddPBCollider.subPath));
            pbIndex = serializedObject.FindProperty(nameof(AddPBCollider.pbIndex));
            colliders = serializedObject.FindProperty(nameof(AddPBCollider.colliders));
        }

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var c = target as AddPBCollider;
                var pb = EditorGUILayout.ObjectField("PhysBone", c.targetPb, typeof(VRCPhysBone), true);
                if (check.changed)
                {
                    Undo.RecordObject(c, "Change targetPb");
                    c.targetPb = pb as VRCPhysBone;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(c);
                }
            }

            serializedObject.UpdateIfRequiredOrScript();
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(boneReference);
                EditorGUILayout.PropertyField(subPath);
                EditorGUILayout.PropertyField(pbIndex);
            }
            EditorGUILayout.PropertyField(colliders, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
