using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "CatStats", menuName = "Scriptable Objects/CatStats")]
public class CatStats : ScriptableObject
{
    public enum RarityType { Common, Rare, EasterEgg }
    [SerializeField] bool isDog;
    [SerializeField] string displayName;
    [SerializeField] RarityType rarity;
    [SerializeField] [Range(0.1f, 3f)] float size = 1;
    [SerializeField] [TextArea(3, 5)] string description;
    [SerializeField] SpriteLibraryAsset sprites;
    [SerializeField] [Range(0f, 20f)] float minMoveSpeed;
    [SerializeField] [Range(0f, 20f)] float maxMoveSpeed;
    [SerializeField] [Range(0f, 40f)] float runSpeed;
    [SerializeField] public AudioClip audioClip;

    public SpriteLibraryAsset SpriteLibraryAsset { get { return sprites; } }
    public float MinMoveSpeed { get {  return minMoveSpeed; } }
    public float MaxMoveSpeed { get {  return maxMoveSpeed; } }
    public float RunSpeed { get {  return runSpeed; } }
    public RarityType Rarity { get {  return rarity; } }
    public float Size { get {  return size; } }
    public AudioClip AudioClip { get { return audioClip; } }

    public int GetSpawnWeight()
    {
        switch (rarity)
        {
            case RarityType.Common:
                return 8;
            case RarityType.Rare:
                return 3;
            case RarityType.EasterEgg:
            default:
                return 1;
        }
    }

    public int GetScore()
    {
        if (isDog)
            return -100;
        switch (rarity)
        {
            case RarityType.Common:
                return 10;
            case RarityType.Rare:
                return 30;
            case RarityType.EasterEgg:
            default:
                return 100;
        }
    }

}
