using System;
using Microsoft.WindowsAPICodePack.Sensors;

namespace NexusIM.Managers
{
	public class SensorUtilityManager
	{
		public static void Setup()
		{
			SensorList<AmbientLightSensor> sensors;
			try	{
				sensors = SensorManager.GetSensorsByTypeId<AmbientLightSensor>();
			} catch (Exception) {
				return;
			}

			sensors = sensors;
		}
	}
}