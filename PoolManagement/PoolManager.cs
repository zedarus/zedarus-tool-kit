﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Math;

namespace Zedarus.ToolKit.PoolManagement
{
	public class PoolManager<T> where T : MonoBehaviour, IPoolItem
	{
		private List<T> _pool;
		private List<T> _active;
		private Transform _container;
		private bool _autoReuse;

		private T[] _prefabs;
		private int[] _prefabsShuffleBag;
		private bool _autoResize;
		private int _poolSize;

		#region Contstructors
		public PoolManager(Transform container, int size, bool autoReuse, bool autoResize,T[] prefabs, params int[] prefabsShuffleBag)
		{
			_autoReuse = autoReuse;
			_autoResize = autoResize;
			_poolSize = size;
			_prefabs = prefabs;
			_prefabsShuffleBag = prefabsShuffleBag;

			_pool = new List<T>();
			_active = new List<T>();

			_container = container;
			T[] children = _container.GetComponentsInChildren<T>();
			foreach (T child in children)
			{
				child.Init();
				_pool.Add(child);
			}



			prefabs = null;
		}

		public PoolManager(Transform container, int size, bool autoReuse, bool autoResize, string[] prefabs, params int[] prefabsShuffleBag) : this(container, size, autoReuse,autoResize, ConvertPathsToPrefabs(prefabs), prefabsShuffleBag) {}
		public PoolManager(Transform container, int size, bool autoReuse, string[] prefabs, params int[] prefabsShuffleBag) : this(container, size, autoReuse,false, ConvertPathsToPrefabs(prefabs), prefabsShuffleBag) {}

		public PoolManager(Transform container, int size, bool autoReuse,bool autoResize = false) : this(container, size, autoReuse,autoResize, new string[0]) {}

		public PoolManager(Transform parent, string containerName, int size, bool autoReuse,bool autoResize, T[] prefabs, params int[] prefabsShuffleBag) : this(CreateContainer(parent, containerName), size, autoReuse,autoResize, prefabs, prefabsShuffleBag) {}
		public PoolManager(Transform parent, string containerName, int size, bool autoReuse,bool autoResize, string[] prefabs, params int[] prefabsShuffleBag) : this(CreateContainer(parent, containerName), size, autoReuse,autoResize, prefabs, prefabsShuffleBag) {}
		public PoolManager(T prefab, Transform container, int size, bool autoReuse, bool autoResize = false) : this(container, size, autoReuse, autoResize, new T[] { prefab }, 1) { }
		public PoolManager(string prefab, Transform container, int size, bool autoReuse,bool autoResize = false) : this(Resources.Load<T>(prefab), container, size, autoReuse,autoResize) { }
		public PoolManager(T prefab, Transform parent, string containerName, int size, bool autoReuse,bool autoResize = false) : this(prefab, CreateContainer(parent, containerName), size, autoReuse,autoResize) { }
		public PoolManager(string prefab, Transform parent, string containerName, int size, bool autoReuse,bool autoResize = false) : this(prefab, CreateContainer(parent, containerName), size, autoReuse,autoResize) { }
		#endregion

		#region Constructor Helpers
		private static Transform CreateContainer(Transform parent, string containerName)
		{
			GameObject go = new GameObject(containerName);
			go.transform.SetParent(parent);
			go.transform.localPosition = Vector3.zero;
			return go.transform;
		}

		private static T[] ConvertPathsToPrefabs(string[] paths)
		{
			List<T> prefabs = new List<T>();

			foreach (string path in paths)
			{
				prefabs.Add(Resources.Load<T>(path));
			}

			return prefabs.ToArray();
		}
		#endregion

		public virtual void ReturnInactiveItemsToPool(System.Action<T> returnToPoolCallback = null)
		{
			for (int i = _active.Count - 1; i >= 0; i--)
			{
				if (_active[i].ReturnToPool)
				{
					if (returnToPoolCallback != null)
					{
						returnToPoolCallback(_active[i]);
					}

					ReturnItemToPoolAtIndex(i);
				}
			}
		}

		public virtual T GetItemFromPool()
		{
			if (_pool.Count == 0 && _autoResize)
			{
				FillPool();
			}
			if (_pool.Count > 0)
			{
				T item = _pool[0];
				_pool.RemoveAt(0);
				_active.Add(item);
				return item;
			}
			else if (_autoReuse && _active.Count > 0)
			{
				ReturnItemToPool(_active[0]);
				return GetItemFromPool();
			}
			else
				return null;
		}

		public virtual T[] GetItemFromPool(int number)
		{
			List<T> items = new List<T>();
			for (int i = 0; i < number; i++)
			{
				T item = GetItemFromPool();
				if (item)
					items.Add(item);
			}

			return items.ToArray();
		}
		
		public virtual void ReturnItemToPool(T item)
		{
			_active.Remove(item);
			item.Deactivate();
			_pool.Add(item);
		}

		public virtual void ReturnItemToPoolAtIndex(int index)
		{
			T item = _active[index];
			_active.RemoveAt(index);
			item.Deactivate();
			_pool.Add(item);
		}

		public void ReturnAllItemsToPool()
		{
			for (int i = _active.Count - 1; i >= 0; i--)
				ReturnItemToPoolAtIndex(i);
		}

		public List<T> Active
		{
			get { return _active; }
		}

		private void FillPool()
		{
			if (_pool.Count < _poolSize && _prefabs != null && _prefabs.Length > 0 && _prefabs[0] != null)
			{
				ShuffleBag<int> bag = new ShuffleBag<int>();
				for (int i = 0; i < _prefabs.Length; i++)
				{
					bag.Add(i, _prefabsShuffleBag.Length > i ? _prefabsShuffleBag[i] : 1);
				}

				int attempts = _poolSize  - _pool.Count  + 200;
				while (_pool.Count < _poolSize && attempts > 0)
				{
					T prefab = _prefabs[bag.Next()];
					if (prefab != null)
					{
						T newPoolObject = GameObject.Instantiate(_prefabs[bag.Next()]) as T;
						if (newPoolObject != null)
						{
							newPoolObject.transform.SetParent(_container);
							newPoolObject.Init();
							_pool.Add(newPoolObject);
						}
						else
							attempts--;
					}
					else
						attempts--;
				}

				bag.Clear();
				bag = null;
			}
		}
	}
}
