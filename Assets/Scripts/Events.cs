using UnityEngine;

namespace TankGame.Events
{

    public delegate void ValueChangedEvent<T>(T oldValue, T newValue);
    public delegate void RaycastHitEvent(RaycastHit hit);
    
}