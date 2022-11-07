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
    [SerializeField] private List<Button> giftItembuttons = new List<Button>();
    private int _numberOfPicksAvailable;

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
        foreach (var item in giftItembuttons)
            item.interactable = true;
    }

    void UpdatePicksUIText(int val = 0)
    {
        _numberOfPicksAvailable = PlayerPrefs.GetInt("Picks") + val;
        PlayerPrefs.SetInt("Picks", _numberOfPicksAvailable);
        picksInfoText.text = $"You have {_numberOfPicksAvailable} picks!";

        ScreenController.Instance.freePicksTimer = _numberOfPicksAvailable.Equals(0);
        ScreenController.Instance.TimeLeft = 60;
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
    /// Instantiate and set gift item variables
    /// </summary>
    /// <param name="parent"></param>
    public void OpenGift(Transform parent)
    {
        //return if no picks available
        if (_numberOfPicksAvailable <= 0) return;

        //decrement count after every pick
        //_numberOfPicksAvailable -= 1;

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

    /// <summary>
    /// remove all the instantiate items when changing screens
    /// </summary>
    private void ResetGiftsList()
    {
        foreach (var item in _giftItemsList)
        {
            Destroy(item);
        }

        _giftItemsList.Clear();
    }
}
