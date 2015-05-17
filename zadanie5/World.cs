using System;
using System.Collections.Generic;

namespace learning
{
	public class World
	{
		public enum StateType{
			StateTypeNormal,
			StateTypeTerminal,
			StateTypeSpecial,
			StateTypeForbidden
		}

		public readonly int xsize, ysize;

		StateType[,] map;

		public Random rnd = new Random ();

		public double normal_reward = -1.0;
		public double special_reward = 5.0;
		public double terminal_reward = 10.0;
		public State initial_state = new State(0,0);
		public double digression_probability = 0.1;

		public World (int x, int y)
		{
			xsize = x;
			ysize = y;
			map = new StateType[xsize, ysize];
		}
		public void SetStateType(int x, int y, StateType s){
			map [x, y] = s;
		}
		public StateType GetStateType(State s){
			if (s.x < 0 || s.y < 0 || s.x >= xsize || s.y >= ysize)
				return StateType.StateTypeForbidden;
			else
				return map [s.x, s.y];
		}

		public bool IsStateForbidden(State s){
			return GetStateType (s) == StateType.StateTypeForbidden;
		}

		public Actor SpawnNewActor(){
			Actor a = new Actor (this);
			a.s = initial_state;
			return a;
		}

		public double GetImmediateReward(State s){
			switch (GetStateType (s)) {
			case StateType.StateTypeNormal:
				return normal_reward;
			case StateType.StateTypeSpecial:
				return special_reward;
			case StateType.StateTypeTerminal:
				return terminal_reward;
			default:
				return 0.0;
			}
		}

		public IEnumerable<State> GetStates(){
			for (int y = 0; y < ysize; y++) {
				for (int x = 0; x < xsize; x++) {
					yield return new State (x, y);
				}
			}
			yield break;
		}
		public IEnumerable<State> GetAllowedStates(){
			for (int y = 0; y < ysize; y++) {
				for (int x = 0; x < xsize; x++) {
					if(!IsStateForbidden(new State(x,y)))
						yield return new State (x, y);
				}
			}
			yield break;
		}

		public Move DigressA(Move m){
			return m.Perpendicular ();
		}
		public Move DigressB(Move m){
			return -m.Perpendicular ();
		}

		public Move PerformDigression(Move m){
			double q = rnd.NextDouble ();
			foreach (KeyValuePair<Move,double> k in GetDigressedMoves(m)) {
				if (q < k.Value)
					return k.Key;
				q -= k.Value;
			}
			throw new ArgumentOutOfRangeException (); //tmp
		}

		public IEnumerable<KeyValuePair<Move,double>> GetDigressedMoves(Move m){
			if (digression_probability != 0.0) {
				yield return new KeyValuePair<Move, double> (DigressA (m), digression_probability);
				yield return new KeyValuePair<Move, double> (DigressB (m), digression_probability);
			}
			yield return new KeyValuePair<Move, double> (m, 1.0 - 2.0*digression_probability);
			yield break;
		}

		public IEnumerable<KeyValuePair<State,double>> GetDigressedStates(State s, Move m){
			foreach (KeyValuePair<Move, double> k in GetDigressedMoves(m)) {
				State newstate = s + k.Key;
				if (IsStateForbidden (newstate))
					newstate = s;
				if(k.Value != 0.0)
					yield return new KeyValuePair<State,double> (newstate, k.Value);
			}
			yield break;
		}
	}
}

