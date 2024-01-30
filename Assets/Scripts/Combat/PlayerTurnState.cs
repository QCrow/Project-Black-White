using System;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class PlayerTurnState : ICombatState
{
  public void EnterState()
  {
    RefreshPieces();
    DecreaseRedeployTimers();
    InputManager.OnCellSelected += HandleCellSelected;
  }

  public void UpdateState()
  {

  }

  public void ExitState()
  {
    InputManager.OnCellSelected -= HandleCellSelected;
  }

  private void HandleCellSelected(Cell cell)
  {
    if (cell.PieceOnCell != null)
    {
      // TODO: Show possible actions for the selected piece

    }
  }


  private void RefreshPieces()
  {
    foreach (var piece in CombatManager.Instance.PlayerOnBoardPieces)
    {
      if (piece.IsShadowed == piece.CellUnderPiece.IsShadowed)
      {
        piece.VeiledRefreshActions();
      }
      else
      {
        piece.UnveiledRefreshActions();
      }
    }
  }

  private void DecreaseRedeployTimers()
  {
    foreach (var piece in CombatManager.Instance.PlayerOffBoardPieces)
    {
      if (piece.RedeployTimer > 0)
      {
        piece.RedeployTimer--;
      }
    }
  }
}