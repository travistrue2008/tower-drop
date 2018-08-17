/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TRUEStudios.State {
	public class PopupService : PrefabFactoryService<Popup> {
		#region Fields
		[SerializeField]
		private Image _stackBlockerImage;
		[SerializeField]
		private Image _transitionBlockerImage;
		[SerializeField]
		private Transform _popupSpawn;

		private bool _transitioningOut;
		private Coroutine _transitionRoutine;
		private List<Popup> _stack = new List<Popup>();
		#endregion

		#region Properties
		public int StackSize { get { return _stack.Count; } }

		public bool IsTransitioning {
			get {
				return (_transitionRoutine != null);
			}
		}

		public Popup FirstPopup {
			get {
				return (StackSize > 0) ? _stack[0] : null;
			}
		}

		public Popup currentPopup {
			get {
				return (StackSize > 0) ? _stack[StackSize - 1] : null;
			}
		}
		#endregion

		#region Load/Release Popup Profiles
		protected override void OnInitialize () {
			// make sure required references are set in the Inspector
			if (_stackBlockerImage == null || _transitionBlockerImage == null || _popupSpawn == null) {
				throw new Exception("Check the Inspector for missing references.");
			}
		}
		#endregion

		#region Stack Controls
		protected override void OnPushInstance (Popup popup) {
			// make sure both blocker images are active
			_stackBlockerImage.gameObject.SetActive(true);
			_transitionBlockerImage.gameObject.SetActive(true);

			// setup the rect transform
			RectTransform rectTransform = (RectTransform)popup.transform;
			rectTransform.position = Vector3.zero;
			rectTransform.localScale = Vector3.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.one;

			// reset the popup's transition tween to the beginning
			if (popup.TransitionTween != null) {
				popup.TransitionTween.ResetToBegin();
			}

			// check if there is more than one popup already in the stack
			if (StackSize > 1) {
				// instantly hide the second-to-current popup in case it's currently transitioning out
				Popup previousPopup = _stack[StackSize - 2];
				if (previousPopup.TransitionTween != null) {
					previousPopup.TransitionTween.ResetToBegin();
				}

				previousPopup.gameObject.SetActive(false);
			}
			
			// push the new popup onto the stack
			popup.gameObject.SetActive(false);
			_stack.Add(popup);

			// stop any current transition, and begin the new one
			CancelTransition();
			_transitionRoutine = StartCoroutine(ProcessPush());
		}

		public T PushPopup<T> (string prefabName) where T : Popup {
			return PushInstance<T>(prefabName, _popupSpawn);
		}

		public T PushPopup<T> (Popup popupPrefab) where T : Popup {
			return PushInstance<T>(popupPrefab.gameObject, _popupSpawn);
		}

		public Popup PopPopup () {
			// make sure there is a popup to dismiss
			Popup dismissingPopup = currentPopup;
			if (dismissingPopup != null) {
				// check if currently transitioning out with only one popup left
				if (StackSize == 1 && _transitioningOut) {
					Debug.LogWarning("No popups available to dismiss.");
					return null;
				}

				// make sure both blocker images are active
				_stackBlockerImage.gameObject.SetActive(true);
				_transitionBlockerImage.gameObject.SetActive(true);

				// stop any current transition, and begin the new one
				CancelTransition();
				_transitionRoutine = StartCoroutine(ProcessPop());
			} else {
				Debug.LogWarning("No popups available to dismiss.");
			}

			return dismissingPopup;
		}

		public void ClearStack () {
			CancelTransition();

			// destroy all popup GameObjects
			foreach (Popup popup in _stack) {
				Destroy(popup.gameObject);
			}

			_stack.Clear(); // release all dangling Popup references

			// deactivate both blocker images
			_stackBlockerImage.gameObject.SetActive(false);
			_popupSpawn.gameObject.SetActive(false);
		}
		#endregion

		#region Private Methods
		private void CancelTransition () {
			// stop the current transition if it's active
			if (_transitionRoutine == null) {
				return;
			}

			StopCoroutine(_transitionRoutine);
			_transitionRoutine = null;

			// handle transitiong out logic
			if (_transitioningOut) {
				// remove the popup reference, and destroy the popup
				Popup removedPopup = currentPopup;
				_stack.Remove(removedPopup);
				Destroy(removedPopup.gameObject);

				// activate the new current popup
				if (currentPopup != null) {
					currentPopup.gameObject.SetActive(true);
				}

				_transitioningOut = false;
			}
		}
		#endregion

		#region Coroutines
		private IEnumerator ProcessPush () {
			// check if there was a previous popup
			if (StackSize > 1) {
				// get the previous popup, and check if it's got a transition tween
				Popup previousPopup = _stack[StackSize - 2];
				if (previousPopup.TransitionTween != null) {
					// check if the popup is active
					if (previousPopup.isActiveAndEnabled) {
						yield return previousPopup.TransitionTween.Play(false);
					} else {
						previousPopup.TransitionTween.ResetToBegin();
					}
				}

				previousPopup.gameObject.SetActive(false); // hide the previous popup
			}

			// display the current popup
			currentPopup.gameObject.SetActive(true);
			if (currentPopup.TransitionTween != null) {
				yield return currentPopup.TransitionTween.Play();
			}

			// deactivate the transition blocker image, and stop invalidate the transition coroutine
			_transitionBlockerImage.gameObject.SetActive(false);
			_transitionRoutine = null;
		}

		private IEnumerator ProcessPop () {
			// set flag
			_transitioningOut = true; {
				// hide the current popup
				if (currentPopup.isActiveAndEnabled && currentPopup.TransitionTween != null) {
					yield return currentPopup.TransitionTween.Play(false);
				}
				
				// remove the popup reference, and destroy the popup
				Popup removedPopup = currentPopup;
				_stack.Remove(removedPopup);
				Destroy(removedPopup.gameObject);
			}
			_transitioningOut = false; // unset flag

			// check if there are any popups left in the stack
			if (currentPopup != null) {
				// activate, and transition the popup
				currentPopup.gameObject.SetActive(true);
				if (currentPopup.TransitionTween != null) {
					yield return currentPopup.TransitionTween.Play();
				}
			}

			// deactivate the transition blocker image, and stop invalidate the transition coroutine
			_stackBlockerImage.gameObject.SetActive(StackSize > 0);
			_transitionBlockerImage.gameObject.SetActive(false);
			_transitionRoutine = null;
		}
		#endregion
	}
}
