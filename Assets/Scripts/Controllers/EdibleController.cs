using UnityEngine;
using UnityEngine.Events;

public class EdibleController : MonoBehaviour
{
    public UnityAction<SlimeController> onEat;

    private void Awake()
    {
        onEat += isEaten;
    }

    private void isEaten( SlimeController slime )
    {
        Destroy( gameObject );
    }
}