using System;
using UnityEngine;

public class ObjectAnimator : MonoBehaviour
{
    const float RotationSpeed = 90f;

    [SerializeField] float animationVelocity = 0.005f;
    float animationAccelerationDrop = 3f;
    float animationAccelerationStill = 0.1f;
    float animationAcceleration = 0.1f;
    float equilibriumVelocity = -0.1f;
    int dir = 1;
    float equilibriumY = -0.3f;
    [SerializeField] float equilibriumTarget = -0.3f;
    Vector3 equilibriumVector;
    bool atEquilibrium = false;

    float hoverTimer = 0;
    float speed = 0.8f;
    float amplitude = 0.08f;
    float loopTime;

    // Object hovers and rotates
    public void Reset()
    {
        dir = 0;
        loopTime = 6.28319f / speed;
        equilibriumY = equilibriumTarget;
        animationAcceleration = animationAccelerationDrop;
        equilibriumVector = new Vector3(transform.position.x,equilibriumTarget,transform.position.z);
        atEquilibrium = false;
        dir = 1;
        animationVelocity = 0.005f;
        firstDone = false;
    }
    // Object hovers and rotates
    private void Start()
    {
        Reset();
    }

    bool firstDone = false;
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, RotationSpeed * Time.deltaTime);
        transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
        if(!firstDone)
            VelocityHover();
        else
            SinusHover();
        

    }

    private void SinusHover()
    {        
        hoverTimer += Time.deltaTime;
        if(hoverTimer> loopTime) hoverTimer -= loopTime;

        // At Equilibrium
        float newY = Mathf.Sin(hoverTimer * speed) * amplitude;
        Vector3 newPos = equilibriumVector + Vector3.down*newY;
        transform.position = newPos;

        hoverTimer += Time.deltaTime;
    }

    private void VelocityHover()
    {
        // gravity drop to equilibrium
        if (transform.position.y <= equilibriumTarget && !atEquilibrium)
        {
            animationAcceleration = animationAccelerationStill;
            animationVelocity = equilibriumVelocity;
            atEquilibrium = true;
        }

        dir = (transform.position.y > equilibriumTarget) ? -1 : 1;
        animationVelocity += animationAcceleration * dir * Time.deltaTime;
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + animationVelocity * Time.deltaTime, transform.position.z);
        if(transform.position.y < equilibriumY && newPos.y>= equilibriumY)
        {
            firstDone = true;
            hoverTimer = loopTime/2;
        }
        transform.position = newPos;

    }
}
