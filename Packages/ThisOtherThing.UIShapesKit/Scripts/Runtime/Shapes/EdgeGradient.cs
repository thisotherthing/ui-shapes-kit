using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ThisOtherThing.UI.Shapes
{

	[AddComponentMenu("UI/Shapes/Edge Gradient", 100)]
	public class EdgeGradient : MaskableGraphic, IShape
	{
		public enum Positions
		{
			Top,
			Bottom,
			Left,
			Right,

			OuterTop,
			OuterBottom,
			OuterLeft,
			OuterRight
		}

		public GradientProperty[] Properties = { new GradientProperty()};

		[System.Serializable]
		public class GradientProperty
		{
			public float Size = 20.0f;
			public Color32 Color = new Color32(127, 127, 127, 255);

			public Positions Position = Positions.Top;
		}

		Vector3 topLeft = Vector3.zero;
		Color32 gradientColor = new Color32(127, 127, 127, 255);

		public void ForceMeshUpdate()
		{
			SetVerticesDirty();
			SetMaterialDirty();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			Rect pixelRect = RectTransformUtility.PixelAdjustRect(rectTransform, canvas);

			for (int i = 0; i < Properties.Length; i++)
			{

				gradientColor.r = Properties[i].Color.r;
				gradientColor.g = Properties[i].Color.g;
				gradientColor.b = Properties[i].Color.b;
				gradientColor.a = 0;

				switch (Properties[i].Position)
				{
					case Positions.Top:
						topLeft.x = pixelRect.xMin;
						topLeft.y = pixelRect.yMax;

						ShapeUtils.Rects.AddVerticalTwoColorRect(
							ref vh,
							topLeft,
							Properties[i].Size,
							pixelRect.width,
							Properties[i].Color,
							gradientColor,
							GeoUtils.ZeroV2
						);
						break;

					case Positions.Bottom:
						topLeft.x = pixelRect.xMin;
						topLeft.y = pixelRect.yMin + Properties[i].Size;

						ShapeUtils.Rects.AddVerticalTwoColorRect(
							ref vh,
							topLeft,
							Properties[i].Size,
							pixelRect.width,
							gradientColor,
							Properties[i].Color,
							GeoUtils.ZeroV2
						);
						break;

					case Positions.Left:
						topLeft.x = pixelRect.xMin;
						topLeft.y = pixelRect.yMax;

						ShapeUtils.Rects.AddHorizontalTwoColorRect(
							ref vh,
							topLeft,
							pixelRect.height,
							Properties[i].Size,
							Properties[i].Color,
							gradientColor,
							GeoUtils.ZeroV2
						);
						break;

					case Positions.Right:
						topLeft.x = pixelRect.xMax - Properties[i].Size;
						topLeft.y = pixelRect.yMax;

						ShapeUtils.Rects.AddHorizontalTwoColorRect(
							ref vh,
							topLeft,
							pixelRect.height,
							Properties[i].Size,
							gradientColor,
							Properties[i].Color,
							GeoUtils.ZeroV2
						);
						break;

					case Positions.OuterTop:
						topLeft.x = pixelRect.xMin;
						topLeft.y = pixelRect.yMax + Properties[i].Size;

						ShapeUtils.Rects.AddVerticalTwoColorRect(
							ref vh,
							topLeft,
							Properties[i].Size,
							pixelRect.width,
							gradientColor,
							Properties[i].Color,
							GeoUtils.ZeroV2
						);
						break;

					case Positions.OuterBottom:
						topLeft.x = pixelRect.xMin;
						topLeft.y = pixelRect.yMin;

						ShapeUtils.Rects.AddVerticalTwoColorRect(
							ref vh,
							topLeft,
							Properties[i].Size,
							pixelRect.width,
							Properties[i].Color,
							gradientColor,
							GeoUtils.ZeroV2
						);
						break;

					case Positions.OuterLeft:
						topLeft.x = pixelRect.xMin - Properties[i].Size;
						topLeft.y = pixelRect.yMax;

						ShapeUtils.Rects.AddHorizontalTwoColorRect(
							ref vh,
							topLeft,
							pixelRect.height,
							Properties[i].Size,
							gradientColor,
							Properties[i].Color,
							GeoUtils.ZeroV2
						);
						break;

					case Positions.OuterRight:
						topLeft.x = pixelRect.xMax;
						topLeft.y = pixelRect.yMax;

						ShapeUtils.Rects.AddHorizontalTwoColorRect(
							ref vh,
							topLeft,
							pixelRect.height,
							Properties[i].Size,
							Properties[i].Color,
							gradientColor,
							GeoUtils.ZeroV2
						);
						break;
				}
			}
		}
	}
}
