using System;

namespace learning
{

	public class Actor
	{
		public State s;
		World world;

		public Actor (World w)
		{
			world = w;
		}

		public void PerformMove(Move m){
			Move m2 = world.PerformDigression (m);
			if (!world.IsStateForbidden (s + m2))
				s += m2;

		}

		public bool Terminated(){
			return world.GetStateType (s) == World.StateType.StateTypeTerminal;
		}
		public double Reward(){
			return world.GetImmediateReward (s);
		}
	}
}

