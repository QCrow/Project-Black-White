using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }
    public CommandInvoker Invoker;

    private Player _playerData;
    public List<Ally> PlayerPieces = new List<Ally>();
    public List<Ally> PlayerOnBoardPieces = new List<Ally>();
    public List<Ally> PlayerOffBoardPieces = new List<Ally>();

    public List<Enemy> EnemyPieces = new List<Enemy>();

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
            Ally playerPiece = PieceFactory.Instance.CreatePiece(character);

            playerPiece.CellUnderPiece = BoardManager.Graveyard;
            PlayerPieces.Add(playerPiece);
            PlayerOffBoardPieces.Add(playerPiece);
        }

        this.Invoker = new CommandInvoker();

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