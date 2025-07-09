using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class FooterCanvas : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup _HLGButtons;

    [SerializeField] private Button[] _buttons;

    [SerializeField] private GameObject[] _modalWindows;


    [SerializeField] private Sprite _basicBackgroundSprite;
    [SerializeField] private Sprite _selectedBackgroundSprite;
    [SerializeField] private Sprite _shopBackgroundSprite;

    [SerializeField] private int _selectedButtonIndex = -1;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AdjustFooterSize();
        SetActiveOffOpenedModalWindows();
    }

    //function, if width of the screen is less than 1080, then set the footer to be 80% width and height 50px
    public void AdjustFooterSize()
    {
        if (Screen.width < 1080)
        {
            // rectTransform.sizeDelta = new Vector2(Screen.width * 0.8f, 50);
            _HLGButtons.spacing = 0;
            DebugLogger.Log("FooterCanvas: Changing Spacing to 0");
        }
        else
        {
            DebugLogger.Log("FooterCanvas: All Good");
        }
    }
    //SET ACTIVE OFF OPENED MODAL WINDOWS
    public void SetActiveOffOpenedModalWindows()
    {
        for (int i = 0; i < _modalWindows.Length; i++)
        {
            if (_modalWindows[i] != null)
            {
                _modalWindows[i].SetActive(false);
                DebugLogger.Log("FooterCanvas: SetActiveOffOpenedModalWindows: " + _modalWindows[i].name);
            }
        }
    }

    //FROM BUTTONS
    public void SetSelectedButton(int index)
    {
        if (index < 0 || index >= _buttons.Length)
        {
            DebugLogger.LogError("FooterCanvas: Invalid button index");
            return;
        }
        if (_selectedButtonIndex == index)
        {
            DebugLogger.Log("FooterCanvas: Button already selected!");
            return;
        }
        // Reset the previously selected button's background
        //iterate through all buttons and set the background to basicBackgroundSprite
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (i == 2) _buttons[i].GetComponent<Image>().sprite = _shopBackgroundSprite;
            else _buttons[i].GetComponent<Image>().sprite = _basicBackgroundSprite;
            if (_modalWindows[i] != null)
            {
                _modalWindows[i].SetActive(false);

            }
        }

        _selectedButtonIndex = index;
        _buttons[_selectedButtonIndex].GetComponent<Image>().sprite = _selectedBackgroundSprite;
        //open modal window corresponding to the selected button
        _modalWindows[_selectedButtonIndex].SetActive(true);
    }

    public void CloseWindow()
    {
        if (_selectedButtonIndex < 0 || _selectedButtonIndex >= _buttons.Length)
        {
            DebugLogger.LogError("FooterCanvas: Invalid selected button index");
            return;
        }
        //close all modal windows
        for (int i = 0; i < _modalWindows.Length; i++)
        {
            if (_modalWindows[_selectedButtonIndex] != null)
            {
                _modalWindows[_selectedButtonIndex].SetActive(false);
            }
        }
        if (_selectedButtonIndex == 2) // If the shop button was selected, reset its background
            _buttons[_selectedButtonIndex].GetComponent<Image>().sprite = _shopBackgroundSprite;
        else _buttons[_selectedButtonIndex].GetComponent<Image>().sprite = _basicBackgroundSprite;
        _selectedButtonIndex = -1; // Reset selected button index
    }

}
