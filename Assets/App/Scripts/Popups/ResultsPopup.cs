using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TRUEStudios.State;
using TRUEStudios.Tweens;
using TMPro;

public class ResultsPopup : Popup {
	#region Fields
	[SerializeField]
	private TextMeshProUGUI _resultLabel;
	[SerializeField]
	private TextMeshProUGUI _recordLabel;
	#endregion

	#region Properties
	#endregion

	#region MonoBehaviour Hooks
	public void Set (bool passed, int level, int bestScore) {
		string result = passed ? "Passed" : "Failed";
		_resultLabel.text = $"Level {level} {result}";
		if (bestScore > 0) {
			_recordLabel.text = $"NEW RECORD\n{bestScore}";
			var tween = _recordLabel.GetComponent<Tween>();
			tween.PlayForward();
		}
	}
	#endregion
}
