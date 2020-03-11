using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using EdgeGradient = ThisOtherThing.UI.Shapes.EdgeGradient;

[CustomEditor(typeof(EdgeGradient))]
[CanEditMultipleObjects]
public class EdgeGradientEditor : GraphicEditor
{
	protected SerializedProperty materialProp;
	protected SerializedProperty raycastTargetProp;

	protected SerializedProperty propertiesProp;

	protected override void OnEnable()
	{
		materialProp = serializedObject.FindProperty("m_Material");
		raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");

		propertiesProp = serializedObject.FindProperty("Properties");
	}

	protected override void OnDisable()
	{
		Tools.hidden = false;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(materialProp);
		EditorGUILayout.PropertyField(raycastTargetProp);
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(propertiesProp, new GUIContent("Edges"), true);

		serializedObject.ApplyModifiedProperties();
	}
}
