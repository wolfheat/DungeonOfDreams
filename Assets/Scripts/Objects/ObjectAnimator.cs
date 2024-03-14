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
    float equilibriumY;
    float equilibriumTarget = -0.3f;
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
        atEquilibrium = false;
        dir = 1;
        animationVelocity = 0.005f;
    }
    // Object hovers and rotates
    private void Start()
    {
        equilibriumY = equilibriumTarget;
        //equilibriumVector = new Vector3(transform.position.x,equilibriumTarget,transform.position.z);
        animationAcceleration = animationAccelerationDrop;
        //equilibriumY = transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {

        transform.RotateAround(transform.position, Vector3.up, RotationSpeed * Time.deltaTime);
        transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);

        VelocityHover();
        //SinusHover();
        

    }

    private void SinusHover()
    {        
        hoverTimer += Time.deltaTime;
        if(hoverTimer> loopTime) hoverTimer -= loopTime;

        // gravity drop to equilibrium
        if (!atEquilibrium)
        {
            if (transform.position.y > equilibriumTarget)
            {
                animationVelocity -= animationAcceleration * Time.deltaTime;
                transform.position = new Vector3(transform.position.x, transform.position.y + animationVelocity * Time.deltaTime, transform.position.z);
                return;
            }
            else
            {
                equilibriumVector = new Vector3(transform.position.x, equilibriumTarget, transform.position.z);
                atEquilibrium = true;
            }
        }

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
        transform.position = newPos;

    }
}
