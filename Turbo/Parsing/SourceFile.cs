namespace Turbo.Language.Parsing;

public class SourceFile
{
    public FileInfo? FileInfo { get; }

    public bool EndOfFile => IsEndOfFile();
    public char Current => GetCurrentCharacter();
    public char Peek => GetNextCharacter();
    public int CurrentIndex => Math.Min(_currentIndex, _text.Length);
    public int CurrentLine => _currentLineNumber;
    public bool IsNewLine => IsCurrentNewLine();
    
    private readonly string _text;
    private readonly List<(int start, int end)> _lines = [];
    private LineEndingStyle _lineEndingStyle;

    private int _currentIndex = -1;
    private int _currentLineNumber = 0;
    
    public SourceFile(string text)
    {
        _text = text;
        
        InitializeLines();
    }

    public SourceFile(FileInfo fileInfo)
    {
        FileInfo = fileInfo;
        
        // TODO: maybe throw here if the file was not successfully opened
        _text = File.ReadAllText(fileInfo.FullName);
        
        InitializeLines();
    }

    public void MoveNext()
    {
        if (_currentIndex >= _text.Length) return;

        _currentIndex++;

        if (IsCurrentNewLine())
        {
            _currentIndex = _lineEndingStyle is LineEndingStyle.Windows
                ? Math.Min(_currentIndex + 2, _text.Length)
                : Math.Min(_currentIndex + 1, _text.Length);

            _currentLineNumber++;
        }
    }

    public void MoveToNonWhiteSpaceCharacter()
    {
        MoveNext();

        while (_currentIndex < _text.Length && char.IsWhiteSpace(_text[_currentIndex]))
        {
            MoveNext();
        }
    }

    public void MoveToNextLine()
    {
        var lastLineNumber = _currentLineNumber;
        while (lastLineNumber == _currentLineNumber)
        {
            MoveNext();
        }
    }
    
    private char GetNextCharacter()
    {
        if (_currentIndex + 1 < 0 || _currentIndex + 1 >= _lines.Count) return '\0';
        return _text[_currentIndex + 1];
    }
    
    private char GetCurrentCharacter()
    {
        if (_currentIndex < 0 || _currentIndex >= _lines.Count) throw new IndexOutOfRangeException("Failed to get the current character of the source file.");
        return _text[_currentIndex];
    }

    private bool IsEndOfFile()
    {
        return _currentIndex >= _text.Length;
    }
    
    private bool IsCurrentNewLine()
    {
        return _lineEndingStyle switch
        {
            LineEndingStyle.Windows => _text[_currentIndex] is '\r' && _currentIndex + 1 < _text.Length && _text[_currentIndex + 1] is '\n',
            LineEndingStyle.MacOS => _text[_currentIndex] is '\r',
            LineEndingStyle.Unix => _text[_currentIndex] is '\n',
            _ => false
        };
    }

    private void InitializeLines()
    {
        var startIndex = 0;
        
        for (var i = 0; i < _text.Length; i++)
        {
            if (_text[i] is '\r' && i + 1 < _text.Length && _text[i + 1] == '\n')
            {
                if (_lineEndingStyle is not LineEndingStyle.None and not LineEndingStyle.Windows)
                {
                    throw new InvalidOperationException("Cannot have a source file with more than one type of line ending.");
                }
                
                _lineEndingStyle = LineEndingStyle.Windows;
                i++;

                _lines.Add((startIndex, i));
                
                startIndex = i + 1;
            }
            
            if (_text[i] is '\r')
            {
                if (_lineEndingStyle is not LineEndingStyle.None and not LineEndingStyle.MacOS)
                {
                    throw new InvalidOperationException("Cannot have a source file with more than one type of line ending.");
                }
                
                _lineEndingStyle = LineEndingStyle.MacOS;
                
                _lines.Add((startIndex, i));

                startIndex = i + 1;
            }
            
            if (_text[i] is '\n')
            {
                if (_lineEndingStyle is not LineEndingStyle.None and not LineEndingStyle.Unix)
                {
                    throw new InvalidOperationException("Cannot have a source file with more than one type of line ending.");
                }
                
                _lineEndingStyle = LineEndingStyle.Unix;

                _lines.Add((startIndex, i));
                
                startIndex = i + 1;
            }
        }
    }

    public enum LineEndingStyle
    {
        None,
        Windows,
        MacOS,
        Unix
    }
}