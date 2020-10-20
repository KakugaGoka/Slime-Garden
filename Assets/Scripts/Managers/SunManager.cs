using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.rotation = Quaternion.Euler(TimeManager.main.TimeToDegrees(), 0, 0);
    }
}
