using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ThisOtherThing.UI.Shapes
{
	[AddComponentMenu("UI/Shapes/Ellipse", 1)]
	public class Ellipse : MaskableGraphic, IShape
	{
		public GeoUtils.OutlineShapeProperties ShapeProperties =
			new GeoUtils.OutlineShapeProperties();

		public ShapeUtils.Ellipses.EllipseProperties EllipseProperties =
			new ShapeUtils.Ellipses.EllipseProperties();

		public GeoUtils.OutlineProperties OutlineProperties = 
			new GeoUtils.OutlineProperties();

		public GeoUtils.ShadowsProperties ShadowProperties = new GeoUtils.ShadowsProperties();

		public GeoUtils.AntiAliasingProperties AntiAliasingProperties =
			new GeoUtils.AntiAliasingProperties();

		public Sprite Sprite;

		GeoUtils.UnitPositionData unitPositionData;
		GeoUtils.EdgeGradientData edgeGradientData;
		Vector2 radius = Vector2.one;

		public void ForceMeshUpdate()
		{
			SetVerticesDirty();
			SetMaterialDirty();
		}

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			EllipseProperties.OnCheck();
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

			Rect pixelRect = RectTransformUtility.PixelAdjustRect(rectTransform, canvas);

			ShapeUtils.Ellipses.SetRadius(
				ref radius,
				pixelRect.width,
				pixelRect.height,
				EllipseProperties
			);

			EllipseProperties.UpdateAdjusted(radius, 0.0f);
			AntiAliasingProperties.UpdateAdjusted(canvas);


			// draw fill shadows
			if (ShadowProperties.ShadowsEnabled)
			{
				if (ShapeProperties.DrawFill && ShapeProperties.DrawFillShadow)
				{
					for (int i = 0; i < ShadowProperties.Shadows.Length; i++)
					{
						edgeGradientData.SetActiveData(
							1.0f - ShadowProperties.Shadows[i].Softness,
							ShadowProperties.Shadows[i].Size,
							AntiAliasingProperties.Adjusted
						);

						ShapeUtils.Ellipses.AddCircle(
							ref vh,
							ShadowProperties.GetCenterOffset(pixelRect.center, i),
							radius,
							EllipseProperties,
							ShadowProperties.Shadows[i].Color,
							GeoUtils.ZeroV2,
							ref unitPositionData,
							edgeGradientData
						);
					}
				}
			}

			if (ShadowProperties.ShowShape && ShapeProperties.DrawFill)
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

				ShapeUtils.Ellipses.AddCircle(
					ref vh,
					(Vector3)pixelRect.center,
					radius,
					EllipseProperties,
					ShapeProperties.FillColor,
					GeoUtils.ZeroV2,
					ref unitPositionData,
					edgeGradientData
				);
			}

			if (ShadowProperties.ShadowsEnabled)
			{
				// draw outline shadows
				if (ShapeProperties.DrawOutline && ShapeProperties.DrawOutlineShadow)
				{
					for (int i = 0; i < ShadowProperties.Shadows.Length; i++)
					{
						edgeGradientData.SetActiveData(
							1.0f - ShadowProperties.Shadows[i].Softness,
							ShadowProperties.Shadows[i].Size,
							AntiAliasingProperties.Adjusted
						);

						ShapeUtils.Ellipses.AddRing(
							ref vh,
							ShadowProperties.GetCenterOffset(pixelRect.center, i),
							radius,
							OutlineProperties,
							EllipseProperties,
							ShadowProperties.Shadows[i].Color,
							GeoUtils.ZeroV2,
							ref unitPositionData,
							edgeGradientData
						);
					}
				}
			}


			// fill
			if (ShadowProperties.ShowShape && ShapeProperties.DrawOutline)
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

				ShapeUtils.Ellipses.AddRing(
					ref vh,
					(Vector3)pixelRect.center,
					radius,
					OutlineProperties,
					EllipseProperties,
					ShapeProperties.OutlineColor,
					Vector2.zero,
					ref unitPositionData,
					edgeGradientData
				);
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
