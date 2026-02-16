using UnityEngine;

public class SpawnCollectibles : MonoBehaviour
{
    public GameObject prefab;
    public int amount = 20;

    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-50f, 50f),
                0.5f,
                Random.Range(-50f, 50f)
            );
            Instantiate(prefab, pos, Quaternion.identity, transform);
        }
    }
}
