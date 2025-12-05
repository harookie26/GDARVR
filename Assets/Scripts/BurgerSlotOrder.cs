using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VRCooking/BurgerSlotOrder")]
public class BurgerSlotOrder : ScriptableObject
{
    [Tooltip("List of slot indexes in bottom-to-top order (0 = bottom slot).")]
    public List<int> slotIndex = new List<int>();
}
