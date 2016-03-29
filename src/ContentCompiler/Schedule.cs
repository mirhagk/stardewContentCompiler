using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentCompiler
{
    class Schedule
    {
        public static Schedule Decompile(Dictionary<string, string> content, string character)
        {
            var schedule = new Schedule() { Character = character };
            foreach (var keyPair in content)
            {
                if (keyPair.Key.EndsWith("_Replacement"))
                    continue;
                var value = keyPair.Value;
                schedule.ScheduledItems.Add(new Schedule.ScheduleItem()
                {
                    Key = keyPair.Key
                });
                if (value.StartsWith("NOT"))
                {
                    var pieces = value.Split('/')[0].Split(' ');
                    schedule.ScheduledItems.Last().Condition = new Schedule.ScheduleItem.NotCondition()
                    {
                        Type = pieces[1],
                        Name = pieces[2],
                        Level = int.Parse(pieces[3]),
                    };
                    value = string.Join("/", value.Split('/').Skip(1));
                }
                if (value.StartsWith("GOTO"))
                {
                    schedule.ScheduledItems.Last().GotoKey = keyPair.Value.Substring(5);
                    continue;
                }
                var items = value.Split('/');
                foreach (var item in items)
                {
                    var pieces = item.Split(' ');
                    schedule.ScheduledItems.Last().TargetLocations.Add(new Schedule.ScheduleItem.TargetLocationTime()
                    {
                        Time = int.Parse(pieces[0]),
                        Location = pieces[1],
                        X = int.Parse(pieces[2]),
                        Y = int.Parse(pieces[3]),
                        Direction = pieces[4].GetInt() ?? 2,
                    });
                }
                Console.WriteLine(keyPair.Key);
                Console.WriteLine(keyPair.Value);
            }
            return schedule;
        }
        public Dictionary<string, string> Compile()
        {
            throw new NotImplementedException();
        }
        public class ScheduleItem
        {
            public class TargetLocationTime
            {
                public int Time { get; set; }
                public string Location { get; set; }
                public int X { get; set; }
                public int Y { get; set; }
                public int Direction { get; set; }

            }
            public class NotCondition
            {
                public string Type { get; set; }
                public string Name { get; set; }
                public int Level { get; set; }
            }
            public string Key { get; set; }
            public List<TargetLocationTime> TargetLocations { get; set; } = new List<TargetLocationTime>();
            public string GotoKey { get; set; }
            public NotCondition Condition { get; set; }
        }
        public string Character { get; set; }
        public List<ScheduleItem> ScheduledItems { get; set; } = new List<ScheduleItem>();
    }
}
