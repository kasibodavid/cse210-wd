using System;
using System.Collections.Generic;

// Comment class represents a single comment on a video
public class Comment
{
    public string CommenterName { get; set; }
    public string CommentText { get; set; }

    public Comment(string commenterName, string commentText)
    {
        CommenterName = commenterName;
        CommentText = commentText;
    }
}

// Video class represents a YouTube video with title, author, length, and comments
public class Video
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int LengthInSeconds { get; set; }
    private List<Comment> Comments { get; set; } = new List<Comment>();

    public Video(string title, string author, int lengthInSeconds)
    {
        Title = title;
        Author = author;
        LengthInSeconds = lengthInSeconds;
    }

    // Add a comment to the video
    public void AddComment(Comment comment)
    {
        Comments.Add(comment);
    }

    // Return the number of comments
    public int GetCommentCount()
    {
        return Comments.Count;
    }

    // Display video details and its comments
    public void DisplayVideoInfo()
    {
        Console.WriteLine($"Title: {Title}");
        Console.WriteLine($"Author: {Author}");
        Console.WriteLine($"Length: {LengthInSeconds} seconds");
        Console.WriteLine($"Number of Comments: {GetCommentCount()}");
        Console.WriteLine("Comments:");
        foreach (var comment in Comments)
        {
            Console.WriteLine($" - {comment.CommenterName}: {comment.CommentText}");
        }
        Console.WriteLine();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World! This is the YouTubeVideos Project.");
        
        // Create a list of videos
        List<Video> videos = new List<Video>();

        // Video 1
        Video video1 = new Video("Learn C# in 10 Minutes", "Tech Guru", 600);
        video1.AddComment(new Comment("Alice", "This was so helpful!"));
        video1.AddComment(new Comment("Bob", "Great explanation, thanks."));
        video1.AddComment(new Comment("Charlie", "Could you do one on LINQ?"));
        videos.Add(video1);

        // Video 2
        Video video2 = new Video("Top 10 Programming Languages", "Code World", 900);
        video2.AddComment(new Comment("Daniel", "Python forever!"));
        video2.AddComment(new Comment("Eva", "I prefer JavaScript."));
        video2.AddComment(new Comment("Frank", "Good list, very informative."));
        videos.Add(video2);

        // Video 3
        Video video3 = new Video("History of Computers", "Tech Talks", 1200);
        video3.AddComment(new Comment("Grace", "Fascinating!"));
        video3.AddComment(new Comment("Hannah", "Loved the visuals."));
        video3.AddComment(new Comment("Ian", "So much history packed in!"));
        videos.Add(video3);

        // Loop through and display all video info
        foreach (var video in videos)
        {
            video.DisplayVideoInfo();
        }
    }
}
