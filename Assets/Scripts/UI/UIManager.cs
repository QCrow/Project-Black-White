using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  [SerializeField] private Button _confirmButton;
  [SerializeField] private Button _baseAttackButton;
  [SerializeField] private Button _skillButton;
  public static UIManager Instance { get; private set; }

  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public void SetConfirmButtonInteractable(bool isInteractable)
  {
    if (_confirmButton != null)
    {
      _confirmButton.interactable = isInteractable;
    }
  }

  public void SetConfirmButtonActive(bool isActive)
  {
    if (_confirmButton != null)
    {
      _confirmButton.gameObject.SetActive(isActive);
    }
  }

  public void OnConfirmButtonClick()
  {
    if (CombatManager.Instance.CurrentState is SetupCombatState)
    {
      SetupCombatState state = (SetupCombatState)CombatManager.Instance.CurrentState;
      state.ConfirmedForNextState = true;
    }
  }

  public void SetActionButtonsInteractable(bool isInteractable)
  {
    if (_baseAttackButton != null)
    {
      _baseAttackButton.interactable = isInteractable;
    }
    if (_skillButton != null)
    {
      _skillButton.interactable = isInteractable;
    }
  }

  public void SetAttackButtonInteractable(bool isInteractable)
  {
    if (_baseAttackButton != null)
    {
      _baseAttackButton.interactable = isInteractable;
    }
  }

  public void SetSkillButtonInteractable(bool isInteractable)
  {
    if (_skillButton != null)
    {
      _skillButton.interactable = isInteractable;
    }
  }

  public void OnAttackButtonClick()
  {
    if (CombatManager.Instance.CurrentState is PlayerCombatState)
    {
      PlayerCombatState state = (PlayerCombatState)CombatManager.Instance.CurrentState;
      state.ActionSelectionState = ActionSelectionState.Attack;
    }
  }

  public void OnSkillButtonClick()
  {
    if (CombatManager.Instance.CurrentState is PlayerCombatState)
    {
      PlayerCombatState state = (PlayerCombatState)CombatManager.Instance.CurrentState;
      state.ActionSelectionState = ActionSelectionState.Skill;
    }
  }
}