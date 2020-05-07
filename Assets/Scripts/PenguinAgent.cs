using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

[RequireComponent(typeof(Rigidbody))]
public class PenguinAgent : Agent
{
    public float movementSpeed = 5.0f;
    public float turnSpeed = 180.0f;
    public GameObject heartPrefab;
    public GameObject regurgitatedFishPrefab;

    private PenguinArea penguinArea;
    private Rigidbody rigidbody;
    private GameObject baby;
    private bool isFull;
    private float feedRadius = 0.0f;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        penguinArea = GetComponentInParent<PenguinArea>();
        baby = penguinArea.penguinBaby;
        rigidbody = GetComponent<Rigidbody>();
    }



    public override void AgentAction(float[] vectorAction)
    {
        float forwardAmount = vectorAction[0];

        float turnAmount = 0.0f;
        if (vectorAction[1] == 1.0f)
        {
            turnAmount = -1.0f;
        }
        else if (vectorAction[1] == 2.0f)
        {
            turnAmount = -1.0f;
        }

        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * movementSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        if (maxStep > 0)
        {
            AddReward(-1.0f / maxStep);
        }
    }

    

    public override float[] Heuristic()
    {
        float forwardAction = 0.0f;
        float turnAction = 0.0f;


        if (Input.GetKey(KeyCode.W))
        {
            // move forward
            forwardAction = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // turn left
            turnAction = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // turn right
            turnAction = 2f;
        }

        return new float[] { forwardAction, turnAction };

    }

    public override void AgentReset()
    {
        isFull = false;
        penguinArea.ResetArea();
        feedRadius = Academy.Instance.FloatProperties.GetPropertyWithDefault("feed_radius", 0f);
    }

    public override void CollectObservations()
    {
        // has penguin eaten yet
        AddVectorObs(isFull);
        // distance to baby from adult penguin
        AddVectorObs(Vector3.Distance(baby.transform.position, transform.position));
        // direction to baby
        AddVectorObs((baby.transform.position - transform.position).normalized);
        // adult penguins current direction
        AddVectorObs(transform.forward);

        // 8 total values
    }

    private void FixedUpdate()
    {
        if (GetStepCount() % 5 == 0)
        {
            RequestDecision();
        }
        else
        {
            RequestAction();
        }


        if (Vector3.Distance(baby.transform.position, transform.position) < feedRadius)
        {
            RegurgitateFish();
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Fish")
        {
            EatFish(collision.gameObject);
        }
        else if (collision.transform.tag == "Baby")
        {
            RegurgitateFish();
        }
    }

    void EatFish(GameObject fish)
    {
        if (isFull)
        {
            return;
        }

        isFull = true;
        penguinArea.RemoveSpecificFish(fish);

        AddReward(1.0f);

    }

    void RegurgitateFish()
    {
        if (isFull == false)
        {
            return;
        }

        isFull = false;

        GameObject regurgitatedFish = Instantiate(regurgitatedFishPrefab,baby.transform.position, Quaternion.identity);
        Destroy(regurgitatedFish,4.0f);

        GameObject heart = Instantiate(heartPrefab, baby.transform.position + Vector3.up, Quaternion.identity);
        Destroy(heart, 4.0f);

        AddReward(1.0f);

        if (penguinArea.FishRemaining() <= 0)
        {
            Done();
        }
    }
}
