using UnityEngine;
using UnityEngine.Events;

public class EdibleController : MonoBehaviour
{

    public UnityAction<SlimeController> onEat;

    void Awake()
    {
        onEat += isEaten;
    }

    private void isEaten(SlimeController slime) {
        Destroy(gameObject);
    }
}
