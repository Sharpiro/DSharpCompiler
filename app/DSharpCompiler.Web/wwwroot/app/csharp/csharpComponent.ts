import {Component, Inject} from "@angular/core"
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
`int Add(int x, int y)
{
    var temp = 2;
    return x + y;
}
var result = Add(2, 3);
var temp = 3;`;
    private input = this.defaultInput;
    private output: Observable<any>;

    constructor( @Inject("ICompilerServiceToken") private dataService: ICompilerService)
    {
    }

    private compile(): void
    {
        this.output = this.dataService.compileCSharp(this.input);
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