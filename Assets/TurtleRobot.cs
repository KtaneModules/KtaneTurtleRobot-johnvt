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
    public GameObject Cursor;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _isSolved;

    private enum Verb { Fd, Lt, Rt }
    private enum MoveType { Line, Rotation, Arc }

    private int[] _distFactor = { 1, 2, 3, 4, 6 };
    private int _cursor = 0;
    private List<Command> _commands;

    private Dictionary<string, List<Command>> _shapes;

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
            { "dog_house", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 30 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 120 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 30 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Lt, Degrees = 180, Distance = 1 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
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
            { "shape_shift", new List<Command>() {
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Lt, Degrees = 90, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 30 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 120 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 30 },
                new Command() { Verb = Verb.Fd, Distance = 4 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Lt, Degrees = 90, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
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
                new Command() { Verb = Verb.Fd, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 1 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Lt, Degrees = 180, Distance = 1 },
                new Command() { Verb = Verb.Rt, Degrees = 90 },
                new Command() { Verb = Verb.Fd, Distance = 2 },
                new Command() { Verb = Verb.Rt, Degrees = 180, Distance = 1 },
                new Command() { Verb = Verb.Fd, Distance = 1 },
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

        _commands = _shapes.ElementAt(Rnd.Range(0, _shapes.Count)).Value;
        //Debug.LogFormat("[Turtle Robot #{0}] Initial commands: \n{1}", _moduleId, string.Join("\n", GetPencilCodeCommands(_commands).ToArray()));

        _commands = AddBugs(_commands);
        //Debug.LogFormat("[Turtle Robot #{0}] Add bugs: \n{1}", _moduleId, string.Join("\n", GetPencilCodeCommands(_commands).ToArray()));

        _commands = Randomize(_commands);
        //Debug.LogFormat("[Turtle Robot #{0}] Randomize: \n{1}", _moduleId, string.Join("\n", GetPencilCodeCommands(_commands).ToArray()));

        _commands = RandomFactor(_commands);
        Debug.LogFormat("[Turtle Robot #{0}] Solution, bugs marked with #:", _moduleId);
        foreach (var command in _commands)
        {
            Debug.LogFormat("[Turtle Robot #{0}] - {1}", _moduleId, FormatCommand(command, true));
        }

        StartCoroutine(Blink());
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Display.GetComponent<TextMesh>().text = String.Join("\n", new string[] {
            "Turtle Robot",
            "============",
            "  " + FormatCommand(_commands[_cursor == 0 ? _commands.Count() - 1 : _cursor - 1]),
            "> " + FormatCommand(_commands[_cursor]),
            "  " + FormatCommand(_commands[_cursor == _commands.Count() - 1 ? 0  : _cursor + 1])
        });
    }

    private string FormatCommand(Command command, bool forLog = false)
    {
        return (forLog && command.Bug ? "#" : "")
            + (!forLog && command.Commented ? "#" : "")
            + command.Verb.ToString().ToUpper()
            + (command.Degrees != 0 ? " " + command.Degrees.ToString() : "")
            + (command.Distance != 0 ? " " + command.Distance.ToString() : "");
    }

    public IEnumerator Blink()
    {
        yield return new WaitForSeconds(Rnd.Range(0f, 1f));
        var on = true;
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            on = !on;
            Cursor.SetActive(on);
        }
    }

    void Update()
    {

    }

    private List<Command> AddBugs(List<Command> commands)
    {
        var result = new List<Command>();

        // Determine random breakpoints so they are 3+ commands apart
        var breakpoints = new int[3];

        // Random breakpoints
        TryAgain:
        for (var i = 0; i < breakpoints.Count(); i++)
            breakpoints[i] = Rnd.Range(0, commands.Count);

        // For each pair check if they have enough distance between them
        for (var i = 0; i < breakpoints.Count(); i++)
            for (var j = i; j < breakpoints.Count(); j++)
                if (i != j && GetMinDistance(breakpoints[i], breakpoints[j], commands.Count()) < 3)
                    goto TryAgain;

        //Debug.Log("Breakpoints: " + String.Join(", ", breakpoints.Select(x => x.ToString()).ToArray()));

        for (var i = 0; i < commands.Count; i++)
        {
            // Not a breakpoint, just copy to new lines
            if (!breakpoints.Contains(i))
            {
                result.Add((Command)_commands[i].Clone());
                continue;
            }

            // Determine next command
            var next = commands[i == commands.Count - 1 ? 0 : i + 1];
            var nextType = next.Verb == Verb.Fd ? MoveType.Line : (next.Distance == 0 ? MoveType.Rotation : MoveType.Arc);

            // Forward command
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
                        //Debug.Log("Splitting up and inserting a rotation.");
                        result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Bug = true });
                    }
                    else
                    {
                        //Debug.Log("Splitting up and inserting an arc.");
                        result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Distance = RandomDistance(), Bug = true });
                    }

                    // Add second part
                    result.Add((Command)(commands[i].Clone()));
                    result[result.Count() - 1].Distance -= firstPart;
                }

                // Otherwise
                else
                {
                    // Add command
                    result.Add((Command)commands[i].Clone());

                    // Add bug
                    if (nextType == MoveType.Arc)
                    {
                        //Debug.Log("Adding a rotation.");
                        result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Bug = true });
                    }
                    else
                    {
                        //Debug.Log("Adding an arc.");
                        result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Distance = RandomDistance(), Bug = true });
                    }
                }
            }

            // Rotate or arc command
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
                        //Debug.Log("Splitting up and inserting a straight line.");
                        result.Add(new Command() { Verb = Verb.Fd, Distance = RandomDistance(), Bug = true });
                    }
                    else
                    {
                        // If we are splitting up a rotation
                        if (commands[i].Distance == 0)
                        {
                            //Debug.Log("Splitting up and inserting an arc.");
                            result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Distance = RandomDistance(), Bug = true });
                        }

                        // If we are splitting up an arc
                        else
                        {
                            //Debug.Log("Splitting up and inserting a rotation.");
                            result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Bug = true });
                        }
                    }

                    // Add second part
                    result.Add((Command)commands[i].Clone());
                    result[result.Count() - 1].Degrees = 90;
                }

                // Otherwise
                else
                {
                    // Add command
                    result.Add((Command)commands[i].Clone());

                    // Add bug
                    // If this is a rotate
                    if (commands[i].Distance == 0)
                    {
                        if (nextType == MoveType.Arc)
                        {
                            //Debug.Log("Adding a straight line.");
                            result.Add(new Command() { Verb = Verb.Fd, Distance = RandomDistance(), Bug = true });
                        }
                        else
                        {
                            //Debug.Log("Adding an arc.");
                            result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Distance = RandomDistance(), Bug = true });
                        }
                    }

                    // If this is an arc
                    else
                    {
                        if (nextType == MoveType.Arc)
                        {
                            if (Rnd.Range(0f, 1f) < .5)
                            {
                                //Debug.Log("Adding a straight line.");
                                result.Add(new Command() { Verb = Verb.Fd, Distance = RandomDistance(), Bug = true });
                            }
                            else
                            {
                                //Debug.Log("Adding a rotation.");
                                result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Bug = true });
                            }
                        }
                        else if (nextType == MoveType.Rotation)
                        {
                            //Debug.Log("Adding a straight line.");
                            result.Add(new Command() { Verb = Verb.Fd, Distance = RandomDistance(), Bug = true });
                        }
                        else
                        {
                            //Debug.Log("Adding a rotation.");
                            result.Add(new Command() { Verb = LtOrRt(), Degrees = 90, Bug = true });
                        }
                    }
                }
            }
        }

        return result;
    }

    private List<Command> RandomFactor(List<Command> input)
    {
        var _factor = _distFactor[Rnd.Range(0, _distFactor.Count())];
        //Debug.Log("Applying factor " + _factor);
        foreach (var command in input)
            command.Distance *= _factor;
        return input;
    }

    private List<Command> Randomize(List<Command> input)
    {
        // Sometimes switch left and right
        if (Rnd.Range(0f, 1f) < .5)
        {
            //Debug.Log("Switching left and right");
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
            //Debug.Log("Reversing order");
            input.Reverse();
        }

        // Random starting point
        // 0, 1, 2, 3
        var num = Rnd.Range(0, input.Count());
        //Debug.Log("Starting at " + (input.Count() - num));
        for (var i = 0; i < num; i++)
        {
            input.Insert(0, input[input.Count() - 1]);
            input.RemoveAt(input.Count() - 1);
        }

        return input;
    }

    private List<string> GetPencilCodeCommands(List<Command> commands)
    {
        var list = new List<string>
        {
            "speed 200",
            "pen black, 5"
        };

        foreach (var command in commands)
        {
            var str = command.Verb.ToString().ToLower() + " "
                + (command.Degrees > 0 ? command.Degrees.ToString() : "")
                + (command.Degrees > 0 && command.Distance > 0 ? ", " : "")
                + (command.Distance > 0 ? (command.Distance * 25).ToString() : "");

            list.Add(str);
        }

        return list;
    }

    private void PressArrow(int direction)
    {
        if (_isSolved) return;

        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch(.5f);
        Cursor.SetActive(true);

        _cursor += direction;
        if (_cursor >= _commands.Count()) _cursor = 0;
        else if (_cursor < 0) _cursor = _commands.Count() - 1;
        UpdateDisplay();
    }

    private void PressDelete()
    {
        if (_isSolved) return;

        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch(.5f);
        Cursor.SetActive(true);

        if (!_commands[_cursor].Bug)
        {
            GetComponent<KMBombModule>().HandleStrike();
        }
        else
        {
            _commands[_cursor].Commented = true;
            UpdateDisplay();
        }

        if (_commands.Count(x => x.Commented) == _commands.Count(x => x.Bug))
        {
            _isSolved = true;
            GetComponent<KMAudio>().PlaySoundAtTransform("plotter", transform);
            GetComponent<KMBombModule>().HandlePass();
        }
    }

    private int GetMinDistance(int a, int b, int length)
    {
        return new[] {
            Math.Abs(a - b),
            Math.Abs(a + length - b),
            Math.Abs(a - length - b)
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
        public bool Bug { get; set; }
        public bool Commented { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

#pragma warning disable 414
    private string TwitchHelpMessage = @"Use “!{0} up” to press the up button. Use “!{0} down 3” to press the down button three times.  Use “!{0} comment” to comment out the current position. Shortcuts: u, d, #.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        var split = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

	    if (split[0] == "press")
	    {
		    split = split.Skip(1).ToArray();
	    }

        if ((new string[] { "up", "u", "down", "d" }).Contains(split[0]))
        {
            int amount = 1;

            if (split.Length == 2 && "123456789".Contains(split[1]) && int.Parse(split[1]) < 17)
            {
                amount = int.Parse(split[1]);
            }

            yield return null;

            for (int x = 0; x < amount; x++)
            {
                PressArrow((split[0] == "up" || split[0] == "u") ? -1 : 1);
                yield return new WaitForSeconds(.1f);
            }
        }

        if (split.Length == 1 && split[0] == "comment" || split[0] == "#")
        {
            yield return null;
            PressDelete();
        }
    }    
}
