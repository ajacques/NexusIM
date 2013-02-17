using System.Text;

namespace InstantMessage.Protocols.AudioVideo
{
	public abstract class SdpPayloadType
	{
		protected SdpPayloadType(int id, string name)
		{
			Id = id;
			Name = name;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("Payload: [{1}] {0} - {2} Channel", Name, Id, Channels);
			if (Channels != 1)
				sb.Append('s');

			if (ClockRate.HasValue)
				sb.AppendFormat(" - {0:N0} KHz", ClockRate.Value / 1000);

			return sb.ToString();
		}

		public int Id
		{
			get;
			private set;
		}
		public string Name
		{
			get;
			private set;
		}

		public int? ClockRate
		{
			get;
			set;
		}
		public int Channels
		{
			get;
			set;
		}
	}
}
