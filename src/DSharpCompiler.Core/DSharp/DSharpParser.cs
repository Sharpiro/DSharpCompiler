using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.Common.Exceptions;
using DSharpCompiler.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DSharpCompiler.Core.DSharp
{
    public class DSharpParser : ITokenParser
    {
        private readonly TypesTable _typesTable;
        private IList<Token> _tokens;
        private Token _currentToken;
        private int _currentIndex;

        public DSharpParser(TypesTable typesTable)
        {
            _typesTable = typesTable;
        }

        public Node Program(IList<Token> tokens)
        {
            _tokens = tokens;
            _currentToken = _tokens.FirstOrDefault();
            _currentIndex = 0;
            var node = new CompoundNode(Enumerable.Empty<Node>(), new List<Node>())
            { Name = DsharpConstants.Identifiers.EntryPoint };
            var children = StatementList(node);
            node.Children = children;
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

        private IEnumerable<Node> ArgumentList(Type parentType)
        {
            var node = Argument(parentType);

            var nodes = new List<Node> { node };
            while (_currentToken?.Value == DsharpConstants.Symbols.Comma)
            {
                EatToken(TokenType.Symbol);
                nodes.Add(Argument(parentType));
            }
            return nodes;
        }

        private Node Argument(Type parentType)
        {
            Node node = null;
            if (_currentToken.Type == TokenType.Identifier || _currentToken.Type == TokenType.NumericConstant
                || _currentToken.Type == TokenType.StringConstant)
            {
                node = Expression(parentType);
            }
            else
                node = new EmptyNode();
            return node;
        }

        private Node Parameter()
        {
            Node node = null;
            if (_currentToken.Type == TokenType.Identifier || _currentToken.Type == TokenType.NumericConstant
                || _currentToken.Type == TokenType.StringConstant)
            {
                var variableType = _typesTable[_currentToken.Value];
                if (variableType == null)
                    throw new ArgumentException($"The type: {_currentToken.Value} does not exist in the current context");

                EatToken(TokenType.Identifier);
                var variableNode = Variable(variableType);
                variableNode.ValueType = variableType;
                node = variableNode;
            }
            else
                node = new EmptyNode();
            return node;
        }

        private IEnumerable<Node> StatementList(CompoundNode parentNode)
        {
            var node = Statement(parentNode);

            var nodes = new List<Node> { node };
            while (_currentToken?.Value == DsharpConstants.Symbols.SemiColan)
            {
                EatToken(TokenType.Symbol);
                nodes.Add(Statement(parentNode));
            }
            return nodes;
        }

        private Node Statement(CompoundNode parentNode)
        {
            Node node = null;
            if (_currentToken?.Value == DsharpConstants.Keywords.Func)
            {
                var compoundNode = CompoundStatement();
                node = compoundNode;
            }
            else if (_currentToken?.Value == DsharpConstants.Keywords.Return)
            {
                node = ReturnStatement(parentNode);
            }
            else if (_currentToken?.Value == DsharpConstants.Keywords.Let)
            {
                node = AssignmentStatement(parentNode.ValueType);
            }
            else if (_currentToken?.Value == DsharpConstants.Keywords.If)
            {
                node = ConditionalStatement(parentNode);
            }
            else if (_currentToken?.Type == TokenType.Identifier)
            {
                node = RoutineStatement();
            }
            else
                node = Empty();
            return node;
        }

        private CompoundNode CompoundStatement()
        {
            EatToken(TokenType.Keyword);
            var functionReturnType = _typesTable[_currentToken.Value];
            if (functionReturnType == null)
                throw new ArgumentException($"The type: {functionReturnType} does not exist in the current context");

            EatToken(TokenType.Identifier);
            var functionName = _currentToken.Value;
            EatToken(TokenType.Identifier);

            EatToken(TokenType.Symbol);
            var parameters = ParameterList();
            EatToken(TokenType.Symbol);
            EatToken(TokenType.Symbol);

            var node = new CompoundNode(Enumerable.Empty<Node>(), parameters) { Name = functionName, ValueType = functionReturnType };
            _typesTable.Add(node.Name, functionReturnType);
            var statements = StatementList(node);

            var returnType = GetReturnType(statements);
            if (returnType != functionReturnType)
                throw new TypeMismatchException($"Expected return type: {functionReturnType.Name}, but was actually: {returnType.Name}");

            node.Children = statements;
            EatToken(TokenType.Symbol);
            return node;
        }

        private Node ConditionalStatement(CompoundNode parentNode)
        {
            EatToken(TokenType.Keyword);
            EatToken(TokenType.Symbol);

            var expressionOne = Expression(parentNode.ValueType);
            EatToken(TokenType.Keyword);
            var expressionTwo = Expression(parentNode.ValueType);

            EatToken(TokenType.Symbol);
            EatToken(TokenType.Symbol);
            var node = new CompoundNode(Enumerable.Empty<Node>(), new List<Node> { expressionOne, expressionTwo })
            {
                NodeType = NodeType.Conditional,
                ValueType = parentNode.ValueType
            };
            var statements = StatementList(node);
            EatToken(TokenType.Symbol);

            var returnType = GetReturnType(statements);
            if (returnType != typeof(void) && returnType != parentNode.ValueType)
                throw new TypeMismatchException($"Expected return type: {parentNode.ValueType.Name}, but was actually: {returnType.Name}");

            node.Children = statements;
            return node;
        }

        private Node AssignmentStatement(Type parentType)
        {
            EatToken(TokenType.Keyword);
            var variable = Variable(typeof(void));
            var token = _currentToken;
            EatToken(TokenType.Symbol);
            var node = new BinaryNode(variable, token, Expression(parentType)) { NodeType = NodeType.Assignment };
            return node;
        }

        private Node ReturnStatement(CompoundNode compoundNode)
        {
            var token = _currentToken;
            EatToken(TokenType.Keyword);
            var expression = Expression(compoundNode.ValueType);
            if (expression.ValueType == null)
                expression.ValueType = compoundNode.ValueType;
            if (expression.ValueType != compoundNode.ValueType)
                throw new TypeMismatchException($"Expected return type: {compoundNode.ValueType.Name}, but was actually: {expression.ValueType}");

            var node = new UnaryNode(token, expression)
            {
                NodeType = NodeType.Return,
                ValueType = compoundNode.ValueType
            };
            return node;
        }

        private Node RoutineStatement()
        {
            string routineName;
            var returnType = GetRoutineType(out routineName);

            EatToken(TokenType.Symbol);
            var arguments = ArgumentList(returnType);
            EatToken(TokenType.Symbol);

            var node = new RoutineNode(routineName, arguments) { ValueType = returnType };
            return node;
        }

        private Type GetRoutineType(out string routineName)
        {
            routineName = _currentToken.Value;
            EatToken(TokenType.Identifier);

            var returnType = _typesTable[routineName];
            if (returnType == null)
                throw new TypeNotFoundException($"Could not find type: {returnType}");

            if (_currentToken.Value != ".")
                return returnType;

            EatToken(TokenType.Symbol);
            var subRoutine = _currentToken.Value;
            EatToken(TokenType.Identifier);

            routineName = $"{routineName}.{subRoutine}";
            returnType = returnType.GetRuntimeMethods()
                .Single(m => m.Name.Equals(subRoutine, StringComparison.OrdinalIgnoreCase)).ReturnType;

            return returnType;
        }

        private Node Empty()
        {
            var node = new EmptyNode();
            return node;
        }

        private Node Expression(Type parentType)
        {
            var node = Term(parentType);

            while (_currentToken != null && _currentToken.Value.In(DsharpConstants.Symbols.Plus
                , DsharpConstants.Symbols.Minus))
            {
                var token = _currentToken;
                EatToken(TokenType.Symbol);
                var x = Term(parentType);
                if (x.ValueType != node.ValueType)
                    throw new TypeMismatchException($"Expected return type: {x.ValueType.Name}, but was actually: {node.ValueType}");
                node = new BinaryNode(node, token, x) { ValueType = parentType };
            }
            return node;
        }

        private Node Term(Type parentType)
        {
            var node = Factor(parentType);
            while (_currentToken != null && _currentToken.Value.In(DsharpConstants.Symbols.Multiply
                , DsharpConstants.Symbols.Divide))
            {
                var token = _currentToken;
                EatToken(TokenType.Symbol);
                node = new BinaryNode(node, token, Factor(parentType)) { ValueType = parentType };
            }
            return node;
        }

        private Node Factor(Type parentType)
        {
            var token = _currentToken;
            Node node = null;
            if (token.Type == TokenType.NumericConstant)
            {
                EatToken(TokenType.NumericConstant);
                node = new VariableNode(token, NodeType.Numeric) { ValueType = typeof(int) };
            }
            else if (token.Type == TokenType.StringConstant)
            {
                EatToken(TokenType.StringConstant);
                node = new VariableNode(token, NodeType.String) { ValueType = typeof(string) };
            }
            else if (token.Type == TokenType.Keyword)
            {
                EatToken(TokenType.Keyword);
                node = new VariableNode(token, NodeType.Variable);
            }
            else if (token.Value.In(DsharpConstants.Symbols.Plus, DsharpConstants.Symbols.Minus))
            {
                EatToken(TokenType.Symbol);
                node = new UnaryNode(token, Factor(parentType)) { ValueType = parentType };
            }
            else if (IsRoutineNode(token))
            {
                node = RoutineStatement();
            }
            else if (token.Value == DsharpConstants.Symbols.LeftParenthesis)
            {
                EatToken(TokenType.Symbol);
                node = Expression(parentType);
                EatToken(TokenType.Symbol);
            }
            else
                node = Variable(parentType);

            return node;
        }

        private VariableNode Variable(Type variableType)
        {
            var node = new VariableNode(_currentToken, NodeType.Variable)
            {
                ValueType = variableType
            };
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

        private Token PeekToken(int peekAmount = 1)
        {
            try
            {
                return _tokens[_currentIndex + peekAmount];
            }
            catch (ArgumentOutOfRangeException) { }
            return null;
        }

        private bool IsRoutineNode(Token currentToken)
        {
            if (currentToken.Type != TokenType.Identifier) return false;

            int i;
            for (i = 1; (currentToken = PeekToken(i))?.Value == DsharpConstants.Symbols.Period
                || currentToken?.Type == TokenType.Identifier; i++)
            {

            }
            return PeekToken(i)?.Value == DsharpConstants.Symbols.LeftParenthesis;
        }

        private Type GetReturnType(IEnumerable<Node> nodes)
        {
            var returnType = nodes.SingleOrDefault(s => s.NodeType == NodeType.Return);
            if (returnType != null)
                return returnType.ValueType;

            return typeof(void);
        }
    }
}