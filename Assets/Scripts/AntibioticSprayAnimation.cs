using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AntibioticSprayAnimation : MonoBehaviour
{
    [SerializeField] GameObject particles;
    [SerializeField] GameObject target;
    [SerializeField] GameObject bottle;

    private Vector3 startLocation;
    private Quaternion startRotation;

    public void StartSpray()
    {
        StartCoroutine(_spray());
        StartCoroutine(_moveToFinish());
    }

    private IEnumerator _spray()
    {
        float start = Time.time;
        float end = start + 1.675f;
        float elapsed;

        while (Time.time < end)
        {
            elapsed = Time.time - start;
            if (elapsed > 0.21 && elapsed < 0.8)
            {
                particles.SetActive(true);
            }
            else if (elapsed > 0.8 && elapsed < 1.04)
            {
                particles.SetActive(false);
            }
            else if (elapsed > 1.04 && elapsed < 1.5)
            {
                particles.SetActive(true);
            }
            else particles.SetActive(false);
            yield return null;
        }
    }

    private IEnumerator _moveToFinish()
    {
        yield return new WaitForSeconds(1.675f);

        float start = Time.time;
        float end = Time.time + 1;
        startLocation = bottle.transform.position;
        startRotation = bottle.transform.rotation;


        while (Time.time < end)
        {
            bottle.transform.position = Vector3.Lerp(startLocation, target.transform.position, Time.time - start);
            bottle.transform.rotation = Quaternion.Lerp(startRotation, target.transform.rotation, Time.time - start);
            yield return 1;
        }

    }
}
