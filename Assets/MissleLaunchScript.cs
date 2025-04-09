using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleLaunchScript : MonoBehaviour
{
    public GameObject misslePrefab;
    public GameObject boss;
    public float timePassed;

    // Update is called once per frame
    void Update()
    {

        // missle launcher position is the same as the boss
        // but with a little offset
        //missileLauncherRockets.transform.position = new Vector3(transform.position.x + 3.3f, transform.position.y + 5.5f, transform.position.z);
        gameObject.transform.position = new Vector3(boss.transform.position.x + 2.3f, boss.transform.position.y + 5.5f, boss.transform.position.z);
        // spawn missles every 3 seconds
        if (timePassed > 5)
        {
            timePassed = 0;
            SpawnMissles();
        }
        else
        {
            timePassed += Time.deltaTime;
        }

    }
    void SpawnMissles()
    {
        /*
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);
                Instantiate(misslePrefab, gameObject.transform.position, Quaternion.identity);*/
        // instead put them apart from each other with a 0.1 distance from y axis and another 6 the same way but x difference with 0.5
        /*for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Instantiate(misslePrefab, new Vector3(gameObject.transform.position.x + (i * 0.5f), gameObject.transform.position.y + (j * 0.1f), gameObject.transform.position.z), Quaternion.identity);
            }
        }
*/
        // but spawn them with a little delay
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                // spawn them with a little delay
                StartCoroutine(SpawnMissleWithDelay(i, j));
            }
        }
        // set the position of the missles to be the same as the boss
        // misslePrefab.transform.position = gameObject.transform.position;

    }
    IEnumerator SpawnMissleWithDelay(int i, int j)
    {
        yield return new WaitForSeconds(i * 0.1f + j * 0.1f);
        //Instantiate(misslePrefab, new Vector3(gameObject.transform.position.x + (i * 0.5f), gameObject.transform.position.y + (j * 0.1f), gameObject.transform.position.z), Quaternion.identity);
        // but make them have random rotations
        float randomZ = Random.Range(-1f, 1f);
        Instantiate(misslePrefab, 
            new Vector3(
            gameObject.transform.position.x + (i * 0.5f) - 1, 
            gameObject.transform.position.y - (j * 0.5f) + 1,
            gameObject.transform.position.z), 
            Quaternion.Euler(new Vector3(0, 0, randomZ*100)));
    }
}
