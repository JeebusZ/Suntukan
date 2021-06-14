using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName ="Hero", menuName ="New Hero")]
public class Hero : ScriptableObject
{
    public string Name;
    public string Description;
    public float Health;
    public float Stamina;
    public float Armor;
    public float Damage;
}
