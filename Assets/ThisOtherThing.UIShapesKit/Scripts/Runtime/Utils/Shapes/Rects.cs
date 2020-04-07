using UnityEngine;
using UnityEngine.UI;

namespace ThisOtherThing.UI.ShapeUtils
{
	public class Rects
	{
		static Vector3 tmpPos = Vector3.zero;
		static Vector2 tmpUVPos = Vector2.zero;

		public static void AddRect(
			ref VertexHelper vh,
			Vector2 center,
			float width,
			float height,
			Color32 color,
			Vector2 uv
		) {
			AddRectVertRing(
				ref vh,
				center,
				width,
				height,
				color,
				width,
				height
			);

			AddRectQuadIndices(ref vh);
		}

		public static void AddRect(
			ref VertexHelper vh,
			Vector2 center,
			float width,
			float height,
			Color32 color,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData
		) {
			width += edgeGradientData.ShadowOffset * 2.0f;
			height += edgeGradientData.ShadowOffset * 2.0f;

			float innerOffset = Mathf.Min(width, height) * (1.0f - edgeGradientData.InnerScale);

			AddRectVertRing(
				ref vh,
				center,
				width - innerOffset,
				height - innerOffset,
				color,
				width,
				height
			);

			AddRectQuadIndices(ref vh);

			if (edgeGradientData.IsActive)
			{
				color.a  = 0;

				UI.GeoUtils.AddOffset(
					ref width,
					ref height,
					edgeGradientData.SizeAdd
				);

				AddRectVertRing(
					ref vh,
					center,
					width,
					height,
					color,
					width - edgeGradientData.SizeAdd * 2.0f,
					height - edgeGradientData.SizeAdd * 2.0f,
					true
				);
			}
		}

		public static void AddRectRing(
			ref VertexHelper vh,
			UI.GeoUtils.OutlineProperties OutlineProperties,
			Vector2 center,
			float width,
			float height,
			Color32 color,
			Vector2 uv,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData
		) {
			byte alpha = color.a;

			float fullWidth = width + OutlineProperties.GetOuterDistace() * 2.0f;
			float fullHeight = height + OutlineProperties.GetOuterDistace() * 2.0f;

			width += OutlineProperties.GetCenterDistace() * 2.0f;
			height += OutlineProperties.GetCenterDistace() * 2.0f;

			float halfLineWeightOffset = OutlineProperties.HalfLineWeight * 2.0f + edgeGradientData.ShadowOffset;
			float halfLineWeightInnerOffset = halfLineWeightOffset * edgeGradientData.InnerScale;

			if (edgeGradientData.IsActive)
			{
				color.a  = 0;

				AddRectVertRing(
					ref vh,
					center,
					width - halfLineWeightOffset - edgeGradientData.SizeAdd,
					height - halfLineWeightOffset - edgeGradientData.SizeAdd,
					color,
					fullWidth,
					fullHeight
				);

				color.a = alpha;
			}



			AddRectVertRing(
				ref vh,
				center,
				width - halfLineWeightInnerOffset,
				height - halfLineWeightInnerOffset,
				color,
				fullWidth,
				fullHeight,
				edgeGradientData.IsActive
			);

			AddRectVertRing(
				ref vh,
				center,
				width + halfLineWeightInnerOffset,
				height + halfLineWeightInnerOffset,
				color,
				fullWidth,
				fullHeight,
				true
			);

			if (edgeGradientData.IsActive)
			{
				color.a  = 0;

				AddRectVertRing(
					ref vh,
					center,
					width + halfLineWeightOffset + edgeGradientData.SizeAdd,
					height + halfLineWeightOffset + edgeGradientData.SizeAdd,
					color,
					fullWidth,
					fullHeight,
					true
				);
			}
		}

		public static void AddRectVertRing(
			ref VertexHelper vh,
			Vector2 center,
			float width,
			float height,
			Color32 color,
			float totalWidth,
			float totalHeight,
			bool addRingIndices = false
		) {
			float uvXInset = 0.5f - width / totalWidth * 0.5f;
			float uvYInset = 0.5f - height / totalHeight * 0.5f;

			// TL
			tmpPos.x = center.x - width * 0.5f;
			tmpPos.y = center.y + height * 0.5f;
			tmpUVPos.x = uvXInset;
			tmpUVPos.y = 1.0f - uvYInset;
			vh.AddVert(tmpPos, color, tmpUVPos, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			// TR
			tmpPos.x += width;
			tmpUVPos.x = 1.0f - uvXInset;
			vh.AddVert(tmpPos, color, tmpUVPos, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			// BR
			tmpPos.y -= height;
			tmpUVPos.y = uvYInset;
			vh.AddVert(tmpPos, color, tmpUVPos, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			// BL
			tmpPos.x -= width;
			tmpUVPos.x = uvXInset;
			vh.AddVert(tmpPos, color, tmpUVPos, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			if (addRingIndices)
			{
				int baseIndex = vh.currentVertCount - 8;

				vh.AddTriangle(baseIndex + 4, baseIndex + 5, baseIndex);
				vh.AddTriangle(baseIndex, baseIndex + 5, baseIndex + 1);

				vh.AddTriangle(baseIndex + 1, baseIndex + 5, baseIndex + 6);
				vh.AddTriangle(baseIndex + 1, baseIndex + 6, baseIndex + 2);

				vh.AddTriangle(baseIndex + 2, baseIndex + 6, baseIndex + 7);
				vh.AddTriangle(baseIndex + 7, baseIndex + 3, baseIndex + 2);

				vh.AddTriangle(baseIndex + 4, baseIndex + 3, baseIndex + 7);
				vh.AddTriangle(baseIndex + 4, baseIndex, baseIndex + 3);
			}
		}

		public static void AddRectQuadIndices(
			ref VertexHelper vh
		) {
			int baseIndex = vh.currentVertCount - 4;

			vh.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 3);
			vh.AddTriangle(baseIndex + 3, baseIndex + 1, baseIndex + 2);
		}

		public static void AddVerticalTwoColorRect(
			ref VertexHelper vh,
			Vector3 topLeft,
			float height,
			float width,
			Color32 topColor,
			Color32 bottomColor,
			Vector2 uv
		) {
			int numVertices = vh.currentVertCount;

			vh.AddVert(topLeft, topColor, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent); // TL
			vh.AddVert(topLeft + UI.GeoUtils.RightV3 * width, topColor, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent); // TR
			vh.AddVert(topLeft + UI.GeoUtils.DownV3 * height, bottomColor, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent); // BL
			vh.AddVert(topLeft + UI.GeoUtils.RightV3 * width + UI.GeoUtils.DownV3 * height, bottomColor, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent); // BR

			vh.AddTriangle(numVertices, numVertices + 1, numVertices + 2);
			vh.AddTriangle(numVertices + 2, numVertices + 1, numVertices + 3);
		}

		public static void AddHorizontalTwoColorRect(
			ref VertexHelper vh,
			Vector3 topLeft,
			float height,
			float width,
			Color32 leftColor,
			Color32 rightColor,
			Vector2 uv
		) {
			int numVertices = vh.currentVertCount;

			vh.AddVert(topLeft, leftColor, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent); // TL
			vh.AddVert(topLeft + UI.GeoUtils.RightV3 * width, rightColor, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent); // TR
			vh.AddVert(topLeft + UI.GeoUtils.DownV3 * height, leftColor, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent); // BL
			vh.AddVert(topLeft + UI.GeoUtils.RightV3 * width + UI.GeoUtils.DownV3 * height, rightColor, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent); // BR


			vh.AddTriangle(numVertices, numVertices + 1, numVertices + 2);
			vh.AddTriangle(numVertices + 2, numVertices + 1, numVertices + 3);
		}
	}
}
