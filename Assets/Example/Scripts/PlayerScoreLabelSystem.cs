using TMPro;
using Unity.Collections;
using Unity.Entities;

namespace TriggerSystem.Example
{
	public class PlayerScoreLabelSystem : ComponentSystem
	{
		private EntityQuery _entityQuery;
		private EntityQuery _playerEntityQuery;

		protected override void OnCreate()
		{
			_entityQuery = GetEntityQuery(
				typeof(TextMeshProUGUI),
				typeof(PlayerScore)
			);

			_playerEntityQuery = GetEntityQuery(
				typeof(PlayerComponent)
			);
		}

		protected override void OnUpdate()
		{
			var players = _playerEntityQuery.ToComponentDataArray<PlayerComponent>(Allocator.TempJob);

			int playerScore = -1;

			if (players.Length > 0) playerScore = players[0].Score;

			var labels = _entityQuery.ToComponentArray<TextMeshProUGUI>();
			var scores = _entityQuery.ToComponentDataArray<PlayerScore>(Allocator.TempJob);

			for (var i = 0; i < labels.Length; i++)
			{
				var score = scores[i];

				if (playerScore > 0 && score.Score != playerScore)
				{
					score.Score = playerScore;
					labels[i].text = score.Score.ToString("0000");
					scores[i] = score;
				}
			}

			_entityQuery.CopyFromComponentDataArray(scores);

			scores.Dispose();
			players.Dispose();
		}
	}
}