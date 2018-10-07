using UnityEngine;
using UnityEngine.UI;

namespace ThisOtherThing.UI.ShapeUtils
{
	public class Ellipses
	{
		[System.Serializable]
		public class EllipseProperties
		{
			public enum EllipseFitting
			{
				Ellipse,
				UniformInner,
				UniformOuter
			}

			public enum ResolutionType
			{
				Calculated,
				Fixed
			}

			public EllipseFitting Fitting = EllipseFitting.UniformInner;

			public float BaseAngle = 0.0f;

			public ResolutionType Resolution = ResolutionType.Calculated;
			public int FixedResolution = 50;
			public float ResolutionMaxDistance = 4.0f;

			public int AdjustedResolution { private set; get; }

			public void OnCheck()
			{
				FixedResolution = Mathf.Max(FixedResolution, 3);
				ResolutionMaxDistance = Mathf.Max(ResolutionMaxDistance, 0.1f);
			}

			public void UpdateAdjusted(Vector2 radius, float offset)
			{
				radius.x += offset;
				radius.y += offset;

				switch (Resolution)
				{
					case ResolutionType.Calculated:
						float circumference;

						if (radius.x == radius.y)
						{
							circumference = GeoUtils.TwoPI * radius.x;
						}
						else
						{
							circumference = Mathf.PI * (
								3.0f * (radius.x + radius.y) -
								Mathf.Sqrt(
									(3.0f * radius.x + radius.y) *
									(radius.x + 3.0f * radius.y)
								)
							);
						}

						AdjustedResolution = Mathf.CeilToInt(circumference / ResolutionMaxDistance);
						break;
					case ResolutionType.Fixed:
						AdjustedResolution = FixedResolution;
						break;
					default:
						throw new System.ArgumentOutOfRangeException ();
				}
			}
		}

		static Vector3 tmpVertPos = Vector3.zero;
		static Vector2 tmpUVPos = Vector2.zero;
		static Vector3 tmpInnerRadius = Vector3.one;
		static Vector3 tmpOuterRadius = Vector3.one;

		public static void SetRadius(
			ref Vector2 radius,
			float width,
			float height,
			EllipseProperties properties
		) {
			width *= 0.5f;
			height *= 0.5f;

			switch (properties.Fitting)
			{
				case EllipseProperties.EllipseFitting.UniformInner:
					radius.x = Mathf.Min(width, height);
					radius.y = radius.x;
					break;
				case EllipseProperties.EllipseFitting.UniformOuter:
					radius.x = Mathf.Max(width, height);
					radius.y = radius.x;
					break;
				case EllipseProperties.EllipseFitting.Ellipse:
					radius.x = width;
					radius.y = height;
					break;
			}
		}

		public static void AddCircle(
			ref VertexHelper vh,
			Vector2 center,
			Vector2 radius,
			EllipseProperties ellipseProperties,
			Color32 color,
			Vector2 uv,
			ref UI.GeoUtils.UnitPositionData unitPositionData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData
		) {

			UI.GeoUtils.SetUnitPositionData(
				ref unitPositionData,
				ellipseProperties.AdjustedResolution,
				ellipseProperties.BaseAngle
			);

			int numVertices = vh.currentVertCount;

			tmpUVPos.x = 0.5f;
			tmpUVPos.y = 0.5f;
			vh.AddVert(center, color, tmpUVPos, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			// add first circle vertex
			tmpVertPos.x = center.x + unitPositionData.UnitPositions[0].x * (radius.x + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;
			tmpVertPos.y = center.y + unitPositionData.UnitPositions[0].y * (radius.y + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;
			tmpVertPos.z = 0.0f;

			tmpUVPos.x = (unitPositionData.UnitPositions[0].x * edgeGradientData.InnerScale + 1.0f) * 0.5f;
			tmpUVPos.y = (unitPositionData.UnitPositions[0].y * edgeGradientData.InnerScale + 1.0f) * 0.5f;
			vh.AddVert(tmpVertPos, color, tmpUVPos, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			for (int i = 1; i < ellipseProperties.AdjustedResolution; i++)
			{
				tmpVertPos.x = center.x + unitPositionData.UnitPositions[i].x * (radius.x + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;
				tmpVertPos.y = center.y + unitPositionData.UnitPositions[i].y * (radius.y + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;
				tmpVertPos.z = 0.0f;

				tmpUVPos.x = (unitPositionData.UnitPositions[i].x * edgeGradientData.InnerScale + 1.0f) * 0.5f;
				tmpUVPos.y = (unitPositionData.UnitPositions[i].y * edgeGradientData.InnerScale + 1.0f) * 0.5f;
				vh.AddVert(tmpVertPos, color, tmpUVPos, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				vh.AddTriangle(numVertices, numVertices + i, numVertices + i + 1);
			}

			vh.AddTriangle(numVertices, numVertices + ellipseProperties.AdjustedResolution, numVertices + 1);

			if (edgeGradientData.IsActive)
			{
				radius.x += edgeGradientData.ShadowOffset + edgeGradientData.SizeAdd;
				radius.y += edgeGradientData.ShadowOffset + edgeGradientData.SizeAdd;

				int outerFirstIndex = numVertices + ellipseProperties.AdjustedResolution;

				color.a = 0;

				// add first point
				tmpVertPos.x = center.x + unitPositionData.UnitPositions[0].x * radius.x;
				tmpVertPos.y = center.y + unitPositionData.UnitPositions[0].y * radius.y;
				tmpVertPos.z = 0.0f;

				tmpUVPos.x = (unitPositionData.UnitPositions[0].x + 1.0f) * 0.5f;
				tmpUVPos.y = (unitPositionData.UnitPositions[0].y + 1.0f) * 0.5f;
				vh.AddVert(tmpVertPos, color, tmpUVPos, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				for (int i = 1; i < ellipseProperties.AdjustedResolution; i++)
				{
					tmpVertPos.x = center.x + unitPositionData.UnitPositions[i].x * radius.x;
					tmpVertPos.y = center.y + unitPositionData.UnitPositions[i].y * radius.y;
					tmpVertPos.z = 0.0f;

					tmpUVPos.x = (unitPositionData.UnitPositions[i].x + 1.0f) * 0.5f;
					tmpUVPos.y = (unitPositionData.UnitPositions[i].y + 1.0f) * 0.5f;
					vh.AddVert(tmpVertPos, color, tmpUVPos, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					vh.AddTriangle(numVertices + i + 1, outerFirstIndex + i, outerFirstIndex + i + 1);
					vh.AddTriangle(numVertices + i + 1, outerFirstIndex + i + 1, numVertices + i + 2);
				}

				vh.AddTriangle(numVertices + 1, outerFirstIndex, outerFirstIndex + 1);
				vh.AddTriangle(numVertices + 2, numVertices + 1, outerFirstIndex + 1);
			}
		}



		public static void AddRing(
			ref VertexHelper vh,
			Vector2 center,
			Vector2 radius,
			UI.GeoUtils.OutlineProperties outlineProperties,
			EllipseProperties ellipseProperties,
			Color32 color,
			Vector2 uv,
			ref UI.GeoUtils.UnitPositionData unitPositionData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData
		) {
			UI.GeoUtils.SetUnitPositionData(
				ref unitPositionData,
				ellipseProperties.AdjustedResolution,
				ellipseProperties.BaseAngle
			);

			float halfLineWeightOffset = (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;

			tmpInnerRadius.x = radius.x + outlineProperties.GetCenterDistace() - halfLineWeightOffset;
			tmpInnerRadius.y = radius.y + outlineProperties.GetCenterDistace() - halfLineWeightOffset;

			tmpOuterRadius.x = radius.x + outlineProperties.GetCenterDistace() + halfLineWeightOffset;
			tmpOuterRadius.y = radius.y + outlineProperties.GetCenterDistace() + halfLineWeightOffset;

			int numVertices = vh.currentVertCount;
			int startVertex = numVertices - 1;

			int baseIndex;

			float uvMaxResolution = (float)ellipseProperties.AdjustedResolution;

			for (int i = 0; i < ellipseProperties.AdjustedResolution; i++)
			{
				uv.x = i / uvMaxResolution;

				tmpVertPos.x = center.x + unitPositionData.UnitPositions[i].x * tmpInnerRadius.x;
				tmpVertPos.y = center.y + unitPositionData.UnitPositions[i].y * tmpInnerRadius.y;
				tmpVertPos.z = 0.0f;
				uv.y = 0.0f;
				vh.AddVert(tmpVertPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				tmpVertPos.x = center.x + unitPositionData.UnitPositions[i].x * tmpOuterRadius.x;
				tmpVertPos.y = center.y + unitPositionData.UnitPositions[i].y * tmpOuterRadius.y;
				tmpVertPos.z = 0.0f;
				uv.y = 1.0f;
				vh.AddVert(tmpVertPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				if (i > 0)
				{
					baseIndex = startVertex + i * 2;
					vh.AddTriangle(baseIndex - 1, baseIndex, baseIndex + 1);
					vh.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 1);
				}
			}

			// add last quad
			{
				uv.x = 1.0f;

				tmpVertPos.x = center.x + unitPositionData.UnitPositions[0].x * tmpInnerRadius.x;
				tmpVertPos.y = center.y + unitPositionData.UnitPositions[0].y * tmpInnerRadius.y;
				tmpVertPos.z = 0.0f;
				uv.y = 0.0f;
				vh.AddVert(tmpVertPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				tmpVertPos.x = center.x + unitPositionData.UnitPositions[0].x * tmpOuterRadius.x;
				tmpVertPos.y = center.y + unitPositionData.UnitPositions[0].y * tmpOuterRadius.y;
				tmpVertPos.z = 0.0f;
				uv.y = 1.0f;
				vh.AddVert(tmpVertPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				baseIndex = startVertex + ellipseProperties.AdjustedResolution * 2;
				vh.AddTriangle(baseIndex - 1, baseIndex, baseIndex + 1);
				vh.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 1);
			}

			if (edgeGradientData.IsActive)
			{
				halfLineWeightOffset = outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset + edgeGradientData.SizeAdd;

				tmpInnerRadius.x = radius.x + outlineProperties.GetCenterDistace() - halfLineWeightOffset;
				tmpInnerRadius.y = radius.y + outlineProperties.GetCenterDistace() - halfLineWeightOffset;

				tmpOuterRadius.x = radius.x + outlineProperties.GetCenterDistace() + halfLineWeightOffset;
				tmpOuterRadius.y = radius.y + outlineProperties.GetCenterDistace() + halfLineWeightOffset;

				color.a = 0;

				int edgesBaseIndex;
				int innerBaseIndex;

				for (int i = 0; i < ellipseProperties.AdjustedResolution; i++)
				{
					uv.x = i / uvMaxResolution;

					tmpVertPos.x = center.x + unitPositionData.UnitPositions[i].x * tmpInnerRadius.x;
					tmpVertPos.y = center.y + unitPositionData.UnitPositions[i].y * tmpInnerRadius.y;
					tmpVertPos.z = 0.0f;
					uv.y = 0.0f;
					vh.AddVert(tmpVertPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					tmpVertPos.x = center.x + unitPositionData.UnitPositions[i].x * tmpOuterRadius.x;
					tmpVertPos.y = center.y + unitPositionData.UnitPositions[i].y * tmpOuterRadius.y;
					tmpVertPos.z = 0.0f;
					uv.y = 1.0f;
					vh.AddVert(tmpVertPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					edgesBaseIndex = baseIndex + i * 2;
					innerBaseIndex = startVertex + i * 2;

					if (i > 0)
					{
						// inner quad
						vh.AddTriangle(innerBaseIndex - 1, innerBaseIndex + 1, edgesBaseIndex + 3);
						vh.AddTriangle(edgesBaseIndex + 1, innerBaseIndex - 1, edgesBaseIndex + 3);

						// outer quad
						vh.AddTriangle(innerBaseIndex, edgesBaseIndex + 2, innerBaseIndex + 2);
						vh.AddTriangle(edgesBaseIndex + 2, edgesBaseIndex + 4, innerBaseIndex + 2);
					}
				}

				// add last quads
				{
					uv.x = 1.0f;

					tmpVertPos.x = center.x + unitPositionData.UnitPositions[0].x * tmpInnerRadius.x;
					tmpVertPos.y = center.y + unitPositionData.UnitPositions[0].y * tmpInnerRadius.y;
					tmpVertPos.z = 0.0f;
					uv.y = 0.0f;
					vh.AddVert(tmpVertPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					tmpVertPos.x = center.x + unitPositionData.UnitPositions[0].x * tmpOuterRadius.x;
					tmpVertPos.y = center.y + unitPositionData.UnitPositions[0].y * tmpOuterRadius.y;
					tmpVertPos.z = 0.0f;
					uv.y = 1.0f;
					vh.AddVert(tmpVertPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					edgesBaseIndex = baseIndex + ellipseProperties.AdjustedResolution * 2;
					innerBaseIndex = startVertex + ellipseProperties.AdjustedResolution * 2;

					// inner quad
					vh.AddTriangle(innerBaseIndex - 1, innerBaseIndex + 1, edgesBaseIndex + 3);
					vh.AddTriangle(edgesBaseIndex + 1, innerBaseIndex - 1, edgesBaseIndex + 3);

					// outer quad
					vh.AddTriangle(innerBaseIndex, edgesBaseIndex + 2, innerBaseIndex + 2);
					vh.AddTriangle(edgesBaseIndex + 2, edgesBaseIndex + 4, innerBaseIndex + 2);
				}
			}
		}

	}
}
