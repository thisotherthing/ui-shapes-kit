using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using Rectangle = ThisOtherThing.UI.Shapes.Rectangle;

[CustomEditor(typeof(Rectangle))]
[CanEditMultipleObjects]
public class RectangleEditor : GraphicEditor
{
	Rectangle rectangle;

	protected SerializedProperty materialProp;
	protected SerializedProperty spriteProp;
	protected SerializedProperty raycastTargetProp;

	protected SerializedProperty shapePropertiesProp;
	protected SerializedProperty roundedPropertiesProp;
	protected SerializedProperty outlinePropertiesProp;
	protected SerializedProperty shadowPropertiesProp;
	protected SerializedProperty antiAliasingPropertiesProp;

	protected override void OnEnable() 
	{
		rectangle = (Rectangle)target;

		materialProp = serializedObject.FindProperty("m_Material");
		spriteProp = serializedObject.FindProperty("Sprite");
		raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");

		shapePropertiesProp = serializedObject.FindProperty("ShapeProperties");
		roundedPropertiesProp = serializedObject.FindProperty("RoundedProperties");
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
		EditorGUILayout.PropertyField(roundedPropertiesProp, true);

		if (rectangle.ShapeProperties.DrawOutline)
		{
			EditorGUILayout.PropertyField(outlinePropertiesProp, true);
		}

		EditorGUILayout.PropertyField(shadowPropertiesProp, true);
		EditorGUILayout.PropertyField(antiAliasingPropertiesProp, true);

		serializedObject.ApplyModifiedProperties();
	}
}
