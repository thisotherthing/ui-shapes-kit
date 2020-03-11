using UnityEngine;
using UnityEngine.UI;

namespace ThisOtherThing.UI.ShapeUtils
{
	public class Arcs
	{
		static Vector3 tmpPosition = Vector3.zero;

		static Vector2 tmpInnerRadius = Vector2.one;
		static Vector2 tmpOuterRadius = Vector2.one;

		static Vector2 tmpArcInnerRadius = Vector2.one;
		static Vector2 tmpArcOuterRadius = Vector2.one;

		static Vector3 tmpOffsetCenter = Vector3.one;

		[System.Serializable]
		public class ArcProperties
		{
			public enum ArcDirection
			{
				Backward,
				Centered,
				Forward
			}

			public ArcDirection Direction = ArcDirection.Forward;
			[Range(0.0f, 1.0f)] public float Length = 1.0f;

			public int AdjustedResolution { get; private set; }
			public float AdjustedBaseAngle { get; private set; }
			public float AdjustedDirection { get; private set; }
			public float SegmentAngle { get; private set; }

			public float EndSegmentAngle { get; private set; }

			Vector3 endSegmentUnitPosition = Vector3.zero;
			public Vector3 EndSegmentUnitPosition { get { return endSegmentUnitPosition; } }

			Vector3 startTangent = Vector3.zero;
			public Vector3 StartTangent { get { return startTangent; } }

			Vector3 endTangent = Vector3.zero;
			public Vector3 EndTangent { get { return endTangent; } }

			Vector3 centerNormal = Vector3.zero;
			public Vector3 CenterNormal { get { return centerNormal; } }


			public void UpdateAdjusted(
				int FullCircleResolution,
				float BaseAngle
			) {
				switch (Direction)
				{
					case ArcDirection.Backward:
						AdjustedDirection = -1.0f;
						break;
					case ArcDirection.Centered:
						AdjustedDirection = 1.0f;
						BaseAngle -= Length;
						break;
					case ArcDirection.Forward:
						AdjustedDirection = 1.0f;
						break;
					default:
						throw new System.ArgumentOutOfRangeException();
				}

				AdjustedResolution = Mathf.CeilToInt((float)FullCircleResolution * Length);
				AdjustedBaseAngle = BaseAngle * Mathf.PI;

				SegmentAngle = (Mathf.PI * 2.0f) / (float)AdjustedResolution;

				EndSegmentAngle = AdjustedBaseAngle + ((Mathf.PI * 2.0f) * Length) * AdjustedDirection;

				endSegmentUnitPosition.x = Mathf.Sin(EndSegmentAngle);
				endSegmentUnitPosition.y = Mathf.Cos(EndSegmentAngle);

				endTangent.x = endSegmentUnitPosition.y * AdjustedDirection;
				endTangent.y = endSegmentUnitPosition.x * -AdjustedDirection;

				startTangent.x = Mathf.Cos(AdjustedBaseAngle) * -AdjustedDirection;
				startTangent.y = Mathf.Sin(AdjustedBaseAngle) * AdjustedDirection;

				float centerAngle = AdjustedBaseAngle + (Mathf.PI * Length) * AdjustedDirection;
				float lengthScaler = 1.0f / Mathf.Sin(Mathf.PI * Length);
				lengthScaler = Mathf.Min(4.0f, lengthScaler);
				centerNormal.x = -Mathf.Sin(centerAngle) * lengthScaler;
				centerNormal.y = -Mathf.Cos(centerAngle) * lengthScaler;
			}
		}

		public static void AddSegment(
			ref VertexHelper vh,
			Vector2 center,
			Vector2 radius,
			ShapeUtils.Ellipses.EllipseProperties circleProperties,
			ArcProperties arcProperties,
			Color32 color,
			Vector2 uv,
			ref UI.GeoUtils.UnitPositionData unitPositionData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData
		) {
			if (arcProperties.Length <= 0.0f)
			{
				return;
			}

			bool isReversed = arcProperties.Direction == ArcProperties.ArcDirection.Backward;

			int reversedZeroForwardMinus = isReversed ? 0 : -1;
			int reversedMinusForwardZero = isReversed ? -1 : 0;

			int reversed1Forward2 = isReversed ? 1 : 2;
			int reversed2Forward1 = isReversed ? 2 : 1;

			int reversed1Forward0 = isReversed ? 1 : 0;
			int reversed0Forward1 = isReversed ? 0 : 1;

			UI.GeoUtils.SetUnitPositionData(
				ref unitPositionData, 
				circleProperties.AdjustedResolution, 
				arcProperties.AdjustedBaseAngle,
				arcProperties.AdjustedDirection
			);

			int numVertices = vh.currentVertCount;

			tmpOuterRadius.x = (radius.x + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;
			tmpOuterRadius.y = (radius.y + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;

			float capsExtensionLength = edgeGradientData.ShadowOffset * edgeGradientData.InnerScale;

			tmpOffsetCenter.x = center.x + arcProperties.CenterNormal.x * radius.x * (edgeGradientData.InnerScale - 1.0f) * 0.2f;
			tmpOffsetCenter.y = center.y + arcProperties.CenterNormal.y * radius.y * (edgeGradientData.InnerScale - 1.0f) * 0.2f;
			tmpOffsetCenter.z = 0.0f;

			if (arcProperties.Length >= 1.0f)
			{
				capsExtensionLength = 0.0f;
				tmpOffsetCenter = center;
			}

			vh.AddVert(
				tmpOffsetCenter + arcProperties.CenterNormal * capsExtensionLength ,
				color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			tmpPosition.x = tmpOffsetCenter.x + unitPositionData.UnitPositions[0].x * tmpOuterRadius.x + arcProperties.StartTangent.x * capsExtensionLength;
			tmpPosition.y = tmpOffsetCenter.y + unitPositionData.UnitPositions[0].y * tmpOuterRadius.y + arcProperties.StartTangent.y * capsExtensionLength;
			tmpPosition.z = tmpOffsetCenter.z;
			vh.AddVert(tmpPosition, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			for (int i = 1; i < arcProperties.AdjustedResolution; i++)
			{
				tmpPosition.x = tmpOffsetCenter.x + unitPositionData.UnitPositions[i].x * tmpOuterRadius.x;
				tmpPosition.y = tmpOffsetCenter.y + unitPositionData.UnitPositions[i].y * tmpOuterRadius.y;
				tmpPosition.z = tmpOffsetCenter.z;
				vh.AddVert(tmpPosition, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				vh.AddTriangle(numVertices, numVertices + i + reversedZeroForwardMinus, numVertices + i + reversedMinusForwardZero);
			}

			int lastFullIndex = numVertices + arcProperties.AdjustedResolution;

			// add last partial segment
			tmpPosition.x = tmpOffsetCenter.x + arcProperties.EndSegmentUnitPosition.x * tmpOuterRadius.x + arcProperties.EndTangent.x * capsExtensionLength;
			tmpPosition.y = tmpOffsetCenter.y + arcProperties.EndSegmentUnitPosition.y * tmpOuterRadius.y + arcProperties.EndTangent.y * capsExtensionLength;
			tmpPosition.z = tmpOffsetCenter.z + + arcProperties.EndTangent.z * capsExtensionLength;
			vh.AddVert(tmpPosition, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			if (isReversed)
			{
				vh.AddTriangle(numVertices, lastFullIndex, lastFullIndex - 1);
				vh.AddTriangle(numVertices, lastFullIndex + 1, lastFullIndex);
			}
			else
			{
				vh.AddTriangle(numVertices, lastFullIndex - 1, lastFullIndex);
				vh.AddTriangle(numVertices, lastFullIndex, lastFullIndex + 1);
			}
				

			if (edgeGradientData.IsActive)
			{
				radius.x += edgeGradientData.SizeAdd + edgeGradientData.ShadowOffset;
				radius.y += edgeGradientData.SizeAdd + edgeGradientData.ShadowOffset;
				color.a = 0;

				tmpPosition.x = center.x + unitPositionData.UnitPositions[0].x * radius.x;
				tmpPosition.y = center.y + unitPositionData.UnitPositions[0].y * radius.y;
				tmpPosition.z = 0.0f;
				vh.AddVert(tmpPosition, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);
				int innerBase, outerBase;
				for (int i = 1; i <= arcProperties.AdjustedResolution; i++)
				{
					if (i < arcProperties.AdjustedResolution)
					{
						tmpPosition.x = center.x + unitPositionData.UnitPositions[i].x * radius.x;
						tmpPosition.y = center.y + unitPositionData.UnitPositions[i].y * radius.y;
						tmpPosition.z = 0.0f;
						vh.AddVert(tmpPosition, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);
					}
					else
					{
						tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * radius.x;
						tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * radius.y;
						tmpPosition.z = 0.0f;
						vh.AddVert(tmpPosition, color, uv, UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);
					}

					innerBase = numVertices + i;
					outerBase = innerBase + arcProperties.AdjustedResolution;

					vh.AddTriangle(outerBase + 2, innerBase + reversed0Forward1, innerBase + reversed1Forward0);
					vh.AddTriangle(innerBase, outerBase + reversed2Forward1, outerBase + reversed1Forward2);
				}

				if (arcProperties.Length >= 1.0f)
				{
					return;
				}

				tmpOuterRadius.x = edgeGradientData.SizeAdd + edgeGradientData.ShadowOffset;
				tmpOuterRadius.y = tmpOuterRadius.x;

				// add start outer vertex
				tmpPosition.x = center.x + unitPositionData.UnitPositions[0].x * radius.x + arcProperties.StartTangent.x * tmpOuterRadius.x;
				tmpPosition.y = center.y + unitPositionData.UnitPositions[0].y * radius.y + arcProperties.StartTangent.y * tmpOuterRadius.y;
				tmpPosition.z = 0.0f;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				// add end outer vertex
				tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * radius.x + arcProperties.EndTangent.x * tmpOuterRadius.x;
				tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * radius.y + arcProperties.EndTangent.y * tmpOuterRadius.y;
				tmpPosition.z = 0.0f;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				radius.x -= edgeGradientData.SizeAdd;
				radius.y -= edgeGradientData.SizeAdd;

				// add start inner vertex
				tmpPosition.x = center.x + unitPositionData.UnitPositions[0].x * radius.x + arcProperties.StartTangent.x * tmpOuterRadius.x;
				tmpPosition.y = center.y + unitPositionData.UnitPositions[0].y * radius.y + arcProperties.StartTangent.y * tmpOuterRadius.y;
				tmpPosition.z = 0.0f;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				// add end inner vertex
				tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * radius.x + arcProperties.EndTangent.x * tmpOuterRadius.x;
				tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * radius.y + arcProperties.EndTangent.y * tmpOuterRadius.y;
				tmpPosition.z = 0.0f;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				// add center extruded vertex
				tmpPosition.x = center.x + arcProperties.CenterNormal.x * tmpOuterRadius.x;
				tmpPosition.y = center.y + arcProperties.CenterNormal.y * tmpOuterRadius.y;
				tmpPosition.z = 0.0f;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				int baseCornersIndex = vh.currentVertCount - 5;
				int baseOuterIndex = numVertices + arcProperties.AdjustedResolution;
				int secondOuterIndex = numVertices + arcProperties.AdjustedResolution * 2;



				if (isReversed)
				{
					// start corner
					vh.AddTriangle(baseCornersIndex, baseCornersIndex + 2, numVertices + 1);
					vh.AddTriangle(baseCornersIndex, numVertices + 1, baseOuterIndex + 2);

					// end corner
					vh.AddTriangle(baseCornersIndex + 1, baseOuterIndex + 1, baseCornersIndex + 3);
					vh.AddTriangle(baseCornersIndex + 1, secondOuterIndex + 2, baseOuterIndex + 1);

					// start corner to center
					vh.AddTriangle(baseCornersIndex + 2, numVertices, numVertices + 1);
					vh.AddTriangle(baseCornersIndex + 2, baseCornersIndex + 4, numVertices);

					// end corner to center
					vh.AddTriangle(baseCornersIndex + 3, baseOuterIndex + 1, baseCornersIndex + 4);
					vh.AddTriangle(baseCornersIndex + 4, baseOuterIndex + 1, numVertices);
				}
				else
				{
					// start corner
					vh.AddTriangle(baseCornersIndex, numVertices + 1, baseCornersIndex + 2);
					vh.AddTriangle(baseCornersIndex, baseOuterIndex + 2, numVertices + 1);

					// end corner
					vh.AddTriangle(baseCornersIndex + 1, baseCornersIndex + 3, baseOuterIndex + 1);
					vh.AddTriangle(baseCornersIndex + 1, baseOuterIndex + 1, secondOuterIndex + 2);

					// start corner to center
					vh.AddTriangle(baseCornersIndex + 2, numVertices + 1, numVertices);
					vh.AddTriangle(baseCornersIndex + 2, numVertices, baseCornersIndex + 4);

					// end corner to center
					vh.AddTriangle(baseCornersIndex + 3, baseCornersIndex + 4, baseOuterIndex + 1);
					vh.AddTriangle(baseCornersIndex + 4, numVertices, baseOuterIndex + 1);
				}
			}
		}

		static Vector3 noOverlapInnerOffset = Vector3.zero;
		static Vector3 noOverlapOuterOffset = Vector3.zero;

		public static void AddArcRing(
			ref VertexHelper vh,
			Vector2 center,
			Vector2 radius,
			ShapeUtils.Ellipses.EllipseProperties ellipseProperties,
			ArcProperties arcProperties,
			UI.GeoUtils.OutlineProperties outlineProperties,
			Color32 color,
			Vector2 uv,
			ref UI.GeoUtils.UnitPositionData unitPositionData,
			ThisOtherThing.UI.GeoUtils.EdgeGradientData edgeGradientData
		) {
			if (arcProperties.Length <= 0.0f)
			{
				return;
			}

			UI.GeoUtils.SetUnitPositionData(
				ref unitPositionData, 
				ellipseProperties.AdjustedResolution,
				arcProperties.AdjustedBaseAngle,
				arcProperties.AdjustedDirection
			);

			radius.x += outlineProperties.GetCenterDistace();
			radius.y += outlineProperties.GetCenterDistace();

			float halfLineWeightOffset = (outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset) * edgeGradientData.InnerScale;

			if (arcProperties.Direction == ArcProperties.ArcDirection.Backward)
			{
				tmpInnerRadius.x = radius.x + halfLineWeightOffset;
				tmpInnerRadius.y = radius.y + halfLineWeightOffset;

				tmpOuterRadius.x = radius.x - halfLineWeightOffset;
				tmpOuterRadius.y = radius.y - halfLineWeightOffset;
			}
			else
			{
				tmpInnerRadius.x = radius.x - halfLineWeightOffset;
				tmpInnerRadius.y = radius.y - halfLineWeightOffset;

				tmpOuterRadius.x = radius.x + halfLineWeightOffset;
				tmpOuterRadius.y = radius.y + halfLineWeightOffset;
			}

			float capsExtensionLength = edgeGradientData.ShadowOffset * edgeGradientData.InnerScale;

			if (arcProperties.Length >= 1.0f)
			{
				capsExtensionLength = 0.0f;
			}

			int numVertices = vh.currentVertCount;
			int startVertex = numVertices - 1;

			int baseIndex;
			tmpPosition.x = center.x + unitPositionData.UnitPositions[0].x * tmpInnerRadius.x + arcProperties.StartTangent.x * capsExtensionLength;
			tmpPosition.y = center.y + unitPositionData.UnitPositions[0].y * tmpInnerRadius.y + arcProperties.StartTangent.y * capsExtensionLength;
			tmpPosition.z = 0.0f;
			vh.AddVert(
				tmpPosition, color, uv,
				UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			tmpPosition.x = center.x + unitPositionData.UnitPositions[0].x * tmpOuterRadius.x + arcProperties.StartTangent.x * capsExtensionLength;
			tmpPosition.y = center.y + unitPositionData.UnitPositions[0].y * tmpOuterRadius.y + arcProperties.StartTangent.y * capsExtensionLength;
			tmpPosition.z = 0.0f;
			vh.AddVert(
				tmpPosition, color, uv,
				UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			for (int i = 1; i < arcProperties.AdjustedResolution; i++)
			{
				tmpPosition.x = center.x + unitPositionData.UnitPositions[i].x * tmpInnerRadius.x;
				tmpPosition.y = center.y + unitPositionData.UnitPositions[i].y * tmpInnerRadius.y;
				tmpPosition.z = 0.0f;
				vh.AddVert(
					tmpPosition, color, uv,
						UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				tmpPosition.x = center.x + unitPositionData.UnitPositions[i].x * tmpOuterRadius.x;
				tmpPosition.y = center.y + unitPositionData.UnitPositions[i].y * tmpOuterRadius.y;
				tmpPosition.z = 0.0f;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);
			
				baseIndex = startVertex + i * 2;
				vh.AddTriangle(baseIndex - 1, baseIndex, baseIndex + 1);
				vh.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 1);
			}

			// add last partial segment
			tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * tmpInnerRadius.x + arcProperties.EndTangent.x * capsExtensionLength;
			tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * tmpInnerRadius.y + arcProperties.EndTangent.y * capsExtensionLength;
			tmpPosition.z = 0.0f;
			vh.AddVert(
				tmpPosition, color, uv,
				UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * tmpOuterRadius.x + arcProperties.EndTangent.x * capsExtensionLength;
			tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * tmpOuterRadius.y + arcProperties.EndTangent.y * capsExtensionLength;
			tmpPosition.z = 0.0f;
			vh.AddVert(
				tmpPosition, color, uv,
				UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

			baseIndex = startVertex + arcProperties.AdjustedResolution * 2;
			vh.AddTriangle(baseIndex - 1, baseIndex, baseIndex + 1);
			vh.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 1);

			if (edgeGradientData.IsActive)
			{
				halfLineWeightOffset = outlineProperties.HalfLineWeight + edgeGradientData.ShadowOffset + edgeGradientData.SizeAdd;

				if (arcProperties.Direction == ArcProperties.ArcDirection.Backward)
				{
					tmpOuterRadius.x = radius.x - halfLineWeightOffset;
					tmpOuterRadius.y = radius.y - halfLineWeightOffset;

					tmpInnerRadius.x = radius.x + halfLineWeightOffset;
					tmpInnerRadius.y = radius.y + halfLineWeightOffset;
				}
				else
				{
					tmpOuterRadius.x = radius.x + halfLineWeightOffset;
					tmpOuterRadius.y = radius.y + halfLineWeightOffset;

					tmpInnerRadius.x = radius.x - halfLineWeightOffset;
					tmpInnerRadius.y = radius.y - halfLineWeightOffset;
				}

				color.a = 0;

				int edgesBaseIndex;
				int innerBaseIndex;

				// ensure inner vertices don't overlap
				tmpArcInnerRadius.x = Mathf.Max(0.0f, tmpInnerRadius.x);
				tmpArcInnerRadius.y = Mathf.Max(0.0f, tmpInnerRadius.y);

				tmpArcOuterRadius.x = Mathf.Max(0.0f, tmpOuterRadius.x);
				tmpArcOuterRadius.y = Mathf.Max(0.0f, tmpOuterRadius.y);

				noOverlapInnerOffset.x = arcProperties.CenterNormal.x * -Mathf.Min(0.0f, tmpInnerRadius.x);
				noOverlapInnerOffset.y = arcProperties.CenterNormal.y * -Mathf.Min(0.0f, tmpInnerRadius.y);
				noOverlapInnerOffset.z = 0.0f;

				noOverlapOuterOffset.x = arcProperties.CenterNormal.x * -Mathf.Min(0.0f, tmpOuterRadius.x);
				noOverlapOuterOffset.y = arcProperties.CenterNormal.y * -Mathf.Min(0.0f, tmpOuterRadius.y);
				noOverlapOuterOffset.z = 0.0f;

				if (arcProperties.Length >= 1.0f)
				{
					noOverlapInnerOffset.x = 0.0f;
					noOverlapInnerOffset.y = 0.0f;
					noOverlapInnerOffset.z = 0.0f;

					noOverlapOuterOffset.x = 0.0f;
					noOverlapOuterOffset.y = 0.0f;
					noOverlapOuterOffset.z = 0.0f;
				}

				for (int i = 0; i < arcProperties.AdjustedResolution; i++)
				{
					tmpPosition.x = center.x + unitPositionData.UnitPositions[i].x * tmpArcInnerRadius.x + noOverlapInnerOffset.x;
					tmpPosition.y = center.y + unitPositionData.UnitPositions[i].y * tmpArcInnerRadius.y + noOverlapInnerOffset.y;
					tmpPosition.z = noOverlapInnerOffset.z;
					vh.AddVert(
						tmpPosition, color, uv,
						UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

					tmpPosition.x = center.x + unitPositionData.UnitPositions[i].x * tmpArcOuterRadius.x + noOverlapOuterOffset.x;
					tmpPosition.y = center.y + unitPositionData.UnitPositions[i].y * tmpArcOuterRadius.y + noOverlapOuterOffset.y;
					tmpPosition.z = noOverlapOuterOffset.z;
					vh.AddVert(
						tmpPosition, color, uv,
						UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

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

				// add partial segment antiAliasing
				tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * tmpArcInnerRadius.x + noOverlapInnerOffset.x;
				tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * tmpArcInnerRadius.y + noOverlapInnerOffset.y;
				tmpPosition.z = noOverlapInnerOffset.z;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * tmpArcOuterRadius.x + noOverlapOuterOffset.x;
				tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * tmpArcOuterRadius.y + noOverlapOuterOffset.y;
				tmpPosition.z = noOverlapOuterOffset.z;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				edgesBaseIndex = baseIndex + arcProperties.AdjustedResolution * 2;
				innerBaseIndex = startVertex + arcProperties.AdjustedResolution * 2;

				// inner quad
				vh.AddTriangle(innerBaseIndex - 1, innerBaseIndex + 1, edgesBaseIndex + 3);
				vh.AddTriangle(edgesBaseIndex + 1, innerBaseIndex - 1, edgesBaseIndex + 3);

				// outer quad
				vh.AddTriangle(innerBaseIndex, edgesBaseIndex + 2, innerBaseIndex + 2);
				vh.AddTriangle(edgesBaseIndex + 2, edgesBaseIndex + 4, innerBaseIndex + 2);

				// skip end antiAliasing if full ring is being generated
				if (arcProperties.Length >= 1.0f)
				{
					return;
				}

				capsExtensionLength = edgeGradientData.SizeAdd + edgeGradientData.ShadowOffset;

				// add start outer antiAliasing
				tmpPosition.x = center.x + unitPositionData.UnitPositions[0].x * tmpInnerRadius.x + arcProperties.StartTangent.x * capsExtensionLength;
				tmpPosition.y = center.y + unitPositionData.UnitPositions[0].y * tmpInnerRadius.y + arcProperties.StartTangent.y * capsExtensionLength;
				tmpPosition.z = arcProperties.StartTangent.z * capsExtensionLength;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);


				tmpPosition.x = center.x + unitPositionData.UnitPositions[0].x * tmpOuterRadius.x + arcProperties.StartTangent.x * capsExtensionLength;
				tmpPosition.y = center.y + unitPositionData.UnitPositions[0].y * tmpOuterRadius.y + arcProperties.StartTangent.y * capsExtensionLength;
				tmpPosition.z = 0.0f;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				// add end outer antiAliasing
				tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * tmpInnerRadius.x + arcProperties.EndTangent.x * capsExtensionLength;
				tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * tmpInnerRadius.y + arcProperties.EndTangent.y * capsExtensionLength;
				tmpPosition.z = arcProperties.EndTangent.z * capsExtensionLength;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * tmpOuterRadius.x + arcProperties.EndTangent.x * capsExtensionLength;
				tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * tmpOuterRadius.y + arcProperties.EndTangent.y * capsExtensionLength;
				tmpPosition.z = arcProperties.EndTangent.z * capsExtensionLength;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				if (arcProperties.Direction == ArcProperties.ArcDirection.Backward)
				{
					tmpOuterRadius.x += edgeGradientData.SizeAdd;
					tmpOuterRadius.y += edgeGradientData.SizeAdd;

					tmpInnerRadius.x -= edgeGradientData.SizeAdd;
					tmpInnerRadius.y -= edgeGradientData.SizeAdd;
				}
				else
				{
					tmpOuterRadius.x -= edgeGradientData.SizeAdd;
					tmpOuterRadius.y -= edgeGradientData.SizeAdd;

					tmpInnerRadius.x += edgeGradientData.SizeAdd;
					tmpInnerRadius.y += edgeGradientData.SizeAdd;
				}

				// add start inner antiAliasing
				tmpPosition.x = center.x + unitPositionData.UnitPositions[0].x * tmpInnerRadius.x + arcProperties.StartTangent.x * capsExtensionLength;
				tmpPosition.y = center.y + unitPositionData.UnitPositions[0].y * tmpInnerRadius.y + arcProperties.StartTangent.y * capsExtensionLength;
				tmpPosition.z = arcProperties.StartTangent.z * capsExtensionLength;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				tmpPosition.x = center.x + unitPositionData.UnitPositions[0].x * tmpOuterRadius.x + arcProperties.StartTangent.x * capsExtensionLength;
				tmpPosition.y = center.y + unitPositionData.UnitPositions[0].y * tmpOuterRadius.y + arcProperties.StartTangent.y * capsExtensionLength;
				tmpPosition.z = arcProperties.StartTangent.z * capsExtensionLength;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				// add end inner antiAliasing
				tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * tmpInnerRadius.x + arcProperties.EndTangent.x * capsExtensionLength;
				tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * tmpInnerRadius.y + arcProperties.EndTangent.y * capsExtensionLength;
				tmpPosition.z = arcProperties.EndTangent.z * capsExtensionLength;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				tmpPosition.x = center.x + arcProperties.EndSegmentUnitPosition.x * tmpOuterRadius.x + arcProperties.EndTangent.x * capsExtensionLength;
				tmpPosition.y = center.y + arcProperties.EndSegmentUnitPosition.y * tmpOuterRadius.y + arcProperties.EndTangent.y * capsExtensionLength;
				tmpPosition.z = arcProperties.EndTangent.z * capsExtensionLength;
				vh.AddVert(
					tmpPosition, color, uv,
					UI.GeoUtils.ZeroV2, UI.GeoUtils.UINormal, UI.GeoUtils.UITangent);

				int currentVertCount = vh.currentVertCount;

				// add end antiAliasing triangles

				// center
				vh.AddTriangle(currentVertCount - 1, currentVertCount - 2, innerBaseIndex + 1);
				vh.AddTriangle(currentVertCount - 1, innerBaseIndex + 1, innerBaseIndex + 2);

				// inner
				vh.AddTriangle(edgesBaseIndex + 3, innerBaseIndex + 1, currentVertCount - 6);
				vh.AddTriangle(currentVertCount - 6, innerBaseIndex + 1, currentVertCount - 2);

				// outer
				vh.AddTriangle(edgesBaseIndex + 4, currentVertCount - 5, innerBaseIndex + 2);
				vh.AddTriangle(currentVertCount - 5, currentVertCount - 1, innerBaseIndex + 2);


				// add start antiAliasing triangles

				// center
				vh.AddTriangle(currentVertCount - 3, numVertices, currentVertCount - 4);
				vh.AddTriangle(currentVertCount - 3, numVertices + 1, numVertices);

				// inner
				vh.AddTriangle(currentVertCount - 4, numVertices, currentVertCount - 8);
				vh.AddTriangle(innerBaseIndex + 3, currentVertCount - 8, numVertices);

				// outer
				vh.AddTriangle(currentVertCount - 7, innerBaseIndex + 4, numVertices + 1);
				vh.AddTriangle(currentVertCount - 7, numVertices + 1, currentVertCount - 3);
			}
		}
	}
}
