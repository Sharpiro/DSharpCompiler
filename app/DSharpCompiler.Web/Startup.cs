﻿using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.DSharp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace DSharpCompiler.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ITokenDefinitions, DSharpTokenDefinitions>();
            services.AddTransient<LexicalAnalyzer, LexicalAnalyzer>();
            services.AddTransient<ITokenParser, DSharpParser>();
            services.AddTransient<NodeVisitor, NodeVisitor>();
            services.AddTransient<Interpreter, Interpreter>();
            services.AddTransient<TypesTable, TypesTable>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc(options => options.MapRoute("defaultApi", "api/{controller}/{action}/{id?}"));
            app.UseFileServer();
        }

        public static void Main(string[] args)
        {
            new WebHostBuilder().UseKestrel().UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration().UseStartup<Startup>().Build().Run();
        }
    }
}