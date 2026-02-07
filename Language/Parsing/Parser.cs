using Turbo.Language.Diagnostics;
using Turbo.Language.Parsing.Nodes;

namespace Turbo.Language.Parsing;

public class Parser
{
    private readonly SourceFile _sourceFile;
    
    public Parser(SourceFile sourceFile)
    {
        _sourceFile = sourceFile;
    }

    public List<Node> ParseFile()
    {
        _sourceFile.MoveNext();
        
        var list = new List<Node>();

        while (!_sourceFile.EndOfFile)
        {
            if (char.IsWhiteSpace(_sourceFile.Current))
            {
                _sourceFile.MoveToNonWhiteSpaceCharacter(); 
                if (_sourceFile.EndOfFile) break;
            }

            if (_sourceFile.Current is ';')
            {
                list.Add(ParseSingleLineComment());
                continue;
            }
            
            list.Add(ParseList());
            
            _sourceFile.MoveToNonWhiteSpaceCharacter();
        }

        return list;
    }

    private SingleLineCommentNode ParseSingleLineComment()
    {
        var location = Location.New(_sourceFile);
        
        if (_sourceFile.Current is not ';')
        {
            Report.Error("Expected a single line comment.", location);
        }

        var comment = string.Empty;
        
        while (!_sourceFile.EndOfFile)
        {
            comment += _sourceFile.Current;
            
            _sourceFile.MoveNextAndRespectNewLines();
            
            if (_sourceFile.IsNewLine) break;
        }

        location.End = _sourceFile.Current;

        return new SingleLineCommentNode
        {
            Location = location,
            Text = comment
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
            Report.Error("Expected a list.", Location.New(_sourceFile));
        }
        
        _sourceFile.MoveToNonWhiteSpaceCharacter();

        if (_sourceFile.Current is not ')')
        {
            while (!_sourceFile.EndOfFile)
            {
                listNode.Nodes.Add(_sourceFile.Current switch
                {
                    '(' => ParseList(),
                    '\'' when _sourceFile.Peek is '(' => ParseList(),
                    '\'' => ParseSymbol(),
                    '"' => ParseStringLiteral(),
                    ';' => ParseSingleLineComment(),
                    var c when char.IsNumber(c) || c is '.' => ParseNumber(),
                    _ => ParseIdentifier()
                });
            
                _sourceFile.MoveToNonWhiteSpaceCharacter();

                if (_sourceFile.EndOfFile)
                {
                    Report.Error("Expected another item or a closing parenthesis.", Location.New(_sourceFile));
                }
            
                if (_sourceFile.Current is ')') break;
            }
        }
        
        if (_sourceFile.Current is not ')')
        {
            Report.Error("Expected a closing parenthesis.", Location.New(_sourceFile));
        }

        listNode.Location.End = _sourceFile.CurrentIndex;
        
        return listNode;
    }

    private NumberLiteralNode ParseNumber()
    {
        var location = Location.New(_sourceFile);
        
        if (!char.IsNumber(_sourceFile.Current) && _sourceFile.Current is not '.')
        {
            Report.Error("Expected number literal.", location);
        }

        var number = string.Empty;
        var hasDecimalPoint = false;

        while (!_sourceFile.EndOfFile)
        {
            if (!char.IsNumber(_sourceFile.Current) && _sourceFile.Current is not '.' && _sourceFile.Current is not ',')
            {
                Report.Error("This cannot be part of the number.", Location.New(_sourceFile));
                break;
            }

            switch (_sourceFile.Current)
            {
                case '.' when !hasDecimalPoint:
                    hasDecimalPoint = true;
                    break;
                case '.' when hasDecimalPoint:
                    Report.Error("The number should only have one decimal point.", Location.New(_sourceFile));
                    break;
            }
            
            number += _sourceFile.Current;
            
            if (char.IsWhiteSpace(_sourceFile.Peek) || _sourceFile.Peek is ')') break;
            
            _sourceFile.MoveNext();
        }

        return new NumberLiteralNode
        {
            Location = location,
            Text = number.Replace(",", string.Empty)
        };
    }

    private StringLiteralNode ParseStringLiteral()
    {
        var location = Location.New(_sourceFile);
        
        if (_sourceFile.Current is not '"')
        {
            Report.Error("String literal expected.", location);
        }
        
        _sourceFile.MoveNext();
        
        var stringLiteral = string.Empty;
        
        while (!_sourceFile.EndOfFile)
        {
            if (_sourceFile.Current is '"') break;

            stringLiteral += _sourceFile.Current;
            
            _sourceFile.MoveNext();
        }

        if (_sourceFile.Current is not '"')
        {
            Report.Error("String literals should end with a quotation mark.", location);
        }
        
        location.End = _sourceFile.CurrentIndex;

        return new StringLiteralNode
        {
            Location = location,
            Text = stringLiteral
        };
    }
    
    private IdentifierNode ParseIdentifier()
    {
        var location = Location.New(_sourceFile);
        
        if (char.IsWhiteSpace(_sourceFile.Current))
        {
            Report.Error("Expected something other than whitespace.", location);
        }
        
        var identifier = string.Empty;

        while (!_sourceFile.EndOfFile)
        {
            if (_sourceFile.Current is '(')
            {
                Report.Error("This cannot be part of the identifier.", Location.New(_sourceFile));
                break;
            }
            
            identifier += _sourceFile.Current;

            if (char.IsWhiteSpace(_sourceFile.Peek) || _sourceFile.Peek is ')') break;
            
            _sourceFile.MoveNext();
        }

        location.End = _sourceFile.CurrentIndex;
        
        return new IdentifierNode
        {
            Location = location,
            Text = identifier
        };
    }
    
    private SymbolNode ParseSymbol()
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
            if (_sourceFile.Current is '(')
            {
                Report.Error("This cannot be part of the symbol.", Location.New(_sourceFile));
                break;
            }

            symbol += _sourceFile.Current;

            if (char.IsWhiteSpace(_sourceFile.Peek) || _sourceFile.Peek is ')') break;
            
            _sourceFile.MoveNext();
        }
        
        location.End = _sourceFile.CurrentIndex;

        return new SymbolNode
        {
            Location = location,
            Text = symbol
        };
    }
}