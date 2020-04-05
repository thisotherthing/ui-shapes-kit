using UnityEngine;
using UnityEngine.UI;

namespace ThisOtherThing.UI.Shapes
{

	[AddComponentMenu("UI/Shapes/Empty Fill Rect", 200)]
	public class EmptyFillRect : Graphic
	{
		public override void SetMaterialDirty() { return; }
		public override void SetVerticesDirty() { return; }

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
	}
}
