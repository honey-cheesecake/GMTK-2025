using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "CatStats", menuName = "Scriptable Objects/CatStats")]
public class CatStats : ScriptableObject
{
    [SerializeField] string displayName;
    [SerializeField] [TextArea(3, 5)] string description;
    [SerializeField] SpriteLibraryAsset sprites;
    [SerializeField] [Range(0f, 20f)] float minMoveSpeed;
    [SerializeField] [Range(0f, 20f)] float maxMoveSpeed;

    public SpriteLibraryAsset SpriteLibraryAsset { get { return sprites; } }

}
