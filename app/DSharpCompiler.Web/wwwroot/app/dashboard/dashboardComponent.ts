import {Component, OnInit, Inject, EventEmitter, Output, AfterViewInit, ChangeDetectorRef} from "@angular/core"
import {Observable} from "rxjs/Rx"
import {IVehicleService} from "../vehicles/vehicles"
import {CustomPipe, NestedComponent, LowerCasePipe} from "../blocks/blocks"

@Component({
    selector: "my-app",
    templateUrl: "./app/dashboard/dashboardComponent.html",
    styleUrls: ["./app/dashboard/dashboardComponent.css"],
    directives: [NestedComponent],
    pipes: [CustomPipe, LowerCasePipe],
})
export class DashboardComponent implements OnInit
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

    constructor( @Inject("IVehicleServiceToken") private dataService: IVehicleService, private cdrService: ChangeDetectorRef)
    {
    }

    public ngOnInit(): void
    {
        //this.output = this.dataService.compile(this.input);
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