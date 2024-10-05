using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Qutility.Type
{

    [CustomPropertyDrawer(typeof(BoolMatrix))]
    public class BoolMatrixDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float singleHeight = EditorGUIUtility.singleLineHeight;

            Rect rectFoldout = GetRectFirstChild(position, position.width * 0.9f, singleHeight);
            property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, label);

            var matrixProperty = property.FindPropertyRelative("matrix");
            var sizeProperty = matrixProperty.FindPropertyRelative("Array.size");

            Rect matrixSizeRect = GetRectRight(position, position.width * 0.1f, singleHeight, -(position.width * 0.1f));
            int matrixSize = EditorGUI.IntField(matrixSizeRect, GUIContent.none, sizeProperty.intValue);

            if (matrixSize != sizeProperty.intValue)
            {
                sizeProperty.intValue = matrixSize;

                matrixProperty = property.FindPropertyRelative("matrix");

                property.isExpanded = matrixSize != 0;
            }

            if (!property.isExpanded) return;

            var isAscendingProperty = property.FindPropertyRelative("isYup");
            Rect yAscenedingRect = GetExactRectUnder(rectFoldout, singleHeight, singleHeight);
            isAscendingProperty.boolValue = EditorGUI.Toggle(yAscenedingRect, "Screen coordinate", isAscendingProperty.boolValue);

            bool isSortYincrease = isAscendingProperty.boolValue;

            SerializedProperty[] cacheRowProperties = new SerializedProperty[matrixSize];
            float padding = 4;
            float cellHeight = singleHeight - padding;

            Rect rowRect = GetExactRectUnder(rectFoldout, singleHeight, 0, singleHeight);
            Vector2 startMatrixPos = Vector2.zero;

            for (int i = 0; i < matrixSize; i++)
            {
                int y = isSortYincrease ? i : matrixSize - 1 - i;

                // row name index
                Rect cellRect = GetRectFirstChild(rowRect, singleHeight * 3, singleHeight, singleHeight);
                EditorGUI.LabelField(cellRect, y.ToString());

                cacheRowProperties[y] = matrixProperty.GetArrayElementAtIndex(y);

                var rowBoolArray = cacheRowProperties[y].stringValue.ToCharArray();
                if (rowBoolArray.Length != matrixSize)
                {
                    var newChars = new char[matrixSize];

                    // Keep Old Value
                    for (int idC = 0; idC < matrixSize; idC++)
                    {
                        if (idC < rowBoolArray.Length)
                            newChars[idC] = rowBoolArray[idC];
                        else
                            newChars[idC] = BoolMatrix.falseC;
                    }

                    rowBoolArray = newChars;

                    cacheRowProperties[y].stringValue = new string(rowBoolArray);
                }

                // start Serialize Cell Element
                cellRect = GetRectRight(cellRect, cellHeight, cellHeight, padding, padding / 2f);

                if (i == 0) startMatrixPos = new Vector2(cellRect.x, cellRect.y);

                for (int x = 0; x < matrixSize; x++)
                {
                    if (rowBoolArray[x] == BoolMatrix.trueC)
                    {
                        EditorGUI.DrawRect(GetZoomRect(cellRect, -2), Color.white);
                    }
                    else
                    {
                        EditorGUI.DrawRect(cellRect, Color.gray);
                    }

                    // next Cell Rect
                    cellRect = GetRectRight(cellRect, cellHeight, cellHeight, padding);
                }
                // nextRow Rect
                rowRect = GetExactRectUnder(rowRect, singleHeight);
            }


            rowRect = GetRectFirstChild(rowRect, rowRect.width - singleHeight, singleHeight, singleHeight);
            EditorGUI.LabelField(rowRect, new GUIContent("Hold LAlt to remove matrix value"));

            Rect DragRect = new Rect(startMatrixPos.x, startMatrixPos.y, matrixSize * singleHeight, matrixSize * singleHeight);

            // Handle mouse events for dragging toggles
            HandleMouseDrag(DragRect, cacheRowProperties, matrixSize, singleHeight, isSortYincrease);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            var size = property.FindPropertyRelative("matrix").arraySize;
            return (size + 3) * EditorGUIUtility.singleLineHeight;
        }

        private void HandleMouseDrag(Rect position, SerializedProperty[] cacheRowProperty, int newSize, float toggleWidth, bool isSortYincrease)
        {
            bool hasDrag = false;
            Event currentEvent = Event.current;
            if (currentEvent.button == 0 &&
                currentEvent.type == EventType.MouseDrag &&
                position.Contains(currentEvent.mousePosition))
            {

                Vector2Int indexPos = GetIndexPos(currentEvent.mousePosition);

                Debug.Log($"Mouse Drag at {indexPos}");

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
                if (x > -1 && x < newSize &&
                    y > -1 && y < newSize)
                {
                    var charArray = cacheRowProperty[y].stringValue.ToCharArray();
                    charArray[x] = BoolMatrix.BoolChar(charArray[x] != BoolMatrix.trueC);
                    // Toggle the boolean value 
                    cacheRowProperty[y].stringValue = new string(charArray);
                }
            }

            void SetSerializeValue(int x, int y, bool value)
            {
                // Ensure the index is within bounds
                if (x > -1 && x < newSize &&
                    y > -1 && y < newSize)
                {
                    var charArray = cacheRowProperty[y].stringValue.ToCharArray();
                    charArray[x] = BoolMatrix.BoolChar(value);
                    cacheRowProperty[y].stringValue = new string(charArray);
                }
            }

            Vector2Int GetIndexPos(Vector2 mousePos)
            {
                // Calculate the index based on mouse position
                float xPos = mousePos.x - position.x;
                int indexX = Mathf.FloorToInt(xPos / toggleWidth);

                float yPos = mousePos.y - position.y;

                int indexY = isSortYincrease ? Mathf.FloorToInt(yPos / toggleWidth) : newSize - 1 - Mathf.FloorToInt(yPos / toggleWidth);



                return new Vector2Int(indexX, indexY);
            }
        }

        static Rect GetRectFirstChild(Rect parentRect, float witdh, float height, float paddingX = 0, float paddingY = 0)
        {
            return new Rect(parentRect.x + paddingX, parentRect.y + paddingY, witdh, height);
        }

        static Rect GetRectRight(Rect parentRect, float itemWitdh, float itemHeight, float paddingX = 0, float deltaY = 0)
        {
            return new Rect(parentRect.x + parentRect.width + paddingX, parentRect.y + deltaY, itemWitdh, itemHeight);
        }
        static Rect GetExactRectUnder(Rect parentRect, float height, float deltaX = 0, float paddingY = 0)
        {
            return new Rect(parentRect.x + deltaX, parentRect.y + parentRect.height + paddingY, parentRect.width - deltaX, height);
        }

        /// <summary>
        /// Zoom Up extra > 0
        /// Zoom Down extra < 0
        /// </summary>
        static Rect GetZoomRect(Rect parentRect, float extra)
        {
            return new Rect(parentRect.x - extra, parentRect.y - extra, parentRect.width + extra * 2, parentRect.height + extra * 2);
        }

    }

}
