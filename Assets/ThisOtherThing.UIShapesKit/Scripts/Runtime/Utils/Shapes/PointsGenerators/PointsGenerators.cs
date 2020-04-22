using System.Collections;
using UnityEngine;

using PointListProperties = ThisOtherThing.UI.ShapeUtils.PointsList.PointListProperties;

namespace ThisOtherThing.UI.ShapeUtils
{
	public class PointsGenerator
	{
		public static void SetPoints(
			ref Vector2[] positions,
			PointsList.PointListGeneratorData data
		) {
			switch (data.Generator) {
				case PointsList.PointListGeneratorData.Generators.Custom:
					break;

				case PointsList.PointListGeneratorData.Generators.Rect:
					SetPointsRect(ref positions, data);
					break;

				case PointsList.PointListGeneratorData.Generators.Round:
					SetPointsRound(ref positions, data);
					break;

				case PointsList.PointListGeneratorData.Generators.RadialGraph:
					SetPointsRadialGraph(ref positions, data);
					break;

				case PointsList.PointListGeneratorData.Generators.LineGraph:
					SetPointsLineGraph(ref positions, data);
					break;

				case PointsList.PointListGeneratorData.Generators.AngleLine:
					SetPointsAngleLine(ref positions, data);
					break;

				case PointsList.PointListGeneratorData.Generators.Star:
					SetPointsStar(ref positions, data);
					break;

				case PointsList.PointListGeneratorData.Generators.Gear:
					SetPointsGear(ref positions, data);
					break;
			}
		}

		public static void SetPointsRect(
			ref Vector2[] positions,
			PointsList.PointListGeneratorData data
		) {
			if (
				positions == null ||
				positions.Length != 4
			) {
				positions = new Vector2[4];
			}

			float halfWidth = data.Width * 0.5f;
			float halfHeight = data.Height * 0.5f;

			int offset = data.IntStartOffset % 4;

			offset = 4 + offset;
			offset %= 4;

			for (int i = 0; i < 4; i++)
			{
				int index = i + offset;
				index %= 4;

				switch (index)
				{
					case 0:
						positions[i].x = data.Center.x - halfWidth;
						positions[i].y = data.Center.y + halfHeight;
						break;
					case 1:
						positions[i].x = data.Center.x + halfWidth;
						positions[i].y = data.Center.y + halfHeight;
						break;
					case 2:
						positions[i].x = data.Center.x + halfWidth;
						positions[i].y = data.Center.y - halfHeight;
						break;
					case 3:
						positions[i].x = data.Center.x - halfWidth;
						positions[i].y = data.Center.y - halfHeight;
						break;
				}
			}
		}

		public static void SetPointsRound(
			ref Vector2[] positions,
			PointsList.PointListGeneratorData data
		) {
			float absLength = Mathf.Abs(data.Length);
			
			int numFullSteps = Mathf.CeilToInt(data.Resolution * absLength);
			float partStepAmount = 1.0f + ((data.Resolution * absLength) - (float)numFullSteps);

			bool addPartialStep = partStepAmount >= 0.0001f;

			int resolution = numFullSteps;

			if (addPartialStep) {
				resolution++;
			}

			if (data.CenterPoint)
			{
				resolution++;
			}

			if (
				positions == null ||
				positions.Length != resolution
			) {
				positions = new Vector2[resolution];
			}

			if (data.CenterPoint)
			{
				positions[resolution-1].x = data.Center.x;
				positions[resolution-1].y = data.Center.y;
			}

			float halfWidth = Mathf.Max(0.001f, data.Width * 0.5f);
			float halfHeight = Mathf.Max(0.001f, data.Height * 0.5f);

			float angle = data.FloatStartOffset * GeoUtils.TwoPI;
			float angleIncrement = (GeoUtils.TwoPI) / (float)(data.Resolution);

			if (data.SkipLastPosition)
			{
				angleIncrement = (GeoUtils.TwoPI) / ((float)data.Resolution + 1);
			}

			angleIncrement *= Mathf.Sign(data.Direction);

			float relCompletion;

			for (int i = 0; i < numFullSteps; i++)
			{
				relCompletion = (float)i / (float)resolution;

				positions[i].x = data.Center.x + Mathf.Sin(angle) * (halfWidth + (halfWidth * data.EndRadius * relCompletion));
				positions[i].y = data.Center.y + Mathf.Cos(angle) * (halfHeight + (halfHeight * data.EndRadius * relCompletion));

				angle += angleIncrement;
			}

			// add last point
			if (addPartialStep)
			{
				relCompletion = ((float)numFullSteps + partStepAmount) / (float)resolution;
//				angle -= angleIncrement * (1.0f - partStepAmount);
				positions[numFullSteps].x = data.Center.x + Mathf.Sin(angle) * (halfWidth + (halfWidth * data.EndRadius * relCompletion));
				positions[numFullSteps].y = data.Center.y + Mathf.Cos(angle) * (halfHeight + (halfHeight * data.EndRadius * relCompletion));

				int prevStep = Mathf.Max(numFullSteps-1, 0);

				// lerp back to partial position
				positions[numFullSteps].x = Mathf.LerpUnclamped(positions[prevStep].x, positions[numFullSteps].x, partStepAmount);
				positions[numFullSteps].y = Mathf.LerpUnclamped(positions[prevStep].y, positions[numFullSteps].y, partStepAmount);
			}
		}

		public static void SetPointsRadialGraph(
			ref Vector2[] positions,
			PointsList.PointListGeneratorData data
		) {
			int resolution = data.FloatValues.Length;

			if (data.FloatValues.Length < 3)
				return;

			if (
				positions == null ||
				positions.Length != resolution
			) {
				positions = new Vector2[resolution];
			}

			float angle = data.FloatStartOffset * GeoUtils.TwoPI;
			float angleIncrement = GeoUtils.TwoPI / (float)(resolution);

			for (int i = 0; i < resolution; i++)
			{
				float value = Mathf.InverseLerp(
					data.MinFloatValue,
					data.MaxFloatValue,
					data.FloatValues[i]
				);

				value *= data.Radius;

				positions[i].x = data.Center.x + Mathf.Sin(angle) * value;
				positions[i].y = data.Center.y + Mathf.Cos(angle) * value;

				angle += angleIncrement;
			}
		}

		public static void SetPointsLineGraph(
			ref Vector2[] positions,
			PointsList.PointListGeneratorData data
		) {
			int resolution = data.FloatValues.Length;

			if (data.FloatValues.Length < 2)
				return;

			if (data.CenterPoint)
				resolution += 2;

			if (
				positions == null ||
				positions.Length != resolution
			) {
				positions = new Vector2[resolution];
			}

			float xPos = data.Center.x + data.Width * -0.5f;

			float xStep = data.Width / (float)(data.FloatValues.Length - 1.0f);

			for (int i = 0; i < data.FloatValues.Length; i++)
			{
				float value = Mathf.InverseLerp(
					data.MinFloatValue,
					data.MaxFloatValue,
					data.FloatValues[i]
				);

				value -= 0.5f;

				value *= data.Height;

				positions[i].x = xPos;
				positions[i].y = data.Center.y + value;

				xPos += xStep;
			}

			if (data.CenterPoint)
			{
				positions[data.FloatValues.Length].x = data.Center.x + data.Width * 0.5f;
				positions[data.FloatValues.Length].y = data.Center.y - data.Height * 0.5f;

				positions[data.FloatValues.Length + 1].x = data.Center.x + data.Width * -0.5f;
				positions[data.FloatValues.Length + 1].y = positions[data.FloatValues.Length].y;
			}
		}

		public static void SetPointsAngleLine(
			ref Vector2[] positions,
			PointsList.PointListGeneratorData data
		) {
			if (
				positions == null ||
				positions.Length != 2
			) {
				positions = new Vector2[2];
			}

			float xDir = Mathf.Sin(data.Angle * GeoUtils.TwoPI);
			float yDir = Mathf.Cos(data.Angle * GeoUtils.TwoPI);

			float startOffset = data.Length * data.FloatStartOffset;

			positions[0].x = data.Center.x + xDir * startOffset;
			positions[0].y = data.Center.y + yDir * startOffset;

			positions[1].x = data.Center.x + xDir * (data.Length + startOffset);
			positions[1].y = data.Center.y + yDir * (data.Length + startOffset);
		}

		public static void SetPointsStar(
			ref Vector2[] positions,
			PointsList.PointListGeneratorData data
		) {
			int resolution = data.Resolution * 2;

			if (
				positions == null ||
				positions.Length != resolution
			) {
				positions = new Vector2[resolution];
			}

			float angle = data.FloatStartOffset * GeoUtils.TwoPI;
			float angleIncrement = (GeoUtils.TwoPI * data.Length) / (float)resolution;

			float outerRadiusX = data.Width;
			float outerRadiusY = data.Height;

			float innerRadiusX = data.EndRadius * outerRadiusX;
			float innerRadiusY = data.EndRadius * outerRadiusX;

			for (int i = 0; i < resolution; i+= 2)
			{
				// add outer point
				positions[i].x = data.Center.x + Mathf.Sin(angle) * outerRadiusX;
				positions[i].y = data.Center.y + Mathf.Cos(angle) * outerRadiusY;

				angle += angleIncrement;

				// add inner point
				positions[i+1].x = data.Center.x + Mathf.Sin(angle) * innerRadiusX;
				positions[i+1].y = data.Center.y + Mathf.Cos(angle) * innerRadiusY;

				angle += angleIncrement;
			}
		}

		public static void SetPointsGear(
			ref Vector2[] positions,
			PointsList.PointListGeneratorData data
		) {
			int resolution = data.Resolution * 4;

			if (
				positions == null ||
				positions.Length != resolution
			) {
				positions = new Vector2[resolution];
			}

			float angle = data.FloatStartOffset * GeoUtils.TwoPI;
			float angleIncrement = GeoUtils.TwoPI / (float)data.Resolution;

			float outerRadiusX = data.Width;
			float outerRadiusY = data.Height;

			float innerRadiusX = data.EndRadius * outerRadiusX;
			float innerRadiusY = data.EndRadius * outerRadiusY;

			float bottomAngleOffset = angleIncrement * 0.49f * data.InnerScaler;
			float topAngleOffset = angleIncrement * 0.49f * data.OuterScaler;

			int index;

			for (int i = 0; i < data.Resolution; i++)
			{
				index = i * 4;

				// add first inner point
				positions[index].x = data.Center.x + Mathf.Sin(angle - bottomAngleOffset) * innerRadiusX;
				positions[index].y = data.Center.y + Mathf.Cos(angle - bottomAngleOffset) * innerRadiusY;

				// add first outer point
				positions[index + 1].x = data.Center.x + Mathf.Sin(angle - topAngleOffset) * outerRadiusX;
				positions[index + 1].y = data.Center.y + Mathf.Cos(angle - topAngleOffset) * outerRadiusY;

				// add secont outer point
				positions[index + 2].x = data.Center.x + Mathf.Sin(angle + topAngleOffset) * outerRadiusX;
				positions[index + 2].y = data.Center.y + Mathf.Cos(angle + topAngleOffset) * outerRadiusY;

				// add second inner point
				positions[index + 3].x = data.Center.x + Mathf.Sin(angle + bottomAngleOffset) * innerRadiusX;
				positions[index + 3].y = data.Center.y + Mathf.Cos(angle + bottomAngleOffset) * innerRadiusY;

				angle += angleIncrement;
			}
		}
	}
}
