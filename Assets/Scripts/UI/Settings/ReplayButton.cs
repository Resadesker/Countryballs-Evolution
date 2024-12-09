using UnityEngine;

public class ReplayButton : MonoBehaviour
{
    private void Start() {
        gameObject.SetActive(Saves.Instance.LoadExtra("gameEnded") == 1);
    }

    public void OnClick()
    {
        Saves.Instance.DropAllTables();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
