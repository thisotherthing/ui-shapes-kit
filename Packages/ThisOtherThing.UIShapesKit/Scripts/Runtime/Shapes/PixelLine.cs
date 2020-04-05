using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ThisOtherThing.UI.Shapes
{
	[AddComponentMenu("UI/Shapes/Pixel Line", 100)]
	public class PixelLine : MaskableGraphic, IShape
	{

		public float LineWeight = 1.0f;

		public GeoUtils.SnappedPositionAndOrientationProperties SnappedProperties = 
			new GeoUtils.SnappedPositionAndOrientationProperties();

		Vector3 center = Vector3.zero;

		public void ForceMeshUpdate()
		{
			SetVerticesDirty();
			SetMaterialDirty();
		}

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			ForceMeshUpdate();
		}
		#endif

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			Rect pixelRect = RectTransformUtility.PixelAdjustRect(rectTransform, canvas);

			float pixelSizeScaler = 1.0f;

			if (canvas != null)
			{
				pixelSizeScaler = 1.0f / canvas.scaleFactor;
			}

			float adjustedLineWeight = LineWeight * pixelSizeScaler;

			switch (SnappedProperties.Position)
			{
				case GeoUtils.SnappedPositionAndOrientationProperties.PositionTypes.Center:
					center.x = pixelRect.center.x;
					center.y = pixelRect.center.y;
					break;
				case GeoUtils.SnappedPositionAndOrientationProperties.PositionTypes.Top:
					center.x = pixelRect.center.x;
					center.y = pixelRect.yMax - adjustedLineWeight;
					break;
				case GeoUtils.SnappedPositionAndOrientationProperties.PositionTypes.Bottom:
					center.x = pixelRect.center.x;
					center.y = pixelRect.yMin;
					break;
				case GeoUtils.SnappedPositionAndOrientationProperties.PositionTypes.Left:
					center.x = pixelRect.xMin;
					center.y = pixelRect.center.y;
					break;
				case GeoUtils.SnappedPositionAndOrientationProperties.PositionTypes.Right:
					center.x = pixelRect.xMax;
					center.y = pixelRect.center.y;
					break;
				default:
					throw new System.ArgumentOutOfRangeException ();
			}

			float width = 0.0f;
			float height = 0.0f;

			switch (SnappedProperties.Orientation)
			{
				case GeoUtils.SnappedPositionAndOrientationProperties.OrientationTypes.Horizontal:
					width = pixelRect.width;
					height = adjustedLineWeight;

	//				topLeft.x -= width * 0.5f + adjustedLineWeight;
					break;
				case GeoUtils.SnappedPositionAndOrientationProperties.OrientationTypes.Vertical:
					width = adjustedLineWeight;
					height = pixelRect.height;

	//				topLeft.y += height * 0.5f - adjustedLineWeight;
					break;
				default:
					throw new System.ArgumentOutOfRangeException ();
			}

			ShapeUtils.Rects.AddRect(
				ref vh,
				center,
				width,
				height,
				color,
				GeoUtils.ZeroV2
			);

		}
	}
}
