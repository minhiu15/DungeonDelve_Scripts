using System;
using UnityEngine;
using System.Linq;
using UnityEditor;


/// <summary>
/// Cấp 1 ID duy nhất cho đối tượng mỗi khi đối tượng được tạo trong game.
/// Và ID này sẽ duy trì cả trên Editor/Build, nếu xóa object hoặc xóa component này khỏi đối tượng và add lại component thì ID sẽ được tạo lại cái mới.
/// </summary>
public class MonoBehaviourID : MonoBehaviour
{
    [Serializable]
    private struct UniqueID
    {
        public string Value;
    }

    
    [SerializeField] private UniqueID uniqueID;
    public string GetID => uniqueID.Value;
    
    
    [ContextMenu("Force Reset ID")]
    private void ResetID()
    {
        uniqueID.Value = Guid.NewGuid().ToString();
        Debug.Log("Setting new ID on Object: " + gameObject.name, gameObject);
    }
    public static bool IsUnique(string ID) => Resources.FindObjectsOfTypeAll<MonoBehaviourID>().Any(x => x.GetID == ID);
    protected void OnValidate()
    {
        if (!gameObject.scene.IsValid()) // Nếu Object chưa khởi tạo thì không tạo ID để tránh lỗi. Vd: Prefab
        {
            uniqueID.Value = string.Empty;
            return;
        }
        if (string.IsNullOrEmpty(GetID) || !IsUnique(GetID)) // Nếu ID đang Null hoặc chưa có trong toàn bộ Script <MonoBehaviourID> đang có trên đối tượng
        {
            ResetID();
        }
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UniqueID))]
    private class UniqueIDDrawer : PropertyDrawer
    {
        private const float buttonWidth = 120f;
        private const float padding = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            GUI.enabled = false;
            var valueRect = position;
            valueRect.width -= buttonWidth + padding;
            var idProperty = property.FindPropertyRelative("Value");
            EditorGUI.PropertyField(valueRect, idProperty, GUIContent.none);
            GUI.enabled = true;

            var buttonRect = position;
            buttonRect.x += position.width - buttonWidth;
            buttonRect.width = buttonWidth;
            if (GUI.Button(buttonRect, "Copy to clipboard"))
            {
                EditorGUIUtility.systemCopyBuffer = idProperty.stringValue;
            }

            EditorGUI.EndProperty();
        }   
    }
    
#endif
    
}
