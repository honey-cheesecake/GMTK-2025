using UnityEngine;

[CreateAssetMenu(fileName = "CatStats", menuName = "Scriptable Objects/CatStats")]
public class CatStats : ScriptableObject
{
    [SerializeField] string displayName;
    [SerializeField] Sprite sprite;
    [SerializeField] [Range(0f, 20f)] float moveSpeed;

}
