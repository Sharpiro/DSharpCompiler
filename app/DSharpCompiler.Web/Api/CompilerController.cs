using DSharpCodeAnalysis.Parser;
using DSharpCompiler.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSharpCompiler.Web.Api
{
    public class CompilerController : Controller
    {
        private readonly Interpreter _interpreter;

        public CompilerController(Interpreter interpreter)
        {
            _interpreter = interpreter;
        }

        [HttpPost]
        public object CompilePascal([FromBody]object postData)
        {
            if (postData == null)
                throw new ArgumentNullException(nameof(postData));
            var code = JObject.FromObject(postData).SelectToken("source").Value<string>();
            var dictionary = _interpreter.Interpret(code).SymbolsTable.Where(pair => pair.Value.GetType() == typeof(int)).ToDictionary(pair => pair.Key, y => y.Value);
            var response = new { Data = new { Output = dictionary } };
            return response;
        }

        [HttpPost]
        public object CompileDSharp([FromBody]object postData)
        {
            try
            {
                if (postData == null)
                    throw new ArgumentNullException(nameof(postData));
                var code = JObject.FromObject(postData).SelectToken("source").Value<string>();
                var trimmed = _interpreter.Interpret(code).ConsoleOutput;
                var response = new { Data = new { Output = trimmed } };
                return response;
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                return ex.Message;
            }
        }

        [HttpPost]
        public async Task<object> CompileCSharp([FromBody]object postData)
        {
            if (postData == null)
                throw new ArgumentNullException(nameof(postData));
            var code = JObject.FromObject(postData).SelectToken("source").Value<string>();
            var lexer = new DLexer(code);
            var tokens = lexer.Lex();
            var parser = new DParser(tokens);
            var compilation = parser.ParseCompilationUnit();
            var stringResult = compilation.ToString();

            var results = await CSharpScript.RunAsync(stringResult);
            var variables = results.Variables.ToDictionary(v => v.Name, v => v.Value);
            var response = new { Data = new { Output = variables } };
            return response;
        }

        [HttpPost]
        public object GetSyntaxTree([FromBody]object postData)
        {
            if (postData == null)
                throw new ArgumentNullException(nameof(postData));
            var code = JObject.FromObject(postData).SelectToken("source").Value<string>();
            var lexer = new DLexer(code);
            var tokens = lexer.Lex();
            var parser = new DParser(tokens);
            var compilation = parser.ParseCompilationUnit();
            var hierarchy = compilation.DescendantHierarchy();
            var json = JsonConvert.SerializeObject(hierarchy, Formatting.Indented);
            var response = new { Data = new { Output = json } };
            return response;
        }
    }
}