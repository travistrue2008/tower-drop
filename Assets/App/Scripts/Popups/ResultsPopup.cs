using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRUEStudios.Foundation.Events;
using TRUEStudios.Foundation.Tweens;
using TRUEStudios.Foundation.UI;
using TRUEStudios.Foundation.Variables;
using TMPro;

public class ResultsPopup : Popup {
	#region Fields
	[SerializeField]
	private BoolEventHub _onFinishLevelHub;
	[SerializeField]
	private BoolReference _levelPassedReference;
	[SerializeField]
	private IntReference _levelReference;
	[SerializeField]
	private IntReference _scoreReference;
	[SerializeField]
	private Hiscores _hiscores;
	[SerializeField]
	private TextMeshProUGUI _resultLabel;
	[SerializeField]
	private TextMeshProUGUI _recordLabel;
	#endregion

	#region Methods
	private void Awake () {
		OnClose.AddListener(HandleOnClose);

		string result = _levelPassedReference.Value ? "Passed" : "Failed";
		_resultLabel.text = $"Level {_levelReference.Value} {result}";
	}

	private void Start () {
		// submit the hiscore, and display the new record if it's new
		if (_hiscores.Submit(_scoreReference.Value, _levelReference.Value)) {
			_recordLabel.text = $"NEW RECORD\n{_scoreReference.Value}";
			var tween = _recordLabel.GetComponent<Tween>();
			tween.PlayForward();
		}
	}

	private void HandleOnClose () {
		_onFinishLevelHub.Raise(_levelPassedReference.Value);
	}
	#endregion
}
