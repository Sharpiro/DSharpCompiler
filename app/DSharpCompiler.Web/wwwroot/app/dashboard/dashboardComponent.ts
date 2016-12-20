import {Component, Inject} from "@angular/core"
import {Observable} from "rxjs/Rx"
import {ICompilerService} from "../compiler/compiler"
import {CustomPipe, LowerCasePipe} from "../blocks/blocks"

@Component({
    selector: "my-app",
    templateUrl: "./app/dashboard/dashboardComponent.html",
    styleUrls: ["./app/dashboard/dashboardComponent.css"],
    pipes: [CustomPipe, LowerCasePipe]
})
export class DashboardComponent
{
    private defaultInput =
`type functions
{
    func int add(int x, int y)
    {
        return x + y;
    };

    func int fib(int n)
    {
        if (n eq 0)
        {
            return 0;
        };
        if (n eq 1)
        {
            return 1;
        };
        return functions.fib(n - 2) + functions.fib(n - 1);
    };
};
type moreFunctions
{
    func int subtract(int x, int y)
    {
        return x - y;
    };
};
let b = functions.add(2, 6);
let c = moreFunctions.subtract(2, 6);
let d = b - c;
let e = functions.fib(18);
dConsole.printInt(b);
dConsole.printInt(c);
dConsole.printInt(d);
dConsole.printInt(e);`;
    private input = this.defaultInput;
    private output: Observable<any>;

    constructor( @Inject("ICompilerServiceToken") private dataService: ICompilerService)
    {
    }

    private compile(): void
    {
        this.output = this.dataService.compileDSharp(this.input);
    }

    private reset(): void
    {
        this.input = this.defaultInput;
        this.output = undefined;
    }
}