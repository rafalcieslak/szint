using System;
using System.Collections.Generic;
namespace learning
{
	public class PolicyMap{
		World world;
		Move[,] map;
		public PolicyMap(World w){
			world = w;
			Clear();
		}
		public void Clear(){
			map = new Move[world.xsize, world.ysize];
		}
		public void Display(){
			for (int y = 0; y < world.ysize; y++) {
				for (int x = 0; x < world.xsize; x++) {
					if(world.GetStateType(new State(x,y)) == World.StateType.StateTypeTerminal)
						Console.Write (".|");
					else if(world.GetStateType(new State(x,y)) == World.StateType.StateTypeForbidden)
						Console.Write (" |");
					else 
						Console.Write ("{0}|", map [x, y].ToString ());
				}
				Console.Write ("\n");
			}
			Console.Write ("\n");
		}

		public Move GetRecommendedMove(State s){
			return map [s.x, s.y];
		}

		public static PolicyMap CreateFromUsabilities(UsabilityMap U){
			World world = U.world;
			PolicyMap P = new PolicyMap (world);
			foreach (State s in world.GetStates()) {
				MoveEstimate ExpectedMoveValue = (m) => {
					double sum = 0;
					foreach (KeyValuePair<State,double> k in world.GetDigressedStates(s,m)) {
						sum += k.Value * U.GetUsability (k.Key); // will check only non-forbidden states
					}
					return sum;
				};
				KeyValuePair<Move,double> kp = Max(new Move[]{Move.UP, Move.LEFT, Move.DOWN, Move.RIGHT}, ExpectedMoveValue);
				P.map [s.x, s.y] = kp.Key;
			}
			return P;
		}
		delegate double MoveEstimate(Move m);
		static KeyValuePair<Move,double> Max (Move[] list, MoveEstimate f){
			double max = Double.NegativeInfinity;
			Move best = list[0];
			foreach (Move move in list) {
				double n = f (move);
				//Console.Write (" for " + move + " val is " + n + " ");
				if (n > max) {
					best = move;
					max = n;
				}
			}
			return new KeyValuePair<Move,double>(best,max);
		}

		public static PolicyMap CreateFromQFunction(QFunction q){
			World world = q.world;
			PolicyMap p = new PolicyMap (world);
			foreach (State s in world.GetStates()) {
				Move best = Move.UP;
				double bestval = double.NegativeInfinity;
				foreach (Move m in Move.All()) {
					double newval = q.GetValue (s, m);
					if (newval > bestval) {
						bestval = newval;
						best = m;
					}
				}
				p.map [s.x, s.y] = best;
			}
			return p;
		}
	}
}

