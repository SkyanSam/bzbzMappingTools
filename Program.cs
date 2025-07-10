using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

// I'm aware this code isn't well optimized, but it exists until I can make a proper level editor.
// Thanks for considering making maps for this little experiment of mine! - SkyanSam
double ConvertMidiTimeToSeconds(long midiTime, MidiFile midiFile)
{
    var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(midiTime, midiFile.GetTempoMap());
    return (double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f;
}
List<NoteDataRaw> GetNotes(MidiFile midiFile)
{
    int counter = 0;
    string uniqueID = "0";
    var newList = new List<NoteDataRaw>();
    var notes = midiFile.GetNotes();
    var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
    notes.CopyTo(array, 0);
    foreach (var note in array)
    {
        Console.WriteLine($"{note.NoteName}{note.Octave} {note.Time} {note.EndTime}");
        double start = ConvertMidiTimeToSeconds(note.Time, midiFile);
        double end = ConvertMidiTimeToSeconds(note.EndTime, midiFile);
        if (note.Octave == 4)
        {
            switch (note.NoteName)
            {
                case Melanchall.DryWetMidi.MusicTheory.NoteName.E:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "tap", startTime = start.ToString(), endTime = end.ToString(), xValue = "-3" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.DSharp:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "tapSlideLeft", startTime = start.ToString(), endTime = end.ToString(), xValue = "-2" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.D:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "tap", startTime = start.ToString(), endTime = end.ToString(), xValue = "-2" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.CSharp:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "tapSlideRight", startTime = start.ToString(), endTime = end.ToString(), xValue = "-2" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.C:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "tap", startTime = start.ToString(), endTime = end.ToString(), xValue = "-1" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.G:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "hold", startTime = start.ToString(), endTime = end.ToString(), xValue = "-2" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.F:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "stompHold", startTime = start.ToString(), endTime = end.ToString(), xValue = "-2" });
                    break;
            }
        }
        else if (note.Octave == 3)
        {
            switch (note.NoteName)
            {
                case Melanchall.DryWetMidi.MusicTheory.NoteName.E:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "tap", startTime = start.ToString(), endTime = end.ToString(), xValue = "1" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.DSharp:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "tapSlideLeft", startTime = start.ToString(), endTime = end.ToString(), xValue = "2" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.D:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "tap", startTime = start.ToString(), endTime = end.ToString(), xValue = "2" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.CSharp:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "tapSlideRight", startTime = start.ToString(), endTime = end.ToString(), xValue = "2" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.C:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "tap", startTime = start.ToString(), endTime = end.ToString(), xValue = "3" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.G:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "hold", startTime = start.ToString(), endTime = end.ToString(), xValue = "2" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.F:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "stompHold", startTime = start.ToString(), endTime = end.ToString(), xValue = "2" });
                    break;
            }
        }
        counter++;
        uniqueID = counter.ToString();
    }
    return newList;
}

// I would not use relative paths (m,c,l). Did some testing and there are a couple of bugs.
string[] GetBezierPoints(string pathString, int width, int height, bool isDebug=false)
{
    string pattern = @"(-?\d+(\.\d+)?)|C|L|M|c|l|m";
    pathString = " M 0 100 C 0.394 30.608 17.297 -2.716 50.68 0 C 83.275 -1.851 99.725 31.492 100 100";
    List<List<string>> points = [[]];
    int index = 0;
    bool x = true;
    Match match = Regex.Match(pathString, pattern);
    string current = "0,0";
    double currentX = 0;
    double currentY = 0;
    string state = "";
    while (match.Success)
    {
        Console.WriteLine($"Found match: {match.Value}");
        switch (match.Value)
        {
            case "M":
            case "m":
                current = "";
                state = match.Value;
                break;
            case "C":
            case "c":
            case "L":
            case "l":
                state = match.Value;
                break;
            default:
                double matchValue = DoubleParse(match.Value);
                if (state == "M" || state == "m")
                {
                    if (x)
                    {
                        current = state == "m" ? (matchValue - currentX).ToString() : match.Value;
                        currentX = matchValue;
                        x = !x;
                    }
                    else
                    {
                        current += "," + (state == "m" ? (matchValue - currentY).ToString() : match.Value);
                        currentY = matchValue;
                        x = !x;
                    }
                }
                if (state == "C" || state == "c")
                {
                    if (points[index].Count == 0)
                    {
                        points[index].Add(current);
                    }
                    if (x)
                    {
                        points[index].Add(state == "c" ? (matchValue - currentX).ToString() : match.Value);
                        x = !x;
                    }
                    else
                    {
                        points[index][points[index].Count - 1] += $",{(state == "c" ? (matchValue - currentY).ToString() : match.Value)}";
                        if (points[index].Count == 4)
                        {
                            current = points[index][3];
                            currentX = DoubleParse(current.Split(",")[0]);
                            currentY = DoubleParse(current.Split(",")[1]);
                            index++;
                            points.Add([]);
                        }
                        x = !x;
                    }
                }
                if (state == "L" || state == "l")
                {
                    if (points[index].Count == 0)
                    {
                        points[index].Add(current);
                    }
                    if (x)
                    {
                        points[index].Add(state == "l" ? (matchValue - currentX).ToString() : match.Value);
                        x = !x;
                    }
                    else
                    {
                        points[index][points[index].Count - 1] += $",{(state == "l" ? (matchValue - currentY).ToString() : match.Value)}";
                        double endX = DoubleParse(points[index][points[index].Count - 1].Split(',')[0]);
                        points[index].Insert(1, $"{Lerp(currentX, endX, 0.33)},{Lerp(currentY, matchValue, 0.33)}");
                        points[index].Insert(2, $"{Lerp(currentX, endX, 0.66)},{Lerp(currentY, matchValue, 0.66)}");
                        current = points[index][3];
                        currentX = DoubleParse(current.Split(",")[0]);
                        currentY = DoubleParse(current.Split(",")[1]);
                        index++;
                        points.Add([]);
                        x = !x;
                    }
                }
                break;
        }
        match = match.NextMatch();
    }
    var final = new List<string>();
    foreach (var e in points) {
        if (isDebug) Console.WriteLine("--");
        foreach (var e2 in e)
        {
            var split = e2.Split(',');
            final.Add($"{double.Parse(split[0]) / width},{double.Parse(split[1]) / height}");
            if (isDebug) Console.WriteLine(final[final.Count - 1]);
        }
    }
    return final.ToArray();
}
double Lerp(double a, double b, double t)
{
    return ((b - a) * t) + a;
}
double DoubleParse(string s)
{
    double res;
    if (!double.TryParse(s, out res)) Console.WriteLine("ERROR: DOUBLE FAILED PARSE " + s);
    return res;
}

GetBezierPoints("",100,100,isDebug: true);
MidiFile midiFile = MidiFile.Read(@"C:\\Users\\sam\\Documents\\Github\\bzbz\\Assets\\Audio\\bmsdemo.mid");
var notes = GetNotes(midiFile);
var str = JsonSerializer.Serialize(notes);
System.IO.File.WriteAllText(@"C:\Users\sam\source\repos\bzbzMappingDemo\mymap.json", str);
public class NoteDataRaw
{
    public string id { get; set; } = "";
    public string type { get; set; } = "";
    public string xValue { get; set; } = "";
    public string startTime { get; set; } = "";
    public string endTime { get; set; } = "";
    public string[] bezierPoints { get; set; } = [];
    public string[] bezierScratches { get; set; } = [];
}

