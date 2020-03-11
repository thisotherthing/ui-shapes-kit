using UnityEngine;
using UnityEngine.UI;

using ThisOtherThing.Utils;

namespace ThisOtherThing.UI.ShapeUtils
{
	public class Polygons
	{
		static Vector3 tmpPos = Vector3.zero;

		[System.Serializable]
		public class PolygonProperties
		{
			public enum CenterTypes
			{
				Calculated,
				Offset,
				CustomPosition,
				Cutout
			}

			public CenterTypes CenterType = CenterTypes.Calculated;
			public Vector2 CenterOffset = Vector2.zero;
			public Vector2 CustomCenter = Vector2.zero;

			[HideInInspector]
			public Vector2 AdjustedCenter = Vector2.zero;

			public CutoutProperties CutoutProperties = new CutoutProperties();

			public void UpdateAdjusted(PointsList.PointListProperties pointListProperties)
			{
				AdjustedCenter.x = 0.0f;
				AdjustedCenter.y = 0.0f;

				if (CenterType == CenterTypes.CustomPosition)
				{
					AdjustedCenter.x = CustomCenter.x;
					AdjustedCenter.y = CustomCenter.y;
				}
				else
				{
					for (int i = 0; i < pointListProperties.Positions.Length; i++)
					{
						AdjustedCenter.x += pointListProperties.Positions[i].x;
						AdjustedCenter.y += pointListProperties.Positions[i].y;
					}

					AdjustedCenter.x /= (float)pointListProperties.Positions.Length;
					AdjustedCenter.y /= (float)pointListProperties.Positions.Length;
				}

				if (CenterType == CenterTypes.Cutout)
				{
					float safeRotationOffset = CutoutProperties.RotationOffset;

					if (safeRotationOffset < 0.0f)
					{
						safeRotationOffset = GeoUtils.TwoPI + safeRotationOffset;
					}

					float step = (GeoUtils.TwoPI / CutoutProperties.Resolution);

					safeRotationOffset %= step;
					safeRotationOffset -= step * 0.5f;

					GeoUtils.SetUnitPositionData(
						ref CutoutProperties.UnitPositionData,
						CutoutProperties.Resolution,
						safeRotationOffset
					);
				}

				if (
					CenterType == CenterTypes.Offset ||
					CenterType == CenterTypes.Cutout
				) {
					AdjustedCenter.x += CenterOffset.x;
					AdjustedCenter.y += CenterOffset.y;
				}
			}
		}

		[System.Serializable]
		public class CutoutProperties
		{
			[MinAttribute(3)] public int Resolution = 4;
			[MinAttribute(0.0f)] public float Radius = 1.0f;
			[Range(-3.141592f, 3.141592f)] public float RotationOffset = 0.0f;

			public GeoUtils.UnitPositionData UnitPositionData = new GeoUtils.UnitPositionData();
		}

		public static void AddPolygon(
			ref VertexHelper vh,
			PolygonProperties polygonProperties,
			PointsList.PointListProperties pointListProperties,
			Vector2 positionOffset,
			Color32 color,
			Vector2 uv,
			ref PointsList.PointsData pointsData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData
		) {
			pointListProperties.SetPoints();
			PointsList.SetLineData(pointListProperties, ref pointsData);

			int numVertices = vh.currentVertCount;
			int firstOuterVertex = vh.currentVertCount + polygonProperties.CutoutProperties.Resolution - 1;

			bool usesCutout = polygonProperties.CenterType == PolygonProperties.CenterTypes.Cutout;

			if (usesCutout)
			{
				float cutoutOffsetDistance = polygonProperties.CutoutProperties.Radius - edgeGradientData.ShadowOffset;
				cutoutOffsetDistance += Mathf.LerpUnclamped(
					pointsData.PositionNormals[0].magnitude * edgeGradientData.ShadowOffset * 3.0f,
					0.0f,
					edgeGradientData.InnerScale
				);

				for (int i = 0; i < polygonProperties.CutoutProperties.Resolution; i++)
				{
					tmpPos.x =
						polygonProperties.AdjustedCenter.x +
						positionOffset.x +
						polygonProperties.CutoutProperties.UnitPositionData.UnitPositions[i].x * cutoutOffsetDistance;
					tmpPos.y =
						polygonProperties.AdjustedCenter.y +
						positionOffset.y +
						polygonProperties.CutoutProperties.UnitPositionData.UnitPositions[i].y * cutoutOffsetDistance;
					tmpPos.z = 0.0f;

					vh.AddVert(
						tmpPos,
						color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent
					);
				}
			}
			else
			{
				// add center
				tmpPos.x = polygonProperties.AdjustedCenter.x + positionOffset.x;
				tmpPos.y = polygonProperties.AdjustedCenter.y + positionOffset.y;
				tmpPos.z = 0.0f;

				vh.AddVert(
					tmpPos,
					color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent
				);
			}

			// add first position
			tmpPos.x = positionOffset.x + Mathf.LerpUnclamped(
				polygonProperties.AdjustedCenter.x,
				pointsData.Positions[0].x + pointsData.PositionNormals[0].x * edgeGradientData.ShadowOffset,
				edgeGradientData.InnerScale
			);
			tmpPos.y = positionOffset.y + Mathf.LerpUnclamped(
				polygonProperties.AdjustedCenter.y,
				pointsData.Positions[0].y + pointsData.PositionNormals[0].y * edgeGradientData.ShadowOffset,
				edgeGradientData.InnerScale
			);
			tmpPos.z = 0.0f;

			vh.AddVert(
				tmpPos,
				color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent
			);

			for (int i = 1; i < pointsData.NumPositions; i++)
			{
				tmpPos.x = positionOffset.x + Mathf.LerpUnclamped(
					polygonProperties.AdjustedCenter.x,
					pointsData.Positions[i].x + pointsData.PositionNormals[i].x * edgeGradientData.ShadowOffset,
					edgeGradientData.InnerScale
				);
				tmpPos.y = positionOffset.y + Mathf.LerpUnclamped(
					polygonProperties.AdjustedCenter.y,
					pointsData.Positions[i].y + pointsData.PositionNormals[i].y * edgeGradientData.ShadowOffset,
					edgeGradientData.InnerScale
				);

				vh.AddVert(
					tmpPos,
					color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent
				);

				if (!usesCutout)
				{
					vh.AddTriangle(numVertices, numVertices + i, numVertices + i + 1);
				}
			}

			// add cutout indices
			if (usesCutout)
			{
				for (int i = 1; i < pointsData.NumPositions; i++)
				{
					vh.AddTriangle(
						numVertices + GeoUtils.SimpleMap(i, pointsData.NumPositions, polygonProperties.CutoutProperties.Resolution),
						firstOuterVertex + i,
						firstOuterVertex + i + 1
					);
				}

				for (int i = 1; i < polygonProperties.CutoutProperties.Resolution; i++)
				{
					vh.AddTriangle(
						numVertices + i,
						numVertices + i - 1,
						firstOuterVertex + Mathf.CeilToInt(GeoUtils.SimpleMap((float)i, (float)polygonProperties.CutoutProperties.Resolution, (float)pointsData.NumPositions))
					);
				}
			}

			// add last triangle
			if (usesCutout)
			{
				vh.AddTriangle(
					numVertices,
					firstOuterVertex + pointsData.NumPositions,
					firstOuterVertex + 1
				);

				vh.AddTriangle(
					numVertices,
					firstOuterVertex,
					firstOuterVertex + pointsData.NumPositions
				);
			}
			else
			{
				vh.AddTriangle(numVertices, numVertices + pointsData.NumPositions, numVertices + 1);
			}

			if (edgeGradientData.IsActive)
			{
				color.a = 0;

				int outerFirstIndex = numVertices + pointsData.NumPositions;

				if (usesCutout)
				{
					outerFirstIndex = firstOuterVertex + pointsData.NumPositions;
				}
				else
				{
					firstOuterVertex = numVertices;
				}

				float offset = edgeGradientData.SizeAdd + edgeGradientData.ShadowOffset;

				vh.AddVert(positionOffset + pointsData.Positions[0] + pointsData.PositionNormals[0] * offset,
					color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				for (int i = 1; i < pointsData.NumPositions; i++)
				{
					vh.AddVert(positionOffset + pointsData.Positions[i] + pointsData.PositionNormals[i] * offset,
						color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					vh.AddTriangle(firstOuterVertex + i + 1, outerFirstIndex + i, outerFirstIndex + i + 1);
					vh.AddTriangle(firstOuterVertex + i + 1, outerFirstIndex + i + 1, firstOuterVertex + i + 2);
				}

				// fill last outer quad
				vh.AddTriangle(firstOuterVertex + 1, outerFirstIndex, outerFirstIndex + 1);
				vh.AddTriangle(firstOuterVertex + 2, firstOuterVertex + 1, outerFirstIndex + 1);


				if (usesCutout)
				{
					float radius = (polygonProperties.CutoutProperties.Radius - offset);
					for (int i = 0; i < polygonProperties.CutoutProperties.Resolution; i++)
					{
						tmpPos.x =
							polygonProperties.AdjustedCenter.x +
							positionOffset.x +
							polygonProperties.CutoutProperties.UnitPositionData.UnitPositions[i].x * radius;
						tmpPos.y =
							polygonProperties.AdjustedCenter.y +
							positionOffset.y +
							polygonProperties.CutoutProperties.UnitPositionData.UnitPositions[i].y * radius;
						tmpPos.z = 0.0f;

						vh.AddVert(
							tmpPos,
							color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent
						);
					}

					for (int i = 1; i < polygonProperties.CutoutProperties.Resolution; i++)
					{
						vh.AddTriangle(
							numVertices + i - 1,
							numVertices + i,
							outerFirstIndex + pointsData.NumPositions + i
						);

						vh.AddTriangle(
							numVertices + i,
							outerFirstIndex + pointsData.NumPositions + i + 1,
							outerFirstIndex + pointsData.NumPositions + i
						);
					}

					vh.AddTriangle(
						firstOuterVertex,
						numVertices,
						outerFirstIndex + pointsData.NumPositions + polygonProperties.CutoutProperties.Resolution
					);

					vh.AddTriangle(
						numVertices,
						outerFirstIndex + pointsData.NumPositions + 1,
						outerFirstIndex + pointsData.NumPositions + polygonProperties.CutoutProperties.Resolution
					);
				}
			}
		}

	}
}
