using UnityEngine;
using UnityEditor;

using EllipseProperties = ThisOtherThing.UI.ShapeUtils.Ellipses.EllipseProperties;

[CustomPropertyDrawer(typeof(EllipseProperties))]
public class EllipsePropertiesDrawer : PropertyDrawer
{

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

		if (!property.isExpanded)
			return;

		EditorGUI.BeginProperty(position, label, property);

		EllipseProperties roundedProperties = 
			(EllipseProperties)fieldInfo.GetValue(property.serializedObject.targetObject);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 1;

		Rect propertyPosition = new Rect (position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("Fitting"), new GUIContent("Fitting"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("BaseAngle"), new GUIContent("Base Angle"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		EditorGUI.LabelField(propertyPosition, "Resolution");
		propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("Resolution"), new GUIContent("Mode"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		switch (roundedProperties.Resolution)
		{
			case EllipseProperties.ResolutionType.Calculated:
				EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("ResolutionMaxDistance"), new GUIContent("Max Distance"));
				break;
			case EllipseProperties.ResolutionType.Fixed:
				EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("FixedResolution"), new GUIContent("Resolution"));
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

		return EditorGUIUtility.singleLineHeight * 7.25f;
	}
}
