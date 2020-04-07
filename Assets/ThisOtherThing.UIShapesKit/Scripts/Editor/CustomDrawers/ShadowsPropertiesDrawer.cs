using UnityEngine;
using UnityEditor;

using ShadowsProperties = ThisOtherThing.UI.GeoUtils.ShadowsProperties;
using ShadowProperties = ThisOtherThing.UI.GeoUtils.ShadowProperties;

[CustomPropertyDrawer(typeof(ShadowsProperties))]
public class ShadowsPropertiesDrawer : PropertyDrawer
{

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

		if (!property.isExpanded)
			return;

		EditorGUI.BeginProperty(position, label, property);

		ShadowsProperties shadowProperties = 
			(ShadowsProperties)fieldInfo.GetValue(property.serializedObject.targetObject);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 1;

		Rect propertyPosition = new Rect (position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("ShowShape"), new GUIContent("Shape"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("ShowShadows"), new GUIContent("Shadows"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.5f;

		if (!shadowProperties.ShowShadows)
		{
			EditorGUI.indentLevel = indent;
			return;
		}

		EditorGUI.LabelField(propertyPosition, new GUIContent("Position Offset"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("Angle"), new GUIContent("Angle"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		EditorGUI.PropertyField(propertyPosition, property.FindPropertyRelative("Distance"), new GUIContent("Distance"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.5f;


		SerializedProperty shadowsProp = property.FindPropertyRelative("Shadows");

		EditorGUI.PropertyField(propertyPosition, shadowsProp, new GUIContent("Shadows"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		EditorGUI.indentLevel++;

		SerializedProperty singleShadowProp;

		if (shadowsProp.isExpanded && shadowProperties.Shadows != null)
		{
			for (int i = 0; i < shadowProperties.Shadows.Length; i++)
			{
				singleShadowProp = shadowsProp.GetArrayElementAtIndex(i);

				bool delete = DrawSingleShadowButtons(singleShadowProp, shadowsProp, propertyPosition, i, shadowProperties.Shadows.Length-1);
				EditorGUI.PropertyField(propertyPosition, singleShadowProp, new GUIContent("Shadow"), true);
				propertyPosition.y += EditorGUI.GetPropertyHeight(singleShadowProp) + EditorGUIUtility.singleLineHeight * (singleShadowProp.isExpanded ? 0.75f : 0.25f);

				if (delete)
				{
					// shift isExpanded downwards, since the delete messes it up otherwise
					for (int j = i; j < shadowProperties.Shadows.Length-1; j++)
					{
						shadowsProp.GetArrayElementAtIndex(j).isExpanded = shadowsProp.GetArrayElementAtIndex(j+1).isExpanded;
					}

					shadowsProp.DeleteArrayElementAtIndex(i);
					break;
				}
			}
		}

		propertyPosition.x += 20.0f;
		propertyPosition.width -= 60.0f;
		if (
			shadowsProp.isExpanded &&
			(
				shadowProperties.Shadows == null ||
				(shadowProperties.Shadows != null && shadowProperties.Shadows.Length == 0)
			) &&
			GUI.Button(propertyPosition, "add shadow")
		) {
			Object targetObject = shadowsProp.serializedObject.targetObject;

			// register undo, since InsertArrayElementAtIndex doesn't set default values
			Undo.RecordObject(targetObject , "added shadow");

			shadowProperties.Shadows = new ShadowProperties[1];
			shadowProperties.Shadows[0] = new ShadowProperties();

			// toogle graphic off and on to force the graphic to be updated
			((UnityEngine.UI.Graphic)targetObject).enabled = false;
			((UnityEngine.UI.Graphic)targetObject).enabled = true;
		}

		EditorGUI.indentLevel--;

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (!property.isExpanded)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		float height = EditorGUIUtility.singleLineHeight * 5.75f;

		ShadowsProperties shadowProperties =
			(ShadowsProperties)fieldInfo.GetValue(property.serializedObject.targetObject);

		if (!shadowProperties.ShowShadows)
		{
			return EditorGUIUtility.singleLineHeight * 3.0f;
		}

		height += EditorGUIUtility.singleLineHeight;
		height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Shadows"));


		// remove height for hidden size field
		SerializedProperty shadowsProp = property.FindPropertyRelative("Shadows");
		if (shadowsProp.isExpanded && shadowProperties.Shadows != null && shadowProperties.Shadows.Length > 0)
		{
			height -= EditorGUIUtility.singleLineHeight;

			// add shadow buttons spacing
			SerializedProperty singleShadowProp;
			for (int i = 0; i < shadowProperties.Shadows.Length - 1; i++)
			{
				singleShadowProp = shadowsProp.GetArrayElementAtIndex(i);
				height += EditorGUIUtility.singleLineHeight * (singleShadowProp.isExpanded ? 0.75f : 0.25f);
			}
		}

		return height;
	}

	float buttonWidth = 20.0f;
	bool DrawSingleShadowButtons(
		SerializedProperty singleShadowProp,
		SerializedProperty shadowsProp,
		Rect position,
		int index,
		int maxIndex
	) {
		GUI.depth--;

		position.x += 100.0f;
		position.width = buttonWidth;

		if (GUI.Button(position, "▼", EditorStyles.miniButtonLeft))
		{
			int newIndex = Mathf.Min(index+1, maxIndex);

			shadowsProp.GetArrayElementAtIndex(newIndex).isExpanded = true;
			shadowsProp.MoveArrayElement(index, newIndex);
		}

		position.x += buttonWidth;
		if (GUI.Button(position, "▲", EditorStyles.miniButtonRight))
		{
			int newIndex = Mathf.Max(index-1, 0);

			shadowsProp.GetArrayElementAtIndex(newIndex).isExpanded = true;
			shadowsProp.MoveArrayElement(index, newIndex);
		}

		position.x += buttonWidth;
		if (GUI.Button(position, "-", EditorStyles.miniButtonLeft))
		{
			return true;
		}

		position.x += buttonWidth;
		if (GUI.Button(position, "+", EditorStyles.miniButtonRight))
		{
			shadowsProp.InsertArrayElementAtIndex(index);
		}

		return false;
	}
}
