using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    public void ButtonClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
