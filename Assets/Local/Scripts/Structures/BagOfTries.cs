using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class BagOfTries
    {
        public List<bool> Values = new List<bool>();
        public int CurrentIndex = 0;

        public bool TryGetNext(out int lastIndex, out bool changedValue, out int nextIndex, out bool nextValue)
        {
            if (Values.Count == 0)
            {
                lastIndex = 0;
                changedValue = false;
                nextIndex = 0;
                nextValue = false;
                return false;
            }

            lastIndex = CurrentIndex;

            changedValue = Random.Range(0f, 1f) > 0.5f;
            Values[CurrentIndex] = changedValue;

            CurrentIndex++;
            if (CurrentIndex >= Values.Count)
            {
                CurrentIndex = 0;
            }

            nextIndex = CurrentIndex;
            nextValue = Values[CurrentIndex];
            return true;
        }

        public void Restore()
        {
            for (var i = 0; i < Values.Count; i++)
            {
                Values[i] = true;
            }
        }

        public void Resize(int size)
        {
            var newValues = new List<bool>(size);

            for (var i = 0; i < size; i++)
            {
                newValues.Add(i < Values.Count ? Values[i] : true);
            }
            
            Values = newValues;
        }
    }
}