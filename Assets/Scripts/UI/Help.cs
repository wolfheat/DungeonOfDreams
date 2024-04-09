using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Help : MonoBehaviour
{

    [SerializeField] PickUpController pickUpController;
    [SerializeField] TextMeshProUGUI wall;
    [SerializeField] TextMeshProUGUI enemy;
    [SerializeField] TextMeshProUGUI mock;

    // Update is called once per frame
    void Update()
    {
        wall.text = pickUpController.Wall?.name;
        enemy.text = pickUpController.Enemy?.name;
        mock.text = pickUpController.Mockup?.name;
    }
}
