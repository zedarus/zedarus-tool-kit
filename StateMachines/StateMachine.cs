using UnityEngine;
using System;
using System.Collections.Generic;

namespace Zedarus.ToolKit.StateMachines
{
	public class StateMachine
	{
		#region Properties
		private List<StateMachineState> _states;
		private int _currentStateIndex;
		private List<int> _statesHistory;
		private bool _changingStateInProgress = false;
		private int _queuedState = NONE;
		#endregion

		#region Settings
		private int _historyLenght;
		private const int NONE = -1;
		#endregion

		#region Initialization
		public StateMachine(int historyLenght = 10)
		{
			_states = new List<StateMachineState>();
			_statesHistory = new List<int>();
			_currentStateIndex = -1;
			_historyLenght = historyLenght;
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

		public void RevertToPreviousState(bool skipSameState = false)
		{
			if (_statesHistory.Count > 0)
			{
				int previousStateIndex = _statesHistory[_statesHistory.Count - 1];
				if (skipSameState)
				{
					int depth = 2;
					while (_currentStateIndex == previousStateIndex && _statesHistory.Count - depth >= 0)
					{
						previousStateIndex = _statesHistory[_statesHistory.Count - depth];
						depth++;
					}
				}
				ChangeState(_states[previousStateIndex].State);
			}
		}

		public void ChangeState(int newState, bool allowQueue = false)
		{
			if (_changingStateInProgress)
			{
				Debug.Log("You can't change state from state Enter of Exit methods. Use Update method instead");
				if (allowQueue)
					_queuedState = newState;
				return;
			}

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
				_changingStateInProgress = true;
				if (_currentStateIndex != -1)
					_states[_currentStateIndex].Exit();

				_states[nextStateIndex].Enter();

				_statesHistory.Add(_currentStateIndex);
				if (_statesHistory.Count > _historyLenght)
					_statesHistory.RemoveAt(0);
				_currentStateIndex = nextStateIndex;
				_changingStateInProgress = false;
			}

			if (_queuedState != NONE)
			{
				int qs = _queuedState;
				_queuedState = NONE;
				ChangeState(qs);
			}
		}

		public void Destroy()
		{
			foreach (StateMachineState state in _states)
				state.Destroy();

			_states.Clear();
			_states = null;
		}
		#endregion

		#region Queries
		public int CurrentState
		{
			get { return _states[_currentStateIndex].State; }
		}
		#endregion
	}
}
