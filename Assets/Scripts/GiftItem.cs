using System;
using TMPro;
using UnityEngine;

public class GiftItem : MonoBehaviour
{
    public TextMeshProUGUI textBox;

    internal GameController.Gift Gift;
    internal Action Post;

    /// <summary>
    /// schedule delayed invoke for reveling gift item info
    /// </summary>
    internal void Schedule() => Invoke(nameof(ViewGiftItem), 1f);

    /// <summary>
    /// invoked with delay, creates pop effect for gift item while reveling the info
    /// </summary>
    private void ViewGiftItem()
    {
        GetComponent<AudioSource>().Play();

        transform.localScale = Vector3.one * .9f;
        textBox.text = $"{Gift.prize}\n{Gift.val}";
        if (Gift.prize.Equals("Picks"))
        {
            GameController.UpdatePicksUI?.Invoke(Gift.val);
        }

        Invoke(nameof(ReEnableEventSystem), 1f);
    }

    /// <summary>
    /// re-enable eventsystem to avail input system
    /// </summary>
    internal void ReEnableEventSystem() => Post?.Invoke();
}
