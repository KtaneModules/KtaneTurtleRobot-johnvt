using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class TurtleRobot : MonoBehaviour
{
    public KMSelectable ButtonUp;
    public KMSelectable ButtonDown;
    public KMSelectable ButtonDelete;
    public GameObject Display;

    private int _moduleId;
    private static int _moduleIdCounter = 1;

    private enum Verb { Fd, Lt, Rt }
    private enum MoveType { Line, Rotation, Arc }

    private int[] _distFactor = { 1, 2, 3, 4, 6 };
    private int _cursor = 0;
    private List<Command> _commands;

    private Dictionary<string, List<Command>> _shapes;
    private int[,,] _conversions;
    void Start()
    {
        _moduleId = _moduleIdCounter++;

        ButtonUp.OnInteract += delegate () { PressArrow(-1); return false; };
        ButtonDown.OnInteract += delegate () { PressArrow(1); return false; };
        ButtonDelete.OnInteract += delegate () { PressDelete(); return false; };

        _shapes = new Dictionary<string, List<Command>>() {
            { "spades", new List<Command>() {
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 180 },
                new Command() { Verb = Verb.Lt, Degrees = 90, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Lt, Degrees = 90, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 180 },
                new Command() { Verb = Verb.Fd, Distance = 6 },
                new Command() { Verb = Verb.Rt, Degrees = 180 },
                new Command() { Verb = Verb.Lt, Degrees = 90, Distance = 2 }
            } },
            { "clubs", new List<Command>() {
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Lt, Degrees = 90, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 180 },
                new Command() { Verb = Verb.Fd, Distance = 6 },
                new Command() { Verb = Verb.Rt, Degrees = 180 },
                new Command() { Verb = Verb.Lt, Degrees = 90, Distance = 2 }
            } },
            { "crown", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 150 },
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Lt, Degrees = 120 },
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Rt, Degrees = 120 },
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Lt, Degrees = 120 },
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Rt, Degrees = 150 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 6 },
                new Command() { Verb = Verb.Rt, Degrees = 90 }
            } },
            { "house", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 30 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 120 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 30 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 }
            } },
            { "car", new List<Command>() {
                new Command() { Verb = Verb.Rt, Degrees = 90, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 90, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 1 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 1 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 90 }
            } },
            { "mushroom", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 }
            } },
            { "bottle", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90, Distance = 1 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 90, Distance = 1 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Rt, Degrees = 90 }
            } },
            { "sock", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 1 },
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Rt, Degrees = 90, Distance = 1 }
            } },
            { "tree", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 }
            } },
            { "tshirt", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Lt, Degrees = 180, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 3 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 }
            } },
            { "tulip", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 90, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 150 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 120 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 150 },
                new Command() { Verb = Verb.Rt, Degrees = 90, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 3 }
            } },
            { "key", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 6 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Lt, Degrees = 90 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 2 }
            } }
        };

        _conversions = new int[,,]
        {
            { { 0, 2 }, { 1, 4 }, { 3, 5 } },
            { { 1, 3 }, { 0, 5 }, { 2, 4 } },
            { { 1, 2 }, { 3, 5 }, { 0, 4 } },
            { { 2, 5 }, { 3, 4 }, { 0, 1 } }
        };

        _commands = _shapes.ElementAt(Rnd.Range(0, _shapes.Count)).Value;
        Debug.LogFormat("[Turtle Robot #{0}] Original commands: \n{1}", _moduleId, string.Join("\n", GetPencilCodeCommands(_commands).ToArray()));

        _commands = AddBugs(_commands);
        Debug.LogFormat("[Turtle Robot #{0}] Added bugs: \n{1}", _moduleId, string.Join("\n", GetPencilCodeCommands(_commands).ToArray()));

        _commands = Randomize(_commands);
        Debug.LogFormat("[Turtle Robot #{0}] Randomized commands: \n{1}", _moduleId, string.Join("\n", GetModuleInstructions(_commands).ToArray()));


        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Display.GetComponent<TextMesh>().text = String.Join("\n", new string[] {
            "Turtle Robot",
            "============",
            "  " + GetForDisplay(_commands[_cursor == 0 ? _commands.Count() - 1 : _cursor - 1]),
            "> " + GetForDisplay(_commands[_cursor]),
            "  " + GetForDisplay(_commands[_cursor == _commands.Count() - 1 ? 0  : _cursor + 1])
        });
    }

    private string GetForDisplay(Command command)
    {
        return command.Code
            + (command.Degrees != 0 ? "-" + command.Degrees.ToString() : "")
            + (command.Distance != 0 ? "-" + command.Distance.ToString() : "");
    }

    void Update()
    {

    }

    private List<Command> AddBugs(List<Command> commands)
    {
        var result = new List<Command>();

        // Determine random breakpoints so they are 3+ instructions apart
        var breakpoints = new int[3];
        while (true)
        {
            // Random breakpoints
            for (var i = 0; i < breakpoints.Count(); i++)
                breakpoints[i] = Rnd.Range(0, commands.Count);

            // For each pair check if they have enough distance between them
            for (var i = 0; i < breakpoints.Count(); i++)
                for (var j = 0; j < breakpoints.Count(); j++)
                    if (GetMinDistance(breakpoints[i], breakpoints[j], breakpoints.Count()) < 3)
                        continue;

            // Yay!
            break;

        }
        Console.WriteLine("Breakpoints: " + String.Join(", ", breakpoints.Select(x => x.ToString()).ToArray()));

        foreach (var index in breakpoints)
            commands[index].Bug = true;

        for (var i = 0; i < commands.Count; i++)
        {
            // Not a breakpoint, just copy to new lines
            if (!commands[i].Bug)
            {
                result.Add((Command)_commands[i].Clone());
                continue;
            }

            // Determine next instruction
            var next = commands[i == commands.Count - 1 ? 0 : i + 1];
            var nextType = next.Verb == Verb.Fd ? MoveType.Line : (next.Distance == 0 ? MoveType.Rotation : MoveType.Arc);

            // Forward instruction
            if (commands[i].Verb == Verb.Fd)
            {
                // If it's not the smallest size, and chance wants it, split into two
                if (commands[i].Distance > 1 && Rnd.Range(0f, 1f) < .5)
                {
                    // Determine split
                    var firstPart = Rnd.Range(1, commands[i].Distance);

                    // Add first part
                    result.Add((Command)commands[i].Clone());
                    result[result.Count() - 1].Distance = firstPart;

                    // Add random bug
                    if (Rnd.Range(0f, 1f) < .5)
                    {
                        Console.WriteLine("Splitting up and inserting a rotation.");
                        result.Add(new Command() { Verb = LtOrRt(), Degrees = 90 });
                    }
                    else
                    {
                        Console.WriteLine("Splitting up and inserting an arc.");
                        result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Distance = RandomDistance() });
                    }

                    // Add second part
                    result.Add((Command)(commands[i].Clone()));
                    result[result.Count() - 1].Distance -= firstPart;
                }

                // Otherwise
                else
                {
                    // Add instruction
                    result.Add((Command)commands[i].Clone());

                    // Add bug
                    if (nextType == MoveType.Arc)
                    {
                        Console.WriteLine("Adding a rotation.");
                        result.Add(new Command() { Verb = LtOrRt(), Degrees = 90 });
                    }
                    else
                    {
                        Console.WriteLine("Adding an arc.");
                        result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Distance = RandomDistance() });
                    }
                }
            }

            // Rotate or arc instruction
            else
            {
                // If it's 180 degrees, and chance wants it, split into two
                if (commands[i].Degrees == 180 && Rnd.Range(0f, 1f) < .5)
                {
                    // Add first part
                    result.Add((Command)commands[i].Clone());
                    result[result.Count() - 1].Degrees = 90;

                    // Add random bug
                    if (Rnd.Range(0f, 1f) < .5)
                    {
                        Console.WriteLine("Splitting up and inserting a straight line.");
                        result.Add(new Command() { Verb = Verb.Fd, Distance = RandomDistance() });
                    }
                    else
                    {
                        // If we are splitting up a rotation
                        if (commands[i].Distance == 0)
                        {
                            Console.WriteLine("Splitting up and inserting an arc.");
                            result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Distance = RandomDistance() });
                        }

                        // If we are splitting up an arc
                        else
                        {
                            Console.WriteLine("Splitting up and inserting a rotation.");
                            result.Add(new Command() { Verb = LtOrRt(), Degrees = 90 });
                        }
                    }

                    // Add second part
                    result.Add((Command)commands[i].Clone());
                    result[result.Count() - 1].Degrees = 90;
                }

                // Otherwise
                else
                {
                    // Add instruction
                    result.Add((Command)commands[i].Clone());

                    // Add bug
                    // If this is a rotate
                    if (commands[i].Distance == 0)
                    {
                        if (nextType == MoveType.Arc)
                        {
                            Console.WriteLine("Adding a straight line.");
                            result.Add(new Command() { Verb = Verb.Fd, Distance = RandomDistance() });
                        }
                        else
                        {
                            Console.WriteLine("Adding an arc.");
                            result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Distance = RandomDistance() });
                        }
                    }

                    // If this is an arc
                    else
                    {
                        if (nextType == MoveType.Arc)
                        {
                            if (Rnd.Range(0f, 1f) < .5)
                            {
                                Console.WriteLine("Adding a straight line.");
                                result.Add(new Command() { Verb = Verb.Fd, Distance = RandomDistance() });
                            }
                            else
                            {
                                Console.WriteLine("Adding a rotation.");
                                result.Add(new Command() { Verb = LtOrRt(), Degrees = 90 });
                            }
                        }
                        else if (nextType == MoveType.Rotation)
                        {
                            Console.WriteLine("Adding a straight line.");
                            result.Add(new Command() { Verb = Verb.Fd, Distance = RandomDistance() });
                        }
                        else
                        {
                            Console.WriteLine("Adding a rotation.");
                            result.Add(new Command() { Verb = LtOrRt(), Degrees = 90 });
                        }
                    }
                }
            }
        }

        return result;
    }

    private List<Command> Randomize(List<Command> input)
    {
        // Sometimes switch left and right
        if (Rnd.Range(0f, 1f) < .5)
        {
            Console.WriteLine("Switching left and right");
            foreach (var command in input)
            {
                if (command.Verb == Verb.Lt)
                    command.Verb = Verb.Rt;
                else if (command.Verb == Verb.Rt)
                    command.Verb = Verb.Lt;
            }
        }

        // Sometimes reverse the order
        if (Rnd.Range(0f, 1f) < .5)
        {
            Console.WriteLine("Reversing order");
            input.Reverse();
        }

        // Apply random factor to the distances
        var factor = _distFactor[Rnd.Range(0, _distFactor.Count())];
        Console.WriteLine("Apllying factor " + factor);
        foreach (var command in input)
            command.Distance *= factor;

        // Random starting point
        // 0, 1, 2, 3
        var num = Rnd.Range(0, input.Count());
        Console.WriteLine("Starting at " + (input.Count() - num));
        for (var i = 0; i < num; i++)
        {
            input.Insert(0, input[input.Count() - 1]);
            input.RemoveAt(input.Count() - 1);
        }

        // Random code for display
        foreach (Command command in input)
        {
            command.Code = ((Char)(_conversions[0, (int)command.Verb, Rnd.Range(0, 2)] + 65)).ToString();
        }

        return input;
    }

    private List<string> GetPencilCodeCommands(List<Command> instructions)
    {
        var list = new List<string>
        {
            "speed 200",
            "pen black, 5"
        };

        foreach (var instruction in instructions)
        {
            var str = instruction.Verb.ToString().ToLower() + " "
                + (instruction.Degrees > 0 ? instruction.Degrees.ToString() : "")
                + (instruction.Degrees > 0 && instruction.Distance > 0 ? ", " : "")
                + (instruction.Distance > 0 ? (instruction.Distance * 25).ToString() : "");

            list.Add(str);
        }

        return list;
    }

    private List<string> GetModuleInstructions(List<Command> instructions)
    {
        var list = new List<string>();

        foreach (var instruction in instructions)
        {
            var str = instruction.Code
                + (instruction.Degrees > 0 ? "-" + instruction.Degrees.ToString() : "")
                + (instruction.Distance > 0 ? "-" + instruction.Distance.ToString() : "");
            list.Add(str);
        }

        return list;
    }

    private void PressArrow(int direction)
    {
        _cursor += direction;
        if (_cursor >= _commands.Count()) _cursor = 0;
        else if (_cursor < 0) _cursor = _commands.Count() - 1;
        UpdateDisplay();
    }

    private void PressDelete()
    {
        _commands.RemoveAt(_cursor);
        if (_cursor >= _commands.Count()) _cursor = 0;
        UpdateDisplay();
    }

    private int GetMinDistance(int a, int b, int length)
    {
        return new[] {
            Math.Abs(a - b),
            Math.Abs(a + length - b),
            Math.Abs(a - b + length)
        }.Min();
    }

    private Verb LtOrRt()
    {
        return Rnd.Range(0f, 1f) < .5 ? Verb.Lt : Verb.Rt;
    }

    private int RandomDistance()
    {
        var distances = new int[] { 1, 2, 3, 4 };
        return distances[Rnd.Range(0, distances.Count())];
    }

    class Command : ICloneable
    {
        public Verb Verb { get; set; }
        public int Degrees { get; set; }
        public int Distance { get; set; }
        public string Code { get; set; }
        public bool Bug { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}