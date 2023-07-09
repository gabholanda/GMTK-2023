using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent Event;
    public UnityEvent Response = new UnityEvent();

    private void OnEnable()
    { TryRegister(); }

    private void OnDisable()
    { Event.UnregisterListener(this); }

    public void OnEventRaised()
    { Response.Invoke(); }

    public void TryRegister()
    {
        Event?.RegisterListener(this);
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }

}
