using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void CellSelectedHandler(Cell cell);
    public static event CellSelectedHandler OnCellSelected;

    public delegate void CellHoveredHandler(Cell cell);
    public static event CellHoveredHandler OnCellHovered;

    private Cell _lastHoveredCell;
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy any duplicate instances.
        }
    }

    void Update()
    {
        if (CombatManager.Instance.CurrentState is PlayerCombatState)
        {
            Cell cell = FindCellUnderMouse();

            // Check if the cell under the mouse has changed
            if (cell != null && cell != _lastHoveredCell)
            {
                // Revert the action for the previously hovered cell
                if (_lastHoveredCell != null)
                {
                    OnCellHovered?.Invoke(_lastHoveredCell);
                }

                // Fire the action for the current hovered cell
                OnCellHovered?.Invoke(cell);

                // Update the last hovered cell
                _lastHoveredCell = cell;
            }
            else if (cell == null && _lastHoveredCell != null)
            {
                // Revert the action when no cell is being hovered
                OnCellHovered?.Invoke(_lastHoveredCell);
                _lastHoveredCell = null;
            }
        }

        // Check for actions related to left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            Cell cell = FindCellUnderMouse();

            if (cell == null) return;

            OnCellSelected?.Invoke(cell);
        }
    }

    private Cell FindCellUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        RaycastHit closestHit = new RaycastHit();
        float closestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Cell"))
            {
                float distance = Vector3.Distance(Camera.main.transform.position, hit.point);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHit = hit;
                }
            }
        }
        Cell cell = closestHit.collider?.gameObject.GetComponent<Cell>();

        return cell;
    }
}