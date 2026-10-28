using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startschrem : MonoBehaviour
{
    private string gameName = "";

    public void Startgame()
    {
        SceneManager.LoadScene(gameName);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
