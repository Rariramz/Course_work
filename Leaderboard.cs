using System.Collections.Generic;

namespace Minesweeper
{
    public class Leaderboard
    {
        public List<(string, int)> Easy { get; set; }
        public List<(string, int)> Medium { get; set; }
        public List<(string, int)> Hard { get; set; }
        public enum Difficulties
        {
            Easy,
            Medium,
            Hard
        }
        public Leaderboard()
        {
            Easy = new List<(string, int)>();
            Medium = new List<(string, int)>();
            Hard = new List<(string, int)>();
        }
        
        public void Add(string name, int time, Difficulties difficulty)
        {
            if (difficulty == Difficulties.Easy)
            {
                Easy.Add((name, time));
                Easy.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            }
            else if (difficulty == Difficulties.Medium)
            {
                Medium.Add((name, time));
                Medium.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            }
            else if (difficulty == Difficulties.Hard)
            {
                Hard.Add((name, time));
                Hard.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            }
        }
    }
}
