using System;
using System.Collections.Generic;

namespace learning
{
	public class Route
	{
		struct RouteItem{
			public State s;
			public Move m;
			public double val;
			public RouteItem(State _s, Move _m, double _val){
				s = _s;
				m = _m;
				val = _val;
			}
		}
		LinkedList<RouteItem> list;
		LinkedList<RouteItem> r;

		public Route ()
		{
			list = new LinkedList<RouteItem> ();
			r = new LinkedList<RouteItem> ();
		}
		public void Append(State s, Move m, double val){
			list.AddLast (new RouteItem (s, m, val));
		}
		public State LastState(){
			return list.Last.Value.s;
		}
		public Move LastMove(){
			return list.Last.Value.m;
		}
		public double LastReward(){
			return list.Last.Value.val;
		}
		public bool Empty(){
			return list.Count == 0;
		}
		public void Retract(){
			r.AddFirst (list.Last.Value);
			list.RemoveLast ();
		}
	}
}

