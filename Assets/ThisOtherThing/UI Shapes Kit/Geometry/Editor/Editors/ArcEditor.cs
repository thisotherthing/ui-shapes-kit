using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using Arc = ThisOtherThing.UI.Shapes.Arc;

[CustomEditor(typeof(Arc))]
[CanEditMultipleObjects]
public class ArcEditor : GraphicEditor
{
	protected SerializedProperty materialProp;
	protected SerializedProperty raycastTargetProp;

	protected SerializedProperty shapePropertiesProp;
	protected SerializedProperty ellipsePropertiesProp;
	protected SerializedProperty arcPropertiesProp;
	protected SerializedProperty lineCapProp;
	protected SerializedProperty CapRoundingPropertiesProp;
	protected SerializedProperty outlinePropertiesProp;
	protected SerializedProperty shadowPropertiesProp;
	protected SerializedProperty antiAliasingPropertiesProp;

	bool capExpanded = false;

	protected override void OnEnable()
	{
		materialProp = serializedObject.FindProperty("m_Material");
		raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");

		shapePropertiesProp = serializedObject.FindProperty("ShapeProperties");
		ellipsePropertiesProp = serializedObject.FindProperty("EllipseProperties");
		arcPropertiesProp = serializedObject.FindProperty("ArcProperties");
		outlinePropertiesProp = serializedObject.FindProperty("OutlineProperties");
		shadowPropertiesProp = serializedObject.FindProperty("ShadowProperties");
		antiAliasingPropertiesProp = serializedObject.FindProperty("AntiAliasingProperties");

		lineCapProp = serializedObject.FindProperty("LineProperties").FindPropertyRelative("LineCap");
		CapRoundingPropertiesProp = serializedObject.FindProperty("LineProperties").FindPropertyRelative("RoundedCapResolution");
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

		capExpanded = EditorGUILayout.Foldout(capExpanded, "Cap");
		if (capExpanded)
		{
			EditorGUILayout.PropertyField(lineCapProp);

			if (lineCapProp.enumValueIndex == (int)ThisOtherThing.UI.ShapeUtils.Lines.LineProperties.LineCapTypes.Round)
			{
				EditorGUILayout.PropertyField(CapRoundingPropertiesProp);
			}
		}

		EditorGUILayout.PropertyField(outlinePropertiesProp, true);

		EditorGUILayout.PropertyField(shadowPropertiesProp, true);
		EditorGUILayout.PropertyField(antiAliasingPropertiesProp, true);

		serializedObject.ApplyModifiedProperties();
	}
}
