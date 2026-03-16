using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class MissileLaunchScript : MonoBehaviour
{
    [FormerlySerializedAs("misslePrefab")] public GameObject missilePrefab;
    public GameObject boss;

    private void OnEnable()
    {
        // Spawn one wave of missiles, then deactivate.
        // BossScript controls when to reactivate for the next wave.
        SpawnMissiles();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void SpawnMissiles()
    {
        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < 6; j++)
            {
                StartCoroutine(SpawnMissileWithDelay(i, j));
            }
        }

        // Deactivate after last coroutine delay finishes
        StartCoroutine(DeactivateAfterSpawn());
    }

    private IEnumerator DeactivateAfterSpawn()
    {
        // Wait for all missiles to finish spawning (max delay: 1*0.1 + 5*0.1 = 0.6s)
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false);
    }

    private IEnumerator SpawnMissileWithDelay(int i, int j)
    {
        yield return new WaitForSeconds(i * 0.1f + j * 0.1f);

        var spreadAngle = (i * 6 + j - 5.5f) * 10f;
        Instantiate(missilePrefab,
            new Vector3(
                gameObject.transform.position.x + (i * 0.5f) - 1,
                gameObject.transform.position.y - (j * 0.5f) + 1,
                gameObject.transform.position.z),
            Quaternion.Euler(0, 0, 180f + spreadAngle));
    }
}
