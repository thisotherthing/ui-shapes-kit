using UnityEngine;
using UnityEditor;

using LineProperties = ThisOtherThing.UI.ShapeUtils.Lines.LineProperties;

[CustomPropertyDrawer(typeof(LineProperties))]
public class LinePropertiesDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

		if (!property.isExpanded)
			return;

		EditorGUI.BeginProperty(position, label, property);

		LineProperties lineProperties = 
			(LineProperties)fieldInfo.GetValue(property.serializedObject.targetObject);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel++;

		Rect propertyPosition = new Rect (position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("Closed"), new GUIContent("Closed"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

		if (lineProperties.Closed)
		{
			EditorGUI.indentLevel--;
			return;
		}

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("LineCap"), new GUIContent("Cap"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

		switch (lineProperties.LineCap)
		{
			case LineProperties.LineCapTypes.Round:
				EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("RoundedCapResolution"), new GUIContent("Resolution"));
				break;
		}

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty ();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (!property.isExpanded)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		LineProperties lineProperties = 
			(LineProperties)fieldInfo.GetValue(property.serializedObject.targetObject);

		if (lineProperties.Closed)
		{
			return EditorGUIUtility.singleLineHeight * 2.0f;
		}

		if (lineProperties.LineCap != LineProperties.LineCapTypes.Round)
		{
			return EditorGUIUtility.singleLineHeight * 3.25f;
		}

		return EditorGUIUtility.singleLineHeight * 6.5f;
	}
}
