using System;
using System.Collections.Generic;

namespace learning
{
	public class UsabilityMap{
		public World world;
		double[,] map;
		public UsabilityMap(World w){
			world = w;
			Clear();
		}
		public void Clear(){
			map = new double[world.xsize, world.ysize];
		}
		public void Display(){
			for (int y = 0; y < world.ysize; y++) {
				for (int x = 0; x < world.xsize; x++) {
					Console.Write("{0}|",map[x,y].ToString("F4"));
				}
				Console.Write ("\n");
			}
			Console.Write ("\n");
		}
		public void DefaultFromWorld(){
			for (int y = 0; y < world.ysize; y++) {
				for (int x = 0; x < world.xsize; x++) {
					map [x, y] = world.GetImmediateReward (new State (x, y));
				}
			}
		}
		public double GetUsability(State s){
			if (world.IsStateForbidden(s))
				return Double.NegativeInfinity;
			else
				return map [s.x, s.y];
		}
		public static UsabilityMap CreateUpdatedFromPolicy(PolicyMap P, UsabilityMap U, double discount) {
			World world = U.world;
			UsabilityMap newU = new UsabilityMap (world);
			foreach (State s in U.world.GetStates()) {
				if (U.world.GetStateType (s) != World.StateType.StateTypeTerminal) {
					double sum = 0.0;
					Move m = P.GetRecommendedMove (s);
					foreach (KeyValuePair<State,double> k in world.GetDigressedStates(s,m)) {
						sum += k.Value * U.GetUsability (k.Key);
					}
					newU.map [s.x, s.y] = discount * sum + world.GetImmediateReward (s);
				} else {
					newU.map [s.x, s.y] = world.GetImmediateReward (s);
				}
			}
			return newU;
		}
		public static bool Similar(UsabilityMap u1, UsabilityMap u2){
			if (!ReferenceEquals (u1.world, u2.world))
				throw new ArgumentException ();
			foreach (State s in u1.world.GetStates()) {
				if (Math.Abs (u1.GetUsability (s) - u2.GetUsability (s)) > 0.00005)
					return false;
			}
			return true;
		}
		public string LogData(){
			string log = "";
			foreach (State s in world.GetAllowedStates()) {
				log += GetUsability (s) + " ";
			}
			return log;
		}
		public static UsabilityMap CreateFromQFunction(QFunction q){
			World world = q.world;
			UsabilityMap u = new UsabilityMap (world);
			foreach (State s in world.GetStates()) {
				double bestval = double.NegativeInfinity;
				foreach (Move m in Move.All()) {
					double newval = q.GetValue (s, m);
					if (newval > bestval) {
						bestval = newval;
					}
				}
				u.map [s.x, s.y] = bestval;
			}
			return u;
		}
	}
}

