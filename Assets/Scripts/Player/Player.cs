using System.Collections.Generic;
using UnityEngine;

public class Player
{
  public List<CharacterDataSO> ActiveCharacters = new List<CharacterDataSO>();

  public List<CharacterDataSO> AllCharacters = new List<CharacterDataSO>();

  public int Money = 0;
  public Player(List<CharacterDataSO> activeCharacters, List<CharacterDataSO> allCharacters)
  {
    ActiveCharacters = activeCharacters;
    AllCharacters = allCharacters;
    Money = 0;
  }
}