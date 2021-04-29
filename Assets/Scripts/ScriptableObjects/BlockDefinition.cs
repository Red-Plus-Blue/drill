using UnityEngine;

using System.Collections.Generic;

[CreateAssetMenu(menuName="Block")]
public class BlockDefinition : ScriptableObject
{
    public int Money;
    public float Fuel;
    public bool Impassable;

    public Sprite DamagedSprite;
    public List<GameObject> SpawnOnDeath = new List<GameObject>();
}
