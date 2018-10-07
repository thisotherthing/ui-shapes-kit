using UnityEngine;
using UnityEditor;

using PolygonProperties = ThisOtherThing.UI.ShapeUtils.Polygons.PolygonProperties;

[CustomPropertyDrawer(typeof(PolygonProperties))]
public class PolygonPropertiesDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

		if (!property.isExpanded)
		{
			return;
		}

		EditorGUI.BeginProperty(position, label, property);

		PolygonProperties polygonProperties = 
			(PolygonProperties)fieldInfo.GetValue(property.serializedObject.targetObject);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 1;

		Rect propertyPosition = new Rect (position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("CenterType"), new GUIContent("Center Mode"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

		switch (polygonProperties.CenterType)
		{
			case PolygonProperties.CenterTypes.CustomPosition:
				EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("CustomCenter"), new GUIContent("Custom Center"));
				break;

			case PolygonProperties.CenterTypes.Offset:
				EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("CenterOffset"), new GUIContent("Offset"));
				break;

			case PolygonProperties.CenterTypes.Calculated:
				break;

			case PolygonProperties.CenterTypes.Cutout:
				EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("CenterOffset"), new GUIContent("Offset"));
				propertyPosition.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("CenterOffset"));
				EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("CutoutProperties"), new GUIContent("Cutout Properties"), true);
				break;
			
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

		PolygonProperties polygonProperties = 
			(PolygonProperties)fieldInfo.GetValue(property.serializedObject.targetObject);

		SerializedProperty centerOffsetProp = property.FindPropertyRelative("CenterOffset");
		SerializedProperty customCenterProp = property.FindPropertyRelative("CustomCenter");

		switch (polygonProperties.CenterType) {
			case PolygonProperties.CenterTypes.Calculated:
				return EditorGUIUtility.singleLineHeight * 2.0f;

			case PolygonProperties.CenterTypes.CustomPosition:
				return EditorGUIUtility.singleLineHeight * 2.25f + EditorGUI.GetPropertyHeight(customCenterProp);

			case PolygonProperties.CenterTypes.Offset:
				return EditorGUIUtility.singleLineHeight * 2.25f + EditorGUI.GetPropertyHeight(centerOffsetProp);

			case PolygonProperties.CenterTypes.Cutout:
				SerializedProperty cutoutProp = property.FindPropertyRelative("CutoutProperties");

				if (cutoutProp.isExpanded)
				{
					return EditorGUIUtility.singleLineHeight * 2.5f + EditorGUI.GetPropertyHeight(cutoutProp) + EditorGUI.GetPropertyHeight(centerOffsetProp);
				}
				else
				{
					return EditorGUIUtility.singleLineHeight * 3.25f + EditorGUI.GetPropertyHeight(centerOffsetProp);
				}

			default:
				return EditorGUIUtility.singleLineHeight * 2.0f;
		}
	}
}
