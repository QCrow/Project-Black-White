using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
  public static DataManager Instance { get; private set; }

  //! The following should be removed when there is a initial character selection
  private List<Character> Characters;
  public List<CharacterBaseDataSO> CharacterBaseDatas;
  public Player Player;


  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }

    // ! This responsibility might be moved to a different class
    Characters = CharacterBaseDatas.Select(data => new Character(data)).ToList();

    Player = new Player(Characters, Characters);
  }
}