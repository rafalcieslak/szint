using System;

namespace Connect4
{
	public interface StateEvaluator
	{
		void Link(GameState s);
		void Update(int x, int y);
		double Evaluate();
		Player isWon();
		string DebugState();
		StateEvaluator Clone();
	}
}

