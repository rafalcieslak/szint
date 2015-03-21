using System;
using System.Collections.Generic;

namespace Connect4
{
	public class mmNode
	{
		public enum Mode{
			MIN,
			MAX
		};
		readonly mmTreeState state;
		public double value = Double.NaN;
		double alpha = Double.NegativeInfinity;
		double beta = Double.PositiveInfinity;
		public int bestmove = -1;

		public Dictionary<int,mmNode> children = new Dictionary<int, mmNode>();

		private static Mode Not(Mode m){
			if (m == Mode.MAX) return Mode.MIN;
			else return Mode.MAX;
		}

		public mmNode (mmTreeState s) {
			state = s.Clone();
		}

		public void Run(int depth, Mode md){
			if (state.isWon() == Player.A || state.isWon() == Player.B) {
				alpha = beta = value = depth * state.Evaluate(); // multiplying win score by depth, to promote winning asap
			}else if (depth == 0) {
				alpha = beta = value = state.Evaluate();
				//Console.WriteLine("DEPTH 0, v=  " + value);
			} else {
				List<int> moves = state.ListValidMoves();
				foreach (int move in moves) {
					//Console.WriteLine("trying move " + move);
					//Console.WriteLine("alpha " + alpha);
					//Console.WriteLine("beta " + beta);
					mmNode newnode;

					if (children.ContainsKey(move)) {
						newnode = children [move];
						newnode.alpha = alpha;
						newnode.beta = beta;
						newnode.Run(depth - 1, Not(md));
					} else {
						mmTreeState newstate = state.Clone();
						newstate.PerformMove(move);

						newnode = new mmNode (newstate);
						newnode.alpha = alpha;
						newnode.beta = beta;
						newnode.Run(depth - 1, Not(md));

						children.Add(move, newnode);
					}

					if (md == Mode.MIN) {
						beta = Math.Min(beta, newnode.value);
						if (Double.IsNaN(value) || value > newnode.value) {
							value = newnode.value;
							bestmove = move;
						}
					} else {
						alpha = Math.Max(alpha, newnode.value);
						if (Double.IsNaN(value) || value < newnode.value) {
							value = newnode.value;
							bestmove = move;
						}
					}

					if (beta < alpha)
						break; // pruning

				}
			}
		}

		public void Reset(){
			value = 0;
			alpha = Double.NegativeInfinity;
			beta = Double.PositiveInfinity;
			value = Double.NaN;
			foreach (mmNode ch in children.Values)
				ch.Reset();
		}

		public mmNode GetChildNode(int move){
			mmNode res;
			if (children.ContainsKey(move))
				res = children[move];
			else {
				mmTreeState newstate = state.Clone();
				newstate.PerformMove(move);
				res = new mmNode (newstate);
			}
			return res;
		}
	}
}

