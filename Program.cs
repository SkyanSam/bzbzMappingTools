using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Xml;
using System.Security.Cryptography;
using Microsoft.VisualBasic;

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
    var scratches = new List<Scratch>();
    var notes = midiFile.GetNotes();
    var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
    notes.CopyTo(array, 0);
    foreach (var note in array)
    {
        Console.WriteLine($"{note.NoteName}{note.Octave} {note.Time} {note.EndTime}");
        double start = ConvertMidiTimeToSeconds(note.Time, midiFile);
        double end = ConvertMidiTimeToSeconds(note.EndTime, midiFile);
        if (note.Octave == 5)
        {
            switch(note.NoteName)
            {
                case Melanchall.DryWetMidi.MusicTheory.NoteName.B:
                    scratches.Add(new Scratch() { xValue = -3, timeStamp = start });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.A:
                    scratches.Add(new Scratch() { xValue = -1, timeStamp = start });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.G:
                    scratches.Add(new Scratch() { xValue = 1, timeStamp = start });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.F:
                    scratches.Add(new Scratch() { xValue = 3, timeStamp = start });
                    break;
            }
        }
        else if (note.Octave == 4)
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
                case Melanchall.DryWetMidi.MusicTheory.NoteName.FSharp:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "stomp", startTime = start.ToString(), endTime = end.ToString(), xValue = "-2" });
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
                case Melanchall.DryWetMidi.MusicTheory.NoteName.FSharp:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "stomp", startTime = start.ToString(), endTime = end.ToString(), xValue = "2" });
                    break;
                case Melanchall.DryWetMidi.MusicTheory.NoteName.F:
                    newList.Add(new NoteDataRaw { id = uniqueID, type = "stompHold", startTime = start.ToString(), endTime = end.ToString(), xValue = "2" });
                    break;
            }
        }
        else if (note.Octave == 2)
        {
            // TODO: Bullets.
        }
        counter++;
        uniqueID = counter.ToString();
    }
    foreach (var s in scratches)
    {
        var index = newList.FindIndex((n) => ((float.Parse(n.xValue) > 0 && s.xValue > 0) || (float.Parse(n.xValue) < 0 && s.xValue < 0)) && double.Parse(n.startTime) <= s.timeStamp && s.timeStamp <= double.Parse(n.endTime));
        if (index == -1)
        {
            Console.WriteLine($"ERROR : Couldn't find scratch for x : {s.xValue}, time : {s.timeStamp}");
            continue;
        }
        // This nested loop is so bad, spare me please.
        var bezierScratches = newList[index].bezierScratches.ToList();
        Console.WriteLine("bezscratch0 : " + index + " : " + bezierScratches.Count);
        bezierScratches.Add($"{s.xValue},{s.timeStamp}");
        bezierScratches.Sort(new Comparison<string>((string s1, string s2) => s1.Split(',')[1].dp() > s2.Split(',')[1].dp() ? 1 : -1));
        Console.WriteLine("bezscratch1 : " + index + " : " + bezierScratches.Count);
        newList[index].bezierScratches = bezierScratches.ToArray();
        Console.WriteLine("bezscratch2 : " + index + " : " + newList[index].bezierScratches.Length);
    }
    return newList;
}

// I would not use relative paths (m,c,l). Did some testing and there are a couple of bugs.
string[] GetBezierPoints(string pathString, int width, int height, bool isDebug=false)
{
    string pattern = @"(-?\d+(\.\d+)?)|C|L|M|c|l|m";
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
SVGScrape ScrapeSVG(string txt, bool isDebug = false)
{
    SVGScrape result = new SVGScrape();
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(txt);
    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
    nsmgr.AddNamespace("svg", "http://www.w3.org/2000/svg");

    XmlNode? svgRoot = xmlDoc.DocumentElement;

    if (svgRoot != null && svgRoot.Attributes != null)
    {
        result.width = int.Parse(Regex.Match(svgRoot.Attributes["width"]?.Value ?? "0", @"[0-9]+").Value);
        result.height = int.Parse(Regex.Match(svgRoot.Attributes["height"]?.Value ?? "0", @"[0-9]+").Value);

        XmlNode? pathNode = svgRoot.SelectSingleNode(".//svg:path", nsmgr);

        if (pathNode != null)
        {
            string? dAttr = pathNode.Attributes?["d"]?.Value;
            result.path = dAttr ?? "";
        }
        else
        {
            Console.WriteLine("ERROR : No <path> element found.");
        }
    }
    else
    {
        Console.WriteLine("ERROR : No <svg> element or attribute found.");
    }
    
    if (isDebug) Console.WriteLine($"Scrape Svg Result {result.width}, {result.height}, {result.path}");
    return result;
}
double Lerp(double a, double b, double t)
{
    return ((b - a) * t) + a;
}
static double DoubleParse(string s)
{
    double res;
    if (!double.TryParse(s, out res)) Console.WriteLine("ERROR: DOUBLE FAILED PARSE " + s);
    return res;
}


Console.Write("Midi Path (.mid) (enter blank for default path) :");
var midiPathRes = Console.ReadLine();
var midiPath = midiPathRes == null || midiPathRes == "" ? @"C:\Users\sam\source\repos\bzbzMappingDemo\map.mid" : midiPathRes;
Console.Write("SVG Path (folder) (enter blank for default path) :");
var svgPathRes = Console.ReadLine();
var svgPath = svgPathRes == null || svgPathRes == "" ? @"C:\Users\sam\source\repos\bzbzMappingDemo\svg\" : svgPathRes;
Console.Write("Hold Mapping Path (.txt) (enter blank for default path) :");
var holdPathRes = Console.ReadLine();
var holdPath = holdPathRes == null || holdPathRes == "" ? @"C:\Users\sam\source\repos\bzbzMappingDemo\holdmap.txt" : holdPathRes;
Console.Write("Export Path (.json) (enter blank for default path) :");
var exportPathRes = Console.ReadLine();
var exportPath = exportPathRes == null || exportPathRes == "" ? @"C:\Users\sam\source\repos\bzbzMappingDemo\map.json" : exportPathRes;


Console.WriteLine("Final Midi Path : " + midiPath);
Console.WriteLine("Final SVG Path : " + svgPath);
Console.WriteLine("Final Hold Path : " + holdPath);
Console.WriteLine("Exporting to " + exportPath);



MidiFile midiFile = MidiFile.Read(midiPath);
var notes = GetNotes(midiFile);

Dictionary<string, string[]> beziers = new Dictionary<string, string[]>();

var holdLines = File.ReadAllLines(holdPath);
Console.WriteLine("Hold Lines Count : " + holdLines.Length.ToString());
foreach (var line in holdLines)
{
    var split = line.Split(' ');
    string[] pts;
    if (!beziers.ContainsKey(split[2]))
    {
        var scrape = ScrapeSVG(File.ReadAllText(svgPath + split[2]));
        pts = GetBezierPoints(scrape.path, scrape.width, scrape.height);
        beziers[split[2]] = pts;
    }
    else
    {
        pts = beziers[split[2]];
    }
    for (int i = 0; i < notes.Count; i++)
    {
        if (notes[i].type != "hold") continue;
        bool leftCondition = split[0] == "L" && float.Parse(notes[i].xValue) <= 0;
        bool rightCondition = split[0] == "R" && float.Parse(notes[i].xValue) >= 0;
        if (!leftCondition && !rightCondition) continue;
        bool timeCondition = double.Parse(notes[i].startTime) <= double.Parse(split[1]) && double.Parse(split[1]) <= double.Parse(notes[i].endTime);
        if (!timeCondition) continue;

        var startTime = double.Parse(notes[i].startTime);
        var endTime = double.Parse(notes[i].endTime);
        notes[i].bezierPoints = new string[pts.Length];
        for (int j = 0; j < pts.Length; j++)
        {
            var ptsSplit = pts[j].Split(',');
            var xt = double.Parse(ptsSplit[0]);
            var yt = double.Parse(ptsSplit[1]);
            var x = Lerp(startTime, endTime, xt);
            var y = split[0] == "R" ? Lerp(1.0, 3.0, yt) : Lerp(-3.0, -1.0, yt);
            notes[i].bezierPoints[j] = $"{y},{x}";
            Console.WriteLine($"{notes[i].id} , st {startTime} , en {endTime} , xt {xt}, yt {yt}, x {x}, y {y} -> {notes[i].bezierPoints[j]}");
        }
    }
}

var str = JsonSerializer.Serialize(notes);
File.WriteAllText(exportPath, str);
Console.WriteLine("Exported Successfully.");
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
public class SVGScrape
{
    public int width = 0;
    public int height = 0;
    public string path = "";
}
public class Scratch
{
    public float xValue = 0;
    public double timeStamp = 0;
}
public class BulletRawData
{
    public string type = "";
    public string posX = "";
    public string posZ = "";
    public string velX = "";
    public string velZ = "";
    public string minDeg = "";
    public string maxDeg = "";
}
public static class MyExtension
{
    public static double dp(this string s)
    {
        double res;
        if (!double.TryParse(s, out res)) Console.WriteLine("ERROR: DOUBLE FAILED PARSE " + s);
        return res;
    }
}
// TODO assign beziers to hold notes
// assign scratches to hold notes
// Bullets!!!

