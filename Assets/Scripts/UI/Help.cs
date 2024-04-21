using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Help : MonoBehaviour
{

    [SerializeField] PickUpController pickUpController;
    [SerializeField] PlayerController player;
    [SerializeField] TextMeshProUGUI wall;
    [SerializeField] TextMeshProUGUI enemy;
    [SerializeField] TextMeshProUGUI mock;
    [SerializeField] TextMeshProUGUI playerPos;

    // Update is called once per frame
    void Update()
    {
        wall.text = pickUpController.Wall?.name;
        enemy.text = pickUpController.Enemy?.name;
        mock.text = pickUpController.Mockup?.name;
        playerPos.text = player?.transform.position.ToString() ?? "";
    }
}
