using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
  public static CombatManager Instance { get; private set; }
  public CommandInvoker Invoker;

  private Player _playerData;
  public List<Piece> PlayerPieces = new List<Piece>();
  public List<Piece> PlayerOnBoardPieces = new List<Piece>();
  public List<Piece> PlayerOffBoardPieces = new List<Piece>();

  public List<Piece> EnemyPieces = new List<Piece>();

  public ICombatState CurrentState { get; private set; }
  // Event system for when the current state changes
  public delegate void CombatStateChangedHandler(ICombatState newState);
  public static event CombatStateChangedHandler OnCombatStateChanged;

  private void Start()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else if (Instance != this)
    {
      Destroy(gameObject); // Destroy any duplicate instances.
    }
    _playerData = DataManager.Instance.Player;

    // Instantiate all player pieces and put them into the faraway graveyard cell
    foreach (var character in _playerData.ActiveCharacters)
    {
      Piece piece = CharacterFactory.Instance.CreatePiece(character);

      piece.CellUnderPiece = BoardManager.Graveyard;
      PlayerPieces.Add(piece);
      PlayerOffBoardPieces.Add(piece);
    }

    this.Invoker = new CommandInvoker();
    // Set the state to the initial state
    ChangeState(new InitCombatState());
  }

  private void Update()
  {
    CurrentState.UpdateState();
  }

  public void ChangeState(ICombatState newState)
  {
    CurrentState?.ExitState();
    CurrentState = newState;
    // Notify that the state has changed
    OnCombatStateChanged?.Invoke(newState);
    CurrentState.EnterState();
  }
}