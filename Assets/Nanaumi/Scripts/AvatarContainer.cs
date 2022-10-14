using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarContainer : DebugParent, IEnumerable<AvatarContainerChild>
{
    private List<AvatarContainerChild> avaterList = new List<AvatarContainerChild>();

    public AvatarContainerChild this[int index] => avaterList[index];
    public int Count => avaterList.Count;

    private void OnTransformChildrenChanged()
    {
        avaterList.Clear();
        foreach (Transform child in transform)
        {
            avaterList.Add(child.GetComponent<AvatarContainerChild>());
            Debug.Log(child);
        }
    }

    public IEnumerator<AvatarContainerChild> GetEnumerator()
    {
        return avaterList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
