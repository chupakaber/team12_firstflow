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
        private Vector3 _baseRotation;
        [SerializeField]
        private Vector3 _dynamicRotation;
        [SerializeField]
        private List<ExclusiveStack> _exclusiveStacks = new List<ExclusiveStack>();
        private Dictionary<ItemType, ExclusiveStack> _exclusiveStacksDictionary = new Dictionary<ItemType, ExclusiveStack>();

        public ItemStackAlignType Align = ItemStackAlignType.VERTICAL_STACK;

        public int Count { get; private set; }
        public float Offset { get { return _offset; } }

        private List<ItemView> _items = new List<ItemView>();
        private Vector3 _temporaryWorldPosition;
        private bool _useTemporaryPosition;
        private bool _isMoving;
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
                    GetAlignedPosition(item, counter, out var newPosition, out var newRotation);
                    item.CurrentPosition = newPosition;
                    item.transform.localRotation = newRotation;
                    if (_useTemporaryPosition)
                    {
                        item.CurrentPosition += _temporaryWorldPosition;
                    }
                    if (!_isMoving)
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

                        GetAlignedPosition(item, counter, out var newPosition, out var newRotation);
                        item.transform.localPosition = newPosition;
                        item.transform.localRotation = newRotation;
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
            for (var i = _items.Count - 1; i >= 0; i--)
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
            _isMoving = true;
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
            _isMoving = false;
        }

        private void GetAlignedPosition(ItemView item, int index, out Vector3 position, out Quaternion rotation)
        {
            if (Align == ItemStackAlignType.VERTICAL_STACK)
            {
                rotation = Quaternion.identity;
                position = new Vector3(0f, index * _offset, 0f);
                return;
            }
            else if (Align == ItemStackAlignType.HEAP)
            {
                var r1 = Mathf.Sin(index * 2312f);
                var r2 = Mathf.Sin(index * 3714f);
                var r3 = Mathf.Sin(index * 7654f);
                rotation = Quaternion.Euler(_baseRotation + new Vector3(_dynamicRotation.x * r1, _dynamicRotation.y * r2, _dynamicRotation.z * r3));
                position = new Vector3(Mathf.Sin(index * 28.7f) * _offset * r1, _offset * (r2 + index * 0.1f), Mathf.Cos(index * 28.7f) * _offset * r3);
                return;
            }
            rotation = Quaternion.identity;
            position = Vector3.zero;
        }

        public enum ItemStackAlignType
        {
            NONE = 0,
            VERTICAL_STACK = 1,
            HEAP = 2,
        }

        [Serializable]
        public class ExclusiveStack
        {
            public ItemType ItemType;
            public Transform Transform;
        }
    }
}
