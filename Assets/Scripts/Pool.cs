using System.Collections.Generic;
using UnityEngine;

namespace Wolfheat.Pool
{
    public class Pool<T> where T : MonoBehaviour
    {
        private List<T> unused = new();
        private List<T> used = new();

        public int Count { get { return used.Count + unused.Count; } }
        public int UsedCount { get { return used.Count; } }
        public int UnusedCount { get { return unused.Count; } }
        public T GetNextFromPool(T prefab)
        {
            T item;
            if (unused.Count > 0)
            {
                item = unused[0];
                item.gameObject.SetActive(true);
                unused.RemoveAt(0);
            }
            else
            {
                //Debug.Log("Had to Instantiate New item");
                item = Object.Instantiate(prefab);

            }
            used.Add(item);

            return item;
        }
        public void ReturnToPool(T item)
        {
            if (used.Contains(item))
                used.Remove(item);
            unused.Add(item);
            item.gameObject.SetActive(false);
        }

        public void Add(T item)
        {
            used.Add(item);
        }
    }
}