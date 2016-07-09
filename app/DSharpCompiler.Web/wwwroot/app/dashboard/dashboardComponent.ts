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
    `func main
    {
        c = 12;
        func action
        {
            number = 5 - - - + - (3 + 4 * 2) - +2;
        };

        func actionTwo
        {
            b = 10 * number;
        };
    };`
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