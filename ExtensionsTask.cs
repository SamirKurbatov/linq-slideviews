using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public static class ExtensionsTask
{
    /// <summary>
    /// Медиана списка из нечетного количества элементов — это серединный элемент списка после сортировки.
    /// Медиана списка из четного количества элементов — это среднее арифметическое 
    /// двух серединных элементов списка после сортировки.
    /// </summary>
    /// <exception cref="InvalidOperationException">Если последовательность не содержит элементов</exception>
    public static double Median(this IEnumerable<double> items)
    {
        if (items == null)
            throw new InvalidOperationException();

        var sortedItems = items.OrderBy(x => x).ToList();
        var itemCount = sortedItems.Count;
        var middleIndex = itemCount / 2;

        return itemCount % 2 == 0
            ? sortedItems.Skip(middleIndex - 1).Take(2).Average()
            : sortedItems[middleIndex];
    }

    /// <returns>
    /// Возвращает последовательность, состоящую из пар соседних элементов.
    /// Например, по последовательности {1,2,3} метод должен вернуть две пары: (1,2) и (2,3).
    /// </returns>
    public static IEnumerable<(T First, T Second)> Bigrams<T>(this IEnumerable<T> items)
    {
        using (var enumerator = items.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                yield break;
            }

            T currentItem = enumerator.Current;

            while (enumerator.MoveNext())
            {
                T nextItem = enumerator.Current;
                yield return (currentItem, nextItem);
                currentItem = nextItem;
            }
        }
    }
}