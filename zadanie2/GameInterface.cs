using System;

namespace Connect4
{
	public class GameInterface
	{
		GameState gs;
		mmNode node;

		private int DepthFunc(int moves){
			if (moves < 4)
				return 5;
			if (moves < 8)
				return 7;
			if (moves < 18)
				return 9;
			if (moves < 22)
				return 10;
			else
				return 12;
		}

		private void PerformAIMove(){
			Console.WriteLine("Thinking...");
			node.Run(DepthFunc(gs.moves), mmNode.Mode.MAX);
			Console.WriteLine("Best move: " + (node.bestmove+1) + " (" + node.value + ")");
			gs.PerformMove(node.bestmove);
			node = node.GetChildNode(node.bestmove);
			node.Reset();
			Console.Write(gs.PrintStateBIG());
		}

		private void PerformHumanMove(){
			Console.Write("Choose your move [1-7] or ask for help [?]. ");
			char c = '\0'; 
			while (!(c <= '7' && c >= '1') && c != '?')
				c = Console.ReadKey().KeyChar;
			Console.Write("\n");
			if (c == '?') {
				Console.WriteLine("Thinking...");
				node.Run(10, mmNode.Mode.MIN);
				Console.WriteLine("Recommended move: " + (node.bestmove+1) + " (" + node.value + ")");
			}else{
				int move = c - '0' - 1;
				if (!gs.PerformMove(move)) {
					Console.WriteLine("Invalid move!");
					return;
				};
				node = node.GetChildNode(move);
				node.Reset();
				Console.Write(gs.PrintStateBIG());
			}
		}

		public void Run () {
			StateEvaluator se = new EvaluatorA ();
			gs = new GameState (se);

			Console.Write("Do you wish to play first? [y/n]");

			char c = '\0'; 
			while (c != 'y' && c !='n')
				c = Console.ReadKey().KeyChar;
			Console.Write("\n");
			if (c == 'n') {
				gs.NextTurn();
			}
			node = new mmNode (gs);
			
			Console.Write(gs.PrintStateBIG());

			while (true) {
				if (gs.turn == Player.B) PerformAIMove();
				else if (gs.turn == Player.A) PerformHumanMove();
				if (gs.isWon() == Player.A) {
					Console.WriteLine("Congratulations, you win!");
					return;
				}
				else if (gs.isWon() == Player.B) {
					Console.WriteLine("The AI wins!");
					return;
				}
			}

		}
	}
}

