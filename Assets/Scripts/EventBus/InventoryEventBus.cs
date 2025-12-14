using System;
using UnityEngine;
public abstract class Event { }

namespace SA.EventBusSystem
{
    public class InventoryEventBus<T> where T : Event
    {
        public static event Action<T> OnEvent;

        public static void Publish(T pEvent)
        {
            OnEvent?.Invoke(pEvent);
        }
    }

    public class ItemCollected : Event
    {
        public int index;
        public ItemCollected(int index)
        {
            this.index = index;
        }
    }
}