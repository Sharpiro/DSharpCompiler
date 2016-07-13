import {Component, Inject} from "@angular/core"
import {Observable} from "rxjs/Rx"
import {ICompilerService} from "../compiler/compiler"
import {CustomPipe, NestedComponent, LowerCasePipe} from "../blocks/blocks"

@Component({
    selector: "my-app",
    templateUrl: "./app/dashboard/dashboardComponent.html",
    styleUrls: ["./app/dashboard/dashboardComponent.css"],
    directives: [NestedComponent],
    pipes: [CustomPipe, LowerCasePipe],
})
export class DashboardComponent
{
    private defaultInput =
    `let a = 1;
func doWork
{
    let b = 2;
};
func doMoreWork
{
    let c = "hello world";
};
let d = 4;
doWork;
doMoreWork;`;
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