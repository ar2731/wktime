using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace wktime
{
	public class WorkTimeData
	{
        private static readonly string DefaultNormalizedStrSeparator = " - ";

        private static readonly char[] Separators = new char[]
		{
			' ', '.', ',', ';', '-', '_', '|', '!', '*', '/',
			'+', '·', '•', '➔'
		};

		public DateTime Date { get; }
		public TimeSpan StartTime => Timestamps[0];
		public TimeSpan EndTime => Timestamps[Timestamps.Count - 1];
		public TimeSpan BreakDuration { get; }
		public  ReadOnlyCollection<BreakInterval> BreakIntervals { get; }
		public TimeSpan WorkHours => EndTime - StartTime - BreakDuration;
		public ReadOnlyCollection<TimeSpan> Timestamps { get; }
		public string DbStartTime => string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd} {1}", Date, FormatTimeSpanHHmm(StartTime));
		public string DbEndTime => string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd} {1}", Date, FormatTimeSpanHHmm(EndTime));
		public string DbBreakDuration => BreakDuration.TotalHours.ToString("F2", CultureInfo.InvariantCulture);
		public string DbWorkHours => WorkHours.TotalHours.ToString("F2", CultureInfo.InvariantCulture);
		public string HumanReadableStats
		{
			get
			{
				StringBuilder sb = new StringBuilder();

				sb.AppendFormat("Date: {0:d}{1}", Date, Environment.NewLine);
				sb.AppendFormat("Start: {0} - End: {1}{2}", FormatTimeSpanHHmm(StartTime), FormatTimeSpanHHmm(EndTime), Environment.NewLine);
				sb.AppendFormat("Break total time: {0}{1}", FormatTimeSpanHHmm(BreakDuration), Environment.NewLine);
				sb.AppendFormat("N. of breaks taken: {0}{1}", BreakCount, Environment.NewLine);

				if (BreakCount > 0)
				{
					for (int i = 0; i < BreakCount; i++)
					{
						sb.AppendFormat("   Break {0}: {1} - {2} ({3}){4}",
							i + 1,
							FormatTimeSpanHHmm(BreakIntervals[i].BreakStart),
							FormatTimeSpanHHmm(BreakIntervals[i].BreakEnd),
							FormatTimeSpanHHmm(BreakIntervals[i].BreakEnd - BreakIntervals[i].BreakStart),
							Environment.NewLine);
					}
				}

				sb.AppendFormat("Work hours: {0}{1}", FormatTimeSpanHHmm(WorkHours), Environment.NewLine);
				sb.AppendLine();
				sb.AppendLine("In decimal:");
				sb.AppendLine();
				sb.AppendFormat("Break total time: {0:N2}{1}", BreakDuration.TotalHours, Environment.NewLine);
				sb.AppendFormat("Work hours: {0:N2}{1}", WorkHours.TotalHours, Environment.NewLine);

				return sb.ToString();
			}
		}

		public string NormalizedTimeData => GetNormalizedTimeData();
		public string OriginalTimeData { get; }
		public int BreakCount => BreakIntervals.Count;

		public string GetNormalizedTimeData() => GetNormalizedTimeData(DefaultNormalizedStrSeparator);

		public string GetNormalizedTimeData(string separator)
		{
			if (string.IsNullOrEmpty(separator))
            {
				separator = DefaultNormalizedStrSeparator;
			}

			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < Timestamps.Count; i++)
			{
				if (i > 0)
				{
					sb.Append(separator);
				}

				sb.AppendFormat("{0:00}:{1:00}", Timestamps[i].Hours, Timestamps[i].Minutes);
			}

			return sb.ToString();
		}

		public WorkTimeData(DateTime date, string originalTimeData)
		{
			if (originalTimeData == null)
			{
				throw new ArgumentNullException(nameof(originalTimeData));
			}

			Date = date;
			OriginalTimeData = originalTimeData;
			originalTimeData = originalTimeData.Replace(":", string.Empty);
			string[] rawPieces = originalTimeData.Split(Separators);
			List<string> normalizedPieces = new List<string>();

			for (int i = 0; i < rawPieces.Length; i++)
			{
				if (rawPieces[i].Length == 1)
				{
					normalizedPieces.Add("0" + rawPieces[i] + "00");
				}
				else if (rawPieces[i].Length == 2)
				{
					normalizedPieces.Add(rawPieces[i] + "00");
				}
				else if (rawPieces[i].Length == 3)
				{
					normalizedPieces.Add("0" + rawPieces[i]);
				}
				else if (rawPieces[i].Length == 4)
				{
					normalizedPieces.Add(rawPieces[i]);
				}
				else if (rawPieces[i].Length > 4)
				{
					throw new FormatException("Invalid time component length.");
				}
			}

			if (normalizedPieces.Count < 2)
			{
				throw new FormatException("At least two valid timestamps are required.");
			}

			if (normalizedPieces.Count % 2 != 0)
			{
				throw new FormatException("Not an even number of (valid) pieces.");
			}

			TimeSpan[] timestamps = new TimeSpan[normalizedPieces.Count];

			for (int i = 0; i < normalizedPieces.Count; i++)
			{
				int hours;
				int minutes;

				if (!int.TryParse(normalizedPieces[i].Substring(0, 2), out hours) ||
					!int.TryParse(normalizedPieces[i].Substring(2, 2), out minutes))
				{
					throw new FormatException("Invalid time format.");
				}

				if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
				{
					throw new FormatException("Invalid time value.");
				}

				timestamps[i] = new TimeSpan(hours, minutes, 0);
			}

			Array.Sort(timestamps);

			for (int i = 1; i < timestamps.Length; i++)
			{
				if (timestamps[i] == timestamps[i - 1])
				{
					throw new FormatException("Duplicate timestamps are not allowed.");
				}
			}

			Timestamps = Array.AsReadOnly(timestamps);

			List<BreakInterval> breakIntervals = new List<BreakInterval>();

			for (int i = 1; i < timestamps.Length - 1; i += 2)
			{
				breakIntervals.Add(new BreakInterval(timestamps[i], timestamps[i + 1]));
			}

			BreakIntervals = breakIntervals.AsReadOnly();

			TimeSpan totalBreakDuration = TimeSpan.Zero;

			for (int i = 0; i < breakIntervals.Count; i++)
			{
				totalBreakDuration += breakIntervals[i].BreakEnd - breakIntervals[i].BreakStart;
			}

			BreakDuration = totalBreakDuration;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("Input: ");
			sb.AppendLine(NormalizedTimeData);
			sb.AppendLine();
			sb.AppendLine("Stats:");
			sb.AppendLine();
			sb.Append(HumanReadableStats);

			return sb.ToString();
		}

		private static string FormatTimeSpanHHmm(TimeSpan value)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", (int)value.TotalHours, value.Minutes);
		}
	}
}
