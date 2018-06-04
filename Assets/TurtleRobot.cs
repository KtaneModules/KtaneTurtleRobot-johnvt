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

    private int[] _dir = { (int)Verb.Lt, (int)Verb.Rt };
    private int[] _dist = { 1, 2, 3, 4 };
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
        Debug.LogFormat("[Turtle Robot #{0}] Original instructions: \n{1}", _moduleId, string.Join("\n", GetPencilCodeCommands(_commands).ToArray()));

        _commands = AddFaults(_commands);
        Debug.LogFormat("[Turtle Robot #{0}] Added faults: \n{1}", _moduleId, string.Join("\n", GetPencilCodeCommands(_commands).ToArray()));

        _commands = Randomize(_commands);
        Debug.LogFormat("[Turtle Robot #{0}] Module instructions: \n{1}", _moduleId, string.Join("\n", GetModuleInstructions(_commands).ToArray()));


        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Display.GetComponent<TextMesh>().text = String.Join("\n", new string[] {
            "Turtle Robot",
            "============",
            "  " + EncodeInstructions(_commands[_cursor == 0 ? _commands.Count() - 1 : _cursor - 1]),
            "> " + EncodeInstructions(_commands[_cursor]),
            "  " + EncodeInstructions(_commands[_cursor == _commands.Count() - 1 ? 0  : _cursor + 1])
        });
    }

    private string EncodeInstructions(List<int> list)
    {
        // For now only use first column
        return ((char)(_conversions[0, list[0], Rnd.Range(0, 2)] + 65)).ToString() + "-" + list[1] + (list.Count() == 3 ? "-" + list[2] : "");
    }

    void Update()
    {

    }

    private List<Command> AddFaults(List<Command> input)
    {
        var output = new List<List<int>>();

        // Determine two random breakpoints that are 3+ instructions apart
        var breakpoints = new int[2];
        int distance;
        do
        {
            breakpoints[0] = Rnd.Range(0, input.Count);
            breakpoints[1] = Rnd.Range(0, input.Count);
            distance = new[] {
                Math.Abs(breakpoints[0] - breakpoints[1]),
                Math.Abs(breakpoints[0] + input.Count - breakpoints[1]),
                Math.Abs(breakpoints[0] - breakpoints[1] + input.Count)
            }.Min();
        }
        while (distance < 3);
        Console.WriteLine("Breakpoints: " + breakpoints[0] + " and " + breakpoints[1]);

        for (var i = 0; i < input.Count; i++)
        {
            // Not a breakpoint, just copy to new lines
            if (!breakpoints.Contains(i))
            {
                output.Add(new List<int>(input[i]));
                continue;
            }

            // Determine next instruction
            var next = input[i == input.Count - 1 ? 0 : i + 1];
            var nextType = next[0] == (int)Verb.Fd ? MoveType.Line : (next.Count() == 2 ? MoveType.Rotation : MoveType.Arc);

            // Forward instruction
            if (input[i][0] == (int)Verb.Fd)
            {
                // If it's not the smallest size, and chance wants it, split into two
                if (input[i][1] > 1 && Rnd.Range(0, 2) == 0)
                {
                    // Determine split
                    var firstPart = Rnd.Range(1, input[i][1]);

                    // Add first part
                    output.Add(new List<int>(input[i]));
                    output[output.Count() - 1][1] = firstPart;

                    // Add random faulty part
                    if (Rnd.Range(0, 2) == 0)
                    {
                        Console.WriteLine("Splitting up and inserting a rotation.");
                        output.Add(new List<int>() { _dir[Rnd.Range(0, _dir.Count())], 90 });
                    }
                    else
                    {
                        Console.WriteLine("Splitting up and inserting an arc.");
                        output.Add(new List<int>() { _dir[Rnd.Range(0, _dir.Count())], 90, _dist[Rnd.Range(0, _dist.Count())] });
                    }

                    // Add second part
                    output.Add(new List<int>(input[i]));
                    output[output.Count() - 1][1] = input[i][1] - firstPart;
                }

                // Otherwise
                else
                {
                    // Add instruction
                    output.Add(new List<int>(input[i]));

                    // Add faulty part
                    if (nextType == MoveType.Arc)
                    {
                        Console.WriteLine("Adding a rotation.");
                        output.Add(new List<int>() { _dir[Rnd.Range(0, _dir.Count())], 90 });
                    }
                    else
                    {
                        Console.WriteLine("Adding an arc.");
                        output.Add(new List<int>() { _dir[Rnd.Range(0, _dir.Count())], 90, _dist[Rnd.Range(0, _dist.Count())] });
                    }
                }
            }

            // Rotate or arc instruction
            else
            {
                // If it's not the smallest size, and chance wants it, split into two
                if (input[i][1] > 90 && Rnd.Range(0, 2) == 0)
                {
                    // Determine split
                    var firstPart = Rnd.Range(1, input[i][1] / 90) * 90;

                    // Add first part
                    output.Add(new List<int>(input[i]));
                    output[output.Count() - 1][1] = firstPart;

                    // Add random faulty part
                    if (Rnd.Range(0, 2) == 0)
                    {
                        Console.WriteLine("Splitting up and inserting a straight line.");
                        output.Add(new List<int>() { (int)Verb.Fd, _dist[Rnd.Range(0, _dist.Count())] });
                    }
                    else
                    {
                        if (input[i].Count() == 3)
                        {
                            Console.WriteLine("Splitting up and inserting a rotation.");
                            output.Add(new List<int>() { _dir[Rnd.Range(0, _dir.Count())], 90 });
                        }
                        else
                        {
                            Console.WriteLine("Splitting up and inserting an arc.");
                            output.Add(new List<int>() { _dir[Rnd.Range(0, _dir.Count())], 90, _dist[Rnd.Range(0, _dist.Count())] });
                        }
                    }

                    // Add second part
                    output.Add(new List<int>(input[i]));
                    output[output.Count() - 1][1] = input[i][1] - firstPart;
                }

                // Otherwise
                else
                {
                    // Add instruction
                    output.Add(new List<int>(input[i]));

                    // Add faulty part
                    // If this is a rotate
                    if (input[i].Count() == 2)
                    {
                        if (nextType == MoveType.Arc)
                        {
                            Console.WriteLine("Adding a straight line.");
                            output.Add(new List<int>() { (int)Verb.Fd, _dist[Rnd.Range(0, _dist.Count())] });
                        }
                        else
                        {
                            Console.WriteLine("Adding an arc.");
                            output.Add(new List<int>() { _dir[Rnd.Range(0, _dir.Count())], 90, _dist[Rnd.Range(0, _dist.Count())] });
                        }
                    }

                    // If this is an arc
                    else
                    {
                        if (nextType == MoveType.Arc)
                        {
                            if (Rnd.Range(0, 2) == 0)
                            {
                                Console.WriteLine("Adding a straight line.");
                                output.Add(new List<int>() { (int)Verb.Fd, _dist[Rnd.Range(0, _dist.Count())] });
                            }
                            else
                            {
                                Console.WriteLine("Adding a rotation.");
                                output.Add(new List<int>() { _dir[Rnd.Range(0, _dir.Count())], 90 });
                            }
                        }
                        else if (nextType == MoveType.Rotation)
                        {
                            Console.WriteLine("Adding a straight line.");
                            output.Add(new List<int>() { (int)Verb.Fd, _dist[Rnd.Range(0, _dist.Count())] });
                        }
                        else
                        {
                            Console.WriteLine("Adding a rotation.");
                            output.Add(new List<int>() { _dir[Rnd.Range(0, _dir.Count())], 90 });
                        }
                    }
                }
            }
        }

        return output;
    }

    private List<Command> Randomize(List<Command> input)
    {
        // Sometimes switch left and right
        if (Rnd.Range(0, 2) == 0)
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
        if (Rnd.Range(0, 2) == 0)
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
            input.Code ((char)(_conversions[0, list[0], Rnd.Range(0, 2)] + 65)).ToString() + "-" + list[1] + (list.Count() == 3 ? "-" + list[2] : "");

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

        var factor = 1;

        foreach (var instruction in instructions)
        {
            factor = (Verb)instruction[0] == Verb.Fd || instruction.Count == 3 ? 25 : 1;
            var str = ((Verb)instruction[0]).ToString().ToLower() + " ";
            if (instruction.Count == 2)
                str += (instruction[1] * factor);
            else
                str += (instruction[1] + ", " + instruction[2] * factor);

            list.Add(str);
        }

        return list;
    }

    private List<string> GetModuleInstructions(List<Command> instructions)
    {
        var list = new List<string>();

        foreach (var instruction in instructions)
        {
            var str = ((Verb)instruction[0]).ToString() + " " + instruction[1];
            if (instruction.Count == 3)
            {
                str += " " + instruction[2];
            }
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

    class Command
    {
        public Verb Verb { get; set; }
        public int Degrees { get; set; }
        public int Distance { get; set; }
        public string Code { get; set; }
        public bool Bug { get; set; }
    }
}
