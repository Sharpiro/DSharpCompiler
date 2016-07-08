using DSharpCompiler.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace DSharpCompiler.Web.Api
{
    public class CompilerController
    {
        [HttpGet]
        public string Get()
        {
            return "test string";
        }

        [HttpPost]
        public object Compile([FromBody]object postData)
        {
            if (postData == null)
                throw new ArgumentNullException(nameof(postData));
            var source = JObject.FromObject(postData).SelectToken("source").Value<string>();
            var analyzer = new LexicalAnalyzer(source);
            var tokens = analyzer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            var response = new { Data = new { Output = result } };
            return response;
        }
    }
}