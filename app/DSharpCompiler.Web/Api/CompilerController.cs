using DSharpCompiler.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var dictionary = _interpreter.Interpret(code).Where(pair => pair.Value.GetType() == typeof(int)).ToDictionary(pair => pair.Key, y => y.Value);
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
            var trimmed = dictionary
                .Where(pair => pair.Value.Value.GetType() == typeof(int) || pair.Value.Value.GetType() == typeof(string))
                .Select(pair => new KeyValuePair<string, object>(pair.Key, pair.Value.Value))
                .ToDictionary(pair => pair.Key, y => y.Value);
            var response = new { Data = new { Output = trimmed } };
            return response;
        }
    }
}