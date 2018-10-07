//#define CENTER_ROUNDED_CAPS

using UnityEngine;
using UnityEngine.UI;

using RoundingProperties = ThisOtherThing.UI.GeoUtils.RoundingProperties;

namespace ThisOtherThing.UI.ShapeUtils
{
	public class Lines
	{
		[System.Serializable]
		public class LineProperties
		{
			public enum LineCapTypes
			{
				Close,
				Projected,
				Round
			}

			public LineCapTypes LineCap = LineCapTypes.Close;

			public bool Closed = false;

			public RoundingProperties RoundedCapResolution = new RoundingProperties();

			public void OnCheck()
			{
				RoundedCapResolution.OnCheck(1);
			}
		}

		static Vector3 tmpPos = Vector3.zero;
		static Vector2 tmpPos2 = Vector2.zero;

		public static void AddLine(
			ref VertexHelper vh,
			LineProperties lineProperties,
			PointsList.PointListProperties pointListProperties,
			Vector2 positionOffset,
			UI.GeoUtils.OutlineProperties outlineProperties,
			Color32 color,
			Vector2 uv,
			ref PointsList.PointsData pointsData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData
		) {
			pointListProperties.SetPoints();
			pointsData.IsClosed = lineProperties.Closed && pointListProperties.Positions.Length > 2;

			pointsData.GenerateRoundedCaps = lineProperties.LineCap == LineProperties.LineCapTypes.Round;

			pointsData.LineWeight = outlineProperties.LineWeight;

			if (pointsData.GenerateRoundedCaps)
			{
				lineProperties.RoundedCapResolution.UpdateAdjusted(outlineProperties.HalfLineWeight, 0.0f, 2.0f);
				pointsData.RoundedCapResolution = lineProperties.RoundedCapResolution.AdjustedResolution;
			}

			if (!PointsList.SetLineData(pointListProperties, ref pointsData))
			{
				return;
			}


			// scale uv x for caps
			float uvXMin = 0.0f;
			float uvXLength = 1.0f;

			if (
				!lineProperties.Closed &&
				lineProperties.LineCap != LineProperties.LineCapTypes.Close
			) {
				float uvStartOffset = outlineProperties.LineWeight / pointsData.TotalLength;

				uvXMin = uvStartOffset * 0.5f;
				uvXLength = 1.0f - uvXMin * 2.0f;
			}

			float innerOffset = outlineProperties.GetCenterDistace() - (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;
			float outerOffset = outlineProperties.GetCenterDistace() + (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;

			float capOffsetAmount = 0.0f;

			if (!lineProperties.Closed && lineProperties.LineCap == LineProperties.LineCapTypes.Close)
			{
				capOffsetAmount = edgeGradientData.ShadowOffset * (edgeGradientData.InnerScale * 2.0f - 1.0f);
			}


			int numVertices = vh.currentVertCount;
			int startVertex = numVertices - 1;
			int baseIndex;

			uv.x = uvXMin + pointsData.NormalizedPositionDistances[0] * uvXLength;
			uv.y = 0.0f;

			{
				tmpPos.x = positionOffset.x + pointsData.Positions[0].x + pointsData.PositionNormals[0].x * innerOffset + pointsData.StartCapOffset.x * capOffsetAmount;
				tmpPos.y = positionOffset.y + pointsData.Positions[0].y + pointsData.PositionNormals[0].y * innerOffset + pointsData.StartCapOffset.y * capOffsetAmount;
			}

			vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			uv.y = 1.0f;

			{
				tmpPos.x = positionOffset.x + pointsData.Positions[0].x + pointsData.PositionNormals[0].x * outerOffset + pointsData.StartCapOffset.x * capOffsetAmount;
				tmpPos.y = positionOffset.y + pointsData.Positions[0].y + pointsData.PositionNormals[0].y * outerOffset + pointsData.StartCapOffset.y * capOffsetAmount;
			}

			vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			for (int i = 1; i < pointsData.NumPositions - 1; i++)
			{
				uv.x = uvXMin + pointsData.NormalizedPositionDistances[i] * uvXLength;
				uv.y = 0.0f;

				{
					tmpPos.x = positionOffset.x + pointsData.Positions[i].x + pointsData.PositionNormals[i].x * innerOffset;
					tmpPos.y = positionOffset.y + pointsData.Positions[i].y + pointsData.PositionNormals[i].y * innerOffset;
				}

				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				uv.y = 1.0f;

				{
					tmpPos.x = positionOffset.x + pointsData.Positions[i].x + pointsData.PositionNormals[i].x * outerOffset;
					tmpPos.y = positionOffset.y + pointsData.Positions[i].y + pointsData.PositionNormals[i].y * outerOffset;
				}

				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				baseIndex = startVertex + i * 2;
				vh.AddTriangle(baseIndex - 1, baseIndex, baseIndex + 1);
				vh.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 1);
			}

			// add end vertices
			int endIndex = pointsData.NumPositions - 1;
			uv.x = uvXMin + pointsData.NormalizedPositionDistances[endIndex] * uvXLength;
			uv.y = 0.0f;

			{
				tmpPos.x = positionOffset.x + pointsData.Positions[endIndex].x + pointsData.PositionNormals[endIndex].x * innerOffset + pointsData.EndCapOffset.x * capOffsetAmount;
				tmpPos.y = positionOffset.y + pointsData.Positions[endIndex].y + pointsData.PositionNormals[endIndex].y * innerOffset + pointsData.EndCapOffset.y * capOffsetAmount;
			}

			vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			uv.y = 1.0f;

			{
				tmpPos.x = positionOffset.x + pointsData.Positions[endIndex].x + pointsData.PositionNormals[endIndex].x * outerOffset + pointsData.EndCapOffset.x * capOffsetAmount;
				tmpPos.y = positionOffset.y + pointsData.Positions[endIndex].y + pointsData.PositionNormals[endIndex].y * outerOffset + pointsData.EndCapOffset.y * capOffsetAmount;
			}

			vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			baseIndex = startVertex + endIndex * 2;
			vh.AddTriangle(baseIndex - 1, baseIndex, baseIndex + 1);
			vh.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 1);

			if (lineProperties.Closed)
			{
				uv.x = 1.0f;
				uv.y = 0.0f;

				{
					tmpPos.x = positionOffset.x + pointsData.Positions[0].x + pointsData.PositionNormals[0].x * innerOffset + pointsData.StartCapOffset.x * capOffsetAmount;
					tmpPos.y = positionOffset.y + pointsData.Positions[0].y + pointsData.PositionNormals[0].y * innerOffset + pointsData.StartCapOffset.y * capOffsetAmount;
				}

				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				uv.y = 1.0f;

				{
					tmpPos.x = positionOffset.x + pointsData.Positions[0].x + pointsData.PositionNormals[0].x * outerOffset + pointsData.StartCapOffset.x * capOffsetAmount;
					tmpPos.y = positionOffset.y + pointsData.Positions[0].y + pointsData.PositionNormals[0].y * outerOffset + pointsData.StartCapOffset.y * capOffsetAmount;
				}

				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				baseIndex = startVertex + endIndex * 2 + 2;
				vh.AddTriangle(baseIndex - 1, baseIndex, baseIndex + 1);
				vh.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 1);
			}

			if (edgeGradientData.IsActive)
			{
				byte colorAlpha = color.a;

				innerOffset = outlineProperties.GetCenterDistace() - (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset);
				outerOffset = outlineProperties.GetCenterDistace() + (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset);

				innerOffset -= edgeGradientData.SizeAdd;
				outerOffset += edgeGradientData.SizeAdd;

				color.a = 0;

				int outerBaseIndex = numVertices + pointsData.NumPositions * 2;

				if (lineProperties.Closed)
					outerBaseIndex += 2;

				uv.x = uvXMin + pointsData.NormalizedPositionDistances[0] * uvXLength;
				uv.y = 0.0f;

				{
					tmpPos.x = positionOffset.x + pointsData.Positions[0].x + pointsData.PositionNormals[0].x * innerOffset;
					tmpPos.y = positionOffset.y + pointsData.Positions[0].y + pointsData.PositionNormals[0].y * innerOffset;
				}

				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				uv.y = 1.0f;

				{
					tmpPos.x = positionOffset.x + pointsData.Positions[0].x + pointsData.PositionNormals[0].x * outerOffset;
					tmpPos.y = positionOffset.y + pointsData.Positions[0].y + pointsData.PositionNormals[0].y * outerOffset;
				}

				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				for (int i = 1; i < pointsData.NumPositions; i++)
				{
					uv.x = uvXMin + pointsData.NormalizedPositionDistances[i] * uvXLength;
					uv.y = 0.0f;

					{
						tmpPos.x = positionOffset.x + pointsData.Positions[i].x + pointsData.PositionNormals[i].x * innerOffset;
						tmpPos.y = positionOffset.y + pointsData.Positions[i].y + pointsData.PositionNormals[i].y * innerOffset;
					}

					vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					uv.y = 1.0f;

					{
						tmpPos.x = positionOffset.x + pointsData.Positions[i].x + pointsData.PositionNormals[i].x * outerOffset;
						tmpPos.y = positionOffset.y + pointsData.Positions[i].y + pointsData.PositionNormals[i].y * outerOffset;
					}

					vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					// inner quad
					vh.AddTriangle(startVertex + i * 2 - 1, startVertex + i * 2 + 1, outerBaseIndex + i * 2);
					vh.AddTriangle(startVertex + i * 2 - 1, outerBaseIndex + i * 2, outerBaseIndex + i * 2 - 2);

					// outer quad
					vh.AddTriangle(startVertex + i * 2, outerBaseIndex + i * 2 - 1, startVertex + i * 2 + 2);
					vh.AddTriangle(startVertex + i * 2 + 2, outerBaseIndex + i * 2 - 1, outerBaseIndex + i * 2 + 1);
				}

				if (lineProperties.Closed)
				{
					int lastIndex = pointsData.NumPositions;

					uv.x = 1.0f;
					uv.y = 0.0f;

					{
						tmpPos.x = positionOffset.x + pointsData.Positions[0].x + pointsData.PositionNormals[0].x * innerOffset;
						tmpPos.y = positionOffset.y + pointsData.Positions[0].y + pointsData.PositionNormals[0].y * innerOffset;
					}

					vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					uv.y = 1.0f;

					{
						tmpPos.x = positionOffset.x + pointsData.Positions[0].x + pointsData.PositionNormals[0].x * outerOffset;
						tmpPos.y = positionOffset.y + pointsData.Positions[0].y + pointsData.PositionNormals[0].y * outerOffset;
					}

					vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					// inner quad
					vh.AddTriangle(startVertex + lastIndex * 2 - 1, startVertex + lastIndex * 2 + 1, outerBaseIndex + lastIndex * 2);
					vh.AddTriangle(startVertex + lastIndex * 2 - 1, outerBaseIndex + lastIndex * 2, outerBaseIndex + lastIndex * 2 - 2);

					// outer quad
					vh.AddTriangle(startVertex + lastIndex * 2, outerBaseIndex + lastIndex * 2 - 1, startVertex + lastIndex * 2 + 2);
					vh.AddTriangle(startVertex + lastIndex * 2 + 2, outerBaseIndex + lastIndex * 2 - 1, outerBaseIndex + lastIndex * 2 + 1);
				}

				color.a = colorAlpha;
			}

			// close line or add caps
			if (!lineProperties.Closed)
			{
				AddStartCap(
					ref vh,
					lineProperties,
					positionOffset,
					outlineProperties,
					color,
					uv,
					uvXMin,
					uvXLength,
					pointsData,
					edgeGradientData
				);
					
				AddEndCap(
					ref vh,
					lineProperties,
					positionOffset,
					outlineProperties,
					color,
					uv,
					uvXMin,
					uvXLength,
					pointsData,
					edgeGradientData
				);
			}
		}

		public static void AddStartCap(
			ref VertexHelper vh,
			LineProperties lineProperties,
			Vector2 positionOffset,
			UI.GeoUtils.OutlineProperties outlineProperties,
			Color32 color,
			Vector2 uv,
			float uvXMin,
			float uvXLength,
			PointsList.PointsData pointsData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData
		) {
			int currentVertCount = vh.currentVertCount;
			int startIndex = currentVertCount - pointsData.NumPositions * 2;

			if (edgeGradientData.IsActive)
			{
				startIndex -= pointsData.NumPositions * 2;
			}

			tmpPos2.x = positionOffset.x + pointsData.Positions[0].x;
			tmpPos2.y = positionOffset.y + pointsData.Positions[0].y;

			switch (lineProperties.LineCap)
			{
				case LineProperties.LineCapTypes.Close:
					AddCloseCap(
						ref vh,
						true,
						startIndex,
						tmpPos2,
						pointsData.PositionNormals[0],
						pointsData.StartCapOffset,
						0,
						lineProperties,
						outlineProperties,
						color,
						uv,
						pointsData,
						edgeGradientData,
						currentVertCount
					);

					break;
				case LineProperties.LineCapTypes.Projected:
					AddProjectedCap(
						ref vh,
						true,
						startIndex,
						tmpPos2,
						pointsData.PositionNormals[0],
						pointsData.StartCapOffset,
						0,
						lineProperties,
						outlineProperties,
						color,
						uv,
						pointsData,
						edgeGradientData,
						currentVertCount
					);

					break;
				case LineProperties.LineCapTypes.Round:
					AddRoundedCap(
						ref vh,
						true,
						startIndex,
						tmpPos2,
						pointsData.PositionNormals[0],
						pointsData.StartCapOffset,
						0,
						lineProperties,
						outlineProperties,
						color,
						uv,
						pointsData,
						edgeGradientData,
						pointsData.StartCapOffsets,
						pointsData.StartCapUVs,
						uvXMin,
						uvXLength,
						currentVertCount
					);
					break;
			}
		}

		public static void AddEndCap(
			ref VertexHelper vh,
			LineProperties lineProperties,
			Vector2 positionOffset,
			UI.GeoUtils.OutlineProperties outlineProperties,
			Color32 color,
			Vector2 uv,
			float uvXMin,
			float uvXLength,
			PointsList.PointsData pointsData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData
		) {
			int currentVertCount = vh.currentVertCount;
			int startIndex = currentVertCount;

			if (edgeGradientData.IsActive)
			{
				startIndex -= pointsData.NumPositions * 2;
			}

			int lastPositionIndex = pointsData.NumPositions - 1;

			tmpPos2.x = positionOffset.x + pointsData.Positions[lastPositionIndex].x;
			tmpPos2.y = positionOffset.y + pointsData.Positions[lastPositionIndex].y;

			switch (lineProperties.LineCap)
			{
				case LineProperties.LineCapTypes.Close:

					startIndex -= 4;

					AddCloseCap(
						ref vh,
						false,
						startIndex,
						tmpPos2,
						pointsData.PositionNormals[lastPositionIndex],
						pointsData.EndCapOffset,
						1,
						lineProperties,
						outlineProperties,
						color,
						uv,
						pointsData,
						edgeGradientData,
						currentVertCount
					);

					break;
				case LineProperties.LineCapTypes.Projected:

					startIndex -= 6;

					AddProjectedCap(
						ref vh,
						false,
						startIndex,
						tmpPos2,
						pointsData.PositionNormals[lastPositionIndex],
						pointsData.EndCapOffset,
						1,
						lineProperties,
						outlineProperties,
						color,
						uv,
						pointsData,
						edgeGradientData,
						currentVertCount
					);

					break;
				case LineProperties.LineCapTypes.Round:
					#if CENTER_ROUNDED_CAPS
					startIndex -= pointsData.RoundedCapResolution + 3;
					#else
					startIndex -= pointsData.RoundedCapResolution + 2;
					#endif

					if (edgeGradientData.IsActive)
					{
						startIndex -= pointsData.RoundedCapResolution;
					}

					AddRoundedCap(
						ref vh,
						false,
						startIndex,
						tmpPos2,
						pointsData.PositionNormals[lastPositionIndex],
						pointsData.EndCapOffset,
						1,
						lineProperties,
						outlineProperties,
						color,
						uv,
						pointsData,
						edgeGradientData,
						pointsData.EndCapOffsets,
						pointsData.EndCapUVs,
						uvXMin,
						uvXLength,
						currentVertCount
					);

					break;
			}
		}

		public static void AddCloseCap(
			ref VertexHelper vh,
			bool isStart,
			int firstVertIndex,
			Vector2 position,
			Vector2 normal,
			Vector2 capOffset,
			int invertIndices,
			LineProperties lineProperties,
			UI.GeoUtils.OutlineProperties outlineProperties,
			Color32 color,
			Vector2 uv,
			PointsList.PointsData pointsData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData,
			int currentVertCount
		) {
			if (edgeGradientData.IsActive)
			{
				int baseIndex = currentVertCount;

				float innerOffset = outlineProperties.GetCenterDistace() - (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset) - edgeGradientData.SizeAdd;
				float outerOffset = outlineProperties.GetCenterDistace() + (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset) + edgeGradientData.SizeAdd;

				float capOffsetAmount = edgeGradientData.SizeAdd + edgeGradientData.ShadowOffset;

				color.a = 0;

				uv.y = 0.0f;

				{
					tmpPos.x = position.x + normal.x * innerOffset + capOffset.x * capOffsetAmount;
					tmpPos.y = position.y + normal.y * innerOffset + capOffset.y * capOffsetAmount;
				}

				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				uv.y = 1.0f;

				{
					tmpPos.x = position.x + normal.x * outerOffset + capOffset.x * capOffsetAmount;
					tmpPos.y = position.y + normal.y * outerOffset + capOffset.y * capOffsetAmount;
				}

				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				vh.AddTriangle(firstVertIndex, baseIndex + invertIndices, baseIndex + 1 - invertIndices);
				vh.AddTriangle(firstVertIndex + invertIndices, baseIndex + 1, firstVertIndex + 1 - invertIndices);

				int antiAliasedIndex = firstVertIndex + pointsData.NumPositions * 2;

				if (invertIndices != 0)
				{
					vh.AddTriangle(firstVertIndex, baseIndex, antiAliasedIndex);
					vh.AddTriangle(firstVertIndex + 1, antiAliasedIndex + 1, baseIndex + 1);
				}
				else
				{
					vh.AddTriangle(firstVertIndex, antiAliasedIndex, baseIndex);
					vh.AddTriangle(firstVertIndex + 1, baseIndex + 1, antiAliasedIndex + 1);
				}
			}
		}

		public static void AddProjectedCap(
			ref VertexHelper vh,
			bool isStart,
			int firstVertIndex,
			Vector2 position,
			Vector2 normal,
			Vector2 capOffset,
			int invertIndices,
			LineProperties lineProperties,
			UI.GeoUtils.OutlineProperties outlineProperties,
			Color32 color,
			Vector2 uv,
			PointsList.PointsData pointsData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData,
			int currentVertCount
		) {
			int baseIndex = currentVertCount;

			if (isStart)
			{
				uv.x = 0.0f;
			}
			else
			{
				uv.x = 1.0f;
			}

			float innerOffset = outlineProperties.GetCenterDistace() - (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;
			float outerOffset = outlineProperties.GetCenterDistace() + (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;

			float capOffsetAmount = edgeGradientData.ShadowOffset + outlineProperties.LineWeight * 0.5f;
			capOffsetAmount *= edgeGradientData.InnerScale;

			// add lineWeight to position
			{
				tmpPos.x = position.x + normal.x * innerOffset + capOffset.x * capOffsetAmount;
				tmpPos.y = position.y + normal.y * innerOffset + capOffset.y * capOffsetAmount;
			}

			uv.y = 0.0f;
			vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			{
				tmpPos.x = position.x + normal.x * outerOffset + capOffset.x * capOffsetAmount;
				tmpPos.y = position.y + normal.y * outerOffset + capOffset.y * capOffsetAmount;
			}

			uv.y = 1.0f;
			vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			vh.AddTriangle(firstVertIndex, baseIndex + invertIndices, baseIndex + 1 - invertIndices);
			vh.AddTriangle(firstVertIndex + invertIndices, baseIndex + 1, firstVertIndex + 1 - invertIndices);

			if (edgeGradientData.IsActive)
			{
				innerOffset = outlineProperties.GetCenterDistace() - (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset) - edgeGradientData.SizeAdd;
				outerOffset = outlineProperties.GetCenterDistace() + (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset) + edgeGradientData.SizeAdd;

				capOffsetAmount = outlineProperties.HalfLineWeight + edgeGradientData.SizeAdd + edgeGradientData.ShadowOffset;

				color.a = 0;

				{
					tmpPos.x = position.x + normal.x * innerOffset + capOffset.x * capOffsetAmount;
					tmpPos.y = position.y + normal.y * innerOffset + capOffset.y * capOffsetAmount;
				}

				uv.y = 0.0f;
				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);


				{
					tmpPos.x = position.x + normal.x * outerOffset + capOffset.x * capOffsetAmount;
					tmpPos.y = position.y + normal.y * outerOffset + capOffset.y * capOffsetAmount;
				}

				uv.y = 1.0f;
				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				int antiAliasedIndex = firstVertIndex + pointsData.NumPositions * 2;
				baseIndex += 2;

				if (invertIndices != 0)
				{
					vh.AddTriangle(firstVertIndex, baseIndex, antiAliasedIndex);
					vh.AddTriangle(firstVertIndex + 1, antiAliasedIndex + 1, baseIndex + 1);

					vh.AddTriangle(baseIndex-2, baseIndex-1, baseIndex);
					vh.AddTriangle(baseIndex + 1, baseIndex, baseIndex-1);

					vh.AddTriangle(firstVertIndex, baseIndex - 2, baseIndex);
					vh.AddTriangle(firstVertIndex + 1, baseIndex + 1, baseIndex - 1);
				}
				else
				{
					vh.AddTriangle(firstVertIndex, antiAliasedIndex, baseIndex);
					vh.AddTriangle(firstVertIndex + 1, baseIndex + 1, antiAliasedIndex + 1);

					vh.AddTriangle(baseIndex-2, baseIndex, baseIndex-1);
					vh.AddTriangle(baseIndex + 1, baseIndex-1, baseIndex);

					vh.AddTriangle(firstVertIndex, baseIndex, baseIndex - 2);
					vh.AddTriangle(firstVertIndex + 1, baseIndex - 1, baseIndex + 1);
				}
			}
		}

		public static void AddRoundedCap(
			ref VertexHelper vh,
			bool isStart,
			int firstVertIndex,
			Vector2 position,
			Vector2 normal,
			Vector2 capOffset,
			int invertIndices,
			LineProperties lineProperties,
			UI.GeoUtils.OutlineProperties outlineProperties,
			Color32 color,
			Vector2 uv,
			PointsList.PointsData pointsData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData,
			Vector2[] capOffsets,
			Vector2[] uvOffsets,
			float uvXMin,
			float uvXLength,
			int currentVertCount
		) {
			int baseIndex = currentVertCount;

			float innerOffset = outlineProperties.GetCenterDistace();
			float capOffsetAmount = (edgeGradientData.ShadowOffset + outlineProperties.HalfLineWeight) * edgeGradientData.InnerScale;

			if (isStart)
			{
				uv.x = uvXMin;
			}
			else
			{
				uv.x = uvXMin + uvXLength;
			}

			#if CENTER_ROUNDED_CAPS
			// add center vert
			tmpPos.x = position.x;
			tmpPos.y = position.y;
			uv.y = 0.5f;

			vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);
			#endif

			for (int i = 0; i < capOffsets.Length; i++)
			{
				{
					tmpPos.x = position.x + normal.x * innerOffset + capOffsets[i].x * capOffsetAmount;
					tmpPos.y = position.y + normal.y * innerOffset + capOffsets[i].y * capOffsetAmount;
				}

				if (isStart)
				{
					uv.x = Mathf.LerpUnclamped(uvXMin, 0.0f, uvOffsets[i].x);
				}
				else
				{
					uv.x = Mathf.LerpUnclamped(uvXMin + uvXLength, 1.0f, uvOffsets[i].x);
				}
				uv.y = uvOffsets[i].y;

				vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				if (i > 0)
				{
					#if CENTER_ROUNDED_CAPS
					vh.AddTriangle(baseIndex, baseIndex + i - 1, baseIndex + i);
					#else
					vh.AddTriangle(firstVertIndex, baseIndex + i - 1, baseIndex + i);
					#endif
				}
			}

			// last fans
			if (isStart)
			{
				#if CENTER_ROUNDED_CAPS
				// starting triangle
				vh.AddTriangle(baseIndex + 1, baseIndex, firstVertIndex);
				
				// end triangles
				vh.AddTriangle(baseIndex, baseIndex + capOffsets.Length - 1, baseIndex + capOffsets.Length);
				vh.AddTriangle(baseIndex, baseIndex + capOffsets.Length, firstVertIndex + 1);
				#else
				vh.AddTriangle(baseIndex + capOffsets.Length - 1, firstVertIndex + 1, firstVertIndex);
				#endif
			}
			else
			{
				#if CENTER_ROUNDED_CAPS
				// starting triangle
				vh.AddTriangle(baseIndex + 1, baseIndex, firstVertIndex + 1);

				// end triangles
				vh.AddTriangle(baseIndex, baseIndex + capOffsets.Length - 1, baseIndex + capOffsets.Length);
				vh.AddTriangle(baseIndex, baseIndex + capOffsets.Length, firstVertIndex);
				#else
				vh.AddTriangle(baseIndex, firstVertIndex, firstVertIndex + 1);
				#endif
			}

			if (edgeGradientData.IsActive)
			{
				color.a = 0;

				innerOffset = outlineProperties.GetCenterDistace();

				capOffsetAmount = outlineProperties.HalfLineWeight + edgeGradientData.SizeAdd + edgeGradientData.ShadowOffset;

				int antiAliasedIndex = firstVertIndex + pointsData.NumPositions * 2;

				for (int i = 0; i < capOffsets.Length; i++)
				{
					{
						tmpPos.x = position.x + normal.x * innerOffset + capOffsets[i].x * capOffsetAmount;
						tmpPos.y = position.y + normal.y * innerOffset + capOffsets[i].y * capOffsetAmount;
					}

					if (isStart)
					{
						uv.x = Mathf.LerpUnclamped(uvXMin, 0.0f, uvOffsets[i].x);
					}
					else
					{
						uv.x = Mathf.LerpUnclamped(uvXMin + uvXLength, 1.0f, uvOffsets[i].x);
					}
					uv.y = uvOffsets[i].y;

					vh.AddVert(tmpPos, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					if (i > 0)
					{
						vh.AddTriangle(baseIndex + i - 1, baseIndex + capOffsets.Length + i - 1, baseIndex + i);
						vh.AddTriangle(baseIndex + capOffsets.Length + i, baseIndex + i, baseIndex + capOffsets.Length + i - 1);
					}
				}

				if (!isStart)
				{
					vh.AddTriangle(baseIndex, firstVertIndex + 1, antiAliasedIndex + 1);
					vh.AddTriangle(antiAliasedIndex + 1, baseIndex + capOffsets.Length, baseIndex);

					vh.AddTriangle(baseIndex + capOffsets.Length * 2 - 1, antiAliasedIndex, firstVertIndex);
					vh.AddTriangle(baseIndex + capOffsets.Length - 1, baseIndex + capOffsets.Length * 2 - 1, firstVertIndex);
				}
				else
				{
					vh.AddTriangle(firstVertIndex + 1, baseIndex + capOffsets.Length - 1, baseIndex + capOffsets.Length * 2 - 1);
					vh.AddTriangle(antiAliasedIndex + 1, firstVertIndex + 1, baseIndex + capOffsets.Length * 2 - 1);

					vh.AddTriangle(antiAliasedIndex, baseIndex, firstVertIndex);
					vh.AddTriangle(baseIndex + capOffsets.Length, baseIndex, antiAliasedIndex);
				}
			}
		}

	}
}
