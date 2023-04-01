namespace InfoWave.Common;

public record EventTag(string Name);

public record EventTag<T1>(string Name, T1 Value1) : EventTag(Name);

public record EventTag<T1, T2>(string Name, T1 Value1, T2 Value2) : EventTag(Name);

public record EventTag<T1, T2, T3>(string Name, T1 Value1, T2 Value2, T3 Value3) : EventTag(Name);