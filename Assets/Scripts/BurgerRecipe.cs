using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VRCooking/BurgerRecipe")]
public class BurgerRecipe : ScriptableObject
{
    [Tooltip("List of piece IDs in bottom-to-top order (0 = bottom slot).")]
    public List<string> pieceIds = new List<string>();
}
