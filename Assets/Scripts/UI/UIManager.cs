using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  public Button confirmButton;

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
    if (confirmButton != null)
    {
      confirmButton.interactable = isInteractable;
    }
  }

  public void OnConfirmButtonClick()
  {
    if (CombatManager.Instance.CurrentState is SetupState)
    {
      SetupState state = (SetupState)CombatManager.Instance.CurrentState;
      state.ConfirmedForNextState = true;
    }
  }
}