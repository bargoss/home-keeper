using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    public Coroutine StartCoroutineFromOutside(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }
}