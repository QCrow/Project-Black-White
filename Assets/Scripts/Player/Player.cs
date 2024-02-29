using System.Collections.Generic;
using UnityEngine;

public class Player // Player run progression data
{
    public List<Character> ActiveCharacters = new List<Character>();

    public List<Character> AllCharacters = new List<Character>();

    public int Money = 0;
    public Player(List<Character> activeCharacters, List<Character> allCharacters)
    {
        ActiveCharacters = activeCharacters;
        AllCharacters = allCharacters;
        Money = 0;
    }
}