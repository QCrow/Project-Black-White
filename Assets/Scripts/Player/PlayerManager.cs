using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
  public static PlayerManager Instance { get; private set; }
  public List<CharacterDataSO> Characters;
  public Player Player;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }

    Player = new Player(Characters, Characters);
  }
}