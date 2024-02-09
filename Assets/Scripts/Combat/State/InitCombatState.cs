public class InitCombatState : ICombatState
{
    public void EnterState()
    {
        UIManager.Instance.SetActionButtonsInteractable(false);
        UIManager.Instance.SetConfirmButtonInteractable(false);

        // Initialize the game board
        BoardManager.Instance.InitializeBoard();
        // TODO: Should initialize the enemy pieces here so the player can see them before placing their own pieces

        // Init state only occurs once to initialize the board
        // Regardless of how many players are playing
        CombatManager.Instance.ChangeState(new SetupCombatState());
    }

    public void ExitState() { }

    public void UpdateState() { }
}