using CsvHelper;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;

namespace Core;

public class CsvHelperExample
{
    public void Write(string path, IEnumerable<Person> people)
    {
        using var sw = new StreamWriter(path);
        using var cw = new CsvWriter(sw, CultureInfo.InvariantCulture);
        cw.WriteRecords(people);
    }

    public IEnumerable<Person> Read(string path)
    {
        using var sr = new StreamReader(path);
        using var cr = new CsvReader(sr, CultureInfo.InvariantCulture);
        return cr.GetRecords<Person>().ToList();
    }
}