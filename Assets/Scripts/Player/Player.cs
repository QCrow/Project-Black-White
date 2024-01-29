using System.Collections.Generic;
using UnityEngine;

public class Player
{
  public List<CharacterDataSO> ActiveCharacters = new List<CharacterDataSO>();

  public List<CharacterDataSO> AllCharacters = new List<CharacterDataSO>();

  public int Money = 0;
}