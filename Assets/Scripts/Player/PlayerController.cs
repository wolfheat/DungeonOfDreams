using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public enum MoveActionType{Move,Rotate}
public class MoveAction
{
    public MoveActionType moveType;
    public int motionX = 0;
    public int motionY = 0;
    public MoveAction(MoveActionType t, int m, int m2 = 0)
    {
        moveType = t;
        motionX = m;
        motionY = m2;
    }
}

public class PlayerController : MonoBehaviour
{
    public bool DoingAction { get; set; } = false;
    private MoveAction savedAction = null;

    private float timer = 0;
    private const float MoveTime = 0.2f;
    private void Start()
    {
        Inputs.Instance.Controls.Player.Move.performed += MovePerformed;
        Inputs.Instance.Controls.Player.Move.canceled += MoveCanceled;
        Inputs.Instance.Controls.Player.Turn.performed += TurnPerformed;
    }

    private void MoveCanceled(InputAction.CallbackContext obj)
    {

    }

    private void Update()
    {
        if(DoingAction) return;

        if (savedAction != null)
        {
            if (savedAction.moveType == MoveActionType.Move)
                StartCoroutine(Move(EndPositionForMotion(savedAction)));
            else if (savedAction.moveType == MoveActionType.Rotate)
                StartCoroutine(Rotate(EndRotationForMotion(savedAction)));
            savedAction = null;
        }
    }

    private void TurnPerformed(InputAction.CallbackContext obj)
    {
        TurnPerformed();
    }
    private void TurnPerformed()
    {
        float movement = Inputs.Instance.Controls.Player.Turn.ReadValue<float>();
        if (movement == 0) return;

        MoveAction moveAction = new MoveAction(MoveActionType.Rotate, (int)movement);
        savedAction = moveAction;
    }

    private void MovePerformed(InputAction.CallbackContext obj)
    {
        MovePerformed();
    }
    private void MovePerformed()
    {
        Vector2 movement = Inputs.Instance.Controls.Player.Move.ReadValue<Vector2>();
        if (movement.magnitude == 0) return;

        MoveAction moveAction = new MoveAction(MoveActionType.Move, (int)movement.x, (int)movement.y);
        savedAction = moveAction;
    }

    private IEnumerator Move(Vector3 target)
    {
        DoingAction = true;
        Vector3 start = transform.position;
        Vector3 end = target;
        timer = 0;
        while (timer < MoveTime)
        {
            yield return null;
            transform.position = Vector3.Lerp(start,end,timer/MoveTime);
            timer += Time.deltaTime;
        }
        transform.position = target;
        DoingAction = false;
        // Check if player is holding button
        MovePerformed();
        TurnPerformed();
    }

    private Vector3 EndPositionForMotion(MoveAction motion)
    {
        if (motion.motionX != 0)
            return transform.position+ motion.motionX*transform.right;
        else
            return transform.position + motion.motionY * transform.forward;

    }

    private Quaternion EndRotationForMotion(MoveAction motion)
    {
        return Quaternion.LookRotation(transform.right * motion.motionX, Vector3.up);
    }

    private IEnumerator Rotate(Quaternion target)
    {
        DoingAction = true;
        Quaternion start = transform.rotation;
        Quaternion end = target;
        timer = 0;
        while (timer < MoveTime)
        {
            yield return null;
            transform.rotation = Quaternion.Lerp(start,end,timer/MoveTime);
            timer += Time.deltaTime;
        }
        transform.rotation = target;
        DoingAction = false;
        TurnPerformed();
        MovePerformed();
    }
}
