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
`let a = 2;
func doWork()
{
    let b = "data";
    let c = a * (2 + - 3);
    return c;
};
let d = doWork();
let a = 5;
let c = doWork() + 1;
func add(e, f)
{
    return e + f;
};
let e = add(2, 4);`;
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