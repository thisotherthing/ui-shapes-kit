using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ThisOtherThing.UI.Shapes
{
	[AddComponentMenu("UI/Shapes/Line", 30)]
	public class Line : MaskableGraphic, IShape
	{
		public GeoUtils.ShapeProperties ShapeProperties =
			new GeoUtils.ShapeProperties();

		public ShapeUtils.PointsList.PointListsProperties PointListsProperties =
			new ShapeUtils.PointsList.PointListsProperties();

		public ShapeUtils.Lines.LineProperties LineProperties = 
			new ShapeUtils.Lines.LineProperties();

		public GeoUtils.OutlineProperties OutlineProperties = 
			new GeoUtils.OutlineProperties();

		public GeoUtils.ShadowsProperties ShadowProperties = new GeoUtils.ShadowsProperties();

		public GeoUtils.AntiAliasingProperties AntiAliasingProperties = 
			new GeoUtils.AntiAliasingProperties();

		public Sprite Sprite;

		ShapeUtils.PointsList.PointsData[] pointsListData =
			new ThisOtherThing.UI.ShapeUtils.PointsList.PointsData[] { new ShapeUtils.PointsList.PointsData()};
		
		GeoUtils.EdgeGradientData edgeGradientData;

		public void ForceMeshUpdate()
		{
			if (pointsListData == null || pointsListData.Length != PointListsProperties.PointListProperties.Length)
			{
				System.Array.Resize(ref pointsListData, PointListsProperties.PointListProperties.Length);
			}

			for (int i = 0; i < pointsListData.Length; i++)
			{
				pointsListData[i].NeedsUpdate = true;
				PointListsProperties.PointListProperties[i].GeneratorData.NeedsUpdate = true;
			}

			SetVerticesDirty();
			SetMaterialDirty();
		}

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			LineProperties.OnCheck();
			OutlineProperties.OnCheck();
			AntiAliasingProperties.OnCheck();

			ForceMeshUpdate();
		}
		#endif

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			OutlineProperties.UpdateAdjusted();
			ShadowProperties.UpdateAdjusted();

			if (pointsListData == null || pointsListData.Length != PointListsProperties.PointListProperties.Length)
			{
				System.Array.Resize(ref pointsListData, PointListsProperties.PointListProperties.Length);

				for (int i = 0; i < pointsListData.Length; i++)
				{
					pointsListData[i].NeedsUpdate = true;
					PointListsProperties.PointListProperties[i].GeneratorData.NeedsUpdate = true;
				}
			}

			for (int i = 0; i < PointListsProperties.PointListProperties.Length; i++)
				PointListsProperties.PointListProperties[i].SetPoints();

			for (int i = 0; i < PointListsProperties.PointListProperties.Length; i++)
			{
				if (
					PointListsProperties.PointListProperties[i].Positions != null &&
					PointListsProperties.PointListProperties[i].Positions.Length > 1
				) {
					AntiAliasingProperties.UpdateAdjusted(canvas);

					// shadows
					if (ShadowProperties.ShadowsEnabled)
					{
						for (int j = 0; j < ShadowProperties.Shadows.Length; j++)
						{
							edgeGradientData.SetActiveData(
								1.0f - ShadowProperties.Shadows[j].Softness,
								ShadowProperties.Shadows[j].Size,
								AntiAliasingProperties.Adjusted
							);

							ShapeUtils.Lines.AddLine(
								ref vh,
								LineProperties,
								PointListsProperties.PointListProperties[i],
								ShadowProperties.GetCenterOffset(GeoUtils.ZeroV2, j),
								OutlineProperties,
								ShadowProperties.Shadows[j].Color,
								GeoUtils.ZeroV2,
								ref pointsListData[i],
								edgeGradientData
							);
						}
					}
				}
			}

			for (int i = 0; i < PointListsProperties.PointListProperties.Length; i++)
			{
				if (
					PointListsProperties.PointListProperties[i].Positions != null &&
					PointListsProperties.PointListProperties[i].Positions.Length > 1
				) {
					// fill
					if (ShadowProperties.ShowShape)
					{
						if (AntiAliasingProperties.Adjusted > 0.0f)
						{
							edgeGradientData.SetActiveData(
								1.0f,
								0.0f,
								AntiAliasingProperties.Adjusted
							);
						}
						else
						{
							edgeGradientData.Reset();
						}

						ShapeUtils.Lines.AddLine(
							ref vh,
							LineProperties,
							PointListsProperties.PointListProperties[i],
							GeoUtils.ZeroV2,
							OutlineProperties,
							ShapeProperties.FillColor,
							GeoUtils.ZeroV2,
							ref pointsListData[i],
							edgeGradientData
						);
					}
				}
			}


		}

		protected override void UpdateMaterial()
		{
			base.UpdateMaterial();

			// check if this sprite has an associated alpha texture (generated when splitting RGBA = RGB + A as two textures without alpha)

			if (Sprite == null)
			{
				canvasRenderer.SetAlphaTexture(null);
				return;
			}

			Texture2D alphaTex = Sprite.associatedAlphaSplitTexture;

			if (alphaTex != null)
			{
				canvasRenderer.SetAlphaTexture(alphaTex);
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if (Sprite == null)
				{
					if (material != null && material.mainTexture != null)
					{
						return material.mainTexture;
					}
					return s_WhiteTexture;
				}

				return Sprite.texture;
			}
		}

	}
}
