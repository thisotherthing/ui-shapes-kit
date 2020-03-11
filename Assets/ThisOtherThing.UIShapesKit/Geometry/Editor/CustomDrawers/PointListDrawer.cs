using UnityEngine;
using UnityEditor;

public class PointListDrawer
{

	static Vector3 worldPosition;
	static Vector3 uiNormal = Vector3.forward;
	static Vector3 tmpUiPos = Vector3.zero;
	static Vector3 tmpUiPos2 = Vector3.zero;

	static Vector3 draggedPosition = Vector3.zero;
	static Vector2 offset = Vector3.zero;

	public static bool Draw(
		ref Vector2[] positions,
		RectTransform rectTransform,
		bool isClosed,
		int minPoints
	) {
		bool needsUpdate = false;

		bool runDelete = Event.current.modifiers == EventModifiers.Control;
		bool axisSnapping = Event.current.modifiers == EventModifiers.Shift;

		if (runDelete)
		{
			needsUpdate |= DrawRemovePointPosition(ref positions, rectTransform, minPoints);
		}
		else
		{

			for (int i = 0; i < positions.Length; i++)
			{
				needsUpdate |= DrawUpdatePointPosition(ref positions[i], rectTransform, axisSnapping);
			}

			needsUpdate |= DrawInbetweenButtons(ref positions, rectTransform, isClosed);
		}

		return needsUpdate;
	}

	static bool DrawUpdatePointPosition(
		ref Vector2 position,
		RectTransform rectTransform,
		bool axisSnapping
	) {
		worldPosition = rectTransform.TransformPoint(position);

		draggedPosition = rectTransform.InverseTransformPoint(
			Handles.FreeMoveHandle(
				worldPosition,
				Quaternion.identity,
				HandleUtility.GetHandleSize(worldPosition) * 0.1f,
				Vector3.zero,
				DrawPointHandle
			)
		);

		offset.x = draggedPosition.x - position.x;
		offset.y = draggedPosition.y - position.y;

		/// TODO snapping

		position.x += offset.x;
		position.y += offset.y;

		return offset.x != 0.0f || offset.y != 0.0f;
	}

	static bool DrawRemovePointPosition(
		ref Vector2[] positions,
		RectTransform rectTransform,
		int minPoints
	) {
		bool removedPoint = false;

		for (int i = 0; i < positions.Length; i++)
		{
			worldPosition = rectTransform.TransformPoint(positions[i]);

			float handleSize = HandleUtility.GetHandleSize(worldPosition) * 0.1f;

			if (
				Handles.Button(worldPosition, Quaternion.identity, handleSize, handleSize, DrawRemovePointHandle) &&
				positions.Length > minPoints
			) {
				// shift other points
				for (int j = i; j < positions.Length - 1; j++)
				{
					positions[j] = positions[j + 1];
				}

				System.Array.Resize(ref positions, positions.Length - 1);

				removedPoint = true;
			}
		}

		return removedPoint;
	}

	static bool DrawInbetweenButtons(
		ref Vector2[] positions,
		RectTransform rectTransform,
		bool isClosed
	) {
		bool addedPoint = false;

		Handles.color = Color.red;

		float handleSize;

		for (int i = positions.Length - 2; i >= 0; i--)
		{

			worldPosition.x = (positions[i].x + positions[i + 1].x) * 0.5f;
			worldPosition.y = (positions[i].y + positions[i + 1].y) * 0.5f;
			worldPosition.z = 0.0f;

			worldPosition =  rectTransform.TransformPoint(worldPosition);

			handleSize = HandleUtility.GetHandleSize(worldPosition) * 0.08f;

			if (
				Handles.Button(worldPosition, Quaternion.identity, handleSize, handleSize, DrawAddPointHandle)
			) {
				System.Array.Resize(ref positions, positions.Length + 1);

				// shift other points
				for (int j = positions.Length - 1; j > i; j--)
				{
					positions[j] = positions[j - 1];
				}

				positions[i+1] = rectTransform.InverseTransformPoint(worldPosition);

				addedPoint = true;
			}
		}
			
		if (isClosed)
		{
			worldPosition.x = (positions[0].x + positions[positions.Length - 1].x) * 0.5f;
			worldPosition.y = (positions[0].y + positions[positions.Length - 1].y) * 0.5f;
			worldPosition.z = 0.0f;

			worldPosition =  rectTransform.TransformPoint(worldPosition);

			handleSize = HandleUtility.GetHandleSize(worldPosition) * 0.08f;

			if (
				Handles.Button(worldPosition, Quaternion.identity, handleSize, handleSize, DrawAddPointHandle)
			) {
				System.Array.Resize(ref positions, positions.Length + 1);

				positions[positions.Length - 1] = rectTransform.InverseTransformPoint(worldPosition);

				// slightly offset positionif there is a closed loop and the new point is right between the two other points
				if (isClosed && positions.Length == 3)
				{
					positions[positions.Length - 1].y += 0.1f;
				}

				addedPoint = true;
			}
		}

		return addedPoint;
	}

	static void DrawPointHandle(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType){
		Handles.color = Color.black;

		Handles.DrawSolidDisc(position, uiNormal, size * 1.4f);

		Handles.color = Color.white;
		Handles.DrawSolidDisc(position, uiNormal, size);
		Handles.CircleHandleCap(controlId, position, rotation, size, eventType);

		Handles.color = Color.black;
		Handles.DrawSolidDisc(position, uiNormal, size * 0.8f);
	}

	static void DrawRemovePointHandle(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType){
		Handles.color = Color.black;

		Handles.DrawSolidDisc(position, uiNormal, size * 1.4f);
		Handles.CircleHandleCap(controlId, position, rotation,  size * 1.4f, eventType);

		Handles.color = Color.red;
		Handles.DrawSolidDisc(position, uiNormal, size);

		Handles.color = Color.black;
		Handles.DrawSolidDisc(position, uiNormal, size * 0.8f);
	}

	static void DrawAddPointHandle(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType){
		Handles.color = Color.black;
		Handles.CircleHandleCap(controlId, position, rotation, size, eventType);
		Handles.DrawSolidDisc(position, uiNormal, size);

		Handles.color = Color.white;
		Handles.DrawSolidDisc(position, uiNormal, size * 0.2f);

	}

	static void DrawPlus(Vector3 position, float size)
	{
		tmpUiPos.x = position.x - size * 0.5f;
		tmpUiPos.y = position.y;
		tmpUiPos.z = position.z;

		tmpUiPos2.x = position.x + size * 0.5f;
		tmpUiPos2.y = position.y;
		tmpUiPos2.z = position.z;

		Handles.DrawLine(tmpUiPos, tmpUiPos2);

		tmpUiPos.x = position.x;
		tmpUiPos.y = position.y - size * 0.5f;

		tmpUiPos2.x = position.x;
		tmpUiPos2.y = position.y + size * 0.5f;

		Handles.DrawLine(tmpUiPos, tmpUiPos2);
	}
}
