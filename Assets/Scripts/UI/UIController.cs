using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.Inputs;

public enum GameStates { Running, Paused }

public class UIController : MonoBehaviour
{
    [SerializeField] InteractableUI interactableUI;
    [SerializeField] TransitionScreen transitionScreen;
    [SerializeField] DeathScreenController deathScreen;

	public static UIController Instance { get; private set; }

    [SerializeField] PauseController pauseScreen;

    private void Start()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

    }
    public void OnEnable()
    {
        Inputs.Instance.Controls.Player.Esc.started += Pause;
        TransitionScreen.AnimationComplete += TransitionComplete;
        Pause(false);
    }

    public void OnDisable()
    {

        Inputs.Instance.Controls.Player.Esc.started -= Pause;
        TransitionScreen.AnimationComplete -= TransitionComplete;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        // Player can not toggle pause when dead
        //if (playerStats.IsDead) return;

        bool doPause = GameState.state == GameStates.Running;
        Pause(doPause);
        pauseScreen.SetActive(doPause);
    }

    public void Pause(bool pause = true)
    {
        GameState.state = pause ? GameStates.Paused : GameStates.Running;
        Debug.Log("Gamestate set to " + GameState.state);
        Time.timeScale = pause ? 0f : 1f;
    }

    public void UpdateShownItemsUI(List<ItemData> data,bool resetList = false)
	{
		interactableUI.UpdateItems(data,resetList);
	}
	
	public void AddPickedUp(ItemData data)
	{
		interactableUI.AddPickedUp(data);
	}

    public void ShowDeathScreen()
	{

        // Transition to Dark
		transitionScreen.Darken();
        open = UIActions.DeathScreen;
	}

    private UIActions open = UIActions.None;
    //private UIActions close = UIActions.None;

    public enum UIActions {None,DeathScreen }

    public void TransitionComplete()
	{
        Debug.Log("Transition Complete");        
        switch (open)
        {
            case UIActions.None:
                break;
            case UIActions.DeathScreen:
                deathScreen.Show();
                break;
        }
        open = UIActions.None;
        /*
        switch (close)
        {
            case UIActions.None:
                break;
            case UIActions.DeathScreen:
                deathScreen.Hide();
                break;
        }
        close = UIActions.None;
        */
    }


}
