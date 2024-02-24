using UnityEngine;

public class ObjectAnimator : MonoBehaviour
{
    const float RotationSpeed = 90f;

    [SerializeField] float animationVelocity = 0.005f;
    [SerializeField][Range(0.01f,0.5f)] float animationAcceleration = 0.05f;
    int dir = 1;
    float equilibriumY;


    // Object hovers and rotates
    private void Start()
    {
        equilibriumY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, RotationSpeed * Time.deltaTime);
        transform.Rotate(0, RotationSpeed*Time.deltaTime, 0);

        dir = (transform.position.y > equilibriumY) ? -1 :1;
        animationVelocity += animationAcceleration * dir * Time.deltaTime;   

        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + animationVelocity * Time.deltaTime, transform.position.z);
        transform.position = newPos;
    }
}
