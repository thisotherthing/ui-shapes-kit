using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UnityEditor;

using ThisOtherThing.UI.Shapes;

public class MenuItems
{

	[MenuItem("GameObject/UI/Shapes/Rectangle", false, 1)]
	public static void AddRectangle(MenuCommand menuCommand)
	{
		Undo.AddComponent<Rectangle>(CreateShapeGO("Rectangle", menuCommand));
	}

	[MenuItem("GameObject/UI/Shapes/Ellipse", false, 1)]
	public static void AddEllipse(MenuCommand menuCommand)
	{
		Undo.AddComponent<Ellipse>(CreateShapeGO("Ellipse", menuCommand));
	}


	[MenuItem("GameObject/UI/Shapes/Line", false, 30)]
	public static void AddLine(MenuCommand menuCommand)
	{
		Undo.AddComponent<Line>(CreateShapeGO("Line", menuCommand));
	}

	[MenuItem("GameObject/UI/Shapes/Polygon", false, 30)]
	public static void AddPolygon(MenuCommand menuCommand)
	{
		Undo.AddComponent<Polygon>(CreateShapeGO("Polygon", menuCommand));
	}


	[MenuItem("GameObject/UI/Shapes/Arc", false, 50)]
	public static void AddArc(MenuCommand menuCommand)
	{
		Undo.AddComponent<Arc>(CreateShapeGO("Arc", menuCommand));
	}

	[MenuItem("GameObject/UI/Shapes/Sector", false, 50)]
	public static void AddSector(MenuCommand menuCommand)
	{
		Undo.AddComponent<Sector>(CreateShapeGO("Sector", menuCommand));
	}


	[MenuItem("GameObject/UI/Shapes/Edge Gradient", false, 100)]
	public static void AddEdgeGradient(MenuCommand menuCommand)
	{
		Undo.AddComponent<EdgeGradient>(CreateShapeGO("Edge Gradient", menuCommand));
	}

	[MenuItem("GameObject/UI/Shapes/Pixel Line", false, 100)]
	public static void AddPixelLine(MenuCommand menuCommand)
	{
		Undo.AddComponent<PixelLine>(CreateShapeGO("Pixel Line", menuCommand));
	}


	[MenuItem("GameObject/UI/Shapes/Empty Fill Rect", false, 100)]
	public static void AddEmptyFillRect(MenuCommand menuCommand)
	{
		Undo.AddComponent<EmptyFillRect>(CreateShapeGO("Empty Fill Rect", menuCommand));
	}


	public static GameObject CreateShapeGO(string name, MenuCommand menuCommand)
	{
		GameObject shapeGO = new GameObject(name);
		Undo.RegisterCreatedObjectUndo(shapeGO, "Created " + name + " shape");

		GameObject parent = (GameObject)menuCommand.context;

		if (
			parent != null &&
			(parent.GetComponent<Canvas>() || parent.GetComponentInParent<Canvas>())
		) {
			Undo.SetTransformParent(
				shapeGO.transform,
				parent.transform,
				"Set " + name + " parent"
			);

			Undo.RecordObject(shapeGO.transform, "centered " + name);
			shapeGO.transform.localPosition = Vector3.zero;
			shapeGO.transform.localScale = Vector3.one;
		}

		Selection.activeGameObject = shapeGO;

		return shapeGO;
	}

	public static GameObject CreateCanvas()
	{
		GameObject canvasGO = new GameObject("Canvas");
		Undo.RegisterCreatedObjectUndo(canvasGO, "Created Canvas");

		Canvas canvas = Undo.AddComponent<Canvas>(canvasGO);
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		Undo.AddComponent<CanvasScaler>(canvasGO);
		Undo.AddComponent<GraphicRaycaster>(canvasGO);

		return canvasGO;
	}
}
