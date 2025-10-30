using UnityEngine;
using FronkonGames.Glitches.Artifacts;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private static GameManager instance;    // Singleton instance of GameManager
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private Artifacts.Settings ArtifactsSettings; // Reference to the settings for the monster

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // Set the singleton instance
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
    }

    private void OnEnable()
    {
        ReSetArtifacts();
    }

    public void ReSetArtifacts()
    {
        if (ArtifactsSettings == null)
        {
            ArtifactsSettings = Artifacts.Instance.settings; // Get the Artifacts settings instance
        }

        ArtifactsSettings.intensity = 0.0f; // Reset the artifacts intensity to 0
    }

    public void SetArtifacts(float intensity)
    {
        if (ArtifactsSettings == null)
        {
            ArtifactsSettings = Artifacts.Instance.settings; // Get the Artifacts settings instance
        }

        ArtifactsSettings.intensity = intensity;
    }

    public void LoadNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // Load the specified scene
    }
}
