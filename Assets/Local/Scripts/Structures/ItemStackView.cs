using Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class ItemStackView: MonoBehaviour
    {
        [SerializeField]
        private float _offset;
        [SerializeField]
        private List<ExclusiveStack> _exclusiveStacks = new List<ExclusiveStack>();
        private Dictionary<ItemType, ExclusiveStack> _exclusiveStacksDictionary = new Dictionary<ItemType, ExclusiveStack>();

        public int Count { get; private set; }
        public float Offset { get { return _offset; } }

        private List<ItemView> _items = new List<ItemView>();
        private Vector3 _temporaryWorldPosition;
        private bool _useTemporaryPosition;
        private float _lastMovingTime = -1f;

        public void SortItems()
        {
            if (_useTemporaryPosition && _items.Count > 0 && _items[0].transform.parent != null)
            {
                foreach (var item in _items)
                {
                    if (item.gameObject.activeSelf && !_exclusiveStacksDictionary.ContainsKey(item.ItemType))
                    {
                        item.transform.parent = null;
                    }
                }
            }
            else if (!_useTemporaryPosition && _items.Count > 0 && _items[0].transform.parent == null)
            {
                foreach (var item in _items)
                {
                    if (item.gameObject.activeSelf && !_exclusiveStacksDictionary.ContainsKey(item.ItemType))
                    {
                        item.transform.parent = transform;
                    }
                }
            }

            if (_exclusiveStacksDictionary.Count < _exclusiveStacks.Count)
            {
                foreach (var stack in _exclusiveStacks)
                {
                    _exclusiveStacksDictionary.TryAdd(stack.ItemType, stack);
                }
            }

            var counter = 0;
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item.gameObject.activeSelf && !_exclusiveStacksDictionary.ContainsKey(item.ItemType))
                {
                    item.LastPosition = item.transform.localPosition;
                    item.CurrentPosition = new Vector3(0, counter * _offset, 0);
                    item.transform.localRotation = Quaternion.identity;
                    if (_useTemporaryPosition)
                    {
                        item.CurrentPosition += _temporaryWorldPosition;
                    }
                    else
                    {
                        item.transform.localPosition = item.CurrentPosition;
                    }
                    counter++;
                }
            }

            foreach (var exclusiveStack in _exclusiveStacks)
            {
                counter = 0;
                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    if (item.ItemType == exclusiveStack.ItemType)
                    {
                        if (item.transform.parent != exclusiveStack.Transform)
                        {
                            item.transform.SetParent(exclusiveStack.Transform);
                        }

                        item.transform.localPosition = new Vector3(0, counter * _offset, 0);
                        item.transform.localRotation = Quaternion.identity;
                        counter++;
                    }
                }
            }
        }

        public void AddItem(ItemView itemView)
        {
            Count++;

            _items.Add(itemView);

            itemView.transform.SetParent(transform);

            SortItems();
        }

        public void RemoveItem(ItemType itemType, int removeCount)
        {
            Count -= removeCount;
            
            int a = 0;
            for (var i = 0; i < _items.Count; i++) 
            {
                var item = _items[i];
                if (item.ItemType == itemType)
                {
                    _items.Remove(item);
                    item.Release();
                    SortItems();
                    i--;
                    a++;
                    if(a >= removeCount) 
                    { 
                        return; 
                    }
                }
            }
        }

        public void ToggleExclusiveItemStack(ItemType itemType, bool value)
        {
            if (_exclusiveStacksDictionary.TryGetValue(itemType, out var exclusiveStack))
            {
                exclusiveStack.Transform.gameObject.SetActive(value);
            }
        }

        public Vector3 GetTopPosition(ItemType itemType)
        {
            var position = transform.position;
            var count = Count;
            if (itemType != ItemType.NONE)
            {
                if (_exclusiveStacksDictionary.TryGetValue(itemType, out var exclusiveStack))
                {
                    if (!exclusiveStack.Transform.gameObject.activeSelf)
                    {
                        return Vector3.zero;
                    }
                    
                    position = exclusiveStack.Transform.position;
                }

                count = 0;
                for (var i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    if (item.ItemType == itemType)
                    {
                        count++;
                    }
                }
            }
            position.y += count * Offset;
            return position;
        }

        public void Drop(Vector3 worldPosition)
        {
            if (_useTemporaryPosition)
            {
                return;
            }

            _temporaryWorldPosition = worldPosition;
            _useTemporaryPosition = true;

            SortItems();

            _lastMovingTime = Time.time;
            StartCoroutine(AsyncMovingPivot());
        }

        public void PickUp()
        {
            if (!_useTemporaryPosition)
            {
                return;
            }

            _useTemporaryPosition = false;
            SortItems();

            _lastMovingTime = Time.time;
            StartCoroutine(AsyncMovingPivot());
        }

        public IEnumerator AsyncMovingPivot()
        {
            while (Time.time - _lastMovingTime < 0.3f)
            {
                var d = (Time.time - _lastMovingTime) / 0.3f;
                var verticalDelta = (0.5f - Mathf.Abs(d - 0.5f)) * 2f;
                var i = 0;
                foreach (var item in _items)
                {
                    if (item.gameObject.activeSelf && !_exclusiveStacksDictionary.ContainsKey(item.ItemType))
                    {
                        item.transform.localPosition = Vector3.Lerp(item.LastPosition, item.CurrentPosition, d) + new Vector3(0f, verticalDelta * (1f + (0.3f * i)), 0f);
                    }
                    i++;
                }
                yield return new WaitForEndOfFrame();
            }
            foreach (var item in _items)
            {
                if (item.gameObject.activeSelf && !_exclusiveStacksDictionary.ContainsKey(item.ItemType))
                {
                    item.transform.localPosition = item.CurrentPosition;
                }
            }
        }

        [Serializable]
        public class ExclusiveStack
        {
            public ItemType ItemType;
            public Transform Transform;
        }
    }
}
