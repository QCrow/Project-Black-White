using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
  public static CombatManager Instance { get; private set; }
  public CommandInvoker Invoker;
  public PlayerSetupSO PlayerSetup;
  // TODO: Enemy setup data should be stored here
  // public EnemySetupSO EnemySetup;

  public List<Piece> PlayerPieces = new List<Piece>();
  public List<Piece> AlivePieces = new List<Piece>();

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

    // Instantiate all player pieces and put them into the faraway graveyard cell
    foreach (var prefab in PlayerSetup.Pieces)
    {
      GameObject piece = Instantiate(prefab);
      Piece pieceScript = piece.GetComponent<Piece>();
      pieceScript.CellUnderPiece = BoardManager.Graveyard;

      PlayerPieces.Add(pieceScript);
    }

    this.Invoker = new CommandInvoker();
    // Set the state to the initial state
    ChangeState(new InitState());
  }

  private void Update()
  {
    CurrentState.UpdateState(this);
  }

  public void ChangeState(ICombatState newState)
  {
    CurrentState?.ExitState(this);
    CurrentState = newState;
    // Notify that the state has changed
    OnCombatStateChanged?.Invoke(newState);
    CurrentState.EnterState(this);
  }
}