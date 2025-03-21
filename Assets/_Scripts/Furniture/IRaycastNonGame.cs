using UnityEngine;

public interface IRaycastNonGame
{
    void OnClicked(Transform player);
    void OnStartLooking();
    void OnEndLooking();
}