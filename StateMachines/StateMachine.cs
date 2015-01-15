using UnityEngine;
using System;
using System.Collections.Generic;

namespace Zedarus.ToolKit.StateMachines
{
	public class StateMachine
	{
		#region Properties
		private List<StateMachineState> _states;
		private int _currentStateIndex = -1;
		#endregion

		#region Initialization
		public StateMachine()
		{
			_states = new List<StateMachineState>();
			_currentStateIndex = -1;
		}
		#endregion

		#region Controls
		public void Update(float deltaTime)
		{
			if (_currentStateIndex >= 0 && _currentStateIndex < _states.Count)
				_states[_currentStateIndex].Update(deltaTime);
		}

		public void CreateState(int state, Action<float> CycleHandler, Action EnterHandler, Action ExitHandler)
		{
			_states.Add(new StateMachineState(state, CycleHandler, EnterHandler, ExitHandler));
		}

		public void ChangeState(int newState)
		{
			int nextStateIndex = -1;
			for (int i = 0; i < _states.Count; i++)
			{
				if (_states[i].State == newState)
				{
					nextStateIndex = i;
				}
			}

			if (nextStateIndex == -1)
				return;

			if (nextStateIndex != _currentStateIndex)
			{
				if (_currentStateIndex != -1)
					_states[_currentStateIndex].Exit();

				_states[nextStateIndex].Enter();
				_currentStateIndex = nextStateIndex;
			}
		}
		#endregion
	}
}
