using UnityEngine;
using UnityEngine.Events;

public class EdibleController : MonoBehaviour
{

    public UnityAction<Slime> onEat;

    void Awake()
    {
        onEat += isEaten;
    }

    private void isEaten(Slime slime) {
        Destroy(gameObject);
    }
}
