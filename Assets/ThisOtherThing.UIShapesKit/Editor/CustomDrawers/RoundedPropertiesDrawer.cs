using UnityEngine;
using UnityEditor;

using RoundedProperties = ThisOtherThing.UI.ShapeUtils.RoundedRects.RoundedProperties;

[CustomPropertyDrawer(typeof(RoundedProperties))]
public class RoundedPropertiesDrawer : PropertyDrawer
{
	bool showRadiusSettings = false;
	bool showResolutionSettings = false;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

		if (!property.isExpanded)
			return;

		EditorGUI.BeginProperty(position, label, property);

		RoundedProperties roundedProperties = 
			(RoundedProperties)fieldInfo.GetValue(property.serializedObject.targetObject);


		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 1;

		// Calculate rects
		Rect propertyPosition = new Rect (position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField (propertyPosition, property.FindPropertyRelative("Type"), new GUIContent("Type"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		if (roundedProperties.Type == RoundedProperties.RoundedType.None)
		{
			EditorGUI.indentLevel = 0;
			return;
		}

		showRadiusSettings = EditorGUI.Foldout(propertyPosition, showRadiusSettings, new GUIContent("Radius"));

		if (showRadiusSettings)
		{
			EditorGUI.indentLevel++;

			propertyPosition.y += EditorGUIUtility.singleLineHeight;

			switch (roundedProperties.Type)
			{
				case RoundedProperties.RoundedType.Uniform:
					EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("UseMaxRadius"), new GUIContent("Use Max Radius"));

					if (!property.FindPropertyRelative("UseMaxRadius").boolValue)
					{
						propertyPosition.y += EditorGUIUtility.singleLineHeight;
						EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("UniformRadius"), new GUIContent("Radius"));
					}
					break;
				case RoundedProperties.RoundedType.Individual:
					EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("TLRadius"), new GUIContent("Top Left"));
					propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

					EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("TRRadius"), new GUIContent("Top Right"));
					propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

					EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("BRRadius"), new GUIContent("Bottom Right"));
					propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

					EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("BLRadius"), new GUIContent("Bottom Left"));
					propertyPosition.y += EditorGUIUtility.singleLineHeight * 0.25f;

					break;
			}

			EditorGUI.indentLevel--;
		}


		propertyPosition.y += EditorGUIUtility.singleLineHeight;
		showResolutionSettings = EditorGUI.Foldout(propertyPosition, showResolutionSettings, new GUIContent("Resolution"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		if (roundedProperties.Type != RoundedProperties.RoundedType.None && showResolutionSettings)
		{
			EditorGUI.indentLevel++;

			EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("ResolutionMode"), new GUIContent("Mode"));
			propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.5f;

			switch (roundedProperties.ResolutionMode)
			{
				case RoundedProperties.ResolutionType.Uniform:
					DrawFoldedProperty(
						ref propertyPosition,
						property.FindPropertyRelative("UniformResolution"),
						new GUIContent("Uniform")
					);
					break;
				case RoundedProperties.ResolutionType.Individual:
					DrawFoldedProperty(
						ref propertyPosition,
						property.FindPropertyRelative("TLResolution"),
						new GUIContent("Top Left")
					);

					DrawFoldedProperty(
						ref propertyPosition,
						property.FindPropertyRelative("TRResolution"),
						new GUIContent("Top Right")
					);

					DrawFoldedProperty(
						ref propertyPosition,
						property.FindPropertyRelative("BRResolution"),
						new GUIContent("Bottom Right")
					);

					DrawFoldedProperty(
						ref propertyPosition,
						property.FindPropertyRelative("BLResolution"),
						new GUIContent("Bottom Left")
					);
					break;
			}

			EditorGUI.indentLevel--;
		}

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty ();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float height = EditorGUIUtility.singleLineHeight;

		RoundedProperties roundedProperties = 
			(RoundedProperties)fieldInfo.GetValue(property.serializedObject.targetObject);

		if (!property.isExpanded)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		height += EditorGUIUtility.singleLineHeight * 3.0f;

		switch (roundedProperties.Type)
		{
			case RoundedProperties.RoundedType.None:
				height -= EditorGUIUtility.singleLineHeight * 2.0f;
				break;
			case RoundedProperties.RoundedType.Uniform:

				if (showRadiusSettings)
				{
					if (roundedProperties.UseMaxRadius)
					{
						height += EditorGUIUtility.singleLineHeight;
					}
					else
					{
						height += EditorGUIUtility.singleLineHeight * 2.0f;
					}
				}

				break;

			case RoundedProperties.RoundedType.Individual:
				if (showRadiusSettings)
				{
					height += EditorGUIUtility.singleLineHeight * 5.0f;
				}
				break;
		}

		// add resolution settings height
		if (roundedProperties.Type != RoundedProperties.RoundedType.None && showResolutionSettings)
		{
			switch (roundedProperties.ResolutionMode)
			{
				case RoundedProperties.ResolutionType.Uniform:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("UniformResolution"));
					break;
				case RoundedProperties.ResolutionType.Individual:
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("TLResolution"));
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("TRResolution"));
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("BRResolution"));
					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("BLResolution")); 
					break;
				default:
					throw new System.ArgumentOutOfRangeException ();
			}

			height += EditorGUIUtility.singleLineHeight * 0.5f;
		}

		return height;
	}

	float GetFoldedPropertyHeight(SerializedProperty property, float expandedHeight, float foldedHeight = 0.0f)
	{
		return property.isExpanded ? expandedHeight : foldedHeight;
	}

	void DrawFoldedProperty(
		ref Rect propertyPosition,
		SerializedProperty property,
		GUIContent label,
		bool checkChildren = true
	) {
		EditorGUI.BeginProperty(propertyPosition, label, property);

		EditorGUI.PropertyField(propertyPosition, property, label, checkChildren);
		propertyPosition.y += EditorGUI.GetPropertyHeight(property);

		EditorGUI.EndProperty();
	}
}
