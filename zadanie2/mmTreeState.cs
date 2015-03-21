using System;
using System.Collections.Generic;

namespace Connect4
{	
	public enum Player{
		NONE,
		UNREACHABLE,
		A,
		B
	}
	public interface mmTreeState
	{	
		Player isWon();
		double Evaluate();
		List<int> ListValidMoves();
		bool PerformMove(int move);
		mmTreeState Clone();
		string PrintState();
	}
}

