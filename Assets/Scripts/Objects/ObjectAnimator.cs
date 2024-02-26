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
    bool atEquilibrium = false;


    // Object hovers and rotates
    private void Start()
    {
        equilibriumY = equilibriumTarget;
        animationAcceleration = animationAccelerationDrop;
        //equilibriumY = transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {
        // gravity drop to equilibrium
        if (transform.position.y <= equilibriumTarget && !atEquilibrium)
        {
            animationAcceleration = animationAccelerationStill;
            animationVelocity = equilibriumVelocity;
            atEquilibrium = true;
        }



        
        transform.RotateAround(transform.position, Vector3.up, RotationSpeed * Time.deltaTime);
        transform.Rotate(0, RotationSpeed*Time.deltaTime, 0);
        

        dir = (transform.position.y > equilibriumTarget) ? -1 :1;
        animationVelocity += animationAcceleration * dir * Time.deltaTime;   

        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + animationVelocity * Time.deltaTime, transform.position.z);
        transform.position = newPos;
    }
}
