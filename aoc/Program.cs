﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Math;

namespace aoc;

public class Program
{
    static void Main()
    {
        Main_1_1();
        Main_1_2();
    }
    
    static void Main_8_2()
    {
        var lines = File
            .ReadAllLines("day8.txt");

        var max = 0L;
        for (var y = 1; y < lines.Length - 1; y++)
        for (var x = 1; x < lines[0].Length - 1; x++)
        {
            var r = 1L;
            var c = 0L;
            for (var x2 = x + 1; x2 < lines[0].Length; x2++)
            {
                c++;
                if (lines[y][x2] >= lines[y][x])
                    break;
            }
            r *= c;
            c = 0L;
            for (var x2 = x - 1; x2 >= 0; x2--)
            {
                c++;
                if (lines[y][x2] >= lines[y][x])
                    break;
            }
            r *= c;
            c = 0L;
            for (var y2 = y + 1; y2 < lines.Length; y2++)
            {
                c++;
                if (lines[y2][x] >= lines[y][x])
                    break;
            }
            r *= c;
            c = 0L;
            for (var y2 = y - 1; y2 >= 0; y2--)
            {
                c++;
                if (lines[y2][x] >= lines[y][x])
                    break;
            }
            r *= c;
            if (r > max)
                max = r;
        }

        Console.WriteLine(max);
    }

    static void Main_8_1()
    {
        var lines = File
            .ReadAllLines("day8.txt");
        
        var map = new Map<bool>(lines[0].Length, lines.Length);
        
        for (var y = 0; y < map.sizeY; y++)
        {
            var cur = -1;
            for (var x = 0; x < map.sizeX; x++)
            {
                var next = lines[y][x] - '0';
                if (next > cur)
                {
                    cur = next;
                    map[new V(x, y)] = true;
                }
            }
            cur = -1;
            for (var x = map.sizeX - 1; x >= 0; x--)
            {
                var next = lines[y][x] - '0';
                if (next > cur)
                {
                    cur = next;
                    map[new V(x, y)] = true;
                }
            }
        }

        for (var x = 0; x < map.sizeX; x++)
        {
            var cur = -1;
            for (var y = 0; y < map.sizeY; y++)
            {
                var next = lines[y][x] - '0';
                if (next > cur)
                {
                    cur = next;
                    map[new V(x, y)] = true;
                }
            }
            cur = -1;
            for (var y = map.sizeX - 1; y >= 0; y--)
            {
                var next = lines[y][x] - '0';
                if (next > cur)
                {
                    cur = next;
                    map[new V(x, y)] = true;
                }
            }
        }
        
        Console.WriteLine(map.All().Count(v => map[v]));
    }

    class Entry
    {
        public Entry(bool isDir, Entry? parent)
        {
            IsDir = isDir;
            Parent = parent;
            FlattenDirs = parent?.FlattenDirs ?? new List<Entry>();
            if (isDir)
                FlattenDirs.Add(this);
        }

        public List<Entry> FlattenDirs { get; }
        public bool IsDir { get; }
        public Entry? Parent { get; }
        public long Size { get; set; }
        public Dictionary<string, Entry> Children { get; } = new();
    }

    static void Main_7()
    {
        var lines = File
            .ReadAllLines("day7.txt")
            .Select(x => x.Split())
            .ToArray();

        var root = new Entry(true, null);
        var cur = root;
        foreach (var line in lines)
        {
            if (line[0] == "$")
            {
                if (line[1] == "cd")
                {
                    cur = line[2] switch
                    {
                        "/" => root,
                        ".." => cur.Parent!,
                        _ => cur.Children[line[2]]
                    };
                }
            }
            else
            {
                if (cur.Children.ContainsKey(line[1]))
                    continue;
                if (line[0] == "dir")
                    cur.Children.Add(line[1], new Entry(true, cur));
                else
                {
                    var size = long.Parse(line[0]);
                    cur.Children.Add(line[1], new Entry(false, cur) { Size = size });
                    for (var c = cur; c != null; c = c.Parent)
                        c.Size += size;
                }
            }
        }

        Console.WriteLine($"Part 1: {root.FlattenDirs.Where(x => x.Size <= 100000).Sum(x => x.Size)}");

        var spaceLeft = 70000000L - root.Size;
        var spaceToFree = 30000000L - spaceLeft;
        var dirToRemove = root.FlattenDirs.OrderBy(x => x.Size).SkipWhile(x => x.Size < spaceToFree).First();

        Console.Out.WriteLine($"Part 2: {dirToRemove.Size}");
    }

    static void Main_6_2()
    {
        const int count = 14;

        var input = File.ReadAllLines("day6.txt")[0];
        for (var i = 0; i < input.Length; i++)
        {
            if (input.Substring(i, count).ToHashSet().Count == count)
            {
                Console.WriteLine(i + count);
                return;
            }
        }
    }

    static void Main_6_1()
    {
        const int count = 4;

        var input = File.ReadAllLines("day6.txt")[0];
        for (var i = 0; i < input.Length; i++)
        {
            if (input.Substring(i, count).ToHashSet().Count == count)
            {
                Console.WriteLine(i + count);
                return;
            }
        }
    }

    static void Main_5_2()
    {
        var regions = File
            .ReadAllLines("day5.txt")
            .Regions();

        var stacks = regions[0]
            .SkipLast(1)
            .RotateCW()
            .EveryNth(4, startFrom: 1)
            .Select(x => new Stack<char>(x.Where(c => c != ' ')))
            .ToArray();

        var moves = regions[1]
            .ParseAll<(string, long num, string, long from, string, long to)>();

        foreach (var (_, num, _, from, _, to) in moves)
        {
            var tmp = new Stack<char>();
            for (var i = 0; i < num; i++)
                tmp.Push(stacks[from - 1].Pop());
            for (var i = 0; i < num; i++)
                stacks[to - 1].Push(tmp.Pop());
        }

        Console.WriteLine(new string(stacks.Select(x => x.First()).ToArray()));
    }

    static void Main_5_1()
    {
        var regions = File
            .ReadAllLines("day5.txt")
            .Regions();

        var stacks = regions[0]
            .SkipLast(1)
            .RotateCW()
            .EveryNth(4, startFrom: 1)
            .Select(x => new Stack<char>(x.Where(c => c != ' ')))
            .ToArray();

        var moves = regions[1]
            .ParseAll<(string, long num, string, long from, string, long to)>();

        foreach (var (_, num, _, from, _, to) in moves)
            for (var i = 0; i < num; i++)
                stacks[to - 1].Push(stacks[from - 1].Pop());

        Console.WriteLine(new string(stacks.Select(x => x.First()).ToArray()));
    }

    static void Main_4_2()
    {
        Console.WriteLine(File
            .ReadAllLines("day4.txt")
            .Select(x => x.Parse<(R, R)>())
            .Count(x => x.Item1.Overlaps(x.Item2)));
    }

    static void Main_4_1()
    {
        Console.WriteLine(File
            .ReadAllLines("day4.txt")
            .Select(x => x.Parse<(R, R)>())
            .Count(x => x.Item1.Contains(x.Item2) || x.Item2.Contains(x.Item1)));
    }

    static void Main_3_2()
    {
        Console.Out.WriteLine(File
            .ReadAllLines("day3.txt")
            .Batch(3)
            .Select(b => b.Aggregate(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
                (x, y) => x.Intersect(y).ToArray()
            ).Single())
            .Select(c => c switch
            {
                >= 'a' and <= 'z' => c - 'a' + 1,
                >= 'A' and <= 'Z' => c - 'A' + 27,
                _ => throw new Exception()
            }).Sum());
    }

    static void Main_3_1()
    {
        Console.Out.WriteLine(File
            .ReadAllLines("day3.txt")
            .Select(x => new[] { x[..(x.Length / 2)], x[(x.Length / 2)..] })
            .Select(b => b.Aggregate(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
                (x, y) => x.Intersect(y).ToArray()
            ).Single())
            .Select(c => c switch
            {
                >= 'a' and <= 'z' => c - 'a' + 1,
                >= 'A' and <= 'Z' => c - 'A' + 27,
                _ => throw new Exception()
            }).Sum());
    }

    static void Main_2_2()
    {
        Console.Out.WriteLine(File
            .ReadAllLines("day2.txt")
            .ParseAll<(char v1, char outcome)>()
            .Select(x => (v1: x.v1 - 'A', outcome: x.outcome - 'X'))
            .Select(x => (x.v1, v2: (x.v1 + x.outcome + 2) % 3))
            .Select(x => x.v2 + 1 + (x.v2 - x.v1 + 4) % 3 * 3)
            .Sum());
    }

    static void Main_2_1()
    {
        Console.Out.WriteLine(File
            .ReadAllLines("day2.txt")
            .ParseAll<(char v1, char v2)>()
            .Select(x => (v1: x.v1 - 'A', v2: x.v2 - 'X'))
            .Select(x => x.v2 + 1 + (x.v2 - x.v1 + 4) % 3 * 3)
            .Sum());
    }

    static void Main_1_2()
    {
        Console.Out.WriteLine(File
            .ReadAllLines("day1.txt")
            .Regions()
            .Select(x => x.Select(long.Parse).Sum())
            .OrderDescending()
            .Take(3)
            .Sum());
    }

    static void Main_1_1()
    {
        Console.Out.WriteLine(File
            .ReadAllLines("day1.txt")
            .Regions()
            .Select(x => x.Select(long.Parse).Sum())
            .Max());
    }
}