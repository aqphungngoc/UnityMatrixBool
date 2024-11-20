using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Qutility.Type
{
    [CustomPropertyDrawer(typeof(Bool2DArray))]
    public class Bool2DArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float singleHeight = EditorGUIUtility.singleLineHeight;

            Rect rectFoldout = BoolMatrixDrawer.GetRectFirstChild(position, position.width * 0.79f, singleHeight);
            property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, label);

            var matrixProperty = property.FindPropertyRelative("matrix");
            var rowCountProperty = property.FindPropertyRelative("rowCount");
            var colCountProperty = property.FindPropertyRelative("columnCount");

            Rect rowCountRect = BoolMatrixDrawer.GetRectRight(position, position.width * 0.1f, singleHeight, -(position.width * 0.21f));
            int rowCount = EditorGUI.IntField(rowCountRect, GUIContent.none, rowCountProperty.intValue);
            Rect colCountRect = BoolMatrixDrawer.GetRectRight(position, position.width * 0.1f, singleHeight, -(position.width * 0.1f));
            int columnCount = EditorGUI.IntField(colCountRect, GUIContent.none, colCountProperty.intValue);

            if (rowCount != rowCountProperty.intValue)
            {
                rowCountProperty.intValue = rowCount;

                matrixProperty.arraySize = (rowCount * columnCount + 7) / 8;

                property.isExpanded = rowCount != 0;
            }

            if (columnCount != colCountProperty.intValue)
            {
                colCountProperty.intValue = columnCount;

                matrixProperty.arraySize = (rowCount * columnCount + 7) / 8;

                property.isExpanded = columnCount != 0;
            }

            if (!property.isExpanded) return;

            var isAscendingProperty = property.FindPropertyRelative("isYup");
            Rect yAscenedingRect = BoolMatrixDrawer.GetExactRectUnder(rectFoldout, singleHeight, singleHeight);
            isAscendingProperty.boolValue = EditorGUI.Toggle(yAscenedingRect, "Screen coordinate", isAscendingProperty.boolValue);

            bool isSortYincrease = isAscendingProperty.boolValue;

            SerializedProperty[] cacheRowProperties = new SerializedProperty[(rowCount * columnCount + 7) / 8];
            float padding = 4;
            float cellSize = singleHeight - padding;

            Rect rowRect = BoolMatrixDrawer.GetExactRectUnder(rectFoldout, singleHeight, 0, singleHeight);
            Vector2 startMatrixPos = Vector2.zero;

            for (int i = 0; i < rowCount; i++)
            {
                int y = isSortYincrease ? i : rowCount - 1 - i;

                // row name index
                Rect cellRect = BoolMatrixDrawer.GetRectFirstChild(rowRect, singleHeight * 3, singleHeight, singleHeight);
                EditorGUI.LabelField(cellRect, y.ToString());

                // start Serialize Cell Element
                cellRect = BoolMatrixDrawer.GetRectRight(cellRect, cellSize, cellSize, padding, padding / 2f);

                if (i == 0) startMatrixPos = new Vector2(cellRect.x, cellRect.y);

                for (int x = 0; x < columnCount; x++)
                {
                    int index = y * columnCount + x;
                    int byteIndex = index / 8;
                    int bitIndex = index % 8;
                    cacheRowProperties[byteIndex] ??= matrixProperty.GetArrayElementAtIndex(byteIndex);
                    bool cellValue = ((byte)cacheRowProperties[byteIndex].intValue & (1 << bitIndex)) != 0;
                    if (cellValue)
                    {
                        EditorGUI.DrawRect(BoolMatrixDrawer.GetZoomRect(cellRect, -2), Color.white);
                    }
                    else
                    {
                        EditorGUI.DrawRect(cellRect, Color.gray);
                    }

                    // next Cell Rect
                    cellRect = BoolMatrixDrawer.GetRectRight(cellRect, cellSize, cellSize, padding);
                }
                // nextRow Rect
                rowRect = BoolMatrixDrawer.GetExactRectUnder(rowRect, singleHeight);
            }

            rowRect = BoolMatrixDrawer.GetRectFirstChild(rowRect, rowRect.width - singleHeight, singleHeight, singleHeight);
            EditorGUI.LabelField(rowRect, new GUIContent("Hold LAlt to remove matrix value"));

            Rect dragRect = new Rect(startMatrixPos.x, startMatrixPos.y, columnCount * singleHeight, rowCount * singleHeight);

            // Handle mouse events for dragging toggles
            HandleMouseDrag(dragRect, cacheRowProperties, rowCount, columnCount, singleHeight, isSortYincrease);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            var size = property.FindPropertyRelative("rowCount").intValue;
            return (size + 3) * EditorGUIUtility.singleLineHeight;
        }

        private void HandleMouseDrag(Rect position, SerializedProperty[] cacheRowProperty, int rowSize, int colSize, float toggleWidth,
            bool isSortYincrease)
        {
            bool hasDrag = false;
            Event currentEvent = Event.current;
            if (currentEvent.button == 0 &&
                currentEvent.type == EventType.MouseDrag &&
                position.Contains(currentEvent.mousePosition))
            {
                Vector2Int indexPos = GetIndexPos(currentEvent.mousePosition);

                //Debug.Log($"Mouse Drag at {indexPos}");

                SetSerializeValue(indexPos.x, indexPos.y, !currentEvent.alt);
                // Use `Event.current.Use()` to consume the event
                currentEvent.Use();

                hasDrag = true;
            }

            if (hasDrag) return;

            if (currentEvent.button == 0 &&
                currentEvent.type == EventType.MouseDown &&
                !currentEvent.alt &&
                position.Contains(currentEvent.mousePosition))
            {
                Vector2Int indexPos = GetIndexPos(currentEvent.mousePosition);

                ToogleSerializeValue(indexPos.x, indexPos.y);

                currentEvent.Use();
            }

            void ToogleSerializeValue(int x, int y)
            {
                // Ensure the index is within bounds
                if (x > -1 && x < colSize && y > -1 && y < rowSize)
                {
                    int index = y * colSize + x;
                    int byteIndex = index / 8;
                    int bitIndex = index % 8;
                    // Toggle the boolean value 
                    cacheRowProperty[byteIndex].intValue ^= 1 << bitIndex;
                }
            }

            void SetSerializeValue(int x, int y, bool value)
            {
                // Ensure the index is within bounds
                if (x > -1 && x < colSize && y > -1 && y < rowSize)
                {
                    int index = y * colSize + x;
                    int byteIndex = index / 8;
                    int bitIndex = index % 8;
                    cacheRowProperty[byteIndex].intValue = value
                        ? cacheRowProperty[byteIndex].intValue | (1 << bitIndex)
                        : cacheRowProperty[byteIndex].intValue & ~(1 << bitIndex);
                }
            }

            Vector2Int GetIndexPos(Vector2 mousePos)
            {
                // Calculate the index based on mouse position
                float xPos = mousePos.x - position.x;
                int indexX = Mathf.FloorToInt(xPos / toggleWidth);

                float yPos = mousePos.y - position.y;

                int indexY = isSortYincrease ? Mathf.FloorToInt(yPos / toggleWidth) : rowSize - 1 - Mathf.FloorToInt(yPos / toggleWidth);

                return new Vector2Int(indexX, indexY);
            }
        }
    }
}