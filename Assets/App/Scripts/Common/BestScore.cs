using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BestScore {
	#region Constants
	public const string BestScorePrefix = "best_score_";
	#endregion

	#region Methods
	public static void Clear () {
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}

	public static bool Submit (int score, int level) {
		int currentBest = Get(level);
		bool updateScore = (currentBest < score);
		if (updateScore) {
			PlayerPrefs.SetInt(KeyForLevel(level), score);
			PlayerPrefs.Save();
		}

		return updateScore;
	}

	public static int Get (int level) {
		return PlayerPrefs.GetInt(KeyForLevel(level));
	}

	private static string KeyForLevel (int level) {
		return $"{BestScorePrefix}{level:D3}";
	}
	#endregion
}
