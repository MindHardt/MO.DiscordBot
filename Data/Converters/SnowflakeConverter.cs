using Disqord;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data.Converters;

public class SnowflakeConverter() : ValueConverter<Snowflake, ulong>(
    x => x.RawValue,
    x => new Snowflake(x));