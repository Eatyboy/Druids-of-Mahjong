using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager instance;
    public bool isPaused = true;

    [SerializeField] private GameObject pauseMenuObject;
    private InputSystem_Actions actions;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        actions = new();
    }

    private void Start()
    {
        ResumeGame();
        pauseMenuObject.SetActive(false);
    }

    private void OnEnable()
    {
        actions.Enable();

        actions.Player.Pause.performed += (ctx) => SwitchPauseState();
    }

    private void OnDisable()
    {
        actions.Player.Pause.performed -= (ctx) => SwitchPauseState();
        
        actions.Disable();
    }

    public void SwitchPauseState()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0.0f;
        pauseMenuObject.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        pauseMenuObject.SetActive(false);
    }

    public void ReturnToMenu()
    {
        ResumeGame();
        GameManager.instance.QuitToTitleScreen();
    }

    // 
}
