using TMPro;
using UnityEngine;

public class Loading : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private float loadingSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _assignLoadingText();
        _loadGameScene();
    }

    private void _assignLoadingText()
    {
        if (loadingText == null)
        {
            Debug.LogError("Loading Text is not assigned in the inspector.");
            return;
        }

        //asign application version to the loading text
        string version = Application.version;
        loadingText.text = $"v{version}";
    }
    //load game scene with coroutine after a delay
    private void _loadGameScene()
    {
        StartCoroutine(LoadGameSceneCoroutine());
    }
    private System.Collections.IEnumerator LoadGameSceneCoroutine()
    {
        // Simulate loading delay
        yield return new WaitForSeconds(loadingSpeed);

        // Load the game scene (assuming the scene index is 1)
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
