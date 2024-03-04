using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T : MonoBehaviour
{   
    List<T> unused = new();
    List<T> used = new ();

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
            item = Instantiate(prefab);

        }
        used.Add(item);

        return item;
    }
    public void ReturnToPool(T item)
    {
        if(used.Contains(item))
            used.Remove(item);
        unused.Add(item);
        item.gameObject.SetActive(false);
    }

}
