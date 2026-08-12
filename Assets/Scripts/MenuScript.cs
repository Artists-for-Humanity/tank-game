
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour
{
    [SerializeField]
    GameObject playButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playButton.GetComponent<Button>().onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        SceneManager.LoadScene(1);
    }
   
}
