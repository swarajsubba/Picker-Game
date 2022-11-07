using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [Serializable]
    public struct Gift
    {
        [SerializeField] public string prize;
        [SerializeField] public int val;
    }

    private readonly List<Gift> _giftsArray = new List<Gift>();
    private readonly List<int> _probArray = new() { 87, 10, 3, 0 };
    private readonly List<GameObject> _giftItemsList = new List<GameObject>();
    [SerializeField] private List<Button> giftItemButtons = new List<Button>();
    [SerializeField] private int numberOfPicksAvailable;

    private EventSystem _eventSystem;

    [SerializeField] private GiftItem giftItemPrefab;
    [SerializeField] private TextMeshProUGUI picksInfoText;

    public static Action<int> UpdatePicksUI;

    private void OnDisable()
    {
        UpdatePicksUI -= UpdatePicksUIText;
        ResetGiftsList();
    }

    private void OnEnable()
    {
        UpdatePicksUI += UpdatePicksUIText;
        foreach (var item in giftItemButtons)
            item.interactable = true;
    }

    private void Awake() => _eventSystem = FindObjectOfType<EventSystem>();

    /// <summary>
    /// Init with default gift items in a list
    /// </summary>
    private void Start()
    {
        _giftsArray.Add(new Gift() { prize = "-  -" });
        _giftsArray.Add(new Gift() { prize = "Coins", val = 100 });
        _giftsArray.Add(new Gift() { prize = "Picks", val = Random.Range(1, 4) });
        _giftsArray.Add(new Gift() { prize = "<3", val = Random.Range(1, 4) });

        UpdatePicksUIText();
    }

    /// <summary>
    /// Update ui elements
    /// </summary>
    /// <param name="val"></param>
    void UpdatePicksUIText(int val = 0)
    {
        numberOfPicksAvailable = PlayerPrefs.GetInt("Picks") + val;
        PlayerPrefs.SetInt("Picks", numberOfPicksAvailable);

        picksInfoText.text = $"You have {numberOfPicksAvailable} picks!";

        if (giftsOpened.Equals(8) && numberOfPicksAvailable > 0)
        {
            resetButton.SetActive(true);
            Debug.Log("Requires reset");
        }

        ScreenController.Instance.freePicksTimer = numberOfPicksAvailable.Equals(0);
        ScreenController.Instance.TimeLeft = 60;
    }

    /// <summary>
    /// Instantiate and set gift item variables
    /// </summary>
    /// <param name="parent"></param>
    public void OpenGift(Transform parent)
    {
        //return if no picks available
        if (numberOfPicksAvailable <= 0) return;

        giftsOpened++;

        //just ui text update
        UpdatePicksUIText(-1);

        var gItem = PickItem();

        var gift = Instantiate(giftItemPrefab, parent.position, Quaternion.identity);
        var transform1 = gift.transform;
        transform1.SetParent(parent);
        transform1.localScale = Vector3.one * .8f;

        ToggleEventSystem(false);
        parent.GetComponent<Button>().interactable = false;

        gift.Gift = gItem;
        gift.Post += () => ToggleEventSystem(true);
        gift.Schedule();

        _giftItemsList.Add(gift.gameObject);
    }

    /// <summary>
    /// pick item from the list of gift items
    /// </summary>
    /// <returns></returns>
    Gift PickItem()
    {
        var random = Random.Range(1, 101);

        return random switch
        {
            > 50 and <= 100 => _giftsArray[0],
            > 30 and <= 50 => _giftsArray[1],
            > 10 and <= 30 => _giftsArray[2],
            > 0 and <= 10 => _giftsArray[3],
            _ => default
        };
    }

    /// <summary>
    /// disable and enable eventsystem for blocking input while reveling the gift items
    /// </summary>
    /// <param name="b"></param>
    private void ToggleEventSystem(bool b)
    {
        _eventSystem.enabled = b;
    }

    [SerializeField] private int giftsOpened;
    [SerializeField] private GameObject resetButton;

    /// <summary>
    /// remove all the instantiate items when changing screens
    /// </summary>
    public void ResetGiftsList()
    {
        if (giftsOpened.Equals(8))
        {
            giftsOpened = 0;
            resetButton.SetActive(false);
        }

        foreach (var item in _giftItemsList)
            Destroy(item);

        _giftItemsList.Clear();

        foreach (var item in giftItemButtons)
            item.interactable = true;
    }
}
