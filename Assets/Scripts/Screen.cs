using UnityEngine;

public class Screen : MonoBehaviour
{
    [SerializeField] internal ThisScreen screenId;
    [SerializeField] internal GameObject screenUIObject;
    [SerializeField] internal ThisScreen prevScreenId;

    /// <summary>
    /// Enable Screen UI Objects
    /// </summary>
    internal void EnableScreen() => screenUIObject.SetActive(true);

    /// <summary>
    /// Disable Screen UI Objects
    /// </summary>
    internal void DisableScreen() => screenUIObject.SetActive(false);
}
