using System.Collections.Generic;

// this is for A star path finding, for patrol enemy'
// this list structure ensures that the lowest priority value is dequeued first.
public class PriorityQueue<T>
{
    private List<(T item, int priority)> elements = new List<(T item, int priority)>();

    public int Count => elements.Count;

    //Modify list and sort

    //Add item to list
    public void Enqueue(T item, int priority)
    {
        elements.Add((item, priority));
        elements.Sort((a, b) => a.priority.CompareTo(b.priority)); // Sort by priority (smallest first)
    }

    //Delete item to list
    public T Dequeue()
    {
        if (elements.Count == 0) return default;
        T bestItem = elements[0].item;
        elements.RemoveAt(0);
        return bestItem;
    }

    //check if item in list
    public bool Contains(T item)
    {
        return elements.Exists(e => EqualityComparer<T>.Default.Equals(e.item, item));
    }
}