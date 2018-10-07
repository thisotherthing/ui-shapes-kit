using UnityEngine;
using UnityEditor;

using PointListProperties = ThisOtherThing.UI.ShapeUtils.PointsList.PointListProperties;
using PointListGeneratorData = ThisOtherThing.UI.ShapeUtils.PointsList.PointListGeneratorData;

[CustomPropertyDrawer(typeof(PointListProperties))]
public class PointListPropertiesDrawer : PropertyDrawer
{
	static GUIStyle warningStyle;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

		if (!property.isExpanded)
			return;

		EditorGUI.BeginProperty(position, label, property);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel++;

		Rect propertyPosition = new Rect (position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

		SerializedProperty generatorDataProp = property.FindPropertyRelative("GeneratorData");
		SerializedProperty generatorProp = generatorDataProp.FindPropertyRelative("Generator");

		EditorGUI.BeginChangeCheck();

		EditorGUI.PropertyField(propertyPosition, generatorProp, new GUIContent("Generator"));
		propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.5f;

		PointListGeneratorData.Generators currentGenerator = (PointListGeneratorData.Generators)generatorProp.enumValueIndex;

		SerializedProperty floatValuesProp = generatorDataProp.FindPropertyRelative("FloatValues");

		if (EditorGUI.EndChangeCheck())
		{
			// reset values on generator change
			generatorDataProp.FindPropertyRelative("Center").vector2Value = Vector2.zero;

			generatorDataProp.FindPropertyRelative("IntStartOffset").intValue = 0;
			generatorDataProp.FindPropertyRelative("FloatStartOffset").floatValue = 0.0f;

			generatorDataProp.FindPropertyRelative("Width").floatValue = 50.0f;
			generatorDataProp.FindPropertyRelative("Height").floatValue = 50.0f;

			generatorDataProp.FindPropertyRelative("Radius").floatValue = 50.0f;

			generatorDataProp.FindPropertyRelative("Length").floatValue = 1.0f;
			generatorDataProp.FindPropertyRelative("EndRadius").floatValue = 0.0f;

			generatorDataProp.FindPropertyRelative("Resolution").intValue = 30;

			generatorDataProp.FindPropertyRelative("Angle").floatValue = 0.0f;

			// initialize float values
			if (GeneratorUsesFloatValues(currentGenerator))
			{
				if (ShouldShowTooFewValuesMessage(floatValuesProp))
				{
					generatorDataProp.FindPropertyRelative("MinFloatValue").floatValue = 0.0f;
					generatorDataProp.FindPropertyRelative("MaxFloatValue").floatValue = 20.0f;

					floatValuesProp.arraySize = 3;
					floatValuesProp.GetArrayElementAtIndex(0).floatValue = 5.0f;
					floatValuesProp.GetArrayElementAtIndex(1).floatValue = 9.0f;
					floatValuesProp.GetArrayElementAtIndex(2).floatValue = 15.0f;
				}
			}
			else
			{
				floatValuesProp.arraySize = 0;
			}

			switch (currentGenerator) {
				case PointListGeneratorData.Generators.Custom:
					break;

				case PointListGeneratorData.Generators.Rect:
					break;

				case PointListGeneratorData.Generators.Round:
					break;

				case PointListGeneratorData.Generators.RadialGraph:
					break;

				case PointListGeneratorData.Generators.LineGraph:
					break;

				case PointListGeneratorData.Generators.AngleLine:
					generatorDataProp.FindPropertyRelative ("Length").floatValue = 20.0f;
					generatorDataProp.FindPropertyRelative ("FloatStartOffset").floatValue = -0.5f;
					break;
				case PointListGeneratorData.Generators.Star:
					generatorDataProp.FindPropertyRelative ("EndRadius").floatValue = 0.4f;
					generatorDataProp.FindPropertyRelative("Resolution").intValue = 5;
					break;
				
				case PointListGeneratorData.Generators.Gear:
					generatorDataProp.FindPropertyRelative ("EndRadius").floatValue = 0.6f;
					generatorDataProp.FindPropertyRelative("Resolution").intValue = 5;

					generatorDataProp.FindPropertyRelative ("InnerScaler").floatValue = 0.6f;
					generatorDataProp.FindPropertyRelative ("OuterScaler").floatValue = 0.15f;
					break;
			}
		}

		if (GeneratorUsesFloatValues(currentGenerator))
		{
			propertyPosition.y += CheckAndShowTooFewValuesMessage(propertyPosition, floatValuesProp);
		}

		switch (currentGenerator) {
			#region custom
			case PointListGeneratorData.Generators.Custom:
				SerializedProperty positionsProp = property.FindPropertyRelative("Positions");
				EditorGUI.PropertyField(propertyPosition, positionsProp, new GUIContent("Positions"), true);
				propertyPosition.y += EditorGUI.GetPropertyHeight(positionsProp);

				break;
			#endregion

			#region Rect
			case PointListGeneratorData.Generators.Rect:
				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Center"),
					new GUIContent("Center"));
				propertyPosition.y +=
					EditorGUI.GetPropertyHeight(generatorDataProp.FindPropertyRelative("Center")) +
					EditorGUIUtility.singleLineHeight * 0.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Width"),
					new GUIContent("Width"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Height"),
					new GUIContent("Height"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("IntStartOffset"),
					new GUIContent("Start Offset"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				break;
			#endregion
			
			#region Round
			case PointListGeneratorData.Generators.Round:
				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Center"),
					new GUIContent("Center"));
				propertyPosition.y +=
					EditorGUI.GetPropertyHeight(generatorDataProp.FindPropertyRelative("Center")) +
					EditorGUIUtility.singleLineHeight * 0.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Width"),
					new GUIContent("Width"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Height"),
					new GUIContent("Height"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("FloatStartOffset"),
					new GUIContent("Start Offset"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Length"),
					new GUIContent("Length"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("EndRadius"),
					new GUIContent("EndRadius"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Resolution"),
					new GUIContent("Resolution"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("CenterPoint"),
					new GUIContent("Add Center"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("SkipLastPosition"),
					new GUIContent("Skip Last Point"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				break;
			#endregion

			#region RadialGraph
			case PointListGeneratorData.Generators.RadialGraph:
				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Center"),
					new GUIContent("Center"));
				propertyPosition.y +=
					EditorGUI.GetPropertyHeight(generatorDataProp.FindPropertyRelative("Center")) +
					EditorGUIUtility.singleLineHeight * 0.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Radius"),
					new GUIContent("Radius"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("FloatStartOffset"),
					new GUIContent("Angle Offset"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("MinFloatValue"),
					new GUIContent("Min Value"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("MaxFloatValue"),
					new GUIContent("Max Value"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(propertyPosition, floatValuesProp, new GUIContent("Values"), true);
				propertyPosition.y += EditorGUI.GetPropertyHeight(floatValuesProp);

				break;
			#endregion

			#region LineGraph
			case PointListGeneratorData.Generators.LineGraph:
				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Center"),
					new GUIContent("Center"));
				propertyPosition.y +=
					EditorGUI.GetPropertyHeight(generatorDataProp.FindPropertyRelative("Center")) +
					EditorGUIUtility.singleLineHeight * 0.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Width"),
					new GUIContent("Width"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Height"),
					new GUIContent("Height"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("MinFloatValue"),
					new GUIContent("Min Value"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("MaxFloatValue"),
					new GUIContent("Max Value"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.PropertyField(propertyPosition, floatValuesProp, new GUIContent("Values"), true);
				propertyPosition.y += EditorGUI.GetPropertyHeight(floatValuesProp);

				propertyPosition.y += EditorGUIUtility.singleLineHeight * 0.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("CenterPoint"),
					new GUIContent("Add Bottom Points"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				break;
			#endregion

			#region AngleLine
			case PointListGeneratorData.Generators.AngleLine:
				if (!RunPolygonWarning(property, propertyPosition))
				{
					EditorGUI.PropertyField(
						propertyPosition,
						generatorDataProp.FindPropertyRelative("Center"),
						new GUIContent("Center"));
					propertyPosition.y +=
						EditorGUI.GetPropertyHeight(generatorDataProp.FindPropertyRelative("Center")) +
						EditorGUIUtility.singleLineHeight * 0.25f;

					EditorGUI.PropertyField(
						propertyPosition,
						generatorDataProp.FindPropertyRelative("Angle"),
						new GUIContent("Angle"));
					propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

					EditorGUI.PropertyField(
						propertyPosition,
						generatorDataProp.FindPropertyRelative("Length"),
						new GUIContent("Length"));
					propertyPosition.y += EditorGUIUtility.singleLineHeight;

					EditorGUI.PropertyField(
						propertyPosition,
						generatorDataProp.FindPropertyRelative("FloatStartOffset"),
						new GUIContent("Relative Position"));
					propertyPosition.y += EditorGUIUtility.singleLineHeight;
				}
				else
				{
					propertyPosition.y += EditorGUIUtility.singleLineHeight;
				}

				break;
			#endregion

			#region Star
			case PointListGeneratorData.Generators.Star:
				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Center"),
					new GUIContent("Center"));
				propertyPosition.y +=
					EditorGUI.GetPropertyHeight(generatorDataProp.FindPropertyRelative("Center")) +
					EditorGUIUtility.singleLineHeight * 0.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Radius"),
					new GUIContent("Radius"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("FloatStartOffset"),
					new GUIContent("Rotate"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Resolution"),
					new GUIContent("Resolution"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.Slider(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("EndRadius"),
					0.0f,
					0.99f,
					new GUIContent("Spike Amount")
				);
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				break;
				#endregion

			#region Gear
			case PointListGeneratorData.Generators.Gear:
				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Center"),
					new GUIContent("Center"));
				propertyPosition.y +=
					EditorGUI.GetPropertyHeight(generatorDataProp.FindPropertyRelative("Center")) +
					EditorGUIUtility.singleLineHeight * 0.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Radius"),
					new GUIContent("Radius"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("FloatStartOffset"),
					new GUIContent("Rotate"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.25f;

				EditorGUI.PropertyField(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("Resolution"),
					new GUIContent("Resolution"));
				propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.5f;

				EditorGUI.Slider(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("EndRadius"),
					0.1f,
					1.0f,
					new GUIContent("Teeth Amount")
				);
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.Slider(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("InnerScaler"),
					0.01f,
					1.0f,
					new GUIContent("Inner Size")
				);
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.Slider(
					propertyPosition,
					generatorDataProp.FindPropertyRelative("OuterScaler"),
					0.01f,
					1.0f,
					new GUIContent("Outer Size")
				);
				propertyPosition.y += EditorGUIUtility.singleLineHeight;

				break;
				#endregion
		}

		propertyPosition.y += EditorGUIUtility.singleLineHeight * 0.5f;

		EditorGUI.PropertyField(
			propertyPosition,
			property.FindPropertyRelative("MaxAngle"),
			new GUIContent("Max Angle")
		);
		propertyPosition.y += EditorGUIUtility.singleLineHeight;

		EditorGUI.PropertyField(
			propertyPosition,
			property.FindPropertyRelative("RoundingDistance"),
			new GUIContent("Rounding Distance")
		);
		propertyPosition.y += EditorGUIUtility.singleLineHeight * 1.5f;

		EditorGUI.PropertyField(
			propertyPosition,
			property.FindPropertyRelative("CornerRounding"),
			new GUIContent("Corner Rounding")
		);
		propertyPosition.y += EditorGUIUtility.singleLineHeight * 3.5f;

		if (currentGenerator == PointListGeneratorData.Generators.Custom)
		EditorGUI.PropertyField(
			propertyPosition,
			property.FindPropertyRelative("ShowHandles"),
			new GUIContent("Show Handles")
		);

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty ();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		SerializedProperty generatorDataProp = property.FindPropertyRelative("GeneratorData");
		SerializedProperty generatorProp = generatorDataProp.FindPropertyRelative("Generator");

		if (!property.isExpanded)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		PointListGeneratorData.Generators currentGenerator = (PointListGeneratorData.Generators)generatorProp.enumValueIndex;

		SerializedProperty floatValuesProp = generatorDataProp.FindPropertyRelative("FloatValues");

		float propHeight = EditorGUIUtility.singleLineHeight * 8.25f;


		if (GeneratorUsesFloatValues(currentGenerator) && ShouldShowTooFewValuesMessage(floatValuesProp))
		{
			propHeight += EditorGUIUtility.singleLineHeight * 1.5f;
		}

		SerializedProperty centerProp = generatorDataProp.FindPropertyRelative("Center");

		switch (currentGenerator) {
			case PointListGeneratorData.Generators.Custom:
				SerializedProperty positionsProp = property.FindPropertyRelative("Positions");
				propHeight += EditorGUI.GetPropertyHeight(positionsProp);

				// add space for show handles
				propHeight += EditorGUIUtility.singleLineHeight * 2.0f;

				break;

			case PointListGeneratorData.Generators.Rect:
				propHeight += EditorGUI.GetPropertyHeight(centerProp);
				propHeight += EditorGUIUtility.singleLineHeight * 3.5f;
				break;
				
			case PointListGeneratorData.Generators.Round:
				propHeight += EditorGUI.GetPropertyHeight(centerProp);
				propHeight += EditorGUIUtility.singleLineHeight * 9.5f;
				break;
				
			case PointListGeneratorData.Generators.RadialGraph:
				propHeight += EditorGUI.GetPropertyHeight(centerProp);
				propHeight += EditorGUIUtility.singleLineHeight * 4.75f;

				propHeight += EditorGUI.GetPropertyHeight(floatValuesProp);

				break;
				
			case PointListGeneratorData.Generators.LineGraph:
				propHeight += EditorGUI.GetPropertyHeight(centerProp);
				propHeight += EditorGUIUtility.singleLineHeight * 6.0f;

				propHeight += EditorGUI.GetPropertyHeight(floatValuesProp);
				break;
				
			case PointListGeneratorData.Generators.AngleLine:
				propHeight += EditorGUI.GetPropertyHeight(centerProp);
				propHeight += EditorGUIUtility.singleLineHeight * 3.5f;
				break;

			case PointListGeneratorData.Generators.Star:
				propHeight += EditorGUI.GetPropertyHeight(centerProp);
				propHeight += EditorGUIUtility.singleLineHeight * 5.0f;
				break;

			case PointListGeneratorData.Generators.Gear:
				propHeight += EditorGUI.GetPropertyHeight(centerProp);
				propHeight += EditorGUIUtility.singleLineHeight * 7.5f;
				break;
		}

		return propHeight;
	}

	bool GeneratorUsesFloatValues(PointListGeneratorData.Generators generator)
	{
		return 
			generator == PointListGeneratorData.Generators.LineGraph ||
			generator == PointListGeneratorData.Generators.RadialGraph;
	}

	bool ShouldShowTooFewValuesMessage(SerializedProperty floatValuesProp)
	{
		return floatValuesProp.arraySize < 3;
	}

	float CheckAndShowTooFewValuesMessage(Rect position, SerializedProperty floatValuesProp)
	{
		if (ShouldShowTooFewValuesMessage(floatValuesProp))
		{
			if (warningStyle == null)
			{
				warningStyle = new GUIStyle();
				warningStyle.normal.textColor = Color.yellow;
			}

			EditorGUI.LabelField(position, "Please add at least 3 values", warningStyle);

			return EditorGUIUtility.singleLineHeight * 1.5f;
		}

		return 0.0f;
	}

	bool RunPolygonWarning(SerializedProperty property, Rect position)
	{
		var polygon = property.serializedObject.targetObject as ThisOtherThing.UI.Shapes.Polygon;

		if (polygon != null)
		{
			if (warningStyle == null)
			{
				warningStyle = new GUIStyle();
				warningStyle.normal.textColor = Color.yellow;
			}

			EditorGUI.LabelField(position, "Angle Line isn't supportet with this shape", warningStyle);

			return true;
		}
		else
		{
			return false;
		}
	}
}
