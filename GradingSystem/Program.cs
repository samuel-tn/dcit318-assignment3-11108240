using System;
using System.Collections.Generic;
using System.IO;

// ---------------- Custom Exceptions ----------------
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// ---------------- Student Class ----------------
public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Score { get; set; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

// ---------------- StudentResultProcessor Class ----------------
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                string[] parts = line.Split(',');

                if (parts.Length != 3)
                    throw new MissingFieldException($"Missing fields on line {lineNumber}: '{line}'");

                try
                {
                    int id = int.Parse(parts[0].Trim());
                    string fullName = parts[1].Trim();
                    if (string.IsNullOrWhiteSpace(fullName))
                        throw new MissingFieldException($"Name is missing on line {lineNumber}");

                    if (!int.TryParse(parts[2].Trim(), out int score))
                        throw new InvalidScoreFormatException($"Invalid score format on line {lineNumber}: '{parts[2]}'");

                    students.Add(new Student(id, fullName, score));
                }
                catch (FormatException)
                {
                    throw new InvalidScoreFormatException($"Invalid ID format on line {lineNumber}: '{parts[0]}'");
                }
            }
        }
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

// ---------------- Main Application ----------------
class Program
{
    static void Main()
    {
        string inputFilePath = "students.txt";
        string outputFilePath = "report.txt";

        // Step 1: Let the user enter student details
        Console.Write("How many students do you want to enter? ");
        int count = int.Parse(Console.ReadLine());

        using (var writer = new StreamWriter(inputFilePath))
        {
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"\n--- Enter details for Student {i + 1} ---");
                Console.Write("ID: ");
                string id = Console.ReadLine();

                Console.Write("Full Name: ");
                string name = Console.ReadLine();

                Console.Write("Score: ");
                string score = Console.ReadLine();

                writer.WriteLine($"{id}, {name}, {score}");
            }
        }

        var processor = new StudentResultProcessor();

        try
        {
            var students = processor.ReadStudentsFromFile(inputFilePath);
            processor.WriteReportToFile(students, outputFilePath);
            Console.WriteLine($"\nReport successfully generated: {outputFilePath}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
