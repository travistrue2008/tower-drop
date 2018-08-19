/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System;
using UnityEngine;
using UnityEngine.Events;

namespace TRUEStudios.Core {
	[Serializable]
	public class CounterEvent : UnityEvent<int> { }

	public class Counter : MonoBehaviour {
		#region Fields
		[SerializeField]
		private int _targetValue = 0;
		[SerializeField]
		private UnityEvent _onReachTarget = new UnityEvent();
		[SerializeField]
		private CounterEvent _onChange = new CounterEvent();

		private int _value = 0;
		#endregion

		#region Properties
		public UnityEvent OnReachTarget { get { return _onReachTarget; } }
		public CounterEvent OnChange { get { return _onChange; } }

		public int TargetValue {
			set {
				_targetValue = value;
				if (_value == _targetValue) {
					_onReachTarget.Invoke();
				}
			}

			get { return _value; }
		}

		public int Value {
			set {
				if (_value != value) {
					_value = value;
					_onChange.Invoke(_value);
				}

				if (_value == _targetValue) {
					_onReachTarget.Invoke();
				}
			}

			get { return _value; }
		}
		#endregion

		#region Methods
		public void Offset (int offset) {
			Value += offset;
		}

		public void Increment () {
			++Value;
		}

		public void Decrement () {
			--Value;
		}

		public void Reset () {
			Value = 0;
		}
		#endregion
	}
}
