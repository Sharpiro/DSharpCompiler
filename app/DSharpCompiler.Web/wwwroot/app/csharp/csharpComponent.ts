﻿import {Component, Inject} from "@angular/core"
import {Observable} from "rxjs/Rx"
import {ICompilerService} from "../compiler/compiler"
import {CustomPipe, LowerCasePipe} from "../blocks/blocks"

@Component({
    selector: "my-app",
    templateUrl: "./app/csharp/csharpComponent.html",
    styleUrls: ["./app/csharp/csharpComponent.css"],
    pipes: [CustomPipe, LowerCasePipe]
})
export class CsharpComponent
{
    private defaultInput =
`type Adder
{
    external func int Add(int x, int y)
    {
        let temp = 2;
        return x + y;
    }
}
let test = System.Exception.New();
let adder = Adder.New();
let result = adder.Add(2, 3);
let xxx = Adder.New().Add(1, 1);`;
    private input = this.defaultInput;
    private output: Observable<any>;

    constructor( @Inject("ICompilerServiceToken") private dataService: ICompilerService)
    {
    }

    private compile(): void
    {
        this.output = this.dataService.compileCSharp(this.input);
    }

    private transpile(): void
    {
        this.output = this.dataService.transpileCSharp(this.input);
    }

    private getTree(): void
    {
        this.output = this.dataService.getSyntaxTree(this.input);
    }

    private reset(): void
    {
        this.input = this.defaultInput;
        this.output = undefined;
    }
}