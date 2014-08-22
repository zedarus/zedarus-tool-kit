using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Gameplay.Controllers
{
	public class Controller : MonoBehaviour
	{
		#region Unity Methods
		private void Start()
		{
			Init();
		}
		
		private void Update()
		{
			Cycle(Time.deltaTime);
		}
		
		private void OnDestroy()
		{
			Destroy();
		}
		#endregion
		
		#region Initialization
		protected virtual void Init()
		{
			CreateEventListeners();
		}
		
		protected virtual void Destroy()
		{
			RemoveEventListeners();
		}
		#endregion
		
		#region Update
		protected virtual void Cycle(float deltaTime)
		{
			
		}
		#endregion
		
		#region Event Listeners
		protected virtual void CreateEventListeners()
		{
		}
		
		protected virtual void RemoveEventListeners()
		{
		}
		#endregion
	}
}
