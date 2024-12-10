using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public void LoadLevel(int levelIndex)
    {
        // Load the scene by build index
        SceneManager.LoadScene(levelIndex);
    }
}