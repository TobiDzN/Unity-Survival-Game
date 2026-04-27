using UnityEngine;

[CreateAssetMenu(fileName = "MaterialsData", menuName = "Scriptable Objects/MaterialsData")]
public class MaterialsData : ScriptableObject
{
    public string displayName;
    public Sprite SpriteIcon;
    public int maxStack;
    public float burnTime;
    public float hardness;
    public bool isFlammable;
    public Texture itemIcon;
    public string description;
    public Texture hoverHighlightIcon;

}
