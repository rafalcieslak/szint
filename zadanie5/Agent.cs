using System;
using System.Collections.Generic;

namespace learning
{
	public class Agent
	{
		UsabilityMap optimalU;
		PolicyMap optimalP;
		QFunction optimalQ;
		World world;

		public Agent (World w)
		{
			world = w;
			optimalU = new UsabilityMap (world);
			optimalU.DefaultFromWorld ();
			optimalP = PolicyMap.CreateFromUsabilities (optimalU);
		}

		public void PolicyIteration(double discount, ref string log, bool interactive){
			UsabilityMap U, newU;
			PolicyMap P;

			// log header
			log = "n ";
			foreach (State s in world.GetAllowedStates()) {
				log += s + " ";
			}
			log += "\n";

			// step 0
			U = new UsabilityMap (world);
			U.DefaultFromWorld ();
			P = PolicyMap.CreateFromUsabilities (U);

			int n = 0;
			while (true) {
				if (interactive) {
					U.Display ();
					P.Display ();
				}
				if (interactive) {
					Console.WriteLine ("Press Return to coutinue...");
					Console.ReadLine ();
				}
				newU = UsabilityMap.CreateUpdatedFromPolicy (P, U, discount);
				n++;
				log += n + " " + newU.LogData () + "\n";
				if(UsabilityMap.Similar(U,newU)){
					Console.WriteLine ("Converged in " + n + " steps.");
					break;
				}
				U = newU;
				P = PolicyMap.CreateFromUsabilities (U);
			}

			optimalU = newU;
			optimalP = P;
		}

		public void Qlearn(double discount, ref string log, double epsilon, bool interactive){
			// init
			PolicyMap P = new PolicyMap(world);
			QFunction Q = new QFunction (world);

			// log header
			log = "n ";
			foreach (State s in world.GetAllowedStates()) {
				log += s + " ";
			}
			log += "\n";

			for (int i = 1; i < 100000; i++) {
				// wygeneruj trase uzywajac polityki i epsilona
				Route route = new Route ();
				Actor actor = world.SpawnNewActor ();
				Move m;
				while (!actor.Terminated ()) {
					if (world.rnd.NextDouble () <= epsilon) {
						m = P.GetRecommendedMove (actor.s);
					} else {
						m = Move.Random ();
					}
					//State oldstate = actor.s;
					route.Append (actor.s, m, actor.Reward());
					actor.PerformMove (m);
				}
				route.Append (actor.s, Move.UP, actor.Reward ());

				// podaj trase qfunkcji, niech sie uczy
				Q.Learn (route,discount);

				// od czasu do czasu popraw polityke
				if (i % 10 == 0)
					P = PolicyMap.CreateFromQFunction (Q);

				// od czasu do czasu wpakuj wartosci do loga
				if (i % 50 == 1) {
					optimalU = UsabilityMap.CreateFromQFunction (Q);
					log += i + " " + optimalU.LogData () + "\n";
				}

				if (interactive) {
					optimalU = UsabilityMap.CreateFromQFunction (Q);
					optimalP = PolicyMap.CreateFromQFunction (Q);
					optimalU.Display ();
					optimalP.Display ();
					Console.WriteLine ("Press Return to coutinue...");
					Console.ReadLine ();
				}
			}
			optimalU = UsabilityMap.CreateFromQFunction (Q);
			optimalP = PolicyMap.CreateFromQFunction (Q);
			optimalQ = Q;
		}

		public void Display(){
			Console.WriteLine ("Usabilities:");
			optimalU.Display ();
			Console.WriteLine ("Policy:");
			optimalP.Display ();
		}

		public void DisplayQ(){
			Console.WriteLine ("Q function:");
			optimalQ.Display ();
		}
	}
}

