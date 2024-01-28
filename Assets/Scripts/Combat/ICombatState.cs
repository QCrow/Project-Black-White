using System.Collections.Generic;

public interface ICombatState
{
  void EnterState(CombatManager manager);
  void UpdateState(CombatManager manager);
  void ExitState(CombatManager manager);
}