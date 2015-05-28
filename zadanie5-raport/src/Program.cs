using System;

namespace learning
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// Access dataset file
			Console.WriteLine ("Data set?");
			string dataset = "datasets" + System.IO.Path.DirectorySeparatorChar + Console.ReadLine ();
			Console.WriteLine ("Opening file " + dataset + "...");
			string[] lines;
			try{
				lines = System.IO.File.ReadAllLines(dataset);
				Console.WriteLine ("No such data file.");
			}catch(System.IO.FileNotFoundException){
				return;
			}catch(System.IO.DirectoryNotFoundException){
				Console.WriteLine ("Dataset directory missing.");
				return;
			}

			// Parse world properties
			string[] nm = lines [0].Split (new char[]{' '},3);
			int n = Int32.Parse (nm [0]);
			int m = Int32.Parse (nm [1]);
			Console.WriteLine ("World dimensions: " + n.ToString () + " x " + m.ToString ());
			double dp = Double.Parse (lines [1].Split(new char[]{' '},2)[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
			Console.WriteLine ("Digression probability = 2*" + dp);
			double nr = Double.Parse (lines [2].Split(new char[]{' '},2)[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
			double sr = Double.Parse (lines [3].Split(new char[]{' '},2)[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
			double tr = Double.Parse (lines [4].Split(new char[]{' '},2)[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
			Console.WriteLine ("Rewards: " + nr + "," + sr + "," + tr + ".");
			double discount = Double.Parse (lines [5].Split(new char[]{' '},2)[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
			Console.WriteLine ("Discount: " + discount);

			World w = new World (n, m);
			w.digression_probability = dp;
			w.normal_reward = nr;
			w.special_reward = sr;
			w.terminal_reward = tr;

			bool interactive = false;
			bool qlearning = false;
			double epsilon = 0.0;

			Console.Write ("Q-learning or Policy iteration? [q/P] ");
			char c = (char)Console.Read ();
			if (c == 'q')
				qlearning = true;
			Console.WriteLine ();

			if (qlearning) {
				Console.Write ("Epsilon? ");
				epsilon = double.Parse (Console.ReadLine (), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
			}

			Console.Write ("Interactive mode? [y/N] ");
			c = (char)Console.Read ();
			if (c == 'y')
				interactive = true;
			Console.WriteLine ();

			// Parse world map
			for (int y = 0; y < m; y++) {
				for (int x = 0; x < n; x++) {
					switch (lines [6 + y] [x]) {
					case '.':
						w.SetStateType (x, y, World.StateType.StateTypeNormal);
						break;
					case 'X':
						w.SetStateType (x, y, World.StateType.StateTypeForbidden);
						break;
					case 'T':
						w.SetStateType (x, y, World.StateType.StateTypeTerminal);
						break;
					case 'S':
						w.SetStateType (x, y, World.StateType.StateTypeSpecial);
						break;
					case 'I':
						w.SetStateType (x, y, World.StateType.StateTypeNormal);
						w.initial_state = new State (x, y);
						break;
					default:
						throw new ArgumentException ();
					}
				}
			}

			// Run algorithm
			String log = "";
			Agent a = new Agent(w);
			if (qlearning)
				a.Qlearn (discount, ref log, epsilon, interactive);
			else
				a.PolicyIteration (discount,ref log, interactive);

			// Print out results
			if (qlearning)
				a.DisplayQ ();
			a.Display ();

			// Gnuplot data
			Console.Write ("Generate gnuplot data? [y/N] ");
			c = (char)Console.Read ();
			if (c == 'y') {
				// Data log
				System.IO.File.WriteAllText ("log.txt", log);

				// Gnuplot script
				string gpcommand = "set terminal png size 1200,600 enhanced font \"Sans,10\" \nset output \"plot.png\"\nplot ";
				int col = 2;
				foreach (State s in w.GetAllowedStates()) {
					gpcommand += "\"log.txt\" using 1:" + col + " with lines title columnheader(" + col + "), ";
					col++;
				}
				System.IO.File.WriteAllText ("gnuplot.txt", gpcommand.Remove (gpcommand.Length - 2) + "\n");
			}
			Console.WriteLine ();
		}

	}
}
