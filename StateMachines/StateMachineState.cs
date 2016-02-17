using UnityEngine;
using System;
using System.Collections.Generic;

namespace Zedarus.ToolKit.StateMachines
{
	public class StateMachineState
	{
		#region Properties
		private int _state;
		private Action<float> _cycleHandler;
		private Action _enterHandler;
		private Action _exitHandler;
		private Func<bool> _enterConditionHandler;
		#endregion

		#region Initialization
		public StateMachineState(int state, Action<float> CycleHandler, Action EnterHandler, Action ExitHandler, Func<bool> EnterConditionHandler)
		{
			_state = state;
			_cycleHandler = CycleHandler;
			_enterHandler = EnterHandler;
			_exitHandler = ExitHandler;
			_enterConditionHandler = EnterConditionHandler;
		}
		#endregion

		#region Controls
		public void Update(float deltaTime)
		{
			if (_cycleHandler != null)
				_cycleHandler(deltaTime);
		}

		public void Enter()
		{
			if (_enterHandler != null)
				_enterHandler();
		}

		public void Exit()
		{
			if (_exitHandler != null)
				_exitHandler();
		}

		public bool CheckEnterCondition()
		{
			if (_enterConditionHandler != null)
				return _enterConditionHandler();
			else
				return true;
		}

		public void Destroy()
		{
			_state = 0;
			_cycleHandler = null;
			_enterHandler = null;
			_exitHandler = null;
			_enterConditionHandler = null;
		}
		#endregion

		#region Getters
		public int State
		{
			get { return _state; }
		}
		#endregion
	}
}
