using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace SpellWork.GameTables
{
    public static class GameTable<T> where T : struct
    {
        private static Dictionary<int, T> _records { get; } = new Dictionary<int, T>();

        public static T? GetRecord(int key)
        {
            return _records.TryGetValue(key, out var record) ? record : null;
        }

        public static void Open(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
                var lineIndex = -1;
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        break;

                    ++lineIndex;
                    if (lineIndex == 0)
                        continue;

                    var lineTokens = line.Split('\t');
                    if (lineTokens.Length != fields.Length)
                        throw new FileFormatException($"Unexpected column count in {fileName} line {lineIndex}");

                    var record = new T();
                    for (var fieldIndex = 0; fieldIndex < fields.Length; ++fieldIndex)
                    {
                        if (string.IsNullOrEmpty(lineTokens[fieldIndex]))
                            continue;

                        var field = fields[fieldIndex];

                        // ReSharper disable once SwitchStatementMissingSomeCases
                        switch (Type.GetTypeCode(field.FieldType))
                        {
                            case TypeCode.UInt32:
                                field.SetValue(record, uint.Parse(lineTokens[fieldIndex], CultureInfo.InvariantCulture));
                                break;
                            case TypeCode.Single:
                                field.SetValue(record, float.Parse(lineTokens[fieldIndex], CultureInfo.InvariantCulture));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(null, $"Unexpected field type {Type.GetTypeCode(field.FieldType)} in {nameof(T)}");
                        }
                    }

                    _records.Add(lineIndex, record);
                }
            }
        }
    }
}
