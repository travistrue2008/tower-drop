/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TRUEStudios.State {
	public class Services : MonoBehaviour {
		#region Constants
		private const string Tag = "Services";
		private const string PrefabPath = "Prefabs/Core/Services";
		#endregion

		#region Fields
		private static bool _shuttingDown = false;
		private static Services _instance;

		private Dictionary<Type, Service> _services = new Dictionary<Type, Service>();
		#endregion

		#region Properties
		public static bool IsReady { get { return (Instance != null); } }

		public static Services Instance {
			get {
				// attempt to get an instance if not found
				if (!_shuttingDown && _instance == null) {
					// attempt to find the GameObject in the hierarchy
					GameObject obj = GameObject.FindWithTag(Tag);
					if (obj != null) {
						// get the component
						_instance = obj.GetComponent<Services>();
						if (_instance != null) {
							Debug.Log(Tag + " GameObject found in hierarchy.");
							return _instance;
						}
					} else {
						// attempt to instantiate its prefab, if it exists
						try {
							// attempt to instantiate the Services prefab from Resources
							obj = (GameObject)Instantiate(Resources.Load(PrefabPath));
							obj.name = Tag;
							_instance = obj.GetComponent<Services>();

							// grab the Services component, and make sure it's found
							if (_instance == null) {
								// destroy the GameObject prefab if it wasn't found
								Debug.LogWarning("Services prefab found in Resources, but it doesn't have a Services component.");
								Destroy(obj);
								obj = null;
							} else {
								return _instance;
							}
						} catch {
							Debug.LogWarning("Services prefab not found.");
						}
					}

					// if all else fails, create a new GameObject
					Debug.Log("Creating new Services GameObject.");
					obj = new GameObject(Tag);
					_instance = obj.AddComponent<Services>();
				}

				return _instance;
			}
		}
		#endregion

		#region Class Methods
		public static TService Get<TService> () where TService : Service {
			// check if shutting down
			if (_shuttingDown) {
				return default(TService);
			}

			Service component = null;
			Type serviceType = typeof(TService);

			// attempt to retrieve the service reference from the dictionary (if it exists)
			if (Instance._services.TryGetValue(serviceType, out component)) {
				return component as TService;
			}

			Instance.RefreshServices(); // refresh services if nothing was found

			// attempt to retrieve the service reference from the dictionary again (if it exists)
			if (Instance._services.TryGetValue(serviceType, out component)) {
				return component as TService;
			}

			// if at this point, add the desired Service component to the Services GameObject, add it to the list, and return it
			TService service = Instance.gameObject.AddComponent<TService>();
			Instance._services[service.GetType()] = service;
			return service;
		}

		public static bool Remove<TService> () where TService : Service {
			// get the component
			TService component = Instance.GetComponent<TService>();
			if (component == null) {
				return false;
			}

			Destroy(component);
			return (!_shuttingDown && Instance._services.Remove(typeof(TService)));
		}

		public static Coroutine Release () {
			return Instance.StartCoroutine(Instance.ProcessCleanup());
		}

		public static string TrimDots(string input) {
			int lastDotIndex = input.LastIndexOf('.');
			if (lastDotIndex > -1 && lastDotIndex + 1 < input.Length) {
				input = input.Substring(lastDotIndex + 1);
			}

			return input;
		}
		#endregion

		#region Constructors for singleton initialization
		static Services () { }
		private Services () { }
		#endregion

		#region MonoBehaviour Hooks
		private void Awake () {
			// destroy self if the Service has already been created
			if (_instance != null && _instance != this) {
				Destroy(gameObject);
				return;
			}

			DontDestroyOnLoad(gameObject); // If survived to this point, thrive!
		}

		private void Start () {
			// list the services
			if (Debug.isDebugBuild) {
				ListServices();
			}
		}

		private void OnDestroy () {
			if (_instance == this) {
				_shuttingDown = true;
			}

			_services.Clear();
		}
		#endregion

		#region Internal Methods
		private void RefreshServices () {
			// get all service component references attached to the object
			Service[] services = GetComponents<Service>();
			foreach (Service service in services) {
				// initialize, and add to the list
				service.Initialize();
				_services[service.GetType()] = service;
			}
		}

		private IEnumerator ProcessCleanup () {
			// wait for Unity to finish cleaningi up resources
			AsyncOperation op = Resources.UnloadUnusedAssets();
			while (!op.isDone) {
				yield return null;
			}
			
			GC.Collect();
		}
		#endregion

		#region Actions
		public static void ListServices () {
			// list all services
			string content = "-----Available Services-----";
			foreach (KeyValuePair<Type, Service> kvp in Instance._services) {
				content += $"  {kvp.Key}\n";
			}

			content += "----------------------------";
		}
		#endregion

		#region Menu Items
		#if UNITY_EDITOR
		[MenuItem("Tools/Game/Clear Preferences")]
		public static void ClearPreferences () {
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
		}
		#endif
		#endregion
	}
}
