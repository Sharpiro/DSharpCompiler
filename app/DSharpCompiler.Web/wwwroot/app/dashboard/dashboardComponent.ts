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
`func int fib(int n)
{
    if (n eq 0)
    {
        return 0;
    };
    if (n eq 1)
    {
        return 1;
    };
    return fib(n - 2) + fib(n - 1);
};
let b = fib(18);`;
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