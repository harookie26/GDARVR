using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VRCooking/BurgerSet")]
public class BurgerSet : ScriptableObject
{
    public BurgerRecipe recipe;
    public BurgerSlotOrder slotOrder;
}
