using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    private bool isPaused = false;
    private float oldTimeScale = 1;
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            oldTimeScale = Time.timeScale;
            Time.timeScale = 0;
            canvas.enabled = true;
        }
        else
        {
            Time.timeScale = oldTimeScale;
            canvas.enabled = false;
        }
    }

    public void Quit()
    {
        InGameManager.instance.Quit();
    }
}
