using System;
using System.Collections.Generic;

namespace learning
{
	public struct State{
		public int x, y;
		public State(int _x, int _y){
			x = _x; y = _y;
		}
		public override String ToString(){
			return "{" + x + "," + y + "}";
		}
	}

	public struct Move{
		enum Moves
		{
			MoveUp,
			MoveDown,
			MoveLeft,
			MoveRight
		}
		public static readonly Move UP = new Move(Moves.MoveUp);
		public static readonly Move DOWN = new Move(Moves.MoveDown);
		public static readonly Move RIGHT = new Move(Moves.MoveRight);
		public static readonly Move LEFT = new Move(Moves.MoveLeft);

		Moves m;
		Move(Moves _m){
			m = _m;
		}
		public static Move operator -(Move m){
			if (m.m == Moves.MoveUp)         return DOWN;
			else if (m.m == Moves.MoveDown)  return UP;
			else if (m.m == Moves.MoveLeft)  return RIGHT;
			else if (m.m == Moves.MoveRight) return LEFT;
			else throw new ArgumentException ();
		}
		public override String ToString(){
			if (m == Moves.MoveUp)
				return "^";
			else if (m == Moves.MoveDown)
				return "v";
			else if (m == Moves.MoveLeft)
				return "<";
			else if (m == Moves.MoveRight)
				return ">";
			else
				return "?";
		}
		public static State operator +(State s, Move m){
			if (m.m == Moves.MoveUp)    s.y--;
			if (m.m == Moves.MoveDown)  s.y++;
			if (m.m == Moves.MoveLeft)  s.x--;
			if (m.m == Moves.MoveRight)
				s.x++;
			return s;
		}
		public Move Perpendicular(){
			if      (m == Moves.MoveUp)    return LEFT;
			else if (m == Moves.MoveDown)  return RIGHT;
			else if (m == Moves.MoveLeft)  return DOWN;
			else if (m == Moves.MoveRight) return UP;
			else throw new ArgumentException ();
		}
		public static IEnumerable<Move> All(){
			yield return UP;
			yield return DOWN;
			yield return LEFT;
			yield return RIGHT;
			yield break;
		}
		public int ToInt(){
			return (int)m;
		}
		static Random rnd = new Random();
		public static Move Random(){
			return new Move ((Moves)(rnd.Next () % 4));
		}
	}
}

