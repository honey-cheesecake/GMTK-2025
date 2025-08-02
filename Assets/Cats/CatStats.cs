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
    [SerializeField] [Range(0f, 20f)] float runSpeed;

    public SpriteLibraryAsset SpriteLibraryAsset { get { return sprites; } }
    public float MinMoveSpeed { get {  return minMoveSpeed; } }
    public float MaxMoveSpeed { get {  return maxMoveSpeed; } }
    public float RunSpeed { get {  return runSpeed; } }

}
