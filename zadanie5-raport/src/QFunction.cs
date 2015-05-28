using System;
using System.Collections.Generic;

namespace learning
{
	struct QAtom{
		public double value;
		public int hits;
	}
	public class QFunction
	{
		QAtom[,,] map;
		public World world;
		public QFunction (World w)
		{
			world = w;
			Clear ();
		}
		public void Clear(){
			map = new QAtom[world.xsize, world.ysize, 4];
		}
		public double GetValue(State s, Move m){
			return map [s.x, s.y, m.ToInt ()].value;
		}
		double MaxVal(State s){
			double max = double.NegativeInfinity;
			foreach (Move m in Move.All()) {
				double n = map [s.x, s.y, m.ToInt()].value;
				if (n > max)
					max = n;
			}
			return max;
		}
		public void Learn(Route r, double discount){
			State terminal = r.LastState ();
			if (world.GetStateType (terminal) != World.StateType.StateTypeTerminal)
				throw new ArgumentException ();
			foreach (Move m in Move.All()) {
				map [terminal.x, terminal.y, m.ToInt ()].value = r.LastReward ();
			}
			r.Retract ();

			while (!r.Empty ()) {
				State s = r.LastState ();
				Move m = r.LastMove ();
				double val = r.LastReward ();

				if (world.GetStateType (s) == World.StateType.StateTypeTerminal) {
					// should never happen.
					throw new NotImplementedException ();
				} else {
					// mark hit!
					map[s.x, s.y, m.ToInt()].hits++;
					double alpha = 1.0 / map [s.x, s.y, m.ToInt()].hits;
					double origvalue = map [s.x, s.y, m.ToInt()].value;
					State movetarget = s + m;
					if (world.IsStateForbidden (movetarget))
						movetarget = s;
					double newvalue = val + discount*MaxVal(movetarget);
					double q = (1.0 - alpha) * origvalue + alpha * newvalue;
					map [s.x, s.y, m.ToInt ()].value = q;
				}
				r.Retract ();
			}
		}
		public void Display(){
			foreach (State s in world.GetStates()) {
				foreach (Move m in Move.All()) {
					Console.WriteLine ("State {0}, move '{1}': {2} (visited {3} times)", s.ToString(), m.ToString(), map[s.x,s.y,m.ToInt()].value, map[s.x,s.y,m.ToInt()].hits);
				}
			}
		}
	}
}

