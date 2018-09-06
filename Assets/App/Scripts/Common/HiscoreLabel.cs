using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRUEStudios.Foundation.Tweens;
using TMPro;

public class HiscoreLabel : MonoBehaviour {
	#region Fields
	[SerializeField]
	private Hiscores _hiscores;

	private TextMeshProUGUI _label;
	#endregion

	#region Actions
	private void Awake () {
		_label = GetComponent<TextMeshProUGUI>();
	}

	public void Set (int level) {
		int bestScore = _hiscores.Get(level);
		_label.text = (bestScore > 0) ? $"Best: {bestScore}" : "";
	}
	#endregion
}
