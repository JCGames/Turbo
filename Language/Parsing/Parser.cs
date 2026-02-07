using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;

namespace Turbo.Language.Parsing;

public class Parser
{
    private readonly SourceFile _sourceFile;

    private readonly Func<char, bool> _isCharacterAllowedInIdentifier = c =>
        c is not ('(' or ')' or '{' or '}' or '"');
    
    public Parser(SourceFile sourceFile)
    {
        _sourceFile = sourceFile;
    }

    public List<ListNode> ParseFile()
    {
        _sourceFile.MoveNext();
        
        var list = new List<ListNode>();

        while (!_sourceFile.EndOfFile)
        {
            if (char.IsWhiteSpace(_sourceFile.Current))
            {
                _sourceFile.MoveToNonWhiteSpaceCharacter(); 
                if (_sourceFile.EndOfFile) break;
            }

            if (_sourceFile.Current is ';')
            {
                _sourceFile.MoveToNextLine();
                continue;
            }
            
            list.Add(ParseListNode());
            
            _sourceFile.MoveToNonWhiteSpaceCharacter();
        }

        return list;
    }

    private Node ParseNextInList()
    {
        return _sourceFile.Current switch
        {
            '(' => ParseListNode(),
            '\'' when _sourceFile.Peek is '(' => ParseListNode(),
            '\'' => ParseSymbolNode(),
            _ => ParseIdentifierNode()
        };
    }
    
    private ListNode ParseListNode()
    {
        var listNode = new ListNode
        {
            Location = Location.New(_sourceFile)
        };
        
        if (_sourceFile.Current is '\'')
        {
            listNode.IsQuoted = true;
            _sourceFile.MoveNext();

            if (_sourceFile.Current is not '(')
            {
                Report.Error("Expected an open parenthesis after the single quote.", Location.New(_sourceFile));
            }
        }
        else if (_sourceFile.Current is not '(')
        {
            Report.Error("Expected a list.", Location.New(_sourceFile));
        }
        
        _sourceFile.MoveToNonWhiteSpaceCharacter();
        
        while (!_sourceFile.EndOfFile)
        {
            listNode.Nodes.Add(ParseNextInList());
            
            _sourceFile.MoveToNonWhiteSpaceCharacter();

            if (_sourceFile.EndOfFile)
            {
                Report.Error("Expected another item or a closing parenthesis.", Location.New(_sourceFile));
            }
            
            if (_sourceFile.Current is ')') break;
        }
        
        if (_sourceFile.Current is not ')')
        {
            Report.Error("Expected a closing parenthesis.", Location.New(_sourceFile));
        }

        listNode.Location.End = _sourceFile.CurrentIndex;
        
        return listNode;
    }

    private IdentifierNode ParseIdentifierNode()
    {
        var location = Location.New(_sourceFile);
        
        if (char.IsWhiteSpace(_sourceFile.Current))
        {
            Report.Error("Expected something other than whitespace.", location);
        }
        
        var identifier = string.Empty;

        while (!_sourceFile.EndOfFile)
        {
            identifier += _sourceFile.Current;

            if (char.IsWhiteSpace(_sourceFile.Peek) || _sourceFile.Peek is '(' or ')') break;
            
            _sourceFile.MoveNext();
        }

        return new IdentifierNode
        {
            Location = location,
            Text = identifier
        };
    }
    
    private SymbolNode ParseSymbolNode()
    {
        if (_sourceFile.Current is not '\'')
        {
            Report.Error("Expected a symbol.", Location.New(_sourceFile));
        }
        
        _sourceFile.MoveNext();

        if (char.IsWhiteSpace(_sourceFile.Current))
        {
            Report.Error("Symbols should start right after their declaration.", Location.New(_sourceFile));
        }
        
        var location = Location.New(_sourceFile);
        var symbol = string.Empty;

        while (!_sourceFile.EndOfFile)
        {
            symbol += _sourceFile.Current;

            if (char.IsWhiteSpace(_sourceFile.Peek) || _sourceFile.Peek is '(' or ')') break;
            
            _sourceFile.MoveNext();
        }

        return new SymbolNode
        {
            Location = location,
            Text = symbol
        };
    }
}