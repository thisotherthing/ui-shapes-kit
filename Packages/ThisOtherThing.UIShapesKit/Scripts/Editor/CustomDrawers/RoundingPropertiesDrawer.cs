using UnityEngine;
using UnityEditor;

using RoundingProperties = ThisOtherThing.UI.GeoUtils.RoundingProperties;

[CustomPropertyDrawer(typeof(RoundingProperties))]
public class RoundingPropertiesDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = EditorGUIUtility.singleLineHeight;

		EditorGUI.BeginProperty(position, label, property);

		RoundingProperties.ResolutionType resolutionType =
			(RoundingProperties.ResolutionType)property.FindPropertyRelative("Resolution").enumValueIndex;

		EditorGUI.LabelField(position, label);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel++;

		Rect propertyPosition = new Rect (position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("Resolution"), new GUIContent("Type"));

		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		switch (resolutionType)
		{
			case RoundingProperties.ResolutionType.Calculated:
				EditorGUI.PropertyField(
					propertyPosition,
					property.FindPropertyRelative("ResolutionMaxDistance"),
					new GUIContent("Max Distance")
				);
				break;

			case RoundingProperties.ResolutionType.Fixed:
				EditorGUI.PropertyField(
					propertyPosition,
					property.FindPropertyRelative("FixedResolution"),
					new GUIContent("Resolution")
				);
				break;
		}

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty ();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight * 4.0f;
	}
}
