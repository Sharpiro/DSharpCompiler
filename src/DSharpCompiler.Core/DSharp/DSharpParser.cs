using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCompiler.Core.DSharp
{
    public class DSharpParser : ITokenParser
    {
        private IList<Token> _tokens;
        private Token _currentToken;
        private int _currentIndex;

        public Node Program(IList<Token> tokens)
        {
            _tokens = tokens;
            _currentToken = _tokens.FirstOrDefault();
            _currentIndex = 0;
            var children = StatementList();
            var node = new CompoundNode(children, new List<Node>()) { Name = DsharpConstants.Identifiers.EntryPoint };
            return node;
        }

        private IEnumerable<Node> ParameterList()
        {
            var node = Parameter();

            var nodes = new List<Node> { node };
            while (_currentToken?.Value == DsharpConstants.Symbols.Comma)
            {
                EatToken(TokenType.Symbol);
                nodes.Add(Parameter());
            }
            return nodes;
        }

        private Node Parameter()
        {
            Node node = null;
            if (_currentToken.Type == TokenType.Identifier || _currentToken.Type == TokenType.NumericConstant
                || _currentToken.Type == TokenType.StringConstant)
                node = Expression();
            else
                node = new EmptyNode();
            return node;
        }

        private IEnumerable<Node> StatementList()
        {
            var node = Statement();

            var nodes = new List<Node> { node };
            while (_currentToken?.Value == DsharpConstants.Symbols.SemiColan)
            {
                EatToken(TokenType.Symbol);
                nodes.Add(Statement());
            }
            return nodes;
        }

        private Node Statement()
        {
            Node node = null;
            if (_currentToken?.Value == DsharpConstants.Keywords.Func)
            {
                node = CompoundStatement();
            }
            else if (_currentToken?.Value == DsharpConstants.Keywords.Return)
            {
                node = ReturnStatement();
            }
            else if (_currentToken?.Value == DsharpConstants.Keywords.Let)
            {
                node = AssignmentStatement();
            }
            else if (_currentToken?.Value == DsharpConstants.Keywords.If)
            {
                node = ConditionalStatement();
            }
            else if (_currentToken?.Type == TokenType.Identifier)
            {
                node = RoutineStatement();
            }
            else
                node = Empty();
            return node;
        }

        private Node CompoundStatement()
        {
            EatToken(TokenType.Keyword);
            var functionName = _currentToken.Value;
            EatToken(TokenType.Identifier);

            EatToken(TokenType.Symbol);
            var parameters = ParameterList();
            EatToken(TokenType.Symbol);

            EatToken(TokenType.Symbol);
            var statements = StatementList();
            var node = new CompoundNode(statements, parameters) { Name = functionName };
            EatToken(TokenType.Symbol);
            return node;
        }

        private Node ConditionalStatement()
        {
            EatToken(TokenType.Keyword);
            EatToken(TokenType.Symbol);

            //condition
            //var condition = Condition();
            var expressionOne = Expression();
            EatToken(TokenType.Keyword);
            var expressionTwo = Expression();

            EatToken(TokenType.Symbol);
            EatToken(TokenType.Symbol);
            var statements = StatementList();
            EatToken(TokenType.Symbol);

            var node = new CompoundNode(statements, new List<Node> { expressionOne, expressionTwo })
            { Type = NodeType.Conditional };
            return node;
        }

        //private Node Condition()
        //{
        //    Node node = null;
        //    if (_currentToken.Type == TokenType.Keyword)
        //    {
        //        var condition = _currentToken.Value;
        //        EatToken(TokenType.Keyword);
        //        node = null;
        //    }
        //    else
        //        throw new InvalidOperationException("Invalid condition provided");
        //    var expressionOne = Expression();
        //    EatToken(TokenType.Symbol);
        //    var expressionTwo = Expression();
        //    return node;
        //}

        private Node AssignmentStatement()
        {
            EatToken(TokenType.Keyword);
            var variable = Variable();
            var token = _currentToken;
            EatToken(TokenType.Symbol);
            var node = new BinaryNode(variable, token, Expression()) { Type = NodeType.Assignment };
            return node;
        }

        private Node ReturnStatement()
        {
            var token = _currentToken;
            EatToken(TokenType.Keyword);
            var node = new UnaryNode(token, Expression()) { Type = NodeType.Return };
            return node;
        }

        private Node RoutineStatement()
        {
            var token = _currentToken;
            EatToken(TokenType.Identifier);

            EatToken(TokenType.Symbol);
            var arguments = ParameterList();
            EatToken(TokenType.Symbol);

            var node = new RoutineNode(token.Value, arguments);
            return node;
        }

        private Node Empty()
        {
            var node = new EmptyNode();
            return node;
        }

        private Node Expression()
        {
            var node = Term();

            while (_currentToken != null && _currentToken.Value.In(DsharpConstants.Symbols.Plus
                , DsharpConstants.Symbols.Minus))
            {
                var token = _currentToken;
                EatToken(TokenType.Symbol);
                node = new BinaryNode(node, token, Term());
            }
            return node;
        }

        private Node Term()
        {
            var node = Factor();
            while (_currentToken != null && _currentToken.Value.In(DsharpConstants.Symbols.Multiply
                , DsharpConstants.Symbols.Divide))
            {
                var token = _currentToken;
                EatToken(TokenType.Symbol);
                node = new BinaryNode(node, token, Factor());
            }
            return node;
        }

        private Node Factor()
        {
            var token = _currentToken;
            Node node = null;
            if (token.Type == TokenType.NumericConstant)
            {
                EatToken(TokenType.NumericConstant);
                node = new NumericNode(token);
            }
            else if (token.Type == TokenType.StringConstant)
            {
                EatToken(TokenType.StringConstant);
                node = new StringNode(token);
            }
            else if (token.Type == TokenType.Keyword)
            {
                EatToken(TokenType.Keyword);
                node = new BooleanNode(token);
            }
            else if (token.Value.In(DsharpConstants.Symbols.Plus, DsharpConstants.Symbols.Minus))
            {
                EatToken(TokenType.Symbol);
                node = new UnaryNode(token, Factor());
            }
            else if (token.Type == TokenType.Identifier &&
                PeekToken()?.Value == DsharpConstants.Symbols.LeftParenthesis)
            {
                node = RoutineStatement();
            }
            else if (token.Value == DsharpConstants.Symbols.LeftParenthesis)
            {
                EatToken(TokenType.Symbol);
                node = Expression();
                EatToken(TokenType.Symbol);
            }
            else
                node = Variable();
            return node;
        }

        private Node Variable()
        {
            var node = new VariableNode(_currentToken);
            EatToken(TokenType.Identifier);
            return node;
        }

        private void EatToken(TokenType type)
        {
            if (_currentToken == null || _currentToken.Type != type)
                throw new IndexOutOfRangeException();
            _currentToken = NextToken();
        }

        private Token NextToken()
        {
            try
            {
                _currentIndex++;
                return _tokens[_currentIndex];
            }
            catch (ArgumentOutOfRangeException) { }
            return null;
        }

        private Token PeekToken()
        {
            try
            {
                return _tokens[_currentIndex + 1];
            }
            catch (ArgumentOutOfRangeException) { }
            return null;
        }
    }
}