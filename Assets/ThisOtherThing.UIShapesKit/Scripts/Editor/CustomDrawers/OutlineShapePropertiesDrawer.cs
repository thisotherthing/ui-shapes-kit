using UnityEngine;
using UnityEditor;

using ShapeProperties = ThisOtherThing.UI.GeoUtils.OutlineShapeProperties;

[CustomPropertyDrawer(typeof(ShapeProperties))]
public class OutlineShapePropertiesDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

		if (!property.isExpanded)
			return;

		EditorGUI.BeginProperty(position, label, property);

		ShapeProperties shapeProperties = 
			(ShapeProperties)fieldInfo.GetValue(property.serializedObject.targetObject);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 1;

		Rect propertyPosition = new Rect (position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("DrawFill"), new GUIContent("Draw Fill"));

		if (shapeProperties.DrawFill)
		{
			propertyPosition.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("DrawFillShadow"), new GUIContent("Shadow"));

			propertyPosition.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("FillColor"), new GUIContent("Color"));
		}

		propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("DrawOutline"), new GUIContent("Draw Outline"));

		if (shapeProperties.DrawOutline)
		{
			propertyPosition.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("DrawOutlineShadow"), new GUIContent("Shadow"));

			propertyPosition.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("OutlineColor"), new GUIContent("Color"));
		}

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (!property.isExpanded)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		float height = EditorGUIUtility.singleLineHeight * 3.25f;

		ShapeProperties shapeProperties = 
			(ShapeProperties)fieldInfo.GetValue(property.serializedObject.targetObject);

		if (shapeProperties.DrawFill)
		{
			height += EditorGUIUtility.singleLineHeight * 2.0f;
		}

		if (shapeProperties.DrawOutline)
		{
			height += EditorGUIUtility.singleLineHeight * 2.0f;
		}

		return height;
	}
}
