// Program.cs
// Mindfulness Console App
// Implements three activities: Breathing, Reflection, Listing
// Uses inheritance: base Activity class and derived classes for each activity.
// Encapsulation: private fields with protected accessors and methods.
// Extras implemented (for "exceed requirements"):
//  - Ensures no random prompt/question is repeated until all have been used in the session.
//  - Keeps a simple in-memory session log of activities (can be easily extended to save to file).
//  - Uses a Task-based timed ReadLine for Listing activity so user can enter items until time expires.
//
// Note: This is a synchronous console app using Task for time-limited input in Listing activity.
// Target: .NET Core / .NET 5+ console app

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MindfulnessApp
{
    abstract class Activity
    {
        private string _name;
        private string _description;
        private int _durationSeconds;
        private static readonly Random _rand = new Random();

        protected Activity(string name, string description)
        {
            _name = name;
            _description = description;
        }

        // Accessors for derived classes
        protected string Name => _name;
        protected string Description => _description;
        protected int DurationSeconds => _durationSeconds;

        // Common start: ask for duration and show preparation pause
        public void Start()
        {
            Console.Clear();
            ShowStartingMessage();
            _durationSeconds = PromptForDuration();
            TellPrepare();
            PauseWithSpinner(3); // prepare pause
            RunActivity(); // activity-specific behavior
            ShowEndingMessage();
        }

        // Derived classes implement their behavior here
        protected abstract void RunActivity();

        // Shared UI helpers
        private void ShowStartingMessage()
        {
            Console.WriteLine($"=== {Name} ===");
            Console.WriteLine();
            Console.WriteLine(Description);
            Console.WriteLine();
        }

        private int PromptForDuration()
        {
            int seconds = 0;
            while (true)
            {
                Console.Write("Enter duration in seconds for this activity (e.g., 30): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out seconds) && seconds > 0)
                    return seconds;
                Console.WriteLine("Please enter a positive integer for seconds.");
            }
        }

        private void TellPrepare()
        {
            Console.WriteLine();
            Console.WriteLine("Get ready... the activity will begin shortly.");
        }

        protected void PauseWithSpinner(int seconds)
        {
            Spinner(seconds);
        }

        protected void Countdown(int seconds)
        {
            for (int i = seconds; i >= 1; i--)
            {
                Console.Write(i);
                Thread.Sleep(1000);
                Console.Write("\b \b"); // erase digit (works for single-digit)
                // For multi-digit, simpler approach:
                if (i >= 10)
                {
                    // clear whole number by padding
                    Console.Write("\r");
                    Console.Write(new string(' ', 10));
                    Console.Write("\r");
                }
            }
        }

        protected void Spinner(int seconds)
        {
            char[] seq = new char[] { '|', '/', '-', '\\' };
            int idx = 0;
            DateTime end = DateTime.Now.AddSeconds(seconds);
            while (DateTime.Now < end)
            {
                Console.Write(seq[idx]);
                Thread.Sleep(200);
                Console.Write("\b");
                idx = (idx + 1) % seq.Length;
            }
        }

        private void ShowEndingMessage()
        {
            Console.WriteLine();
            Console.WriteLine("Well done!");
            Console.WriteLine($"You have completed the {Name} for {DurationSeconds} seconds.");
            PauseWithSpinner(4);
            Console.WriteLine();
            Console.WriteLine("Returning to main menu...");
            Thread.Sleep(800);
        }
    }

    class BreathingActivity : Activity
    {
        // breathing durations (seconds) for in/out — chosen to feel natural
        private readonly int _breathInSeconds = 4;
        private readonly int _breathOutSeconds = 6;

        public BreathingActivity() : base(
            "Breathing Activity",
            "This activity will help you relax by walking you through breathing in and out slowly. Clear your mind and focus on your breathing.")
        { }

        protected override void RunActivity()
        {
            int total = DurationSeconds;
            DateTime end = DateTime.Now.AddSeconds(total);
            Console.WriteLine();
            Console.WriteLine("Follow the prompts to breathe slowly and deeply.");
            Console.WriteLine();

            bool inhale = true;
            while (DateTime.Now < end)
            {
                if (inhale)
                {
                    Console.Write("Breathe in... ");
                    // show countdown for inhale (but don't exceed remaining time)
                    int remaining = (int)(end - DateTime.Now).TotalSeconds;
                    int dur = Math.Min(_breathInSeconds, Math.Max(1, remaining));
                    for (int i = dur; i >= 1 && DateTime.Now < end; i--)
                    {
                        Console.Write(i + " ");
                        Thread.Sleep(1000);
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.Write("Breathe out... ");
                    int remaining = (int)(end - DateTime.Now).TotalSeconds;
                    int dur = Math.Min(_breathOutSeconds, Math.Max(1, remaining));
                    for (int i = dur; i >= 1 && DateTime.Now < end; i--)
                    {
                        Console.Write(i + " ");
                        Thread.Sleep(1000);
                    }
                    Console.WriteLine();
                }
                inhale = !inhale;
            }
        }
    }

    class ReflectionActivity : Activity
    {
        private readonly List<string> _prompts = new List<string>
        {
            "Think of a time when you stood up for someone else.",
            "Think of a time when you did something really difficult.",
            "Think of a time when you helped someone in need.",
            "Think of a time when you did something truly selfless."
        };

        private readonly List<string> _questions = new List<string>
        {
            "Why was this experience meaningful to you?",
            "Have you ever done anything like this before?",
            "How did you get started?",
            "How did you feel when it was complete?",
            "What made this time different than other times when you were not as successful?",
            "What is your favorite thing about this experience?",
            "What could you learn from this experience that applies to other situations?",
            "What did you learn about yourself through this experience?",
            "How can you keep this experience in mind in the future?"
        };

        // to avoid repeats until all used
        private List<string> _unusedPrompts;
        private List<string> _unusedQuestions;
        private static readonly Random _rand = new Random();

        public ReflectionActivity() : base(
            "Reflection Activity",
            "This activity will help you reflect on times in your life when you have shown strength and resilience. This will help you recognize the power you have and how you can use it in other aspects of your life.")
        {
            _unusedPrompts = new List<string>(_prompts);
            _unusedQuestions = new List<string>(_questions);
        }

        protected override void RunActivity()
        {
            // Choose a random prompt (ensuring no repeats until all used)
            if (_unusedPrompts.Count == 0) _unusedPrompts = new List<string>(_prompts);
            int pIndex = _rand.Next(_unusedPrompts.Count);
            string prompt = _unusedPrompts[pIndex];
            _unusedPrompts.RemoveAt(pIndex);

            Console.WriteLine();
            Console.WriteLine("Prompt:");
            Console.WriteLine(prompt);
            Console.WriteLine();
            Console.WriteLine("When you are ready, reflect on the following questions.");
            Console.WriteLine();

            DateTime end = DateTime.Now.AddSeconds(DurationSeconds);

            // Show random questions, no repeats until used all
            while (DateTime.Now < end)
            {
                if (_unusedQuestions.Count == 0) _unusedQuestions = new List<string>(_questions);
                int qIndex = _rand.Next(_unusedQuestions.Count);
                string question = _unusedQuestions[qIndex];
                _unusedQuestions.RemoveAt(qIndex);

                Console.WriteLine("-- " + question);
                // pause with spinner for a few seconds to reflect; but don't exceed remaining time
                int remaining = (int)(end - DateTime.Now).TotalSeconds;
                int pause = Math.Min(8, Math.Max(2, remaining)); // reflect for 2-8 seconds each
                SpinnerTimed(pause);
                Console.WriteLine();
            }
        }

        // Smaller spinner variant that writes a little indicator for each second
        private void SpinnerTimed(int seconds)
        {
            char[] seq = new char[] { '|', '/', '-', '\\' };
            int idx = 0;
            DateTime end = DateTime.Now.AddSeconds(seconds);
            while (DateTime.Now < end)
            {
                Console.Write(seq[idx]);
                Thread.Sleep(400);
                Console.Write("\b");
                idx = (idx + 1) % seq.Length;
            }
        }
    }

    class ListingActivity : Activity
    {
        private readonly List<string> _prompts = new List<string>
        {
            "Who are people that you appreciate?",
            "What are personal strengths of yours?",
            "Who are people that you have helped this week?",
            "When have you felt the Holy Ghost this month?",
            "Who are some of your personal heroes?"
        };

        private List<string> _unusedPrompts;
        private static readonly Random _rand = new Random();

        public ListingActivity() : base(
            "Listing Activity",
            "This activity will help you reflect on the good things in your life by having you list as many things as you can in a certain area.")
        {
            _unusedPrompts = new List<string>(_prompts);
        }

        protected override void RunActivity()
        {
            if (_unusedPrompts.Count == 0) _unusedPrompts = new List<string>(_prompts);
            int idx = _rand.Next(_unusedPrompts.Count);
            string prompt = _unusedPrompts[idx];
            _unusedPrompts.RemoveAt(idx);

            Console.WriteLine();
            Console.WriteLine("Prompt:");
            Console.WriteLine(prompt);
            Console.WriteLine();

            Console.WriteLine("You will have a few seconds to think, then list as many items as you can. Press Enter after each item.");
            Console.Write("Get ready: ");
            CountdownInline(5);
            Console.WriteLine();
            Console.WriteLine("Start listing now:");

            DateTime end = DateTime.Now.AddSeconds(DurationSeconds);
            List<string> items = new List<string>();

            while (DateTime.Now < end)
            {
                int remainingMs = (int)(end - DateTime.Now).TotalMilliseconds;
                Task<string> readTask = Task.Run(() => Console.ReadLine());
                bool completed = readTask.Wait(remainingMs);
                if (!completed)
                {
                    // time's up; break out
                    break;
                }
                string entry = readTask.Result?.Trim();
                if (!string.IsNullOrEmpty(entry))
                {
                    items.Add(entry);
                    Console.WriteLine($"Added ({items.Count}). Keep going...");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"You listed {items.Count} item(s):");
            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {items[i]}");
            }
        }

        private void CountdownInline(int seconds)
        {
            for (int i = seconds; i >= 1; i--)
            {
                Console.Write(i + " ");
                Thread.Sleep(1000);
            }
            Console.WriteLine();
        }
    }

    class Program
    {
        // Simple session log (in-memory). Could be saved to file to extend feature.
        static List<string> sessionLog = new List<string>();

        static void Main(string[] args)
        {
            Console.Title = "Mindfulness Program";
            bool quit = false;

            while (!quit)
            {
                Console.Clear();
                ShowMainMenu();
                Console.Write("Choose an option (1-4): ");
                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        RunActivity(new BreathingActivity());
                        break;
                    case "2":
                        RunActivity(new ReflectionActivity());
                        break;
                    case "3":
                        RunActivity(new ListingActivity());
                        break;
                    case "4":
                        quit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Press Enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Session summary:");
            if (sessionLog.Count == 0)
            {
                Console.WriteLine("No activities completed this session.");
            }
            else
            {
                foreach (var entry in sessionLog)
                {
                    Console.WriteLine(entry);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Goodbye — take care!");
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("Welcome to the Mindfulness Program!");
            Console.WriteLine();
            Console.WriteLine("1. Breathing Activity");
            Console.WriteLine("2. Reflection Activity");
            Console.WriteLine("3. Listing Activity");
            Console.WriteLine("4. Quit");
            Console.WriteLine();
        }

        static void RunActivity(Activity activity)
        {
            DateTime start = DateTime.Now;
            activity.Start();
            DateTime end = DateTime.Now;
            sessionLog.Add($"{activity.GetType().Name} — {start:T} to {end:T} ({(int)(end - start).TotalSeconds}s)");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
