using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRUEStudios.Foundation.Tweens;
using TRUEStudios.Foundation.Variables;
using TMPro;

public class StreakLabel : MonoBehaviour {
	#region Fields
	[SerializeField]
	private IntReference _streakReference;
	#endregion

	#region Actions
	private void Awake () {
		if (_streakReference.Value > 0) {
			var label = GetComponent<TextMeshProUGUI>();
			label.text = $"+{_streakReference.Value}";

			var tween = GetComponent<Tween>();
			Destroy(gameObject, tween.Duration);
		} else {
			Destroy(gameObject);
		}
	}
	#endregion
}
