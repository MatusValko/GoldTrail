using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //every 9 seconds, set the value of the slider to 0f after is filled up
        _slider.value += Time.deltaTime / 9f; // Adjust the divisor to control the speed of filling
        if (_slider.value >= 1f) // Check if the slider value has reached or exceeded 1
        {
            _slider.value = 0f; // Reset the slider value to 0 when it reaches 1
                                //TODO Acquire resources or perform any action you want when the slider is full

        }

    }
}
