namespace Turbo.Language.Parsing;

public interface IReadOnlySourceFile
{
    public FileInfo? FileInfo { get; }
    public bool EndOfFile { get; }
    public char Current { get; }
    public char Peek { get; }
    public int CurrentIndex { get; }
    public int CurrentLine { get; }
    public bool IsNewLine { get; }

    public ReadOnlySpan<char> GetSpan(int start, int end);
    public ReadOnlySpan<char> GetLineSpan(int line);
    public (int start, int end) GetLineStartAndEnd(int line);
}

public class SourceFile : IReadOnlySourceFile
{
    public FileInfo? FileInfo { get; }

    public bool EndOfFile => IsEndOfFile();
    public char Current => PeekCurrentCharacter();
    public char Peek => PeekNextCharacter();
    public int CurrentIndex => Math.Min(_currentIndex, _text.Length);
    public int CurrentLine => _currentLineNumber;
    public bool IsNewLine => GetCurrentLineEnding() is not LineEndingStyle.None;
    
    private readonly char[] _text;
    private readonly List<(int start, int end)> _lines = [];

    private int _currentIndex = -1;
    private int _currentLineNumber = 1;
    
    public SourceFile(string text)
    {
        _text = [..text.ToCharArray(), '\0'];
        
        InitializeLines();
    }

    public SourceFile(FileInfo fileInfo)
    {
        FileInfo = fileInfo;
        
        // TODO: maybe throw here if the file was not successfully opened
        _text = [..File.ReadAllText(fileInfo.FullName).ToCharArray(), '\0'];
        
        InitializeLines();
    }

    /// <summary>
    /// Move to the next character in the source file.
    /// </summary>
    public void MoveNext()
    {
        if (_currentIndex >= _text.Length) return;
        if (TryMovePastNewLine()) return;

        _currentIndex++;
        
        // if we land on a new line move past it
        TryMovePastNewLine();
    }

    /// <summary>
    /// Move to the next character in the source file and don't skip over new lines instantly.
    /// </summary>
    public void MoveNextAndRespectNewLines()
    {
        if (_currentIndex >= _text.Length) return;
        if (TryMovePastNewLine()) return;

        _currentIndex++;
    }

    /// <summary>
    /// Move to the next non-whitespace character in the source file.
    /// </summary>
    public void MoveToNonWhiteSpaceCharacter()
    {
        MoveNext();

        while (_currentIndex < _text.Length && char.IsWhiteSpace(_text[_currentIndex]))
        {
            MoveNext();
        }
    }

    /// <summary>
    /// Move to the next line in the source file.
    /// </summary>
    public void MoveToNextLine()
    {
        var lastLineNumber = _currentLineNumber;
        while (lastLineNumber == _currentLineNumber)
        {
            MoveNext();
        }
    }

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> between <b>start</b> and <b>end</b> from the source file.
    /// </summary>
    /// <param name="start">The starting index of the span.</param>
    /// <param name="end">The ending index of the span.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> between start and end.</returns>
    /// <exception cref="IndexOutOfRangeException">If start or end are out of range.</exception>
    public ReadOnlySpan<char> GetSpan(int start, int end)
    {
        if (start < 0 || start >= _text.Length) throw new IndexOutOfRangeException("Start index is out of range.");
        if (end < 0 || end >= _text.Length) throw new IndexOutOfRangeException("End index is out of range.");

        var length = end - start + 1;
        return new ReadOnlySpan<char>(_text, start, length);
    }
    
    /// <summary>
    /// Gets a line from the source file as a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="line">The line number from the source file.</param>
    /// <returns>A line from the source file as a <see cref="ReadOnlySpan{T}"/>.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// If line is out of range. line should be a number between 1 and <see cref="int.MaxValue"/>.
    /// </exception>
    public ReadOnlySpan<char> GetLineSpan(int line)
    {
        // force line to be zero indexed
        line--;
        
        if (line < 0 || line >= _lines.Count) throw new IndexOutOfRangeException("Line number out of range.");

        var length = _lines[line].end - _lines[line].start + 1;

        var span = new ReadOnlySpan<char>(_text, _lines[line].start, length);

        return span.EndsWith("\r\n") 
            ? span[..(length - 2)] 
            : span[..(length - 1)];
    }

    /// <summary>
    /// Gets the <b>start</b> and <b>end</b> of a line from the source file.
    /// </summary>
    /// <param name="line">The line number from the source file.</param>
    /// <returns>A tuple representing the <b>start</b> and <b>end</b> of the line.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// If line is out of range. line should be a number between 1 and <see cref="int.MaxValue"/>.
    /// </exception>
    public (int start, int end) GetLineStartAndEnd(int line)
    {
        // force line to be zero indexed
        line--;
        
        if (line < 0 || line >= _lines.Count) throw new IndexOutOfRangeException("Line number out of range.");
        
        var length = _lines[line].end - _lines[line].start + 1;
        var span = new ReadOnlySpan<char>(_text, _lines[line].start, length);

        return span.EndsWith("\r\n")
            ? (_lines[line].start, _lines[line].end - 2)
            : (_lines[line].start, _lines[line].end - 1);
    }

    private bool TryMovePastNewLine()
    {
        // if we get to the end of a line GetCurrentLineEnding
        // will return something other than LineEndingStyle.None
        // which can help us determine how many characters to skip
        
        var lineEnding = GetCurrentLineEnding();
        if (lineEnding is LineEndingStyle.None) return false;
        
        _currentIndex = lineEnding is LineEndingStyle.Windows
            ? Math.Min(_currentIndex + 2, _text.Length)
            : Math.Min(_currentIndex + 1, _text.Length);

        _currentLineNumber++;

        return true;
    }
    
    private char PeekNextCharacter()
    {
        if (_currentIndex + 1 < 0 || _currentIndex + 1 >= _text.Length) return '\0';
        return _text[_currentIndex + 1];
    }
    
    private char PeekCurrentCharacter()
    {
        // this is supposed to throw
        // i know it is annoying sometimes
        // but it is important fix your code
        // if it causes this to throw
        if (_currentIndex < 0 || _currentIndex >= _text.Length) throw new IndexOutOfRangeException("Failed to get the current character of the source file.");
        return _text[_currentIndex];
    }

    private bool IsEndOfFile()
    {
        return _currentIndex >= _text.Length || _text[_currentIndex] is '\0';
    }
    
    private LineEndingStyle GetCurrentLineEnding()
    {
        if (_currentIndex < 0 || _currentIndex >= _text.Length) return LineEndingStyle.None;

        if (_text[_currentIndex] is '\r' && _currentIndex + 1 < _text.Length && _text[_currentIndex + 1] is '\n')
        {
            return LineEndingStyle.Windows;
        }

        return _text[_currentIndex] switch
        {
            '\r' => LineEndingStyle.MacOS,
            '\n' => LineEndingStyle.Unix,
            _ => LineEndingStyle.None
        };
    }

    private void InitializeLines()
    {
        var startIndex = 0;
        
        for (var i = 0; i < _text.Length; i++)
        {
            switch (_text[i])
            {
                case '\r' when i + 1 < _text.Length && _text[i + 1] == '\n':
                    i++;
                    _lines.Add((startIndex, i));
                    startIndex = i + 1;
                    break;
                case '\r':
                case '\n':
                    _lines.Add((startIndex, i));
                    startIndex = i + 1;
                    break;
            }
        }

        if (startIndex < _text.Length - 1)
        {
            _lines.Add((startIndex, _text.Length - 1));
        }
    }

    private enum LineEndingStyle
    {
        None,
        Windows,
        // ReSharper disable once InconsistentNaming
        MacOS,
        Unix
    }
}