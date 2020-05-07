using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.UI;

public class PenguinArea : MonoBehaviour
{
    public PenguinAgent penguinAgent;

    public GameObject penguinBaby;

    public Text rewardText;

    public Fish fishPrefab;

    private List<GameObject> fishes = new List<GameObject>();

    private void Start()
    {
        ResetArea();
    }

    private void Update()
    {
        rewardText.text = penguinAgent.GetCumulativeReward().ToString("F2");
    }

    public void ResetArea()
    {
        RemoveAllFish();
        PlacePenguin();
        PlaceBaby();
        SpawnFish(4, Academy.Instance.FloatProperties.GetPropertyWithDefault("fish_speed", 0.5f));
        
    }

    public void RemoveSpecificFish(GameObject fish)
    {
        fishes.Remove(fish);
        Destroy(fish);
    }

    public int FishRemaining ()
    {
        return fishes.Count;
    }

    public static Vector3 ChooseRandomPosition (Vector3 centre, float minAngle, float maxAngle, float minRadius, float maxRadius)
    {
        float radius = minRadius;
        float angle = minAngle;

        if (maxRadius > minRadius)
        {
            radius = Random.Range(minRadius, maxRadius);
        }

        if (maxAngle > minAngle)
        {
            angle = Random.Range(minAngle,maxAngle);
        }

        return centre + Quaternion.Euler(0.0f, angle, 0.0f) * Vector3.forward * radius;
    }

    private void RemoveAllFish()
    {
        if (fishes != null)
        {
            foreach (GameObject fish in fishes)
            {
                Destroy(fish);
            }
        }

        fishes.Clear();
    }

    private void PlacePenguin()
    {
        Rigidbody rigidbody = penguinAgent.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        penguinAgent.transform.position = ChooseRandomPosition(transform.position, 0.0f ,360.0f, 0.0f, 9.0f) + Vector3.up * 0.5f;
        penguinAgent.transform.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
    }


    private void PlaceBaby()
    {
        Rigidbody rigidbody = penguinBaby.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        penguinBaby.transform.position = ChooseRandomPosition(transform.position, -45f, 45f, 4f, 9f) + Vector3.up * .5f;
        penguinBaby.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    private void SpawnFish(int count, float fishSpeed)
    {
        for (int i = 0; i < count; i++)
        {
            Fish fish = Instantiate(
                fishPrefab,
                ChooseRandomPosition(transform.position, 100.0f, 260.0f, 2.0f, 13.0f) + Vector3.up * 0.5f,
                Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f) , 0.0f)
                );

            fish.transform.SetParent(transform);

            fishes.Add(fish.gameObject);

            fish.fishSpeed = fishSpeed;
        }
    }
}
