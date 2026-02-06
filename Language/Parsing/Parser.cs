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
            
            list.Add(ParseList());
            
            _sourceFile.MoveToNonWhiteSpaceCharacter();
        }

        return list;
    }

    private Node ParseLispOrSymbolOrStructNode()
    {
        return _sourceFile.Current switch
        {
            '(' or '\'' => ParseList(),
            _ => ParseSymbol()
        };
    }
    
    private ListNode ParseList()
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
            Report.Error("Expected a lisp.", Location.New(_sourceFile));
        }
        
        _sourceFile.MoveToNonWhiteSpaceCharacter();
        
        while (!_sourceFile.EndOfFile)
        {
            listNode.Nodes.Add(ParseLispOrSymbolOrStructNode());
            
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

    private IdentifierNode ParseSymbol()
    {
        var symbol = string.Empty;
        var location = Location.New(_sourceFile);

        while (!_sourceFile.EndOfFile)
        {
            symbol += _sourceFile.Current;

            if (char.IsWhiteSpace(_sourceFile.Peek) || _sourceFile.Peek is '(' or ')') break;
            
            _sourceFile.MoveNext();
        }

        return new IdentifierNode
        {
            Location = location,
            Text = symbol
        };
    }
}