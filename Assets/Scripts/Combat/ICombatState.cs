using System.Collections.Generic;

public interface ICombatState
{
  void EnterState();
  void UpdateState();
  void ExitState();
}