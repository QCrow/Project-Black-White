using UnityEngine;

public class PlayerTurnState : ICombatState
{

  public void EnterState(CombatManager manager)
  {
    Debug.Log("Entering Player Turn State");
    RefreshPieces(manager);
  }

  public void ExitState(CombatManager manager)
  {
    throw new System.NotImplementedException();
  }

  public void UpdateState(CombatManager manager)
  {
    throw new System.NotImplementedException();
  }

  private void RefreshPieces(CombatManager manager)
  {
    manager.PlayerOnBoardPieces.ForEach(piece => piece.RefreshActions());
  }
}