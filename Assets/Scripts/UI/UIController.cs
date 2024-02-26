using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public enum GameStates { Running, Paused }


public class UIController : MonoBehaviour
{
    [SerializeField] InteractableUI interactableUI;

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
        Pause(false);
    }

    public void OnDisable()
    {

        Inputs.Instance.Controls.Player.Esc.started -= Pause;
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

    public void UpdateShownItemsUI(List<string> names,bool resetList = false)
	{
		interactableUI.UpdateItems(names,resetList);
	}
	
	public void AddPickedUp(string name)
	{
		interactableUI.AddPickedUp(name);
	}

}
