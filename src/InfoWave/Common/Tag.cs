namespace InfoWave.Common;

public record Tag(string Name);

public record Tag<T1>(string Name, T1 Value1) : Tag(Name);

public record Tag<T1, T2>(string Name, T1 Value1, T2 Value2) : Tag(Name);

public record Tag<T1, T2, T3>(string Name, T1 Value1, T2 Value2, T3 Value3) : Tag(Name);