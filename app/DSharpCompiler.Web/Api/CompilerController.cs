using DSharpCompiler.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}