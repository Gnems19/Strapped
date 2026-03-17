using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MissileLaunchScript : MonoBehaviour
{
    [FormerlySerializedAs("misslePrefab")] public GameObject missilePrefab;
    public GameObject boss;
    [SerializeField] private float spawnDelay = 0.1f;

    private readonly List<Transform> _spawnPoints = new();

    private void Awake()
    {
        CacheSpawnPoints();
    }

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
        if (_spawnPoints.Count > 0)
        {
            for (var i = 0; i < _spawnPoints.Count; i++)
            {
                StartCoroutine(SpawnMissileWithDelay(_spawnPoints[i], i));
            }

            StartCoroutine(DeactivateAfterSpawn(_spawnPoints.Count));
            return;
        }

        // Fallback to the old grid pattern if no prefab-authored spawn markers exist.
        const int rows = 2;
        const int columns = 6;
        var spawnCount = rows * columns;
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                StartCoroutine(SpawnFallbackMissileWithDelay(i, j, i * columns + j));
            }
        }

        StartCoroutine(DeactivateAfterSpawn(spawnCount));
    }

    private void CacheSpawnPoints()
    {
        if (_spawnPoints.Count > 0) return;

        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (!child.CompareTag("HomingMissile")) continue;

            _spawnPoints.Add(child);
            child.gameObject.SetActive(false);
        }
    }

    private IEnumerator DeactivateAfterSpawn(int spawnCount)
    {
        var lastSpawnDelay = Mathf.Max(0, spawnCount - 1) * spawnDelay;
        yield return new WaitForSeconds(lastSpawnDelay + 0.1f);
        gameObject.SetActive(false);
    }

    private IEnumerator SpawnMissileWithDelay(Transform spawnPoint, int spawnIndex)
    {
        yield return new WaitForSeconds(spawnIndex * spawnDelay);

        var spawnPosition = transform.TransformPoint(spawnPoint.localPosition);
        spawnPosition.z = transform.position.z;
        var spawnRotation = transform.rotation * spawnPoint.localRotation;

        Instantiate(missilePrefab, spawnPosition, spawnRotation);
    }

    private IEnumerator SpawnFallbackMissileWithDelay(int i, int j, int spawnIndex)
    {
        yield return new WaitForSeconds(spawnIndex * spawnDelay);

        var spreadAngle = (i * 6 + j - 5.5f) * 10f;
        Instantiate(missilePrefab,
            new Vector3(
                gameObject.transform.position.x + (i * 0.5f) - 1,
                gameObject.transform.position.y - (j * 0.5f) + 1,
                gameObject.transform.position.z),
            Quaternion.Euler(0, 0, 180f + spreadAngle));
    }
}
