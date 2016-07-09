using DSharpCompiler.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;

namespace DSharpCompiler.Web.Api
{
    public class CompilerController
    {
        private readonly Interpreter _interpreter;

        public CompilerController(Interpreter interpreter)
        {
            _interpreter = interpreter;
        }

        [HttpGet]
        public string Get()
        {
            return "test string";
        }

        [HttpPost]
        public object CompilePascal([FromBody]object postData)
        {
            if (postData == null)
                throw new ArgumentNullException(nameof(postData));
            var code = JObject.FromObject(postData).SelectToken("source").Value<string>();
            var dictionary = _interpreter.Interpret(code);
            var response = new { Data = new { Output = dictionary } };
            return response;
        }

        [HttpPost]
        public object CompileDSharp([FromBody]object postData)
        {
            if (postData == null)
                throw new ArgumentNullException(nameof(postData));
            var code = JObject.FromObject(postData).SelectToken("source").Value<string>();
            var dictionary = _interpreter.Interpret(code);
            var response = new { Data = new { Output = dictionary } };
            return response;
        }
    }
}