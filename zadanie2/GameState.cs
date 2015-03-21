using System;
using System.Collections.Generic;

namespace Connect4
{
	public class GameState : mmTreeState
	{
		public BitMask PlayerATokenMask;
		public BitMask PlayerBTokenMask;

		const int rowWidth = 7;

		public Player turn;
		StateEvaluator evaluator;
		public int moves = 0;

		public GameState (StateEvaluator se){
			evaluator = se;
			PlayerATokenMask = 0;
			PlayerBTokenMask = 0;
			turn = Player.A;
			evaluator.Link(this);
		}

		public void Reset(){
			PlayerATokenMask = 0;
			PlayerBTokenMask = 0;
			turn = Player.A;
			evaluator.Link(this);
		}

		public double Evaluate(){
			return evaluator.Evaluate();
		}
		public Player isWon(){
			return evaluator.isWon();
		}

		public List<int> ListValidMoves(){
			List<int> l = new List<int>();
			for (int i = 0; i < rowWidth; i++)
				if (GetColumnHeight (i) < 6)
					l.Add (i);
			return l;
		}

		public bool PerformMove(int move){
			int y = 5-GetColumnHeight (move);
			if (y < 0) return false;
			SetField(move, y, turn);
			moves++;
			evaluator.Update(move, y);
			NextTurn();
			return true;
		}

		public void NextTurn(){
			if (turn == Player.A)
				turn = Player.B;
			else if (turn == Player.B)
				turn = Player.A;
		}

		public GameState CloneGS(){
			StateEvaluator newse = evaluator.Clone();
			GameState newgs = new GameState (newse);
			newgs.PlayerATokenMask = PlayerATokenMask;
			newgs.PlayerBTokenMask = PlayerBTokenMask;
			newgs.turn = turn;
			newgs.moves = moves;
			return newgs;
		}

		public mmTreeState Clone(){
			return CloneGS();
		}

		private int CoordsToN(int x, int y){
			return y * rowWidth + x;
		}


		public Player GetField(int x, int y){
			if (PlayerATokenMask[CoordsToN (x, y)])
				return Player.A;
			if (PlayerBTokenMask[CoordsToN (x, y)])
				return Player.B;
			return Player.NONE;
		}

		public int GetColumnHeight(int x){
			for (int i = 0; i <=5; i++) {
				if(PlayerATokenMask.GetBit( CoordsToN(x,i) ) || PlayerBTokenMask.GetBit( CoordsToN(x,i) ))
					return 6 - i;
			}
			return 0;
		}

		private void SetField(int x, int y, Player p){
			int n = CoordsToN (x, y);
			if (p == Player.NONE) {
				PlayerATokenMask[n] = false;
				PlayerBTokenMask[n] = false;
			}else if (p == Player.A) {
				PlayerATokenMask[n] = true;
				PlayerBTokenMask[n] = false;
			}else if (p == Player.B) {
				PlayerATokenMask[n] = false;
				PlayerBTokenMask[n] = true;
			}
		}

		public String PrintState(){
			string s = "";
			for(int y = 0; y < 6; y++){
				for (int x = 0; x < rowWidth; x++) {
					Player p = GetField (x, y);
					if (p == Player.NONE) s += ".";
					if (p == Player.A)    s += "O";
					if (p == Player.B)    s += "X";
				}
				s += "\n";
		    }
			s += evaluator.DebugState();
			return s;
		}

		public String PrintStateBIG(){
			string s = "";
			for(int y = 0; y < 6; y++){
				s += "+--1--+--2--+--3--+--4--+--5--+--6--+--7--+\n";
				for (int x = 0; x < rowWidth; x++) {
					Player p = GetField(x, y);
					if (p == Player.NONE) s += "|     ";
					if (p == Player.A)    s += "|0OOO0";
					if (p == Player.B)    s += "| X X ";
				}
				s += "|\n";
				for (int x = 0; x < rowWidth; x++) {
					Player p = GetField(x, y);
					if (p == Player.NONE) s += "|     ";
					if (p == Player.A)    s += "|O   O";
					if (p == Player.B)    s += "|  X  ";
				}
				s += "|\n";
				for (int x = 0; x < rowWidth; x++) {
					Player p = GetField(x, y);
					if (p == Player.NONE) s += "|     ";
					if (p == Player.A)    s += "|0OOO0";
					if (p == Player.B)    s += "| X X ";
				}
				s += "|\n";
			}
			s += "+--1--+--2--+--3--+--4--+--5--+--6--+--7--+\n";
			return s;
		}
	}
}

