using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallBehavior : MonoBehaviour
{
    Rigidbody Ball;
    Vector3 mouseUpPos;
    Vector3 mouseDownPos;
    Vector3 velocity;
    public bool Scored;
    public bool ballKicked = false;
    public int goals;
    public float gravity;
    private float maxF = 40.0f;
    private float mouseNotPressed = 0;
    private float mousePressed = 0;
    Vector3 initialPos;
    [SerializeField] GameObject shadow;
    // Start is called before the first frame update
    void Start()
    {
        gravity = -9.81f;
        Scored = false;
        goals = 0;
        Ball = GetComponent<Rigidbody>();
        Ball.useGravity = false;
        StartCoroutine(initicalPosition());
    }
    // The ball gets kicked taking the direction and the time it was held down to calculate the strenght of the kick
    public void apllyForce(Vector3 direction, float lvelocity)
    {
        Ball.velocity = (direction * lvelocity);
        ballKicked = true;
    }

    private float calculateVelocity(float Time)
    {
        float maxHoldTime = 2.0f;
        float normalizedHoldTime = Mathf.Clamp01(Time / maxHoldTime);
        float velocity = normalizedHoldTime * maxF;

        return velocity;
    }

    private Vector3 CalculateNormalizedDirection()
    {
        Vector3 temp;
        temp = (mouseDownPos - mouseUpPos).normalized;

        return temp;
    }
    // Update is called once per frame
    void Update()
    {
        if (Ball.freezeRotation)
            Ball.freezeRotation = false;


        mousePressed = Time.time - mouseNotPressed;

        if (Input.GetMouseButtonDown(0) && !ballKicked)
        {
            mouseDownPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100);
            mouseNotPressed = Time.time;
        }

        if (Input.GetMouseButtonUp(0) && !ballKicked)
        {
            mouseUpPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            velocity = CalculateNormalizedDirection() * calculateVelocity(mousePressed);
            apllyForce(CalculateNormalizedDirection(), calculateVelocity(mousePressed));
            StartCoroutine(resetGame());
        }
    }
    // Provides vertical force to the ball when kicked
    private void FixedUpdate()
    {
        Ball.AddForce(gravity * new Vector3(0.0f, 0.89f, 0.0f), ForceMode.Acceleration);
    }
    // Ball collision with the net will add a point to the score
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Net")
        {
            Scored = true;
        }
    }

    IEnumerator resetGame()
    {
        yield return new WaitForSeconds(3);
        if (Scored)
        {
            goals++;
        }
        Ball.transform.position = new Vector3(0.0f, 1.0f, -5.27f);
        Ball.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        Ball.freezeRotation = true;

        ballKicked = false;
        Scored = false;
    }

    IEnumerator initialPosition()
    {
        yield return new WaitForSeconds(0.8);
        initialPos = Ball.position;
    }
    // UI calculations
    public float calcVelY()
    {
        return Mathf.Round(velocity.y);
    }
    public float calcVelX()
    {
        return Mathf.Round(Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z));
    }
    public float calcRange()
    {
        float temp;
        float time = -velocity.y * 2.0f / gravity;
        temp = calcVelX() * time;
        return temp;
    }
    public float calcAngle()
    {
        float temp;
        float Hipoth = Mathf.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z);
        float alphaR = Mathf.Asin(velocity.y / Hipoth);
        temp = alphaR;
        return temp;
    }
}
