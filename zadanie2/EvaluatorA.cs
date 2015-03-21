using System;

namespace Connect4
{
	public class EvaluatorA : StateEvaluator
	{
		GameState game;

		//Function to get random number
		private static readonly Random rnd = new Random(Guid.NewGuid().GetHashCode());
		public static double getRand()
		{
			return rnd.Next(0, 10000)/10000.0;
		}

		byte len4A = 0;
		byte len4B = 0;
		byte len3A = 0;
		byte len3B = 0;

		public void Link(GameState s){
			game = s;
		}

		public double Evaluate(){
			/*
			if (len4A > 0) return -1000.0;
			if (len4B > 0) return 1000.0;
			return -2.0*len3A + 2.0*len3B;
			*/
			return -1000.0*len4A + 1000.0*len4B -2.0*len3A + 2.0*len3B + 0.1*getRand();
		}
		
		private bool isX(int x, int y, Player p){
			//if(isUnreachable(x,y)) return false;
			return game.GetField(x,y) == p;
		}
		private bool isUnreachable(int x, int y){
			return x < 0 || x > 6 || y < 0 || y > 5;
		}
		private Player other(Player p){
			if(p == Player.A) return Player.B;
			if(p == Player.B) return Player.A;
			return Player.NONE;
		}
		private void add4(Player p, short n){
			if(p == Player.A) len4A+=(byte)n;
			if(p == Player.B) len4B+=(byte)n;
		}
		private void add3(Player p, short n){
			if(p == Player.A) len3A+=(byte)n;
			if(p == Player.B) len3B+=(byte)n;
		}

		private Tuple<int, Player> Count(int x1, int y1, int xdir, int ydir, Player p){
			int x = x1 + xdir;
			int y = y1 + ydir;
			int n = 0;
			while (true) {
				//Console.WriteLine("trying " + x + " " + y);
				if (isUnreachable(x, y)) return new Tuple<int, Player> (n, Player.UNREACHABLE);
				if (isX(x, y, Player.NONE)) return new Tuple<int, Player> (n, Player.NONE);
				if (isX(x, y, other(p))) return new Tuple<int, Player> (n, other(p));
				x += xdir;
				y += ydir;
				n++;
			}
		}

		public void TestAxis(int x1, int y1, int xdir, int ydir, Player p){
			Tuple<int, Player> a = Count(x1, y1, xdir, ydir, p);
			Tuple<int, Player> b = Count(x1, y1,-xdir,-ydir, p);
			int len = a.Item1 + b.Item1 + 1;
			if (len >= 4) add4(p, (short)(1 + len - 4));
			if (len == 3 && (a.Item2 == Player.NONE || b.Item2 == Player.NONE)) add3(p, 1);
		}
		public void TestLock(int x1, int y1, int xdir, int ydir, Player p){
			if(!isUnreachable(x1+xdir,y1+ydir) && isX(x1+xdir,y1+ydir,other(p)) ){
				Tuple<int, Player> t = Count(x1, y1, xdir, ydir, other(p) );
				if (t.Item1 == 3 && t.Item2 != Player.NONE) add3(other(p), -1);
			}
		}

		public void Update(int x, int y){
			Player p = game.GetField(x, y);
			//Console.WriteLine("updating " + p);
			if (p == Player.NONE) return;
			
			TestAxis(x, y, 1, 0, p);
			TestAxis(x, y, 0, 1, p);
			TestAxis(x, y, 1, 1, p);
			TestAxis(x, y, 1,-1, p);
			
			TestLock(x, y, 1, 0, p);
			TestLock(x, y,-1, 0, p);
			TestLock(x, y, 1, 1, p);
			TestLock(x, y,-1, 1, p);
			TestLock(x, y, 1, -1, p);
			TestLock(x, y,-1, -1, p);
			TestLock(x, y, 0, 1, p);
		}

		public Player isWon(){
			if(len4A != 0) return Player.A;
			if(len4B != 0) return Player.B;
			return Player.NONE;
		}
		public string DebugState(){
			string s = "";
			s += "4A: " + len4A + "\n";
			s += "4B: " + len4B + "\n";
			s += "3A: " + len3A + "\n";
			s += "3B: " + len3B + "\n";
			s += "Value: " + Evaluate() + "\n";
			return s;
		}

		public EvaluatorA CloneEA(){
			EvaluatorA ea = new EvaluatorA();
			ea.len3A = len3A;
			ea.len3B = len3B;
			ea.len4A = len4A;
			ea.len4B = len4B;
			ea.Link(game);
			return ea;
		}
		public StateEvaluator Clone(){
			return CloneEA();
		}
	}
}

