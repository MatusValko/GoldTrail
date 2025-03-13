using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int targetFrameRate = 120;
    public static GameManager instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        _setTargetFrameRate(targetFrameRate);
    }



    private void _setTargetFrameRate(int frameRate)
    {
        double refreshRate = Screen.currentResolution.refreshRateRatio.value;

        if (refreshRate >= frameRate)
        {
            Application.targetFrameRate = frameRate;  // Use frameRate if the device supports it
        }
        else
        {
            Application.targetFrameRate = 60;  // Use 60 FPS if it's a low-refresh-rate device
        }
    }



    // Update is called once per frame
    void Update()
    {

    }
}
