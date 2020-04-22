using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using PixelLine = ThisOtherThing.UI.Shapes.PixelLine;

[CustomEditor(typeof(PixelLine))]
[CanEditMultipleObjects]
public class PixelLineEditor : GraphicEditor
{
	protected SerializedProperty colorProp;
	protected SerializedProperty materialProp;
	protected SerializedProperty raycastTargetProp;

	protected SerializedProperty lineWeightProp;
	protected SerializedProperty snappedPropertiesProp;

	protected override void OnEnable()
	{
		colorProp = serializedObject.FindProperty("m_Color");
		materialProp = serializedObject.FindProperty("m_Material");
		raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");

		lineWeightProp = serializedObject.FindProperty("LineWeight");
		snappedPropertiesProp = serializedObject.FindProperty("SnappedProperties");
	}

	protected override void OnDisable()
	{
		Tools.hidden = false;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(colorProp);
		EditorGUILayout.PropertyField(materialProp);
		EditorGUILayout.PropertyField(raycastTargetProp);
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(lineWeightProp);

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(snappedPropertiesProp, true);

		serializedObject.ApplyModifiedProperties();
	}
}
