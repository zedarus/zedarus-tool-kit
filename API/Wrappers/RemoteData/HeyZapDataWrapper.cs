using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
#if API_ADS_HEYZAP
using Heyzap;
#endif

namespace Zedarus.ToolKit.API
{
	public class HeyZapDataWrapper : APIWrapper<HeyZapDataWrapper>, IRemoteDataWrapperInterface
	{
		#region Events
		public event Action<string> DataReceived;
		#endregion

		#region Properties
		private string _remoteData = null;
		#endregion

		#region Setup
		protected override void Setup(APIWrapperSettings settings)
		{
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return null;
		}
		#endregion

		#region Controls
		public void RequestData()
		{
		}
		#endregion

		#region Queries
		public string RemoteData
		{
			get
			{
				return _remoteData;
			}
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners()
		{
			#if API_ADS_HEYZAP
			Heyzap.HeyzapAds.SetNetworkCallbackListener(NetworkCallbackListener);
			#endif
		}

		protected override void RemoveEventListeners()
		{
			#if API_ADS_HEYZAP
			Heyzap.HeyzapAds.SetNetworkCallbackListener(null);
			#endif
		}
		#endregion

		#region Event Handlers
		private void NetworkCallbackListener(string network, string callback)
		{
			if ((network.Equals(Heyzap.HeyzapAds.Network.HEYZAP_EXCHANGE) || network.Equals(Heyzap.HeyzapAds.Network.HEYZAP)) && callback.Equals(Heyzap.HeyzapAds.NetworkCallback.INITIALIZED))
			{
				_remoteData = Heyzap.HeyzapAds.GetRemoteData();

				if (DataReceived != null)
				{
					DataReceived(RemoteData);
				}
			}
		}
		#endregion
	}
}
