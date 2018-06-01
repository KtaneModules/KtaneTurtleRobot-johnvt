using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

/**
 * 
 * 
 * 
 * ADJUST CURSOR after delete!
 * 
 * 
 */ 
public class TurtleRobot : MonoBehaviour
{
    public KMSelectable ButtonUp;
    public KMSelectable ButtonDown;
    public KMSelectable ButtonDelete;
    public GameObject Display;

    private int _moduleId;
    private static int _moduleIdCounter = 1;

    private enum Move { Fd, Lt, Rt }
    private enum MoveType { Line, Rotation, Arc }

    private int[] _dir = { (int)Move.Lt, (int)Move.Rt };
    private int[] _dist = { 1, 2, 3, 4 };
    private int[] _distFactor = { 1, 2, 3, 4, 6 };
    private int _cursor = 0;
    private List<List<int>> _instructions;
    private List<int> _encoding1;
    private List<string> _encoding2;
    private Dictionary<string, List<List<int>>> _shapes;

    void Start()
    {
        _moduleId = _moduleIdCounter++;

        ButtonUp.OnInteract += delegate () { PressArrow(-1); return false; };
        ButtonDown.OnInteract += delegate () { PressArrow(1); return false; };
        ButtonDelete.OnInteract += delegate () { PressDelete(); return false; };

        _shapes = new Dictionary<string, List<List<int>>>() {
            { "spades", new List<List<int>>() {
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Rt, 180, 2 },
                new List<int>() { (int)Move.Lt, 90, 2 },
                new List<int>() { (int)Move.Rt, 180 },
                new List<int>() { (int)Move.Lt, 90, 2 },
                new List<int>() { (int)Move.Rt, 180, 2 },
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Lt, 90, 2 },
                new List<int>() { (int)Move.Rt, 180 },
                new List<int>() { (int)Move.Fd, 6 },
                new List<int>() { (int)Move.Rt, 180 },
                new List<int>() { (int)Move.Lt, 90, 2 }
            } },
            { "clubs", new List<List<int>>() {
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Rt, 180, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Rt, 180, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Rt, 180, 2 },
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Lt, 90, 2 },
                new List<int>() { (int)Move.Rt, 180 },
                new List<int>() { (int)Move.Fd, 6 },
                new List<int>() { (int)Move.Rt, 180 },
                new List<int>() { (int)Move.Lt, 90, 2 }
            } },
            { "crown", new List<List<int>>() {
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 150 },
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Lt, 120 },
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Rt, 120 },
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Lt, 120 },
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Rt, 150 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 6 },
                new List<int>() { (int)Move.Rt, 90 }
            } },
            { "house", new List<List<int>>() {
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 30 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 120 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 30 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 90 }
            } },
            { "car", new List<List<int>>() {
                new List<int>() { (int)Move.Rt, 90, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Rt, 180, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Rt, 90, 2 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Rt, 180, 1 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Rt, 180, 1 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Rt, 90 }
            } },
            { "mushroom", new List<List<int>>() {
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Rt, 180, 4 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 90 }
            } },
            { "bottle", new List<List<int>>() {
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 90, 1 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Rt, 90, 1 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Rt, 90 }
            } },
            { "sock", new List<List<int>>() {
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 180, 1 },
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Rt, 90, 1 }
            } },
            { "tree", new List<List<int>>() {
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Rt, 180, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Rt, 180, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Rt, 180, 2 },
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 90 }
            } },
            { "tshirt", new List<List<int>>() {
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Lt, 180, 1 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 3 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 90 }
            } },
            { "tulip", new List<List<int>>() {
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Rt, 90, 2 },
                new List<int>() { (int)Move.Lt, 150 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 120 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Lt, 150 },
                new List<int>() { (int)Move.Rt, 90, 2 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Rt, 180, 3 }
            } },
            { "key", new List<List<int>>() {
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Rt, 180, 2 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 6 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 2 },
                new List<int>() { (int)Move.Rt, 90 },
                new List<int>() { (int)Move.Fd, 1 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Fd, 4 },
                new List<int>() { (int)Move.Lt, 90 },
                new List<int>() { (int)Move.Rt, 180, 2 }
            } }
        };

        /*
        FOO L150
        BAR L90
        BAZ R60
        FAM L
        QUX L60
        ZIF R180
        YUB R30
        YOG F
        HEF R120
        LUG L120
        LIZ R150
        RIS L30
        NOM R
        TIL L180
        TOB R90
         */

        _encoding1 = new List<int>()
        {
            (int)Move.Lt + 150,
            (int)Move.Lt + 90,
            (int)Move.Rt + 60,
            (int)Move.Lt,
            (int)Move.Lt + 60,
            (int)Move.Rt + 180,
            (int)Move.Rt + 30,
            (int)Move.Fd,
            (int)Move.Rt + 120,
            (int)Move.Lt + 120,
            (int)Move.Rt + 150,
            (int)Move.Lt + 30,
            (int)Move.Rt,
            (int)Move.Lt + 180,
            (int)Move.Rt + 90,
        };

        _encoding2 = new List<string>()
        {
            "FOO",
            "BAR",
            "BAZ",
            "FAM",
            "QUX",
            "ZIF",
            "YUB",
            "YOG",
            "HEF",
            "LUG",
            "LIZ",
            "RIS",
            "NOM",
            "TIL",
            "TOB"
        };

        _instructions = _shapes.ElementAt(Rnd.Range(0, _shapes.Count)).Value;
        Debug.LogFormat("[Turtle Robot #{0}] Original instructions: \n{1}", _moduleId, string.Join("\n", GetPencilCodeCommands(_instructions).ToArray()));

        _instructions = AddFaults(_instructions);
        Debug.LogFormat("[Turtle Robot #{0}] Added faults: \n{1}", _moduleId, string.Join("\n", GetPencilCodeCommands(_instructions).ToArray()));

        _instructions = Randomize(_instructions);
        Debug.LogFormat("[Turtle Robot #{0}] Module instructions: \n{1}", _moduleId, string.Join("\n", GetModuleInstructions(_instructions).ToArray()));
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Display.GetComponent<TextMesh>().text = String.Join("\n", new string[] {
            "Turtle Robot",
            "============",
            "  " + EncodeInstructions(_instructions[_cursor == 0 ? _instructions.Count() - 1 : _cursor - 1]),
            "> " + EncodeInstructions(_instructions[_cursor]),
            "  " + EncodeInstructions(_instructions[_cursor == _instructions.Count() - 1 ? 0  : _cursor + 1])
        });
    }

    private string EncodeInstructions(List<int> list)
    {
        int lookup = list[0] == (int)Move.Fd ? 0 : list[0] + list[1];
        return _encoding2[_encoding1.IndexOf(lookup)] + (list.Count() == 3 ? " " + list[2] : "");
    }

    void Update()
    {

    }

    private List<List<int>> AddFaults(List<List<int>> input)
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
            var nextType = next[0] == (int)Move.Fd ? MoveType.Line : (next.Count() == 2 ? MoveType.Rotation : MoveType.Arc);

            // Forward instruction
            if (input[i][0] == (int)Move.Fd)
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
                        output.Add(new List<int>() { (int)Move.Fd, _dist[Rnd.Range(0, _dist.Count())] });
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
                            output.Add(new List<int>() { (int)Move.Fd, _dist[Rnd.Range(0, _dist.Count())] });
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
                                output.Add(new List<int>() { (int)Move.Fd, _dist[Rnd.Range(0, _dist.Count())] });
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
                            output.Add(new List<int>() { (int)Move.Fd, _dist[Rnd.Range(0, _dist.Count())] });
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

    private List<List<int>> Randomize(List<List<int>> input)
    {
        // Sometimes switch left and right
        if (Rnd.Range(0, 2) == 0)
        {
            Console.WriteLine("Switching left and right");
            foreach (var instruction in input)
            {
                if (instruction[0] == (int)Move.Lt)
                    instruction[0] = (int)Move.Rt;
                else if (instruction[0] == (int)Move.Rt)
                    instruction[0] = (int)Move.Lt;
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
        foreach (var instruction in input)
            if ((Move)instruction[0] == Move.Fd || instruction.Count == 3)
                instruction[instruction.Count - 1] *= factor;

        // Random starting point
        // 0, 1, 2, 3
        var num = Rnd.Range(0, input.Count());
        Console.WriteLine("Starting at " + (input.Count() - num));

        for (var i = 0; i < num; i++)
        {
            input.Insert(0, input[input.Count() - 1]);
            input.RemoveAt(input.Count() - 1);
        }

        return input;
    }

    private List<string> GetPencilCodeCommands(List<List<int>> instructions)
    {
        var list = new List<string>
        {
            "speed 200",
            "pen black, 5"
        };

        var factor = 1;

        foreach (var instruction in instructions)
        {
            factor = (Move)instruction[0] == Move.Fd || instruction.Count == 3 ? 25 : 1;
            var str = ((Move)instruction[0]).ToString().ToLower() + " ";
            if (instruction.Count == 2)
                str += (instruction[1] * factor);
            else
                str += (instruction[1] + ", " + instruction[2] * factor);

            list.Add(str);
        }

        return list;
    }

    private List<string> GetModuleInstructions(List<List<int>> instructions)
    {
        var list = new List<string>();

        foreach (var instruction in instructions)
        {
            var str = ((Move)instruction[0]).ToString() + " " + instruction[1];
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
        if (_cursor >= _instructions.Count()) _cursor = 0;
        else if (_cursor < 0) _cursor = _instructions.Count() - 1;
        UpdateDisplay();
    }

    private void PressDelete()
    {
        _instructions.RemoveAt(_cursor);
        UpdateDisplay();
    }
}
