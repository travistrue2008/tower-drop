/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TRUEStudios.Core {
	[Serializable]
	public class TimerEvent : UnityEvent<float> { }

	public class Timer : MonoBehaviour {
		#region Fields
		[SerializeField]
		private bool _playOnAwake;
		[SerializeField]
		private bool _looping;
		[SerializeField]
		private float _duration = 1.0f;
		[SerializeField]
		private UnityEvent _onPlay = new UnityEvent();
		[SerializeField]
		private UnityEvent _onFinish = new UnityEvent();
		[SerializeField]
		private UnityEvent _onIterate = new UnityEvent();
		[SerializeField]
		private TimerEvent _onUpdate = new TimerEvent();
		[SerializeField]
		private TimerEvent _onProgress = new TimerEvent();

		private float _currentTime = 0.0f;
		private Coroutine _timerRoutine;
		#endregion

		#region Properties
		public bool IsPlaying { get { return _timerRoutine != null; } }
		public UnityEvent OnPlay { get { return _onPlay; } }
		public UnityEvent OnFinish { get { return _onFinish; } }
		public UnityEvent OnIterate { get { return _onIterate; } }
		public TimerEvent OnUpdate { get { return _onUpdate; } }
		public TimerEvent OnProgress { get { return _onProgress; } }

		public bool IsLooping {
			set { _looping = value; }
			get { return _looping; }
		}

		public float Duration {
			set { _duration = Mathf.Max(Mathf.Epsilon, value); }
			get { return _duration; }
		}

		public float Progress {
			set {
				float val = Mathf.Clamp01(value);
				_currentTime = val / _duration;
				_onUpdate.Invoke(_currentTime);
				_onProgress.Invoke(val);
			}

			get { return _currentTime / _duration; }
		}
		#endregion

		#region Private Methods
		private void Awake () {
			if (_playOnAwake) {
				Play();
			}
		}

		private void OnValidate () {
			_duration = Mathf.Max(Mathf.Epsilon, _duration);
		}

		private IEnumerator ProcessTimer (float delay) {
			// process the delay
			if (delay > 0.0f) {
				yield return new WaitForSeconds(delay);
			}

			// update the current time
			_onPlay.Invoke(); // signal play event
			while (_looping || _currentTime < _duration) {
				Increment();
				yield return null;
			}

			_timerRoutine = null;
		}
		#endregion

		#region Actions
		public void Play (float delay = 0.0f) {
			// start the coroutine if not already
			if (_timerRoutine == null) {
				_timerRoutine = StartCoroutine(ProcessTimer(delay));
			}
		}

		public void Increment () {
			// make sure the timer can run
			if (_currentTime >= _duration) {
				return;
			}

			// update and bound _currentTime
			_currentTime = Mathf.Min(_currentTime + Time.deltaTime, _duration);
			if (_currentTime >= _duration) {
				// check if looping
				if (_looping) {
					// decrement the duration and signal iteration
					_currentTime -= _duration;
					_onIterate.Invoke();
				} else {
					_currentTime = _duration; // bound to duration
				}
			}

			// signal changed event
			_onUpdate.Invoke(_currentTime);
			_onProgress.Invoke(Progress);

			// check if finished
			if (Progress == 1.0f) {
				_onFinish.Invoke();
			}
		}

		public void Pause () {
			// stop the currently-playing coroutine
			if (_timerRoutine != null) {
				StopCoroutine(_timerRoutine);
				_timerRoutine = null;
			}
		}

		public void Stop () {
			// similar to pause except progress is reset
			Pause();
			Reset();
		}

		public void Reset () {
			Progress = 0.0f;
		}
		#endregion
	}
}
