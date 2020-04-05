using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using Ellipse = ThisOtherThing.UI.Shapes.Ellipse;

[CustomEditor(typeof(Ellipse))]
[CanEditMultipleObjects]
public class EllipseEditor : GraphicEditor
{
	Ellipse ellipse;

	protected SerializedProperty materialProp;
	protected SerializedProperty spriteProp;
	protected SerializedProperty raycastTargetProp;

	protected SerializedProperty shapePropertiesProp;
	protected SerializedProperty ellipsePropertiesProp;
	protected SerializedProperty outlinePropertiesProp;
	protected SerializedProperty shadowPropertiesProp;
	protected SerializedProperty antiAliasingPropertiesProp;

	protected override void OnEnable()
	{
		ellipse = (Ellipse)target;

		materialProp = serializedObject.FindProperty("m_Material");
		spriteProp = serializedObject.FindProperty("Sprite");
		raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");

		shapePropertiesProp = serializedObject.FindProperty("ShapeProperties");
		ellipsePropertiesProp = serializedObject.FindProperty("EllipseProperties");
		outlinePropertiesProp = serializedObject.FindProperty("OutlineProperties");
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
		EditorGUILayout.PropertyField(spriteProp);
		EditorGUILayout.PropertyField(raycastTargetProp);
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(shapePropertiesProp, true);
		EditorGUILayout.PropertyField(ellipsePropertiesProp, true);

		if (ellipse.ShapeProperties.DrawOutline)
		{
			EditorGUILayout.PropertyField(outlinePropertiesProp, true);
		}

		EditorGUILayout.PropertyField(shadowPropertiesProp, true);
		EditorGUILayout.PropertyField(antiAliasingPropertiesProp, true);

		serializedObject.ApplyModifiedProperties();
	}
}
