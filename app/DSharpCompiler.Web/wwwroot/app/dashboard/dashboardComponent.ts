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
`func doWork
{
    return 2 * 2 + - 3;
};
func doMoreWork
{
    return 2 / (2 + - 4);
};
let a = doWork;
let b = doMoreWork;
let c = doWork + doMoreWork;`;
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