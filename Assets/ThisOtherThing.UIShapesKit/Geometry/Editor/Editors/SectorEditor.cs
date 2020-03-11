using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using Sector = ThisOtherThing.UI.Shapes.Sector;

[CustomEditor(typeof(Sector))]
[CanEditMultipleObjects]
public class SectorEditor : GraphicEditor
{
	protected SerializedProperty materialProp;
	protected SerializedProperty raycastTargetProp;

	protected SerializedProperty shapePropertiesProp;
	protected SerializedProperty ellipsePropertiesProp;
	protected SerializedProperty arcPropertiesProp;
	protected SerializedProperty shadowPropertiesProp;
	protected SerializedProperty antiAliasingPropertiesProp;

	protected override void OnEnable()
	{
		materialProp = serializedObject.FindProperty("m_Material");
		raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");

		shapePropertiesProp = serializedObject.FindProperty("ShapeProperties");
		ellipsePropertiesProp = serializedObject.FindProperty("EllipseProperties");
		arcPropertiesProp = serializedObject.FindProperty("ArcProperties");
		shadowPropertiesProp = serializedObject.FindProperty("ShadowProperties");
		antiAliasingPropertiesProp = serializedObject.FindProperty("AntiAliasingProperties");
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

		EditorGUILayout.PropertyField(shapePropertiesProp, true);
		EditorGUILayout.PropertyField(ellipsePropertiesProp, true);
		EditorGUILayout.PropertyField(arcPropertiesProp, true);

		EditorGUILayout.PropertyField(shadowPropertiesProp, true);
		EditorGUILayout.PropertyField(antiAliasingPropertiesProp, true);

		serializedObject.ApplyModifiedProperties();
	}
}
