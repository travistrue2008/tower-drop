using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRUEStudios.Foundation.Core;

[CreateAssetMenu(menuName = "Game/Hiscores", fileName = "New Hiscores")]
public class Hiscores : ScriptableObject {
	[Serializable]
	private class ScoreTable : SerializableDictionary<int, int> {}

	#region Fields
	[SerializeField]
	private ScoreTable _scores = new ScoreTable();
	#endregion

	#region Methods
	public void Clear () {
		_scores.Clear();
	}

	public bool Submit (int score, int level) {
		int currentBest = Get(level);
		bool updateScore = (score > currentBest);
		if (updateScore) {
			_scores[level] = score;
		}

		return updateScore;
	}

	public int Get (int level) {
		int score = 0;

		_scores.TryGetValue(level, out score);
		return score;
	}
	#endregion
}
