using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using Line = ThisOtherThing.UI.Shapes.Line;

[CustomEditor(typeof(Line))]
[CanEditMultipleObjects]
public class LineEditor : GraphicEditor
{
	Line linearLine;

	ThisOtherThing.UI.ShapeUtils.PointsList.PointListsProperties pointListsProperties;
	RectTransform rectTransform;

	protected SerializedProperty materialProp;
	protected SerializedProperty spriteProp;
	protected SerializedProperty raycastTargetProp;

	protected SerializedProperty shapePropertiesProp;
	protected SerializedProperty pointListPropertiesProp;
	protected SerializedProperty linePropertiesPropertiesProp;
	protected SerializedProperty outlinePropertiesProp;
	protected SerializedProperty shadowPropertiesProp;
	protected SerializedProperty antiAliasingPropertiesProp;

	protected override void OnEnable()
	{
		linearLine = (Line)target;

		rectTransform = linearLine.rectTransform;
		pointListsProperties = linearLine.PointListsProperties;

		materialProp = serializedObject.FindProperty("m_Material");
		spriteProp = serializedObject.FindProperty("Sprite");
		raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");

		shapePropertiesProp = serializedObject.FindProperty("ShapeProperties");
		pointListPropertiesProp = serializedObject.FindProperty("PointListsProperties");
		linePropertiesPropertiesProp = serializedObject.FindProperty("LineProperties");
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
		EditorGUILayout.PropertyField(pointListPropertiesProp, true);
		EditorGUILayout.PropertyField(linePropertiesPropertiesProp, true);
		EditorGUILayout.PropertyField(outlinePropertiesProp, true);

		EditorGUILayout.PropertyField(shadowPropertiesProp, true);
		EditorGUILayout.PropertyField(antiAliasingPropertiesProp, true);

		serializedObject.ApplyModifiedProperties();
	}

	void OnSceneGUI()
	{
		Undo.RecordObject(linearLine, "LinarLine");

		for (int i = 0; i < pointListsProperties.PointListProperties.Length; i++)
		{
			if (
				pointListsProperties.PointListProperties[i].ShowHandles &&
				pointListsProperties.PointListProperties[i].GeneratorData.Generator == ThisOtherThing.UI.ShapeUtils.PointsList.PointListGeneratorData.Generators.Custom
			) {
				if (PointListDrawer.Draw(
					ref pointListsProperties.PointListProperties[i].Positions,
					rectTransform,
					linearLine.LineProperties.Closed,
					2
				))
					linearLine.ForceMeshUpdate();
			}
		}
			

		// if (!Application.isPlaying && linearLine.enabled)
		// {
		// 	linearLine.enabled = false;
		// 	linearLine.enabled = true;
		// }
	}


}
