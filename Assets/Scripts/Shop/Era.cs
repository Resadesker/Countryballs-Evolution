using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Era")]
public class Era : ScriptableObject
{
      public string _name;
      
      public BuildingShopSlot[] _buildingShopSlots;
      
      // TODO: Change this approach! Next era shouldn't be mentioned in this era!
      public Sprite nextAgeAvatar;
}