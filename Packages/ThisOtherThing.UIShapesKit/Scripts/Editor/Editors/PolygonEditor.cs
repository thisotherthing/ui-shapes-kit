using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using Polygon = ThisOtherThing.UI.Shapes.Polygon;

[CustomEditor(typeof(Polygon))]
public class PolygonEditor : GraphicEditor
{
	Polygon polygon;
	ThisOtherThing.UI.ShapeUtils.PointsList.PointListsProperties pointListsProperties;
	RectTransform rectTransform;

	protected SerializedProperty materialProp;
	protected SerializedProperty raycastTargetProp;

	protected SerializedProperty shapePropertiesProp;
	protected SerializedProperty pointListPropertiesProp;
	protected SerializedProperty polygonPropertiesProp;
	protected SerializedProperty shadowPropertiesProp;
	protected SerializedProperty antiAliasingPropertiesProp;

	protected override void OnEnable()
	{
		polygon = (Polygon)target;

		rectTransform = polygon.rectTransform;
		pointListsProperties = polygon.PointListsProperties;

		materialProp = serializedObject.FindProperty("m_Material");
		raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");

		shapePropertiesProp = serializedObject.FindProperty("ShapeProperties");
		pointListPropertiesProp = serializedObject.FindProperty("PointListsProperties");
		polygonPropertiesProp = serializedObject.FindProperty("PolygonProperties");
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
		EditorGUILayout.PropertyField(raycastTargetProp);
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(shapePropertiesProp, true);
		EditorGUILayout.PropertyField(pointListPropertiesProp, true);
		EditorGUILayout.PropertyField(polygonPropertiesProp, true);

		EditorGUILayout.PropertyField(shadowPropertiesProp, true);
		EditorGUILayout.PropertyField(antiAliasingPropertiesProp, true);

		serializedObject.ApplyModifiedProperties();
	}

	void OnSceneGUI()
	{
		Undo.RecordObject(polygon, "test");

		for (int i = 0; i < pointListsProperties.PointListProperties.Length; i++)
		{
			if (
				pointListsProperties.PointListProperties[i].ShowHandles &&
				pointListsProperties.PointListProperties[i].GeneratorData.Generator == ThisOtherThing.UI.ShapeUtils.PointsList.PointListGeneratorData.Generators.Custom
			) {
				if (PointListDrawer.Draw(
					ref pointListsProperties.PointListProperties[i].Positions,
					rectTransform,
					true,
					3
				))
					polygon.ForceMeshUpdate();
			}
		}


		// if (!Application.isPlaying && polygon.enabled)
		// {
		// 	polygon.enabled = false;
		// 	polygon.enabled = true;
		// }
	}
}
