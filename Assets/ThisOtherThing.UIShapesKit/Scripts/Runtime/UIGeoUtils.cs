using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using ThisOtherThing.Utils;

namespace ThisOtherThing.UI
{
	public class GeoUtils
	{

		[System.Serializable]
		public class ShapeProperties
		{
			public Color32 FillColor = new Color32(255, 255, 255, 255);
		}

		[System.Serializable]
		public class OutlineShapeProperties : ShapeProperties
		{
			public bool DrawFill = true;
			public bool DrawFillShadow = true;

			public bool DrawOutline = false;
			public Color32 OutlineColor = new Color32(255, 255, 255, 255);
			public bool DrawOutlineShadow = false;
		}

		[System.Serializable]
		public class AntiAliasingProperties
		{
			public float AntiAliasing = 1.25f;

			public float Adjusted { get ; private set; }

			public void UpdateAdjusted(Canvas canvas)
			{
				if (canvas != null)
				{
					Adjusted = AntiAliasing * (1.0f / canvas.scaleFactor);
				}
				else
				{
					Adjusted = AntiAliasing;
				}
			}

			public void OnCheck()
			{
				AntiAliasing = Mathf.Max(AntiAliasing, 0.0f);
			}
		}

		[System.Serializable]
		public class RoundingProperties
		{
			public enum ResolutionType
			{
				Calculated,
				Fixed
			}

			public ResolutionType Resolution = ResolutionType.Calculated;
			[MinAttribute(2)]public int FixedResolution = 10;
			[MinAttribute(0.01f)]public float ResolutionMaxDistance = 4.0f;

			public int AdjustedResolution { private set; get; }
			public bool MakeSharpCorner { private set; get; }

			public void OnCheck(int minFixedResolution = 2)
			{
				FixedResolution = Mathf.Max(FixedResolution, minFixedResolution);
				ResolutionMaxDistance = Mathf.Max(ResolutionMaxDistance, 0.1f);
			}

			public void UpdateAdjusted(float radius, float offset, float numCorners)
			{
				UpdateAdjusted(radius, offset, this, numCorners);
			}

			public void UpdateAdjusted(
				float radius,
				float offset,
				RoundingProperties overrideProperties,
				float numCorners
			) {
				MakeSharpCorner = radius < 0.001f;

				radius += offset;

				switch (overrideProperties.Resolution)
				{
					case ResolutionType.Calculated:
						float circumference = GeoUtils.TwoPI * radius;

						AdjustedResolution = Mathf.CeilToInt(circumference / overrideProperties.ResolutionMaxDistance / numCorners);
						AdjustedResolution = Mathf.Max(AdjustedResolution, 2);
						break;
					case ResolutionType.Fixed:
						AdjustedResolution = overrideProperties.FixedResolution;
						break;
				}
			}
		}

		[System.Serializable]
		public class OutlineProperties
		{
			public enum LineType
			{
				Inner,
				Center,
				Outer
			}

			public LineType Type = LineType.Center;
			public float LineWeight = 2.0f;

			public float HalfLineWeight { private set; get; }

			public float GetOuterDistace()
			{
				switch (Type)
				{
					case LineType.Inner:
						return 0.0f;

					case LineType.Outer:
						return LineWeight;

					case LineType.Center:
						return LineWeight * 0.5f;

					default:
						throw new System.ArgumentOutOfRangeException();
				}
			}

			public float GetCenterDistace()
			{
				switch (Type)
				{
					case LineType.Inner:
						return LineWeight * -0.5f;

					case LineType.Outer:
						return LineWeight * 0.5f;

					case LineType.Center:
						return 0.0f;

					default:
						throw new System.ArgumentOutOfRangeException();
				}
			}

			public float GetInnerDistace()
			{
				switch (Type)
				{
					case LineType.Inner:
						return -LineWeight;

					case LineType.Outer:
						return 0.0f;

					case LineType.Center:
						return LineWeight * -0.5f;

					default:
						throw new System.ArgumentOutOfRangeException();
				}
			}

			public void OnCheck()
			{
				LineWeight = Mathf.Max(LineWeight, 0.0f);
			}

			public void UpdateAdjusted()
			{
				HalfLineWeight = LineWeight * 0.5f;
			}
		}

		[System.Serializable]
		public class ShadowsProperties
		{
			public bool ShowShape = true;
			public bool ShowShadows = true;

			[Range(-1.0f, 1.0f)] public float Angle = 0.0f;
			[MinAttribute(0.0f)] public float Distance = 0.0f;
			public ShadowProperties[] Shadows;

			[HideInInspector] public Vector2 Offset = Vector2.zero;

			public bool ShadowsEnabled
			{
				get
				{
					return ShowShadows && Shadows != null && Shadows.Length > 0; 
				}
			}

			public void UpdateAdjusted()
			{
				Offset.x = Mathf.Sin(Angle * Mathf.PI - Mathf.PI) * Distance;
				Offset.y = Mathf.Cos(Angle * Mathf.PI - Mathf.PI) * Distance;
			}

			public Vector2 GetCenterOffset(Vector2 center, int index)
			{
				center.x += Offset.x + Shadows[index].Offset.x;
				center.y += Offset.y + Shadows[index].Offset.y;

				return center;
			}
		}

		[System.Serializable]
		public class ShadowProperties
		{
			public Color32 Color = new Color32(0, 0, 0, 120);

			public Vector2 Offset = Vector2.zero;

			[MinAttribute(0.0f)] public float Size = 5.0f;

			[Range(0.0f, 1.0f)] public float Softness = 0.5f;
		}

		public struct EdgeGradientData
		{
			public bool IsActive;

			public float InnerScale;

			public float ShadowOffset;
			public float SizeAdd;

			public void SetActiveData(
				float innerScale,
				float shadowOffset,
				float sizeAdd
			) {
				IsActive = true;

				InnerScale = innerScale;

				ShadowOffset = shadowOffset;
				SizeAdd = sizeAdd;
			}

			public void Reset()
			{
				IsActive = false;

				InnerScale = 1.0f;

				ShadowOffset = 0.0f;
				SizeAdd = 0.0f;
			}
		}

		[System.Serializable]
		public class SnappedPositionAndOrientationProperties
		{
			public enum OrientationTypes
			{
				Horizontal,
				Vertical
			}

			public enum PositionTypes
			{
				Center,
				Top,
				Bottom,
				Left,
				Right
			}

			public OrientationTypes Orientation = OrientationTypes.Horizontal;
			public PositionTypes Position = PositionTypes.Center;
		}

		public struct UnitPositionData
		{
			public Vector3[] UnitPositions;

			public float LastBaseAngle;
			public float LastDirection;
		}

		public static readonly Vector3 UpV3 = Vector3.up;
		public static readonly Vector3 DownV3 = Vector3.down;
		public static readonly Vector3 LeftV3 = Vector3.left;
		public static readonly Vector3 RightV3 = Vector3.right;

		public static readonly Vector3 ZeroV3 = Vector3.zero;
		public static readonly Vector2 ZeroV2 = Vector2.zero;

		public static readonly Vector3 UINormal = Vector3.back;
		public static readonly Vector4 UITangent = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);

		public static readonly float HalfPI = Mathf.PI * 0.5f;
		public static readonly float TwoPI = Mathf.PI * 2.0f;

		public static float GetAdjustedAntiAliasing(
			Canvas canvas,
			float antiAliasing
		) {
			return antiAliasing * (1.0f / canvas.scaleFactor);
		}

		public static void AddOffset(
			ref float width,
			ref float height,
			float offset
		) {
			width += offset * 2.0f;
			height += offset * 2.0f;
		}


		public static void SetUnitPositionData(
			ref UnitPositionData unitPositionData,
			int resolution,
			float baseAngle = 0.0f,
			float direction = 1.0f
		) {
			bool needsUpdate = false;

			if (
				unitPositionData.UnitPositions == null ||
				unitPositionData.UnitPositions.Length != resolution)
			{
				unitPositionData.UnitPositions = new Vector3[resolution];

				for ( int i = 0; i < unitPositionData.UnitPositions.Length; i++ )
				{
					unitPositionData.UnitPositions[i] = ZeroV3;
				}

				needsUpdate = true;
			}

			needsUpdate |= 
				baseAngle != unitPositionData.LastBaseAngle ||
				direction != unitPositionData.LastDirection;

			if (needsUpdate)
			{
				float angleIncrement = TwoPI / (float)resolution;
				angleIncrement *= direction;
				float angle;

				for ( int i = 0; i < resolution; i++ )
				{
					angle = baseAngle + (angleIncrement * i);

					unitPositionData.UnitPositions[i].x = Mathf.Sin(angle);
					unitPositionData.UnitPositions[i].y = Mathf.Cos(angle);
				}

				unitPositionData.LastBaseAngle = baseAngle;
				unitPositionData.LastDirection = direction;
			}
		}

		public static void SetUnitPositions(
			ref Vector2[] positions,
			int resolution,
			float angleOffset = 0.0f,
			float radius = 1.0f
		) {
			float angle = angleOffset;
			float angleIncrement = GeoUtils.TwoPI / (float)(resolution);

			bool needsUpdate = false;

			if (
				positions == null ||
				positions.Length != resolution
			) {
				positions = new Vector2[resolution];

				needsUpdate = true;
			}

			// check for radius change
			if (!needsUpdate)
			{
				needsUpdate |= (positions[0].x * positions[0].x + positions[0].y * positions[0].y != radius * radius);
			}

			if (needsUpdate)
			{
				for (int i = 0; i < resolution; i++)
				{
					positions[i].x = Mathf.Sin(angle) * radius;
					positions[i].y = Mathf.Cos(angle) * radius;

					angle += angleIncrement;
				}
			}
		}

		public static float RadianAngleDifference( float angle1, float angle2 )
		{
			float diff = (angle2 - angle1 + Mathf.PI) % TwoPI - Mathf.PI;
			return diff < -Mathf.PI ? diff + TwoPI : diff;
		}

		public static int SimpleMap(int x, int in_max, int out_max)
		{
			return x * out_max / in_max;
		}

		public static float SimpleMap(float x, float in_max, float out_max)
		{
			return x * out_max / in_max;
		}

		public static float Map(float x, float in_min, float in_max, float out_min, float out_max)
		{
			return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
		}
	}
}
