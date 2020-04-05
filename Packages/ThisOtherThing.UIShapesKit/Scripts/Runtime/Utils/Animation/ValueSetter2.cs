using UnityEngine;
using System.Collections;
using System.Reflection;

namespace ThisOtherThing.Utils.Animation
{
	[ExecuteInEditMode]
	public class ValueSetter2 : MonoBehaviour
	{
		public static BindingFlags binding = BindingFlags.Instance | BindingFlags.Public;

		public int FieldType = 0;

		public bool IsInArray;
		public int ArrayItemIndex;

		public bool IsInClass;

		public string TargetTypeName;
		public string TargetFieldName;
		public string FieldName;
		public string ArrayFieldName;

		public string TargetClassFieldName;
		public string ClassFieldName;

		public float FloatValue;
		public Color ColorValue;

		float cachedFloatValue = float.NegativeInfinity;
		Color cachedColorValue;
		bool cachedBoolValue;

		ThisOtherThing.UI.Shapes.IShape target;
		System.Object targetField;
		FieldInfo fieldInfo;

		void OnValidate()
		{
			UpdateCachedReferences ();
		}

		void Start()
		{
			UpdateCachedReferences();
		}

		// this needs to be in update, since Animation can only set public fields and not porperties :(
		void Update()
		{
			if (
				targetField != null &&
				fieldInfo != null &&
				(!cachedFloatValue.Equals(FloatValue) || !cachedColorValue.Equals(ColorValue))
			) {
				if (FieldType == 0)
				{
					fieldInfo.SetValue(targetField, FloatValue);
				}
				else if (FieldType == 1 )
				{
					fieldInfo.SetValue(targetField, FloatValue >= 0.99f);
				}
				else if (FieldType == 2)
				{
					fieldInfo.SetValue(targetField, (Color32)ColorValue);
				}

				target.ForceMeshUpdate();
				cachedFloatValue = FloatValue;
				cachedColorValue = ColorValue;
			}
		}

		void UpdateCachedReferences()
		{
			if (TargetTypeName == null || TargetFieldName == null)
				return;

			if (target == null)
			{
				target = gameObject.GetComponent<ThisOtherThing.UI.Shapes.IShape>();
			}

			targetField = target.GetType()
				.GetField(TargetFieldName, binding)
				.GetValue(target);

			if (IsInArray)
			{
				FieldInfo fieldNameInfo = targetField.GetType().GetField(FieldName);

				if (fieldNameInfo == null)
					return;

				System.Type elementType = fieldNameInfo.FieldType.GetElementType();

				if (elementType != null)
				{
					fieldInfo = targetField.GetType()
						.GetField(FieldName, binding)
						.FieldType
						.GetElementType()
						.GetField(ArrayFieldName, binding);

					System.Array arr = (System.Array)targetField.GetType()
						.GetField(FieldName, binding)
						.GetValue(targetField);
					targetField = arr.GetValue(ArrayItemIndex);
				}
			}
			else
			{
				fieldInfo = System.Type.GetType(TargetTypeName)
					.GetField(FieldName, BindingFlags.Instance | BindingFlags.Public);
			}

			if (IsInClass)
			{
				if (TargetClassFieldName.Length == 0 || ClassFieldName.Length == 0)
					return;

				FieldInfo tmpTargetFieldInfo = targetField.GetType()
					.GetField(TargetClassFieldName, binding);

				if (tmpTargetFieldInfo == null)
					return;

				targetField = tmpTargetFieldInfo
					.GetValue(targetField);

				fieldInfo = targetField.GetType()
					.GetField(ClassFieldName, binding);
			}
		}
	}
}